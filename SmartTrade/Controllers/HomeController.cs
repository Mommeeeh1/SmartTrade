using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartTrade.Controllers;

[AllowAnonymous] // Allow access without authentication
public class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public HomeController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [Route("/Home/Error")]
    public IActionResult Error()
    {
        // Get exception details from the exception handler feature
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        
        // Get the exception that occurred
        var exception = exceptionHandlerPathFeature?.Error;
        
        // SECURITY: Only show error details in Development, hide in Production
        if (_environment.IsDevelopment())
        {
            ViewBag.ErrorMessage = exception?.Message ?? "An error occurred while processing your request.";
            ViewBag.ShowDetails = true;
        }
        else
        {
            // In Production: Show generic message only (no details for security)
            ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
            ViewBag.ShowDetails = false;
        }
        
        return View();
    }
}