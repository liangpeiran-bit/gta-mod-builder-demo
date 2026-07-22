using System;
using System.IO;
using ModProject.LiveStudio;

namespace LiveStudioContractTests
{
    internal static class Program
    {
        private static int _failures;

        private static int Main()
        {
            Check("subscribe-ack.json", evt =>
            {
                var subscription = evt as SubscriptionEvent;
                Assert(subscription != null, "subscribe acknowledgement must parse as SubscriptionEvent");
                Assert(subscription != null && subscription.Status == "ok", "subscription status must be preserved");
                Assert(subscription != null && subscription.Name == "IM_MESSAGE_TRANSPORT", "subscription name must be preserved");
            });
            Check("chat.json", evt =>
            {
                var chat = evt as ChatEvent;
                Assert(chat != null, "chat fixture must parse as ChatEvent");
                Assert(chat != null && chat.Content == "boost", "chat content must be preserved");
            });
            Check("gift-repeat-end-number.json", evt =>
            {
                var gift = evt as GiftEvent;
                Assert(gift != null, "numeric repeatEnd fixture must parse as GiftEvent");
                Assert(gift != null && gift.RepeatEnd, "numeric repeatEnd=1 must normalize to true");
                Assert(gift != null && gift.GiftId == "5655", "top-level giftId must be supported");
            });
            Check("gift-repeat-end-bool.json", evt =>
            {
                var gift = evt as GiftEvent;
                Assert(gift != null && gift.RepeatEnd, "boolean repeatEnd=true must normalize to true");
                Assert(gift != null && gift.GiftId == "5655", "nested gift.id fallback must be supported");
            });
            Check("gift-combo-start.json", evt =>
            {
                var gift = evt as GiftEvent;
                Assert(gift != null && !gift.RepeatEnd, "numeric repeatEnd=0 must normalize to false");
            });
            Check("malformed.json", evt => Assert(evt == null, "malformed JSON must be ignored"));

            if (_failures == 0)
            {
                Console.WriteLine("LIVE Studio protocol fixtures passed.");
                return 0;
            }

            Console.Error.WriteLine(_failures + " LIVE Studio protocol fixture assertion(s) failed.");
            return 1;
        }

        private static void Check(string fileName, Action<LiveStudioEvent> assertion)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fixtures", fileName);
            var raw = File.ReadAllText(path);
            assertion(LiveStudioParser.Parse(raw));
        }

        private static void Assert(bool condition, string message)
        {
            if (condition) return;
            _failures++;
            Console.Error.WriteLine("FAILED: " + message);
        }
    }
}
