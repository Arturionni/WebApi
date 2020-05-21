using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.BusinessLogic;
using WebApi.Data;
using WebApi.Services;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferAccountsController : ControllerBase
    {
        private readonly TransactionsRequestHundler _transactionsRequestHundler;

        public TransferAccountsController(TransactionsRequestHundler transactionsRequestHundler)
        {
            _transactionsRequestHundler = transactionsRequestHundler;
        }
        // POST: api/TransferAccounts
        [Authorize]
        [HttpPost]
        public IActionResult PostTransferAccount([FromBody] TransferAccount transferAccount)
        {
            return _transactionsRequestHundler.MakeTransfer(transferAccount);
        }
    }
}