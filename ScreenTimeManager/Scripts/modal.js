$(function () {
    $.ajaxSetup({ cache: false });        ///////////////////////////////////// Document never ready

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
    $("button.confirm-apply").on("click", function(e) {
        e.preventDefault();
        $("form").submit();
    });

    // any <form> tag, the context in which that form tag is found
    $("form", dialog).submit(function () { // Why is this not called when hitting the submit button?
        //alert("entered submit function");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                alert(result);
                if (result.success) {
                    $("#myModal").modal("hide");
                    //Refresh
                    location.reload();
                } else {
                    $("#myModalContent").html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}