"use strict";

$(function () {
    $.ajaxSetup({ cache: false });

    $("a[data-modal]").on("click", function (e) {
        $("#myModalContent").load(this.href, function () {

            $("#myModal").modal({
                backdrop: "static",
                keyboard: true
            }, "show");

            bindForm(this);
        });

        return false;
    });
});

function bindForm(dialog) {

    $("form", dialog).submit(function () {
        alert("Entering bindForm");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function success(result) {
                if (result.success) {
                    $("#myModal").modal("hide");
                    //Refresh
                    location.reload();
                    alert("success");
                } else {
                    $("#myModalContent").html(result);
                    bindForm();
                    alert("fail");
                }
            }
        });
        return false;
    });
}

