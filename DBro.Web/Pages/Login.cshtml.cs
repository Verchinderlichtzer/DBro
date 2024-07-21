using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace DBro.Web.Pages;

public class LoginModel(IAuthService AuthService) : PageModel
{
    [BindProperty] public LoginDTO LoginDTO { get; set; } = new();

    [TempData] public string Notif { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string returnUrl = null!)
    {
        if (User.Identity!.IsAuthenticated)
            return LocalRedirect(Url.Content("~/"));
        if (!string.IsNullOrEmpty(Notif))
            ModelState.AddModelError(string.Empty, Notif);

        returnUrl ??= Url.Content("~/");

        //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await HttpContext.SignOutAsync("Cookies");

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var tokenClaimsDTO = await AuthService.LoginAsync(LoginDTO);

        if (!string.IsNullOrEmpty(tokenClaimsDTO?.Token))
        {
            AuthenticationProperties authenticationProperties = new()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                IsPersistent = true
            };

            ClaimsIdentity identity = new(tokenClaimsDTO.Claims.Select(x => new Claim(x.Type, x.Value)), "Cookies");
            ClaimsPrincipal principal = new(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            Response.Cookies.Append("api_token", tokenClaimsDTO.Token, new()
            {
                Expires = DateTimeOffset.UtcNow.AddDays(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return LocalRedirect("/");
        }
        return Page();
    }

    /*
     public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var token = await AuthService.LoginAsync(LoginDTO);

        if (!string.IsNullOrEmpty(token.Token))
        {
            //AuthenticationProperties authenticationProperties = new()
            //{
            //    AllowRefresh = true,
            //    ExpiresUtc = DateTimeOffset.Now.AddDays(1),
            //    IsPersistent = true
            //};

            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, "Aku"),
                new(ClaimTypes.Name, "Kamu"),
                new(ClaimTypes.Role, "Anda")
            ];

            ClaimsIdentity identity = new(token.Claims.Select(x => new Claim(x.Type, x.Value)), "Cookies");
            ClaimsPrincipal principal = new(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            Response.Cookies.Append("api_token", token.Token, new()
            {
                Expires = DateTimeOffset.UtcNow.AddDays(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return LocalRedirect("/");
        }
        return Page();
    }
     */
}