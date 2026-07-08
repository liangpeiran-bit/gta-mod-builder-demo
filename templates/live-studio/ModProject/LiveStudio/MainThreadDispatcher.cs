using System;
using System.Collections.Concurrent;

namespace ModProject.LiveStudio
{
    public static class MainThreadDispatcher
    {
        private const int MaxDrainPerTick = 32;
        private const int MaxQueueDepth = 512;

        private static readonly ConcurrentQueue<Action> Queue = new ConcurrentQueue<Action>();
        private static int _depth;

        public static int PendingCount => _depth;

        public static bool Enqueue(Action action)
        {
            if (action == null) return false;
            if (System.Threading.Interlocked.Increment(ref _depth) > MaxQueueDepth)
            {
                System.Threading.Interlocked.Decrement(ref _depth);
                return false;
            }
            Queue.Enqueue(action);
            return true;
        }

        public static void DrainOnTick()
        {
            int processed = 0;
            while (processed < MaxDrainPerTick && Queue.TryDequeue(out var action))
            {
                System.Threading.Interlocked.Decrement(ref _depth);
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    try
                    {
                        GTA.UI.Notification.Show("~r~LIVE Studio action error: " + ex.Message);
                    }
                    catch
                    {
                    }
                }
                processed++;
            }
        }
    }
}
