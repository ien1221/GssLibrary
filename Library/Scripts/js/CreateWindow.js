$(document).ready(function () {
    $("#create_window").kendoWindow({
        height: "580px",
        width: "400px",
        visible: false,
        modal: true,
        title: "新增書籍",
        open: function () {
            this.center();
        }
    });
    $("#create_name").kendoTextArea({
        size: "large",
        rows: 3,
        maxLength: 195,
        placeholder: "最大上限195字"
    });

    $("#create_note").kendoTextArea({
        size: "large",
        rows: 4,
        maxLength: 1195,
        placeholder: "最大上限1195字"
    });

    $("#create_bought_date").kendoDatePicker({
        format: "yyyy/MM/dd",
        max: new Date(9999, 12, 31),//DB最大日期
        min: new Date(1753, 1, 1),//DB最小日期
        dateInput: true,
        value: new Date()
    });

    $("#create_submit").click(function () {
        var arg = {
            BookName: $("#create_name").val(),
            Author: $("#create_author").val(),
            Publisher: $("#create_publisher").val(),
            Note: $("#create_note").data("kendoTextArea").value(),
            BoughtDate: kendo.toString($("#create_bought_date").data("kendoDatePicker").value(), "yyyy-MM-dd"),
            CategoryId: $("#create_category").data("kendoDropDownList").value(),
        }
        $.ajax({
            url: "/Library/CreateBook",
            data: arg,
            dataType: "json",
            type: "POST",
            success: function (response) {
                alert(response.message);
                if (response.ok) {
                    if (confirm("是否新增下一筆?")) {
                        InitializeCreateWindow()
                        $("#previous_id").text(response.id);
                        $("#previous_info").removeAttr("hidden");
                    } else {
                        InitializeCreateWindow()
                        window.location.href = "/Library/BookDetail?bookId=" + response.id + "&mode=update"
                    }
                }
            },
            error: function () {
                alert("系統發生錯誤!");
            }
        })

    });

    $("#create_clear").click(function () {
        InitializeCreateWindow();
    });

    $("#validate").kendoValidator({
        rules: {
            emptyColumn: function (input) {
                return input.val().trim() !== "";
            }
        },
        messages: {
            emptyColumn: "不可空白"
        }
    });

    $("#previous_info").click(function () {
        window.location.href = "/Library/BookDetail?bookId=" + $("#previous_id").text() + "&mode=read";
    });
});

function InitializeCreateWindow() {
    $("#create_name").val("");
    $("#create_author").val("");
    $("#create_publisher").val("");
    $("#create_note").data("kendoTextArea").value("");
    $("#create_bought_date").data("kendoDatePicker").value(new Date());
    $("#create_category").data("kendoDropDownList").value("");
    $("#validate").data("kendoValidator").reset();
}