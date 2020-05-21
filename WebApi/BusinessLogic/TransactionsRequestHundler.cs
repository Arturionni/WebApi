using Microsoft.AspNetCore.Mvc;
using System;
using WebApi.Services;
using WebApi.ViewModels;

namespace WebApi.BusinessLogic
{
    public class TransactionsRequestHundler : ControllerBase
    {
        private readonly OperationsService _service;
        public TransactionsRequestHundler(OperationsService service)
        {
            _service = service;
        }
        public IActionResult GetHistory(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }
            var history = _service.GetHistory(id);

            if (history == null)
            {
                return BadRequest();
            }

            return Ok(history);
        }
        public IActionResult MakePayment(PaymentAccount paymentAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _service.MakePayment(paymentAccount);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
        public IActionResult MakeReplenish(ReplenishAccount replenishAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _service.MakeReplenish(replenishAccount);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(new { id = result.Id, value = result.AccountBalance });
        }
        public IActionResult GetTemplate(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var template = _service.GetTemplate(id);

            if (template == null)
            {
                return NotFound();
            }
            return Ok(template);
        }
        public IActionResult UpdateTemplate(TemplatesModel templatesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_service.UpdateTemplate(templatesModel))
                return Ok();

            return BadRequest();
        }
        public IActionResult CreateTemplate(TemplatesModel templatesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var template = _service.CreateTemplate(templatesModel);
            if (template == null)
            {
                return BadRequest();
            }

            return Ok(template);
        }
        public IActionResult MakeTransfer(TransferAccount transferAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _service.MakeTransfer(transferAccount);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
