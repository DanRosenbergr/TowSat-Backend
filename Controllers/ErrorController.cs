using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace TowSat_Backend.Controllers {

    [ApiController]
    public class ErrorController : ControllerBase {
        [Route("/error")]
        [HttpGet, HttpPost, HttpDelete, HttpPut]
        public IActionResult HandleError() {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            return Problem(
                detail: exception?.ToString(),
                title: "An error occurred"
            );
        }
    }
}
