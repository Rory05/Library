using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library3
{
    public partial class Genres
    {
        public Genres()
        {
            BookGenres = new HashSet<BookGenres>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Назва")]
        public string Name { get; set; }
        [Display(Name = "Інформація")]
        public string Information { get; set; }

        public virtual ICollection<BookGenres> BookGenres { get; set; }
    }
}
