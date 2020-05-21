using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using WebApi.Services;
using WebApi.ViewModels;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WebApi.BusinessLogic
{
    public class AuthRequestHundler : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly OperationsService _service;
        public AuthRequestHundler(UserManager<ApplicationUser> userManager, IConfiguration configuration, OperationsService service)
        {
            _userManager = userManager;
            _configuration = configuration;
            _service = service;
        }

        public async Task<IActionResult> RegisterUserAsync(RegisterViewModel model)
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
        public async Task<IActionResult> LoginUserAsync(LoginViewModel model)
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
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
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
        public async Task<IActionResult> GetUser(string id)
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
        public async Task<IActionResult> MakeClient(string id)
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
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
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
        public async Task<IActionResult> UploadImage(UpdateUserModel model)
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
                return Ok(new { Message = "Файл успешно загружен", fileName = uniqueFileName });
            }
            return Ok(new { Message = "" });
        }
        public async Task<IActionResult> GetImage(string id)
        {
            var contentRoot = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            string uploadsFolder = Path.Combine(contentRoot, "images");
            string filePath = Path.Combine(uploadsFolder, id);
            var image = System.IO.File.OpenRead(filePath);

            return File(image, "image/jpeg");
        }
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                user.Status = false;
                await _userManager.UpdateAsync(user);
                _service.CloseAccounts(Guid.Parse(user.Id));

                return Unauthorized();
            }
            return NotFound();
        }
    }
}
