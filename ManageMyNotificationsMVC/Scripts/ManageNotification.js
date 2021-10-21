/// <reference path="jquery-2.1.4.js" />

var ManageNotification = {};

(function () {
    var notificationManager = {
        person: {},
        devices: [],
        notificationGroup: [],
    };

    var isNotificationProfileComplete = true;
    var countryCodes = [];

    var TimeFrame = function (timeZone, name, startTime, endTime, days) {
        this.timeZone = notificationManager.person.timeZone;
        this.name = name;
        this.startTime = startTime;
        this.endTime = endTime;
        this.days = days;

        this.formatTime = function () {
            this.startTime = moment(this.startTime, ["HH:mm"]).format("h:mm A");
            this.endTime = moment(this.endTime, ["HH:mm"]).format("h:mm A");
        };
    };

    var Person = function (person) {
        if (person !== null) {
            this.id = person.Id;
            this.targetName = person.TargetName;
            this.firstName = person.FirstName;
            this.lastName = person.LastName;
            this.timeZone = person.TimeZone;
            this.Status = person.Status;
        }
        else {
            person = null;
        }
    };

    var Device = function (uiId, id, deviceType, emailAddress, phoneNumber, timeFrame, countryCode, countryDialCode) {
        this.uiDeviceId = uiId;
        this.id = id;
        this.deviceType = deviceType;
        this.emailAddress = emailAddress;
        this.phoneNumber = phoneNumber;
        this.countryCode = countryCode;
        this.countryDialCode = countryDialCode;
        this.timeFrame = timeFrame;
    };

    var Notification = function (notification) {
        this.accountBU = [];
        this.accountBU.push(notification.AccountBU);
        this.ParentAccountIDs = [];
        this.ParentAccountIDs.push(notification.ParentAccountIDs);
        this.productCluster = notification.ProductCluster;
        this.PartnerPrefix = notification.PartnerPrefix;
        this.eventsGroupId = notification.EventsGroupId;
        this.eventsGroupName = notification.EventsGroupName;
        this.maintainanceGroupId = notification.MaintainanceGroupId;
        this.maintainanceGroupName = notification.MaintainanceGroupName;
        this.notifications = [];
        that = this;
        $.each(notification.DeviceNotifications, function (i, n) {
            that.notifications.push({ deviceId: n["DeviceId"], deviceType: getDeviceTypeByxmValue(n["DeviceType"]), event: n["IsEventsSubscribed"], maintainance: n["IsMaintainanceSubscribed"] });
        });
    };

    var deviceType = [
        { value: "primaryEmail", text: "Primary Email", xmValue: "Work Email", xmType: "EMAIL", isCreated: false, isEditing: false, isTimeFrame: false },
        { value: "secondaryEmail", text: "Secondary Email", xmValue: "Secondary Email", xmType: "EMAIL", isCreated: false, isEditing: false, isTimeFrame: false },
        { value: "smsPhone", text: "SMS Phone", xmValue: "SMS Phone", isCreated: false, xmType: "TEXT_PHONE", isEditing: false, isTimeFrame: true }
    ];

    var timeFramePreviousTabId = [];

    var selectedTimeFrames = [];

    var updateTooltip = function (selector, tooltipText) {
        $(selector).each(function () {
            $(this).removeAttr("title");
            $(this).attr("title", tooltipText);
        });
    };

    var NotificationTab = function () {
        this.sortColumn = "accountBU";
        this.selectedAccountBU = "";
        this.selectedProductCluster = "";
        this.sortOrder = "asc";
        var self = this;

        this.addOrMergeGroup = function (group) {
            var filterGroup = $.grep(notificationManager.notificationGroup, function (g) {
                return g.eventsGroupName === group.eventsGroupName && g.maintainanceGroupName === group.maintainanceGroupName;
            });
            if (filterGroup.length > 0) {
                filterGroup[0].accountBU.push(group.accountBU[0]);
                filterGroup[0].accountBU.sort();
            }
            else {
                notificationManager.notificationGroup.push(group);
            }
        };

        this.load = function () {
            $("table.notification-group thead tr td span.sort-col").bind("click", this.onNotificationGroupSort);
            $("#account-buselect").bind("change", this.onAccountBUchange);
            $("#product-clusterselect").bind("change", this.onProductClusterchange);
            $("#account-bu-col").attr("data-column-name", "accountBU");
            $("#product-cluster-col").attr("data-column-name", "productCluster");
            $("#check-all-notification").checkbox().chbxChecked(false);
            $("#check-all-notification").bind("click", self.allCheckboxChecked);
        };

        this.checkNotificationFilter = function (group) {
            var filtergroup = false;
            if (self.selectedAccountBU === "" && self.selectedProductCluster === "") {
                filtergroup = true;
            }
            else {
                filtergroup = self.selectedAccountBU === "" && self.selectedProductCluster !== "" ?
                    group.productCluster === self.selectedProductCluster : self.selectedAccountBU !== "" && self.selectedProductCluster === "" ?
                        group.accountBU.includes(self.selectedAccountBU) : group.accountBU.includes(self.selectedAccountBU) && group.productCluster === self.selectedProductCluster;
            }
            return filtergroup;
        };

        this.reBindGrid = function () {
            resetTimer();
            self.setSelectedDropDownValues();
            $("table.notification-group thead tr td.device-type").remove();
            $("table.notification-group tbody").remove();
            $("table.notification-group span.max-rows-msg").remove();
            if (notificationManager.devices.length > 0) {
                $.each(notificationManager.devices, function (index, device) {
                    var filterdeviceType = $.grep(deviceType, function (d) {
                        return d.value === device.deviceType;
                    });
                    if (filterdeviceType) {
                        $("table.notification-group thead tr").append("<td class='device-type'><input type='checkbox' data-device-type='" + device.deviceType + "' /> " + filterdeviceType[0].text + "</td>");
                    }
                });

                $("table.notification-group thead tr td input").checkbox().chbxChecked(false);
                $("table.notification-group thead tr td div.bootstrap-checkbox").bind("click", this.headerCheckboxChecked);
            }
            if (notificationManager.notificationGroup.length > 0 && (notificationManager.notificationGroup.length <= 100 || self.selectedAccountBU !== "" || self.selectedProductCluster !== "")) {
                var index = 0;
                $("table.notification-group").append("<tbody></tbody>");
                $.each(notificationManager.notificationGroup, function (groupIndex, group) {

                    if (self.checkNotificationFilter(group)) {
                        index++;
                        $("table.notification-group tbody").append("<tr id='" + group.uiNotificationId + "'></tr>");
                        if (index % 2 !== 0) {
                            $("table.notification-group tbody tr[id='" + group.uiNotificationId + "']").addClass("odd-tr");
                        }
                        var groupText = "<ul>";
                        if (group.accountBU.length > 1) {
                            groupText = "<ul class='bulletstyle'>"
                        }
                        $.each(group.accountBU, function (gIndex, g) {
                            groupText += "<li title='" + g + "'>" + g + "</li>";
                        });
                        groupText += "</ul>";
                        $("table.notification-group tbody tr[id='" + group.uiNotificationId + "']").append("<td>" + groupText + "</td>");
                        $("table.notification-group tbody tr[id='" + group.uiNotificationId + "']").append("<td title='" + group.productCluster + "'>" + group.productCluster + "</td>");
                        $.each(notificationManager.devices, function (deviceIndex, device) {
                            var notificationDeviceType = $.grep(group.notifications, function (d) {
                                return d.deviceType === device.deviceType;
                            });
                            if (notificationDeviceType.length > 0) {
                                $("table.notification-group tbody tr[id='" + group.uiNotificationId + "']").append('<td class="device-column-' + notificationDeviceType[0].deviceType + '"></td>');
                                $("table.notification-group tbody tr[id='" + group.uiNotificationId + "'] td.device-column-" + notificationDeviceType[0].deviceType).append('<div> <input type="checkbox" ' + (notificationDeviceType[0].event ? 'checked="checked"' : '') + ' data-group-id="' + group.uiNotificationId + '" data-device-type="' + notificationDeviceType[0].deviceType + '" data-notification-type="event" /> Events</div>');
                                if (group.PartnerPrefix == "" || group.PartnerPrefix == "INC") {
                                    $("table.notification-group tbody tr[id='" + group.uiNotificationId + "'] td.device-column-" + notificationDeviceType[0].deviceType).append('<div> <input type="checkbox" ' + (notificationDeviceType[0].maintainance ? 'checked = "checked"' : '') + ' data-group-id="' + group.uiNotificationId + '" data-device-type="' + notificationDeviceType[0].deviceType + '" data-notification-type="maintainance" /> Maintenance</div >');
                                }
                            }
                        });
                    }
                });
                $("table.notification-group tbody tr td input").bind("change", this.checkboxChecked);
                self.enableCheckboxes();
            } else {
                if (notificationManager.notificationGroup.length > 0) {
                    $("table.notification-group").append("<span class='max-rows-msg'>Maximum rows exceeded, please select Business Unit</span>");
                }
                self.disableCheckboxes();
            }
            self.bindNotificationGroupSelect();
            self.checkAllCheckboxChecked();
            self.isValid();
            resizePageControl();
        };

        this.disableCheckboxes = function () {
            resetTimer();
            $("#check-all-notification").addClass("chk-all-disabled");
            $("table.notification-group thead tr td div.bootstrap-checkbox").addClass("chk-hdr-disabled");
            $("table.notification-group thead tr td div.bootstrap-checkbox").off("click");
        };

        this.enableCheckboxes = function () {
            resetTimer();
            $("#check-all-notification").removeClass("chk-all-disabled");
            $("table.notification-group thead tr td div.bootstrap-checkbox").removeClass("chk-hdr-disabled");
            $("table.notification-group thead tr td div.bootstrap-checkbox").bind("click", self.headerCheckboxChecked);
        };

        this.onNotificationGroupSort = function () {
            resetTimer();
            if (self.sortColumn !== $(this).attr("data-column-name"))
                self.sortOrder = "";
            self.sortColumn = $(this).attr("data-column-name");
            if (self.sortOrder === "" || self.sortOrder === "desc")
                self.sortOrder = "asc";
            else
                self.sortOrder = "desc";
            notificationManager.notificationGroup.sort(self.sortCompare);
            if (self.sortOrder === "desc") {
                notificationManager.notificationGroup.reverse();
            }
            self.reBindGrid();
            self.manageSortIcons($(this));
        };

        this.sortCompare = function (nGroup1, nGroup2) {
            if (!self.sortColumn) {
                self.sortColumn = "accountBU";
            }
            var firstValue = "";
            var secondValue = "";
            if (self.sortColumn === "accountBU") {
                firstValue = nGroup1[self.sortColumn][0].toLowerCase();
                secondValue = nGroup2[self.sortColumn][0].toLowerCase();
            }
            else {
                firstValue = nGroup1[self.sortColumn].toLowerCase();
                secondValue = nGroup2[self.sortColumn].toLowerCase();
            }
            return firstValue < secondValue ? -1 : firstValue > secondValue ? 1 : 0;
        };

        this.manageSortIcons = function (currentTarget) {
            resetTimer();
            $("table.notification-group thead tr td i.fa").removeClass("fa-sort-up");
            $("table.notification-group thead tr td i.fa").removeClass("fa-sort-down");
            if (self.sortOrder === "asc")
                currentTarget.find("i.fa").addClass("fa-sort-up");
            else
                currentTarget.find("i.fa").addClass("fa-sort-down");
        };

        this.setSelectedDropDownValues = function () {
            resetTimer();
            self.selectedAccountBU = $('#account-buselect').val();
            self.selectedProductCluster = $('#product-clusterselect').val();
        };

        this.onAccountBUchange = function () {
            resetTimer();
            self.selectedAccountBU = $('#account-buselect').val();
            self.reBindGrid();
        };
        this.onProductClusterchange = function () {
            resetTimer();
            self.selectedProductCluster = $('#product-clusterselect').val();
            self.reBindGrid();
        };

        this.bindNotificationGroupSelect = function () {
            $('#account-buselect')
                .find('option')
                .remove()
                .end().append($('<option value="" title="All"> All </option>'));

            $('#product-clusterselect')
                .find('option')
                .remove()
                .end().append($('<option value="" title="All"> All </option>'));

            if (notificationManager.notificationGroup.length > 0) {
                var correspondingBU = self.selectedProductCluster !== "" ? $.grep(notificationManager.notificationGroup, function (d) { return d.productCluster === self.selectedProductCluster; }) :
                    JSON.parse(JSON.stringify(notificationManager.notificationGroup));

                var correspondingProductCluster = self.selectedAccountBU !== "" ? $.grep(notificationManager.notificationGroup, function (d) { return d.accountBU.includes(self.selectedAccountBU); }) :
                    JSON.parse(JSON.stringify(notificationManager.notificationGroup));
            }

            if (correspondingBU) {
                var buFiltered = [];
                $.each(correspondingBU, function (index, value) {
                    $.each(value.accountBU, function (indexBU, valueBU) {
                        buFiltered.push(valueBU);
                    });
                });
                var uniqueBU = buFiltered.map(function (item) { return item; }).filter(function (value, index, self) { return self.indexOf(value) === index; }).sort(function (firstValue, secondValue) { return firstValue.toLowerCase() < secondValue.toLowerCase() ? -1 : firstValue.toLowerCase() > secondValue.toLowerCase() ? 1 : 0; });
            }

            if (correspondingProductCluster) {
                var uniqueProductCluster = correspondingProductCluster.map(function (item) { return item.productCluster; }).filter(function (value, index, self) { return self.indexOf(value) === index; }).sort(function (firstValue, secondValue) { return firstValue.toLowerCase() < secondValue.toLowerCase() ? -1 : firstValue.toLowerCase() > secondValue.toLowerCase() ? 1 : 0; });
            }

            if (uniqueBU) {
                $.each(uniqueBU, function (key, value) {
                    $('#account-buselect').append($('<option value="' + value + '" title="' + value + '">' + value + '</option > '));
                });
                reSizeOptionText();
            }
            if (uniqueProductCluster) {
                $.each(uniqueProductCluster, function (key, value) {
                    $('#product-clusterselect').append($('<option value="' + value + '" title="' + value + '"> ' + value + '</option > '));
                });
                reSizeOptionText();
            }

            if (self.selectedAccountBU !== "") {
                $('#account-buselect').val(self.selectedAccountBU);
            }
            if (self.selectedProductCluster !== "All") {
                $('#product-clusterselect').val(self.selectedProductCluster);
            }
        };


        this.checkboxChecked = function () {
            resetTimer();
            var groupId = $(this).attr("data-group-id");
            var deviceType = $(this).attr("data-device-type");
            var notificationType = $(this).attr("data-notification-type");
            var isChecked = $(this).is(":checked");
            var group = $.grep(notificationManager.notificationGroup, function (g) {
                return g.uiNotificationId === groupId;
            });
            if (group.length > 0) {
                var notification = $.grep(group[0].notifications, function (n) {
                    return n.deviceType === deviceType;
                });
                if (notification.length > 0) {
                    notification[0][notificationType] = isChecked;
                }
            }
            self.checkAllCheckboxChecked();
            self.isValid();
            $("#submit-notification, #cancel-notification").attr("disabled", false);
            verifyImpersonationMode();
            resizePageControl();
        };

        this.headerCheckboxChecked = function () {
            resetTimer();
            var deviceType = $(this).attr("data-device-type");
            var isChecked = $(this).hasClass("checked") ? true : $(this).hasClass("ambiguous") ? null : false;
            self.checkAllDeviceType(deviceType, isChecked);
            $("#submit-notification, #cancel-notification").attr("disabled", false);
            self.reBindGrid();
            isDirty = true;
            verifyImpersonationMode();
        };

        this.checkAllDeviceType = function (deviceType, isChecked) {
            resetTimer();
            $.each(notificationManager.notificationGroup, function (groupIndex, group) {
                if (self.checkNotificationFilter(group)) {
                    var notificationDeviceType = $.grep(group.notifications, function (d) {
                        return d.deviceType === deviceType;
                    });
                    if (notificationDeviceType.length > 0) {
                        notificationDeviceType[0].event = isChecked;
                        if (group.PartnerPrefix == "" || group.PartnerPrefix == "INC") {
                            notificationDeviceType[0].maintainance = isChecked;
                        }
                    }
                }
            });
        };

        this.allCheckboxChecked = function () {
            if ($("#check-all-notification").hasClass("chk-all-disabled")) {
                $("#check-all-notification").checkbox().chbxChecked(false);
                return;
            }
            var isChecked = $(this).hasClass("checked") ? true : $(this).hasClass("ambiguous") ? null : false;
            var createdDevices = $.grep(deviceType, function (d) {
                return d.isCreated === true;
            });
            $.each(createdDevices, function (index, dType) {
                self.checkAllDeviceType(dType.value, isChecked);
            });
            self.reBindGrid();
            isDirty = true;
            $("#submit-notification, #cancel-notification").attr("disabled", false);
            verifyImpersonationMode();
        };

        this.checkAllCheckboxChecked = function () {
            resetTimer();
            $.each(notificationManager.devices, function (index, device) {
                var totalDevicesCheckBox = $("#notificationGroups tbody").find("input[data-device-type='" + device.deviceType + "']").length;
                var totalDevicesCheckBoxChecked = $("#notificationGroups tbody").find("input[data-device-type='" + device.deviceType + "']:checked").length;
                $("#notificationGroups thead").find("div[data-device-type='" + device.deviceType + "']").checkbox().chbxChecked(totalDevicesCheckBoxChecked === 0 ? false : totalDevicesCheckBox === totalDevicesCheckBoxChecked ? true : null);
            });

            var totalCheckBox = $("#notificationGroups tbody").find("input[type=checkbox]").length;
            var totalCheckBoxChecked = $("#notificationGroups tbody").find("input[type=checkbox]:checked").length;
            $("#check-all-notification").checkbox().chbxChecked(totalCheckBoxChecked === 0 ? false : totalCheckBox === totalCheckBoxChecked ? true : null);
        };

        this.isValid = function () {
            isvalid = false;
            $.each(notificationManager.notificationGroup, function (index, ng) {
                $.each(ng.notifications, function (i, n) {
                    if (n.event === true || n.maintainance === true) {
                        isvalid = true;
                    }
                });
            });
            if (isvalid) {
                $("#errorSummaryNotification").hide();
            }
            else {
                $("#errorSummaryNotification").show();
            }
            toggleErrorSummaryContainer();
            return isvalid;
        };
    };

    var SaveNotificationManager = function () {
        resetTimer();
        this.Save = function () {
            var notificationObj = {
                "Person": {
                    "Id": notificationManager.person.id,
                    "TargetName": notificationManager.person.targetName,
                    "FirstName": notificationManager.person.firstName,
                    "LastName": notificationManager.person.lastName,
                    "TimeZone": notificationManager.person.timeZone,
                    "Status": notificationManager.person.Status
                },
                "Devices": [],
                "NotificationGroups": []
            };

            $.each(notificationManager.devices, function (i, d) {
                notificationObj.Devices.push({
                    "Id": d.id,
                    "Name": $.grep(deviceType, function (dt) { return dt.value === d.deviceType; })[0].xmValue,
                    "EmailAddress": d.emailAddress,
                    "DeviceType": $.grep(deviceType, function (dt) { return dt.value === d.deviceType; })[0].xmType,
                    "Description": d.deviceType === "primaryEmail" || d.deviceType === "secondaryEmail" ? d.emailAddress : d.phoneNumber,
                    "PhoneNumber": d.deviceType === "smsPhone" ? d.countryDialCode + d.phoneNumber : "",
                    "CountryCode": d.countryCode,
                    "DeviceTimeFrame": d.timeFrame
                });
            });

            $.each(notificationManager.notificationGroup, function (i, ng) {
                var DeviceNotification = [];
                $.each(ng.notifications, function (index, notification) {
                    DeviceNotification.push({
                        "DeviceId": notification.deviceId ? notification.deviceId : "",
                        "DeviceType": $.grep(deviceType, function (dt) { return dt.value === notification.deviceType; })[0].xmValue,
                        "IsEventsSubscribed": notification.event,
                        "IsMaintainanceSubscribed": notification.maintainance
                    });
                });

                notificationObj.NotificationGroups.push({
                    "AccountBU": ng.accountBU[0],
                    "ProductCluster": ng.productCluster,
                    "EventsGroupId": ng.eventsGroupId,
                    "EventsGroupName": ng.eventsGroupName,
                    "MaintainanceGroupId": ng.maintainanceGroupId,
                    "MaintainanceGroupName": ng.maintainanceGroupName,
                    "DeviceNotifications": DeviceNotification
                });
            });
            $("#loading-panel").show();
            $.when(postData("/notificationmanager/savenotification", notificationObj))
                .then(function (data) {
                    try {
                        switch (data) {
                            case "success":
                                clearForm();
                                $(".success-message").show();
                                deviceInit();
                                break;
                            case "invalidPhoneNumber":
                                $("#loading-panel").hide();
                                var smsDeviceType = $.grep(notificationManager.devices, function (d) { return d.deviceType === "smsPhone"; });
                                if (smsDeviceType) {
                                    $("input[name='val_" + smsDeviceType[0].uiDeviceId + "']").parent().append("<label class='smsPhone error'>Phone number should be a valid phone number with no spaces or dashes.</label>");
                                    $("#devicesTab").click();
                                }
                                break;
                            case "invalidPrimaryEmail":
                                $("#loading-panel").hide();
                                var emailPEDeviceType = $.grep(notificationManager.devices, function (d) { return d.deviceType === "primaryEmail"; });
                                if (emailPEDeviceType) {
                                    $("input[name='val_" + emailPEDeviceType[0].uiDeviceId + "']").parent().append("<label class='primaryEmail error'>Please enter a valid email.</label>");
                                    $("#devicesTab").click();
                                }
                                break;
                            case "invalidSecondaryEmail":
                                $("#loading-panel").hide();
                                var emailSEDeviceType = $.grep(notificationManager.devices, function (d) { return d.deviceType === "secondaryEmail"; });
                                if (emailSEDeviceType) {
                                    $("input[name='val_" + emailSEDeviceType[0].uiDeviceId + "']").parent().append("<label class='secondaryEmail error'>Please enter a valid email.</label>");
                                    $("#devicesTab").click();
                                }
                                break;
                            default:
                                clearForm();
                                window.location.href = "/error";
                                break;
                        }
                    }
                    catch (err) {
                        window.location.href = "/error";
                    }
                }, function () {
                    window.location.href = "/error";
                }).fail(function () {
                    $("#loading-panel").hide();
                });
        };
    };

    var manageTimeFramePopup = function () {
        this.isDateChanged = false;
        this.timeframes = [];
        this.open = function (deviceId) {
            resetTimer();
            $(".body-container").addClass("when-model-popup");
            $(".error-message").hide();
            var device = $.grep(notificationManager.devices, function (e) { return e.uiDeviceId === deviceId; })[0];
            this.bind(device.timeFrame, deviceId);
            $("#manageTimeFrameModal").modal();
            timeframeDataChanged = false;
        };

        this.close = function () {
            resetTimer();
            $(".body-container").removeClass("when-model-popup");
            $("#manageTimeFrameModal").modal("hide");
        };

        this.bindEvents = function () {
            $("#time-frame-tab").on("click", ".remove-tab", this.remove);
            $("#content-area-timeframe").on("change", "input.timeFrameName", this.nameChange);
            $("#time-frame-tab").on("click", "li", this.tabClick);
            $("#time-frame-tab").on('mousedown', "li a",
                function (event) {
                    event.preventDefault();
                }
            );
            $("#content-area-timeframe").on("click", ".timeFrameDays label.btn", this.dayChange);
        };

        this.bind = function (timeframe, deviceId) {
            $("#time-frame-tab").empty();
            $("#content-area-timeframe").empty();
            $("#timeFrameTimeZone").val(notificationManager.person.timeZone);
            var lastIndex = 0;
            var that = this;
            if (!(timeframe && typeof timeframe === 'object' && timeframe.constructor === Array)) {
                timeframe = [];
                timeframe.push(new TimeFrame("", "24x7", "12:00 AM", "12:00 AM", ["MO", "TU", "WE", "TH", "FR", "SA", "SU"]));
            }
            $.each(timeframe, function (index, time) {
                that.createTab(index, time);
                lastIndex = index + 1;
            });
            timeFramePreviousTabId = [];
            timeFramePreviousTabId.push("#tab-content-0");
            that.createAddTab(lastIndex);
            updateTooltip(".remove-tab", "Click to remove timeframe");
            $("#update-time-frame").attr("data-device-id", deviceId);
        };

        this.createTab = function (index, time) {
            var ulElement = "<li class='" + (index === 0 ? 'active' : '') + "' title='" + time.name + "'><a data-toggle='tab' href='#tab-content-" + index + "'><span class='name-" + index + "'>" + time.name + "</span>&nbsp;<i class='fa fa-close remove-tab is-saved'></i></a></li>";
            $("#time-frame-tab").append(ulElement);
            var tabId = "#tab-content-" + index;
            var tabElement = "<div id='tab-content-" + index + "' class='tab-pane fade " + (index === 0 ? 'in active' : '') + "'></div >";
            $("#content-area-timeframe").append(tabElement);
            $(".timeframe-template").clone().appendTo(tabId);

            $("#content-area-timeframe .timeframe-template").find(".timeFrameName").val(time.name);
            $("#content-area-timeframe .timeframe-template").find(".timeFrameName").attr("data-name", "name-" + index);
            $("#content-area-timeframe .timeframe-template").find(".timeFrameStartTime").val(time.startTime);
            $("#content-area-timeframe .timeframe-template").find(".timeFrameEndTime").val(time.endTime);
            $.each(time.days, function (dayindex, value) {
                $("#content-area-timeframe .timeframe-template").find(".timeFrameDays input[value='" + value + "']").parent().addClass("active");
            });
            $("#content-area-timeframe .timeframe-template").find('.input-group.date').datetimepicker({
                format: 'LT'
            }).on('dp.change', this.timeChange);
            $("#content-area-timeframe div").removeClass("timeframe-template");
        };

        this.createAddTab = function (index) {
            var ulNewElement = "<li title='Add new timeframe'><a data-toggle='tab' href='#tab-content-" + index + "'><i class='fa fa-plus add-timeframe'></i></a></li>";
            var tabElement = "<div id='tab-content-" + index + "' class='tab-pane fade'></div >";
            $("#time-frame-tab").append(ulNewElement);
            $("#content-area-timeframe").append(tabElement);
        };

        this.tabClick = function () {
            resetTimer();
            var timeframePopup = new manageTimeFramePopup();
            var tabId = $("#time-frame-tab li.active").find("a").attr("href");
            if (tabId !== undefined) {
                $(tabId).find(".error").removeClass("active");
                if (!timeframePopup.isCurrentTabValid(tabId))
                    return false;
                else {
                    timeFramePreviousTabId.push($(this).find("a").attr("href"));
                    var that = this;
                    if ($(this).find("i.add-timeframe").length !== 0) {
                        setTimeout(function () {
                            timeframePopup.add($(that).find("a").attr("href"));
                            $(tabId).find(".timeFrameName").focus();
                        }, 100);
                    }
                }
            }
        };

        this.update = function (deviceId) {
            resetTimer();
            var timeframePopup = new manageTimeFramePopup();
            var tabId = $("#time-frame-tab li.active").find("a").attr("href");
            if (timeframePopup.isCurrentTabValid(tabId) && timeframePopup.addToModel()) {
                var device = $.grep(notificationManager.devices, function (e) { return e.uiDeviceId === deviceId; });
                device[0].timeFrame = selectedTimeFrames;
                timeframeDataChanged = true;
                this.close();
            }
        };

        this.addToModel = function () {
            var timeframePopup = new manageTimeFramePopup();
            selectedTimeFrames = [];
            var hasDuplicate = false;
            $("#content-area-timeframe .tab-pane").each(function () {
                if (!hasDuplicate) {
                    var tabId = "#" + this.id;
                    timeframe = timeframePopup.setTimeFrameFromTab(tabId);
                    if (timeframe.name !== "" && timeframe.name !== undefined) {
                        var errorMsg = timeframePopup.getErrorMessage(timeframe);
                        if (errorMsg === "") {
                            selectedTimeFrames.push(timeframe);
                        }
                        else {
                            timeframePopup.showError(errorMsg);
                            hasDuplicate = true;
                        }
                    }
                }
            });
            return !hasDuplicate;
        };

        this.setTimeFrameFromTab = function (tabId) {
            var name = $(tabId).find(".timeFrameName").val();
            var startTime = $(tabId).find(".timeFrameStartTime").val();
            var endTime = $(tabId).find(".timeFrameEndTime").val();
            var days = [];
            $(tabId).find(".timeFrameDays input").each(function () {
                if ($(this).parent().hasClass("active"))
                    days.push($(this).val());
            });
            var timeframe = new TimeFrame("", name, startTime, endTime, days);
            return timeframe;
        };

        this.add = function (tabId) {
            var timeframePopup = new manageTimeFramePopup();
            $(".timeframe-template").clone().appendTo(tabId);
            var currentIndex = parseInt(tabId.replace("#tab-content-", ""));
            //$(tabId).unbind("click");
            //$("#time-frame-tab li:last a").click();
            $("i.add-timeframe").parent().parent().removeAttr("title");
            $("i.add-timeframe").parent().html("<span class='name-" + currentIndex + "'>New</span>&nbsp;<i class='fa fa-close remove-tab'></i>");
            var ulNewElement = "<li title='Add new timeframe'><a data-toggle='tab' href='#tab-content-" + (currentIndex + 1) + "'><i class='fa fa-plus add-timeframe'></i></a></li>";
            $("#time-frame-tab").append(ulNewElement);
            var tabElement = "<div id='tab-content-" + (currentIndex + 1) + "' class='tab-pane fade'></div >";
            $("#content-area-timeframe").append(tabElement);
            $('#content-area-timeframe div .timeframe-template .input-group.date').datetimepicker({
                format: 'LT'
            }).on('dp.change', timeframePopup.timeChange);
            $("#content-area-timeframe .timeframe-template").find(".timeFrameName").val("");
            $("#content-area-timeframe .timeframe-template").find(".timeFrameName").focus();
            $("#content-area-timeframe .timeframe-template").find(".timeFrameStartTime").val("12:00 AM");
            $("#content-area-timeframe .timeframe-template").find(".timeFrameEndTime").val("12:00 AM");
            $("#content-area-timeframe .timeframe-template").find(".timeFrameName").attr("data-name", "name-" + currentIndex);
            $("#content-area-timeframe .timeframe-template").find(".timeFrameDays input").parent().addClass("active");
            setTimeout(function () {
                $("#manageTimeFrameModal .error").hide();
            }, 10);
            setTimeout(function () {
                $(tabId).find("input.timeFrameName").focus();
            }, 500);
            $("#content-area-timeframe div").removeClass("timeframe-template");
            updateTooltip(".remove-tab", "Click to remove timeframe");
            $("#content-area-timeframe div").removeClass("timeframe-template");
            //$("#time-frame-tab .add-timeframe").parent().bind("click", timeframePopup.add);
        };

        this.remove = function () {
            resetTimer();
            var timeframePopup = new manageTimeFramePopup();
            if ($("#time-frame-tab li").length === 2) {
                timeframePopup.showError("You must have at least one time frame.");
            }
            else {
                var tabId = $(this).parent().attr("href");
                $(this).parent().parent().remove();
                $(tabId).remove();
                if ($(this).hasClass("is-saved")) {
                    timeframePopup.showError("You must click on Update on this screen, then Save on the next screen to delete the time frame(s).");
                }
                var isMatch = false;
                do {
                    var lastTab = timeFramePreviousTabId.pop();
                    if ($(lastTab).length !== 0) {
                        isMatch = true;
                        $("#time-frame-tab li a[href='" + lastTab + "']").click();
                    }
                } while (timeFramePreviousTabId.length !== 0 && !isMatch);
                if (!isMatch)
                    $("#time-frame-tab li:first a").click();
            }
        };

        this.showError = function (errorMsg) {
            $(".error-message").show();
            $(".error-message").text(errorMsg);
            setTimeout(function () {
                $(".error-message").text("");
                $(".error-message").hide("slow");
            }, 10000);
        };

        this.getErrorMessage = function (timeframe) {
            var isMatch = false;
            var errorMessage = "";
            $.each(selectedTimeFrames, function () {
                if (!isMatch) {
                    if (timeframe.name === this.name) {
                        isMatch = true;
                        errorMessage = "Duplicate time frame names are not allowed.";
                    }
                    if (!isMatch &&
                        timeframe.startTime === this.startTime &&
                        timeframe.endTime === this.endTime &&
                        $(timeframe.days).not(this.days).length === 0 &&
                        $(this.days).not(timeframe.days).length === 0) {
                        isMatch = true;
                        errorMessage = "A timeframe with these settings already exists. No two timeframes can have same settings.";
                    }
                }
            });
            return errorMessage;
        };

        this.nameChange = function () {
            var className = $(this).attr('data-name');
            var name = $(this).val();
            if (name === "") {
                $("span." + className).text("Blank Name");
                $("span." + className).parent().parent().attr("title", "Blank Name");
                $(this).parent().find("label.error").text("This field is required.");
                $(this).parent().find("label.error").show();
            }
            else {
                $("span." + className).parent().parent().attr("title", name);
                $("span." + className).text(name);
                $(this).parent().find("label.error").hide();
            }
        };

        this.timeChange = function (event) {
            var haserror = false;
            var value = $(this).find("input").val();
            if (value === "") {
                $(this).parent().find("label.error").text("This field is required.");
                $(this).parent().find("label.error").show();
            }
            else {
                $(this).parent().find("label.error").hide();
                if ($(this).find("input").hasClass("timeFrameEndTime")) {
                    var startTimeValue = $(this).parentsUntil(".tab-pane").find("input.timeFrameStartTime").val();
                    var startTime = new Date("01/01/1900 " + startTimeValue);
                    var endTime = new Date("01/01/1900 " + value);
                    if (startTime > endTime && startTimeValue !== "12:00 AM" && value !== "12:00 AM") {
                        haserror = true;
                        $(this).parent().find("label.error").text("End Time should be greater than or equal to Start Time.");
                        $(this).parent().find("label.error").show();
                    }
                }
                else {
                    startTime = new Date("01/01/1900 " + value);
                    var endTimeValue = $(this).parentsUntil(".tab-pane.active").find("input.timeFrameEndTime").val();
                    endTime = new Date("01/01/1900 " + endTimeValue);
                    if (startTime > endTime && endTimeValue !== "12:00 AM" && value !== "12:00 AM") {
                        haserror = true;
                        $("input.timeFrameEndTime").parent().parent().find("label.error").text("End Time should be greater than or equal to Start Time.");
                        $("input.timeFrameEndTime").parent().parent().find("label.error").show();
                    }
                }
                if (!haserror) {
                    $("input.timeFrameEndTime").parent().parent().find("label.error").hide();
                }
            }
        };

        this.dayChange = function () {
            setTimeout(function () {
                var tabId = $("#time-frame-tab li.active").find("a").attr("href");
                if ($(tabId).find(".timeFrameDays label.active").length === 0) {
                    $(tabId).find(".timeFrameDays").parent().find("label.timeFrameDaysError").text("At least one selected day is required.");
                    $(tabId).find(".timeFrameDays").parent().find("label.timeFrameDaysError").show();
                }
                else {
                    $(tabId).find(".timeFrameDays").parent().find("label.timeFrameDaysError").hide();
                }
            }, 100);
        };

        this.isCurrentTabValid = function (tabId) {
            var isValid = true;
            var isTime = true;
            var timeframePopup = new manageTimeFramePopup();
            var timeframe = timeframePopup.setTimeFrameFromTab(tabId);
            if (timeframe.name === "") {
                isValid = false;
                $(tabId).find("label.timeFrameNameError").text("This field is required.");
                $(tabId).find("label.timeFrameNameError").show();
            }
            else {
                $(tabId).find("label.timeFrameNameError").hide();
            }
            if (timeframe.startTime === "") {
                isTime = false;
                isValid = false;
                $(tabId).find("label.timeFrameStartTimeError").text("This field is required.");
                $(tabId).find("label.timeFrameStartTimeError").show();
            }
            else {
                $(tabId).find("label.timeFrameStartTimeError").hide();
            }
            if (timeframe.endTime === "") {
                isTime = false;
                isValid = false;
                $(tabId).find("label.timeFrameEndTimeError").text("This field is required.");
                $(tabId).find("label.timeFrameEndTimeError").show();
            }
            else {
                $(tabId).find("label.timeFrameEndTimeError").hide();
            }
            if (timeframe.days.length === 0) {
                isValid = false;
                $(tabId).find("label.timeFrameDaysError").text("At least one selected day is required.");
                $(tabId).find("label.timeFrameDaysError").show();
            }
            else {
                $(tabId).find("label.timeFrameDaysError").hide();
            }
            if (isTime) {
                var startTimeValue = timeframe.startTime;
                var endTimeValue = timeframe.endTime;
                var startTime = new Date("01/01/1900 " + startTimeValue);
                var endTime = new Date("01/01/1900 " + endTimeValue);
                if (startTime > endTime && endTimeValue !== "12:00 AM" && startTimeValue !== "12:00 AM") {
                    isValid = false;
                    $(tabId).find("label.timeFrameEndTimeError").text("End Time should be greater than or equal to Start Time.");
                    $(tabId).find("label.timeFrameEndTimeError").show();
                }
                else {
                    $(tabId).find("label.timeFrameEndTimeError").hide();
                }
            }
            if (isValid) {
                if (!timeframePopup.addToModel()) {
                    isValid = false;
                }
            }
            return isValid;
        };
    };

    var deviceIdCreater = 0;
    var timeframeDataChanged = false;
    var timeframeState = {
        startDate: "",
        endDate: ""
    };

    var clearForm = function () {
        isDirty = false;
        notificationManager.person = {};
        notificationManager.devices = [];
        notificationManager.notificationGroup = [];
        deviceIdCreater = 0;
        $("#device-group").empty();
        $.each(deviceType, function (index, d) {
            d.isCreated = false;
        });
        $('#country-code > li > a').removeClass('selected');
        $("#submit-notification, #cancel-notification").attr("disabled", true);
        $("#devicesTab").click();
        verifyImpersonationMode();
    };

    function saveNotificationProfileData() {
        if ($("#profileFirstName").val() == "" || $("#profileLastName").val() == "") {
            $("#notificationProfileinputError").css("display", "block");
        }
        else {
            $("#notificationProfileinputError").css("display", "none");
            var notificationObj = {
                "Person": {
                    "Id": notificationManager.person.id,
                    "TargetName": notificationManager.person.targetName,
                    "FirstName": $("#profileFirstName").val(),
                    "LastName": $("#profileLastName").val(),
                    "TimeZone": notificationManager.person.timeZone,
                    "Status": notificationManager.person.Status
                },
                "Devices": [],
                "NotificationGroups": []
            };

            $("#loading-panel").show();
            $.when(postData("/notificationmanager/savenotificationProfile", notificationObj)).then(function (data) {
                try {
                    switch (data) {
                        case "success":
                            clearForm();
                            $(".success-message").show();
                            deviceInit();
                            break;
                        default:
                            clearForm();
                            window.location.href = "/error";
                            break;
                    }
                }
                catch (err) {
                    window.location.href = "/error";
                }
            }, function () {
                window.location = "/error";
            }).fail(function () {
                $("#loading-panel").hide();
            });
        }
    }

    function deviceInit() {
        $("#manageDevicesForm").css("visibility", "hidden");
        $('#manageDevicesForm').data('validator', null);
        $.validator.unobtrusive.parse($('#manageDevicesForm'));
        $('#manageDevicesForm').validate({
            onfocusout: function (element) {
                this.element(element);
            }
        });
        bindTimeZone();
        loadDevices();
        loadCountryCodes();
    }

    function notificationInit() {
        var notificationTab = new NotificationTab();
        notificationTab.load();
        var timeFramePopup = new manageTimeFramePopup();
        timeFramePopup.bindEvents();
    }

    function devicesTabClick() {
        resetTimer();
        $("#check-all-text-notification").attr('style', 'display:none');
        $("#check-all-help-notification").attr('style', 'display:none');
        resizePageControl();
    }

    function checkScrollAvailable() {
        var elm = $('table.notification-group tbody');
        if (elm[0]) {
            var scrollHeight = elm[0].scrollHeight == null ? 0 : elm[0].scrollHeight;
            var height = elm.height() == null ? 0 : elm.height();
            return scrollHeight > height;
        }
    }

    function notificationTabClick() {
        resetTimer();
        if (validateDevices() & validateTimeZone() & $("#manageDevicesForm").valid()) {
            $("#check-all-text-notification").attr('style', 'display:inline-block');
            $("#check-all-help-notification").attr('style', 'display:inline-block');
            $("#notificationGroups").addClass("active in");
            $("#devices").removeClass("active in");
            var notificationTab = new NotificationTab();
            notificationTab.reBindGrid();
            var elm = $('table.notification-group tbody');
            var hasScroll = checkScrollAvailable();
            if (hasScroll) {
                $('#notificationGroupsTableHead').removeClass("on-head-if-body-no-scroll").addClass("on-head-if-body-scroll");
            }
            else {
                $('#notificationGroupsTableHead').removeClass("on-head-if-body-scroll").addClass("on-head-if-body-no-scroll");
            }
        }
        else
            return false;
    }

    function bindTimeZone(timeZone) {
        $.getJSON("/content/json/timezone.json", function (data) {
            $('#time-zone')
                .find('option')
                .remove()
                .end().append($('<option value="">-- Select --</option>'));
            $.each(data, function (index, item) {
                $('#time-zone').append($('<option value="' + item.key + '">' + item.value + '</option>'));
            });
            $('#time-zone').val(notificationManager.person.timeZone);
        });
    }

    function timeZoneSelected() {
        resetTimer();
        notificationManager.person.timeZone = $('#time-zone').val();
        manageTimeFrameButtonStateChange();
        validateTimeZone();
    }

    function loadCountryCodes() {
        countryCodes = [];
        $.getJSON("/content/json/countryCodes.json", function (data) {
            countryCodes = data;
            createCountryListDropdown();
        });
    }

    function createCountryListDropdown() {
        $('#country-code').empty().prepend($('<div class="divul"></div>'))
            .prepend($('<input type = "text" class="form-control search-country-code pb-focus-color" />'));
        $.each(countryCodes, function (index, item) {
            $('#country-code').append($('<li class="country-code-li"><a value="' + item.countryDialCode + '" data-country-code="' + item.countryCode + '" class="country-code-option"><i class="fa fa-check"></i >' + item.countryName + '</a></li>'));
        });
    }

    function loadDevices() {
        $("#loading-panel").show();
        $.when(getData('/notificationmanager/getpersondevices')).then(function (data) {
            $(".success-message").hide();
            try {
                if (data === "error") {
                    window.location = "/error/incompleteprofile";
                }
                else {
                    var items = [];
                    var person = new Person(data.Person);
                    if (person.firstName == "" || person.firstName == null) {
                        $("#manageNotificationProfileForm").css("display", "block");
                        $("#manageDevicesForm").css("visibility", "hidden");
                        $("#loading-panel").hide();
                        $("#profileLastName").val(person.lastName);
                    }
                    else {
                        $("#manageNotificationProfileForm").css("display", "none");
                        $("#manageDevicesForm").css("visibility", "visible");
                        if (checkEnableDisableProfile(data))
                            return;
                        notificationManager.person = person;
                        notificationManager.person.timeZone = data.Person.TimeZone;
                        if (data.Devices) {
                            $.each(data.Devices, function (index, item) {
                                var deviceTypeExist = $.grep(deviceType, function (d) { return d.xmValue === item.Name; });
                                if (deviceTypeExist.length) {
                                    var newDevice = addToModel(item);
                                    bindDevice(newDevice);
                                    setDeviceValidations(newDevice.uiDeviceId);
                                }
                            });
                        }
                        var notificationTab = new NotificationTab();
                        if (data.NotificationGroups) {
                            $.each(data.NotificationGroups, function (index, item) {
                                var notification = new Notification(item);
                                notification["uiNotificationId"] = "group-" + index;
                                notificationTab.addOrMergeGroup(notification);
                            });
                        }
                        $('#time-zone').val(notificationManager.person.timeZone);
                        $("#loading-panel").hide();
                        $("#manageDevicesForm").valid();
                        validateTimeZone();
                        validateDevices();
                        isAvailableDevice();

                        notificationTab.onNotificationGroupSort();
                        notificationTab.isValid();
                    }
                }
            }
            catch (err) {
                window.location.href = "/error";
            }
        }, function () {
            window.location.href = "/error";
        });
    }

    function checkEnableDisableProfile(data) {
        var rValue = true;
        $("#enable-disable-notification").unbind("click");
        $('#errorSummaryContainer').hide();
        var person = new Person(data.Person);
        notificationManager.person = person;
        $(".notification-status-div").show();
        if (notificationManager.person.id !== null && notificationManager.person.id !== "" && notificationManager.person.Status === "INACTIVE") {
            $("#enable-disable-notification").val("Enable");
            $("#enable-disable-notification").attr("title", "Click to enable notification profile.");
            $("#enable-disable-notification").bind("click", enableNotificationProfile);
            $("#notification-profile-status").addClass("notification-status-disabled");
            $("#notification-profile-status").removeClass("notification-status-enabled");
            $("#notification-profile-status").html("Disabled");
            $("#tab-area").hide();
            $("#action-ctrl-area").hide();
            $("#timeZoneDiv").hide();
        }
        else if (notificationManager.person.Status === "ACTIVE") {
            $("#notification-profile-status").addClass("notification-status-enabled");
            $("#enable-disable-notification").attr("title", "Click to disable notification profile.");
            $("#notification-profile-status").removeClass("notification-status-disabled");
            $("#enable-disable-notification").val("Disable");
            $("#enable-disable-notification").bind("click", showDisableProfilePopup);
            $("#notification-profile-status").html("Enabled");
            $("#tab-area").show();
            $("#action-ctrl-area").show();
            $("#timeZoneDiv").show();
            rValue = false;
        }
        else if (notificationManager.person.id === null || notificationManager.person.id === "") {
            $(".notification-status-div").hide();
            rValue = false;
        }
        $("#loading-panel").hide();
        return rValue;

    }

    function enableNotificationProfile() {
        notificationManager.person.Status = "ACTIVE";
        changeNotificationProfileStatus();
    }

    function disableNotificationProfile() {
        notificationManager.person.Status = "INACTIVE";
        closeDisableProfilePopup();
        changeNotificationProfileStatus();
    }

    function showDisableProfilePopup() {
        $("#dnf-button").unbind("click");
        $("#close-dfw-modal, #cancel-dfw-button").unbind("click");
        resetTimer();
        $("#close-dfw-modal, #cancel-dfw-button").bind("click", closeDisableProfilePopup);
        $("#dnf-button").bind("click", disableNotificationProfile);
        $(".body-container").addClass("when-model-popup");
        $("#disableProfileWarningModal").modal();
    }

    function closeDisableProfilePopup() {
        resetTimer();
        $(".body-container").removeClass("when-model-popup");
        $("#disableProfileWarningModal").modal("hide");
    }

    function changeNotificationProfileStatus() {

        var person = {
            "Id": notificationManager.person.id,
            "TargetName": notificationManager.person.targetName,
            "FirstName": notificationManager.person.firstName,
            "LastName": notificationManager.person.lastName,
            "TimeZone": notificationManager.person.timeZone,
            "Status": notificationManager.person.Status
        };
        $("#loading-panel").show();
        $.when(postData("/notificationmanager/changenotificationprofilestatus", person)).then(function (data) {
            $("#loading-panel").hide();
            try {
                switch (data) {
                    case "success":
                        clearForm();
                        $(".success-message").show();
                        deviceInit();
                        break;
                    default:
                        clearForm();
                        window.location.href = "/error";
                        break;
                }
            }
            catch (err) {
                window.location.href = "/error";
            }
        }).fail(function () {
            $("#loading-panel").hide();
        });
    }

    function addToModel(device) {
        var timeFrame = [];
        if (getDeviceTypeByxmValue(device.Name) === "smsPhone") {
            $.each(device.DeviceTimeFrame, function (i, devicetimeframe) {
                var existingTimeframe = new TimeFrame("", devicetimeframe.Name, devicetimeframe.StartTime, devicetimeframe.EndTime, devicetimeframe.Days);
                existingTimeframe.formatTime();
                timeFrame.push(existingTimeframe);
            });
        }
        var deviceId = addDeviceRow(true, device.Name);
        var phoneInfo = getPersonPhoneInfoFromNumber(device.PhoneNumber, device.CountryCode);
        var newDevice = new Device(deviceId, device.Id, getDeviceTypeByxmValue(device.Name), device.EmailAddress, phoneInfo.phoneNumber, timeFrame, device.CountryCode, phoneInfo.countryDialCode);
        notificationManager.devices.push(newDevice);
        return newDevice;
    }

    function getPersonPhoneInfoFromNumber(phoneNumber, countryCode) {
        if (!phoneNumber) {
            return "";
        }
        var countryItem = $.grep(countryCodes, function (e) { return e.countryCode === countryCode; });
        phoneNumber = phoneNumber.replace(countryItem[0].countryDialCode, "");
        var phoneInfo = { phoneNumber: phoneNumber, countryDialCode: countryItem[0].countryDialCode };
        return phoneInfo;
    }

    function bindDevice(device) {
        var selectedDeviceType = $("#" + device.uiDeviceId).find(".device-types").val();
        $.each(deviceType, function (i, item) {
            if (selectedDeviceType === item.value)
                item.isCreated = false;
            if (item.value === device.deviceType) {
                item.isCreated = true;
            }
        });
        $("#" + device.uiDeviceId).find(".device-description").val(device.deviceType === "primaryEmail" || device.deviceType === "secondaryEmail" ? device.emailAddress : device.phoneNumber);
        if (device.deviceType === "smsPhone") {
            $('#country-code > li > a').removeClass('selected');
            $('#country-codes button span.value').text(device.countryDialCode);
            $('#country-codes ul li a[data-country-code=' + device.countryCode + ']').addClass('selected');
        }
        $("#" + device.uiDeviceId).find(".device-types").val(device.deviceType);
        deviceTypeIconChange(device.uiDeviceId, device.deviceType);
        hideTimeFrame("#" + device.uiDeviceId, device.deviceType !== "smsPhone");
        manageTimeFrameButtonStateChange();
        rebindAllDeviceTypeDropdown();
    }

    function getDeviceTypeByxmValue(deviceType) {
        return deviceType === "Secondary Email" ? "secondaryEmail" : deviceType === "Work Email" ? "primaryEmail" : deviceType === "SMS Phone" ? "smsPhone" : null;
    }

    function addDeviceRow(isLoadingFromXM, device) {
        deviceIdCreater++;
        deviceId = 'device_' + deviceIdCreater;
        var deviceType = "";
        var availableDeviceTypes = availableDevice();
        var isHide = false;
        if (availableDeviceTypes) {
            $(".device-template").clone().appendTo("#device-group");
            $.each(availableDeviceTypes, function (i, item) {
                $("#device-group div.device-template select.device-types").append($('<option value="' + item.value + '">' + item.text + '</option>'));
                if (i === 0) {
                    deviceType = item.value;
                    item.isCreated = true;
                    isHide = !item.isTimeFrame;
                }
            });
            if ((getDeviceTypeByxmValue(device) === "smsPhone") || deviceType === "smsPhone") {
                $('#device-group div.device-template .device-type-smsphone ul input').attr('id', 'searchCountryCode');
                $('#device-group div.device-template .device-type-smsphone').attr('id', 'country-codes');
                $('#device-group div.device-template #country-codes').on('click', ManageNotification.events.dropdownClick);
                $('#device-group').on('scroll', ManageNotification.events.handleScroll);
                var defaultCountryCode = $('#defaultCountryCode').attr('value');
                $('#device-group div.device-template #country-codes ul li a[data-country-code=' + defaultCountryCode + ']').addClass('selected');
                $("#device-group div.device-template #country-codes button").attr("data-device-id", deviceId);
                $('#country-codes ul li a').on('click', ManageNotification.events.changeCountryCode);
                $('#searchCountryCode').on('input', ManageNotification.events.setIndex);
                $('#searchCountryCode').bind('keyup', ManageNotification.events.searchCountryCode);
            }
            $('#device-group div.device-template').attr('id', deviceId);
            $("#device-group div.device-template .device-description").bind("change", ManageNotification.events.decriptionChange);
            $("#device-group div.device-template .device-description").bind("keydown", ManageNotification.events.decriptionKeyDown);
            $("#device-group div.device-template select.device-types").bind("change", ManageNotification.events.selectDeviceType);
            $("#device-group div.device-template select.device-types").attr("data-device-id", deviceId);
            $("#device-group div.device-template select.device-types").bind("change", ManageNotification.events.selectDeviceType);
            if (isLoadingFromXM) {
                $("#device-group div.device-template select.device-types").addClass("select-aslabel");
                $("#device-group div.device-template select.device-types").attr("disabled", "disabled");
            }
            else {
                setTimeout(function () {
                    $("#device-group .device-description").focus();
                }, 50);
            }
            $("#device-group div.device-template .device-description").attr("data-device-id", deviceId);
            $("#device-group div.device-template .manage-time-frame").attr("data-device-id", deviceId);
            $("#device-group div.device-template .manage-time-frame").bind("click", ManageNotification.events.manageTimeFrame);

            $("#device-group div.device-template .remove-device").attr("data-device-id", deviceId);
            $("#device-group div.device-template .remove-device").bind("click", ManageNotification.events.deleteDevice);
            deviceTypeIconChange(deviceId, deviceType);
            hideTimeFrame('#' + deviceId, isHide);
            $("#device-group div").removeClass("device-template");
            rebindAllDeviceTypeDropdown();
        }

        return deviceId;
    }

    function changeCountryCode() {
        resetTimer();
        $('#country-codes button span.value').text($(this).attr('value'));
        $('#country-code > li > a').removeClass('selected');
        $(this).addClass('selected');
        $("#submit-notification, #cancel-notification").attr("disabled", false);
        isDirty = true;
        var device = $.grep(notificationManager.devices, function (e) { return e.deviceType === "smsPhone"; });
        device[0].countryDialCode = $(this).attr('value');//$('#defaultCountryCode').text();
        device[0].countryCode = $(this).attr("data-country-code");
        $('#searchCountryCode').val('');
        searchCountryCode();
        verifyImpersonationMode();
    }
    function setIndex() {
        liSelected = 0;
    }

    function searchCountryCode() {
        resetTimer();
        var searchbox = $.trim($('#searchCountryCode').val().toLowerCase());
        $(".dropdown-menu li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(searchbox) > -1);
        });
        var selectedLiItem = Array.from(document.querySelectorAll
            ('#country-codes >ul > li[style*="display: list-item"]'));
        $('#country-codes >ul > li').css('backgroundColor', 'white');
        //$('#country-code > li > a').removeClass('selected');
        selectedLiItem[liSelected].style.backgroundColor = "#3276b1";
        $("#country-codes ul").scrollTop((liSelected - 2) * 26);
    }

    function addDevice() {
        resetTimer();
        $(".dropdown li").css('display', 'list-item');
        var deviceId = addDeviceRow(false, null);
        var timeFrame = new TimeFrame();
        var device = new Device(deviceId, "", $("#" + deviceId).find(".device-types").val(), "", "", timeFrame, "US", "+1");
        notificationManager.devices.push(device);
        manageTimeFrameButtonStateChange();
        isAvailableDevice();
        setDeviceValidations(deviceId);
        validateDevices();
        $("#submit-notification, #cancel-notification").attr("disabled", false);
        verifyImpersonationMode();
    }

    function decriptionChange() {
        resetTimer();
        var deviceId = $(this).attr("data-device-id");
        var device = $.grep(notificationManager.devices, function (e) { return e.uiDeviceId === deviceId; });

        if (device && device.length === 1) {
            if (device[0].deviceType === "secondaryEmail" || device[0].deviceType === "primaryEmail") {
                device[0].emailAddress = $(this).val();
                device[0].phoneNumber = "";
            }
            else {
                device[0].phoneNumber = $(this).val();
                device[0].emailAddress = "";
            }
        }
        manageTimeFrameButtonStateChange();
    }

    function decriptionKeyDown() {
        resetTimer();
        var deviceId = $(this).attr("data-device-id");
        var device = $.grep(notificationManager.devices, function (e) { return e.uiDeviceId === deviceId; });
        if (device)
            $("." + device[0].deviceType).remove();
    }

    function isAvailableDevice() {
        var availableDeviceTypes = availableDevice();
        if (availableDeviceTypes.length === 0) {
            resizePageControl();
            $("#add-device").attr("disabled", true);
            $("#add-device").attr("title", "All device type already added");
        }
        else {
            $("#add-device").attr("disabled", false);
            $("#add-device").attr("text");
            $("#add-device").attr("title", "Click to Add device");
        }
    }

    function hideTimeFrame(device, isHide) {
        if (isHide) {
            $('#device-group').find(device).find(".device-type-phone").css('visibility', 'hidden');
            $('#device-group').find(device).find(".device-type-smsphone").css('display', 'none');
            $('#device-group').find(device).find(".device-description").removeClass("input-type-phone");
        }
        else {
            $('#device-group').find(device).find(".device-type-phone").css('visibility', 'visible');
            $('#device-group').find(device).find(".device-type-smsphone").css({ 'display': 'inline-block', 'bottom': '0' });
            $('#device-group').find(device).find(".device-description").addClass("input-type-phone");
        }
    }

    function availableDevice() {
        return $.grep(deviceType, function (d) {
            return d.isCreated === false;
        });
    }

    function deleteDevice() {
        resetTimer();
        var deviceDivId = "#" + $(this).attr("data-device-id");
        var deleteDeviceType = "";
        $.each(deviceType, function (i, item) {
            if ($(deviceDivId).find("select").val() === item.value) {
                deleteDeviceType = item.value;
                item.isCreated = false;
            }
        });
        var notificationTab = new NotificationTab();
        notificationTab.checkAllDeviceType(deleteDeviceType, false);
        $(deviceDivId).find('.device-description').rules("remove");
        $(deviceDivId).remove();
        dropdownClick();
        $('body').find("#" + deviceId + " ul input").removeAttr('id');
        var deviceId = $(this).attr("data-device-id");
        notificationManager.devices = $.grep(notificationManager.devices, function (e) { return e.uiDeviceId !== deviceId; });
        manageTimeFrameButtonStateChange();
        isAvailableDevice();
        rebindAllDeviceTypeDropdown();
        validateDevices();
        notificationTab.isValid();
    }

    function updateTimeFrame() {
        resetTimer();
        $("#manageTimeFrameForm").validate();
        if (!$("#manageTimeFrameForm").valid()) {
            return false;
        }
        else {
            var deviceId = $(this).attr("data-device-id");
            var timeframePopup = new manageTimeFramePopup();
            timeframePopup.update(deviceId);
        }
    }

    function cancelTimeFrame() {
        resetTimer();
        $(".body-container").removeClass("when-model-popup");
        $("#manageTimeFrameModal").modal("hide");
    }

    function selectDeviceType() {
        resetTimer();
        var that = this;
        var deviceDivId = "#" + $(this).attr("data-device-id");
        $.each(deviceType, function (i, item) {
            item.isEditing = false;
            if ($(that).find("option[value='" + item.value + "']").length > 0) {
                item.isEditing = true;
            }
        });

        var editingDevice = $.grep(deviceType, function (d) {
            return d.isEditing === true;
        });

        $.each(editingDevice, function (i, item) {
            if ($(that).val() === item.value) {
                item.isCreated = true;
                hideTimeFrame(deviceDivId, !item.isTimeFrame);
            }
            else {
                item.isCreated = false;
            }
        });
        var deviceId = $(this).attr("data-device-id");
        var device = $.grep(notificationManager.devices, function (e) { return e.uiDeviceId === deviceId; });
        if (device && device.length === 1) {
            device[0].deviceType = $(this).val();
            device[0].timeFrame = new TimeFrame();
            if (device[0].deviceType === "smsPhone") {
                $(deviceDivId).find(".device-description").val("");
                $(deviceDivId).find('.form-group-phone div').attr('id', 'country-codes');
                $(deviceDivId).find("ul input").attr('id', 'searchCountryCode');
                var defaultCountryCode = $('#defaultCountryCode').attr('value');
                $(deviceDivId).find('#country-codes ul li a[data-country-code=' + defaultCountryCode + ']').addClass('selected');
                $('#searchCountryCode').bind('keyup', ManageNotification.events.searchCountryCode);
                $('#searchCountryCode').bind('input', ManageNotification.events.setIndex);
                $('#country-codes ul li a').on('click', ManageNotification.events.changeCountryCode);
                dropdownClick();
            }
            else {
                $(deviceDivId).find(".device-description").val("");
                $(deviceDivId).find("ul input").removeAttr('id', 'searchCountryCode');
                $(deviceDivId).find('.form-group-phone div').removeAttr('id', 'country-codes');
                $(deviceDivId).find('#country-codes ul li a[data-country-code=' + defaultCountryCode + ']').removeClass('selected');
            }
            $(deviceDivId).find(".device-description").focus();
        }

        $(deviceDivId + " label.error").remove();
        rebindAllDeviceTypeDropdown();
        deviceTypeIconChange(deviceId, device[0].deviceType);
        setDeviceValidations(deviceId, true);
        manageTimeFrameButtonStateChange();
    }

    function rebindAllDeviceTypeDropdown() {
        $.each(deviceType, function (index, device) {
            if (!device.isCreated) {
                var notificationTab = new NotificationTab();
                notificationTab.checkAllDeviceType(device.value, false);
            }
        });

        $("#devices .device-row .device-types").each(function () {
            var selectedValue = $(this).val();
            var deviceId = $(this).attr("data-device-id");
            var availableDeviceType = $.grep(deviceType, function (d) {
                return d.isCreated === false || d.value === selectedValue;
            });
            $("#" + deviceId).find('.device-row .device-types option').remove().end();
            $.each(availableDeviceType, function (i, item) {
                $("#" + deviceId).find('.device-row .device-types').append($('<option value="' + item.value + '">' + item.text + '</option>'));
            });
            $("#" + deviceId).find('.device-row .device-types').val(selectedValue);
        });
    }

    function deviceTypeIconChange(deviceId, deviceType) {
        $("#" + deviceId).find(".device-type-icon").removeClass("glyphicon-envelope");
        $("#" + deviceId).find(".device-type-icon").removeClass("glyphicon-phone");
        if (deviceType === "smsPhone") {
            $("#" + deviceId).find(".device-type-icon").addClass("glyphicon-phone");
        }
        else {
            $("#" + deviceId).find(".device-type-icon").addClass("glyphicon-envelope");
        }
    }

    function manageTimeFrameButtonStateChange() {
        if (notificationManager.person.timeZone === "" || notificationManager.person.timeZone === null) {
            $(".manage-time-frame").attr("title", "Please select time zone first");
            $(".manage-time-frame").attr("disabled", "disabled");
        }
        else {
            $(".manage-time-frame").removeAttr("title");
            $(".manage-time-frame").removeAttr("disabled");
        }
    }

    function manageTimeFrame() {
        var timeframePopup = new manageTimeFramePopup();
        var deviceId = $(this).attr("data-device-id");
        timeframePopup.open(deviceId);
    }

    function bindTimeframeControlEvents() {
        $("#manageTimeFrameModal input").bind("input  propertychange paste", setTimeframeDataChanged);
        $("#timeFrameDays label").bind("click", setTimeframeDataChanged);
    }

    function setTimeframeDataChanged() {
        timeframeDataChanged = true;
    }

    function setDeviceValidations(deviceId, isSelectChange) {
        var deviceGroup = $("#" + deviceId);
        var input = $(deviceGroup).find("input.device-description");
        var deviceType = $(deviceGroup).find("select").val();
        // Assign unique name for validation purpose.
        input.attr('name', 'val_' + deviceId);

        if (isSelectChange) {
            input.rules('remove');
            $("#val_" + deviceId + "-error").remove();
        }

        input.rules('add', 'required');
        if (deviceType === "smsPhone") {
            input.rules('add', {
                phoneNumberValidate: true
            });
        } else {
            input.rules('add', {
                emailValidate: true
            });
        }
    }

    function updateDevices() {
        resetTimer();
        isValid = true;
        var nTab = new NotificationTab();
        $("#manageDevicesForm").validate();
        if (!$("#manageDevicesForm").valid()) {
            isValid = false;
        }
        if (!validateTimeZone()) {
            isValid = false;
        }
        //if (!validateDevices()) {
        //    isValid = false;
        //}
        //if (!nTab.isValid()) {
        //    isValid = false;
        //}
        if (isValid) {
            var saveNotificationManager = new SaveNotificationManager();
            saveNotificationManager.Save();
        }
    }

    function validateTimeZone() {
        isvalid = true;
        if ($('#time-zone').val() === '') {
            $("#errorSummaryTimeZone").show();
            isvalid = false;
        } else {
            $("#errorSummaryTimeZone").hide();
        }

        toggleErrorSummaryContainer();

        return isvalid;
    }

    function validateDevices() {
        isvalid = true;
        if ($("#device-group").find(".device-row").length === 0) {
            $("#errorSummaryDevice").show();
            isvalid = false;
        } else {
            $("#errorSummaryDevice").hide();
        }

        toggleErrorSummaryContainer();

        return isvalid;
    }

    function toggleErrorSummaryContainer() {
        if ($("#errorSummaryTimeZone").css("display") !== "none" || $("#errorSummaryDevice").css("display") !== "none" || $("#errorSummaryNotification").css("display") !== "none") {
            $('#errorSummaryContainer').show();
        } else {
            $('#errorSummaryContainer').hide();
        }
    }

    function cancelForm() {
        resetTimer();
        window.location.reload();
    }
    function handleScroll() {
        if ($("#country-codes")[0].classList.contains("open")) {
            $("#country-codes").removeClass("open");
        }
    }

    function dropdownClick() {
        var phoneTextElement = $(".input-type-phone");
        var height = $("#device-group").css("max-height");
        var scrollTop = $("#device-group").scrollTop();
        if (phoneTextElement.parent().offset())
            var offset = phoneTextElement.parent().offset().top + 35;
        if (scrollTop > 0)
            $("ul.dropdown-menu").css("top", offset + "px");
        else
            $("ul.dropdown-menu").css("top", "auto");
        $(".dropdown li").css('display', 'list-item');
        $(".dropdown li").css('backgroundColor', 'white');
        var selectedLiItem = Array.from(document.querySelectorAll
            ('#country-codes >ul > li[style*="display: list-item"]'));
        var selectedItem = $('#country-code > li > a[class*="country-code-option selected"] ')[0];
        for (var counter = 0; counter < selectedLiItem.length; counter++) {
            if (selectedLiItem[counter].innerText.trim() === selectedItem.innerText.trim()) {
                liSelected = counter;
                selectedLiItem[liSelected].style.backgroundColor = "#3276b1";
                break;
            }
        }
        $("#country-codes").on("show.bs.dropdown", function () {
            setTimeout(function () { $('#searchCountryCode').focus(); }, 10);
        });
        $("#country-codes").on("hide.bs.dropdown", function () {
            $('#searchCountryCode').val("");
            $(".dropdown li").css('display', 'list-item');
        });
        $("#country-codes").on("shown.bs.dropdown", function () {
            var totalHeight = 0;
            $("#country-codes ul li a").each(function () {
                if ($(this).hasClass('selected')) {
                    return false;
                }
                else {
                    totalHeight += $(this).outerHeight(true);
                }
            });
            $('.dropdown-menu').animate({
                scrollTop: totalHeight
            }, 1);
        });
    }

    ManageNotification.events = {
        addDevice: addDevice,
        deleteDevice: deleteDevice,
        decriptionChange: decriptionChange,
        decriptionKeyDown: decriptionKeyDown,
        selectDeviceType: selectDeviceType,
        manageTimeFrame: manageTimeFrame,
        updateTimeFrame: updateTimeFrame,
        cancelTimeFrame: cancelTimeFrame,
        timeZoneSelected: timeZoneSelected,
        notificationTabClick: notificationTabClick,
        devicesTabClick: devicesTabClick,
        updateDevices: updateDevices,
        saveNotificationProfileData: saveNotificationProfileData,
        cancelForm: cancelForm,
        changeCountryCode: changeCountryCode,
        searchCountryCode: searchCountryCode,
        dropdownClick: dropdownClick,
        handleScroll: handleScroll,
        setIndex: setIndex
    };

    ManageNotification.deviceInstance = {
        init: deviceInit
    };

    ManageNotification.notificationInstance = {
        init: notificationInit
    };
})();

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip();

    $("#add-device").bind("click", ManageNotification.events.addDevice);

    $("#delete-device").bind("click", ManageNotification.events.deleteDevice);

    $("#manage-time-frame").bind("click", ManageNotification.events.manageTimeFrame);

    $("#update-time-frame").bind("click", ManageNotification.events.updateTimeFrame);

    $("#cancel-time-frame, #close-time-frame").bind("click", ManageNotification.events.cancelTimeFrame);

    $("#submit-notification").bind("click", ManageNotification.events.updateDevices);
    //$("#submit-notification").bind("click", TestSave());

    $("#submitProfileData").bind("click", ManageNotification.events.saveNotificationProfileData);

    $("#cancel-notification").bind("click", ManageNotification.events.cancelForm);

    $("#time-zone").bind("change", ManageNotification.events.timeZoneSelected);

    $("#notificationTab").bind("click", ManageNotification.events.notificationTabClick);

    $("#devicesTab").bind("click", ManageNotification.events.devicesTabClick);



    ManageNotification.deviceInstance.init();
    ManageNotification.notificationInstance.init();

    $("#submit-notification, #cancel-notification").attr("disabled", true);

    calculatePageHeight();

    resetTimer();

});

