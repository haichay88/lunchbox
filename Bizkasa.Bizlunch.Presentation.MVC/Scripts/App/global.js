var root = "";
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
            return  "https://"+root + url;
        },
        GetToken: function () {
            var name = "FGO" + "=";
            var decodedCookie = decodeURIComponent(document.cookie);
            var ca = decodedCookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') {
                    c = c.substring(1);
                }
                if (c.indexOf(name) == 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return "";
        }
    };
