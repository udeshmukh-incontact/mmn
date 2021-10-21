(function ($) {
    "use strict";

    var replaceCheckboxElement = function (checkbox, element) {
        var value = element.val(),
            id = element.attr('id'),
            className = element.attr('class'),
            style = element.attr('style'),
            dataDeviceType = element.attr('data-device-type'),
            checked = !!element[0].checked,
            welNew = $('<div></div>');

        element.replaceWith(welNew);

        if (id) { welNew.attr('id', id) }
        if (className) { welNew.attr('class', className) }
        welNew.addClass('bootstrap-checkbox');
        if (style) { welNew.attr('style', style); }
        if (dataDeviceType) { welNew.attr('data-device-type', dataDeviceType); }
        if (checked) { welNew.addClass('checked'); }

        checkbox.value = value;
        checkbox.checked = checked;
        checkbox.element = welNew;
    };

    var changeCheckView = function (element, checked) {
        element.removeClass('ambiguous');
        element.removeClass('checked');

        if (checked === null) {
            element.addClass('ambiguous');
            element.html('<i class="fa fa-minus-square-o"></i>');
        } else if (checked) {
            element.addClass('checked');
            element.html('<i class="fa fa-check-square-o"></i>');
        } else {
            element.html('<i class="fa fa-square-o"></i>');
        }
    };

    var attachEvent = function (checkbox, element) {
        element.on('click', function (e) {
            var checked;
            if (checkbox.checked) {
                checked = false;
            }  else {
                checked = true;
            }

            checkbox.checked = checked;
            changeCheckView(checkbox.element, checked);

            checkbox.element.trigger({
                type: 'check',
                value: checkbox.value,
                checked: checked,
                element: checkbox.element
            });
        });
    };

    var Checkbox = function (element, options) {
        replaceCheckboxElement(this, element);
        attachEvent(this, this.element);
        if (options && options.label) {
            attachEvent(this, $(options.label));
        }
    };

    $.fn.extend({
        checkbox: function (options) {
            var aReplaced = $(this.map(function () {
                var $this = $(this),
                    checkbox = $this.data('checkbox');

                if (!checkbox) {
                    checkbox = new Checkbox($this, options);
                    checkbox.element.data('checkbox', checkbox);
                }

                return checkbox.element[0];
            }));

            aReplaced.selector = this.selector;
            return aReplaced;
        },

        chbxVal: function (value) {
            var $this = $(this[0]);
            var checkbox = $this.data('checkbox');

            if (!checkbox) {
                return;
            }
            if ($.type(value) === "undefined") {
                return checkbox.value;
            } else {
                checkbox.value = value;
                $this.data('checkbox', checkbox);
            }
        },

        chbxChecked: function (checked) {
            var $this = $(this[0]);
            var checkbox = $this.data('checkbox');

            if (!checkbox) {
                return;
            }
            if ($.type(checked) === "undefined") {
                return checkbox.checked;
            } else {
                checkbox.ambiguous = checked === null;
                changeCheckView($this, checked);

                checkbox.checked = checked;
                $this.data('checkbox', checkbox);
            }
        }
    });
})(jQuery);