using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystemAPI.Models
{
    public class Issued_Books
    {
        [Key]
        public int IssueId { get; set; }

        [Required]
        public string BookStockId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateOnly IssueDate { get; set; }

        public DateOnly ReturnDate { get; set; }

      
        public string? BookIStatus { get; set; }
    }
}
