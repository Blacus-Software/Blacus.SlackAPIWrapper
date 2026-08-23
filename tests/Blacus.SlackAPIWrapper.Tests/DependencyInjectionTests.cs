using Blacus.SlackAPIWrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class DependencyInjectionTests
    {

        private class MockHttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext? HttpContext { get; set; }
        }

        [Fact]
        public void AddSlackDependency_RegistersISlackClientAndSlackClient()
        {
            var services = new ServiceCollection();
            services.AddSlackDependency();

            var provider = services.BuildServiceProvider();
            var slackClient = provider.GetService<ISlackClient>();
            var concreteClient = provider.GetService<SlackClient>();

            Assert.NotNull(slackClient);
            Assert.NotNull(concreteClient);
        }

        [Fact]
        public void AddSlackDependency_WithCredential_RegistersAllServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHttpContextAccessor, MockHttpContextAccessor>();
            var credential = new SlackCredential
            {
                BotUserToken = "xoxb-123",
                SigningSecret = "secret456",
                AppToken = "xapp-789"
            };

            services.AddSlackDependency(credential);

            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider.GetService<ISlackClient>());
            Assert.NotNull(provider.GetService<SlackCredential>());
            Assert.Equal("xoxb-123", provider.GetService<SlackCredential>()?.BotUserToken);
            Assert.NotNull(provider.GetService<ISlackASPTransmission>());
        }

    }

}
