$(document).ready(function () {
    var idleTimer;
    resetTimer = function () {
        var now = new Date();
        clearTimeout(idleTimer == undefined ? 0 : idleTimer);
        idleTimer = setTimeout(expireSession, 900000);
    };

    expireSession = function () {
        var timeout = true;
        window.location.href = "/Notification/logout?Timeout=" + timeout;
    };
    resetTimer();
});