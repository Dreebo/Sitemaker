$(function () {
    $(".thumb img").click(function () {
        $(".thumb")
            .children()
            .each((i, element) =>
                $(element).removeClass("selected"));
        $(this).addClass("selected");
    });

    $(".thumbMenu img").click(function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        } else {
            $(this).addClass("selected");
        }
    });


    $("#save").click(
    function () {
        var url = window.location.href;
        let model = {
            name: $("#Name").val(),
            about: $("#Textarea").val(),
            tags: $("#tags").val().split(','),
            selectedTemplate: null,
            selectedMenu: null,
            isTopBarExist: false,
            isSideBarExist: false,
            CreaterId:0
        }
        $(".thumb img.selected").each(function (index, element) {
            model.selectedTemplate = element.id;
        })
        $(".thumbMenu img.selected").each(function (index, element) {
            model.selectedMenu = $(element).attr("data-id");
        })

        $(".thumbMenu img.selected").each(function (index, element) {
            if ($(element).hasClass("top")) {
                model.isTopBarExist = true;
            }
            if ($(element).hasClass("side")) {
                model.isSideBarExist = true;
            }
        })
        

        let file = $("#uploadFile")[0].files[0];
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = function () {
            var site = {
                'Name': model.name,
                'About': model.about,
                'TemplateId': model.selectedTemplate,
                'Menu': {
                    'IsTopBarExicist': model.isTopBarExist,
                    'IsSideBarExicist': model.isSideBarExist
                },
                'MenuTypeId': model.selectedMenu,
                "Logo": reader.result
            };

            var tagsArray = model.tags;

            $.ajax({
                url: 'SaveSite',
                type: 'POST',
                contentType: 'application/json',
                traditional: true,
                success: function (response) {
                    if (response.result == 'Redirect')
                        window.location = response.url;
                },
                fail: function () {
                    alert(data);
                },
                data: JSON.stringify({site : site, tagsArray : tagsArray})
            });
        }

    });
})





