$(document).ready(function () {
    UserRecord.Init();
});



UserRecord = {


    Documents: [] = [],

    NewDocumentFiles: [] = [],
    NewDocumentCount: 0,
    UploadDocuments: function (recordItemId, files) {
        UserRecord.PendingUploads = new FormData();
        UserRecord.PendingUploads.append('item-id', recordItemId);
        $("#divUploadDocumentsTable div[data-upload-document-id]").remove();

        for (var x = 0; x < files.length; x++) {
            UserRecord.PendingUploads.append('file_' + x.toString(), files[x], files[x].name);
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
                UserRecord.PendingUploads.append('description_' + docId, desc);
            });
            UserRecord.PendingUploads.append('returnTable', true);
            $.ajax({
                url: "/Records/UploadDocuments",
                type: 'POST',
                processData: false,
                data: UserRecord.PendingUploads,
                contentType: false,
                async: true,
                cache: false,
                success: function success(results) {
                    $("#divDocumentsContainer").show();
                    $("#divDocuments").html(results);
                    $("#divAddDocuments").hide();

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
            UserRecord.PendingUploads.delete('file_' + docId);
            $(this).closest('div[data-upload-document-id]').remove();
            if ($('#divUploadDocumentsTable div[data-upload-document-id]').length === 0) {
                $("#divAddDocuments").hide();
                UserRecord.PendingUploads = null;
            }

        });
        $("#divAddDocuments").show();
    },
    AddDocumentsToNewRecord: function (files) {
        console.log('in adddocumentstonewrecord');
        for (var x = 0; x < files.length; x++) {
            UserRecord.NewDocumentFiles.push({ id: UserRecord.NewDocumentCount, file: files[x] });

            let $rowCopy = $($("#divDocumentRowTemplate").html());
            $rowCopy.attr('data-file-id', UserRecord.NewDocumentCount);
            $rowCopy.find('div').eq(0).text(files[x].name);
            $("#divDocumentRowTemplate").before($rowCopy);
            UserRecord.NewDocumentCount++;
        }
    },
    LoadItemDetails: function ($container) {

        var recordItemId = Number($container.attr('data-record-item-id'));

        var _id = recordItemId;
        Ajax.Post("/Records/ViewItemDetails", { id: Number(recordItemId), returnView: false }).done(function (results) {

            //new - if the itemuser record doesn't exist for this
            //console.log($("div[data-item-user-id=" + _id + "]").length);
            $("div[data-record-item-id=" + _id + "]").after(results);

            UserRecord.InitRowClickEvents(false);
            UserRecord.InitDetails($("div[data-item-details-container-id=" + _id + "]"));
            UserRecord.Documents = [];
        });
    },
    InitRowClickEvents: function (eventOn) {
        if (eventOn === true) {
            //if the record user item row doesnt exist yet

            $("html").on("click", "div[data-record-item-id]", function () {
                UserRecord.LoadItemDetails($(this));

            });
            $("html").on("click", "div.record-add-document", function (evt) {
                console.log('add document clicked');
                evt.stopPropagation();

            });
        }
        else {
            $("html").off("click", "div[data-record-item-id]");
            $("html").off("click", "div.record-add-document");
        }
    },
    NewUserRecord: function (categoryId) {
        var _categoryId = categoryId;
        Ajax.Get("/Records/userrecord/create/" + categoryId).done(function (results) {
            $(".add-category-item").addClass('active');
            $(".add-category-item[data-category-id=" + _categoryId + "]").empty().html(results);
            UserRecord.InitRowClickEvents(false);
            UserRecord.InitNewDetails($(".add-category-item[data-category-id=" + _categoryId + "]"));
            UserRecord.Documents = [];
            UserRecord.NewDocumentCount = 0;
            UserRecord.NewDocumentFiles = [];
            $("button.add-new-record-item").hide();

        });
    },
    Init: function () {
        UserRecord.InitRowClickEvents(true);
        $("html").on("change", "#selItemType", function () {
            if ($(this).val() === '-1') {
                $("#divCustomName").show();

            }
            else {
                $("#divCustomName").hide();
            }
        });
        $("html").on("click", "button.save-user-data", function () {
            UserRecord.SaveDetails($(this).closest('div[data-item-details-container-id]'));
        });

        $("html").on("click", "button.save-new-user-data", function () {
            //from save button - $(this).closest('.create-user-record').find('#iptNewItemCategoryId')
            let type = null;
            let id = -1;
            if ($(this).closest('div.create-user-record').find('#iptNewItemCategoryId').length > 0) {
                type = 'category';
                id = $(this).closest('div.create-user-record').find('#iptNewItemCategoryId').val();
            }
            else {
                type = 'subcategory';
                id = $(this).closest('div.create-user-record').find('#iptNewItemSubCategoryId').val();
            }
            if (UserRecord.NewDocumentFiles.length === 0) {
                UserRecord.SaveNewDetails($(this).closest('div.create-user-record'), type, id);
            }
            else {
                UserRecord.SaveNewDetailsWithDocuments($(this).closest('div.create-user-record'), type, id);
            }

        });

        $("html").on("click", "a.cancel-new-user-data", function () {
            if ($(this).closest('div.add-sub-category-item').length > 0) {
                $('div.sub-category-item-list[data-sub-category-id=' + $(this).closest('div.add-sub-category-item').attr('data-sub-category-id') + ']').show();
            }
            else {
                $('div.category-item-list[data-category-id=' + $(this).closest('div.add-category-item').attr('data-category-id') + ']').show();
            }
            //
            UserRecord.NewDocumentCount = 0;
            UserRecord.NewDocumentFiles = [];
            $(this).closest('div.create-user-record').remove();
            $("button.add-new-record-item").show();
            $(this).closest('div.add-category-item').empty();
            $(".add-category-item").removeClass('active');
            UserRecord.InitRowClickEvents(true);
            //
            //UserRecord.SaveDetails($(this).closest('div[data-item-details-container-id]'));

        });
        $("html").on("click", "button.add-new-record-item[data-category-id]", function () {
            UserRecord.NewUserRecord($(this).attr('data-category-id'), null);
            console.log('add new category item');
        });
        $("html").on("click", "button.add-new-record-item[data-sub-category-id]", function () {
            UserRecord.NewUserRecord(null, $(this).attr('data-sub-category-id'));
            console.log('add new sub category item');
        });
        $("html").on("click", "button.cancel-user-data", function () {
            UserRecord.InitRowClickEvents(true);
            $(this).closest('div[data-item-details-container-id]').remove();
        });
        $("html").on("click", "a.cancel-user-data", function () {
            UserRecord.InitRowClickEvents(true);
            $(this).closest('div[data-item-details-container-id]').remove();
        });
        $("html").on("click", "a.delete-user-data", function () {
            //category-item-list-container
            //sub-category-item-list-container
            let $container = $(this).closest('div[data-item-details-container-id]');
            //let recordType = null;
            let id = -1;
            if ($container.closest('div.category-item-list-container').length > 0) {
                recordType = 'category';
                id = Number($container.closest('div.category-item-list-container').attr('data-category-id'));
            }
            else {
                recordType = 'subCategory';
                id = Number($container.closest('div.sub-category-item-list-container').attr('data-sub-category-id'));
            }
            let $list = $(this).closest('.category-item-list-container').length > 0 ? $(this).closest('.category-item-list-container') : $(this).closest('.sub-category-item-list-container');
            let idToDelete = Number($(this).closest('div[data-item-details-container-id]').attr('data-item-details-container-id'));
            let $this = $(this);
            Confirm("Do you want to delete this record and all associated documents?", function () {
                Ajax.Delete("/Records/item/delete", { id: idToDelete, showHidden: $('#ShowHiddenRecords').val(), returnView: true }).done(function (results) {

                    //replace the list 
                    UserRecord.InitRowClickEvents(true);
                    $container.remove();
                    //$list.empty().html(results);
                    if (recordType === 'category') {
                        $('div.category-container').empty().html(results);
                    }
                    else {
                        $('div.sub-category-container[data-sub-category-id=' + id + ']').empty().html(results);
                    }
                });
            });






            //var containerId = Number($container.attr('data-item-details-container-id'));
            //var _$container = $container;
            //$.ajax({
            //    url: '/Records/SaveUserData',
            //    type: 'POST',
            //    data: JSON.stringify(UserRecord.GetUserData($container)),
            //    async: true,
            //    cache: false,
            //    //dataType: 'application/json',
            //    contentType: 'application/json',
            //    success: function success(results) {
            //        //$("div[data-item-user-id=" + containerId + "]").replaceWith(results);
            //        //$("div[data-record-item-id=" + containerId + "] > div.primary-display").text(results.displayValue);
            //        if (recordType === 'category') {
            //            $('div.category-item-list-container[data-category-id=' + id + ']').empty().html(results);
            //        }
            //        else {
            //            $('div.sub-category-container[data-sub0category-id=' + id + ']').empty().html(results);
            //        }
            //        UserRecord.InitRowClickEvents(true);
            //        _$container.remove();
            //    },
            //    error: function error(err) {
            //        console.log(err);
            //        alert('error');
            //    }
            //});

        });

        $("html").on("click", "a.hide-user-data", function () {
            //setting to inactive
            let $container = $(this).closest('div[data-item-details-container-id]');
            //let $list = $(this).closest('.category-item-list-container').length > 0 ? $(this).closest('.category-item-list-container') : $(this).closest('.sub-category-item-list-container');

            let recordType = null;
            let id = -1;
            if ($container.closest('div.category-item-list-container').length > 0) {
                recordType = 'category';
                id = Number($container.closest('div.category-item-list-container').attr('data-category-id'));
            }
            else {
                recordType = 'subCategory';
                id = Number($container.closest('div.sub-category-item-list-container').attr('data-sub-category-id'));
            }



            let idToHide = Number($(this).closest('div[data-item-details-container-id]').attr('data-item-details-container-id'));
            let $this = $(this);
            Confirm("Do you want to hide this record?", function () {
                Ajax.Post("/Records/item/hide", { id: idToHide, showHidden: $('#ShowHiddenRecords').val(), returnView: true }).done(function (results) {

                    //replace the list 
                    UserRecord.InitRowClickEvents(true);
                    $container.remove();
                    if (recordType === 'category') {
                        $('div.category-container').empty().html(results);
                    }
                    else {
                        $('div.sub-category-container[data-sub-category-id=' + id + ']').empty().html(results);
                    }
                });
            });

        });
        $("html").on("click", "a.show-user-data", function () {
            //setting to inactive
            let $container = $(this).closest('div[data-item-details-container-id]');
            //let $list = $(this).closest('.category-item-list-container').length > 0 ? $(this).closest('.category-item-list-container') : $(this).closest('.sub-category-item-list-container');

            let recordType = null;
            let id = -1;
            if ($container.closest('div.category-item-list-container').length > 0) {
                recordType = 'category';
                id = Number($container.closest('div.category-item-list-container').attr('data-category-id'));
            }
            else {
                recordType = 'subCategory';
                id = Number($container.closest('div.sub-category-item-list-container').attr('data-sub-category-id'));
            }



            let idToHide = Number($(this).closest('div[data-item-details-container-id]').attr('data-item-details-container-id'));
            let $this = $(this);
            Confirm("Do you want to show this record?", function () {
                Ajax.Post("/Records/item/show", { id: idToHide, showHidden: $('#ShowHiddenRecords').val(), returnView: true }).done(function (results) {

                    //replace the list 
                    UserRecord.InitRowClickEvents(true);
                    $container.remove();
                    if (recordType === 'category') {
                        $('div.category-container').empty().html(results);
                    }
                    else {
                        $('div.sub-category-container[data-sub-category-id=' + id + ']').empty().html(results);
                    }
                });
            });

        });
        $("html").on("click", "button.add-documents-btn", function () {
            console.log('add doc btn');
            $("#iptFileExistingRecord").click();

        });


        $("html").on("change", "#selSubCategory", function () {
            let $parentContainer = $(this).closest('div.add-category-item');
            let $beforeResultsDiv = $(this).closest('div.select-sub-category-container');
            switch (Number($(this).val())) {
                case 0:
                    console.log('existing create-user-record count', $parentContainer.find('.create-user-record').length);
                    $parentContainer.find('.create-user-record').remove();
                    $('.select-sub-category-cancel').show();
                    break;
                case -1:
                    Ajax.Get("/Records/userrecord/new/category/" + $(this).attr('data-category-id')).done(function (results) {
                        console.log('existing create-user-record count', $parentContainer.find('.create-user-record').length);
                        $parentContainer.find('.create-user-record').remove();
                        $beforeResultsDiv.after(results);
                        UserRecord.InitNewDetails($parentContainer.find('.create-user-record'));
                        $('.select-sub-category-cancel').hide();
                    });
                    break;
                default:
                    Ajax.Get("/Records/userrecord/new/subcategory/" + $(this).val()).done(function (results) {
                        console.log('existing create-user-record count', $parentContainer.find('.create-user-record').length);
                        $parentContainer.find('.create-user-record').remove();
                        $beforeResultsDiv.after(results);
                        UserRecord.InitNewDetails($parentContainer.find('.create-user-record'));
                        $('.select-sub-category-cancel').hide();
                    });
                    break;

            }

        });

        $("html").on("click", "button.add-documents-new-record-btn", function () {
            console.log('new file change');
            $("#iptFileNewRecord").click();

        });

        $("html").on("change", "#iptFileExistingRecord", function (evt) {
            var recordItemId = Number($(this).closest('div[data-item-details-container-id').attr('data-item-details-container-id'));
            UserRecord.UploadDocuments(recordItemId, evt.target.files);
            $(this).val(null);
        });

        $("html").on("change", "#iptFileNewRecord", function (evt) {

            UserRecord.AddDocumentsToNewRecord(evt.target.files);
            $(this).val(null);
        });

        $("html").on("click", "#divDocuments div[data-delete-document]", function () {
            console.log('delete docs');
            var idToRemove = Number($(this).closest('div[data-document-id]').attr('data-document-id'));
            console.log(idToRemove);

            Confirm("Do you want to remove the selected document from this record?", function () {
                $.ajax({
                    url: "/Records/DeleteDocument/" + idToRemove.toString(),
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

        $("html").on("click", "#divDocuments div[data-delete-pending-document]", function () {
            console.log('delete docs');
            var idToRemove = Number($(this).closest('div.pending-document-row').attr('data-file-id'));
            console.log(idToRemove);

            Confirm("Do you want to remove the selected document?", function () {
                $("#divDocuments div.pending-document-row[data-file-id=" + idToRemove + "]").remove();
                for (let x = 0; x < UserRecord.NewDocumentFiles.length; x++) {
                    if (UserRecord.NewDocumentFiles[x].id === idToRemove) {
                        UserRecord.NewDocumentFiles.splice(x, 1);
                    }
                }

            });

        });

    },
    GetNewUserData: function ($container) {
        var ary = [];
        var containerObj = {

            recordItemTypeId: $container.find('#selItemType').val(),
            name: $container.find('#txtCustomName').val(),
            elements: []
        };

        if ($container.find('#iptNewItemCategoryId').length > 0) {
            containerObj.categoryId = Number($container.find('#iptNewItemCategoryId').val());
            containerObj.subCategoryId = null;
        }
        else {
            containerObj.subCategoryId = Number($container.find('#iptNewItemSubCategoryId').val());
            containerObj.categoryId = null;
        }
        //categoryId: $container.find('#iptNewItemCategoryId').val(),
        //    subCategoryId: $container.find('#iptNewItemSubCategoryId').val(),
        $container.find('div[data-element-id]').each(function () {
            var obj = { id: Number($(this).attr('data-element-id')), value: null };
            switch ($(this).attr('data-input-type')) {
                case 'autocomplete':
                    if ($(this).find('input[type=text]').attr('data-lookup-id') !== '' && $(this).find('input[type=text]').val() !== '') {
                        obj.value = Number($(this).find('input[type=text]').attr('data-lookup-id'));
                    }
                    break;
                case 'checkbox':
                    obj.value = $(this).find('input[type=checkbox]').is(':checked');
                    break;
                case 'datepicker':
                    obj.value = $(this).find('input[type=text]').datepicker('getDate');
                    break;
                case 'datetimepicker':
                    if ($(this).find('input[type=text]').datepicker('getDate') !== null) {
                        var exactTime = $(this).find('input[type=text]').datepicker('getDate');
                        var hour = Number($(this).find('select[data-hour]').val());
                        var min = Number($(this).find('select[data-min]').val());
                        if ($(this).find('select[data-am-pm]').val() === 'PM') hour = hour + 12;
                        exactTime.setHours(hour);
                        exactTime.setMinutes(min);
                        obj.value = exactTime;
                    }

                    break;
                case 'dropdownlist':
                    obj.value = $(this).find('select').val();
                    break;
                case 'numerictextbox':
                    if ($(this).find('input[type=number]').val() !== '') {
                        obj.value = Number($(this).find('input[type=number]').val());
                    }
                    break;
                case 'radiobuttonlist':
                    if ($(this).find('input[type=radio]:checked').length > 0) {
                        obj.value = $(this).find('input[type=radio]:checked').val();
                    }

                    break;
                case 'textbox':
                    if ($(this).find('input[type=text]').val() !== '') {
                        obj.value = $(this).find('input[type=text]').val();
                    }

                    break;
            }

            if (isNaN(obj.itemElementId)) {
                console.log($(this).html());
            }
            containerObj.elements.push(obj);
        });
        console.log(containerObj);
        return containerObj;
    },
    GetUserData: function ($container) {

        var containerObj = {
            itemId: Number($container.attr('data-item-details-container-id')),
            itemTypeId: $container.find('#selItemType').val(),
            name: $container.find('#txtCustomName').val(),
            showHidden: $('#ShowHiddenRecords').val(),
            userData: []
        };

        if ($container.find('#iptNewItemCategoryId').length > 0) {
            containerObj.categoryId = Number($container.find('#iptNewItemCategoryId').val());
            containerObj.subCategoryId = null;
        }
        else {
            containerObj.subCategoryId = Number($container.find('#iptNewItemSubCategoryId').val());
            containerObj.categoryId = null;
        }

        $container.find('div[data-element-id]').each(function () {
            var obj = { id: Number($(this).attr('data-element-id')), value: null };
            switch ($(this).attr('data-input-type')) {
                case 'autocomplete':
                    if ($(this).find('input[type=text]').attr('data-lookup-id') !== '' && $(this).find('input[type=text]').val() !== '') {
                        obj.value = Number($(this).find('input[type=text]').attr('data-lookup-id'));
                    }
                    break;
                case 'checkbox':
                    obj.value = $(this).find('input[type=checkbox]').is(':checked');
                    break;
                case 'datepicker':
                    obj.value = $(this).find('input[type=text]').datepicker('getDate');
                    break;
                case 'datetimepicker':
                    if ($(this).find('input[type=text]').datepicker('getDate') !== null) {
                        var exactTime = $(this).find('input[type=text]').datepicker('getDate');
                        var hour = Number($(this).find('select[data-hour]').val());
                        var min = Number($(this).find('select[data-min]').val());
                        if ($(this).find('select[data-am-pm]').val() === 'PM') hour = hour + 12;
                        exactTime.setHours(hour);
                        exactTime.setMinutes(min);
                        obj.value = exactTime;
                    }

                    break;
                case 'dropdownlist':
                    obj.value = $(this).find('select').val();
                    break;
                case 'numerictextbox':
                    if ($(this).find('input[type=number]').val() !== '') {
                        obj.value = Number($(this).find('input[type=number]').val());
                    }
                    break;
                case 'radiobuttonlist':
                    if ($(this).find('input[type=radio]:checked').length > 0) {
                        obj.value = $(this).find('input[type=radio]:checked').val();
                    }

                    break;
                case 'textbox':
                    if ($(this).find('input[type=text]').val() !== '') {
                        obj.value = $(this).find('input[type=text]').val();
                    }

                    break;
            }


            containerObj.userData.push(obj);
        });
        console.log(containerObj);
        return containerObj;
    },
    SaveDetails: function ($container) {

        let recordType = null;
        let id = -1;
        if ($container.closest('div.category-item-list-container').length > 0) {
            recordType = 'category';
            id = Number($container.closest('div.category-item-list-container').attr('data-category-id'));
        }
        else {
            recordType = 'subCategory';
            id = Number($container.closest('div.sub-category-item-list-container').attr('data-sub-category-id'));
        }

        var containerId = Number($container.attr('data-item-details-container-id'));
        var _$container = $container;
        $.ajax({
            url: '/Records/SaveUserData',
            type: 'POST',
            data: JSON.stringify(UserRecord.GetUserData($container)),
            async: true,
            cache: false,
            //dataType: 'application/json',
            contentType: 'application/json',
            success: function success(results) {
                //$("div[data-item-user-id=" + containerId + "]").replaceWith(results);
                //$("div[data-record-item-id=" + containerId + "] > div.primary-display").text(results.displayValue);
                if (recordType === 'category') {
                    $('div.category-container').empty().html(results);
                }
                else {
                    $('div.sub-category-container[data-sub-category-id=' + id + ']').empty().html(results);
                }
                UserRecord.InitRowClickEvents(true);
                _$container.remove();
            },
            error: function error(err) {
                console.log(err);
                alert('error');
            }
        });

    },
    SaveNewDetailsWithDocuments: function ($container, parentType, id) {
        let _parentType = parentType;
        let _id = id;
        let _$container = $container;
        let postObj = UserRecord.GetNewUserData($container);
        postObj.showHidden = $('#ShowHiddenRecords').val();
        postObj.returnView = false;
        $.ajax({
            url: '/Records/userrecord/savenew',
            type: 'POST',
            data: JSON.stringify(postObj),
            async: true,
            cache: false,
            contentType: 'application/json',
            success: function success(results) {
                let recordItemId = Number(results.recordItemId);
                let newDocFormData = new FormData();
                newDocFormData.append('item-id', recordItemId);
                $('#divDocuments div[data-file-id]').each(function () {
                    var fileId = Number($(this).attr('data-file-id'));
                    var desc = $(this).find('input[type=text]').val();
                    for (let fileCount = 0; fileCount < UserRecord.NewDocumentFiles.length; fileCount++) {
                        if (UserRecord.NewDocumentFiles[fileCount].id === fileId) {
                            newDocFormData.append('file_' + fileCount.toString(), UserRecord.NewDocumentFiles[fileCount].file, UserRecord.NewDocumentFiles[fileCount].name);
                            break;
                        }
                    }
                    newDocFormData.append('description_' + fileId, desc);
                    newDocFormData.append('show-hidden', $('#ShowHiddenRecords').val());
                });
                console.log(newDocFormData);
                $.ajax({
                    url: '/Records/userrecord/savenewdocuments',
                    type: 'POST',
                    data: newDocFormData,
                    async: true,
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function success(results) {
                        if (_parentType === 'category') {
                            $('div.category-container').empty().html(results);

                        }
                        else {
                            $('div.sub-category-container[data-sub-category-id=' + _id + ']').empty().html(results);
                        }
                        UserRecord.InitRowClickEvents(true);
                        _$container.remove();
                        $('div.add-category-item').empty();
                        $(".add-category-item").removeClass('active');
                        $('button.add-new-record-item').show();
                    },
                    error: function error(err) {
                        console.log(err);
                        alert('error');
                    }
                });
            },
            error: function error(err) {
                console.log(err);
                alert('error');
            }
        });
    },
    SaveNewDetails: function ($container, parentType, id) {
        console.log('showhidden', $('#ShowHiddenRecords').val());
        let _parentType = parentType;
        let _id = id;
        let _$container = $container;
        let postObj = UserRecord.GetNewUserData($container);
        postObj.returnView = true;
        postObj.showHidden = $('#ShowHiddenRecords').val();
        $.ajax({
            url: '/Records/userrecord/savenew',
            type: 'POST',
            data: JSON.stringify(postObj),
            async: true,
            cache: false,
            contentType: 'application/json',
            success: function success(results) {
                if (_parentType === 'category') {
                    $('div.category-container').empty().html(results);
                }
                else {
                    $('div.sub-category-container[data-sub-category-id=' + _id + ']').empty().html(results);
                }
                UserRecord.InitRowClickEvents(true);
                _$container.remove();
                $('button.add-new-record-item').show();
                $('div.add-category-item').empty();
                $(".add-category-item").removeClass('active');

            },
            error: function error(err) {
                console.log(err);
                alert('error');
            }
        });



    },
    InitNewDetails: function ($container) {
        $container.find('div[data-element-id]').each(function () {
            switch ($(this).attr('data-input-type')) {
                case 'autocomplete':
                    UserRecord.InitDetailsAutoComplete($(this));
                    break;
                case 'checkbox':
                    UserRecord.InitDetailsCheckBox($(this));
                    break;
                case 'datepicker':
                    UserRecord.InitDetailsDatePicker($(this));
                    break;
                case 'datetimepicker':
                    UserRecord.InitDetailsDateTimePicker($(this));
                    break;
                case 'dropdownlist':
                    UserRecord.InitDetailsDropDownList($(this));
                    break;
                case 'numerictextbox':
                    UserRecord.InitDetailsNumericTextBox($(this));
                    break;
                case 'radiobuttonlist':
                    UserRecord.InitDetailsRadioButtonList($(this));
                    break;
                case 'textbox':
                    UserRecord.InitDetailsTextBox($(this));
                    break;
            }
        });

        $container.find('input[type=text]').on('click', function () {
            $(this).select();
        });
    },
    InitDetails: function ($container) {
        $container.find('div[data-element-id]').each(function () {
            switch ($(this).attr('data-input-type')) {
                case 'autocomplete':
                    UserRecord.InitDetailsAutoComplete($(this));
                    break;
                case 'checkbox':
                    UserRecord.InitDetailsCheckBox($(this));
                    break;
                case 'datepicker':
                    UserRecord.InitDetailsDatePicker($(this));
                    break;
                case 'datetimepicker':
                    UserRecord.InitDetailsDateTimePicker($(this));
                    break;
                case 'dropdownlist':
                    UserRecord.InitDetailsDropDownList($(this));
                    break;
                case 'numerictextbox':
                    UserRecord.InitDetailsNumericTextBox($(this));
                    break;
                case 'radiobuttonlist':
                    UserRecord.InitDetailsRadioButtonList($(this));
                    break;
                case 'textbox':
                    UserRecord.InitDetailsTextBox($(this));
                    break;
            }
        });

        $container.find('input[type=text]').on('click', function () {
            $(this).select();
        });
    },
    InitDetailsTextBox: function ($container) {
        console.log('textbox');
    },
    InitDetailsAutoComplete: function ($container) {
        console.log('autocomplete');
        var $el = $container.find('input[type=text]');
        var tableName = $el.attr('data-autocomplete-source');
        $el.autocomplete({
            minLength: 4,
            autoFocus: true,
            delay: 300,
            select: function (e, i) {
                $el.attr('data-lookup-id', i.item.id);
            },
            source: function (request, response) {
                var _response = response;
                var url = '/Records/LookupAutocomplete';
                var obj = { tableName: $el.attr('data-autocomplete-source'), searchText: $el.val() };
                $.ajax({
                    url: '/Records/LookupAutocomplete',
                    type: 'POST',
                    data: JSON.stringify(obj),
                    async: true,
                    cache: false,
                    //dataType: 'application/json',
                    contentType: 'application/json',
                    success: function success(results) {
                        var output = [];
                        for (var x = 0; x < results.length; x++) {
                            output.push(results[x]);

                        }
                        _response(output);
                    },
                    error: function error(err) {
                        console.log(err);
                        alert('error');
                    }
                });
            }
        });
    },
    InitDetailsCheckBox: function ($container) {
        console.log('checkbox');
    },
    InitDetailsDatePicker: function ($container) {
        console.log('datepicker');
        $container.find('input').datepicker();
        if ($container.find('input[type=text]').val() !== '') {
            $container.find('input[type=text]').datepicker("setDate", $container.find('input[type=text]').val());
        }

    },
    InitDetailsDateTimePicker: function ($container) {

        $container.find('input[type=text]').datepicker();
        if ($container.find('input[type=text]').val() !== '') {
            $container.find('input[type=text]').datepicker("setDate", $container.find('input[type=text]').val());
        }

    },
    InitDetailsDropDownList: function ($container) {
        console.log('dropdownlist');
    },
    InitDetailsNumericTextBox: function ($container) {
        console.log('numerictextbox');
    },
    InitDetailsRadioButtonList: function ($container) {
        console.log('radiobuttonlist');
    }

};