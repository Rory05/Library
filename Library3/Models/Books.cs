using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library3
{
    public partial class Books
    {
        public Books()
        {
            BookAuthors = new HashSet<BookAuthors>();
            BookGenres = new HashSet<BookGenres>();
            Libraries = new HashSet<Libraries>();
        }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Назва")]
        public string Name { get; set; }
        public int Id { get; set; }
        [Display(Name = "К-ть сторінок")]
        public int? PagesNum { get; set; }
        [Display(Name = "Мова")]
        public int? LanguageId { get; set; }
        
        [Display(Name = "Мова")]
        public virtual Language Language { get; set; }
        [Display(Name = "Опис")]
        public string Description { get; set; }
        [Display(Name = "Автор")]
        public virtual ICollection<BookAuthors> BookAuthors { get; set; }
        [Display(Name = "Жанр")]
        public virtual ICollection<BookGenres> BookGenres { get; set; }
        public virtual ICollection<Libraries> Libraries { get; set; }
    }
}
