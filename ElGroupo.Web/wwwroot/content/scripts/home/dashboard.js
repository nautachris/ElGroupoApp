$(document).ready(function () {
    

    $(".links").on("click", function () {
        $(".links").removeClass('bold');
        $(this).addClass('bold');
        $(".row.tab").hide();
        $(".row." + $(this).attr('data-link-type')).show();

    });


    //$("#secMain").on("click", ".event-delete", function () {
    //    $("#secDeleteDialog").attr('data-event-id', $(this).closest("li").attr("data-event-id"));
    //    $("#spanDeleteEventName").text('Do you want to delete the event ' + $(this).closest("li").find("#spanEventName").text());
    //    $("#secMain").hide();
    //    $("#secDeleteDialog").show();
    //});

    //$("#btnCancelDelete").on("click", function () {
    //    $("#secMain").show();
    //    $("#secDeleteDialog").hide();
    //});

    ////$("#btnConfirmDelete").on("click", function () {

    ////    var eid = Number($("#secDeleteDialog").attr('data-event-id'));
    ////    $.ajax({
    ////        url: "/Events/" + eid.toString(),
    ////        type: 'DELETE',
    ////        async: true,
    ////        cache: false,
    ////        success: function success(results) {
    ////            $("#secMain li[data-event-id=" + eid + "]").remove();

    ////        },
    ////        error: function error(err) {
    ////            alert('fuck me');
    ////        }
    ////    });
    ////    $("#secMain").show();
    ////    $("#secDeleteDialog").hide();
    ////});

});