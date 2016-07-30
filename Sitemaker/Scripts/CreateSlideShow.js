$(function () {
    $(".thumb img").click(function(){
        $(".thumb").children().each((i, element) =>
            $(element).removeClass("selected"))
        $(this).addClass("selected");
    });

        $(".thumbMenu img").click(function(){
            $(".thumbMenu").children().each((i, element) =>
                $(element).removeClass("selected"))
            $(this).addClass("selected");
        });


    $("#save").click(
    function () {
        let model = {
            name: $("#Name").val(),
            about: $("#Textarea").val(),
            selectedTemplate: null,
            selectedMenu: null
        }
        $(".thumb img.selected").each(function (index, element) {
            model.selectedTemplate = element.id;      
        })
        $(".thumbMenu img.selected").each(function (index, element) {
            model.selectedMenu = $(element).attr("data-id");
        })
    

        let file = $("#uploadFile")[0].files[0];
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = function () {
            let dataObject = JSON.stringify({
                'Name': model.name,
                'About': model.about,
                'TemplateId': model.selectedTemplate,
                'MenuId': model.selectedMenu,
                "Logo": reader.result
            });

            $.ajax({
                url: 'SaveSite',
                type: 'POST',
                contentType: 'application/json',
                success: function (response) {
                    if (response.result == 'Redirect')
                        window.location = response.url;
                },
                fail: function () {
                    alert(data);
                },
                data: dataObject
            });
        }
   
    });
})



