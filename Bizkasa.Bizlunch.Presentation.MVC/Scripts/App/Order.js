app.service("OrderService", function ($http) {
    this.GetUsers = function () {
        var request = $http({
            method: "get",
            url: "/User/GetUsers",
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
    this.GetRestaurants = function () {
        var request = $http({
            method: "get",
            url: "/Restaurant/GetRestaurants",
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
    this.AddOrUpdateOrder = function (model) {
        var request = $http({
            method: "post",
            url: "/Order/AddOrUpdateOrder",
            dataType: 'json',
            data: model,
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };

    this.AddOrderDetail = function (model) {
        var request = $http({
            method: "post",
            url: "/Order/AddOrderDetail",
            dataType: 'json',
            data: model,
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
    this.GetOrders = function () {
        var request = $http({
            method: "get",
            url: "/Order/GetOrders",
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
    this.GetOrder = function (val) {
        var request = $http({
            method: "POST",
            url: "/Order/GetOrder",
            dataType: 'json',
            data: JSON.stringify({orderId: val }),
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


app.controller("OrderController", function ($scope, OrderService,CommonService) {
    
    $scope.showAddOrder = function () {
        $("#addOrder").modal("show");
        $scope.GetRestaurants();
    };
   
  
    $scope.InitDate = function () {
        $('#id-date').daterangepicker({
            'applyClass': 'btn-sm btn-success',
            'cancelClass': 'btn-sm btn-default',
            format: 'DD/MM/YYYY h:mm A',
            startDate: moment(),
            singleDatePicker: true,
            showDropdowns: true,
            timePicker: true,
            timePickerIncrement: 5,
        },

     function (start, end, label) {
         $scope.LunchDate = start;
     });
    };
    $scope.AddOrUpdateOrder = function () {
        $scope.Order.LunchDate = $scope.LunchDate;
        var orderDetail = [];
        $.each($scope.Users, function (i,n) {
            if (n.IsSelected) {
                orderDetail.push({ AccountId:n.Id})
            }
        });
        $scope.Order.OrderDetails = orderDetail;
       
        var promisePost = OrderService.AddOrUpdateOrder($scope.Order);
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                toastr.success("Update user Successful !");
                window.location.href = '/order';
            }
            else
                toastr.error(pl.data.Message);
        }, function (err) {
            toastr.error(err.statusText);
        });
    };
    $scope.EditMode = false;
    $scope.AddOrderDetail = function () {
        var orderDetail = [];
        orderDetail.push($scope.OrderDetail);
        if ($scope.OrderDetail) {
            $scope.Order.OrderDetails = orderDetail;
        }
        $scope.Order.OrderDetails = $scope.Order.OrderDetailsCanEdit;
        debugger
        CommonUtils.showWait(true);
        var promisePost = OrderService.AddOrderDetail($scope.Order);
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                toastr.success("Update user Successful !");
                $scope.Order = pl.data.Data;
                $scope.EditMode = false;
            }
            else
                toastr.error(pl.data.Message);
            CommonUtils.showWait(false);
        }, function (err) {
            toastr.error(err.statusText);
        });
    };
    $scope.GetUsers = function () {
        var promisePost = OrderService.GetUsers();
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                $scope.Users = pl.data.Data;
            }
            else
                toastr.error(pl.data.Message);
        }, function (err) {
            toastr.error(err.statusText);
        });
    };

    $scope.GetOrders = function () {

        var token = CommonUtils.GetToken();
        if (!token)
        { return; }
        var request = { Token: token };
        var urlPost = CommonUtils.RootUrl("~/Order/GetOrders");
        CommonService.AjaxPost("/Order/GetOrders", request, function (reponse) {
            var result = reponse.data;
            if (!result.IsError) {
                $scope.Orders = result.Data
            } else {
                toastr.error(result.Message);
            }

        });
        
    };
    var OrderDetailsCanEdit = [];
    $scope.GetOrder = function () {
      
        var orderId = $("#orderId").val();

        var token = CommonUtils.GetToken();
        if (!token)
        { return; }
        var request = { Token: token, Id: orderId };
        var urlPost = "/Order/GetOrder";
        CommonService.AjaxPost(urlPost, request, function (reponse) {
            var result = reponse.data;
            debugger
            if (!result.IsError) {
                $scope.Order = result.Data
            } else {
                toastr.error(result.Message);
            }

        });
       
    };
    $scope.AddItemDetail = function () {
        $scope.Order.OrderDetailsCanEdit.push({ MenuItem: undefined, MenuCost: undefined });
    };
    $scope.RemoveItem = function (val) {
        $scope.Order.OrderDetailsCanEdit = $.grep($scope.Order.OrderDetailsCanEdit, function (i, n) {
            return n != val;
        });
       
    };
    $scope.GetRestaurants = function () {
        CommonUtils.showWait(true);
        var promisePost = OrderService.GetRestaurants();
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                $scope.Restaurants = pl.data.Data;
            } else {
                toastr.error(pl.data.Message);
            }
            CommonUtils.showWait(false);
        }, function (err) {
            toastr.error(err.statusText);
        });
    };

    $scope.ChangeModel = function () {
        $scope.EditMode = !$scope.EditMode;
    };

    $scope.Cancel = function () {
        debugger
        $scope.Order.OrderDetailsCanEdit = OrderDetailsCanEdit;
        $scope.ChangeModel();
    };
  
});