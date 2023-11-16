namespace USerManangementAPP
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using OpenIddict.Abstractions;
    using USerManangementAPP.Data;
    using static OpenIddict.Abstractions.OpenIddictConstants;


    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            // if you change permissions once you created this client, you have to manually add those thing to db or you have to add new client or remove the this client in the db
            if (await manager.FindByClientIdAsync("console") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "console",
                    ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                    DisplayName = "My client application",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Introspection,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.Prefixes.Scope + "api",
                    }
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
