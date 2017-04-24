app.service("RestaurantService", function ($http) {
    this.GetRestaurants = function () {
        var request = $http({
            method: "get",
            url: "/Restaurant/GetRestaurants",
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
    this.GetGroups = function () {
        var request = $http({
            method: "get",
            url: "/User/GetGroups",
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
    this.AddOrUpdateRestaurant = function (model) {
        var request = $http({
            method: "post",
            url: "/Restaurant/AddOrUpdateRestaurant",
            dataType: 'json',
            data: model,
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
    this.GetListRoomClass = function () {
        var request = $http({
            method: "get",
            url: "/CPanelAdmin/Room/GetListRoomClass",
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    }

    this.InsertOrUpdateFloor = function (model) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Floor/InsertOrUpdateFloor",
            dataType: 'json',
            data: model,
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    }
    this.GetFloorBy = function (val) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Floor/GetFloorBy",
            dataType: 'json',
            data: JSON.stringify({ floorId: val }),
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    }
    


    this.DeleteFloor = function (val) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Floor/DeleteFloor",
            dataType: 'json',
            data: JSON.stringify({ Ids: val }),
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
});


app.controller("RestaurantController", function ($scope, RestaurantService) {
    
    $scope.showAddRestaurant = function () {
        $("#AddOrUpdateRestaurant").modal("show");
    };

    $scope.AddOrUpdateRestaurant = function () {
        
        var promisePost = RestaurantService.AddOrUpdateRestaurant($scope.Restaurant);
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                $scope.GetRestaurants();
                toastr.success("Update restaurant Successful !");
                $("#AddOrUpdateRestaurant").modal("hide");
            }
            else
                toastr.error(pl.data.Message);
        }, function (err) {
            toastr.error(err.statusText);
        });
    };
    $scope.GetRestaurants = function () {
        var promisePost = RestaurantService.GetRestaurants();
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                $scope.Restaurants = pl.data.Data;
            } else {
                toastr.error(pl.data.Message);
            }
        }, function (err) {
            toastr.error(err.statusText);
        });
    };

    $scope.GetGroups = function () {
        var promisePost = AccountService.GetGroups();
        promisePost.then(function (pl) {
            $scope.Groups = pl.data.Data;
        }, function (err) {
            toastr.error(err.statusText);
        });
    };

    $scope.ShowPopupAddUser = function () {
        $scope.User = {};
        $("#addOrUpdateUser").modal("show");
    };

  
});