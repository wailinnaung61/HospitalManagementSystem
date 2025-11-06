$(function () {
    $("#dvPreview").attr("src", $('#ImageContent').val());
    $("#dvPreview").attr("class", "thumbnail");

    $("#dvPreview").click(function (e) {
        $("#fileupload").click();
    });
    
    $("#fileupload").change(function () {
        $("#dvPreview").html("");
        var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.jpg|.jpeg|.gif|.png|.bmp|.svg|.doc|.docx|.xlsx|.xls|.pptx|.kml|.gpx|.shp|.cmz|.pdf|.mp3|.mp4)$/;
        if (regex.test($(this).val().toLowerCase())) {
            if (typeof (FileReader) != "undefined") {
                $("#dvPreview").show();
                var reader = new FileReader();
                reader.onload = function (e) {
                    $("#ImageContent").val(e.target.result);
                    $("#dvPreview").attr("src", e.target.result);
                }
                reader.readAsDataURL($(this)[0].files[0]);
            } else {
                alert("This browser does not support FileReader.");
            }
        } else {
            alert("Please upload a valid image file.");
        }
    });
});