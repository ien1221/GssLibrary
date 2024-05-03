$(document).ready(function () {
    $("#book_name").kendoAutoComplete({
        dataTextField: "Text",
        filter: "contains",
        dataSource: {
            transport: {
                read: {
                    url: "/Library/GetBookNameList",
                    type: "POST",
                    dataType: "json"
                }
            }
        }
    });

    $(".k-textbox.category").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        optionLabel: {
            Text : "請選擇",
            Value : ""
        },
        dataSource: {
            transport: {
                read: {
                    url: "/Library/GetCategoryList",
                    type: "POST",
                    dataType : "json"
                }
            }
        }

    });

    $("#book_keeper").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        optionLabel: {
            Text: "請選擇",
            Value: ""
        },
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

    $("#book_status").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        optionLabel: {
            Text: "請選擇",
            Value: ""
        },
        dataSource: {
            transport: {
                read: {
                    url: "/Library/GetStatusList",
                    type: "POST",
                    dataType: "json"
                }
            }
        }
    });

    $("#search_grid").kendoGrid({
        dataSource: {
            data: null,
            schema: {
                model: {
                    fields: {
                        Category: { type: "string" },
                        BookName: { type: "string" },
                        BoughtDate: { type: "string" },
                        Status: { type: "string" },
                        Keeper: { type: "string" }
                    }
                }
            },
            pageSize: 20
        },
        sortable: true,
        pageable: {
            input: true,
            numeric: false
        },
        columns: [
            { field: "Category", title: "圖書類別", width: "13%" },
            { field: "BookName", title: "書名", width: "31%", template: '<a href="/Library/BookDetail?bookId=#= BookId #&mode=read">#= BookName #</a>' },
            { field: "BoughtDate", title: "購書日期", width: "12%"},
            { field: "Status", title: "借閱狀態", width: "11%" },
            { field: "Keeper", title: "借閱人", width: "10%" },
            { command: { text: "借閱紀錄", click: ReadLendRecord }, title: " ", width: "13%" },
            { command: { text: "編輯", click: UpdateBook }, title: " ", width: "10%" },
            { command: { text: "刪除", click: DeleteBook }, title: " ", width: "10%" },
        ]
    })
    $("#search_grid").hide();


    //提交查詢條件
    $("#submit").click(function () {
        var searchArg = {
            BookName: $("#book_name").data("kendoAutoComplete").value(),
            CategoryId: $("#book_category").data("kendoDropDownList").value(),
            KeeperId: $("#book_keeper").data("kendoDropDownList").value(),
            StatusId: $("#book_status").data("kendoDropDownList").value()
        };
        $.ajax({
            type: "POST",
            url: "/Library/GetBookByCondition",
            data: searchArg,
            dataType: "json",
            success: function (response) {
                var dataSource = new kendo.data.DataSource({
                    data: response,
                    schema: {
                        model: {
                            fields: {
                                Category: { type: "string" },
                                BookName: { type: "string" },
                                BoughtDate: { type: "string" },
                                Status: { type: "string" },
                                Keeper: { type: "string" }
                            }
                        }
                    },
                    pageSize: 20
                })
                $("#search_grid").data("kendoGrid").setDataSource(dataSource);
                $("#search_grid").show();
            }, error: function (error) {
            }
        });
    })

    //清除頁面
    $("#clear").click(function () {
        $("#book_name").val("");
        $("#book_category").data("kendoDropDownList").value("");
        $("#book_keeper").data("kendoDropDownList").value("");
        $("#book_status").data("kendoDropDownList").value("");
        var dataSource = new kendo.data.DataSource({
            data: null,
            schema: {
                model: {
                    fields: {
                        Category: { type: "string" },
                        BookName: { type: "string" },
                        BoughtDate: { type: "string" },
                        Status: { type: "string" },
                        Keeper: { type: "string" }
                    }
                }
            },
            pageSize: 20
        })
        $("#search_grid").data("kendoGrid").setDataSource(dataSource);
        $("#search_grid").hide();
    })

    $("#create").click(function () {
        $("#create_window").data("kendoWindow").open();
    })
    
})

//查看借閱紀錄
function ReadLendRecord(e) {
    var id = $("#search_grid").data("kendoGrid").dataItem($(e.currentTarget).closest("tr")).BookId;
    $("#book_id").val(id);
    $("#lend_record_window").data("kendoWindow").open();
}

//
function UpdateBook(e) {
    var grid = $("#search_grid").data("kendoGrid");
    var selectedRow = grid.dataItem($(e.currentTarget).closest("tr"));
    window.location.href = "/Library/BookDetail?bookId=" + selectedRow.BookId + "&mode=update";
}

function DeleteBook(e) {
    var grid = $("#search_grid").data("kendoGrid");
    var selectedRow = grid.dataItem($(e.currentTarget).closest("tr"));
    if (confirm("確認刪除?")) {
        $.ajax({
            url: "/Library/DeleteBook",
            data: "bookId=" + selectedRow.BookId,
            type: "POST",
            success: function (response) {
                alert(response.message);
                if (response.ok) {
                    $("#submit").click();
                }
            },
            error: function () {
                alert("系統發生錯誤")
            }
        })
    }
}