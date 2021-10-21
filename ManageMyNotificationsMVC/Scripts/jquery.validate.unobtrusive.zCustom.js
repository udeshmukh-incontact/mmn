(function ($) {
  $.validator.unobtrusive.parseDynamicContent = function (selector) {
    //use the normal unobstrusive.parse method
    $.validator.unobtrusive.parse(selector);
 
    //get the relevant form
    var form = $(selector).first().closest('form');
   
    //get the collections of unobstrusive validators, and jquery validators
    //and compare the two
    var unobtrusiveValidation = form.data('unobtrusiveValidation');
    if (unobtrusiveValidation === null) {
       // form.data('unobtrusiveValidation', { options: { rules: {} } });
        unobtrusiveValidation = { options: { rules: {} } };
    }
    var validator = form.validate();
 
    $.each(unobtrusiveValidation.options.rules, function (elname, elrules) {
      if (validator.settings.rules[elname] == undefined) {
        var args = {};
        $.extend(args, elrules);
        args.messages = unobtrusiveValidation.options.messages[elname];
        //edit:use quoted strings for the name selector
        $("[name='" + elname + "']").rules("add", args);
      } else {
        $.each(elrules, function (rulename, data) {
          if (validator.settings.rules[elname][rulename] == undefined) {
            var args = {};
            args[rulename] = data;
            args.messages = unobtrusiveValidation.options.messages[elname][rulename];
            //edit:use quoted strings for the name selector
            $("[name='" + elname + "']").rules("add", args);
          }
        });
      }
    });

    $(selector + ' input[data-val-required], ' + selector + ' select[data-val-required], ' + selector + ' textarea[data-val-required]').each(function () {
        if ($(this).attr('type') !== 'checkbox' && $(this).attr('type') !== 'hidden')
            $('label[for=' + $(this).attr('name') + ']').addClass('required-label');
    });
  }
})($);

$(document).ready(function () {
    $('input[data-val-required], select[data-val-required], textarea[data-val-required]').each(function () {
        if ($(this).attr('type') !== 'checkbox' && $(this).attr('type') !== 'hidden')
            $('label[for=' + $(this).attr('id') + ']').addClass('required-label');
    });
});