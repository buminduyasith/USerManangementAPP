using LearnGraphQL.Schema;
using OpenIddict.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<StudentQuery>()
    .AddMutationType<Mutation>();

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

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();
