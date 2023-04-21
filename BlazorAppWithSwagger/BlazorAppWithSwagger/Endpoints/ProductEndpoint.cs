using Microsoft.AspNetCore.Mvc;

namespace BlazorAppWithSwagger.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProductEndpoint : Controller
    {
        [HttpPost]
        public IActionResult Create([FromBody] CreateProductRequest request)
        {
            return Content($"{request.Name} - {DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        }
    }

    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;     
    }
}
