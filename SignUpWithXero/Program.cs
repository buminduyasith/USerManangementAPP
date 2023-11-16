using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "XeroSignIn";
            })
            .AddCookie()
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

               options.CallbackPath = "/signin-oidc";
              /* options.Events = new OpenIdConnectEvents
               {
                   OnTokenValidated = OnTokenValidated(),
               };
*/
           });

static Func<TokenValidatedContext, Task> OnTokenValidated()
{
    return async context =>
    {

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.ReadJwtToken(context.TokenEndpointResponse.AccessToken);
        var idToken = handler.ReadJwtToken(context.TokenEndpointResponse.IdToken);

        // Custom cookie authentication
        var claims = new List<Claim>()
        {
          new Claim("XeroUserId", accessToken.Claims.First(Claim => Claim.Type == "xero_userid").Value),
          new Claim("SessionId", accessToken.Claims.First(claim => claim.Type == "global_session_id").Value),
          new Claim("Name", idToken.Claims.First(claim => claim.Type == "name").Value),
          new Claim("FirstName", idToken.Claims.First(claim => claim.Type == "given_name").Value),
          new Claim("LastName", idToken.Claims.First(claim => claim.Type == "family_name").Value),
          new Claim ("Email", idToken.Claims.First(claim => claim.Type == "email").Value),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        context.Principal.AddIdentity(claimsIdentity);

        await context.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties()
            {
                ExpiresUtc = accessToken.ValidTo,
            });
        return;
    };
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
