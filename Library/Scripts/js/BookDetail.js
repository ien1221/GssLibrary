var book = null;

$(document).ready(function () {
    var param = location.href.split('?')[1].split('&');
    var bookId = param[0].split('=')[1];
    var mode = param[1].split('=')[1];
    console.log("hello");
    console.log("hello");
    console.log("hello");

    $.ajax({
        type: "POST",
        url: "/Library/GetBookById?bookId=" + bookId,
        data: bookId,
        dataType: "json",
        success: function (response) {
            book = response;
            InitializeValue();
            IsKeeperEnable("initialize");
            IsEditable(mode);
        }
    });

    $("#book_name").kendoTextArea({
        rows: 2,
        maxLength: 195,
        placeholder: "最大上限195字"
    })

    $("#book_note").kendoTextArea({
        rows: 4,
        maxLength: 1195,
        placeholder: "最大上限1195字"
    })

    $("#bought_date").kendoDatePicker({
        format: "yyyy/MM/dd",
        max: new Date(9999, 12, 31),//DB最大日期
        min: new Date(1753, 1, 1),//DB最小日期
        dateInput: true
    })

    $("#book_category").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: {
            transport: {
                read: {
                    url: "/Library/GetCategoryList",
                    type: "POST",
                    dataType: "json"
                }
            }
        }

    });

    $("#book_status").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: {
            transport: {
                read: {
                    url: "/Library/GetStatusList",
                    type: "POST",
                    dataType: "json"
                }
            }
        },
        change: IsKeeperEnable
    });

    $("#book_keeper").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: {
            transport: {
                read: {
                    url: "/Library/GetKeeperList",
                    type: "POST",
                    dataType: "json"
                }
            }
        }

    });

    $("#validate").kendoValidator({
        rules: {
            emptyColumn: function (input) {
                var status = $("#book_status").data("kendoDropDownList").value()
                if (input.is("[name=bookKeeper]")) {
                    //B->已借出 C->已借出(未領)
                    if (status == "B" || status == "C") {
                        return input.val() !== "";
                    } else {
                        return true;
                    }
                } else {
                    return input.val().trim() !== "";
                }
            }
        },
        messages: {
            emptyColumn: "不可空白"
        }
    })

    $("#submit").click(function (e) {
        if ($("#validate").data("kendoValidator").validate()) {
            var editedBook = {
                BookId: bookId,
                BookName: $("#book_name").val(),
                Author: $("#book_author").val(),
                Publisher: $("#book_publisher").val(),
                Note: $("#book_note").data("kendoTextArea").value(),
                BoughtDate: kendo.toString($("#bought_date").data("kendoDatePicker").value(), "yyyy-MM-dd"),
                CategoryId: $("#book_category").data("kendoDropDownList").value(),
                StatusId: $("#book_status").data("kendoDropDownList").value(),
                KeeperId: $("#book_keeper").data("kendoDropDownList").value()
            }
            $.ajax({
                url: "/Library/UpdateBookDetail",
                data: editedBook,
                type: "POST",
                dataType: "json",
                success: function (response) {
                    alert(response);
                },
                error: function () {
                    alert("系統發生錯誤");
                }
            })
        }
    })

    $("#edit").click(function () {
        window.location.href = "/Library/BookDetail?bookId=" + bookId + "&mode=update"
    });

    $("#delete").click(function () {
        if (confirm("確認刪除?")) {
            $.ajax({
                url: "/Library/DeleteBook",
                data: "bookId=" + bookId,
                type: "POST",
                success: function (response) {
                    alert(response.message);
                    if (response.ok) {
                        window.location.href = "/Library/Index"
                    }
                },
                error: function () {
                    alert("系統發生錯誤")
                }
            })
        }
    });

    $("#back").click(function () {
        window.location.href = "/Library/Index"
    })

});

function InitializeValue() {
    $("#book_name").val(book.BookName);
    $("#book_author").val(book.Author);
    $("#book_publisher").val(book.Publisher);
    $("#book_note").data("kendoTextArea").value(book.Note);
    $("#bought_date").data("kendoDatePicker").value(book.BoughtDate);
    $("#book_category").data("kendoDropDownList").value(book.CategoryId);
    $("#book_status").data("kendoDropDownList").value(book.StatusId);
    $("#book_keeper").data("kendoDropDownList").value(book.KeeperId);
};

function IsKeeperEnable(mode) {
    var currentStatus = $("#book_status").data("kendoDropDownList").value()
    //
    if (currentStatus == "B" || currentStatus == "C") {
        $("#book_keeper").data("kendoDropDownList").enable(true);
        if (mode !== "initialize") {
            $("#book_keeper").data("kendoDropDownList").select(0);
        }
    } else {
        $("#book_keeper").data("kendoDropDownList").value("");
        $("#book_keeper").data("kendoDropDownList").enable(false);
    }
};

function IsEditable(mode) {
    if (mode == "update") {
        $("#edit").attr("hidden", "hidden");
        $("#mode").text("修改資料");
    } else {
        $("#mode").text("書籍細項");
        $("#book_name").attr("readonly", "readonly");
        $("#book_author").attr("readonly", "readonly");
        $("#book_publisher").attr("readonly", "readonly");
        $("#book_note").data("kendoTextArea").readonly(true);
        $("#bought_date").data("kendoDatePicker").readonly(true);
        $("#book_category").data("kendoDropDownList").readonly(true);
        $("#book_status").data("kendoDropDownList").readonly(true);
        $("#book_keeper").data("kendoDropDownList").readonly(true);
        $("#submit").attr("hidden", "hidden");
        $("#delete").attr("hidden", "hidden");
    }
};