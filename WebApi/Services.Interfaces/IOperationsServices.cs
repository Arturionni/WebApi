using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.ViewModels;

namespace WebApi.Services.Interfaces
{
    public interface IOperationsService
    {
        AccountsModel MakeReplenish(ReplenishAccount replenish);
        object MakeTransfer(TransferAccount transfer);
        object MakePayment(PaymentAccount payment);
        bool CreateAccount(AccountsModel account);
        bool CloseAccount(Guid id);
        bool CloseAccounts(Guid id);
        bool IsAccountExist(string accountNumber);
        IEnumerable GetAccounts(Guid id);
        IEnumerable GetHistory(Guid id);
        TemplatesModel GetTemplate(string id);
        TemplatesModel CreateTemplate(TemplatesModel template);
        bool UpdateTemplate(TemplatesModel template);

    }
}
