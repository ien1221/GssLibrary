using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class LibraryController : Controller
    {
        readonly Models.BookService bookService = new Models.BookService();

        /// <summary>
        /// 首頁
        /// Read-書籍列表
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Update-修改書籍資訊
        /// </summary>
        /// <param name="bookId">Book ID</param>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult BookDetail(int bookId, string mode)
        {
            return View();
        }

        [HttpPost()]
        public JsonResult CreateBook(Models.BookCreateArg arg)
        {
            try
            {
                int bookId = bookService.CreateBook(arg);
                var result = new
                {
                    ok = true,
                    message = "新增成功!",
                    id = bookId
                };
                return Json(result);
            }
            catch
            {
                var result = new
                {
                    ok = false,
                    message = "新增失敗!"
                };
                return Json(result);
            }
        }

        [HttpPost()]
        public JsonResult UpdateBookDetail(Models.Book book)
        {
            try
            {
                bookService.UpdateBookDetail(book);
            }
            catch(Exception ex)//log library
            {
                return Json("修改失敗");
            }
            return Json("修改成功!");
        }

        /// <summary>
        /// Delete-刪除書籍
        /// </summary>
        /// <param name="bookId">Book ID</param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult DeleteBook(int bookId)
        {
            try
            {
                string statusId = bookService.GetBookById(bookId).StatusId;
                //B->已借出 C->已借出(未領)
                if(statusId == "B" || statusId == "C")
                {
                    var result = new
                    {
                        ok = false,
                        message = "該書本已被借出，不可刪除!"
                    };
                    return Json(result);
                }
                else
                {
                    bookService.DeleteBookById(bookId);
                    var result = new
                    {
                        ok = true,
                        message = "刪除成功!"
                    };
                    return this.Json(result);
                }
            }
            catch (Exception)
            {
                var result = new
                {
                    ok = false,
                    message = "刪除失敗!"
                };
                return this.Json(result);
            }
        }

        /// <summary>
        /// Update-查詢借閱紀錄
        /// </summary>
        /// <param name="bookId">Book ID</param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult GetLendRecord(int bookId)
        {
            var result = new
            {
                //lendRecord = bookService.GetLendRecord(bookId),
                lendRecord = bookService.GetLendRecord_SQL(bookId),
                bookName = bookService.GetBookById(bookId).BookName
            };
            return Json(result);
        }

        #region GetListElements
        [HttpPost()]
        public JsonResult GetCategoryList()
        {
            return Json(bookService.GetCategoryList());
        }

        [HttpPost()]
        public JsonResult GetStatusList()
        {
            return Json(bookService.GetStatusList());
        }

        [HttpPost()]
        public JsonResult GetKeeperList()
        {
            return Json(bookService.GetKeeperList());
        }

        [HttpPost()]
        public JsonResult GetBookNameList()
        {
            return Json(bookService.GetBookList());
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult GetBookByCondition(Models.BookSearchArg arg)
        {
            List<Models.Book> books = bookService.GetBookByCondition(arg);
            foreach(Models.Book book in books)
            {
                book.BoughtDate = book.BoughtDate.Replace("-", "/");
                book.Keeper = book.Keeper.Split('-')[0];
            }
            return Json(books);
        }

        [HttpPost()]
        public JsonResult GetBookById(int bookId)
        {
            return Json(bookService.GetBookById(bookId));
        }

    }
}