var isDirty = false;

var isCancelClicked = false;

var idleTimer;
var liSelected;

checkScrollAvailable = function () {
    var elm = $('table.notification-group tbody');
    if (elm[0]) {
        var scrollHeight = elm[0].scrollHeight === null ? 0 : elm[0].scrollHeight;
        var height = elm.height() === null ? 0 : elm.height();
        return scrollHeight > height;
    }
};

calculatePageHeight = function () {
    var pageheight = window.innerHeight;
    var header = $('header').height();
    var footer = $('footer').height();
    $(".notification-container").css("height", pageheight - header - footer + "px");
};


resizePageControl = function () {
    var extraspace = 60;
    var equalSpacing = 15;
    var header = $('header').height();
    var footer = $('footer').height();
    var groupButtons = $('.device-button-group').height();
    var title = $('.page-header').height();
    var timeZone = $('#time-zone-div').height();
    var navTabs = $('.nav-tabs').height();
    var deviceRow = $('.add-device-row').height();
    var tabborder = 2;
    var pageheight = window.innerHeight;
    var totalDevices = $(".device-row-item").length - 2;
    var totalError = $("#device-group label.error").length;
    var totalValidationSummaryError = $("#errorSummaryContainer ul li").length;
    $('#device-group label.error').each(function () {
        if ($(this).css("display") === "none")
            totalError--;
    });
    $('#errorSummaryContainer ul li').each(function () {
        if ($(this).css("display") === "none")
            totalValidationSummaryError--;
    });
    $('#device-group').css("height", "100%");
    var totalEqSpacing = 6 + totalDevices * 2;
    var pageFixedControls = header + footer + groupButtons + extraspace;
    var contentFixedheightminWithoutDevices = title + timeZone + navTabs + tabborder * 2 + totalValidationSummaryError * 30;
    var contentFixedheightmin = contentFixedheightminWithoutDevices + deviceRow + totalDevices * (deviceRow + 1) + totalError * 30;
    var contentAreaSpaceAllowed = pageheight - pageFixedControls - contentFixedheightmin;
    if (totalEqSpacing * equalSpacing > contentAreaSpaceAllowed) {
        equalSpacing = contentAreaSpaceAllowed / totalEqSpacing;
        if (equalSpacing < 5) {
            equalSpacing = 5;
            var setDeviceGroupHeight = pageheight - totalEqSpacing * equalSpacing - pageFixedControls - contentFixedheightminWithoutDevices - deviceRow + 40;
            $('#device-group').css("height", setDeviceGroupHeight);
        }
        else {
            $('#device-group').css("height", "100%");
        }
    }

    $(".equal-spacing").css("margin-top", equalSpacing + "px");
    $(".equal-spacing").css("margin-bottom", equalSpacing + "px");
    var totalEqSpacingGroup = 4;
    var tRow = $('table tbody tr');
    var totalRow = $('table tbody tr').length;
    var thead = $("#notificationGroupsTableHead").height();
    var notificationGroupWidth = $("#notificationGroups").width();
    var minTableWidthRequired = $("#notificationGroups table thead td").length * 130;
    var selectBUWidth = $("#account-buselect").width();
    var selectClusterWidth = $("#product-clusterselect").width();
    $("#account-buselect option").css("width", selectBUWidth + "px");
    $("#product-clusterselect option").css("width", selectClusterWidth + "px");
    if (notificationGroupWidth > minTableWidthRequired) {
        $("#notificationGroups table").css("width", "100%");
        $("#notificationGroups table thead td").css("width", "auto");
    }
    else {
        $("#notificationGroups table").css("width", minTableWidthRequired + 20 + "px");
        $("#notificationGroups table thead td").css("width", "130px");
    }
    var gridRequiredHeight = tRow.height() * totalRow + totalRow;
    var grouptabgridAreaAvailable = pageheight - thead - pageFixedControls - contentFixedheightminWithoutDevices - totalEqSpacingGroup * equalSpacing + 20 - totalValidationSummaryError * 15;
    var gridHeight = 0;
    if (gridRequiredHeight > grouptabgridAreaAvailable) {
        gridHeight = grouptabgridAreaAvailable;
    }
    else {
        gridHeight = gridRequiredHeight;
    }
    var tbody = $('table tbody');
    tbody.css("max-height", gridHeight);
    var hasScroll = checkScrollAvailable();
    if (hasScroll) {
        $('#notificationGroupsTableHead').removeClass("on-head-if-body-no-scroll").addClass("on-head-if-body-scroll");
    }
    else {
        $('#notificationGroupsTableHead').removeClass("on-head-if-body-scroll").addClass("on-head-if-body-no-scroll");
    }
    reSizeOptionText();
};

