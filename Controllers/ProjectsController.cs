using Microsoft.AspNetCore.Mvc;
using TheProjector.Data.DTO;
using TheProjector.Data.Form;
using TheProjector.Data.Request;
using TheProjector.Data.ViewModels;
using TheProjector.Services;

namespace TheProjector.Controllers;

public class ProjectsController : Controller
{

    private ProjectService _service;
    private ProjectAssignmentService _assignmentService;
    private CurrencyService _currencyService;

    public ProjectsController(
        ProjectService projectService,
        ProjectAssignmentService projectAssignmentService,
        CurrencyService currencyService)
    {
        _service = projectService;
        _assignmentService = projectAssignmentService;
        _currencyService = currencyService;
    }

    public async Task<IActionResult> Index(ProjectSearchRequest query)
    {
        ProjectSearchCollection viewModel = await _service.SearchProject(query);
        if (viewModel.CurrentPage > viewModel.TotalPageCount && viewModel.TotalPageCount > 0)
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

    [HttpGet]
    public IActionResult Create()
    {
        ProjectFormViewModel viewModel = new ProjectFormViewModel
        {
            Form = new ProjectForm(),
            Currencies = _currencyService.GetSupportedCurrencyInfo()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectFormViewModel viewModel)
    {
        ProjectForm createForm = viewModel.Form;
        if (await _service.CheckCodeExistence(createForm.Code))
        {
            ModelState.AddModelError("Form.Code", "The code is already taken by another project.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.Currencies = _currencyService.GetSupportedCurrencyInfo();
            return View(viewModel);
        }
        CommandResult result = await _service.CreateProject(createForm);
        if (result.IsSuccessful)
        {
            return RedirectToAction("Index");
        }
        else
        {
            viewModel.Currencies = _currencyService.GetSupportedCurrencyInfo();
            ModelState.AddModelError(String.Empty, result.ErrorMessage!);
            return View(viewModel);
        }
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(long id)
    {
        CommandResult result = await _service.ArchiveProject(id);
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unarchive(long id)
    {
        CommandResult result = await _service.UnarchiveProject(id);
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