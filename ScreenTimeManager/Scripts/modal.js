var numerator;
var denominator;

$(function() {
    $.ajaxSetup({ cache: false }); ///////////////////////////////////// Document never ready

    $("a[data-modal]").on("click",
        function(e) {
            $("#myModalContent").load(this.href,
                function() {

                    $("#myModal").modal({
                            backdrop: "static",
                            keyboard: true
                        },
                        "show");

                    bindForm(this);
                });

            return false;
        });
});

function bindForm(dialog) {

    // So that the interval can be reset when the modal input is modified
    var timeoutId = null;

    ///// BEGIN: Applies only to variable rules

    // AHA moment. Learning javascript. Events will "fire" immediately if the function used is passed with parentheses
    // as it sees the paramater as the result of the function. Do it without the parentheses, and it will see it as 
    // a reference to the function (the function itself is now the paramater)
    $("#myModalContent").on("input", "#minutes-applied", beginUpdateTimeout);
    $("#myModalContent").on("input", "#hours-applied", beginUpdateTimeout);

    function jsonifyForm() {
        var rawData = $("input.modal-input").serializeArray();
        var output = {};

        // convert to key/value pairs
        $.each(rawData,
            function() {
                output[this.name] = this.value;
            });
        return JSON.stringify(output);

    }

    function beginUpdateTimeout() {
        if (timeoutId === null) {
            // TODO: Find a reasonable value for the timeout length. Maybe it doesn't really matter
            timeoutId = setTimeout(updatePendingTime, 1500);
            return;
        }
        // reset interval
        clearTimeout(timeoutId);
        timeoutId = setTimeout(updatePendingTime, 1500);
    }

    function updatePendingTime() {

        timeoutId = null;

        $.ajax({
            url: "RuleBases/UpdatePendingTime", // How to make this relative/variable?
            type: "POST",
            data: { "formData": jsonifyForm },
            success: function(result) {
                var spanRef = $("span#pendingTime");
                if (result.success) {
                    if (result.modifier === "add") { // Time is positive
                        spanRef.text("+ " + result.timespan);
                        spanRef.addClass("pendingTime-add");
                        spanRef.removeClass("pendingTime-neutral");
                        spanRef.removeClass("pendingTime-subtract");
                    } else {
                        spanRef.text("- " + result.timespan);
                        spanRef.addClass("pendingTime-subtract");
                        spanRef.removeClass("pendingTime-neutral");
                        spanRef.removeClass("pendingTime-add");
                    }
                } else {
                    spanRef.text("I AM ERROR");
                }
            }
        });
    }

    ///// END: Applies only to variable rules

    // Submit the form to the server with a standard (see: easily styled) button, rather
    // than via a form input tag
    $("button#modal-submit").on("click",
        function(e) {
            e.preventDefault();
            $("form#modalForm").submit();
        });

    // any <form> tag, the context in which that form tag is found
    $("form#modalForm", dialog).submit(function() { 
        //alert("entered submit function");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function(result) {
                if (result.success) {
                    $("#myModal").modal("hide");
                    // Send to appropriate url
                    if (result.redirectUrl === null) {
                        location.reload();
                    } else {
                        location.replace(result.redirectUrl);
                    }
                    // There was a validation error or something, so update the modal
                } else {
                    $("#myModalContent").html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}