resetTimer = function () {
    var openerWindow = window.opener;
    if (openerWindow) {
        openerWindow.postMessage('resetTimer', '*');
    }
    var now = new Date();
    clearTimeout(idleTimer == undefined ? 0 : idleTimer);
    idleTimer = setTimeout(expireSession, 900000);
};

expireSession = function () {
    var timeout = true;
    window.location.href = "/Notification/logout?Timeout=" + timeout;
};

optionTextWidth = function (text) {
    var spanText = '<span>' + text + '</span>';
    $("#optionTest").html(spanText);
    var width = $("#optionTest").find('span:first').width();
    if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
        width = width / 1.2;
    }
    return width;
};

reBindOptions = function (selector, selectorWidth) {
    var optionCount = $(selector).length;
    var fontSize = 8;
    $(selector).each(function () {
        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
            fontSize = 7;
        }
        var charAllowed = parseInt((selectorWidth - 24) / fontSize);
        var scrollWidth = 20;
        if (optionCount > 20) {
            charAllowed -= 2;
            scrollWidth += 15;
        }

        var itemText = $(this).text();
        if (itemText.length > charAllowed) {
            var increaseWidth = 0;
            do {
                optionWidth = optionTextWidth(itemText.substring(0, charAllowed + increaseWidth++));
            } while (optionWidth < selectorWidth - scrollWidth && increaseWidth < 10);
            if (itemText.substring(0, charAllowed + increaseWidth) !== itemText)
                $(this).text(itemText.substring(0, charAllowed + increaseWidth - 1) + "...");
            else
                $(this).text(itemText);
        }
    });
};

