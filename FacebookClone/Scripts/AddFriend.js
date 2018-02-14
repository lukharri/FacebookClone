// init hub connection
var hub = $.connection.echo;

//hub.client.test = function (msg) {
//    console.log(msg);
//}


// notifies user of a pending friend request
hub.client.NotifyFriend = function (f, count) {
    $("span.notify-friend." + f + "> span").text(count);
    $("span.notify-friend." + f).addClass("red");
};


$.connection.hub
    .start()
    .done(function () {

        // hub.server.hello("SignalR working...");

        // Add friend
        $("a.add-friend").click(function (e) {
            e.preventDefault();

            var friend = '@ViewBag.Username';
            var url = "/profile/AddFriend";

            $.post(url, { friend: friend }, function (data) {
                $(".add-friends-div").removeClass("alert-info").addClass("alert-warning")
                    .html('<p>Pending friend request.</p>');
            }).done(function () {
                hub.server.notify(friend);
            });
        });
    });



