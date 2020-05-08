using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReplenishAccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReplenishAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // POST: api/ReplenishAccounts
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostReplenishAccount([FromBody] ReplenishAccount replenishAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = _context.Accounts.Where(b => b.AccountNumber == replenishAccount.AccountNumber).FirstOrDefault();
            if (account == null)
            {
                return NotFound();
            }
            account.AccountBalance = (float)Math.Round(account.AccountBalance + replenishAccount.Value, 2);
            _context.Entry(account).State = EntityState.Modified;

            var history = new HistoryModel
            {
                AccountId = account.Id,
                Date = DateTime.Now.ToString("yyyy-MM-dd, HH:mm:ss"),
                Type = "Пополнение",
                Value = replenishAccount.Value
            };
            _context.Entry(history).State = EntityState.Added;
            await _context.SaveChangesAsync();

            return Ok(account);
        }
    }
}