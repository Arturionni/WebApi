using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsModelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountsModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AccountsModels
        [Authorize]
        [HttpGet]
        public IEnumerable<AccountsModel> GetAccounts()
        {
            return _context.Accounts;
        }

        // GET: api/AccountsModels/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetAccountsModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountsModel = _context.Accounts.Where(b => b.UserId == id && b.Status == true).OrderBy(b => b.DateCreated);

            if (accountsModel == null)
            {
                return NotFound();
            }

            return Ok(accountsModel);
        }


        // POST: api/AccountsModels
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAccountsModel([FromBody] AccountsModel accountsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            accountsModel.Id = Guid.NewGuid().ToString();
            accountsModel.Status = true;
            accountsModel.AccountBalance = 0;
            accountsModel.DateCreated = DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss");
            Random rnd = new Random();
            var rndNum = (ulong)0;
            do rndNum = 4000000000 + (ulong)rnd.Next(0, 999999999);
                while (_context.Accounts.Any(u => u.AccountNumber == rndNum));
            accountsModel.AccountNumber = rndNum; 
            _context.Accounts.Add(accountsModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccountsModel", new { id = accountsModel.Id }, accountsModel);
        }

        // DELETE: api/AccountsModels/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountsModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountsModel = await _context.Accounts.FindAsync(id);
            if (accountsModel == null)
            {
                return NotFound();
            }
            accountsModel.Status = false;
            _context.Entry(accountsModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(accountsModel);
        }
    }
}
