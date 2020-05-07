using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.ViewModels;

namespace WebApi.Data
{
    public class DummyData
    {
        public static void Initialize(IApplicationBuilder app, UserManager<ApplicationUser> userManager)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                if (context.Accounts != null && context.Accounts.Any())
                    return;

                var accounts = GetAccounts().ToArray();
                context.Accounts.AddRange(accounts);

                if (!context.Users.Any(u => u.UserName == "Owner"))
                {
                    var user = new ApplicationUser
                    {
                        Id = "d92b2f64-059f-4b7f-9db2-e5cada28507b",
                        FirstName = "Artur",
                        LastName = "Kalimgulov",
                        Email = "kalimgulovartur@gmail.com",
                        UserName = "Owner",
                        PhoneNumber = "+111111111111",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        LockoutEnabled = true,
                    };
                    var result = userManager.CreateAsync(user, "123123");
                    if (result.IsCompletedSuccessfully)
                    {
                        userManager.AddToRoleAsync(user, "Admin");
                    }
                }
                context.SaveChanges();

            }
        }

        public static List<AccountsModel> GetAccounts()
        {
            List<AccountsModel> accounts = new List<AccountsModel>() {
                new AccountsModel {
                    Id="2ff8895b-db68-4e3c-b57b-8c07dc9dd12b",
                    AccountBalance=123,
                    AccountNumber=4000000001,
                    UserId="d92b2f64-059f-4b7f-9db2-e5cada28507b"
                },
                new AccountsModel {
                    Id="af3f310b-04c4-4d0a-8043-0ff457592679",
                    AccountBalance=123.5F,
                    AccountNumber=4000000002,
                    UserId="d92b2f64-059f-4b7f-9db2-e5cada28507b"
                },
                new AccountsModel {
                    Id="f62ba951-8cb5-4587-aef1-073a37fc10ea",
                    AccountBalance=56465,
                    AccountNumber=4000000003,
                    UserId="d92b2f64-059f-4b7f-9db2-e5cada28507b"
                },
            };
            return accounts;
        }
    }
}
