using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library3
{
    public partial class Language
    {
        public Language()
        {
            Books = new HashSet<Books>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string Lname { get; set; }

        public virtual ICollection<Books> Books { get; set; }
    }
}
