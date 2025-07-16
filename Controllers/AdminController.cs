using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests;
using travel_agency_back.Services.Interfaces;
namespace travel_agency_back.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AdminController : Controller
    {

        private IPackageService PackageService;
        public AdminController(IPackageService packageService)
        {
            PackageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(BuyPackageDTO buyPackageDTO)
        {
            return Ok();
        }
    }
}
