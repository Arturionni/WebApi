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
    public class PaymentAccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // POST: api/PaymentAccounts
        [HttpPost]
        public async Task<IActionResult> PostPaymentAccount([FromBody] PaymentAccount paymentAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var current = _context.Accounts.Where(b => b.AccountNumber == paymentAccount.AccountNumberCurrent).FirstOrDefault();
            var receiver = _context.Accounts.Where(b => b.AccountNumber == paymentAccount.AccountNumberReceiver).FirstOrDefault();
            if (current == null || receiver == null)
            {
                return NotFound();
            }
            current.AccountBalance = (float)Math.Round(current.AccountBalance + paymentAccount.Value, 2);
            receiver.AccountBalance = (float)Math.Round(receiver.AccountBalance + paymentAccount.Value, 2);
            _context.Entry(current).State = EntityState.Modified;
            _context.Entry(receiver).State = EntityState.Modified;

            var time = DateTime.Now.ToString("yyyy-MM-dd, HH:mm:ss");
            var type = "";
            if (paymentAccount.UseTemplate)
                type = "Платеж по шаблону на счет ";
            else
                type = "Платеж на счет ";
            var history = new HistoryModel
            {
                AccountId = current.Id,
                Date = time,
                Type = type + receiver.AccountNumber.ToString(),
                Value = paymentAccount.Value
            };
            var history2 = new HistoryModel
            {
                AccountId = receiver.Id,
                Date = time,
                Type = "Зачисление со счета " + current.AccountNumber.ToString(),
                Value = paymentAccount.Value
            };
            _context.Entry(history).State = EntityState.Added;
            _context.Entry(history2).State = EntityState.Added;
            await _context.SaveChangesAsync();

            return Ok(new { current, receiver });

        }
    }
}