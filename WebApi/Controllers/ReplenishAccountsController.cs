using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpPost]
        public async Task<IActionResult> PostReplenishAccount([FromBody] ReplenishAccount replenishAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = _context.Accounts.Where(b => b.AccountNumber == replenishAccount.AccountNumber).FirstOrDefault();
            account.AccountBalance += replenishAccount.Value;
            _context.Entry(account).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(account);
        }
    }
}