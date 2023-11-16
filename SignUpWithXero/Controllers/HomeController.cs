using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignUpWithXero.Models;
using System.Diagnostics;

namespace SignUpWithXero.Controllers
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
            return View();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "XeroSignIn")]
        public async Task<IActionResult> Privacy()
        {
            var x = User;
            // var handler = new JwtSecurityTokenHandler();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var IdToken = await HttpContext.GetTokenAsync("id_token");
            var accessToke2n = await HttpContext.GetTokenAsync("XeroSignIn");
            // var token = handler.ReadJwtToken(IdToken);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}