
using Microsoft.AspNetCore.Mvc;
using TheProjector.Data.DTO;
using TheProjector.Data.Form;
using TheProjector.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TheProjector.Controllers;

public class AuthController : Controller
{
    private AuthService _service;
    public AuthController(
            AuthService service
    ) : base()
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    public async Task<IActionResult> Login(LoginForm form, [FromQuery] string? returnUrl)
    {
        CommandResult verifyPasswordResults = await _service.AuthenticateUser(form);
        if (!verifyPasswordResults.IsSuccessful)
        {
            ModelState.AddModelError(String.Empty, verifyPasswordResults.ErrorMessage!);
            return View(form);
        }
        List<Claim>? userClaims = await _service.BuildClaimsFor(form.Email);
        if (userClaims == null)
        {
            ModelState.AddModelError(String.Empty, "Invalid Login Credentials.");
            return View(form);
        }
        var claimsIdentity = new ClaimsIdentity(
            userClaims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity)
        );
        if (returnUrl == null)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return Redirect(returnUrl);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return View();
    }

    public IActionResult Forbidden()
    {
        return View();
    }


}