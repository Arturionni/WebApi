using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using WebApi.Data;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
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
                isClient = false,
                Status = true,
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
            if (user == null)
                return BadRequest(new { Message = "Такого пользователя не существует в системе" });
            if (await _userManager.CheckPasswordAsync(user, model.Password))
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
            else return BadRequest(new { Message = "Неверный логин или пароль" });
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
            return BadRequest(new { Message = "Неверный пароль" });
        }

        [Authorize]
        [HttpGet("getUser/{id}")]
        public async Task<ActionResult> getUser([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var profilePicture = "";
                if (String.IsNullOrEmpty(user.fileName))
                    profilePicture = null;
                else profilePicture = "getImage/" + user.fileName;
                return Ok(
                    new
                    {
                        firstName = user.FirstName,
                        email = user.Email,
                        profilePicture,
                        user.isClient,
                        user.Status
                    }
                );
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("makeClient/{id}")]
        public async Task<ActionResult> makeClient([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.isClient = true;
                await _userManager.UpdateAsync(user);
                return Ok(new { message = "Пользователь успешно стал клиентом" });
            }
            return BadRequest();
        }

        [Route("updateUser")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> updateUser([FromBody] UpdateUserModel model)
        {
            var user = await _userManager.FindByNameAsync(model.OldEmail);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (!String.IsNullOrEmpty(model.NewEmail))
                {
                    user.Email = model.NewEmail;
                    user.UserName = model.NewEmail;
                    user.NormalizedEmail = model.NewEmail;
                    user.NormalizedUserName = model.NewEmail;
                }
                if (!String.IsNullOrEmpty(model.FirstName))
                    user.FirstName = model.FirstName;
                if (!String.IsNullOrEmpty(model.fileName))
                {
                    user.fileName = model.fileName;
                }
                    await _userManager.UpdateAsync(user);
                return Ok(
                    new
                    {
                        Message = "Данные пользователя успешно изменены"
                    });
            }
            return BadRequest(new { Message = "Неверный пароль" });
        }
        [Route("uploadImage")]
        [HttpPost]
        public ActionResult uploadImage([FromForm] UpdateUserModel model)
        {
            string uniqueFileName;
            if (model.ProfileImage != null)
            {
                var contentRoot = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
                string uploadsFolder = Path.Combine(contentRoot, "images");
                uniqueFileName = Guid.NewGuid().ToString();
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
                return Ok(new { Message = "Файл успешно загружен", fileName = uniqueFileName});
            }
            return Ok(new { Message = "" });
        }

        [HttpGet("getImage/{id}")]
        public IActionResult GetImage([FromRoute] string id)
        {
            var contentRoot = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            string uploadsFolder = Path.Combine(contentRoot, "images");
            string filePath = Path.Combine(uploadsFolder, id);
            var image = System.IO.File.OpenRead(filePath);

            return File(image, "image/jpeg");
        }

        [Authorize]
        [HttpDelete("deleteUser/{id}")]
        public async Task<ActionResult> deleteUserAsync([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Status = false;
                await _userManager.UpdateAsync(user);
                var accountsToClose = _context.Accounts.Where(b => b.UserId == id && b.Status == true);
                foreach (AccountsModel account in accountsToClose)
                {
                    account.Status = false;
                    _context.Entry(account).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                return Unauthorized();
            }
            return NotFound();
        }
    }
}