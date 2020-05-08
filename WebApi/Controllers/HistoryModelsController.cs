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
    public class HistoryModelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistoryModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HistoryModels/5
        [HttpGet("{id}")]
        public IActionResult GetHistoryModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var historyModel = _context.History.Where(a => a.AccountId == id);

            if (historyModel == null)
            {
                return NotFound();
            }

            return Ok(historyModel);
        }

    }
}