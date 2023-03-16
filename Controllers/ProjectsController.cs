using Microsoft.AspNetCore.Mvc;
using TheProjector.Data.DTO.Form;
using TheProjector.Data.DTO;
using TheProjector.Services;

namespace TheProjector.Controllers;

public class ProjectsController : Controller
{

    private ProjectService _service;

    public ProjectsController(ProjectService projectService)
    {
        _service = projectService;
    }

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
    public async Task<IActionResult> Create(ProjectForm createForm)
    {
        if (!ModelState.IsValid)
        {
            return View(createForm);
        }
        CommandResult result = await _service.CreateProject(createForm);
        if (result.IsSuccessful)
        {
            return RedirectToAction("Index");
        }
        return NoContent();
    }
}