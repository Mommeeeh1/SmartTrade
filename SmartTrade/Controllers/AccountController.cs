using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTrade.Core.Domain.Entities;
using SmartTrade.Models.DTO;

namespace SmartTrade.Controllers;

/// <summary>
/// Controller for handling user authentication (Register, Login, Logout)
/// </summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    #region Register

    /// <summary>
    /// GET: /Account/Register
    /// Displays the registration form
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // Allow unauthenticated users to access registration
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// POST: /Account/Register
    /// Handles user registration
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Create new ApplicationUser
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            // Create user with password
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} registered successfully", model.Email);

                // Sign in the user automatically after registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect to home page or return URL if provided (prevent redirect loops)
                string? returnUrl = Request.Query["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl) && 
                    Url.IsLocalUrl(returnUrl) &&
                    !returnUrl.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase) &&
                    !returnUrl.Contains("/Account/Register", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Trade");
            }

            // If registration failed, add errors to ModelState
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
        }

        return View(model);
    }

    #endregion

    #region Login

    /// <summary>
    /// GET: /Account/Login
    /// Displays the login form
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // Prevent redirect loops - if ReturnUrl points to login/register, ignore it
        if (!string.IsNullOrEmpty(returnUrl) && 
            (returnUrl.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase) ||
             returnUrl.Contains("/Account/Register", StringComparison.OrdinalIgnoreCase)))
        {
            returnUrl = null;
        }
        
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// POST: /Account/Login
    /// Handles user login
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDTO model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Find user by email
            ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            // Attempt to sign in with password
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe, // Persistent cookie if "Remember Me" is checked
                lockoutOnFailure: true); // Lock account after failed attempts

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully", model.Email);

                // Redirect to return URL if provided and valid, but prevent redirect loops
                if (!string.IsNullOrEmpty(returnUrl) && 
                    Url.IsLocalUrl(returnUrl) &&
                    !returnUrl.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase) &&
                    !returnUrl.Contains("/Account/Register", StringComparison.OrdinalIgnoreCase))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Trade");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} account is locked out", model.Email);
                ModelState.AddModelError(string.Empty, "Your account is locked out. Please try again later.");
                return View(model);
            }

            // Login failed
            ModelState.AddModelError(string.Empty, "Invalid email or password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
        }

        return View(model);
    }

    #endregion

    #region Logout

    /// <summary>
    /// POST: /Account/Logout
    /// Handles user logout
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize] // Only authenticated users can logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Login", "Account");
    }

    #endregion
}

