using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystemAPI.Models
{
    public class Book_Data
    {
        [Key]
        [Required]
        public string BookId { get; set; }

        [Required]
        public string BookTitle {  get; set; }

        [Required]
        public string BookAuthor { get; set; }

        [Required]
        public string BookGenre {  get; set; }
        public string BookRating { get; set; }
        public string BookImg { get; set; }
        public string BookDesc { get; set; }
        public string BookStatus { get; set; }
    }
}
