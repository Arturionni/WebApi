using System;
using Microsoft.AspNetCore.Mvc;
using WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using WebApi.BusinessLogic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsModelsController : ControllerBase
    {
        private readonly AccountsRequestHundler _accountsModelsRequestHundler;
        public AccountsModelsController(AccountsRequestHundler accountsModelsRequestHundler)
        {
            _accountsModelsRequestHundler = accountsModelsRequestHundler;
        }

        // GET: api/AccountsModels/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetAccountsModel([FromRoute] Guid id)
        {
            return _accountsModelsRequestHundler.GetAccounts(id);
        }

        // POST: api/AccountsModels
        [Authorize]
        [HttpPost]
        public IActionResult PostAccountsModel([FromBody] AccountsModel accountsModel)
        {
            return _accountsModelsRequestHundler.CreateAccount(accountsModel);
        }

        // DELETE: api/AccountsModels/5
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteAccountsModel([FromRoute] Guid id)
        {
            return _accountsModelsRequestHundler.CloseAccount(id);
        }
    }
}
