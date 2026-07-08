namespace ModProject.LiveStudio
{
    public abstract class LiveStudioEvent
    {
        public string MsgId { get; }
        public long CreateTime { get; }

        protected LiveStudioEvent(string msgId, long createTime)
        {
            MsgId = msgId;
            CreateTime = createTime;
        }
    }

    public sealed class ChatEvent : LiveStudioEvent
    {
        public string UserId { get; }
        public string Nickname { get; }
        public string Content { get; }

        public ChatEvent(string msgId, long createTime, string userId, string nickname, string content)
            : base(msgId, createTime)
        {
            UserId = userId;
            Nickname = nickname;
            Content = content;
        }
    }

    public sealed class GiftEvent : LiveStudioEvent
    {
        public string UserId { get; }
        public string Nickname { get; }
        public string GiftId { get; }
        public string GiftName { get; }
        public int DiamondCount { get; }
        public int RepeatCount { get; }
        public bool RepeatEnd { get; }
        public int ComboCount { get; }

        public GiftEvent(
            string msgId,
            long createTime,
            string userId,
            string nickname,
            string giftId,
            string giftName,
            int diamondCount,
            int repeatCount,
            bool repeatEnd,
            int comboCount)
            : base(msgId, createTime)
        {
            UserId = userId;
            Nickname = nickname;
            GiftId = giftId;
            GiftName = giftName;
            DiamondCount = diamondCount;
            RepeatCount = repeatCount;
            RepeatEnd = repeatEnd;
            ComboCount = comboCount;
        }
    }
}
