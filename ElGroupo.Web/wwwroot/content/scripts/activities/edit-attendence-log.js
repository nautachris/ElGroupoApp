$(document).ready(function () {
    EditAttendenceLog.Init();
});

EditAttendenceLog = {
    AddDocument: function (file) {
        var idx = EditAttendenceLog.Documents.length + 1;
        EditAttendenceLog.Documents.push({ id: idx, document: file });
        var $newRow = $($("#divRowTemplate").html());
        $newRow.attr('data-document-id', idx);
        $($newRow.find("div").get(0)).text(file.name);
        $($newRow.find("div").get(2)).attr('data-remove-document', idx);
        $("#divDocumentTable header").after($newRow);
    },
    Documents: [],
    GetDocumentsFormData: function (userActivityId) {
        var fd = new FormData();
        fd.append('userActivityId', Number(userActivityId));
        for (var x = 0; x < EditAttendenceLog.Documents.length; x++) {
            fd.append('file_' + x.toString(), EditAttendenceLog.Documents[x].document, EditAttendenceLog.Documents[x].document.name);
            fd.append('description_' + x.toString(), $("#divTable div[data-document-id=" + EditAttendenceLog.Documents[x].id + "] input[type=text]").val());
        }
        return fd;
    },
    InitDocumentTable: function () {
        $("#iptFile").on("change", function (evt) {
            console.log('file change');
            for (var x = 0; x < evt.target.files.length; x++) {
                EditAttendenceLog.AddDocument(evt.target.files[x]);
            }
            if (EditAttendenceLog.Documents.length > 0) $("#divDocumentTable").show();
            else $("#divDocumentTable").hide();
            $("#iptFile").val(null);
        });

        //delete button
        $("#divDocumentTable").on("click", "div[data-remove-document]", function () {

            var idToRemove = Number($(this).closest('div').attr('data-remove-document'));
            console.log(idToRemove);

            $("#divTable div[data-document-id=" + idToRemove + "]").remove();
            for (var x = 0; x < EditAttendenceLog.Documents.length; x++) {
                if (EditAttendenceLog.Documents[x].id === idToRemove) {
                    EditAttendenceLog.Documents.splice(x, 1);
                    break;
                }
            }
        });


        $("#divExistingDocuments").on("click", "div[data-delete-document]", function () {

            var idToRemove = Number($(this).closest('div[data-document-id]').attr('data-document-id'));
            console.log(idToRemove);

            Confirm("Do you want to remove the selected document from this activity?", function () {
                $.ajax({
                    url: "/Activities/DeleteDocument/" + idToRemove.toString(),
                    type: 'DELETE',
                    contentType: "application/json",
                    async: true,
                    cache: false,
                    success: function success(results) {
                        $("#divExistingDocuments div[data-document-id=" + idToRemove + "]").remove();
                    },
                    error: function error(err) {
                        //alert('error');
                        MessageDialog("Error Creating New Activity Group. " + err.error);
                    }
                });

                
            });

        });


    },
    Init: function () {
        //click on any credit type checkboxes
        $("#divCreditTypes :checkbox").on("change", function () {
            if ($("#divCreditTypes :checkbox:checked").length > 0) {
                $("#divCreditCategories").show();
            }
            else {
                $("#divCreditCategories").hide();
            }


            var creditTypeId = Number($(this).attr('data-credit-type-id'));
            var isChecked = $(this).is(':checked');

            if (isChecked) {
                $("#divCreditCategories div[data-credit-type-id=" + creditTypeId.toString() + "]").show();
            }
            else {
                $("#divCreditCategories div[data-credit-type-id=" + creditTypeId.toString() + "]").hide();
            }
        });


        $("#btnUpload").on("click", EditAttendenceLog.Upload);
        $("#btnCreate").on("click", EditAttendenceLog.Create);

        $("#divCreditTypes :checkbox:checked").each(function () {
            $("#divCreditCategories").show();
            var creditTypeId = Number($(this).attr('data-credit-type-id'));
            $("#divCreditCategories div[data-credit-type-id=" + creditTypeId.toString() + "]").show();
        });

        EditAttendenceLog.InitDocumentTable();
        //$("#iptFile").on("change", function (evt) {
        //    if (evt.target.files.length > 0) {
        //        $("#divFileDescription").show();
        //        for (var x = 0; x < evt.target.files.length; x++) {
        //            EditAttendenceLog.Files.push(evt.target.files[x]);
        //        }
        //        console.log(EditAttendenceLog.Files);
        //    }
        //    else {
        //        $("#divFileDescription").hide();
        //    }
        //    //var file = evt.target.files[0];
        //    //console.log(evt.target.files[0]);


        //});

    },
    Upload: function () {
        console.log($("#iptFile")[0].files);
        //what 
        var fd = new FormData();
        var fileList = $("#iptFile")[0].files;
        for (var x = 0; x < fileList.length; x++) {
            fd.append('file', fileList[x], fileList[x].name);
        }
        //fd.append('file', $("#iptFile")[0].files[0], $("#iptFile")[0].files[0].name);
        fd.append('fileName', $("#iptFile")[0].files[0].name);
        fd.append('description', $("#txtFileDescription").val());
        fd.append('userActivityId', Number($("#UserActivityId").val()));
        console.log(fd);
        $.ajax({
            url: "/Activities/UploadDocument",
            type: 'POST',
            processData: false,
            data: fd,
            //contentType: 'multipart/form-data',
            contentType: false,
            async: true,
            cache: false,
            //dataType: 'html',
            success: function success(results) {
                MessageDialog("Attendence Logged");
                window.location.href = '/Activities/Group/' + $("#ActivityGroupId").val();
                //$("#divViewAttendees").html(results);
            },
            error: function error(err) {
                //alert('error');
                MessageDialog("Error Creating New Activity Group. " + err.error);
            }
        });
    },
    Create: function () {
        
        var model = {};
        model.attendenceType = Number($("#divAttendenceType input[type=radio]:checked").attr('data-attendence-type-id'));
        model.activityId = Number($("#ActivityId").val());
        model.userActivityId = Number($("#UserActivityId").val());
        model.credits = [];
        $("div[data-credit-category-id]").each(function () {
            if ($(this).find(':checkbox').is(':checked')) {
                var hours = Number($(this).find('input[type=number]').val());
                model.credits.push({ CreditTypeCategoryId: Number($(this).attr('data-credit-category-id')), NumberOfCredits: hours});
            }
        });
        $.ajax({
            url: "/Activities/EditAttendenceLog",
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(model),
            async: true,
            cache: false,
            //dataType: 'html',
            success: function success(results) {
                if (EditAttendenceLog.Documents.length > 0) {
                    var fd = EditAttendenceLog.GetDocumentsFormData(Number($("#UserActivityId").val()));
                    $.ajax({
                        url: "/Activities/UploadDocuments",
                        type: 'POST',
                        processData: false,
                        data: fd,
                        contentType: false,
                        async: true,
                        cache: false,
                        success: function success(results) {
                            MessageDialog("Activity and Documents Saved", function () {
                                window.location.href = '/Activities/Group/' + $("#ActivityGroupId").val();
                            });
                        },
                        error: function error(err) {
                            //alert('error');
                            MessageDialog("Error Creating New Activity Group. " + err.error);
                        }
                    });
                }
                else {
                    MessageDialog("Activity Created", function () {
                        window.location.href = '/Activities/Group/' + $("#ActivityGroupId").val();
                    });

                }
            },
            error: function error(err) {
                //alert('error');
                MessageDialog("Error Creating New Activity Group. " + err.error);
            }
        });
        //Ajax.Post("/Activities/CreateActivityGroup", model).done(function () {
        //    console.log('success');
        //});
        //console.log(model);




        //group name
        //shared group ids
        //credit categories and hours
        //start date
        //start time
        //end date
        //end time
        //description


    }
}