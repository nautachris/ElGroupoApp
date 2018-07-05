$(document).ready(function () {
    EditElements.Init();
});

EditElements = {
    Init: function () {

        $("button.add-element-btn").on("click", function () {

            $("div.add-element").show();
            $(this).hide();

        });

        $("#chkAddLookupTable").on("change", function () {
            console.log('changed');
            if ($(this).is(':checked')) {
                $("#selAddLookupTables").prop('disabled', false);
            }
            else {
                $("#selAddLookupTables").prop('disabled', true);
            }
        });

        $("button.cancel-add-element").on("click", function () {
            $("#txtAddItem").val('');
            $("div.add-element").hide();
            $("button.add-element-btn").show();
        });

        $("#selAddDataType").on("change", function () {
            RecordsAdmin.GetInputTypesByDataTypeId($(this).val(), function (results) {
                $("#selAddInputType").empty();

                    for (var x = 0; x < results.length; x++) {
                        $("#selAddInputType").append('<option value="' + results[x].id + '">' + results[x].name + '</option>');
                    }
                

            });

        });

        $("button.confirm-add-element").on("click", function () {
            var name = $("#txtAddElement").val();
            if (name === null || name === '') return;
            var displayName = $("#txtAddDisplayName").val();
            if (displayName === null || displayName === '') return;
            var dataTypeId = Number($("#selAddDataType").val());
            if (!dataTypeId) return;
            var ltId = undefined;
            if ($("#chkAddLookupTable").is(':checked')) {
                ltId = Number($("#selAddLookupTables").val());
                if (!ltId) return;
            }
            var itId = Number($("#selAddInputType").val());
            if (!itId) return;
            RecordsAdmin.CreateRecordElement(name, displayName, dataTypeId, ltId, itId, function (results) {
                $("#element-list").empty().html(results);
                $("#txtAddElement").val('');
                $("#txtAddDisplayName").val('');
                $("#selAddDataType").val('');
                $("#selAddInputType").val('');
                $("#selAddLookupTables").val('');
                $("div.add-element").hide();
                $("button.add-element-btn").show();
            });
        });

        $("html").on("click", "a[data-delete]", function () {
            var id = Number($(this).closest('tr').attr('data-element-id'));
            var name = $(this).closest('tr').find('td').eq(0).text();
            Confirm("Do you want to delete the element " + name + "?", function () {
                RecordsAdmin.DeleteRecordElement(id, function (results) {
                    $("#element-list").empty().html(results);
                });
            });
        });



        $("html").on("click", "a[data-cancel]", function () {
            $('a[data-edit]').show();
            $('a[data-delete]').show();
            $('a[data-save]').hide();
            $(this).hide();
            $(this).closest('tr').find('#divLookupTableView').show();
            $(this).closest('tr').find('#divLookupTableEdit').hide();
            $(this).closest('tr').find('#divDataTypeView').show();
            $(this).closest('tr').find('#divDataTypeEdit').hide();
            $(this).closest('tr').find('#divElementNameView').show();
            $(this).closest('tr').find('#divElementNameEdit').hide();
            $(this).closest('tr').find('#divDisplayNameView').show();
            $(this).closest('tr').find('#divDisplayNameEdit').hide();
            $(this).closest('tr').find('#divInputTypeView').show();
            $(this).closest('tr').find('#divInputTypeEdit').hide();

        });


        $("html").on("click", "a[data-save]", function () {
            var $this = $(this);
            var dataTypeId = $(this).closest('tr').find('#selEditDataType').val();
            var lookupTableId = $(this).closest('tr').find('#selEditLookupTable').val();
            var inputTypeId = $(this).closest('tr').find('#selEditInputType').val();
            var name = $(this).closest('tr').find('#txtElementName').val();
            var displayName = $(this).closest('tr').find('#txtDisplayName').val();
            var id = $(this).closest('tr').attr('data-element-id');

            RecordsAdmin.EditRecordElement(id, name, displayName, dataTypeId, lookupTableId === '-1' ? null : lookupTableId, inputTypeId, function (results) {

                $this.closest('tr').find("#selEditDataType").off("change");
                $this.closest('tr').find('#divDataTypeView').text($this.closest('tr').find('#selEditDataType option[value=' + dataTypeId + ']').text());
                $this.closest('tr').find('#divLookupTableView').text($this.closest('tr').find('#selEditLookupTable option[value=' + lookupTableId +']').text());
                $this.closest('tr').find('#divElementNameView').text($this.closest('tr').find('#txtElementName').val());
                $this.closest('tr').find('#divDisplayNameView').text($this.closest('tr').find('#txtDisplayName').val());
                $('a[data-edit]').show();
                $('a[data-delete]').show();
                $('a[data-save]').hide();
                $('a[data-cancel]').hide();
                $this.hide();
                $this.closest('tr').find('#divElementNameView').show();
                $this.closest('tr').find('#divElementNameEdit').hide();
                $this.closest('tr').find('#divDisplayNameView').show();
                $this.closest('tr').find('#divDisplayNameEdit').hide();
                $this.closest('tr').find('#divLookupTableView').show();
                $this.closest('tr').find('#divLookupTableEdit').hide();
                $this.closest('tr').find('#divInputTypeView').show();
                $this.closest('tr').find('#divInputTypeEdit').hide();
                $this.closest('tr').find('#divDataTypeView').show();
                $this.closest('tr').find('#divDataTypeEdit').hide();
            });



            

        });

        $("html").on("click", "a[data-edit]", function () {
            $("#selEditInputType").remove();
            var id = Number($(this).closest('tr').attr('data-element-id'));
            $(this).closest('table').find('tbody tr').each(function () {
                var thisId = Number($(this).attr('data-element-id'));
                if (thisId !== id) {
                    $(this).find('a[data-edit]').hide();
                    $(this).find('a[data-delete]').hide();
                }
                else {

                    $(this).find("#divElementNameEdit").empty();
                    $(this).find("#divElementNameEdit").append('<input type="text" id="txtElementName" value="' + $(this).find('#divElementNameView').text() + '" style="width:200px;"/>');

                    $(this).find("#divDisplayNameEdit").empty();
                    $(this).find("#divDisplayNameEdit").append('<input type="text" id="txtDisplayName" value="' + $(this).find('#divDisplayNameView').text() + '" style="width:200px;"/>');

                    $(this).find("#divInputTypeEdit").empty();
                    $("#selAddInputType").clone().appendTo($(this).find("#divInputTypeEdit"));
                    $(this).find("#selAddInputType").attr('id', 'selEditInputType');
                    RecordsAdmin.GetInputTypesByDataTypeId($(this).find('td[data-data-type-id]').attr('data-data-type-id'), function (results) {
                        console.log('input types');
                        console.log(results);
                        $("#selEditInputType").empty();
                        for (var x = 0; x < results.length; x++) {
                            $("#selEditInputType").append('<option value="' + results[x].id + '">' + results[x].name + '</option>');
                        }
                        console.log($("#selEditInputType option").length);
                    });

                    $(this).find("#divDataTypeEdit").empty();
                    $("#selAddDataType").clone().appendTo($(this).find("#divDataTypeEdit"));
                    $(this).find("#selAddDataType").attr('id', 'selEditDataType');
                    $(this).find("#selEditDataType").val($(this).find('td[data-data-type-id]').attr('data-data-type-id'));
                    $(this).find("#divLookupTableEdit").empty();
                    $("#selAddLookupTables").clone().appendTo($(this).find("#divLookupTableEdit"));
                    $(this).find("#selAddLookupTables").attr('id', 'selEditLookupTable');
                    $("#selEditLookupTable").prepend('<option value="-1"></option>');
                    if ($(this).find('td[data-lookup-table-id]').attr('data-lookup-table-id')) $(this).find("#selEditLookupTable").val($(this).find('td[data-lookup-table-id]').attr('data-lookup-table-id'));
                    else $(this).find("#selEditLookupTable").val("-1");
                    
                    $(this).find("#selEditLookupTable").prop('disabled', false);



                    $(this).find("#selEditDataType").on("change", function () {
                        console.log('edit data type change');
                        RecordsAdmin.GetInputTypesByDataTypeId($(this).val(), function (results) {
                            $("#selEditInputType").empty();
                            for (var x = 0; x < results.length; x++) {
                                $("#selEditInputType").append('<option value="' + results[x].id + '">' + results[x].name + '</option>');
                            }
                        });

                    });

                    //selAddLookupTables
                    //selAddDataType
                    $(this).find('a[data-edit]').hide();
                    $(this).find('a[data-delete]').hide();
                    $(this).find('a[data-cancel]').show();
                    $(this).find('a[data-save]').show();
                    $(this).find('#divLookupTableView').hide();
                    $(this).find('#divLookupTableEdit').show();
                    $(this).find('#divDataTypeView').hide();
                    $(this).find('#divDataTypeEdit').show();
                    $(this).find('#divElementNameView').hide();
                    $(this).find('#divElementNameEdit').show();
                    $(this).find('#divDisplayNameView').hide();
                    $(this).find('#divDisplayNameEdit').show();
                    $(this).find('#divInputTypeView').hide();
                    $(this).find('#divInputTypeEdit').show();
                }
            });
           
        });

    }
};