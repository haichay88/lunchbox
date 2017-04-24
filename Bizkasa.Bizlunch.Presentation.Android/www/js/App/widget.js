app.service("Widgetservice", function ($http) {

    this.GetGroupWidgets = function () {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Widget/GetGroupWidget",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };


    this.AddGroupWidget = function (data) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Widget/AddGroupWidget",
            data: data,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };


    this.AddWidget = function (data) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Widget/AddWidget",
            data: data,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };



    this.DeleteGroupWidget = function (val) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Widget/DeleteGroupWidget",
            dataType: 'json',
            data: JSON.stringify({Id:val}),
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };

    this.GetWidgets = function () {
        var request = $http({
            method: "get",
            url: "/CPanelAdmin/Widget/GetWidget",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };

    this.DeleteWidget = function (val) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Widget/DeleteWidget",
            dataType: 'json',
            data: JSON.stringify({ Ids: val }),
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };

    this.InsertOrUpdateInvoice = function (model) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Invoice/InsertOrUpdateInvoice",
            dataType: 'json',
            data: model,
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    }

    this.GetWidgetById = function (val) {
        var request = $http({
            method: "post",
            url: "/CPanelAdmin/Widget/GetWidgetById",
            dataType: 'json',
            data: JSON.stringify({ Id: val }),
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };


    this.GetWidgetForRecept = function () {
        var request = $http({
            method: "get",
            url: "/CPanelAdmin/Widget/GetWidgetForRecept",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        });
        return request;
    };
});


