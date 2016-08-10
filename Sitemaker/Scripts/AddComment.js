$("#addComment").click(function AddComment() {
    let urlId = window.location.href;
    let link = window.location.href;
    let comment = $("#comment").val();
    let pos = urlId.lastIndexOf("/");
    let id = urlId.slice(pos+1);
    urlId = urlId.slice(0, pos);
    pos = urlId.lastIndexOf("/");
    let userName = urlId.slice(pos+1);
    var message = {
        Id: id,
        UserName: userName,
        Comment: comment
    }

    $.ajax({
        type: 'POST',
        url: "/Sites/AddComment",
        dataType: 'text',
        data: message,
        success: function (data) {
            window.location.replace(link);
        }
    });
   
});
