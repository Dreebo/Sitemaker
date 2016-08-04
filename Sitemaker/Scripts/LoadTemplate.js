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

                if (draggableId == "dragg1") {
                    var text = $(this);
                    $(this).append($("#text").html());                   
                }

                if (draggableId == "dragg2") {
                    var image = $(this);
                    cloudinary.openUploadWidget(
                        {
                            cloud_name: 'dgy6x5krf', upload_preset: 'ntblzmxf', 'theme': 'purple'
                        }, function (error, result) {
                                image.empty();
                                image.append("<img style=\"width:70px; height:70px;\" src=\"" + result[0].secure_url + "\"/>");
                                var contentImage = result[0].secure_url;
                            }
                        );
                   
                }

                if (draggableId == "dragg3") {
                    var video = $(this);
                    var link = null;
                    $('#linkall').dialog({
                        title: 'Введите ссылку',
                        buttons: [{
                            text: "OK", click: function () {
                                link = $("#linkvideo").val();
                                if (link !== null) {
                                    $(video).append("<iframe width=\"560\" height=\"315\" src=\"" + link + "\" frameborder=\"0\" allowfullscreen></iframe>");
                                }
                                $(this).dialog("close")
                            }
                        }],
                        resizable: false
                    });
                   
                }
            }
        });
    }, 20);
};

function SavePage() {
    var page = {
        HtmlCode: $(".no-js").html()
    }

    $.ajax({
        type: 'POST',
        url: "/Home/SavePage",
        dataType: 'text',
        data: JSON.stringify(page),
        contentType: "application/json; charset=utf-8",
        traditional: true,
        success: function (data) {
            window.location.replace(url);
        }
    });
};

