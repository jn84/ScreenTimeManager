/**
 * index.js
 * - All our useful JS goes here, awesome!
 */
var isCounting = false;

var seconds = 360000000;

//console.log("JavaScript is amazing!");

$("button#btn-timer-toggle").click(function() {
  var button = $("button#btn-timer-toggle");
  if (button.hasClass("btn-start")) {
    button.removeClass("btn-start");
    button.addClass("btn-stop");
    button.html("Stop");
    isCounting = true;
  } else if (button.hasClass("btn-stop")) {
    button.removeClass("btn-stop");
    button.addClass("btn-start");
    button.html("Start");
    isCounting = false;
  }
  button.blur();
});

var decrement = function(integer) {
  integer--;
  if (integer < 0) {
    $("p.time-number").css("background-color", "pink");
    return integer;
  }
  $("p.time-number").css("background-color", "lightgreen");
  return integer;
}

var parseCounter = function() {
  var counter = seconds;

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

$(document).ready(function() {
  console.log("document ready");
  parseCounter();

  setInterval(function() {
    if (isCounting) {
      seconds = decrement(seconds);
      parseCounter();
    }
  }, 1000);
});
