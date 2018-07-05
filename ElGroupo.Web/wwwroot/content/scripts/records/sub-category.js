﻿$(document).ready(function () {
    RecordSubCategory.Init();
});

RecordSubCategory = {
    Init: function () {

        $("button.add-item-btn").on("click", function () {

            $("div.add-item").show();
            $(this).hide();

        });

        $("button.cancel-add-item").on("click", function () {
            $("#txtAddItem").val('');
            $("div.add-item").hide();
            $("button.add-item-btn").show();
        });

        $("button.confirm-add-item").on("click", function () {
            var name = $("#txtAddItem").val();
            if (name === null || name === '') return;
            RecordsAdmin.CreateRecordItem(name, null, $("#Id").val(), function (results) {
                console.log('emptyubg subcategory list');
                $("#item-list").empty().html(results);
                $("#txtAddItem").val('');
                $("div.add-item").hide();
                $("button.add-item-btn").show();
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
            RecordsAdmin.CreateSubCategoryDefaultElement($("#Id").val(), $("#selElement").val(), $("#chkPrimaryDisplay").is(':checked'), function (results) {
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
            RecordsAdmin.SetDefaultElementPrimary(id, function (results) {
                $("#element-list").empty().html(results);
            });
        });

    }
};