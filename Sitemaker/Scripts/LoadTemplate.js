function LoadTemplate(e) {
    $.ajax({
        type: 'POST',
        url: "/Home/LoadTemplate",
        data: { id: e.id },
        success: function (data) {
            $("#main-form").empty();
            $("#main-form").append(data);
        }
    });
    LoadScript();
}



    function LoadMenuEditor() {
        $.ajax({
            type: 'POST',
            url: "/Home/LoadMenuEditor",
            success: function (data) {
                $("#allbutId").empty();
                $("#allbutId").append(data);
            }
        });
    }

function LoadScript() {
    $("#dragg1").draggable({ helper: "clone", scope: "push" });
    $("#dragg2").draggable({ helper: "clone", scope: "push" });
    $("#dragg3").draggable({ helper: "clone", scope: "push" });
    setTimeout(function () {
        $(".dropzone").droppable({
            scope: "push",
            over: function () {
                $(this).css("background-color");
            },
            drop: function (event, ui) {
                $(this).empty();
                let draggableId = ui.draggable.attr("id");
                if (draggableId == "dragg1")
                    $(this).append("<textarea></textarea>");
                if (draggableId == "dragg2") {
                    $(this).append(" <input type=\"file\" name=\"upload\" id=\"uploadFile\" />");
                    $('#uploadFile').cloudinary_upload_widget(
                        {
                            cloud_name: 'dgy6x5krf', upload_preset: 'ntblzmxf',
                            cropping: 'server', 'folder': 'user_photos'
                        },
                        function (error, result) {
                            $(this).empty();
                            $(this).append("<img style=\"width:70px; height:70px;\" src=\"" + result[0].secure_url + "\"/>")
                        });
                }
                if (draggableId == "dragg3") {
                    $(this).append("<textarea id=\"link\">Введите ссылку</textarea>")
                    let value = null
                    $(this).append("<button type=\"button\" id=\"video\" class=\"btn btn-template\" \">dfghfs</button>");
                    $("#video").click(function () {
                        value = $("#link").val();
                        if (value !== null) {
                            $(this).empty();
                            $(this).append("<video width=\"640\"  height=\"360\" src=\"https://youtu.be/a-Y59jF5VGA\"  controls autobuffer></video>");
                        }
                    });
                }
            }
        });
    }, 10);
};
