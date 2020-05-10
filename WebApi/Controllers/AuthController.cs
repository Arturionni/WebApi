using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // /register
        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> InsertUser([FromBody] RegisterViewModel model)
        {
            if (_userManager.Users.Where(e => e.Email == model.Email).FirstOrDefault() != null)
                return BadRequest(new { Message = "Такой Email уже существует" });
            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            return Ok(new { Username = user.UserName });
        }

        [Route("login")] // /login
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var signinKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                var token = new JwtSecurityToken(
                  issuer: _configuration["Jwt:Site"],
                  audience: _configuration["Jwt:Site"],
                  expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                  signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(
                  new
                  {
                      token = new JwtSecurityTokenHandler().WriteToken(token),
                      userId = user.Id
                  });
            }
            return Unauthorized();
        }

        [Route("changePassword")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> changePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result != null)
                    return Ok(
                        new
                        {
                            Message = "Пароль успешно изменен"
                        });
                return BadRequest();
            }
            return BadRequest(new { Message = "Неверный пароль"});
        }

        [Route("updateUser")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> updateUser([FromBody] UpdateUserModel model)
        {
            var user = await _userManager.FindByNameAsync(model.OldEmail);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                user.Email = model.NewEmail;
                user.FirstName = model.FirstName;

                await _userManager.UpdateAsync(user);
                return Ok(
                    new
                    {
                        Message = "Данные пользователя успешно изменены"
                    });
            }
            return BadRequest(new { Message = "Неверный пароль" });
        }
    }
}