using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace LibrarySystemAPI.Models
{
    public class Issued_BooksAPI
    {
        [Key]
        [Required]
        public string BookStockId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string? IssueDate { get; set; }

        public string? ReturnDate { get; set; }

    }
}
