/// <reference path="jquery-1.10.2.js" />
$(function ()
{
    $('.phone').text(function (i, text) {
    var n = (text.length) - 5;
    if (n == 4) { var p = n; } else { var p = 3; }
    var regex = new RegExp('(\\d{3})(\\d{' + p + '})(\\d{4})');
    var text = text.replace(regex, "($1) $2-$3");
    return text;
    });

    var handleError = function (msg) {
        if (jq("#errorMsg").html().trim().length == 0) {
            var errorText = "<div class=\"col-md-8 col-sm-6 alert alert-error\">" +
     "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" style=\"font-size:14px\">close X</button>" +
     "<div class=\"validation-summary-errors alert alert-danger\">" +
       "<ul><li>" + msg + " </li></ul></div></div>"
            jq("#errorMsg").html(errorText);
            jq("#errorMsg").show();
        }
        else {
            jq("#errorMsg").show();
            jq("#errorMsg li").remove();
            jq("#errorMsg ul").append("<li>" + msg + "</li>");
        }
    }

    var baseUrl = function () {
        var href = window.location.href.split('/');
        return href[0] + '//' + href[2] + '/' + href[3] + '/';
    }
})
