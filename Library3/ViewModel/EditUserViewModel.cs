using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Library3.ViewModel
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [Display(Name = "Ім'я")]
        public string Name { get; set; }
    }
}
