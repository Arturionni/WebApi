using Microsoft.AspNetCore.Mvc;
using WebApi.BusinessLogic;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentAccountsController : ControllerBase
    {
        private readonly TransactionsRequestHundler _transactionsRequestHundler;

        public PaymentAccountsController(TransactionsRequestHundler transactionsRequestHundler)
        {
             _transactionsRequestHundler = transactionsRequestHundler;
        }

        // POST: api/PaymentAccounts
        [HttpPost]
        public IActionResult PostPaymentAccount([FromBody] PaymentAccount paymentAccount)
        {
            return _transactionsRequestHundler.MakePayment(paymentAccount);
        }
    }
}