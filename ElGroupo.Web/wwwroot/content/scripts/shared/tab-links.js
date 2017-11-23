$(document).ready(function () {
    $(".links").on("click", function () {

        $(".links").removeClass('bold');
        $(this).addClass('bold');
        $(".row.tab").hide();
        $(".row." + $(this).attr('data-link-type')).show();


    });

});