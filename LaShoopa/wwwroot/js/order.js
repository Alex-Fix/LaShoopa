$(function () {
    $(".orders__page__link").on("click", function (event) {
        event.preventDefault();
        let id = $(this).attr("data-id");
        let el = $(this);
        if (!isNaN(+id)) {
            $.ajax({
                url: `/Admin/DelOrderFromDb/${id}`,
                success: function () {
                    el.parent().parent().parent().remove();
                }
            });
        }
    });
});