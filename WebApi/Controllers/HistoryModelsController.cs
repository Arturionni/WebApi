using System;
using Microsoft.AspNetCore.Mvc;
using WebApi.BusinessLogic;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryModelsController : ControllerBase
    {
        private readonly TransactionsRequestHundler _transactionsRequestHundler;

        public HistoryModelsController(TransactionsRequestHundler transactionsRequestHundler)
        {
            _transactionsRequestHundler = transactionsRequestHundler;
        }

        // GET: api/HistoryModels/5
        [HttpGet("{id}")]
        public IActionResult GetHistoryModel([FromRoute] Guid id)
        {
            return _transactionsRequestHundler.GetHistory(id);
        }

    }
}