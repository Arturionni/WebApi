using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ReplenishAccountsController : ControllerBase
    {
        private readonly TransactionsRequestHundler _transactionsRequestHundler;

        public ReplenishAccountsController(TransactionsRequestHundler transactionsRequestHundler)
        {
            _transactionsRequestHundler = transactionsRequestHundler;
        }

        // POST: api/ReplenishAccounts
        [Authorize]
        [HttpPost]
        public IActionResult PostReplenishAccount([FromBody] ReplenishAccount replenishAccount)
        {
            return _transactionsRequestHundler.MakeReplenish(replenishAccount);
        }
    }
}