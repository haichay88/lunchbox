var root = "http://localhost:8111/";
    var CommonUtils = {
        showWait: function (val) {
            if (val)
                $('#loading').show();
            else {
                setTimeout(function () {
                    $('#loading').hide();
                }, 1000);
            }
               
        },
        RootUrl: function (url) {
            return root + url;
        },
        token: function () {
            var context = localStorage.getItem('context');
            return context;
        }
    };
