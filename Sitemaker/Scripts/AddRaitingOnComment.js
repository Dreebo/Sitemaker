$(function () {
    $(document).ready(function () {
        let averageRating = +($(".AverageRating").text());
        $(".star").each(function (i, element) {
            if (i / 2 <= +(averageRating)) $(element).addClass("selected")
            else return false;
        });
        if (+(averageRating) == 5) $(".star").addClass("max");
        if (+(averageRating) == 1) $(".star-1").addClass("min");
    });
    let id = window.location.href;
    let position = id.lastIndexOf("/");
    id = id.slice(position + 1);
    $(".stars label").click(function () {
        $(".star").removeClass("selected");
        $(".star").removeClass("max");
        $(".star").removeClass("min");
        let model = {
            Id: id,
            rating: null
        }
        let elem = $(this).attr("class");
        $(".star").each(function (i, element) {
            if (element.className == elem) {
                model.rating = element.id;
                return false;
            }
            $(element).addClass("selected")
        });
        if (elem == "star star-1") { $(this).addClass("min") }
        $(this).addClass("selected");
        if (elem == "star star-5") { $(".star").addClass("max") }

        $.ajax({
            type: 'POST',
            url: "/Sites/AddRating",
            dataType: 'text',
            data: model,
            success: function (data) {
                window.location.replace(window.location.href);
            }
        });

    });
});