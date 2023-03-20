using Microsoft.AspNetCore.Mvc;
using TheProjector.Data.DTO.Form;
using TheProjector.Services;
using TheProjector.Data.DTO;
using TheProjector.Data.Request;

namespace TheProjector.Controllers;


public class PeopleController : Controller
{

    private PersonService _service;

    public PeopleController(PersonService personService)
    {
        _service = personService;
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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PersonForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }
        CommandResult insertResult = await _service.CreatePerson(form);
        return RedirectToAction("Index");
    }


}