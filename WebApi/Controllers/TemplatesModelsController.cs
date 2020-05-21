using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.BusinessLogic;
using WebApi.Services;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplatesModelsController : ControllerBase
    {
        private readonly TransactionsRequestHundler _transactionsRequestHundler;

        public TemplatesModelsController(TransactionsRequestHundler transactionsRequestHundler)
        {
            _transactionsRequestHundler = transactionsRequestHundler;
        }
        // GET: api/TemplatesModels/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetTemplatesModel([FromRoute] string id)
        {
            return _transactionsRequestHundler.GetTemplate(id);
        }

        // PUT: api/TemplatesModels/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult PutTemplatesModel([FromRoute] Guid id, [FromBody] TemplatesModel templatesModel)
        {
            return _transactionsRequestHundler.UpdateTemplate(templatesModel);
        }

        // POST: api/TemplatesModels
        [Authorize]
        [HttpPost]
        public IActionResult PostTemplatesModel([FromBody] TemplatesModel templatesModel)
        {
            return _transactionsRequestHundler.CreateTemplate(templatesModel);
        }

    }
}