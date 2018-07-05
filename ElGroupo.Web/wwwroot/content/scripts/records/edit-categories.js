$(document).ready(function () {
    RecordCategories.Init();
});

RecordCategories = {
    Init: function () {

        $("button.add-category-btn").on("click", function () {

            $("div.add-category").show();
            $(this).hide();

        });

        $("button.cancel-add-category").on("click", function () {
            $("#txtAddCategory").val('');
            $("div.add-category").hide();
            $("button.add-category-btn").show();
        });

        $("button.confirm-add-category").on("click", function () {
            var name = $("#txtAddCategory").val();
            if (name === null || name === '') return;
            RecordsAdmin.CreateRecordCategory(name, function (results) {
                $("#category-list").empty().html(results);
                $("#txtAddCategory").val('');
                $("div.add-category").hide();
                $("button.add-category-btn").show();
            });
        });

        $("html").on("click", "a[data-remove-category]", function () {
            var id = Number($(this).attr('data-category-id'));
            var name = $(this).closest('tr').find('td').eq(0).text();
            Confirm("Do you want to delete the category " + name + "?", function () {
                RecordsAdmin.DeleteRecordCategory(id, function (results) {
                    $("#category-list").empty().html(results);
                });
            });
        });

    }
};