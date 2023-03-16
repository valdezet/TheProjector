using Microsoft.AspNetCore.Mvc;

namespace TheProjector.Controllers;

public class ProjectsController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
}