using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Edit(string email)
        {
            ViewBag.Email = email;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}