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
    public class TransferAccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransferAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/TransferAccounts
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostTransferAccount([FromBody] TransferAccount transferAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var current = _context.Accounts.Where(b => b.AccountNumber == transferAccount.AccountNumberCurrent).FirstOrDefault();
            var receiver = _context.Accounts.Where(b => b.AccountNumber == transferAccount.AccountNumberReceiver).FirstOrDefault();
            if (current == null || receiver == null)
            {
                return NotFound();
            }
            current.AccountBalance = (float)Math.Round(current.AccountBalance + transferAccount.Value, 2);
            receiver.AccountBalance = (float)Math.Round(receiver.AccountBalance + transferAccount.Value, 2);
            _context.Entry(current).State = EntityState.Modified;
            _context.Entry(receiver).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { current, receiver });
        }




    }
}