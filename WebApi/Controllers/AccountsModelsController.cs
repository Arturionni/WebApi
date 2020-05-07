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
        [HttpGet]
        public IEnumerable<AccountsModel> GetAccounts()
        {
            return _context.Accounts;
        }

        // GET: api/AccountsModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountsModel([FromRoute] string id)
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

            return Ok(accountsModel);
        }


        // POST: api/AccountsModels
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
            Random rnd = new Random();
            var rndNum = (ulong)rnd.Next(0, 999999999);
            accountsModel.AccountNumber = 4000000000 + rndNum; 
            _context.Accounts.Add(accountsModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccountsModel", new { id = accountsModel.Id }, accountsModel);
        }

        // DELETE: api/AccountsModels/5
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

            _context.Accounts.Remove(accountsModel);
            await _context.SaveChangesAsync();

            return Ok(accountsModel);
        }
    }
}
