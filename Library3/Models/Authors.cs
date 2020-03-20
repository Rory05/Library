using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library3
{
    public partial class Authors
    {
        public Authors()
        {
            BookAuthors = new HashSet<BookAuthors>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage ="Поле не повинно бути порожнім")]
        [Display(Name = "Ім'я")]
        public string AuthorName { get; set; }
        [Display(Name = "Дата")]
        public DateTime? Date { get; set; }

        public virtual ICollection<BookAuthors> BookAuthors { get; set; }
    }
}
