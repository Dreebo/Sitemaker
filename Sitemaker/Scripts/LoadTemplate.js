function LoadTemplate(e) {
    $.ajax({
        type: 'POST',
        url: "/Sites/LoadTemplate",
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
            url: "/Sites/LoadMenuEditor",
            success: function (data) {
                $("#allbutId").empty();
                $("#allbutId").append(data);
                setTimeout(function () {
                    LoadScript();
                }, 1);
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
                    let word = null;
                    $(this).append($("#text").html());
                    tinymce.init({
                        selector: '#link'
                    });
                    $("#textinput").click(
                        function () {
                            word = tinyMCE.get("link").getContent();
                            text.empty();
                            text.append(word);
                        });
                }


                if (draggableId == "dragg2") {
                    var image = $(this);
                    cloudinary.openUploadWidget(
                        {
                            cloud_name: 'dgy6x5krf', upload_preset: 'ntblzmxf', 'theme': 'purple',
                            cropping: 'server'
                        }, function (error, result) {
                                image.empty();
                                image.append("<img style=\"width:100%; height:100%;\" src=\"" + result[0].secure_url + "\"/>");
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
                                    $(video).append("<iframe width=\"100%\" height=\"100%\" src=\"" + link + "\" frameborder=\"0\" allowfullscreen></iframe>");
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
    let id = window.location.href;
    let position = id.lastIndexOf("/");
    id = id.slice(position + 1);
    let html = $(".no-js").html();
    
    var savePage = { 
        Id: id,
        HtmlCode: html
    }

    $.ajax({
        type: 'POST',
        url: "/Sites/SavePage",
        dataType: 'text',
        data: JSON.stringify(savePage),
        contentType: "application/json; charset=utf-8",
        traditional: true,
        success: function (data) {
          let  urlId = window.location.href;
          let pos = urlId.lastIndexOf("/");
          urlId = urlId.slice(0, pos);
          pos = urlId.lastIndexOf("/");
          let userName = urlId.slice(0, pos);
          let userNamePos = userName.lastIndexOf("/");
          userName = userName.slice(userNamePos + 1);
          urlId = urlId.slice(pos + 1);
            
          window.location.replace("https://localhost:44396/Sites/FillSite/" + userName + "/" + urlId);
        }
    });
};


