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

public class RoseWantedStreetBrawl : Script
{
    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;
    private DateTime _lastTriggerTime = DateTime.MinValue;
    private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(30);
    private RelationshipGroup _enemyGroup;
    private volatile bool _pendingTrigger;
    private bool _isCleanedUp;
    private readonly object _cleanupLock = new object();

    public RoseWantedStreetBrawl()
    {
        _cts = new CancellationTokenSource();
        _enemyGroup = new RelationshipGroup(Game.GenerateHash("RoseWantedEnemyGroup"));

        Tick += OnTick;
        Aborted += OnAborted;

        Task.Run(() => WebSocketLoop(_cts.Token));
    }

    private void OnTick(object sender, EventArgs e)
    {
        if (Game.Player.Character.IsDead)
        {
            Cleanup();
            return;
        }

        if (_pendingTrigger)
        {
            _pendingTrigger = false;
            ExecuteRoseEffect();
        }
    }

    private void OnAborted(object sender, EventArgs e)
    {
        Cleanup();
    }

    private void ExecuteRoseEffect()
    {
        if (DateTime.UtcNow - _lastTriggerTime < _cooldown)
            return;

        _lastTriggerTime = DateTime.UtcNow;

        try
        {
            Ped playerPed = Game.Player.Character;

            Game.Player.WantedLevel = 3;

            playerPed.Weapons.Give(WeaponHash.SMG, 500, true, true);

            int playerGroupHash = playerPed.RelationshipGroup.Hash;
            int enemyGroupHash = _enemyGroup.Hash;

            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, (int)Relationship.Hate, enemyGroupHash, playerGroupHash);
            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, (int)Relationship.Hate, playerGroupHash, enemyGroupHash);

            Ped[] nearbyPeds = World.GetNearbyPeds(playerPed, 50f);

            foreach (Ped ped in nearbyPeds)
            {
                if (ped == null || ped == playerPed) continue;
                if (!ped.Exists() || ped.IsDead) continue;

                ped.RelationshipGroup = _enemyGroup;
                ped.Task.FightAgainst(playerPed);
            }

            GTA.UI.Notification.Show("~r~Rose Wanted Street Brawl!~w~ 3 stars + SMG. Survive!");
        }
        catch (Exception ex)
        {
            GTA.UI.Notification.Show("~y~Rose effect error: " + ex.Message);
        }
    }

    private void Cleanup()
    {
        lock (_cleanupLock)
        {
            if (_isCleanedUp) return;
            _isCleanedUp = true;
        }

        _cts.Cancel();

        try
        {
            int playerGroupHash = Game.Player.Character.RelationshipGroup.Hash;
            int enemyGroupHash = _enemyGroup.Hash;

            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, (int)Relationship.Neutral, enemyGroupHash, playerGroupHash);
            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, (int)Relationship.Neutral, playerGroupHash, enemyGroupHash);
        }
        catch { }

        _lastTriggerTime = DateTime.MinValue;
        _pendingTrigger = false;

        Tick -= OnTick;
        Aborted -= OnAborted;
    }

    private async Task WebSocketLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var ws = new ClientWebSocket();
            _ws = ws;

            try
            {
                await ws.ConnectAsync(new Uri("ws://127.0.0.1:60080"), ct);

                string subscribeJson = "{\"service\":\"serviceSignalSub\",\"method\":\"IM_MESSAGE_TRANSPORT\"}";
                byte[] subBytes = Encoding.UTF8.GetBytes(subscribeJson);
                await ws.SendAsync(new ArraySegment<byte>(subBytes), WebSocketMessageType.Text, true, ct);

                var buffer = new byte[4096];
                var messageBuffer = new StringBuilder();

                while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
                {
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        try { await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None); } catch { }
                        break;
                    }

                    messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                    if (result.EndOfMessage)
                    {
                        string message = messageBuffer.ToString();
                        messageBuffer.Clear();
                        ProcessMessage(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                try { await Task.Delay(5000, ct); } catch (OperationCanceledException) { break; }
            }
            finally
            {
                _ws = null;
                try { ws.Dispose(); } catch { }
            }
        }
    }

    private void ProcessMessage(string json)
    {
        try
        {
            var serializer = new JavaScriptSerializer();
            var obj = serializer.Deserialize<Dictionary<string, object>>(json);
            if (obj == null) return;

            string type = obj.ContainsKey("type") ? (obj["type"]?.ToString() ?? "") : "";

            if (type == "Aborted")
            {
                Cleanup();
                return;
            }

            if (type == "WebcastGiftMessage")
            {
                int giftId = 0;
                if (obj.ContainsKey("giftId"))
                {
                    int.TryParse(obj["giftId"]?.ToString(), out giftId);
                }
                if (giftId == 5655)
                {
                    TriggerRoseEffect();
                }
                return;
            }

            if (obj.ContainsKey("data") && obj["data"] is Dictionary<string, object> data)
            {
                type = data.ContainsKey("type") ? (data["type"]?.ToString() ?? "") : "";

                if (type == "Aborted")
                {
                    Cleanup();
                    return;
                }

                if (type == "WebcastGiftMessage")
                {
                    int giftId = 0;
                    if (data.ContainsKey("giftId"))
                    {
                        int.TryParse(data["giftId"]?.ToString(), out giftId);
                    }
                    if (giftId == 5655)
                    {
                        TriggerRoseEffect();
                    }
                }
            }
        }
        catch
        {
        }
    }

    private void TriggerRoseEffect()
    {
        if (DateTime.UtcNow - _lastTriggerTime < _cooldown)
            return;

        _pendingTrigger = true;
    }
}