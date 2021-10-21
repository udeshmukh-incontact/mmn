function getData(url) {
    var deferred = $.Deferred();

    $.ajax({
        type: "GET",
        contentType: "application/json",
        url: url,
        success: function (result) {
            deferred.resolve(result);
        },
        error: function (xhr, textStatus, errorThrown) {
            window.location.href = "/Error/Index";
            deferred.reject(textStatus);
            
        }
    });
    return deferred.promise();
};


function postData(url, data) {
    var deferred = $.Deferred();

    $.ajax({
        type: "POST",
        url: url,
        data: data,
        success: function (result) {
            deferred.resolve(result);
        },
        error: function (xhr, textStatus, errorThrown) {
            window.location.href = "/Error/Index";
            deferred.reject(textStatus);
        }
    });

    //$.post(url, data, function (result) {
    //    deferred.resolve(result);
    //}).fail(function (err) {
    //    window.location.href = "/Error/Index";
    //    deferred.reject(err);
    //});
    return deferred.promise();
}