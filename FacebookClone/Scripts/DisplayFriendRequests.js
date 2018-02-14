$(document).ready(function () {

    /*
    / Display friend requests
    */
    $("body").on("click", "span.notify-friend.red", function() {

        $("ul#notify-friend-ul").empty();

        var url = "profile/DisplayFriendRequests";

        $.post(url, { }, function (data) {

            if ($("ul#live-search-ul li.close").length) return;

            if (data.length > 0) {
                $("ul#notify-friend-ul").append("<li class=close>x</li>");
            }

            for (var i = 0; i < data.length; i++) {
                var obj = data[i];

                $("ul#notify-friend-ul").append('<li class=notify-friend-li><a href="/'
                    + obj.Username + '"><img src="Content/Images/' + obj.Id + '.jpg" />' + ' '
                    + obj.FirstName + ' ' + obj.LastName + '</a> ' +  
                    '<a class="accept" href=# data-id="' + obj.Id + '"><span class="glyphicon glyphicon-ok"</a>' + ' ' +
                    '<a class="accept" href=# data-id="' + obj.Id + '"><span class="glyphicon glyphicon-ok"</a> </li>');
            }

        });

    });

    $("body").on("click", "ul#notify-friend-ul li.close", function () {
        $("ul#notify-friend-ul").empty();
    })
});





