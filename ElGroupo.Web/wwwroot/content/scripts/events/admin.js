$(document).ready(function () {
    $("#btnSearch").on("click", function () {
        var val = $("#txtSearch").val();
        if (val === '') return;

        $.ajax({
            url: "/Events/Search/" + val,
            type: 'GET',
            //contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            //data: JSON.stringify(obj),
            success: function success(results) {
                $("#divEventList").html(results);

            },
            error: function error(err) {
                alert('fuck me');
            }
        });

    });

    $("#btnShowAll").on("click", function () {
        $.ajax({
            url: "/Events/Search/*",
            type: 'GET',
            //contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            //data: JSON.stringify(obj),
            success: function success(results) {
                $("#divEventList").html(results);

            },
            error: function error(err) {
                alert('fuck me');
            }
        });

    });

});