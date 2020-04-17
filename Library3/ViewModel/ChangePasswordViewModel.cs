using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Library3.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string NewPassword { get; set; }
    }
}
