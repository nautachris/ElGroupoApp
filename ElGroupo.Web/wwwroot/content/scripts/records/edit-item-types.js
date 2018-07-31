$(document).ready(function () {
    RecordItemTypes.Init();
});

RecordItemTypes = {

    LoadCategories: function () {
        Ajax.Get({ url: "/Records/GetCategories" }).done(function (results) {
            $("#selItemTypeCategory").empty();
            for (var x = 0; x < results.length; x++) {
                $("#selItemTypeCategory").append('<option value="' + results[x].id + '">' + results[x].value + '</option>');
            }
            $("#selItemTypeCategory").off("change");
            $("#selItemTypeCategory").on("change", function () {
                RecordsAdmin.GetSubCategories($(this).val(), function (subcats) {
                    console.log(subcats);
                    $("#selItemTypeSubCategory").empty();
                    $("#selItemTypeSubCategory").append('<option value=""></option>');
                    for (var x = 0; x < subcats.length; x++) {
                        $("#selItemTypeSubCategory").append('<option value="' + subcats[x].id + '">' + subcats[x].name + '</option>');
                    }

                });
            });
            
        });

    },
    Init: function () {
        console.log($("#selItemTypeSubCategory").length);
        RecordItemTypes.LoadCategories();


        $("button.add-item-type-btn").on("click", function () {

            $("div.add-item-type").show();
            $(this).hide();

        });

        $("button.cancel-add-item-type").on("click", function () {
            RecordItemTypes.LoadCategories();
            $("div.add-item-type").hide();
            $("button.add-item-type-btn").show();
        });

        $("button.confirm-add-item-type").on("click", function () {

            RecordsAdmin.CreateItemType($("#txtAddItemTypeName").val(), $("#selItemTypeCategory").val(), $("#selItemTypeSubCategory").val(), function (results) {
                $("#item-type-list").empty().html(results);

                $("div.add-item-type").hide();
                $("button.add-item-type-btn").show();
            });
        });

        $("html").on("click", "a[data-remove-item-type]", function () {
            var id = Number($(this).attr('data-item-type-id'));
            var name = $(this).closest('tr').find('td').eq(0).text();
            Confirm("Do you want to delete the item type " + name + "?", function () {
                RecordsAdmin.DeleteItemType(id, function (results) {
                    $("#item-type-list").empty().html(results);
                });
            });
        });

    }
};