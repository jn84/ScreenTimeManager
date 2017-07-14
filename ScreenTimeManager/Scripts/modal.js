var numerator;
var denominator;

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
    // grab data here. Could we pass it in? Look for a better way than storing it in the page

    var numerator = parseInt($("meta.ratio-data").attr("numerator"));
    var denominator = parseInt($("meta.ratio-data").attr("denominator"));

    $("button.confirm-apply").on("click", function (e) {
        e.preventDefault();
        $("form").submit();
    });

    // need the ratio numbers

    ///// BEGIN: Applies only to variable rules

    // AHA moment. Learning javascript. Events will "fire" immediately if the function used is pass with parentheses
    // as it sees the paramater as the result of the function. Do it without the parentheses, and it will see it as 
    // a reference to the function (the function itself is now the paramater)
    $("#myModalContent").on("input", "#minutesApplied", updatePendingTime);
    $("#myModalContent").on("input", "#hoursApplied", updatePendingTime);

    function updatePendingTime() {

        if (parseInt($("input#hoursApplied").val()) < 0)
            $("input#hoursApplied").val(0);

        if (parseInt($("input#minutesApplied").val()) < 0)
            $("input#minutesApplied").val(0);


        var result = "";

        var hours = parseInt($("input#hoursApplied").val());
        var minutes = parseInt($("input#minutesApplied").val());

        var totalSeconds = Math.floor(((hours * 60 + minutes) * 60) * (numerator / denominator));

        var temp = Math.abs(Math.floor(totalSeconds / 3600));
        result += temp + "h ";
        totalSeconds -= temp * 3600;

        temp = Math.abs(Math.floor(totalSeconds / 60));
        result += temp + "m ";
        totalSeconds -= temp * 60;

        result += totalSeconds + "s";
        
        $("span#pendingTime").text(result);
    }

    ///// END: Applies only to variable rules
    
    // any <form> tag, the context in which that form tag is found
    $("form#modalForm", dialog).submit(function () { // Why is this not called when hitting the submit button?
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