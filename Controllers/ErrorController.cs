using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Models;
using Global.App.Middlewares;

namespace LibraryManagement.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Error(int statusCode)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var errorDetails = HttpContext.Items["ErrorDetails"] as ErrorDetails;

            var model = new ErrorViewModel
            {
                StatusCode = statusCode,
                OriginalPath = feature?.OriginalPath,
                OriginalQueryString = feature?.OriginalQueryString,
                Message = errorDetails?.Message 
            };
            return View(model);
        }
    }
}
