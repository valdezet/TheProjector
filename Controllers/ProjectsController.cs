using Microsoft.AspNetCore.Mvc;
using TheProjector.Data.DTO.Form;

namespace TheProjector.Controllers;

public class ProjectsController : Controller
{

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProjectForm createForm)
    {
        if (!ModelState.IsValid)
        {
            return View(createForm);
        }
        // WIP change noContent();
        return NoContent();
    }
}