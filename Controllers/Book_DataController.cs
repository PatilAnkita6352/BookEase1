using LibrarySystemAPI.DataAccessLayer;
using LibrarySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace LibrarySystemAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    
    public class Book_DataController : ControllerBase
    {
        string status = "Failure";
        string message;
        private LibrarySystemContext _context;

        public Book_DataController(LibrarySystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var book = _context.Book_Data.Where(s=>s.BookStatus != "Removed").ToList();
                if (book == null)
                {
                    throw new Exception("No Book Available");
                }
               
                    foreach (var b in book)
                {
                    var cnt = _context.Book_Stock.Where(s => s.BookId == b.BookId && s.StockStatus== "Available").Count();
                    if (cnt == 0) b.BookStatus = "Not Available";
                    else { b.BookStatus = "Available"; }
                }
                return Ok(new { status = "Success", data = book });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "Failure";
                return Ok(new { status = status, message = message });
            }
           
        }
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var book = _context.Book_Data.Where(s=> s.BookId == id && s.BookStatus!= "Removed");
                if (book == null)
                {
                    throw new Exception("No Book With This ID");
                    
                }
                return Ok(new { status = "Success", data = book });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Ok(new { status = status, message = message });
            }
        }
        [HttpGet("{title}/{genre}")]
        public IActionResult GetBookBySearch(string title, string genre)
        {
            try
            {
                var book = _context.Book_Data.Where(s => s.BookTitle.Contains(title) || s.BookGenre.Contains(genre)).ToList();
                if (book.Count == 0)
                {
                    throw new Exception("Book Not available");
                }
                return Ok(new {status="Success", data=book});
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "Failure";
                return Ok(new { status = status, message = message });
            }

        }



        [HttpPost]
        public IActionResult AddBook(Book_Data model)
        {
            try
            {
                var bo= _context.Book_Data.Where(s=> s.BookId==model.BookId).ToList().Count;
                var bok = _context.Book_Data.Where(s => s.BookTitle == model.BookTitle).ToList().Count;
                if (bo>0)
                {
                    throw new Exception("This Book ID Already Exist");
                }
                if(bok>0)
                {
                    throw new Exception("This Book Name Already Exist");
                }
                _context.Add(model);
                _context.SaveChanges();
                return Ok(new { status = "Success", message = "Book added successfully" });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "Failure";
                return Ok(new { status = status, message = message });
            }
        }

        [HttpPost("{Book}")]
        public IActionResult AddBookStock(string Book, int cnt)
        {
            try
            {
                
                var c = _context.Book_Data.Where(s=>s.BookId==Book).ToList().Count;
                if (c == 0)
                {
                    throw new Exception("No Book with this id available");
                }
                var pre = _context.Book_Stock.Where(s => s.BookId == Book);
                int p = 1;
                if (pre.Count() > 0)
                {
                    p = pre.Count() + 1;
                }
            
               
                for (int i = p; i < cnt+p; i++)
                {
                    var a = Book + "St" + i;
                    Book_Stock stockmodel = new Book_Stock();
                    {
                        stockmodel.BookId = Book;
                        stockmodel.BookStockId = a;
                        stockmodel.StockStatus = "Available";
                    };
                    
                    _context.Book_Stock.Add(stockmodel);
                    
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "Failure";
                return Ok(new { status = status, message = message });
            }
            return Ok(new { status = "Success", message = "Book Stock added successfully" });
         
        }

        [HttpPut("{id}")]
        public IActionResult DeactivateBook(string id) {
            try
            {
                var book = _context.Book_Data.Find(id);
                if (book == null)
                {
                    throw new Exception($"No Book with id {id}");

                }
                var stocks = (from m in _context.Book_Stock where m.BookId == id
                             select new
                             {
                                 bookStockId = m.BookStockId,
                                 bookStatus = m.StockStatus
                             }).ToList();
                
                for(int i=0;i<stocks.Count;i++)
                {
                    if (stocks[i].bookStatus == "Not")
                    {
                        throw new Exception("Some Copies of Books are still issued by users");
                    }
                    else
                    {
                        var st = _context.Book_Stock.Find(stocks[i].bookStockId);
                        _context.Book_Stock.Remove(st);
                    }
                    
                }
               
                book.BookStatus = "Removed";
                _context.SaveChanges();
                status = "Success";
                message = "Book Deactivated Successfully ";

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Ok(new { status = status, message = message });
            }
            return Ok(new { status = status, message = message });

        }
       

        [HttpPut]
        public IActionResult EditBook(Book_Data model)
        {
            try
            {
                if (model == null)
                {
                    throw new Exception("Model is Empty" );
                }
                if (model.BookId == "0")
                {
                    throw new Exception( "Book Id Can not be 0" );
                }
                var book = _context.Book_Data.Find(model.BookId);
                if (book == null)
                {
                    throw new Exception( $"No Book Available with {model.BookId}" );
                }
                if (model.BookTitle != "" || book.BookTitle != model.BookTitle) { book.BookTitle = model.BookTitle; }
                if (model.BookAuthor != "" || book.BookAuthor != model.BookAuthor) { book.BookAuthor = model.BookAuthor; }
                if (model.BookGenre != "" || book.BookGenre != model.BookGenre) { book.BookGenre = model.BookGenre ;}
                if (model.BookRating != ""|| book.BookRating != model.BookRating) { book.BookRating = model.BookRating ; }
                if (model.BookImg != "" || book.BookImg != model.BookImg) { book.BookImg = model.BookImg ; }
                if (model.BookDesc != "" || book.BookDesc != model.BookDesc) { book.BookDesc = model.BookDesc ; }
                if (model.BookStatus != "" || book.BookStatus != model.BookStatus) { book.BookStatus = model.BookStatus ; }

                _context.SaveChanges();
                status = "Success";
                message = "Book Info Updated :)";
            }
            catch (Exception ex)
            {
                message = ex.Message;
              
            }
            return Ok(new { status = status, message = message });
        }
        
        [HttpDelete]
        public IActionResult DeleteBook(string id)
        {
            try
            {
                var book = _context.Book_Data.Find(id);
                if (book == null)
                {
                    throw new Exception( $"No Book with Id {id} is available" );
                }
                Console.WriteLine(book.BookStatus);
                
                _context.Remove(book);
                _context.SaveChanges();
                message = "Book Data Deleted" ;
                status = "Success";
            }
            catch (Exception ex)
            {
                message=ex.Message;
            }
            return Ok(new { status = status, message = message });
        }
    }
}
