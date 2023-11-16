using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace USerManangementAPP.Controllers
{
    public class StaffManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("message")]
        public async Task<IActionResult> GetMessage()
        {
            var subject = User.FindFirst(Claims.Subject)?.Value;

            return Ok(subject);
        }
    }
}
