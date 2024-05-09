using LibrarySystemAPI.DataAccessLayer;
using LibrarySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibrarySystemAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Book_StockController : ControllerBase
    {
        public LibrarySystemContext _context;
        string status = "Failure";
        string message;
        public Book_StockController(LibrarySystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStockCount()
        {
            try
            {
                var stock = _context.Book_Stock.ToList();
                if (stock.Count == 0)
                {
                    throw new Exception("No Stock Available");
                }
                return Ok(new {status= "Success",data=stock});
            }
            catch (Exception ex)
            {
                return Ok(new {status= status,message=ex.Message});
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetStock(string id)
        {
            try
            {
                var stock = (from bs in _context.Book_Stock
                            where bs.BookId == id && bs.StockStatus == "Available"
                            select new {
                                bookStockId= bs.BookStockId,
                                bookstatus = bs.StockStatus
                            }).ToList();


                if (stock.Count == 0)
                {
                    throw new Exception("No Stock Available");
                }
                return Ok(new { status = "Success", data = stock });
            }
            catch (Exception ex)
            {
                return Ok(new { status = status, message = ex.Message });
            }
        }
        [HttpPut]
        public IActionResult Countremove()
        {
            try
            {
                var stock = _context.Book_Data.Where(s => s.BookStatus == "Removed").ToList();
                if (stock.Count == 0)
                {
                    throw new Exception("No book Available");
                }
                return Ok(new { status = "Success", data = stock });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public IActionResult ActivateBook(string id)
        {
            try
            {
                var book = _context.Book_Data.Find(id);
                if (book == null)
                {
                    throw new Exception("Book is not available");
                }
                book.BookStatus = "Not Available";
                _context.SaveChanges();
                return Ok(new { status = "Success", message = "Book Activated" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }

            
        }
    }
}
