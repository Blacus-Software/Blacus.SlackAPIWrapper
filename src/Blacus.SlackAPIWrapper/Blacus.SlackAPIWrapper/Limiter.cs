using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper
{

    /// <summary>
    /// A thread-safe, per-identifier permit limiter. Permits are refilled toward
    /// the configured limit on a fixed interval by a background loop driven by
    /// <see cref="PeriodicTimer"/>.
    /// </summary>
    public sealed class Limiter : IAsyncDisposable
    {

        private static readonly ConcurrentDictionary<string, Lazy<Limiter>> limiters = new();

        public string Id { get; }

        private readonly int limit;
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly PeriodicTimer refillTimer;
        private readonly CancellationTokenSource cancellationSource;
        private readonly Task refillLoop;

        private int disposed;

        private Limiter(string id, int limit, TimeSpan interval)
        {

            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit));

            if (interval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(interval));

            this.Id = id;
            this.limit = limit;
            this.semaphoreSlim = new SemaphoreSlim(limit, limit);
            this.cancellationSource = new CancellationTokenSource();
            this.refillTimer = new PeriodicTimer(interval);

            this.refillLoop = Task.Run(RefillLoopCore);

        }

        private async Task RefillLoopCore()
        {

            while (await refillTimer.WaitForNextTickAsync(cancellationSource.Token).ConfigureAwait(false))
            {

                int toRelease = limit - semaphoreSlim.CurrentCount;

                if (toRelease > 0)
                    semaphoreSlim.Release(toRelease);

            }

        }

        public async Task WaitAsync(CancellationToken cancellationToken = default)
        {

            throwIfDisposed();

            await semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

        }

        public async ValueTask DisposeAsync()
        {

            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            limiters.TryRemove(Id, out _);

            cancellationSource.Cancel();

            try
            {
                await refillLoop.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected: the refill loop was cancelled as part of shutdown.
            }
            finally
            {
                cancellationSource.Dispose();
                refillTimer.Dispose();
                semaphoreSlim.Dispose();
            }

        }

        private void throwIfDisposed()
        {

            if (disposed != 0)
                throw new ObjectDisposedException(nameof(Limiter));

        }

        public static Limiter LimiterFactory(LimiterTier limiterTier)
        {

            int? limit = null;
            TimeSpan? interval = null;

            switch (limiterTier)
            {
                case LimiterTier.WebAPITier1:
                    limit = 1;
                    interval = new(0, 1, 0);
                    break;

                case LimiterTier.WebAPITier2:
                    limit = 20;
                    interval = new(0, 1, 0);
                    break;

                case LimiterTier.WebAPITier3:
                    limit = 50;
                    interval = new(0, 1, 0);
                    break;

                case LimiterTier.WebAPITier4:
                    limit = 100;
                    interval = new(0, 1, 0);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(limiterTier));
            }

            return LimiterFactory(limiterTier.ToString(), limit, interval);

        }

        public static Limiter LimiterFactory(string id, int? limit = null, TimeSpan? timeSpan = null)
        {

            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (limit == null)
                throw new ArgumentNullException(nameof(limit));

            if (timeSpan == null)
                throw new ArgumentNullException(nameof(timeSpan));

            // The first limiter registered for a given id wins; later calls with different
            // parameters are silently ignored, matching the previous GetOrAdd contract.
            Lazy<Limiter> lazy = limiters.GetOrAdd(id, (key) =>
                new Lazy<Limiter>(
                    () => new Limiter(key, limit.Value, timeSpan.Value),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            return lazy.Value;

        }

        public static bool TryGet(string id, out Limiter limiter)
        {

            if (limiters.TryGetValue(id, out Lazy<Limiter>? lazy))
            {
                limiter = lazy.Value;
                return true;
            }

            limiter = null!;
            return false;

        }

        public static async Task<bool> RemoveAsync(string id)
        {

            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (!limiters.TryRemove(id, out Lazy<Limiter>? lazy))
                return false;

            await lazy.Value.DisposeAsync();

            return true;

        }

        public static async Task DisposeAllAsync()
        {

            foreach (string id in limiters.Keys)
                await RemoveAsync(id);

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
