$(function () {

    $(".products__page__link.del__btn").on("click", function (event) {
        event.preventDefault();
        let id = $(this).attr("data-id");

        $.ajax({
            url: `/Admin/DelFromDb/${id}`,
            success: $(this).parent().parent().remove()
        });

        

    });

    

});