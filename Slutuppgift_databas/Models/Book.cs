using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Slutuppgift_databas.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]        
        public int ReleaseYear { get; set; }
        [Required]
        public string Isbn { get; set; }
        [Required]
        public int Rating { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
    }
}
