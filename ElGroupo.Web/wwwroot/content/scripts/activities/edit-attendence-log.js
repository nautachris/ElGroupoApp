$(document).ready(function () {
    EditAttendenceLog.Init();
});
PendingUploads = null,
    EditAttendenceLog = {
        AddDocument: function (files) {
            EditAttendenceLog.PendingUploads = new FormData();
            EditAttendenceLog.PendingUploads.append('userActivityId', Number($("#UserActivityId").val()));

            //we need to add descriptions
            $("#divUploadDocumentsTable div[data-upload-document-id]").remove();
            for (var x = 0; x < files.length; x++) {
                EditAttendenceLog.PendingUploads.append('file_' + x.toString(), files[x], files[x].name);
                var $newRow = $($("#divUploadDocumentRowTemplate").get(0).outerHTML);
                $newRow.attr('data-upload-document-id', x);
                $newRow.removeAttr('id');
                $newRow.find('div[data-file-name]').text(files[x].name);
                $("#divUploadDocumentHeader").after($newRow);

            }
            $("#btnUploadDocuments").off();
            $("#btnUploadDocuments").on("click", function () {
                console.log('btnuploaddocuments click');
                $('#divUploadDocumentsTable div[data-upload-document-id]').each(function () {
                    var docId = $(this).attr('data-upload-document-id');
                    var desc = $(this).find('input[data-file-description]').val();
                    EditAttendenceLog.PendingUploads.append('description_' + docId, desc);
                });
                EditAttendenceLog.PendingUploads.append('returnTable', true);
                $.ajax({
                    url: "/Activities/UploadDocuments",
                    type: 'POST',
                    processData: false,
                    data: EditAttendenceLog.PendingUploads,
                    contentType: false,
                    async: true,
                    cache: false,
                    success: function success(results) {
                        $("#divDocumentsContainer").show();
                        $("#divDocuments").html(results);
                        $("#divAddDocuments").hide();
                        //MessageDialog("Activity and Documents Saved", function () {
                        //    //window.location.href = '/Activities/Group/' + $("#ActivityGroupId").val();
                        //    //window.location.href = '/Activities/Records';
                        //});
                    },
                    error: function error(err) {
                        //alert('error');
                        MessageDialog("Error Creating New Activity Group. " + err.error);
                    }
                });

            });

            $("#divUploadDocumentsTable div[data-delete-pending-document").off();
            $("#divUploadDocumentsTable div[data-delete-pending-document").on("click", function () {
                var docId = $(this).closest('div[data-upload-document-id]').attr('data-upload-document-id');
                EditAttendenceLog.PendingUploads.delete('file_' + docId);
                $(this).closest('div[data-upload-document-id]').remove();
                if ($('#divUploadDocumentsTable div[data-upload-document-id]').length === 0) {
                    $("#divAddDocuments").hide();
                    EditAttendenceLog.PendingUploads = null;
                }

            });
            $("#divAddDocuments").show();
        },
        //Documents: [],
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
                EditAttendenceLog.AddDocument(evt.target.files);
                //console.log('file change');
                //for (var x = 0; x < evt.target.files.length; x++) {
                //    EditAttendenceLog.AddDocument(evt.target.files[x]);
                //}
                //if (EditAttendenceLog.Documents.length > 0) $("#divDocumentTable").show();
                //else $("#divDocumentTable").hide();
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


            $("#divDocuments").on("click", "div[data-delete-document]", function () {

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
                            $("#divDocuments").empty().html(results);
                        },
                        error: function error(err) {
                            //alert('error');
                            MessageDialog("Error Deleting Document. " + err.error);
                        }
                    });


                });

            });


        },
        Init: function () {

            $("#btnCancelAddDocuments").on("click", function () {
                $("#divAddDocuments").hide();
                EditAttendenceLog.PendingUploads = null;
            });
            //click on any credit type checkboxes
            $("#divCreditTypes input[type=text]").on("click", function () {
                console.log('input clicked');
                $(this).select();
            });

            $("#btnAddDocuments").on("click", function () {
                $("#iptFile").click();
            });

            $("#rbPresentingYes, #rbPresentingNo").on("change", function () {
                if ($("#rbPresentingYes").is(":checked")) {
                    $("#divPresentationName").show();
                }
                else {
                    $("#divPresentationName").hide();
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
            model.personalNotes = $("#txtMyNotes").val();
            model.isPresenting = $("#rbPresentingYes").is(":checked");
            if ($("#divPresentationName").is(":visible")) {
                model.presentationName = $("#txtPresentationName").val();
            }
            model.activityId = Number($("#ActivityId").val());
            model.userActivityId = Number($("#UserActivityId").val());
            model.credits = [];
            $("#divCreditTypes input[type=text]").each(function () {
                var hours = Number($(this).val());
                model.credits.push({ CreditTypeCategoryId: Number($(this).attr('data-credit-category-id')), NumberOfCredits: hours });

            });
            $.ajax({
                url: "/Activities/EditAttendenceLog",
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify(model),
                async: true,
                cache: false,
                success: function success(results) {

                    MessageDialog("Activity Updated", function () {
                        window.location.href = '/Activities/Records';
                    });
                },
                error: function error(err) {
                    //alert('error');
                    MessageDialog("Error Creating New Activity Group. " + err.error);
                }
            });



        }
    }