app.controller("WidgetController", function ($scope, Widgetservice) {

  

    var InitGroupWidget = function () {
        var promiseGet = Widgetservice.GetGroupWidgets();
        promiseGet.then(function (pl) {
            $scope.GroupWidgets = pl.data.Data;
        });
    };

  

    $scope.InitWidget = function () {
        CommonUtils.showWait(true);
        var promiseGet = Widgetservice.GetWidgets();
        promiseGet.then(function (pl) {
            if (!pl.data.IsError)
                $scope.Widgets = pl.data.Data;
            else {
                toastr.error(pl.data.Message);
            }
        },
        function (errorPl) {
        });
        CommonUtils.showWait(false);
    };
    var InitWidgetInvoice = function () {
        CommonUtils.showWait(true);
        var promiseGet = Widgetservice.GetWidgetForRecept();
        promiseGet.then(function (pl) {
            if (!pl.data.HasError) {
                $scope.WidgetsInvoice = $.map(pl.data.Data, function (n, i) {
                    return $.map(n.Widgets, function (item, index) {
                        return {
                            GroupName: n.GroupName,
                            Name: item.Name,
                            Price: item.Price,
                            Id: item.Id

                        };
                    });
                });
                CommonUtils.showWait(false);
            }
            else {
                toastr.error(pl.data.Error);
            }
        },
        function (errorPl) {
        });
    };

    $scope.GetHotelBy = function () {
        var promiseGet = Hotelservice.getById();
        promiseGet.then(function (pl) {
            $scope.Hotel = pl.data.Data;
        },
        function (errorPl) {
        });
    };

    $scope.pop = function () {
        InitGroupWidget();
        $("#Addwidget").modal('show');
    
        
    }
    $scope.popGroupWidget = function () {
        InitGroupWidget();
        $("#AddGroupwidget").modal('show');
    };
    $scope.AddGroupWidget = function () {
        var promisePost = Widgetservice.AddGroupWidget($scope.GroupWidget);
        promisePost.then(function (pl) {
            $scope.GroupWidgets = pl.data.Data;
            //toastr.success("Thêm mới thành công !");
            $("#AddGroupwidget").modal('hide');
        }, function (err) {
        });
    }


    $scope.AddWidget = function () {

        if ($scope.Widget.GroupId == undefined) {
            toastr.error("Bạn chưa chọn nhóm dịch vụ !");
            return;
        }
        CommonUtils.showWait(true);
        var promisePost = Widgetservice.AddWidget($scope.Widget);
        promisePost.then(function (pl) {
            if (!pl.data.IsError) {
                $scope.Widgets = pl.data.Data;
                toastr.success("Thêm mới thành công !");
                $("#Addwidget").modal('hide');
            }
            else {
                toastr.error(pl.data.Message);
            }
            CommonUtils.showWait(false);
            
        }, function (err) {
        });
    }

    $scope.DeleteGroupWidget = function (val) {
        CommonUtils.showWait(true);
        var promiseGet = Widgetservice.DeleteGroupWidget(val);
        promiseGet.then(function (pl) {
            if(!pl.data.IsError)
            {
                toastr.success('Xóa thành công !');
                InitGroupWidget();
            }
            else {
                toastr.error(pl.data.Message);
            }
            CommonUtils.showWait(false);
        });
    };
    var WidgetSelecteds = [];
    $scope.WidgetSelected = function (val) {
        if (val.IsSelected) {
            WidgetSelecteds.push(val.Id);
        }
        else {
            WidgetSelecteds = $.grep(WidgetSelecteds, function (item,i) {
                return item != val.Id;
            });
        }
    };
    $scope.reallyDelete = function (item) {
        if (item)
        {
            WidgetSelecteds.push(item.Id);
        }
        if (WidgetSelecteds.length <= 0) {
            toastr.error("Chưa chọn dịch vụ cần xóa !");
            return;
        }
        
        DeleteWidget(WidgetSelecteds);
    };
    var DeleteWidget = function (WidgetSelecteds) {
        var promiseGet = Widgetservice.DeleteWidget(WidgetSelecteds);
        promiseGet.then(function (pl) {
            if (!pl.data.IsError) {
                toastr.success('Xóa thành công !');
                WidgetSelecteds = [];
                $scope.InitWidget();
            } else {
                toastr.error(pl.data.Message);
               
            }

        },
        function (errorPl) {
        });
    };



    $scope.ShowPopup = function (val) {
        $scope.Invoice = {
            InvoiceDetails: [],
            TotalAmount: 0,
            InvoiceType: val
        };
        $scope.InvoiceDetail = {
            Descriptions: undefined,
            ServiceId:0,
            Quantity: undefined,
            Price: undefined,
            Notes: undefined,
            SubAmount: undefined,
            CategoryInvoice:undefined
        }
        InitWidgetInvoice();
        $('#addInvoice').modal('show');
    };
    $scope.AddInvoice = function () {
        if ($.trim($scope.Invoice.CompanyName) == "" && $.trim($scope.Invoice.CustomerName) == "") {
            toastr.error("Cần nhập tên công ty hoặc khách hàng !");
            return;
        }
        CommonUtils.showWait(true);
        var promiseGet = Widgetservice.InsertOrUpdateInvoice($scope.Invoice);
        promiseGet.then(function (pl) {
            if (!pl.data.IsError) {
                toastr.success("Nhập kho thành công !");
                $scope.InitWidget();
                $('#addInvoice').modal('hide');

            } else {
                toastr.error(pl.data.Message);
            }
        },
        function (errorPl) {
        });

        CommonUtils.showWait(false);
    };


    $scope.AddInvoiceDetail = function (val) {
        if ($scope.Service == undefined || $scope.Service == "") {
            toastr.error("Bạn chưa chọn dịch vụ !");
            return;
        }
        if ($scope.InvoiceDetail.Quantity == undefined || $scope.InvoiceDetail.Quantity == "") {
            toastr.error("Bạn chưa nhập số lượng !");
            return;
        }
        if ($scope.InvoiceDetail.Price == undefined || $scope.InvoiceDetail.Price == "")
        {
            toastr.error("Bạn chưa nhập giá nhập !");
            return;
        }
       
        $scope.InvoiceDetail.ServiceId = $scope.Service.Id;
        $scope.InvoiceDetail.CategoryInvoice = 8;
        $scope.InvoiceDetail.Descriptions = $scope.Service.Name;
        $scope.Invoice.InvoiceDetails.push($scope.InvoiceDetail);
        $scope.InvoiceDetail.SubAmount = $scope.InvoiceDetail.Price * $scope.InvoiceDetail.Quantity;
        $scope.Invoice.TotalAmount += $scope.InvoiceDetail.SubAmount;
        $scope.InvoiceDetail = undefined;
        if (val)
            $('#addInvoiceDetail').modal('hide');
    };



    $scope.GetWidgetById = function (val) {
        var promiseGet = Widgetservice.GetWidgetById(val);
        promiseGet.then(function (pl) {
            if (!pl.data.IsError) {
                InitGroupWidget();
                $scope.Widget = pl.data.Data;
                $('#Addwidget').modal('show');
            }
            else
                toastr.error(pl.data.Messeage);
        },
        function (errorPl) {
        });
    };

    function getUrlParameter(sParam) {
        var sPageURL = window.location.search.substring(1);
        var sURLVariables = sPageURL.split('&');
        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] == sParam) {
                return sParameterName[1];
            }
        }
    }
});