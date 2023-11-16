using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using USerManangementAPP.Models;

namespace USerManangementAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            List<StaffMemberDto> inviteList = new List<StaffMemberDto>
            {
                new StaffMemberDto { Name = "John Doe", Email = "johndoe@example.com", IsInvited = true, IsActive = true },
                new StaffMemberDto { Name = "Jane Smith", Email = "janesmith@example.com", IsInvited = false, IsActive = true },
                new StaffMemberDto { Name = "Bob Johnson", Email = "bobj@example.com", IsInvited = true, IsActive = false }
            };

            return View(inviteList);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "XeroSignIn")]
        public async Task<IActionResult> Privacy()
        {

            var x = User;

            var accessTokenww = await HttpContext.GetTokenAsync("XeroSignIn", "access_token");
            var accessTokenww2 = await HttpContext.GetTokenAsync("XeroSignIn", "refresh_token");
            var accessTokenwsd = await HttpContext.GetTokenAsync("xerosignintemp", "access_token");
            var accessTokenwsd2 = await HttpContext.GetTokenAsync("xerosignintemp", "refresh_token");

            // var handler = new JwtSecurityTokenHandler();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var IdToken = await HttpContext.GetTokenAsync("id_token");
            var accessToke2n = await HttpContext.GetTokenAsync("XeroSignIn");
            // var token = handler.ReadJwtToken(IdToken);
            await HttpContext.SignOutAsync("xerosignintemp");
            return View();
        }


        public IActionResult Edit(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Test()
        {
            var x = User;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}