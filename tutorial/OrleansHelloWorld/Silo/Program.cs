using System;
using System.Threading.Tasks;
using Grains;
using Grains.MyDomain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NStore.Core.InMemory;
using NStore.Core.Streams;
using NStore.Domain;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Streams;

namespace Silo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureServices(ConfigureNStore)
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }


        private static void ConfigureNStore(IServiceCollection services)
        {
            var persistence = new InMemoryPersistence();
            var streamFactory = new StreamsFactory(persistence);

            services.AddSingleton<IStreamsFactory>(streamFactory);
            services.AddSingleton<IAggregateFactory>(s => new AggregateFactory(t => (IAggregate) s.GetRequiredService(t)));

            services.AddTransient<TicketAggregate>();
            services.AddTransient<IRepository, Repository>();
        }
    }
}