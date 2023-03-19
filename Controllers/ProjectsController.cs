using Microsoft.AspNetCore.Mvc;
using TheProjector.Data.DTO.Form;
using TheProjector.Data.DTO;
using TheProjector.Data.Request;
using TheProjector.Data.ViewModels;
using TheProjector.Services;

namespace TheProjector.Controllers;

public class ProjectsController : Controller
{

    private ProjectService _service;
    private ProjectAssignmentService _assignmentService;

    public ProjectsController(ProjectService projectService, ProjectAssignmentService projectAssignmentService)
    {
        _service = projectService;
        _assignmentService = projectAssignmentService;
    }

    public async Task<IActionResult> Index(ProjectSearchRequest query)
    {
        ProjectSearchCollection viewModel = await _service.SearchProject(query);
        return View(viewModel);
    }

    public async Task<IActionResult> View(long id)
    {
        try
        {
            ProjectBasicInfo projectInfo = await _service.GetProjectBasicInfo(id);
            ICollection<PersonListItemInfo> assignablePeople = await _assignmentService.GetAssignablePeople(id);

            ProjectViewViewModel viewModel = new ProjectViewViewModel
            {
                BasicInfo = projectInfo,
                AssignablePeople = assignablePeople
            };
            return View(viewModel);
        }
        catch (Exception e) when (e is ArgumentNullException || e is InvalidOperationException)
        {
            return NotFound();
        }

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