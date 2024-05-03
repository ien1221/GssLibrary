$(document).ready(function () {
    $("#lend_record_window").kendoWindow({
        height: "400px",
        width: "800px",
        visible: false,
        modal: true,
        open: function () {
            this.center();
            $.ajax({
                type: "POST",
                url: "/Library/GetLendRecord",
                data: "bookId=" + $("#book_id").val(),
                dataType: "json",
                success: function (response) {
                    $("#lend_record_window").data("kendoWindow").title(response.bookName);
                    var dataSource = new kendo.data.DataSource({
                        data: response.lendRecord,
                        schema: {
                            model: {
                                fields: {
                                    LendDate: { type: "string" },
                                    UserId: { type: "string" },
                                    EName: { type: "string" },
                                    CName: { type: "string" }
                                }
                            }
                        },
                        pageSize: 20
                    })
                    $("#lend_record_grid").data("kendoGrid").setDataSource(dataSource);
                }, error: function (error) {
                }
            });
        }
    });

    $("#lend_record_grid").kendoGrid({
        dataSource: {
            data: null,
            schema: {
                model: {
                    fields: {
                        LendDate: { type: "string" },
                        UserId: { type: "string" },
                        EName: { type: "string" },
                        CName: { type: "string" }
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
            { field: "LendDate", title: "借閱日期", width: "25%" },
            { field: "UserId", title: "借閱人員編號", width: "25%"},
            { field: "EName", title: "英文姓名", width: "25%" },
            { field: "CName", title: "中文姓名", width: "25%" },
        ]
    });
});