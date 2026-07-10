using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;

namespace ModProject
{
    public enum GameState
    {
        Idle,
        Active,
        Success,
        Failed
    }

    public enum ActionType
    {
        RepairVehicle,
        BulletproofTires,
        ClearPursuers,
        SpeedBoost,
        SpawnEscort,
        BurstTire,
        EngineStall,
        SpawnChaser,
        ReduceTraction,
        NearbyExplosion,
        ForceDetour
    }

    public struct QueuedAction
    {
        public ActionType Type;
        public string SenderName;
        public int GiftId;
        public string GiftName;
        public string RawMessage;
    }

    public struct TrackedEntity
    {
        public Entity Entity;
        public bool IsHostile;
        public int SpawnTime;
    }

    public class Mod : Script
    {
        private const string WsUrl = "ws://127.0.0.1:60080";
        private const int MissionTimeMs = 600000;
        private const float EngineHealthMin = 200f;
        private const float SpeedBoostAmount = 15f;
        private const int TickIntervalMs = 80;

        private GameState _state = GameState.Idle;
        private Vehicle _missionVehicle;
        private Blip _waypointBlip;
        private Vector3 _destination;
        private Vector3 _detourPoint;
        private bool _detourActive;

        private int _missionStartTime;
        private int _bulletproofUntil;
        private int _engineStallUntil;
        private int _tractionReducedUntil;
        private bool _tiresBulletproof;

        private readonly Queue<QueuedAction> _actionQueue = new Queue<QueuedAction>();
        private readonly object _queueLock = new object();
        private readonly List<TrackedEntity> _trackedEntities = new List<TrackedEntity>();
        private readonly Dictionary<ActionType, int> _cooldowns = new Dictionary<ActionType, int>();
        private readonly JavaScriptSerializer _json = new JavaScriptSerializer();
        private ClientWebSocket _ws;
        private CancellationTokenSource _wsCts;

        private readonly int[] _cooldownDurations = { 15000, 30000, 30000, 15000, 30000, 10000, 20000, 30000, 15000, 20000, 45000 };

        private static readonly Vector3[] Destinations = new Vector3[]
        {
            new Vector3(-1030f, -2750f, 13f),
            new Vector3(1700f, 3300f, 40f),
            new Vector3(-220f, 6300f, 31f),
            new Vector3(900f, -180f, 80f),
        };

        private static readonly Vector3[] DetourPoints = new Vector3[]
        {
            new Vector3(-500f, -1500f, 20f),
            new Vector3(500f, 1000f, 30f),
            new Vector3(-1500f, 200f, 50f),
            new Vector3(1200f, -2000f, 15f),
        };

        private static readonly string[][] PositiveKeywords = new string[][]
        {
            new[] { "repair", "fix", "repaircar", "fixcar", "修车", "修复" },
            new[] { "bulletproof", "shieldtire", "防爆", "护胎" },
            new[] { "clear", "clearpursuer", "清空", "清除追兵", "clearcops" },
            new[] { "boost", "speed", "加速", "nitro", "nos" },
            new[] { "escort", "guard", "护航", "保护", "bodyguard" },
        };

        private static readonly string[][] NegativeKeywords = new string[][]
        {
            new[] { "burst", "pop", "爆胎", "扎胎", "tirepop" },
            new[] { "stall", "engineoff", "熄火", "引擎", "killengine" },
            new[] { "chaser", "pursue", "追击", "追兵", "chase" },
            new[] { "slip", "drift", "打滑", "抓地", "traction", "ice" },
            new[] { "explode", "boom", "爆炸", "炸", "bomb", "grenade" },
            new[] { "detour", "reroute", "绕路", "改道", "绕行" },
        };

        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;
            Interval = TickIntervalMs;
            InitCooldowns();
            _ = ConnectWebSocketAsync();
        }

        private void InitCooldowns()
        {
            var types = (ActionType[])Enum.GetValues(typeof(ActionType));
            for (int i = 0; i < types.Length && i < _cooldownDurations.Length; i++)
            {
                _cooldowns[types[i]] = 0;
            }
        }

