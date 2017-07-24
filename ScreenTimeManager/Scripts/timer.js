var seconds = 360000000;

//console.log("JavaScript is amazing!");

$(function () {
    bindTimer();

    parseCounter();
});

var bindTimer = function () {
    var totalTimerSeconds = 0;

    var timerIntervalId = null;

    var timerHub = $.connection.timerSyncHub;

    timerHub.client.doTimerStateUpdate = function (isRunning) {

        updateTimerView(isRunning);

        if (isRunning && timerIntervalId == null) {
            timerIntervalId = startTimer();
            parseCounter(totalTimerSeconds);
        } else if (!isRunning && timerIntervalId != null) {
            timerIntervalId = stopTimer(timerIntervalId);
            parseCounter(totalTimerSeconds);
        } else {
            // alert("The timer inverval id was null when it shouldn't be!");
        }
    }

    timerHub.client.doTimerValueUpdate = function (totalSeconds) {
        totalTimerSeconds = totalSeconds;
        parseCounter(totalTimerSeconds);
    }

    // Rework so that the server triggers a change in button state
    $("button#btn-timer-toggle").click(function () {
        timerHub.server.toggleTimerState();
    });

    $.connection.hub.start().done(function() {
        timerHub.server.syncTimer();
    });

    var decrement = function (integer) {
        integer--;
        if (integer < 0) {
            $("p.time-number").css("background-color", "pink");
            return integer;
        }
        $("p.time-number").css("background-color", "lightgreen");
        return integer;
    }

    var startTimer = function () {
        return setInterval(function () {
            totalTimerSeconds = decrement(totalTimerSeconds);
            parseCounter(totalTimerSeconds);
        }, 1000);
    }

    var stopTimer = function (intervalId) {
        clearInterval(intervalId);
        return null;
    }
    // Do we need to call this again?
}


var updateTimerView = function (isRunning) {

    var button = $("button#btn-timer-toggle");

    if (isRunning) {
        button.removeClass("btn-start");
        button.addClass("btn-stop");
        button.html("Stop");
    } else {
        button.removeClass("btn-stop");
        button.addClass("btn-start");
        button.html("Start");        
    }
    button.blur();
}

var parseCounter = function(inputSeconds) {
  var counter = inputSeconds;

  var hrs = ~~(counter / 3600);
  counter = counter % 3600;
  var hourstens = ~~(hrs / 10);
  var hoursones = hrs % 10;

  var mins = ~~(counter / 60);
  counter = counter % 60;
  var minutestens = ~~(mins / 10);
  var minutesones = mins % 10;

  var secs = counter;
  var secondstens = ~~(secs / 10);
  var secondsones = secs % 10;

  $("p.hours-tens").html(Math.abs(hourstens));
  $("p.hours-ones").html(Math.abs(hoursones));
  $("p.minutes-tens").html(Math.abs(minutestens));
  $("p.minutes-ones").html(Math.abs(minutesones));
  $("p.seconds-tens").html(Math.abs(secondstens));
  $("p.seconds-ones").html(Math.abs(secondsones));
};

