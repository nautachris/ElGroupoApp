$(document).ready(function () {
    AddAttendenceLog.Init();
});

AddAttendenceLog = {
    AddDocument: function (file) {
        var idx = AddAttendenceLog.Documents.length + 1;
        AddAttendenceLog.Documents.push({ id: idx, document: file });
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
        for (var x = 0; x < AddAttendenceLog.Documents.length; x++) {
            fd.append('file_' + x.toString(), AddAttendenceLog.Documents[x].document, AddAttendenceLog.Documents[x].document.name);
            fd.append('description_' + x.toString(), $("#divTable div[data-document-id=" + AddAttendenceLog.Documents[x].id + "] input[type=text]").val());
        }
        return fd;
    },
    InitDocumentTable: function () {
        $("#iptFile").on("change", function (evt) {
            console.log('file change');
            for (var x = 0; x < evt.target.files.length; x++) {
                AddAttendenceLog.AddDocument(evt.target.files[x]);
            }
            if (AddAttendenceLog.Documents.length > 0) $("#divDocumentTable").show();
            else $("#divDocumentTable").hide();
            $("#iptFile").val(null);
        });

        //delete button
        $("#divDocumentTable").on("click", "div[data-remove-document]", function () {

            var idToRemove = Number($(this).closest('div').attr('data-remove-document'));
            console.log(idToRemove);

            $("#divTable div[data-document-id=" + idToRemove + "]").remove();
            for (var x = 0; x < AddAttendenceLog.Documents.length; x++) {
                if (AddAttendenceLog.Documents[x].id === idToRemove) {
                    AddAttendenceLog.Documents.splice(x, 1);
                    break;
                }
            }
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


        AddAttendenceLog.InitDocumentTable();

        //$("#btnUpload").on("click", AddAttendenceLog.Upload);
        $("#btnCreate").on("click", AddAttendenceLog.Create);

        //$("#divCreditTypes :checkbox:checked").each(function () {
        //    $("#divCreditCategories").show();
        //    var creditTypeId = Number($(this).attr('data-credit-type-id'));
        //    $("#divCreditCategories div[data-credit-type-id=" + creditTypeId.toString() + "]").show();
        //});


        //$("#iptFile").on("change", function (evt) {
        //    if (evt.target.files.length > 0) {
        //        $("#divFileDescription").show();
        //    }
        //    else {
        //        $("#divFileDescription").hide();
        //    }
        //    //var file = evt.target.files[0];
        //    //console.log(evt.target.files[0]);


        //});

    },

    Create: function () {
        
        var model = {};
        model.personalNotes = $("#txtMyNotes").val();
        model.isPresenting = $("rbPresentingYes").is(":checked");
        if ($("#divPresentationName").is(":visible")) {
            model.presentationName = $("#txtPresentationName").val();
        }
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
            url: "/Activities/AddAttendenceLog",
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(model),
            async: true,
            cache: false,
            //dataType: 'html',
            success: function success(results) {
                if (AddAttendenceLog.Documents.length > 0) {
                    var fd = AddAttendenceLog.GetDocumentsFormData(results.userActivityId);
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
                //$("#divViewAttendees").html(results);
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