        private async Task ConnectWebSocketAsync()
        {
            _wsCts = new CancellationTokenSource();
            try
            {
                _ws = new ClientWebSocket();
                await _ws.ConnectAsync(new Uri(WsUrl), _wsCts.Token);

                var subMsg = "{\"type\":\"serviceSignalSub\",\"service\":\"IM_MESSAGE_TRANSPORT\"}";
                var subBytes = Encoding.UTF8.GetBytes(subMsg);
                await _ws.SendAsync(new ArraySegment<byte>(subBytes), WebSocketMessageType.Text, true, _wsCts.Token);

                await ReceiveLoopAsync(_wsCts.Token);
            }
            catch (Exception)
            {
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken ct)
        {
            var buffer = new byte[8192];
            var sb = new StringBuilder();
            try
            {
                while (!ct.IsCancellationRequested && _ws != null && _ws.State == WebSocketState.Open)
                {
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    if (result.EndOfMessage)
                    {
                        ProcessMessage(sb.ToString());
                        sb.Clear();
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception) { }
        }

        private void ProcessMessage(string raw)
        {
            try
            {
                var dict = _json.Deserialize<Dictionary<string, object>>(raw);
                if (dict == null) return;

                string msgType = null;
                if (dict.ContainsKey("type"))
                    msgType = dict["type"] as string;
                else if (dict.ContainsKey("Type"))
                    msgType = dict["Type"] as string;

                if (msgType == null) return;

                if (msgType == "WebcastChatMessage" || msgType == "chat")
                {
                    ProcessChatMessage(dict, raw);
                }
                else if (msgType == "WebcastGiftMessage" || msgType == "gift")
                {
                    ProcessGiftMessage(dict, raw);
                }
            }
            catch (Exception) { }
        }

        private void ProcessChatMessage(Dictionary<string, object> dict, string raw)
        {
            string content = null;
            string sender = "viewer";

            if (dict.ContainsKey("data") && dict["data"] is Dictionary<string, object> data)
            {
                content = GetStringValue(data, "content") ?? GetStringValue(data, "message") ?? GetStringValue(data, "text");
                if (data.ContainsKey("user") && data["user"] is Dictionary<string, object> userData)
                    sender = GetStringValue(userData, "name") ?? sender;
            }
            else
            {
                content = GetStringValue(dict, "content") ?? GetStringValue(dict, "message") ?? raw;
            }

            if (string.IsNullOrEmpty(content)) return;

            ActionType? action = MatchKeywords(content);
            if (action.HasValue)
            {
                var qa = new QueuedAction
                {
                    Type = action.Value,
                    SenderName = sender,
                    RawMessage = content
                };
                lock (_queueLock) { _actionQueue.Enqueue(qa); }
            }
        }

        private void ProcessGiftMessage(Dictionary<string, object> dict, string raw)
        {
            string giftName = null;
            int giftId = 0;
            string sender = "viewer";

            if (dict.ContainsKey("data") && dict["data"] is Dictionary<string, object> data)
            {
                giftName = GetStringValue(data, "giftName") ?? GetStringValue(data, "name") ?? GetStringValue(data, "gift_name");
                giftId = GetIntValue(data, "giftId", GetIntValue(data, "id", GetIntValue(data, "gift_id", 0)));
                if (data.ContainsKey("user") && data["user"] is Dictionary<string, object> userData)
                    sender = GetStringValue(userData, "name") ?? sender;
            }
            else
            {
                giftName = GetStringValue(dict, "giftName") ?? GetStringValue(dict, "name");
            }

            if (string.IsNullOrEmpty(giftName)) return;

            ActionType? action = MatchKeywords(giftName);
            if (action.HasValue)
            {
                var qa = new QueuedAction
                {
                    Type = action.Value,
                    SenderName = sender,
                    GiftId = giftId,
                    GiftName = giftName,
                    RawMessage = raw
                };
                lock (_queueLock) { _actionQueue.Enqueue(qa); }
            }
        }

        private ActionType? MatchKeywords(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            string lower = text.ToLowerInvariant();

            for (int i = 0; i < PositiveKeywords.Length; i++)
            {
                foreach (string kw in PositiveKeywords[i])
                {
                    if (lower.Contains(kw))
                    {
                        switch (i)
                        {
                            case 0: return ActionType.RepairVehicle;
                            case 1: return ActionType.BulletproofTires;
                            case 2: return ActionType.ClearPursuers;
                            case 3: return ActionType.SpeedBoost;
                            case 4: return ActionType.SpawnEscort;
                        }
                    }
                }
            }

            for (int i = 0; i < NegativeKeywords.Length; i++)
            {
                foreach (string kw in NegativeKeywords[i])
                {
                    if (lower.Contains(kw))
                    {
                        switch (i)
                        {
                            case 0: return ActionType.BurstTire;
                            case 1: return ActionType.EngineStall;
                            case 2: return ActionType.SpawnChaser;
                            case 3: return ActionType.ReduceTraction;
                            case 4: return ActionType.NearbyExplosion;
                            case 5: return ActionType.ForceDetour;
                        }
                    }
                }
            }

            return null;
        }

        private void OnTick(object sender, EventArgs e)
        {
            ProcessQueuedActions();
            UpdateMissionState();
            UpdateTrackedEntities();
            UpdateTimedEffects();
        }

        private void ProcessQueuedActions()
        {
            QueuedAction[] batch = null;
            lock (_queueLock)
            {
                if (_actionQueue.Count == 0) return;
                batch = _actionQueue.ToArray();
                _actionQueue.Clear();
            }

            foreach (var action in batch)
            {
                if (!CanTrigger(action.Type)) continue;
                ApplyCooldown(action.Type);
                ExecuteAction(action);
            }
        }

        private bool CanTrigger(ActionType type)
        {
            int now = Game.GameTime;
            if (_cooldowns.TryGetValue(type, out int cd) && now < cd) return false;
            return true;
        }

        private void ApplyCooldown(ActionType type)
        {
            int durationIdx = (int)type;
            int duration = durationIdx < _cooldownDurations.Length ? _cooldownDurations[durationIdx] : 10000;
            _cooldowns[type] = Game.GameTime + duration;
        }

        private void ExecuteAction(QueuedAction action)
        {
            switch (action.Type)
            {
                case ActionType.RepairVehicle:
                    DoRepairVehicle();
                    break;
                case ActionType.BulletproofTires:
                    DoBulletproofTires();
                    break;
                case ActionType.ClearPursuers:
                    DoClearPursuers();
                    break;
                case ActionType.SpeedBoost:
                    DoSpeedBoost();
                    break;
                case ActionType.SpawnEscort:
                    DoSpawnEscort();
                    break;
                case ActionType.BurstTire:
                    DoBurstTire();
                    break;
                case ActionType.EngineStall:
                    DoEngineStall();
                    break;
                case ActionType.SpawnChaser:
                    DoSpawnChaser();
                    break;
                case ActionType.ReduceTraction:
                    DoReduceTraction();
                    break;
                case ActionType.NearbyExplosion:
                    DoNearbyExplosion();
                    break;
                case ActionType.ForceDetour:
                    DoForceDetour();
                    break;
            }
        }

        private void UpdateMissionState()
        {
            int now = Game.GameTime;
            var player = Game.Player;
            var playerPed = player.Character;

            switch (_state)
            {
                case GameState.Idle:
                    Vehicle currentVeh = playerPed.CurrentVehicle;
                    if (currentVeh != null && currentVeh.Exists() && playerPed.IsInVehicle())
                    {
                        StartMission(currentVeh);
                    }
                    break;

                case GameState.Active:
                    if (_missionVehicle == null || !_missionVehicle.Exists())
                    {
                        EndMission(false);
                        return;
                    }

                    Vehicle playerVeh = playerPed.CurrentVehicle;
                    if (playerVeh != _missionVehicle)
                    {
                        EndMission(false);
                        return;
                    }

                    if (now - _missionStartTime >= MissionTimeMs)
                    {
                        EndMission(false);
                        return;
                    }

                    if (_detourActive)
                    {
                        Vector3 playerPos = _missionVehicle.Position;
                        if (playerPos.DistanceTo(_detourPoint) < 15f)
                        {
                            _detourActive = false;
                            if (_waypointBlip != null && _waypointBlip.Exists())
                                _waypointBlip.Position = _destination;
                        }
                    }

                    Vector3 destCheck = _detourActive ? _detourPoint : _destination;
                    if (_missionVehicle.Position.DistanceTo(destCheck) < 10f && !_detourActive)
                    {
                        EndMission(true);
                    }
                    break;
            }
        }

        private void StartMission(Vehicle vehicle)
        {
            _missionVehicle = vehicle;
            _state = GameState.Active;
            _missionStartTime = Game.GameTime;
            _bulletproofUntil = 0;
            _engineStallUntil = 0;
            _tractionReducedUntil = 0;
            _tiresBulletproof = false;
            _detourActive = false;

            var rng = new Random();
            _destination = Destinations[rng.Next(Destinations.Length)];

            _waypointBlip = _missionVehicle.AddBlip();
            _waypointBlip.Sprite = BlipSprite.Waypoint;
            _waypointBlip.Position = _destination;
            _waypointBlip.Color = BlipColor.Yellow;
            _waypointBlip.Name = "Cargo Destination";

            InitCooldowns();
            lock (_queueLock) { _actionQueue.Clear(); }
            ClearAllTrackedEntities();

            GTA.UI.Screen.ShowSubtitle("~y~CARGO DELIVERY MISSION STARTED~w~~n~Deliver the vehicle in 10 minutes!", 4000);
        }

        private void EndMission(bool success)
        {
            _state = success ? GameState.Success : GameState.Failed;
            ClearAllTrackedEntities();

            if (_waypointBlip != null && _waypointBlip.Exists())
                _waypointBlip.Delete();

            _waypointBlip = null;
            _missionVehicle = null;
            _detourActive = false;
            _tiresBulletproof = false;

            if (success)
                GTA.UI.Screen.ShowSubtitle("~g~MISSION COMPLETE!~w~~n~Cargo delivered successfully!", 4000);
            else
                GTA.UI.Screen.ShowSubtitle("~r~MISSION FAILED!~w~~n~" +
                    (_missionVehicle == null || !_missionVehicle.Exists() ? "Vehicle destroyed!" : "Time's up or vehicle changed!"), 4000);
        }

        private void UpdateTrackedEntities()
        {
            for (int i = _trackedEntities.Count - 1; i >= 0; i--)
            {
                var te = _trackedEntities[i];
                if (te.Entity == null || !te.Entity.Exists())
                {
                    _trackedEntities.RemoveAt(i);
                    continue;
                }

                if (_state == GameState.Active && _missionVehicle != null && _missionVehicle.Exists())
                {
                    if (te.IsHostile)
                    {
                        var ped = te.Entity as Ped;
                        if (ped != null && ped.IsInVehicle() && ped.CurrentVehicle.Exists())
                        {
                            ped.Task.DriveTo(ped.CurrentVehicle, _missionVehicle.Position, 5f, 60f, DrivingStyle.Rushed);
                        }
                    }
                }
            }

            const int maxTracked = 30;
            if (_trackedEntities.Count > maxTracked)
            {
                for (int i = maxTracked; i < _trackedEntities.Count; i++)
                {
                    if (_trackedEntities[i].Entity != null && _trackedEntities[i].Entity.Exists())
                        _trackedEntities[i].Entity.Delete();
                }
                _trackedEntities.RemoveRange(maxTracked, _trackedEntities.Count - maxTracked);
            }
        }

        private void UpdateTimedEffects()
        {
            int now = Game.GameTime;

            if (_tiresBulletproof && now >= _bulletproofUntil)
            {
                _tiresBulletproof = false;
            }

            if (_engineStallUntil > 0 && now >= _engineStallUntil)
            {
                if (_missionVehicle != null && _missionVehicle.Exists())
                    _missionVehicle.IsEngineRunning = true;
                _engineStallUntil = 0;
            }

            if (_tractionReducedUntil > 0)
            {
                if (now >= _tractionReducedUntil)
                {
                    _tractionReducedUntil = 0;
                }
                else if (_missionVehicle != null && _missionVehicle.Exists())
                {
                    if (now % 300 < TickIntervalMs)
                    {
                        var rng = new Random();
                        Vector3 rot = _missionVehicle.Rotation;
                        rot.Z += (float)(rng.NextDouble() - 0.5) * 8f;
                        _missionVehicle.Rotation = rot;

                        float spd = _missionVehicle.Speed;
                        if (spd > 5f)
                            _missionVehicle.Speed = spd * 0.85f;
                    }
                }
            }
        }

        private void DoRepairVehicle()
        {
            if (!EnsureMissionVehicle()) return;
            _missionVehicle.Repair();
            _missionVehicle.EngineHealth = Math.Max(_missionVehicle.EngineHealth, 800f);
            GTA.UI.Screen.ShowSubtitle("~b~[Repair]~w~ Vehicle repaired!", 3000);
        }

        private void DoBulletproofTires()
        {
            if (!EnsureMissionVehicle()) return;
            _tiresBulletproof = true;
            _bulletproofUntil = Game.GameTime + 30000;
            _missionVehicle.Repair();
            GTA.UI.Screen.ShowSubtitle("~b~[Shield]~w~ Tires bulletproof for 30 seconds!", 3000);
        }

        private void DoClearPursuers()
        {
            for (int i = _trackedEntities.Count - 1; i >= 0; i--)
            {
                var te = _trackedEntities[i];
                if (te.IsHostile && te.Entity != null && te.Entity.Exists())
                {
                    te.Entity.Delete();
                    _trackedEntities.RemoveAt(i);
                }
            }
            GTA.UI.Screen.ShowSubtitle("~b~[Clear]~w~ Pursuers eliminated!", 3000);
        }

        private void DoSpeedBoost()
        {
            if (!EnsureMissionVehicle()) return;
            float currentSpeed = _missionVehicle.Speed;
            _missionVehicle.Speed = currentSpeed + SpeedBoostAmount;
            GTA.UI.Screen.ShowSubtitle("~b~[Boost]~w~ Speed boost applied!", 3000);
        }

        private void DoSpawnEscort()
        {
            if (!EnsureMissionVehicle()) return;
            Vector3 spawnPos = _missionVehicle.Position + _missionVehicle.ForwardVector * -15f + Vector3.WorldUp * 2f;
            Model escortModel = new Model(PedHash.Cop01SMY);
            Model vehicleModel = new Model(VehicleHash.Police);

            Vehicle escortVeh = World.CreateVehicle(vehicleModel, spawnPos, _missionVehicle.Heading);
            if (escortVeh == null) return;
            escortVeh.MarkAsNoLongerNeeded();

            Ped escortPed = World.CreatePed(escortModel, spawnPos, _missionVehicle.Heading);
            if (escortPed == null) { escortVeh.Delete(); return; }
            escortPed.MarkAsNoLongerNeeded();
            escortPed.SetIntoVehicle(escortVeh, VehicleSeat.Driver);

            _trackedEntities.Add(new TrackedEntity { Entity = escortVeh, IsHostile = false, SpawnTime = Game.GameTime });
            _trackedEntities.Add(new TrackedEntity { Entity = escortPed, IsHostile = false, SpawnTime = Game.GameTime });

            Vector3 followTarget = _missionVehicle.Position + _missionVehicle.ForwardVector * -8f;
            escortPed.Task.DriveTo(escortVeh, followTarget, 10f, 40f, DrivingStyle.Normal);

            GTA.UI.Screen.ShowSubtitle("~b~[Escort]~w~ Escort vehicle deployed!", 3000);
        }

        private void DoBurstTire()
        {
            if (!EnsureMissionVehicle()) return;
            if (_tiresBulletproof)
            {
                GTA.UI.Screen.ShowSubtitle("~y~[Blocked]~w~ Tires are bulletproof!", 3000);
                return;
            }
            var rng = new Random();
            var wheelBoneIds = new VehicleWheelBoneId[]
            {
                VehicleWheelBoneId.WheelLeftFront,
                VehicleWheelBoneId.WheelRightFront,
                VehicleWheelBoneId.WheelLeftRear,
                VehicleWheelBoneId.WheelRightRear
            };
            _missionVehicle.Wheels[wheelBoneIds[rng.Next(4)]].Burst();
            GTA.UI.Screen.ShowSubtitle("~r~[Sabotage]~w~ Tire burst!", 3000);
        }

        private void DoEngineStall()
        {
            if (!EnsureMissionVehicle()) return;
            _missionVehicle.IsEngineRunning = false;
            _engineStallUntil = Game.GameTime + 3000;
            GTA.UI.Screen.ShowSubtitle("~r~[Stall]~w~ Engine stalled!", 3000);
        }

        private void DoSpawnChaser()
        {
            if (!EnsureMissionVehicle()) return;
            Vector3 spawnPos = _missionVehicle.Position + _missionVehicle.ForwardVector * -40f +
                _missionVehicle.RightVector * (float)(new Random().NextDouble() - 0.5) * 30f + Vector3.WorldUp * 2f;

            Model chaserModel = new Model(PedHash.Cop01SMY);
            Model vehicleModel = new Model(VehicleHash.Schafter2);

            Vehicle chaserVeh = World.CreateVehicle(vehicleModel, spawnPos, _missionVehicle.Heading);
            if (chaserVeh == null) return;
            chaserVeh.MarkAsNoLongerNeeded();

            Ped chaserPed = World.CreatePed(chaserModel, spawnPos, _missionVehicle.Heading);
            if (chaserPed == null) { chaserVeh.Delete(); return; }
            chaserPed.MarkAsNoLongerNeeded();
            chaserPed.SetIntoVehicle(chaserVeh, VehicleSeat.Driver);

            _trackedEntities.Add(new TrackedEntity { Entity = chaserVeh, IsHostile = true, SpawnTime = Game.GameTime });
            _trackedEntities.Add(new TrackedEntity { Entity = chaserPed, IsHostile = true, SpawnTime = Game.GameTime });

            chaserPed.Task.DriveTo(chaserVeh, _missionVehicle.Position, 5f, 55f, DrivingStyle.Rushed);

            GTA.UI.Screen.ShowSubtitle("~r~[Chaser]~w~ Pursuit vehicle incoming!", 3000);
        }

        private void DoReduceTraction()
        {
            if (!EnsureMissionVehicle()) return;
            _tractionReducedUntil = Game.GameTime + 5000;
            GTA.UI.Screen.ShowSubtitle("~r~[Slip]~w~ Reduced traction for 5 seconds!", 3000);
        }

        private void DoNearbyExplosion()
        {
            if (!EnsureMissionVehicle()) return;
            Vector3 explosionPos = _missionVehicle.Position +
                _missionVehicle.RightVector * (float)(new Random().NextDouble() - 0.5) * 12f +
                _missionVehicle.ForwardVector * (float)(new Random().NextDouble() - 0.5) * 12f;

            World.AddExplosion(explosionPos, ExplosionType.Car, 6f, 0.5f, null, true, false);

            if (_missionVehicle.Exists() && _missionVehicle.EngineHealth < EngineHealthMin)
                _missionVehicle.EngineHealth = EngineHealthMin;

            GTA.UI.Screen.ShowSubtitle("~r~[Explosion]~w~ Nearby explosion!", 3000);
        }

        private void DoForceDetour()
        {
            if (!EnsureMissionVehicle() || _detourActive) return;
            var rng = new Random();
            _detourPoint = DetourPoints[rng.Next(DetourPoints.Length)];
            _detourActive = true;

            if (_waypointBlip != null && _waypointBlip.Exists())
                _waypointBlip.Position = _detourPoint;

            GTA.UI.Screen.ShowSubtitle("~r~[Detour]~w~ Route changed! Follow new waypoint.", 3000);
        }

        private bool EnsureMissionVehicle()
        {
            if (_state != GameState.Active) return false;
            if (_missionVehicle == null || !_missionVehicle.Exists()) return false;
            return true;
        }

        private void ClearAllTrackedEntities()
        {
            foreach (var te in _trackedEntities)
            {
                if (te.Entity != null && te.Entity.Exists())
                    te.Entity.Delete();
            }
            _trackedEntities.Clear();
        }

        private static string GetStringValue(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out object val) && val != null)
                return val.ToString();
            return null;
        }

        private static int GetIntValue(Dictionary<string, object> dict, string key, int fallback)
        {
            if (dict.TryGetValue(key, out object val) && val != null)
            {
                if (val is int i) return i;
                if (int.TryParse(val.ToString(), out int p)) return p;
            }
            return fallback;
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _wsCts?.Cancel();
            _ws?.Dispose();
            ClearAllTrackedEntities();
            if (_waypointBlip != null && _waypointBlip.Exists())
                _waypointBlip.Delete();
            Tick -= OnTick;
            Aborted -= OnAborted;
        }
    }
}
