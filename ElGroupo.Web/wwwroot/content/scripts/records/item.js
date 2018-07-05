$(document).ready(function () {
    RecordItem.Init();
});

RecordItem = {
    GetSubCategories: function (cb) {
        var _cb = cb;
        RecordsAdmin.GetSubCategories($("#selEditCategory").val(), function (results) {
            $("#selEditSubCategory").empty();
            $("#selEditSubCategory").append('<option value="-1"></option>');
            if (results.length > 0) {

                for (var x = 0; x < results.length; x++) {
                    $("#selEditSubCategory").append('<option value="' + results[x].id + '">' + results[x].name + '</option>');
                }
                $("#divEditSubCategory").show();
            }
            else {
                $("#divEditSubCategory").hide();
            }
            
            if ($.isFunction(_cb)) _cb();
            //if (_cb instanceof function) _cb();

        });


    },
    Init: function () {
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


        $("button.change-category-btn").on("click", function () {
            $("#divViewCategory").hide();
            $(this).hide();
            $("#divEditCategory").show();
            RecordsAdmin.GetCategories(function (results) {
                $("#selEditCategory").empty();
                console.log(results);
                for (var x = 0; x < results.length; x++) {
                    $("#selEditCategory").append('<option value="' + results[x].id + '">' + results[x].value + '</option>');

                }

                if ($("#SubCategory_Id").length > 0) {
                    $("#selEditCategory").val($("#SubCategory_ParentCategory_Id").val());
                }
                else {
                    $("#selEditCategory").val($("#Category_Id").val());
                }
                RecordItem.GetSubCategories(function () {
                    if ($("#SubCategory_Id").length > 0) {
                        $("#selEditSubCategory").val($("#SubCategory_Id").val());
                    }
                    $("#selEditCategory").on("change", RecordItem.GetSubCategories);

                });
                
            });
        });

        $("a.cancel-category").on("click", function () {
            $("#divEditCategory").hide();
            $("button.change-category-btn").show();
            $("#divViewCategory").show();
            $("#selEditCategory").off("change");
            $("#selEditSubCategory").off("change");
        });
        $("a.save-category").on("click", function () {
            Confirm("Do you want to change the category/sub category this item appears on?", function () {
                RecordsAdmin.EditRecordItem($("#Id").val(), $("#item-name").text(), $("#selEditCategory").val(), $("#selEditSubCategory").val(), function () {
                    window.location.reload();

                });

            });
        });

        $("button.confirm-add-element").on("click", function () {
            var name = $("#txtAddItem").val();
            if (name === null || name === '') return;
            RecordsAdmin.CreateRecordItemElement($("#Id").val(), $("#selElement").val(), $("#chkPrimaryDisplay").is(':checked'), function (results) {
                console.log('emptyubg item list');
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
                RecordsAdmin.DeleteRecordItemElement(id, function (results) {
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