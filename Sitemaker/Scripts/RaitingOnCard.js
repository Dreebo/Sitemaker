$(document).ready(function () {
    $(".stars").each(function (i, mainelement) {
        let id = $(mainelement.children).attr("id");
        let averageRating = +(id);
        let label = $(mainelement.children).children();
        $(label).each(function (i, element) {
            if (mainelement.children.form == element.parentElment) {
                if (i / 2 < +(averageRating)) $(element).addClass("selected")
                else return false;
            }
        });
        if (+(averageRating) == 5) $(label).addClass("max");
    });
});