$(document).ready(function () {

    /*
    / Display unread messages
    */
    $("body").on("click", "span.msg-notify.red", function () {

        $("ul#message-friend-ul").empty();

        var url = "profile/DisplayUnreadMessages";

        $.post(url, {}, function (data) {

            if (data.length > 0) {
                $("ul#message-friend-ul").append("<li class=close>x</li>");
            }

            for (var i = 0; i < data.length; i++) {
                var obj = data[i];

                $("ul#message-friend-ul").append('<li class=msg-notify-li><a href="/' + obj.FromUsername
                    + '"><img src="Content/Images/' + obj.FromUsername + '.jpg" /></a>' + ' ' + obj.Message + '</li>');
            }

        }).done(function () {
            hub.server.notifyOfMessageOwner();
        });

    });

    $("body").on("click", "ul#message-friend-ul li.close", function () {
        $("ul#message-friend-ul").empty();
    });
});





