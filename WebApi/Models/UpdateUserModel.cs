using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class UpdateUserModel
    {
        [Required]
        [EmailAddress]
        public string OldEmail { get; set; }
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        public IFormFile ProfileImage { get; set; }
        public string fileName { get; set; }

    }
}
