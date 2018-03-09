$(document).ready(function () {

    $("a#send-say").click(function (e) {

        e.preventDefault();

        var $this = $(this);

        $this.parent().find("img").removeClass("hide");

        var id = $this.data("id");

        var message = $this.parent().find("textarea").val();

        var url = "/profile/UpdateWallMessage";

        $.post(url, { id: id, message: message }, function (data) {
        }).done(function () {
            $this.parent().find("img").addClass("hide");

        });
    });

});