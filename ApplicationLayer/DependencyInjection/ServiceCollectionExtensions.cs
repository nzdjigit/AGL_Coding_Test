using System;
using System.Net.Http;
using ApplicationLayer.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace ApplicationLayer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IDataProcessor, OwnerAndPetsDataProcessor>();
            services.AddHttpClient<IDataProcessor, OwnerAndPetsDataProcessor>().AddPolicyHandler(GetRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                                       .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                                       .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
