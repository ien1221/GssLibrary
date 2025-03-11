using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Models
{
    public class BookService
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetDBConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }
        #region GetListItem

        public List<SelectListItem> GetBookList()
        {
            string sql = @" SELECT bd.BOOK_NAME AS BookName
	                            , bd.BOOK_ID AS	 BookId
                            FROM BOOK_DATA bd;";
            return GetDropdownlistElements(sql, "BookName", "BookId");
        }
        /// <summary>
        /// 取得DB所有圖書類別名稱&ID
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCategoryList()
        {
            string sql = @"SELECT c.BOOK_CLASS_ID AS ClassId,
                            c.BOOK_CLASS_NAME AS ClassName
                           FROM	 BOOK_CLASS c
                           ORDER BY ClassName;";
            return GetDropdownlistElements(sql, "ClassName", "ClassId");
        }

        /// <summary>
        /// 取得DB所有使用者名稱&ID
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetKeeperList()
        {
                string sql = @"SELECT mm.USER_ID AS [UserId],
                                mm.USER_ENAME + '-' + mm.USER_CNAME AS UserName
                               FROM MEMBER_M mm
                               ORDER BY [UserId];";
            return GetDropdownlistElements(sql, "UserName", "UserId");
        }

        /// <summary>
        /// 取得DB所有書本相關狀況&ID
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetStatusList()
        {
                string sql = @"SELECT bc.CODE_ID AS StatusId,
                                bc.CODE_NAME AS StatusName
                               FROM BOOK_CODE bc
                               WHERE bc.CODE_TYPE = 'BOOK_STATUS'
                               ORDER BY StatusName;";
            return GetDropdownlistElements(sql, "StatusName", "StatusId");
        }

        /// <summary>
        /// 從DB取得下拉式選單資訊
        /// </summary>
        /// <param name="sql">MS SQL query</param>
        /// <param name="name">element name</param>
        /// <param name="id">element id</param>
        /// <returns></returns>
        private List<SelectListItem> GetDropdownlistElements(string sql, string name, string id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(this.GetDBConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sql, connection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(dt);
            }
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row[name].ToString(),
                    Value = row[id].ToString()
                });
            }
            return result;
        }
        #endregion

        #region GetBooks
        /// <summary>
        /// Read-透過Book ID(Primary key)尋找書籍(BOOK_DATA, BOOK_CLASS, BOOK_CODE, MEMBER_M)
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns></returns>
        public Book GetBookById(int id)
        {
            BookSearchArg arg = new BookSearchArg();
            arg.BookId = id;
            List<Book> books = GetBookByCondition(arg);
            if (books.Count != 0)
                return books[0];
            else
                return new Book();
        }

        /// <summary>
        /// Read-與查詢欄相關書籍 (BOOK_DATA, BOOK_CLASS, BOOK_CODE, MEMBER_M)
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public List<Book> GetBookByCondition(Models.BookSearchArg arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
                               bd.BOOK_ID AS BookId
	                           ,bd.BOOK_NAME AS BookName
	                           ,bc.BOOK_CLASS_NAME AS ClassName
	                           ,CONVERT(VARCHAR, bd.BOOK_BOUGHT_DATE, 23) AS BoughtDate
	                           ,mm.USER_ENAME + '-' + mm.USER_CNAME AS UserName
	                           ,bd.BOOK_AUTHOR AS Author
	                           ,bd.BOOK_PUBLISHER AS Publisher
	                           ,bd.BOOK_NOTE AS Note
	                           ,bc1.CODE_NAME AS Status
                               ,bd.BOOK_CLASS_ID as ClassId
                               ,bd.BOOK_STATUS as StatusId
                               ,bd.BOOK_KEEPER as KeeperId
                           FROM BOOK_DATA bd
                           JOIN BOOK_CLASS bc
	                           ON bd.BOOK_CLASS_ID = bc.BOOK_CLASS_ID
                           LEFT JOIN MEMBER_M mm
	                           ON bd.BOOK_KEEPER = mm.[USER_ID]
                           JOIN BOOK_CODE bc1
	                           ON (bd.BOOK_STATUS = bc1.CODE_ID AND bc1.CODE_TYPE = 'BOOK_STATUS')
                           WHERE 
	                           ( UPPER(bd.BOOK_NAME) LIKE UPPER('%' + @BookName + '%') OR @BookName = '')
	                           AND (bc.BOOK_CLASS_ID = @BookCategoryId OR @BookCategoryId = '')
	                           AND (bd.BOOK_KEEPER = @KeeperId OR @KeeperId = '')
	                           AND (bd.BOOK_STATUS = @Status OR @Status = '')
	                           AND (bd.BOOK_ID = @BookId OR @BookId = 0)
                           ORDER BY BoughtDate DESC;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@BookCategoryId", arg.CategoryId == null ? string.Empty : arg.CategoryId));
                cmd.Parameters.Add(new SqlParameter("@KeeperId", arg.KeeperId == null ? string.Empty : arg.KeeperId));
                cmd.Parameters.Add(new SqlParameter("@Status", arg.StatusId == null ? string.Empty : arg.StatusId));
                cmd.Parameters.Add(new SqlParameter("@BookId", arg.BookId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return MapBookDetailToBook(dt);
        }

        /// <summary>
        /// 將DB資料轉為Book型態
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private List<Book> MapBookDetailToBook(DataTable dt)
        {
            List<Book> result = new List<Book>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new Book
                {
                    BookId = int.Parse(row["BookId"].ToString()),
                    BookName = row["BookName"].ToString() == "" ? "Na" : row["BookName"].ToString(),
                    Category = row["ClassName"].ToString() == "" ? "" : row["ClassName"].ToString(),
                    BoughtDate = row["BoughtDate"].ToString() == "" ? "" : row["BoughtDate"].ToString(),
                    Keeper = row["UserName"].ToString() == "" ? "" : row["UserName"].ToString(),
                    Author = row["Author"].ToString() == "" ? "" : row["Author"].ToString(),
                    Publisher = row["Publisher"].ToString() == "" ? "" : row["Publisher"].ToString(),
                    Note = row["Note"].ToString() == "" ? "" : row["Note"].ToString(),
                    Status = row["Status"].ToString(),
                    CategoryId = row["ClassId"].ToString(),
                    StatusId = row["StatusId"].ToString(),
                    KeeperId = row["KeeperId"].ToString()
                });
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Read-查詢該書籍借閱紀錄(BOOK_LEND_RECORD)
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns></returns>
        public List<LendRecordArg> GetLendRecord(int id)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
	                           CONVERT(VARCHAR, blr.LEND_DATE, 111) AS LendDate,
	                           mm.[USER_ID] AS UserId,
	                           mm.USER_ENAME AS EName,
	                           mm.USER_CNAME AS CName,
	                           blr.BOOK_ID
                           FROM BOOK_LEND_RECORD blr
                           LEFT JOIN MEMBER_M mm
	                           ON blr.KEEPER_ID = mm.[USER_ID]
                           JOIN BOOK_DATA bd
	                           ON blr.BOOK_ID = bd.BOOK_ID
                           WHERE blr.BOOK_ID = @BookId
                           ORDER BY LendDate DESC;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", id));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            List<LendRecordArg> result = new List<LendRecordArg>();
            foreach (DataRow row in dt.Rows)
            {
                LendRecordArg lendRecord = new LendRecordArg()
                {
                    LendDate = row["LendDate"].ToString(),
                    UserId = row["UserId"].ToString(),
                    EName = row["EName"].ToString(),
                    CName = row["CName"].ToString()
                };
                result.Add(lendRecord);
            }
            return result;
        }
        /// <summary>
        /// Read-查詢該書籍借閱紀錄(BOOK_LEND_RECORD)
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns></returns>
        public List<LendRecordArg> GetLendRecord_SQL(int id)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
	                           CONVERT(VARCHAR, blr.LEND_DATE, 111) AS LendDate,
	                           mm.[USER_ID] AS UserId,
	                           mm.USER_ENAME AS EName,
	                           mm.USER_CNAME AS CName,
	                           blr.BOOK_ID
                           FROM BOOK_LEND_RECORD blr
                           LEFT JOIN MEMBER_M mm
	                           ON blr.KEEPER_ID = mm.[USER_ID]
                           JOIN BOOK_DATA bd
	                           ON blr.BOOK_ID = bd.BOOK_ID
                           WHERE blr.BOOK_ID = " + id +  "ORDER BY LendDate DESC;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            List<LendRecordArg> result = new List<LendRecordArg>();
            foreach (DataRow row in dt.Rows)
            {
                LendRecordArg lendRecord = new LendRecordArg()
                {
                    LendDate = row["LendDate"].ToString(),
                    UserId = row["UserId"].ToString(),
                    EName = row["EName"].ToString(),
                    CName = row["CName"].ToString()
                };
                result.Add(lendRecord);
            }
            return result;
        }

        /// <summary>
        /// Create-新增書籍(BOOK_DATA)
        /// </summary>
        /// <param name="arg"></param>
        public int CreateBook(BookCreateArg arg)
        {
            string sql = @" INSERT INTO BOOK_DATA (
                                BOOK_NAME,
                                BOOK_CLASS_ID,
                                BOOK_AUTHOR,
                                BOOK_BOUGHT_DATE,
                                BOOK_PUBLISHER,
                                BOOK_NOTE,
                                BOOK_STATUS,
                                BOOK_KEEPER,
                                BOOK_AMOUNT,
                                CREATE_DATE,
                                CREATE_USER,
                                MODIFY_DATE,
                                MODIFY_USER)
			                VALUES (@BookName, @Category, @Author, @BoughtDate, @Publisher,@Note, 'A', N'', 1, GETDATE(), N'', GETDATE(), N'');
                            Select SCOPE_IDENTITY()";
            int bookId;
            using (SqlConnection connection = new SqlConnection(GetDBConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@Category", arg.CategoryId));
                cmd.Parameters.Add(new SqlParameter("@Author", arg.Author));
                cmd.Parameters.Add(new SqlParameter("@BoughtDate", arg.BoughtDate));
                cmd.Parameters.Add(new SqlParameter("@Publisher", arg.Publisher));
                cmd.Parameters.Add(new SqlParameter("@Note", arg.Note));
                bookId = Convert.ToInt32(cmd.ExecuteScalar());
                connection.Close();
            }
            return bookId;
        }

        /// <summary>
        /// Delete-刪除書籍
        /// </summary>
        /// <param name="id"></param>
        public void DeleteBookById(int id)
        {
            string sql = @" BEGIN TRY
	                            BEGIN TRAN
		                            DELETE BOOK_DATA
		                            WHERE BOOK_ID = @BookID

		                            DELETE BOOK_LEND_RECORD
		                            WHERE BOOK_ID = @BookID
	                            COMMIT TRAN
                            END TRY
                            BEGIN CATCH
		                            ROLLBACK TRAN
                            END CATCH";
            using (SqlConnection connection = new SqlConnection(GetDBConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.Add(new SqlParameter("@BookId", id));
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }

        /// <summary>
        /// Update-修改書籍資料
        /// Create-增加借閱紀錄
        /// </summary>
        /// <param name="book"></param>
        public void UpdateBookDetail(Book book)
        {
            string sql = @" BEGIN TRY
    	                        BEGIN TRAN
    		                        UPDATE	BOOK_DATA 
			                        SET BOOK_NAME = @BookName
				                        ,BOOK_CLASS_ID = @Category
				                        ,BOOK_BOUGHT_DATE = @BoughtDate
				                        ,BOOK_STATUS = @Status
				                        ,BOOK_KEEPER = @Keeper
				                        ,BOOK_AUTHOR = @Author
				                        ,BOOK_PUBLISHER = @Publisher
				                        ,BOOK_NOTE = @Note
				                        ,MODIFY_DATE = GETDATE()
			                        WHERE BOOK_ID = @BookId

			                        IF @Status = 'B' OR @Status = 'C' BEGIN  
            	                        INSERT BOOK_LEND_RECORD (BOOK_ID, KEEPER_ID, LEND_DATE, CRE_DATE, CRE_USR, MOD_DATE, MOD_USR)
					                        VALUES (@BookId, @Keeper, GETDATE(), GETDATE(), N'', GETDATE(), N'')
                                    END
    	                        COMMIT TRAN
                            END TRY
                            BEGIN CATCH
    		                        ROLLBACK TRAN
                            END CATCH;";
            using (SqlConnection connection = new SqlConnection(GetDBConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.Add(new SqlParameter("@BookName", book.BookName == "Na" ? (Object)DBNull.Value : book.BookName));
                cmd.Parameters.Add(new SqlParameter("@Category", book.CategoryId));
                cmd.Parameters.Add(new SqlParameter("@BoughtDate", book.BoughtDate));
                cmd.Parameters.Add(new SqlParameter("@Status", book.StatusId));
                cmd.Parameters.Add(new SqlParameter("@Keeper", book.KeeperId == null ? (Object)DBNull.Value : book.KeeperId));
                cmd.Parameters.Add(new SqlParameter("@Author", book.Author));
                cmd.Parameters.Add(new SqlParameter("@Publisher", book.Publisher));
                cmd.Parameters.Add(new SqlParameter("@Note", book.Note));
                cmd.Parameters.Add(new SqlParameter("@BookId", book.BookId));
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    
}