using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using USerManangementAPP;
using USerManangementAPP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseOpenIddict();
});
builder.Services.AddHttpClient();
/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

// JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 4;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
})
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders()
               .AddDefaultUI();
// JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication()
            .AddCookie("xerosignintemp")
           .AddOpenIdConnect("XeroSignIn", options =>
           {
               options.Authority = "https://identity.xero.com";
               options.SaveTokens = true;

               options.ClientId = builder.Configuration["XeroIdentity:ClientId"];
               options.ResponseType = "code";
               options.ResponseMode = "query";

               options.UsePkce = true;

               options.Scope.Clear();
               options.Scope.Add("openid");
               options.Scope.Add("profile");
               options.Scope.Add("email");
               options.Scope.Add("offline_access");

               options.SignInScheme = "xerosignintemp";
               // IdentityConstants.ApplicationScheme; IdentityConstants.ExternalScheme;

               options.CallbackPath = "/signin-oidc";
               /* options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = OnTokenValidated2(),
                };*/

           });

static Func<TokenValidatedContext, Task> OnTokenValidated()
{
    return async context =>
    {
        var services = context.HttpContext.RequestServices;
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var signInManager = services.GetRequiredService<SignInManager<IdentityUser>>();

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.ReadJwtToken(context.TokenEndpointResponse.AccessToken);
        var idToken = handler.ReadJwtToken(context.TokenEndpointResponse.IdToken);

        var newUser = new IdentityUser
        {
            UserName = idToken.Claims.First(claim => claim.Type == "given_name").Value,
            Email = idToken.Claims.First(claim => claim.Type == "email").Value,
        };

        // Add the new user to the database
        var result = await userManager.CreateAsync(newUser);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(newUser, true);
            // await context.HttpContext.;
        }

        // Custom cookie authentication
        /*   var claims = new List<Claim>()
       {
         *//*new Claim("XeroUserId", accessToken.Claims.First(Claim => Claim.Type == "xero_userid").Value),
         new Claim("SessionId", accessToken.Claims.First(claim => claim.Type == "global_session_id").Value),
         new Claim("Name", idToken.Claims.First(claim => claim.Type == "name").Value),
         new Claim("FirstName", idToken.Claims.First(claim => claim.Type == "given_name").Value),
         new Claim("LastName", idToken.Claims.First(claim => claim.Type == "family_name").Value),
         new Claim ("Email", idToken.Claims.First(claim => claim.Type == "email").Value),*//*
         new Claim ("Ammma", "sujatha"),
       };

       var claimsIdentity = new ClaimsIdentity(
           claims, CookieAuthenticationDefaults.AuthenticationScheme);

       context.Principal.AddIdentity(claimsIdentity);

       await context.HttpContext.SignInAsync(
          IdentityConstants.ApplicationScheme,
           new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties()
           {
               ExpiresUtc = accessToken.ValidTo,
           });*/
        return;
    };
}


static Func<TokenValidatedContext, Task> OnTokenValidated2()
{
    return async context =>
    {

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.ReadJwtToken(context.TokenEndpointResponse.AccessToken);
        var idToken = handler.ReadJwtToken(context.TokenEndpointResponse.IdToken);
        var refreshToken = context.TokenEndpointResponse.RefreshToken;
        // Custom cookie authentication
        var claims = new List<Claim>()
        {
          new Claim("XeroUserId", accessToken.Claims.First(Claim => Claim.Type == "xero_userid").Value),
          new Claim("SessionId", accessToken.Claims.First(claim => claim.Type == "global_session_id").Value),
          new Claim("Name", idToken.Claims.First(claim => claim.Type == "name").Value),
          new Claim("FirstName", idToken.Claims.First(claim => claim.Type == "given_name").Value),
          new Claim("LastName", idToken.Claims.First(claim => claim.Type == "family_name").Value),
          new Claim ("Email", idToken.Claims.First(claim => claim.Type == "email").Value),
          new Claim ("refreshtoken", refreshToken),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, "xerosignintemp");

        context.Principal.AddIdentity(claimsIdentity);

        await context.HttpContext.SignInAsync(
           "xerosignintemp",
            new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties()
            {
                ExpiresUtc = accessToken.ValidTo,
            });
        return;
    };
}

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

/*builder.Services.AddDatabaseDeveloperPageExceptionFilter();*/

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/


builder.Services.AddControllersWithViews();

builder.Services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});


builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);


builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();

        // Enable Quartz.NET integration.
        options.UseQuartz();
    })
    .AddServer(options =>
    {
        // Enable the token endpoint.
        options.SetTokenEndpointUris("connect/token");
        options.SetIntrospectionEndpointUris("/connect/introspect");

        // Enable the client credentials flow.
        options.AllowClientCredentialsFlow();

        options.RegisterClaims("customclaim");
        options.RegisterScopes("api");

        // Register the signing and encryption credentials.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.DisableAccessTokenEncryption();

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();
    })
    // Register the OpenIddict validation components.
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

// Register the worker responsible for seeding the database.
// Note: in a real world application, this step should be part of a setup script.
builder.Services.AddHostedService<Worker>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();


