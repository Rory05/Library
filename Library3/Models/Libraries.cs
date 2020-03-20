using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library3
{
    public partial class Libraries
    {
        public int BookId { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        public string LibName { get; set; }
        public int Id { get; set; }

        public virtual Books Book { get; set; }
    }
}
