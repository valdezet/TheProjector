using Microsoft.AspNetCore.Mvc;
using TheProjector.Data.DTO.Form;
using TheProjector.Services;
using TheProjector.Data.DTO;

namespace TheProjector.Controllers;


public class PeopleController : Controller
{

    private PersonService _service;

    public PeopleController(PersonService personService)
    {
        _service = personService;
    }

    public IActionResult Index()
    {
        return View();
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