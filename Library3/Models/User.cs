using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Library3.Models
{
    public class User: IdentityUser
    {
        [Display(Name = "Ім'я")]
        public string Name { get; set; }
       
    }
}