reSizeOptionText = function () {
    $("#account-buselect").css("width", "100%");
    $("#product-clusterselect").css("width", "100%");
    var buWidth = $("#account-buselect").width();
    var clusterWidth = $("#product-clusterselect").width();
    reBindOptions("#account-buselect option", buWidth);
    reBindOptions("#product-clusterselect option", clusterWidth);
};

$(document).on('change', '#time-zone', function () {
    $("#submit-notification, #cancel-notification").attr("disabled", false);
    resizePageControl();
    isDirty = true;
    resetTimer();
    verifyImpersonationMode();
});

$(document).on('input', '#devices input', function () {
    $("#submit-notification, #cancel-notification").attr("disabled", false);
    resizePageControl();
    isDirty = true;
    verifyImpersonationMode();
});

$(document).on('click', '#add-device, .remove-device, #update-time-frame, table.notification-group tbody tr td input', function () {
    $("#submit-notification, #cancel-notification").attr("disabled", false);
    resizePageControl();
    isDirty = true;
    verifyImpersonationMode();
});



$(document).on('blur', 'input', function () {
    resizePageControl();
});

$(window).on('resize', function () {
    resizePageControl();
    calculatePageHeight();
});
$(window).keydown(function (e) {
    var selectedLiItem = Array.from(document.querySelectorAll
        ('#country-codes >ul > li[style*="display: list-item"]'));
    if (e.which === 40) {

        if (selectedLiItem.length > 0) {
            if (liSelected === (selectedLiItem.length - 1)) {
                selectedLiItem[0].style.backgroundColor = "white";
                selectedLiItem[liSelected].style.backgroundColor = "white";
                liSelected = 0;
                selectedLiItem[liSelected].style.backgroundColor = "#3276b1";
                $("#country-codes ul").scrollTop((liSelected - 2) * 26);
            } else {
                selectedLiItem[liSelected].style.backgroundColor = "white";
                liSelected = liSelected + 1;
                selectedLiItem[liSelected].style.backgroundColor = "#3276b1";
                $("#country-codes ul").scrollTop((liSelected - 2) * 26);
            }
        }
        //window.alert("key down");
    }
    if (e.which === 38) {
        if (liSelected > 0) {
            if (liSelected === selectedLiItem.length) {
                selectedLiItem[liSelected - 1].style.backgroundColor = "white";
                liSelected = 0;
            }
            selectedLiItem[liSelected].style.backgroundColor = "white";
            $("#country-codes ul").scrollTop((liSelected - 3) * 26);
            if (selectedLiItem.length > 0) {
                selectedLiItem[liSelected - 1].style.backgroundColor = "#3276b1";
                liSelected = liSelected - 1;
            }
        } else {
            selectedLiItem[0].style.backgroundColor = "white";
            liSelected = selectedLiItem.length - 1;
            selectedLiItem[liSelected].style.backgroundColor = "#3276b1";
            $("#country-codes ul").scrollTop((liSelected - 2) * 26);
        }
    }
    if (e.which === 13) {
        var value = ($('a', selectedLiItem[liSelected]).attr('value'));
        $('#country-codes button span.value').text(value);
        $('#country-code > li > a').removeClass('selected');
        ($('a', selectedLiItem[liSelected])).addClass('selected');
        $("#submit-notification, #cancel-notification").attr("disabled", false);
        isDirty = true;
        $("#country-codes").removeClass("open");
        $('#searchCountryCode').val("");
        verifyImpersonationMode();
    }
});

window.onload = function () {
    window.addEventListener("beforeunload", function (e) {
        if (!isDirty) {
            return undefined;
        }

        var confirmationMessage = 'It looks like you have been editing something. '
            + 'If you leave before saving, your changes will be lost.';

        (e || window.event).returnValue = confirmationMessage; //Gecko + IE
        return confirmationMessage; //Gecko + Webkit, Safari, Chrome etc.
    });
};

verifyImpersonationMode = function () {
    if (($("#impersonationStatus") != "undefined" || $("#impersonationStatus") != null) && $("#impersonationStatus").val()) {
        $("#submit-notification").attr("disabled", true);
    }
}