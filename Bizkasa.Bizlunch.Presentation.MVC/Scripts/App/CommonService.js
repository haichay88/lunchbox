app.service("CommonService", function ($http) {
    
    this.AjaxGet = function (Url, callback) {
        $http({
            method: "get",
            url: Url,
            dataType: 'json',
        }).then(callback);
    };

    this.AjaxPost = function (Url, model, callback) {
        $http({
            method: "POST",
            url: Url,
            data: model,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
        }).then(callback);
    };
});
