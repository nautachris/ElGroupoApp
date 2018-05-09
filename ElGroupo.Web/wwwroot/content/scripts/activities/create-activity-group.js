$(document).ready(function () {
    CreateActivityGroup.Init();
});

CreateActivityGroup = {
    AddDocument: function (file) {
        console.log('in adddocument');
        console.log(file);
        var idx = CreateActivityGroup.Documents.length + 1;
        CreateActivityGroup.Documents.push({ id: idx, document: file });
        var $newRow = $($("#divRowTemplate").html());
        $newRow.attr('data-document-id', idx);
        $($newRow.find("div").get(0)).text(file.name);
        $($newRow.find("div").get(2)).attr('data-remove-document', idx);
        console.log($newRow.html());
        $("#divDocumentTable header").after($newRow);
    },
    Documents: [],
    GetDocumentsFormData: function (userActivityId) {
        var fd = new FormData();
        fd.append('userActivityId', Number(userActivityId));
        for (var x = 0; x < CreateActivityGroup.Documents.length; x++) {
            fd.append('file_' + x.toString(), CreateActivityGroup.Documents[x].document, CreateActivityGroup.Documents[x].document.name);
            fd.append('description_' + x.toString(), $("#divTable div[data-document-id=" + CreateActivityGroup.Documents[x].id + "] input[type=text]").val());
        }
        return fd;
    },
    InitDocumentTable: function () {
        $("#iptFile").on("change", function (evt) {
            console.log('file change');
            for (var x = 0; x < evt.target.files.length; x++) {
                CreateActivityGroup.AddDocument(evt.target.files[x]);
            }
            if (CreateActivityGroup.Documents.length > 0) $("#divDocumentTable").show();
            else $("#divDocumentTable").hide();
            $("#iptFile").val(null);
        });

        //delete button
        $("#divDocumentTable").on("click", "div[data-remove-document]", function () {

            var idToRemove = Number($(this).closest('div').attr('data-remove-document'));
            console.log(idToRemove);

            $("#divTable div[data-document-id=" + idToRemove + "]").remove();
            for (var x = 0; x < CreateActivityGroup.Documents.length; x++) {
                if (CreateActivityGroup.Documents[x].id === idToRemove) {
                    CreateActivityGroup.Documents.splice(x, 1);
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

        $("#StartTime").datepicker({
            defaultDate: new Date(Date.now())
        });
        $("#EndTime").datepicker({
            defaultDate: new Date(Date.now())
        });

        $("#StartTime").datepicker('setDate', new Date());
        $("#EndTime").datepicker('setDate', new Date());

        //$("#EndTime").datepicker();
        CreateActivityGroup.InitDocumentTable();
        $("#btnCreate").on("click", CreateActivityGroup.Create);

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
        //console.log('makepublic');
        //console.log(Boolean($("#MakePublic").val()));
        //console.log($("#MakePublic").val());
        //return;
        var model = {};
        model.attendenceType = Number($("#divAttendenceType input[type=radio]:checked").attr('data-attendence-type-id'));
        model.startTime = CreateActivityGroup.GetStartDate();
        model.endTime = CreateActivityGroup.GetEndDate();
        model.groupName = $("#txtGroupName").val();
        model.activityDescription = $("#txtDescription").val();
        model.activityLocation = $("#txtLocation").val();
        model.makePublic = $("#MakePublic").val() === 'True';
        if (model.makePublic) {
            model.SharedGroupIds = [];
            $(":checkbox[data-group-id]:checked").each(function () {
                model.SharedGroupIds.push(Number($(this).attr('data-group-id')));
            });
        }
        model.credits = [];
        $("div[data-credit-category-id]").each(function () {
            if ($(this).find(':checkbox').is(':checked')) {
                var hours = Number($(this).find('input[type=number]').val());
                model.credits.push({ CreditTypeCategoryId: Number($(this).attr('data-credit-category-id')), NumberOfCredits: hours});
            }
        });

        MessageDialog("Creating Activity Group");
        $.ajax({
            url: "/Activities/ActivityGroup/Create",
            type: 'POST',
            contentType: "application/json",
            data: JSON.stringify(model),
            async: true,
            cache: false,
            //dataType: 'html',
            success: function success(results) {
                if (CreateActivityGroup.Documents.length > 0) {
                    var fd = CreateActivityGroup.GetDocumentsFormData(results.userActivityId);
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
                                window.location.href = '/Activities/Dashboard';
                            });
                        },
                        error: function error(err) {
                            //alert('error');
                            MessageDialog("Error Creating New Activity Group. " + err.error);
                        }
                    });
                }
                else {
                    MessageDialog("Activity Group Saved", function () {
                        window.location.href = '/Activities/Dashboard';
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