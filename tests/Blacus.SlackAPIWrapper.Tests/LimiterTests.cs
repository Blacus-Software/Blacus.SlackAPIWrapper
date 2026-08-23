using Blacus.SlackAPIWrapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class LimiterTests
    {

        [Fact]
        public async Task Limiter_PermitEnforcement_AllowsUpToLimit()
        {
            string limiterId = "test_limiter_" + Guid.NewGuid().ToString();
            var limiter = Limiter.LimiterFactory(limiterId, 2, TimeSpan.FromMilliseconds(200));

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            // Should allow 2 permits immediately
            await limiter.WaitAsync(cts.Token);
            await limiter.WaitAsync(cts.Token);

            // Removing and disposing the limiter
            await Limiter.RemoveAsync(limiterId);
        }

        [Fact]
        public void Limiter_Factory_CreatesCorrectTierSettings()
        {
            var tier1 = Limiter.LimiterFactory(LimiterTier.WebAPITier1);
            Assert.NotNull(tier1);
            Assert.Equal(LimiterTier.WebAPITier1.ToString(), tier1.Id);

            var tier4 = Limiter.LimiterFactory(LimiterTier.WebAPITier4);
            Assert.NotNull(tier4);
            Assert.Equal(LimiterTier.WebAPITier4.ToString(), tier4.Id);
        }

    }

}
