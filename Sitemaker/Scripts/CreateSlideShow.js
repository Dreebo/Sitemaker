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
        $.ajax({
            url: "SaveSite",
            type: "POST",            
            data: {id : model.selectedTemplate, name : model.name, about : model.about, dataid : model.selectedMenu},
            success: function (response) {
              
               //window.location.replace("http://localhost:50489/Home/Create");
            }
        });
    });
})



