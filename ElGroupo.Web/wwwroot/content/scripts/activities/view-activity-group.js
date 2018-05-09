$(document).ready(function () {

    $("a.delete-activity").on("click", function () {
        var name = $(this).closest('div[data-activity-name]').attr('data-activity-name');
        var id = $(this).closest('div[data-activity-id]').attr('data-activity-id');

        Confirm("Do you want to delete the activity " + name + "?", function () {
            $.ajax({
                url: "/Activities/Activity/Delete/" + id,
                type: 'DELETE',
                contentType: "application/json",
                async: true,
                cache: false,
                //dataType: 'html',
                success: function success(results) {
                    MessageDialog("Activity Deleted");
                    $("div.activity-list-item-container[data-activity-id=" + id + "]").remove();

                    //$("#divViewAttendees").html(results);
                },
                error: function error(err) {
                    //alert('error');
                    MessageDialog("Error Creating New Activity Group. " + err.error);
                }
            });

        });
    });

    $("#btnDeleteActivityGroup").on("click", function () {
        var name = $("#lblActivityGroupName").text();
        var id = $("#ActivityGroupId").val();

        Confirm("Do you want to delete the activity group " + name + "?", function () {
            $.ajax({
                url: "/Activities/ActivityGroup/Delete/" + id,
                type: 'DELETE',
                contentType: "application/json",
                async: true,
                cache: false,
                //dataType: 'html',
                success: function success(results) {
                    MessageDialog("Activity Deleted", function () {
                        window.location.href = '/Activities/Dashboard';
                    });


                    //$("#divViewAttendees").html(results);
                },
                error: function error(err) {
                    console.log(err);
                    //alert('error');
                    MessageDialog("Error Deleting Activity Group. " + err.error);
                }
            });

        });
    });
});