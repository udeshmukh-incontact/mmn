$(function () {

    //$.validator.addMethod("endTimeValidate", function (value, element, options) {
    //    var startTime = new Date("01/01/1900 " + $("#timeFrameStartTime").val());
    //    var endTime = new Date("01/01/1900 " + value);
    //    return startTime <= endTime;
    //}, "End Time should be greater than or equal to Start Time.");

    //$.validator.addMethod("daysValidate", function (value, element, options) {
    //    var isSelected = false;
    //    $($(element.parentElement).find("label.btn")).each(function () {
    //        if ($(this).hasClass("active")) {
    //            isSelected = true;
    //            return;
    //        }
    //    });

    //    return isSelected;
    //}, "At least one selected day is required.");

    //$("#manageTimeFrameForm").validate({
    //    rules: {
    //        timeFrameEndTime: { endTimeValidate: true },
    //        timeFrameDays: { daysValidate: true }
    //    },
    //    onfocusout: function (element) {
    //        this.element(element);
    //    },
    //    errorPlacement: function (error, element) {
    //        if (element.attr("name") == "timeFrameStartTime")
    //            error.appendTo('#timeFrameStartTimeError');
    //        else if (element.attr("name") == "timeFrameEndTime")
    //            error.appendTo('#timeFrameEndTimeError');
    //        else
    //            error.insertAfter(element);
    //    }
    //});

    $.validator.addMethod("emailValidate", function (value, element, options) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(value);
    }, "Please enter a valid email.");

    $.validator.addMethod("phoneNumberValidate", function (value, element, options) {
        var regex = /^\d+$/;
        return regex.test(value);
    }, "Phone number should be a valid phone number with no spaces or dashes.");  
});
//"Phone numbers should be 10 digits US phone numbers with no spaces or dashes."