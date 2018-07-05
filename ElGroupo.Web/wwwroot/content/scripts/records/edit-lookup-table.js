$(document).ready(function () {
    RecordLookupTable.Init();
});

RecordLookupTable = {
    Init: function () {

        $("button.add-value-btn").on("click", function () {

            $("div.add-value").show();
            $(this).hide();

        });

        $("button.cancel-add-value").on("click", function () {
            $("#txtAddLookupTableName").val('');
            $("#txtAddLookupTableDescription").val('');
            $("div.add-value").hide();
            $("button.add-value-btn").show();
        });

        $("button.confirm-add-value").on("click", function () {
            var name = $("#txtAddValue").val();
            if (name === null || name === '') return;
            RecordsAdmin.AddItemToRecordElementLookupTable($("#Id").val(), name, function (results) {
                $("#value-list").empty().html(results);
                $("#txtAddLookupTableName").val('');
                $("#txtAddLookupTableDescription").val('');
                $("div.add-value").hide();
                $("button.add-value-btn").show();
            });
        });

        $("html").on("click", "a[data-remove-value]", function () {
            var id = Number($(this).attr('data-value-id'));
            var name = $(this).closest('tr').find('td').eq(0).text();
            Confirm("Do you want to delete the value " + name + "?", function () {
                RecordsAdmin.RemoveItemFromRecordElementLookupTable($("#Id").val(),id, function (results) {
                    $("#value-list").empty().html(results);
                });
            });
        });

    }
};