$(document).ready(function () {
    /* 
    / Login form AJAX
    */
    $("form#loginform").submit(function (e) {
        e.preventDefault();

        var $this = $(this);
        $this.find(".ajax-div").removeClass("hide");

        var username = $("#Username").val();
        var password = $("#Password").val();
        var url = "/account/Login";

        $.post(url, { username: username, password: password }, function (data) {
            var response = data.trim();

            if (response == "ok") {
                document.location.href = "/" + username;
            } else {
                $this.find(".ajax-div").addClass("hide");
                $("div.em").fadeIn("fast");
                setTimeout(function () {
                    $("div.em").fadeOut("fast");
                }, 2000);
            }
        });
    });
});
