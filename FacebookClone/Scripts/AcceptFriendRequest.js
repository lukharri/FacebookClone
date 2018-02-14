$(document).ready(function () {

    $("body").on("click", "a.accept", function (e) {

        e.preventDefault();

        var $this = $(this);
        var friendId = $this.data("id");

        var url = "profile/AcceptFriendRequest";

        $.post(url, { friendId: friendId }, function (data) {
        }).done(function () {
            $this.parent().fadeOut("fast");
            //hub.server.getFriendCount();
            //hub.server.getFriendsCount(friendId);
        });

    });

});