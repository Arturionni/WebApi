using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.BusinessLogic;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthRequestHundler _authRequestHundler;

        public AuthController(AuthRequestHundler authRequestHundler)
        {
            _authRequestHundler = authRequestHundler;
        }

        // /register
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> InsertUserAsync([FromBody] RegisterViewModel model)
        {
            return await _authRequestHundler.RegisterUserAsync(model);
        }

        [Route("login")] // /login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            return await _authRequestHundler.LoginUserAsync(model);
        }

        [Route("changePassword")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> changePassword([FromBody] ChangePasswordModel model)
        {
            return await _authRequestHundler.ChangePassword(model);
        }

        [Authorize]
        [HttpGet("getUser/{id}")]
        public async Task<IActionResult> getUser([FromRoute] string id)
        {
            return await _authRequestHundler.GetUser(id);
        }

        [Authorize]
        [HttpGet("makeClient/{id}")]
        public async Task<IActionResult> makeClient([FromRoute] string id)
        {
            return await _authRequestHundler.MakeClient(id);
        }

        [Route("updateUser")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> updateUser([FromBody] UpdateUserModel model)
        {
            return await _authRequestHundler.UpdateUser(model);
        }
        [Route("uploadImage")]
        [HttpPost]
        public async Task<IActionResult> uploadImageAsync([FromForm] UpdateUserModel model)
        {
            return await _authRequestHundler.UploadImage(model);
        }

        [HttpGet("getImage/{id}")]
        public async Task<IActionResult> GetImage([FromRoute] string id)
        {
            return await _authRequestHundler.GetImage(id);
        }

        [Authorize]
        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> deleteUserAsync([FromRoute] Guid id)
        {
            return await _authRequestHundler.DeleteUser(id);
        }
    }
}