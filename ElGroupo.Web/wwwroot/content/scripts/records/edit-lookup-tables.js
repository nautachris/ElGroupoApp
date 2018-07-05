$(document).ready(function () {
    RecordLookupTables.Init();
});

RecordLookupTables = {
    Init: function () {

        $("button.add-lookup-table-btn").on("click", function () {

            $("div.add-lookup-table").show();
            $(this).hide();

        });

        $("button.cancel-add-lookup-table").on("click", function () {
            $("#txtAddLookupTableName").val('');
            $("#txtAddLookupTableDescription").val('');
            $("div.add-lookup-table").hide();
            $("button.add-lookup-table-btn").show();
        });

        $("button.confirm-add-lookup-table").on("click", function () {
            var name = $("#txtAddLookupTableName").val();
            if (name === null || name === '') return;
            var description = $("#txtAddLookupTableDescription").val();
            if (description === null || description === '') return;
            RecordsAdmin.CreateRecordElementLookupTable($("#txtAddLookupTableName").val(), $("#txtAddLookupTableDescription").val(), function (results) {
                $("#lookup-table-list").empty().html(results);
                $("#txtAddLookupTableName").val('');
                $("#txtAddLookupTableDescription").val('');
                $("div.add-lookup-table").hide();
                $("button.add-lookup-table-btn").show();
            });
        });

        $("html").on("click", "a[data-remove-lookup-table]", function () {
            var id = Number($(this).attr('data-lookup-table-id'));
            var name = $(this).closest('tr').find('td').eq(0).text();
            Confirm("Do you want to delete the lookup table " + name + "?", function () {
                RecordsAdmin.DeleteRecordElementLookupTable(id, function (results) {
                    $("#lookup-table-list").empty().html(results);
                });
            });
        });

    }
};