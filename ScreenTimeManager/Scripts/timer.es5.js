/**
 * index.js
 * - All our useful JS goes here, awesome!
 */
"use strict";

var isCounting = false;

var seconds = 360000000;

//console.log("JavaScript is amazing!");

$("button#btn-timer-toggle").click(function () {
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

var decrement = function decrement(integer) {
  integer--;
  if (integer < 0) {
    $("p.time-number").css("background-color", "pink");
    return integer;
  }
  $("p.time-number").css("background-color", "lightgreen");
  return integer;
};

var parseCounter = function parseCounter() {
  var counter = seconds;

  var hrs = ~ ~(counter / 3600);
  counter = counter % 3600;
  var hourstens = ~ ~(hrs / 10);
  var hoursones = hrs % 10;

  var mins = ~ ~(counter / 60);
  counter = counter % 60;
  var minutestens = ~ ~(mins / 10);
  var minutesones = mins % 10;

  var secs = counter;
  var secondstens = ~ ~(secs / 10);
  var secondsones = secs % 10;

  $("p.hours-tens").html(Math.abs(hourstens));
  $("p.hours-ones").html(Math.abs(hoursones));
  $("p.minutes-tens").html(Math.abs(minutestens));
  $("p.minutes-ones").html(Math.abs(minutesones));
  $("p.seconds-tens").html(Math.abs(secondstens));
  $("p.seconds-ones").html(Math.abs(secondsones));
};

$(document).ready(function () {
  console.log("document ready");
  parseCounter();

  setInterval(function () {
    if (isCounting) {
      seconds = decrement(seconds);
      parseCounter();
    }
  }, 1000);
});

//$(function () {
//    // Initialize numeric spinner input boxes
//    //$(".numeric-spinner").spinedit();
//    // Initialize modal dialog
//    // attach modal-container bootstrap attributes to links with .modal-link class.
//    // when a link is clicked with these attributes, bootstrap will display the href content in a modal dialog.
//    $('body').on('click', '.modal-link', function (e) {
//        e.preventDefault();
//        $(this).attr('data-target', '#modal-container');
//        $(this).attr('data-toggle', 'modal');
//    });
//    // Attach listener to .modal-close-btn's so that when the button is pressed the modal dialog disappears
//    $('body').on('click', '.modal-close-btn', function () {
//        $('#modal-container').modal('hide');
//    });
//    //clear modal cache, so that new content can be loaded
//    $('#modal-container').on('hidden.bs.modal', function () {
//        $(this).removeData('bs.modal');
//    });
//    $('#CancelModal').on('click', function () {
//        return false;
//    });
//});

