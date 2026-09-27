namespace Blacus.SlackAPIWrapper
{

    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class SlackDependencyExtensions
    {

        /// <summary>
        /// Registers <see cref="ISlackClient"/> and <see cref="SlackClient"/> with the dependency injection container
        /// configured using <see cref="System.Net.Http.IHttpClientFactory"/>.
        /// </summary>
        public static IServiceCollection AddSlackDependency(this IServiceCollection services)
        {

            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddHttpContextAccessor();

            services.AddHttpClient<ISlackClient, SlackClient>();
            services.AddTransient<SlackClient>(sp => (SlackClient)sp.GetRequiredService<ISlackClient>());

            return services;

        }

        /// <summary>
        /// Registers <see cref="ISlackClient"/>, <see cref="SlackClient"/>, <see cref="SlackCredential"/>,
        /// and <see cref="ISlackTransmission"/> with the dependency injection container.
        /// </summary>
        public static IServiceCollection AddSlackDependency(this IServiceCollection services, SlackCredential slackCredential)
        {

            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (slackCredential == null)
                throw new ArgumentNullException(nameof(slackCredential));

            services.AddSlackDependency();

            services.AddSingleton(slackCredential);

            services.AddScoped<ISlackTransmission>((sp) =>
                new SlackTransmission(
                    sp.GetRequiredService<IHttpContextAccessor>(),
                    slackCredential 
                    ));

            return services;

        }

    }

}
