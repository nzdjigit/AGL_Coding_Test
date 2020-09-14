using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ApplicationLayer.DependencyInjection;
using ApplicationLayer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AglCodingTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var config = LoadConfiguration();

            var dataProcessor = serviceProvider.GetRequiredService<IDataProcessor>();
            var dataSourceUrl = config["DataSourceUrl"];

            var processedData = await dataProcessor.ProcessDataAsync(dataSourceUrl);
            PrintResults(processedData);

            Console.ReadKey();

            CleanUp(serviceProvider);
        }

        private static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddApplicationServices();

            return serviceCollection.BuildServiceProvider();
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static void CleanUp(ServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }

            if (serviceProvider is IDisposable)
            {
                serviceProvider.Dispose();
            }
        }

        private static void PrintResults(Dictionary<string, List<string>> processedData)
        {
            foreach (var grouping in processedData)
            {
                Console.WriteLine($"{grouping.Key}");

                foreach (var groupingValue in grouping.Value)
                {
                    Console.WriteLine($"\t- {groupingValue}");
                }
            }
        }
    }
}
