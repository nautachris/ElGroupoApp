$(document).ready(function () {
    CreateActivity.Init();
});

CreateActivity = {
    AddDocument: function (file) {
        var idx = CreateActivity.Documents.length + 1;
        CreateActivity.Documents.push({ id: idx, document: file });
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
        for (var x = 0; x < CreateActivity.Documents.length; x++) {
            fd.append('file_' + x.toString(), CreateActivity.Documents[x].document, CreateActivity.Documents[x].document.name);
            fd.append('description_' + x.toString(), $("#divTable div[data-document-id=" + CreateActivity.Documents[x].id + "] input[type=text]").val());
        }
        return fd;
    },
    InitDocumentTable: function () {
        $("#iptFile").on("change", function (evt) {
            console.log('file change');
            for (var x = 0; x < evt.target.files.length; x++) {
                CreateActivity.AddDocument(evt.target.files[x]);
            }
            if (CreateActivity.Documents.length > 0) $("#divDocumentTable").show();
            else $("#divDocumentTable").hide();
            $("#iptFile").val(null);
        });

        //delete button
        $("#divDocumentTable").on("click", "div[data-remove-document]", function () {

            var idToRemove = Number($(this).closest('div').attr('data-remove-document'));
            console.log(idToRemove);

            $("#divTable div[data-document-id=" + idToRemove + "]").remove();
            for (var x = 0; x < CreateActivity.Documents.length; x++) {
                if (CreateActivity.Documents[x].id === idToRemove) {
                    CreateActivity.Documents.splice(x, 1);
                    break;
                }
            }
        });

    },
    Init: function () {
        $("div.credit-type-table input[type=text]").on("click", function () {
            console.log('input clicked');
            $(this).select();
        });
        $("#btnAddDocuments").on("click", function () {
            $("#iptFile").click();
        });
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
                $("div.credit-type-table[data-credit-type-id=" + creditTypeId.toString() + "]").show();
            }
            else {
                $("div.credit-type-table[data-credit-type-id=" + creditTypeId.toString() + "]").hide();
            }


        });
        $("#aEditTitle").on("click", function () {
            $("#divEditGroupName").show();
            $("#txtDescription").select();
            //$("#divGroupName").hide();
        });
        $("#rbPresentingYes, #rbPresentingNo").on("change", function () {           
            if ($("#rbPresentingYes").is(":checked")) {              
                $("#divPresentationName").show();
            }
            else {
                console.log('unchecked');
                $("#divPresentationName").hide();
            }
        });

        CreateActivity.InitDocumentTable();
        $("#StartTime").datepicker({
            defaultDate: new Date(Date.now())
        });
        $("#EndTime").datepicker({
            defaultDate: new Date(Date.now())
        });

        $("#StartTime").datepicker('setDate', new Date());
        $("#EndTime").datepicker('setDate', new Date());


        //$("#EndTime").datepicker();

        $("#btnCreate").on("click", CreateActivity.Create);

    },
    GetStartDate: function () {
        var date = new Date($("#StartTime").val());
        var hour = Number($("#selStartHour").val());
        var ampm = $("#selStartAMPM").val();
        if (ampm === 'AM') {
            if (hour === 12) hour = 0;
        }
        else {
            if (hour !== 12) hour = hour + 12;
        }
        var min = Number($("#selStartMin").val());

        var newDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), hour, min);
        return newDate;
    },
    GetEndDate: function () {
        var date = new Date($("#EndTime").val());
        var hour = Number($("#selEndHour").val());
        var ampm = $("#selEndAMPM").val();
        if (ampm === 'AM') {
            if (hour === 12) hour = 0;
        }
        else {
            if (hour !== 12) hour = hour + 12;
        }
        var min = Number($("#selEndMin").val());

        var newDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), hour, min);
        return newDate;
    },
    Create: function () {

        //var fd = CreateActivity.GetDocumentsFormData(222);
        //console.log(fd);
        //$.ajax({
        //    url: "/Activities/UploadDocument",
        //    type: 'POST',
        //    processData: false,
        //    data: fd,
        //    //contentType: 'multipart/form-data',
        //    contentType: false,
        //    async: true,
        //    cache: false,
        //    //dataType: 'html',
        //    success: function success(results) {
        //        MessageDialog("Attendence Logged");

        //        //$("#divViewAttendees").html(results);
        //    },
        //    error: function error(err) {
        //        //alert('error');
        //        MessageDialog("Error Creating New Activity Group. " + err.error);
        //    }
        //});
        //return;

        var model = {};
        
        model.isPresenting = $("#rbPresentingYes").is(":checked");
        model.publicNotes = $("#txtPublicNotes").val();
        model.personalNotes = $("#txtMyNotes").val();
        model.startTime = CreateActivity.GetStartDate();
        model.endTime = CreateActivity.GetEndDate();
        model.activityGroupId = Number($("#ActivityGroupId").val());
        model.activityDescription = $("#txtDescription").val();
        model.activityLocation = $("#txtLocation").val();
        model.credits = [];
        $("input[type=checkbox][data-credit-type-id]").each(function () {
            if ($(this).is(':checked')) {
                var creditType = $(this).attr('data-credit-type-id');
                $("div.credit-type-table[data-credit-type-id=" + creditType + "] input[type=text]").each(function () {
                    var hourCount = Number($(this).val());
                    if (hourCount > 0) {
                        model.credits.push({ CreditTypeCategoryId: Number($(this).attr('data-credit-category-id')), NumberOfCredits: hourCount });
                    }
                });
            }
        });



        //$("div[data-credit-category-id]").each(function () {
        //    if ($(this).find(':checkbox').is(':checked')) {
        //        var hours = Number($(this).find('input[type=number]').val());
        //        model.credits.push({ CreditTypeCategoryId: Number($(this).attr('data-credit-category-id')), NumberOfCredits: hours });
        //    }
        //});
        $.ajax({
            url: "/Activities/Activity/Create",
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(model),
            async: true,
            cache: false,
            //dataType: 'html',
            success: function success(results) {
                if (CreateActivity.Documents.length > 0) {
                    var fd = CreateActivity.GetDocumentsFormData(results.userActivityId);
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