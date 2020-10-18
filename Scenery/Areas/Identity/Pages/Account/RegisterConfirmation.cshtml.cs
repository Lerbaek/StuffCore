using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Scenery.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class RegisterConfirmationModel : PageModel
  {
    private readonly UserManager<IdentityUser> userManager;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
    private readonly IEmailSender sender;

    public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender)
    {
      this.userManager = userManager;
      this.sender = sender;
      DisplayConfirmAccountLink = false;
    }

    public string Email { get; set; }

    public bool DisplayConfirmAccountLink { get; set; }

    public string EmailConfirmationUrl { get; set; }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
    {
      if (email == null)
        return RedirectToPage("/Index");

      var user = await userManager.FindByEmailAsync(email);
      if (user == null)
        return NotFound($"Unable to load user with email '{email}'.");

      Email = email;
      // ReSharper disable once InvertIf
      if (DisplayConfirmAccountLink)
      {
        var userId = await userManager.GetUserIdAsync(user);
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string area = "Identity";
        EmailConfirmationUrl = Url.Page(
                               "/Account/ConfirmEmail",
                               null,
                               new { area, userId, code, returnUrl },
                               Request.Scheme);
      }

      return Page();
    }
  }
}
