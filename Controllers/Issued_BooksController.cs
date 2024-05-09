using LibrarySystemAPI.DataAccessLayer;
using LibrarySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Runtime.Intrinsics.X86;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibrarySystemAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Issued_BooksController : ControllerBase
    {
        public readonly LibrarySystemContext _context;
        string status_r = "Failure";
        string message;
        public Issued_BooksController(LibrarySystemContext context)
        {
            _context = context;
        }

        [HttpGet("{status}")]
        public IActionResult Getwithstatus(string status) {
            try
            {
                Console.WriteLine(status);
                var books_issue = _context.Issued_Books.ToList();
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                foreach (var b in books_issue)
                {
                    var returnDate = b.ReturnDate.CompareTo(currentDate);
                    if (returnDate < 0)
                    {
                        b.BookIStatus = "Late";
                    }

                }
                var books = _context.Issued_Books.Where(s => s.BookIStatus.Equals(status));
                if (books.Count() == 0)
                {
                    throw new Exception($"No Book With {status} available");
                   
                }
                return Ok(new {status= "Success" , data=books});
            }
            catch (Exception ex)
            {
                return Ok(new { status = status_r, message = ex.Message });
            }
        }

        [HttpGet]

        public IActionResult GetHistory(string id) 
        {
            try
            {
                var books = (from i in _context.Issued_Books
                             join bs in _context.User_Data on i.UserId equals bs.UserId
                             where i.BookStockId.Contains(id) 
                             select new
                             {
                                 UserId = bs.UserId,
                                 UserName = bs.UserName,
                                 BookStockId = i.BookStockId,
                                 IssueDate = i.IssueDate,
                                 ReturnDate = i.ReturnDate,
                                 BookIStatus = i.BookIStatus,
                             }).ToList();



                if (books.Count == 0)
                {
                    throw new Exception($"No History of Book With {id} available");
                }
                return Ok(new { status = "Success", data = books });
            }
            catch (Exception ex)
            {
                return Ok(new { status = status_r, message = ex.Message });
            }

        }

        [HttpPost]
        public IActionResult PostIssueBook([FromBody] Issued_BooksAPI model)
        {
            try
            {
                var I= _context.Issued_Books.ToList();
                var len = I.Count;
                var dateN = DateOnly.FromDateTime(DateTime.Now);
                var rdate = dateN.AddMonths(1);
                var check = _context.Issued_Books.Where(s => s.BookStockId == model.BookStockId && s.BookIStatus != "Returned");
                var checkuser = _context.Issued_Books.Where(p => p.UserId == model.UserId);
                var stock = _context.Book_Stock.Find(model.BookStockId);
                if(checkuser.Count() > 3)
                {
                    throw new Exception("User can Issue upto 3 Books Only");
                }
                if (check.Count() > 0)
                {
                    throw new Exception("Book is Already Issued");
                }
                var u = _context.User_Data.Where(s => s.UserId == model.UserId).ToList();
                if (u.Count <= 0 )
                {
                    throw new Exception("User Don't Exist");
                }
                if(model.IssueDate != "")
                {
                    dateN = DateOnly.Parse(model.IssueDate);
                }
                if(model.ReturnDate != "")
                {
                    rdate = DateOnly.Parse(model.ReturnDate); 
                }
                var bookItem = new Issued_Books()
                {
                    IssueId = len + 1,
                    BookStockId = model.BookStockId,
                    UserId = model.UserId,
                    IssueDate = dateN,
                    ReturnDate = rdate,
                    BookIStatus = "Issued"
                };
                if(stock==null)
                {
                    throw new Exception("Book Stock not available");
                }

                stock.StockStatus = "Not";
                _context.Issued_Books.Add(bookItem);
                _context.SaveChanges();
                status_r = "Success";
                    message = "Book Issued Successful :)";
            }
            catch (Exception ex)
            {
               message= ex.Message;
            }
            return Ok(new { status = status_r, message = message });
        }

        [HttpPut]
        public IActionResult Update()
        {
            try
            {
                var books = _context.Issued_Books.ToList(); // Load all issued books into memory
                var currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (books.Count == 0)
                {
                    throw new Exception("No Issued Book with Delayed Date Found");
                }

                foreach (var book in books)
                {
                    if (book.BookIStatus == "Returned")
                    {
                        _context.Issued_Books.Remove(book);
                    }
                    else
                    {
                        var returnDate = book.ReturnDate.CompareTo(currentDate);


                        if (returnDate < 0)
                        {
                            book.BookIStatus = "Late";
                        }

                    }
                }

                _context.SaveChanges();
                status_r = "Success";
                message = "Late Status Updated";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Ok(new { status = status_r, message = message });
        }

        [HttpPut("{id}/{user}")]
        public IActionResult ReturnBook(string id, string user)
        {
            string status_r = "Error";
            string message = "";

            try
            {
                var book = _context.Issued_Books.SingleOrDefault(b => b.BookStockId == id && b.UserId == user && (b.BookIStatus == "Issued" || b.BookIStatus == "Late"));
                if (book == null)
                {
                    throw new Exception($"Book with ID {id} not found for user {user}");
                }

                book.BookIStatus = "Returned";
                var stock = _context.Book_Stock.Find(id);
                stock.StockStatus = "Available";


                // Save changes to the database
                _context.SaveChanges();

                status_r = "Success";
                message = "Book Returned";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Ok(new { status = status_r, message = message });
        }

    }
}
