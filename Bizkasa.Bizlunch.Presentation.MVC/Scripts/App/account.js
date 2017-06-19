app.service("AccountService", function ($http) {
    this.GetUsers = function () {
        var request = $http({
            method: "get",
            url: "/User/GetUsers",
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
    this.AddOrUpdateUser = function (model) {
        var request = $http({
            method: "post",
            url: "/User/AddOrUpdateAccount",
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


app.controller("AccountController", function ($scope, AccountService) {
    
    // This function is called when someone finishes with the Login
    // Button.  See the onlogin handler attached to it in the sample
    // code below.
    $scope.checkLoginState = function () {
        FB.getLoginStatus(function (response) {
            statusChangeCallback(response);
        });
    };
    $scope.Singin = function () {
        debugger
        if (!$scope.User.Email) {
            toastr.error("Username invalid!");
            return;
        }
        if (!$scope.User.Password) {
            toastr.error("Password invalid !");
            return;
        }
       
        var urlPost = CommonUtils.RootUrl("/Home/Login");
        AccountService.AjaxPost(urlPost, $scope.User, function (reponse) {
            var result = reponse.data.Data;
            

        });
    };
    $scope.LoginFB = function () {
        debugger
        FB.login(function (response) {
            if (response.authResponse) {
                FB.api('/me?fields=link,id,name,taggable_friends,friends', function (response) {
                    console.log('Good to see you, ' + response.name + '.');
                });
            } else {
                console.log('User cancelled login or did not fully authorize.');
            }
        }, { scope:'email,user_friends', return_scopes: true });
    };
    $scope.Getfriends = function () {
        
        FB.api('/me/friends?fields=email,id,name,picture', 'GET', {}, function (response) {
          if (response.data) {
              console.log(response.data);
              $.each(response.data, function (index, friend) {
              });
          } else {
              debugger
              alert("Error!");
          }
      });
    };
    $scope.SendMessage = function () {

        FB.api('/me/friends?fields=email,id,name,picture', 'GET', {}, function (response) {
            if (response.data) {
                console.log(response.data);
                $.each(response.data, function (index, friend) {
                });
            } else {
                debugger
                alert("Error!");
            }
        });
    };

    $scope.AddOrUpdateUser = function () {
        
        var promisePost = AccountService.AddOrUpdateUser($scope.User);
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                $scope.GetUsers();
                toastr.success("Update user Successful !");
                $("#addOrUpdateUser").modal("hide");
            }
            else
                toastr.error(pl.data.Message);
        }, function (err) {
            toastr.error(err.statusText);
        });
    };
    $scope.GetUsers = function () {
        var promisePost = AccountService.GetUsers();
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                $scope.Users = pl.data.Data;
            }else
                toastr.error(pl.data.Message);
        }, function (err) {debugger
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