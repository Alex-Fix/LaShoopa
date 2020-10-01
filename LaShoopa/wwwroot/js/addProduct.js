$(function () {
    $("#addImg").on("click", function (event) {
        event.preventDefault();
        $(this).before("<input type=\"file\" class=\"add__product__input\"  name=\"ProductImgs\">");
    });
    $('#addSize').on("click", function (event) {
        event.preventDefault();
        $(this).before("<input type=\"text\" class=\"add__product__input\" name=\"Sizes\" value=\"\">");
    });
});