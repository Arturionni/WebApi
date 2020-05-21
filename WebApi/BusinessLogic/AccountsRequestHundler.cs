using Microsoft.AspNetCore.Mvc;
using System;
using WebApi.Services;
using WebApi.ViewModels;

namespace WebApi.BusinessLogic
{
    public class AccountsRequestHundler : ControllerBase
    {
        private readonly OperationsService _service;
        public AccountsRequestHundler(OperationsService service)
        {
            _service = service;
        }

        public IActionResult GetAccounts(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }
            var accounts = _service.GetAccounts(id);
            if (accounts == null)
            {
                return BadRequest();
            }

            return Ok(accounts);
        }
        public IActionResult CreateAccount(AccountsModel account)
        {
            Random rnd = new Random();
            ulong rndNum;

            do rndNum = 4000000000 + (ulong)rnd.Next(0, 999999999);
            while (_service.IsAccountExist(rndNum.ToString()));

            account.AccountNumber = rndNum.ToString();
            account.DateCreated = DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss");
            account.Id = Guid.NewGuid();

            if (_service.CreateAccount(account))
                return Ok(account);

            return BadRequest();
        }
        public IActionResult CloseAccount(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }
            if (_service.CloseAccount(id))
                return Ok();

            return BadRequest();
        }
    }
}
