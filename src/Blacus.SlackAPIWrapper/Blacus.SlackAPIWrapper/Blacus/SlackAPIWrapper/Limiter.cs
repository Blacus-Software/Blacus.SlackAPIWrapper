using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper
{

    public class Limiter
    {

        public string Id { get; }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Limiter> limiters = new();

        private System.Timers.Timer processorTimer;

        private SemaphoreSlim semaphoreSlim;

        private int limit = 0;

        private readonly object processorLock = new();

        private void runProcessor()
        {

            int toRelease = 0;

            lock (processorLock)
            {

                if (semaphoreSlim.CurrentCount == limit)
                    processorTimer.Enabled = false;

                toRelease = Math.Min(limit, limit - semaphoreSlim.CurrentCount);

                if (semaphoreSlim.CurrentCount < limit)
                    semaphoreSlim.Release(toRelease);

            }

        }

        private Limiter(string id, int limit, TimeSpan timeSpan)
        {

            this.Id = id;

            this.limit = limit;
            semaphoreSlim = new SemaphoreSlim(limit, limit);

            processorTimer = new System.Timers.Timer(timeSpan.TotalMilliseconds)
            {
                AutoReset = true
            };

            processorTimer.Elapsed += (sender, e) => runProcessor();
            processorTimer.Start();

        }

        public void Wait()
        {

            if (!this.processorTimer.Enabled)
                this.processorTimer.Enabled = true;

            semaphoreSlim.Wait();

        }

        public async Task WaitAsync()
        {

            if (!this.processorTimer.Enabled)
                this.processorTimer.Enabled = true;

            await semaphoreSlim.WaitAsync();

        }

        public static Limiter LimiterFactory(LimiterTier limierTier)
        {

            int? limit = null;
            TimeSpan? timeSpan = null;

            switch (limierTier)
            {
                case LimiterTier.WebAPITier1:
                    limit = 1;
                    timeSpan = new(0, 1, 0);
                    break;

                case LimiterTier.WebAPITier2:
                    limit = 20;
                    timeSpan = new(0, 1, 0);
                    break;

                case LimiterTier.WebAPITier3:
                    limit = 50;
                    timeSpan = new(0, 1, 0);
                    break;

                case LimiterTier.WebAPITier4:
                    limit = 100;
                    timeSpan = new(0, 1, 0);
                    break;
            }

            return LimiterFactory(limierTier.ToString(), limit, timeSpan);

        }

        public static Limiter LimiterFactory(string id, int? limit = null, TimeSpan? timeSpan = null)
        {

            return limiters.GetOrAdd(id, (key) =>
            {

                if (limit == null || timeSpan == null)
                    throw new NullReferenceException("Limit or Timespan not provided");

                return new Limiter(key, limit.Value, timeSpan.Value);

            });

        }

    }

    public enum LimiterTier
    {
        WebAPITier1,
        WebAPITier2,
        WebAPITier3,
        WebAPITier4,
    }

}