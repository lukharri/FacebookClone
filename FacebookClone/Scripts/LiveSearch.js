$(document).ready(function () {
    /*
    / Live Search
    */
    $("input#search-text").keyup(function () {

        var searchVal = $("input#search-text").val();

        $("ul#live-search-ul").empty();

        if (searchVal == "" || searchVal == " ") return false;

        var url = "profile/LiveSearch";

        $.post(url, { searchVal: searchVal }, function (data) {

            if ($("ul#live-search-ul li.close").length) return;

            if (data.length > 0) {
                $("ul#live-search-ul").append("<li class=close>x</li>");
            }

            for (var i = 0; i < data.length; i++) {
                var obj = data[i];

                $("ul#live-search-ul").append('<li class=live-search-li><a href="/'
                    + obj.Username + '"><img src="Content/Images/' + obj.Username + '.jpg" />' + ' '
                    + obj.FirstName + ' ' + obj.LastName + '</a></li>');
            }

        });

    });

    $("body").on("click", "ul#live-search-ul li.close", function () {
        $("ul#live-search-ul").empty();
        $("input#search-text").val("");
    })

});
