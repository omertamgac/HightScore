﻿
using HighScore.Entities.DbContexts;
using HighScore.Entities.Model.Concrete;
using HighScore.Models;
using HighScore.Models.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HighScore.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<MetaUser> _userManager;
        private RoleManager<Role> _roleManager;
        private SignInManager<MetaUser> _signInManager;
        private IEmailSender _emailSender;
        private readonly AppDbContext _context;

        public AccountController(UserManager<MetaUser> userManager, RoleManager<Role> roleManager,
            SignInManager<MetaUser> signInManager, IEmailSender emailSender, AppDbContext context)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }



        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await _userManager.IsLockedOutAsync(user) && !await _userManager.GetLockoutEnabledAsync(user))
                    {
                        await _signInManager.SignOutAsync();
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError("", "Please confirm your account");
                            return View(model);
                        }

                        if (result.Succeeded)
                        {
                            await _userManager.ResetAccessFailedCountAsync(user);
                            await _userManager.SetLockoutEndDateAsync(user, null);

                            return RedirectToAction("Index", "Home");
                        }
                        else if (result.IsLockedOut)
                        {
                            var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                            var timeLeft = lockoutDate.Value - DateTime.UtcNow;
                            ModelState.AddModelError("", $"Your account is locked. Please try again in {timeLeft.Minutes} minute(s).");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid email or password.");
                        }
                    }
                    else if (user.EmailConfirmed == false)
                    {
                        ModelState.AddModelError("", "Please confirm your account");
                    }
                    else
                    {

                        ModelState.AddModelError("", "Your account is locked. Please contact support.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No account found for this email address.");
                }
            }
            return View(model);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new MetaUser { UserName = model.UserName, Email = model.Email };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var roleExists = await _roleManager.RoleExistsAsync("User");
                    if (!roleExists)
                    {
                        var roleResult = await _roleManager.CreateAsync(new Role { Name = "User" });
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Could not create 'User' role");
                            return View(model);
                        }
                    }

                    await _userManager.AddToRoleAsync(user, "User");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token });

                    await _emailSender.SendEmailAsync(user.Email, "Account Verify", $"Please <a href='https://localhost:7228{url}'>click here</a> to confirm your account.");

                    TempData["message"] = "Please click on the confirmation link in your email.";
                    return RedirectToAction("Login", "Account");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View();
        }




        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if (Id == null || token == null)
            {
                TempData["message"] = "Token is invalid";
                return View();
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    user.LockoutEnabled = false;
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        TempData["message"] = "Your Account Has Been Verified";
                    }
                    else
                    {
                        TempData["message"] = "An error occurred while updating the account";
                    }
                    return View();
                }
            }

            TempData["message"] = "Account has not been found";
            return View();
        }



        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {

            if (string.IsNullOrEmpty(Email))
            {
                return View();
            }


            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                TempData["message"] = "This e-mail address is not registered";
                return View();
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Account", new { user.Id, token });
            await _emailSender.SendEmailAsync(user.Email, "Password Reset", $"Please <a href='https://localhost:7228{url}'>click here</a> to reset your password.");

            TempData["message"] = "Please check your Email adress";

            return View();

        }




        public IActionResult ResetPassword(string Id, string token)
        {

            if (Id == null || token == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordVM { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    TempData["message"] = "This E-mail adress is not registered";
                    return RedirectToAction("Login");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    TempData["message"] = "Password is changed";
                    return RedirectToAction("Login");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }


            return View(model);
        }


        public IActionResult AccessDenied()
        {
            return View();
        }




    }





}
