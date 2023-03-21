using Microsoft.AspNetCore.Mvc;
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
        if (viewModel.CurrentPage > viewModel.TotalPageCount)
        {
            query.Page = 1;
            return RedirectToAction("Index", query);
        }
        return View(viewModel);
    }

    public async Task<IActionResult> View(long id)
    {
        try
        {
            ProjectBasicInfo projectInfo = await _service.GetProjectBasicInfo(id);
            ICollection<PersonListItemInfo> assignablePeople = await _assignmentService.GetAssignablePeople(id);
            ICollection<PersonListItemInfo> assignedPeople = await _assignmentService.GetAssignedPeople(id);

            ProjectViewViewModel viewModel = new ProjectViewViewModel
            {
                BasicInfo = projectInfo,
                AssignablePeople = assignablePeople,
                AssignedPeople = assignedPeople
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
    public async Task<IActionResult> Create(ProjectBasicInfo createForm)
    {
        if (await _service.CheckCodeExistence(createForm.Code))
        {
            ModelState.AddModelError("Code", "The code is already taken by another project.");
        }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/[controller]/{projectId:long}/[action]")]
    public async Task<IActionResult> AssignPerson(ProjectPersonPairRequest pair)
    {
        CommandResult result = await _assignmentService.AssignPerson(pair);
        if (result.IsSuccessful)
        {
            return NoContent();
        }
        else
        {
            ModelState.AddModelError(String.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/[controller]/{projectId:long}/[action]")]
    public async Task<IActionResult> UnassignPerson(ProjectPersonPairRequest pair)
    {
        CommandResult result = await _assignmentService.UnassignPerson(pair);
        if (result.IsSuccessful)
        {
            return NoContent();
        }
        else
        {
            ModelState.AddModelError(String.Empty, result.ErrorMessage!);
            return BadRequest(ModelState);
        }
    }
}