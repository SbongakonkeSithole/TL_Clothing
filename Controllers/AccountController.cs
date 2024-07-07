using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using TL_Clothing.Models;
using TL_Clothing.ViewModels;

namespace TL_Clothing.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Customer> _usermanager;
        private readonly SignInManager<Customer> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<Customer> usermanager, SignInManager<Customer> signinManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger)
        {
            _usermanager = usermanager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm model)
        {
            if (ModelState.IsValid)
            {
                var user = new Customer
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    TermsConfirmed = model.TermsConfirmed,
                    PhoneNumber=model.Phone,
                    
                };

                var result = await _usermanager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {


                    Customer customer = _usermanager.Users.FirstOrDefault(x => x.Email == user.Email)!;
                    _usermanager.AddToRoleAsync(customer, SD.Role_Customer).GetAwaiter().GetResult();
                    await _signinManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
           LoginVm loginVm = new LoginVm()
           {
               ReturnUrl = returnUrl,
           };
            return View(loginVm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm login)
        {
            await _signinManager.SignOutAsync();
            var result = await _signinManager.PasswordSignInAsync(login.Email, login.Password, false, true);

            Customer currentUnser = _usermanager.Users.FirstOrDefault(x => x.Email == login.Email)!;
            var token = _usermanager.GenerateTwoFactorTokenAsync(currentUnser, "Email");

            //SEND AN EMAIL TO THE USER
            //var apiKey = SD.SendGrid;
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("support@therentalguy.co.za", "OTP Confirmation NO-REPLY");
            //var subject = "One-time-password";
            //var to = new EmailAddress(currentUnser.Email, "Dear Administrator");
            //var plainTextContent = "";
            //// Include the password reset link in the HTML content
            //var htmlContent = $"<p>OTP : {token.Result}</p>";

            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);


            //await EmailProvider.SendEmailAsync(currentUnser.Email!, subject, htmlContent);

            if (currentUnser.TwoFactorEnabled == true)
            {
                _logger.Log(LogLevel.Warning, token.Result);
                return RedirectToAction("LogInOtp");
            }
            else
            {
                var result1 = await _signinManager.PasswordSignInAsync(login.Email, login.Password, false, true);
                if(!string.IsNullOrEmpty(login.ReturnUrl)&& Url.IsLocalUrl(login.ReturnUrl))
                {
                    if (result1.Succeeded)
                    {
                        return Redirect(login.ReturnUrl);
                    }
                }
                else
                {
                    if (result1.Succeeded)
                    {
                        if (User.IsInRole(SD.Role_Admin))
                        {
                            return RedirectToAction("Index", "Product");
                        }
                        else if (User.IsInRole(SD.Role_Customer))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                }
             
                return View();
            }
           
        }
        [HttpGet]
        [AllowAnonymous]

        public IActionResult LogInOtp()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInOtp(OneTimePinVM code)
        {
            var result = await _signinManager.TwoFactorSignInAsync("Email", code.OTP, false, false);
            if (result.Succeeded)
            {
                if (User.IsInRole(SD.Role_Admin))
                {
                    return RedirectToAction("Index", "Category");
                }
                else if (User.IsInRole(SD.Role_Customer))
                {
                    return RedirectToAction("Index", "Product");
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
