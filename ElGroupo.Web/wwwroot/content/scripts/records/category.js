$(document).ready(function () {
    RecordCategory.Init();
});

RecordCategory = {
    Init: function () {


        $("div.view-description-column a").on("click", function () {
            console.log('edit clicked');
            $(".view-description-column").hide();
            $(".edit-description-column").show();

        });
        $(".cancel-description-column a").on("click", function () {
            $(".view-description-column").show();
            $(".edit-description-column").hide();
        });

        $("a.save-description-column").on("click", function () {
            console.log('save description clicked');
            Ajax.Post("/Records/category/updateDescriptionColumn", {
                id: Number($("#Id").val()), value: $("#txtDescriptionColumn").val(), returnView: false
            }).done(function (results) {
                $("#divDescriptionColumn").text($("#txtDescriptionColumn").val());
                $(".view-description-column").show();
                $(".edit-description-column").hide();
            });
        });

        $("a.save-value-column").on("click", function () {
            Ajax.Post("/Records/category/updateValueColumn", {
                id: Number($("#Id").val()), value: $("#txtValueColumn").val(), returnView: false
            }).done(function (results) {
                $("#divValueColumn").text($("#txtValueColumn").val());
                $(".view-value-column").show();
                $(".edit-value-column").hide();
            });
        });

        $("div.view-value-column a").on("click", function () {
            $(".view-value-column").hide();
            $(".edit-value-column").show();

        });
        $(".cancel-value-column a").on("click", function () {
            $("view-value-column").show();
            $("edit-value-column").hide();
        });

        $("button.add-subcategory-btn").on("click", function () {
            console.log('bark');
            $("div.add-subcategory").show();
            $(this).hide();

        });
        $("button.add-item-btn").on("click", function () {

            $("div.add-item").show();
            $(this).hide();

        });

        $("button.cancel-add-item").on("click", function () {
            $("#txtAddItem").val('');
            $("div.add-item").hide();
            $("button.add-item-btn").show();
        });

        $("button.cancel-add-subcategory").on("click", function () {
            $("#txtAddSubCategory").val('');
            $("div.add-subcategory").hide();
            $("button.add-subcategory-btn").show();
        });

        $("button.confirm-add-item").on("click", function () {
            var name = $("#txtAddItem").val();
            if (name === null || name === '') return;
            RecordsAdmin.CreateRecordItem(name, $("#Id").val(), null, function (results) {
                console.log('emptyubg subcategory list');
                $("#item-list").empty().html(results);
                $("#txtAddItem").val('');
                $("div.add-item").hide();
                $("button.add-item-btn").show();
            });
        });


        $("button.confirm-add-subcategory").on("click", function () {
            var name = $("#txtAddSubCategory").val();
            if (name === null || name === '') return;
            RecordsAdmin.CreateRecordSubCategory(Number($("#Id").val()), name, function (results) {
                console.log('emptyubg subcategory list');
                $("#sub-category-list").empty().html(results);
                $("#txtAddSubCategory").val('');
                $("div.add-subcategory").hide();
                $("button.add-subcategory-btn").show();
            });
        });

        $("html").on("click", "a[data-remove-sub-category]", function () {
            var id = Number($(this).attr('data-sub-category-id'));

            var name = $(this).closest('tr').find('td').eq(0).text();

            Confirm("Do you want to delete the sub category " + name + "?", function () {
                RecordsAdmin.DeleteRecordSubCategory(id, function (results) {
                    $("#sub-category-list").empty().html(results);
                });
            });
        });


        $("html").on("click", "a[data-remove-item]", function () {
            var id = Number($(this).attr('data-item-id'));
            var name = $(this).closest('tr').find('td').eq(0).text();
            Confirm("Do you want to delete the item " + name + "?", function () {
                RecordsAdmin.DeleteRecordItem(id, function (results) {
                    $("#item-list").empty().html(results);
                });
            });
        });





        $("button.add-element-btn").on("click", function () {
            console.log('add-element-btn click');
            $("div.add-element").show();
            $(this).hide();

        });
        $("button.cancel-add-element").on("click", function () {
            $("#selElement").val('');
            $("div.add-element").hide();
            $("button.add-element-btn").show();
        });

        $("button.confirm-add-element").on("click", function () {
            RecordsAdmin.CreateCategoryDefaultElement($("#Id").val(), $("#selElement").val(), $("#chkPrimaryDisplay").is(':checked'), function (results) {
                $("#element-list").empty().html(results);
                $("#selElement").val('');
                $("#chkPrimaryDisplay").prop('checked', false);
                $("div.add-element").hide();
                $("button.add-element-btn").show();
            });
        });

        $("html").on("click", "a[data-remove-element]", function () {
            var id = Number($(this).attr('data-element-id'));
            var name = $(this).closest('tr').find('td').eq(0).text();
            Confirm("Do you want to delete the element " + name + "?", function () {
                RecordsAdmin.DeleteDefaultElement(id, function (results) {
                    $("#element-list").empty().html(results);
                });
            });
        });


        $("html").on("click", "a[data-toggle-primary]", function () {
            var id = Number($(this).attr('data-element-id'));
            RecordsAdmin.SetRecordItemElementPrimary(id, function (results) {
                $("#element-list").empty().html(results);
            });
        });
    }
};