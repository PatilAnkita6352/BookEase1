using System.ComponentModel.DataAnnotations;

namespace LibrarySystemAPI.Models
{
    public class Book_Stock
    {
        [Key]
        [Required]
        public string BookStockId { get; set; }

        [Required]
        public string BookId { get; set;}

        public string StockStatus { get; set;}
    }
}
