$(function () {
  // Nav Toggle
  let nav = $("#nav");
  let navToggle = $("#navToggle");
  navToggle.on("click", function (event) {
    event.preventDefault();
    nav.toggleClass("show");
    $(this).toggleClass("active");
  });


    //AJAX
    $(".product__btn").on("click", function (event) {
        event.preventDefault();
        let id = $(this).attr("data-id");
        $.ajax({
            url: `/ShopCart/AddToCart/${id}`,
            success: changeColorCart()
        });
    });

    $("#addCart").on("click", function (event) {
        event.preventDefault();
        let id = $(this).attr("data-id");
        let boxes = $("input[type=radio]");
        let size = "";
        for (let i = 0; i < boxes.length; i++) {
            if (boxes[i].checked) {
                size = boxes[i].value;
                break;
            }
        }
        $.ajax({
            url: `/ShopCart/AddToCart/${id}?size=${size}`,
            success: changeColorCart()
        });
    });

    function changeColorCart() {
        $("#ShopCartBtn").addClass("active");
        setTimeout(function () {
            $("#ShopCartBtn").removeClass("active");
        }, 500);
    }


    //Del from cart
    $(".delete__btn").on("click", function (event) {
        event.preventDefault();
        let el = $(this);
        let id = $(this).attr("data-id");
        $.ajax({
            url: `/ShopCart/DelFromCart/${id}`,
            success: function () {
                el.parent().parent().parent().remove();
                let countEl = $("#countOfItems");

                let count = parseInt(countEl.html(), 10) - 1;
                countEl.text(`${count} items`);

                let price = 0;
                let prices = $(".cart__item__costs__price");
                for (let i = 0; i < prices.length; i++) {
                    price += parseInt(prices[i].innerHTML, 10);
                }
                $(".ordering__desc.submit").text(`${count} items worth ${price} grn`);
            },
        });
        
    });

   



});
