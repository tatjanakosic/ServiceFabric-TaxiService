using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Communication.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ProductCatalogue
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                // Setup dependency injection
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                // Resolve IMapper
                var mapper = serviceProvider.GetRequiredService<IMapper>();

                // Register the service and pass the resolved mapper instance
                ServiceRuntime.RegisterServiceAsync("ProductCatalogueType",
                    context => new ProductCatalogue(context, mapper)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ProductCatalogue).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configure AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // Add your AutoMapper profiles here
            });

            // Add other services if necessary
            // services.AddTransient<ISomeService, SomeServiceImplementation>();
        }
    }
}
