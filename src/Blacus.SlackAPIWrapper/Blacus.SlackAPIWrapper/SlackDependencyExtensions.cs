namespace Blacus.SlackAPIWrapper
{

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class SlackDependencyExtensions
    {

        public static IServiceCollection AddSlackDependency(this IServiceCollection services, SlackCredential slackCredential)
        {

            services.AddScoped<ISlackASPTransmission>((sp) =>
                new SlackASPTransmission(
                    sp.GetService<IHttpContextAccessor>(),
                    slackCredential 
                    ));

            return services;

        }

    }

}
