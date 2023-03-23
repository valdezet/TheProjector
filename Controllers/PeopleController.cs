using Microsoft.AspNetCore.Mvc;
using TheProjector.Utilities;
using TheProjector.Data.DTO;
using TheProjector.Data.Request;
using TheProjector.Data.ViewModels;

namespace TheProjector.Controllers;


public class PeopleController : Controller
{

    private PersonService _service;
    private ProjectAssignmentService _assignmentService;

    public PeopleController(
        PersonService personService,
        ProjectAssignmentService assignmentService
        )
    {
        _service = personService;
        _assignmentService = assignmentService;
    }

    public async Task<IActionResult> Index(PersonSearchRequest query)
    {
        PersonSearchCollection results = await _service.SearchPeople(query);
        if (results.CurrentPage > results.TotalPageCount)
        {
            query.Page = 1;
            return RedirectToAction("Index", query);
        }
        return View(results);
    }

    public async Task<IActionResult> View(long id)
    {
        try
        {
            PersonBasicInfo personInfo = await _service.GetPersonBasicInfo(id);
            ICollection<ProjectIdName> assignedProjects = await _assignmentService.GetProjectsAssignedTo(id);


            PersonViewViewModel viewModel = new PersonViewViewModel
            {
                BasicInfo = personInfo,
                AssignedProjects = assignedProjects
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
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PersonBasicInfo form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }
        CommandResult insertResult = await _service.CreatePerson(form);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        PersonBasicInfo form = await _service.GetPersonBasicInfo(id);
        return View(form);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PersonBasicInfo form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }
        CommandResult result = await _service.EditPerson(form);
        if (result.IsSuccessful)
        {
            return RedirectToAction("View", new { id = form.Id });
        }
        else
        {
            ModelState.AddModelError(String.Empty, result.ErrorMessage!);
            if (result.Errors != null)
            {
                foreach (KeyValuePair<string, string> error in result.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }
            return View(form);
        }
    }


}