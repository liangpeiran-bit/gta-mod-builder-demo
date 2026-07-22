using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace ModProject.LiveStudio
{
    public static class LiveStudioParser
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer
        {
            MaxJsonLength = 16 * 1024 * 1024,
        };

        public static LiveStudioEvent Parse(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;

            Dictionary<string, object> root;
            try
            {
                root = Serializer.Deserialize<Dictionary<string, object>>(raw);
            }
            catch
            {
                return null;
            }
            if (root == null) return null;

            var body = GetDict(root, "data");
            if (body == null) return null;

            var common = GetDict(body, "common");
            var envelopeType = GetString(root, "type");
            if (common == null && string.Equals(envelopeType, "subscribeResult", StringComparison.Ordinal))
            {
                return new SubscriptionEvent(
                    GetString(root, "id"),
                    GetString(body, "status"),
                    GetString(body, "name"));
            }

            var method = GetString(common, "method");
            if (string.IsNullOrEmpty(method)) return null;

            var msgId = GetString(common, "msgId");
            var createTime = GetLong(common, "createTime");
            var user = GetDict(body, "user");
            var userId = GetString(user, "id");
            var nickname = GetString(user, "nickname");

            switch (method)
            {
                case "WebcastChatMessage":
                {
                    var content = GetString(body, "content");
                    return new ChatEvent(msgId, createTime, userId, nickname, content);
                }
                case "WebcastGiftMessage":
                {
                    var gift = GetDict(body, "gift");
                    var giftId = GetString(body, "giftId");
                    if (string.IsNullOrEmpty(giftId))
                    {
                        giftId = GetString(gift, "id");
                    }
                    var giftName = GetString(gift, "name");
                    var diamondCount = (int)GetLong(gift, "diamondCount");
                    var repeatCount = (int)GetLong(body, "repeatCount");
                    var repeatEnd = GetBool(body, "repeatEnd");
                    var comboCount = (int)GetLong(body, "comboCount");
                    return new GiftEvent(
                        msgId,
                        createTime,
                        userId,
                        nickname,
                        giftId,
                        giftName,
                        diamondCount,
                        repeatCount,
                        repeatEnd,
                        comboCount);
                }
                default:
                    return null;
            }
        }

        private static Dictionary<string, object> GetDict(Dictionary<string, object> d, string key)
        {
            if (d == null || !d.TryGetValue(key, out var v)) return null;
            return v as Dictionary<string, object>;
        }

        private static string GetString(Dictionary<string, object> d, string key)
        {
            if (d == null || !d.TryGetValue(key, out var v) || v == null) return null;
            return v.ToString();
        }

        private static long GetLong(Dictionary<string, object> d, string key)
        {
            if (d == null || !d.TryGetValue(key, out var v) || v == null) return 0L;
            try { return Convert.ToInt64(v); }
            catch { return 0L; }
        }

        private static bool GetBool(Dictionary<string, object> d, string key)
        {
            if (d == null || !d.TryGetValue(key, out var value) || value == null) return false;
            if (value is bool boolean) return boolean;

            var text = value.ToString();
            if (string.Equals(text, "1", StringComparison.Ordinal)) return true;
            if (string.Equals(text, "0", StringComparison.Ordinal)) return false;
            if (bool.TryParse(text, out var parsed)) return parsed;

            try { return Convert.ToInt64(value) != 0L; }
            catch { return false; }
        }
    }
}
