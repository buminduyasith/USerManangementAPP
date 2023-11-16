using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQLConsumer;
using Microsoft.Extensions.Configuration;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(builder.Configuration["GraphQLServer"], new NewtonsoftJsonSerializer()));
builder.Services.AddScoped<OwnerConsumer>();

builder.Services.AddOpenIddict()

    // Register the OpenIddict client components.
    .AddClient(options =>
    {
        // Allow grant_type=client_credentials to be negotiated.
        options.AllowClientCredentialsFlow();

        // Disable token storage, which is not necessary for non-interactive flows like
        // grant_type=password, grant_type=client_credentials or grant_type=refresh_token.
        options.DisableTokenStorage();

        options.UseSystemNetHttp();

        // Add a client registration matching the client application definition in the server project.
        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:7268/", UriKind.Absolute),

            ClientId = "console",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207"
        });
    });

builder.Services.AddOpenIddict()
             .AddValidation(options =>
             {
                 // Note: the validation handler uses OpenID Connect discovery
                 // to retrieve the issuer signing keys used to validate tokens.
                 options.SetIssuer("https://localhost:7268/");
                 options.UseIntrospection()
                 .SetClientId("console")
                     .SetClientSecret("388D45FA-B36B-4988-BA59-B187D329C207");

                 options.UseSystemNetHttp();
                 //  options.UseSystemNetHttp();

                 // Register the ASP.NET Core host.
                 options.UseAspNetCore();
             });

// builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
