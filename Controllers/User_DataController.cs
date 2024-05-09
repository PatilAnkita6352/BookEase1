using LibrarySystemAPI.DataAccessLayer;
using LibrarySystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class User_DataController : ControllerBase
    {
        private readonly LibrarySystemContext _context;
        string status = "Failure";
        string message;
        public User_DataController(LibrarySystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var users = _context.User_Data.ToList();
                if (users.Count == 0)
                {
                    throw new Exception("No user Available");
                }
                return Ok(new {status="Success", data= users });
            }
            catch (Exception ex)
            {
                return Ok(new { status="Failure", message= ex.Message});
            }
        }
        [HttpPost("{id}")]
        public IActionResult GetUserdata(string id)
        {
            try
            {
                var users = _context.User_Data.Find(id);
                if (users == null)
                {
                    throw new Exception("No user Available");
                }
                return Ok(new { status = "Success", data = users });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Failure", message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetByID(string id)
        {
            try
            {
                var users = _context.User_Data.Find(id);
                if (users == null)
                {
                    throw new Exception("No user Available");
                }
                var userhis= (from i in _context.Issued_Books
                              join bs in _context.Book_Stock on i.BookStockId equals bs.BookStockId into bsJoin
                              from bs in bsJoin.DefaultIfEmpty()
                              join b in _context.Book_Data on bs.BookId equals b.BookId into bJoin
                              from b in bJoin.DefaultIfEmpty()
                              where i.UserId == id
                              select new
                              {
                                  BookId = b.BookId,
                                  UserId = i.UserId,
                                  BookTitle = b.BookTitle,
                                  IssueDate = i.IssueDate,
                                  ReturnDate = i.ReturnDate,
                                  BookIStatus = i.BookIStatus
                              }).ToList();

                if(userhis.Count == 0)
                {
                    throw new Exception("This user has no History");
                }
                return Ok(new {status="Success", data= userhis });
            }
            catch (Exception ex)
            {
                return Ok(new { status = status, message = ex.Message });
            }
        }

        [HttpGet("{name}/{password}")]
        public IActionResult GetForLogin(string name, string password)
        {
            try
            {
                if(_context.User_Data == null)
                {
                    throw new Exception("User _Data Table is Empty");
                }
                
                var users= from m in _context.User_Data select m;

                if(!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(password))
                {
                    users= users.Where(s => s.UserEmail == name && s.UserPass == password);
                }
               
                if(users.Count() == 0)
                {
                    throw new Exception("No User with Authentication Found");
                }
                
                return Ok(new {status="Success",message="Login Successful :)"});
            }
            catch (Exception ex)
            {
                return Ok(new {status=status,message=ex.Message});
            }
        }

        [HttpPost]
        public IActionResult Post(User_Data model)
        {
            try
            {
                var user = _context.User_Data.Where(s => s.UserId == model.UserId).ToList();
                if (user.Count >0)
                {
                    throw new Exception("User Id already taken");
                }
                if (model == null)
                {
                    throw new Exception("Model is Null");
                }
                if (model.UserId == "")
                {
                    throw new Exception("Model id Can not have id 0");
                }
                _context.Add(model);
                _context.SaveChanges();
                return Ok(new {status="Success",message= "User Created Successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new {status="Failure", message=ex.Message});
            }
        }

        [HttpPut]
        public IActionResult Put(User_Data model)
        {
            try
            {
                if (model == null)
                {
                    throw new Exception("Model is Null");
                }
                if (model.UserId == "")
                {
                    throw new Exception("User id Can not have id 0");
                }
                var users = _context.User_Data.Find(model.UserId);
                if (users == null)
                {
                    throw new Exception($"User with id {model.UserId} Not exist");
                }

                users.UserName = model.UserName;
                users.UserEmail = model.UserEmail;
                users.UserLoc = model.UserLoc;
                users.UserAuth = model.UserAuth;
                users.UserPass = model.UserPass;
                _context.SaveChanges();
                return Ok(new {status="Success",message= "Data Updated" });
            }
            catch (Exception ex)
            {
                return Ok(new{status=status,message=ex.Message});
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var users = _context.User_Data.Find(id);
                if (users == null)
                {
                    throw new Exception($"Users with Id {id} Not Found");
                }

                _context.User_Data.Remove(users);
                _context.SaveChanges();
                return Ok(new {status="Success",message= "User Data Deleted" });
            }
            catch (Exception ex)
            {
                return Ok(new {status=status,message=ex.Message});
            }
        }
    }
}
