"use strict";

$(function () {
    $.ajaxSetup({ cache: false }); ///////////////////////////////////// Document never ready

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

    // AHA moment. Learning javascript. Events will "fire" immediately if the function used is pass with parentheses
    // as it sees the paramater as the result of the function. Do it without the parentheses, and it will see it as
    // a reference to the function (the function itself is now the paramater)
});

////////////////////////////////////////////////////////////// we need the ratio. Store it in the page source? I think this might be the best choice
function updatePendingTime() {
    // calculte here

    var numer = parseInt($("meta.ratio-data").attr("numerator"));
    var denom = parseInt($("meta.ratio-data").attr("denominator"));

    alert("Num: " + numer + "    Denom: " + denom);

    //var result;

    //$("span#pendingTime").text(result);
}

function bindForm(dialog) {
    $("button.confirm-apply").on("click", function (e) {
        e.preventDefault();
        $("form").submit();
    });

    // need the ratio numbers

    $("#myModalContent").on("input", "#hoursApplied #minutesApplied", updatePendingTime);

    //$("#myModalContent").on("input", "input", updatePendingTime);

    // any <form> tag, the context in which that form tag is found
    $("form#modalForm", dialog).submit(function () {
        // Why is this not called when hitting the submit button?
        //alert("entered submit function");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function success(result) {
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

