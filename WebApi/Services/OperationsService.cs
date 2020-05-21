using Dapper;
using Npgsql;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using WebApi.Services.Interfaces;
using WebApi.ViewModels;

namespace WebApi.Services
{
    public class OperationsService : IOperationsService
    {
        private string connString;
        public OperationsService(string connString)
        {
            this.connString = connString;
        }

        public bool CloseAccounts(Guid id)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();

                    conn.Execute("UPDATE public.\"Accounts\" SET \"Status\" = false WHERE \"UserId\" = @id",
                        new
                        {
                            id
                        });
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        
        public TemplatesModel CreateTemplate(TemplatesModel template)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    string sql = "INSERT INTO public.\"Templates\" VALUES (@Id ,@AccountNumberCurrent, @AccountNumberReceiver, @PaymentName, @ReceiverName, @ReceiverEmail, @PaymentPurpose, @PaymentValue);";
                    conn.Execute(sql,
                        new
                        {
                            Id = Guid.NewGuid(),
                            template.AccountNumberCurrent,
                            template.AccountNumberReceiver,
                            template.PaymentName,
                            template.PaymentPurpose,
                            template.PaymentValue,
                            template.ReceiverEmail,
                            template.ReceiverName
                        });
                    return conn.QueryFirstOrDefault<TemplatesModel>("SELECT * FROM public.\"Templates\" WHERE \"AccountNumberCurrent\" = @AccountNumberCurrent;", new { template.AccountNumberCurrent });
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public bool UpdateTemplate(TemplatesModel template)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    if (conn.ExecuteScalar<bool>("SELECT COUNT(1) FROM public.\"Templates\" WHERE \"Id\" = @Id", new { template.Id })) 
                    {
                        string sql = "UPDATE public.\"Templates\" SET \"AccountNumberCurrent\" = @AccountNumberCurrent, \"AccountNumberReceiver\" = @AccountNumberReceiver, \"PaymentName\" = @PaymentName, \"PaymentPurpose\" = @PaymentPurpose, \"PaymentValue\" = @PaymentValue, \"ReceiverEmail\" = @ReceiverEmail, \"ReceiverName\" = @ReceiverName WHERE \"Id\" = @Id";
                        conn.Execute(sql,
                            new
                            {
                                template.AccountNumberCurrent,
                                template.AccountNumberReceiver,
                                template.Id,
                                template.PaymentName,
                                template.PaymentPurpose,
                                template.PaymentValue,
                                template.ReceiverEmail,
                                template.ReceiverName
                            });
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public TemplatesModel GetTemplate(string accountNumber)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM public.\"Templates\" WHERE \"AccountNumberCurrent\" = @accountNumber;";
                    
                    return conn.QueryFirstOrDefault<TemplatesModel>(sql, new { accountNumber });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public object MakePayment(PaymentAccount payment)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();

                    var current = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberCurrent;", new { payment.AccountNumberCurrent });
                    var receiver = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberReceiver;", new { payment.AccountNumberReceiver });

                    if (current != null && receiver != null)
                    {
                        var sqlUpdate = "UPDATE public.\"Accounts\" SET \"AccountBalance\" = @balance WHERE \"Id\" = @Id;";
                        var sqlInsert = "INSERT INTO public.\"History\" VALUES(@Id, @AccountId, @Type, @Date, @Value);";
                        var time = DateTime.Now.ToString("yyyy.MM.dd, HH:mm:ss");
                        var type = "";
                        if (payment.UseTemplate)
                            type = "Платеж по шаблону на счет ";
                        else
                            type = "Платеж на счет ";

                        conn.Execute(sqlUpdate,
                        new
                        {
                            current.Id,
                            balance = current.AccountBalance - payment.Value
                        });
                        conn.Execute(sqlUpdate,
                            new
                            {
                                receiver.Id,
                                balance = receiver.AccountBalance + payment.Value
                            });
                        conn.Execute(sqlInsert,
                            new
                            {
                                Id = Guid.NewGuid(),
                                AccountId = current.Id,
                                Type = type + receiver.AccountNumber.ToString(),
                                Date = time,
                                payment.Value
                            });
                        conn.Execute(sqlInsert,
                            new
                            {
                                Id = Guid.NewGuid(),
                                AccountId = receiver.Id,
                                Type = "Зачисление со счета " + current.AccountNumber.ToString(),
                                Date = time,
                                payment.Value
                            });

                        current = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberCurrent;", new { payment.AccountNumberCurrent });
                        receiver = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberReceiver;", new { payment.AccountNumberReceiver });

                        return new { current, receiver };
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public object MakeTransfer(TransferAccount transfer)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();

                    var current = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberCurrent;", new { transfer.AccountNumberCurrent });
                    var receiver = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberReceiver;", new { transfer.AccountNumberReceiver });

                    if (current != null && receiver != null)
                    {
                        conn.Execute("UPDATE public.\"Accounts\" SET \"AccountBalance\" = @balance WHERE \"Id\" = @Id;",
                        new
                        {
                            current.Id,
                            balance = current.AccountBalance - transfer.Value
                        });
                        conn.Execute("UPDATE public.\"Accounts\" SET \"AccountBalance\" = @balance WHERE \"Id\" = @Id;",
                            new
                            {
                                receiver.Id,
                                balance = receiver.AccountBalance + transfer.Value
                            });
                        var time = DateTime.Now.ToString("yyyy.MM.dd, HH:mm:ss");
                        conn.Execute("INSERT INTO public.\"History\" VALUES(@Id, @AccountId, @Type, @Date, @Value);",
                            new
                            {
                                Id = Guid.NewGuid(),
                                AccountId = current.Id,
                                Type = "Перевод средств на счет " + receiver.AccountNumber.ToString(),
                                Date = time,
                                transfer.Value
                            });
                        conn.Execute("INSERT INTO public.\"History\" VALUES(@Id, @AccountId, @Type, @Date, @Value);",
                            new
                            {
                                Id = Guid.NewGuid(),
                                AccountId = receiver.Id,
                                Type = "Зачисление со счета " + current.AccountNumber.ToString(),
                                Date = time,
                                transfer.Value
                            });

                        current = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberCurrent;", new { transfer.AccountNumberCurrent });
                        receiver = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumberReceiver;", new { transfer.AccountNumberReceiver });

                        return new { current, receiver };
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public IEnumerable GetHistory(Guid id)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM public.\"History\" WHERE \"AccountId\" = @id;";

                    using (var multi = conn.QueryMultiple(sql, new { id }))
                    {
                        return multi.Read<HistoryModel>().ToList();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public bool IsAccountExist(string accountNumber)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    
                    var result = conn.ExecuteScalar<bool>("SELECT COUNT(1) FROM public.\"Accounts\" WHERE \"AccountNumber\" = @accountNumber",
                        new
                        {
                            accountNumber
                        });
                    return result;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public bool CloseAccount(Guid id)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();

                    conn.Execute("UPDATE public.\"Accounts\" SET \"Status\" = false WHERE \"Id\" = @id",
                        new
                        {
                            id
                        });
                    return true;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public bool CreateAccount(AccountsModel account)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();

                    conn.Execute("INSERT INTO public.\"Accounts\" VALUES(@Id, @AccountNumber, 0, @DateCreated, @UserId, true);",
                        new
                        {
                            account.Id,
                            account.AccountNumber,
                            account.DateCreated,
                            account.UserId
                        });
                    return true;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public IEnumerable GetAccounts(Guid id)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM public.\"Accounts\" WHERE \"UserId\" = @id AND \"Status\" = true ORDER BY \"DateCreated\";";

                    using (var multi = conn.QueryMultiple(sql, new { id }))
                    {
                        return multi.Read<AccountsModel>().ToList();
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public AccountsModel MakeReplenish(ReplenishAccount replenish)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();

                    var account = conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumber;", 
                        new 
                        { 
                            replenish.AccountNumber
                        });
                    conn.Execute("UPDATE public.\"Accounts\" SET \"AccountBalance\" = @balance WHERE \"Id\" = @Id;",
                        new
                        {
                            account.Id,
                            balance = account.AccountBalance + replenish.Value
                        });
                    conn.Execute("INSERT INTO public.\"History\" VALUES(@Id, @AccountId, @Type, @Date, @Value);",
                        new
                        {
                            Id = Guid.NewGuid(),
                            AccountId = account.Id,
                            Type = "Пополнение",
                            Date = DateTime.Now.ToString("yyyy.MM.dd, HH:mm:ss"),
                            replenish.Value
                        });
                    return conn.QueryFirstOrDefault<AccountsModel>("SELECT * FROM public.\"Accounts\" WHERE \"AccountNumber\" = @AccountNumber;", 
                        new 
                        { 
                            replenish.AccountNumber 
                        });
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        
        NpgsqlConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(connString);
            return connection;
        }
    }
}
