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
    public class TemplatesModelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TemplatesModelsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/TemplatesModels/5
        [HttpGet("{id}")]
        public IActionResult GetTemplatesModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var templatesModel = _context.Templates.Where(b => b.AccountNumber == Convert.ToUInt64(id)).FirstOrDefault();

            if (templatesModel == null)
            {
                return NotFound();
            }

            return Ok(templatesModel);
        }

        // PUT: api/TemplatesModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemplatesModel([FromRoute] string id, [FromBody] TemplatesModel templatesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != templatesModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(templatesModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemplatesModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TemplatesModels
        [HttpPost]
        public async Task<IActionResult> PostTemplatesModel([FromBody] TemplatesModel templatesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Templates.Any(u => u.AccountNumber == templatesModel.AccountNumber))
                _context.Templates.Add(templatesModel);
            else
            {
                return BadRequest(new { Message = "Шаблон уже существует"});
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTemplatesModel", new { id = templatesModel.Id }, templatesModel);
        }

        // DELETE: api/TemplatesModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplatesModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var templatesModel = await _context.Templates.FindAsync(id);
            if (templatesModel == null)
            {
                return NotFound();
            }

            _context.Templates.Remove(templatesModel);
            await _context.SaveChangesAsync();

            return Ok(templatesModel);
        }

        private bool TemplatesModelExists(string id)
        {
            return _context.Templates.Any(e => e.Id == id);
        }
    }
}