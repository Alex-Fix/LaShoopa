$(function () {
  let slider = $("#prod__slider");
  slider.slick({
    infinite: true,
    slidesToShow: 1,
    slidesToScroll: 1,
    fade: false,
    arrows: false,
    dots: true,
  });

    let radio = $("#radio-1");
    if (radio.length) {
        radio.attr("checked", "checked");
    }


});
