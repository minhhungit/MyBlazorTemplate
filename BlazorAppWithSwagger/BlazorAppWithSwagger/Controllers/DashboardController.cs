using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Exceptional;

namespace BlazorAppWithSwagger.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TestError()
        {
            throw new Exception("test error from controller");
        }

        //[Authorize]
        [Route("Administration/ExceptionLog/{*pathInfo}"), IgnoreAntiforgeryToken]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task Exceptions() => await ExceptionalMiddleware.HandleRequestAsync(HttpContext);
    }
}
