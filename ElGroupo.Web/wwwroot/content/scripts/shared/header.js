$(document).ready(function () {
    $("#btnSearchEvents").on("click", function () {
        console.log('in event search');
        var searchText = $("#txtEventSearch").val();
        if (searchText === '') return;

        $.ajax({
            url: "/Events/Search/" +searchText,
            type: 'GET',
            //contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            //data: JSON.stringify(obj),
            success: function success(results) {


            },
            error: function error(err) {
                alert('error');
            }
        });

    });
});