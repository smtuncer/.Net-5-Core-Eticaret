using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ETicaret.Data;
using ETicaret.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ETicaret.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="Email boş bırakılamaz...")]
            [EmailAddress(ErrorMessage ="Geçerli bir email adresi giriniz...")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifre boş bırakılamaz...")]
            [StringLength(20, ErrorMessage = "{0} en az {2} en fazla {1} karakter olmalıdır.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Şifreler Eşleşmiyor...")]
            public string ConfirmPassword { get; set; }
            [Required(ErrorMessage = "Ad boş bırakılamaz...")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Soyad boş bırakılamaz...")]
            public string SurName { get; set; }
            public string Adres { get; set; }
            public string Sehir { get; set; }
            public string Semt { get; set; }
            public string PostaKodu { get; set; }
            public string TelefonNo { get; set; }
            public string Role { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Where(i => i.Name != Diger.Role_Birey)
                .Select(x => x.Name).Select(u=>new SelectListItem
                {
                    Text=u,
                    Value=u
                })                
            };
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Adres = Input.Adres,
                    Sehir = Input.Sehir,
                    Semt = Input.Semt,
                    Name = Input.Name,
                    SurName = Input.SurName,
                    PhoneNumber = Input.TelefonNo,
                    PostaKodu = Input.PostaKodu,
                    Role = Input.Role
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_User))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_User));
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_Birey))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_Birey));
                    }
                    if (user.Role==null)
                    {
                        await _userManager.AddToRoleAsync(user, Diger.Role_User);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role==null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
