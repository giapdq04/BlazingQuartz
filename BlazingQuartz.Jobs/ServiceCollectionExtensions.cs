using System;
using BlazeQuartz.Jobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BlazeQuartz.Jobs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazingQuartzJobs(this IServiceCollection services)
        {
            // require to run BlazingQuartz.Jobs.HttpJob
            services.AddHttpClient();
            services.AddHttpClient(Constants.HttpClientIgnoreVerifySsl)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });

            return Abstractions.ServiceCollectionExtensions.AddBlazingQuartzJobs(services);
        }
    }
}

