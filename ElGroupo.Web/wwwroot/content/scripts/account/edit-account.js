$(document).ready(function () {
    EditAccount.Init();
});

EditAccount = {
    Init: function () {
        GoogleContacts.UpdateTableCallback = EditAccount.UpdateImportTable;
        EditAccount.OriginalImageUrl = $("#divImg").css('background-image');
        EditAccount.InitConnectionAutocomplete();
        $("#divAddConnections div").on("click", ".switch-container > div", EditAccount.EventHandlers.SearchModeChanged);
        $("#btnAddNewContact").on("click", EditAccount.AddNewContactClicked);
        $("html").on("click", ".connection-links a", EditAccount.EventHandlers.ConnectionLinkClicked);
        $("html").on("click", "div.connection-info", EditAccount.EventHandlers.ConnectionDivClicked);
        $("#btnImportSelectedContacts").on("click", EditAccount.EventHandlers.ImportSelectedContactsClicked);
        $("#fileInput").on("change", EditAccount.EventHandlers.ImportContactsFileChanged);
        $("#btnCancelImport").on("click", EditAccount.EventHandlers.CancelImportClicked);
        $("#btnImportOutlook").on("click", EditAccount.EventHandlers.ImportOutlookClicked);
        $("#btnImportGoogle").on("click", EditAccount.EventHandlers.ImportGoogleClicked);
        $("#txtSelectConnection").on("input", EditAccount.EventHandlers.ConnectionTextChanged);
        $("#btnAddConnection").on('click', EditAccount.EventHandlers.AddConnectionClicked);
        $("#tblImportList").on("click", "th a", EditAccount.EventHandlers.ImportListCheckedChanged);
        $("#inputNewPhoto").on("change", EditAccount.EventHandlers.PhotoInputChanged);
        $("#divChangePhoto").on("click", EditAccount.EventHandlers.ChangePhotoClicked);
        $("#divContactList").on("click", " #tblContacts a", EditAccount.EventHandlers.ContactLinkClicked);
    },
    EventHandlers: {
        SearchModeChanged: function () {
            if ($(this).attr('data-action') === 'manual') {
                console.log('hiding autocomplete');
                $(".row.select-existing-contacts").hide();
                $(".row.manual-search").show();
            }
            else {
                console.log('showing autocomplete');
                $(".row.select-existing-contacts").show();
                $(".row.manual-search").hide();
            }
        },
        AddNewContactClicked: function () {
            if ($("#selContactType").val() === '') return false;
            if ($("#txtContactValue").val() === '') return false;


            var model = {
                ContactTypeId: Number($("#selContactType").val()),
                Value: $("#txtContactValue").val()
            };

            $.ajax({
                url: "/Account/Contacts/Create",
                type: 'POST',
                dataType: "html",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(model),
                async: true,
                cache: false,
                success: function success(results) {
                    $("#divContactList").html(results);
                },
                error: function error(err) {
                    alert('error');
                }
            });
        },
        ConnectionLinkClicked: function () {
            var $infoDiv = $(this).closest("div[data-user-id]").find("div.connection-info");
            var $this = $(this);
            if ($(this).attr('data-action') == 'profile') {
                //profile link
                window.open($(this).attr('data-profile-link'), '_blank');
            }
            else {
                var name = $(this).closest('div[data-user-id]').find('span.connected-user-name').text();
                Confirm("Do you want to remove " + name + " from your list of connections?", function () {
                    $.ajax({
                        url: "/Account/RemoveRegisteredConnection/" + $this.closest('div[data-user-id]').attr('data-user-id'),
                        type: 'POST',
                        async: true,
                        cache: false,
                        dataType: "html",
                        success: function success(results) {
                            $("#divConnectionList").html(results);
                        },
                        error: function error(err) {
                            alert('error');
                        }
                    });

                });
                //remove


            }
            $(this).closest("div.connection-links").hide();
            $infoDiv.css('opacity', 1);
        },
        ConnectionDivClicked: function () {
            $("div.connection-links").hide();
            $("div.connection-info").css('opacity', 1);
            var $links = $(this).closest("div[data-user-id]").find("div.connection-links");

            $(this).css('opacity', 0.5);
            $links.show();
        },
        ImportSelectedContactsClicked: function () {
            var selectCount = $("#tblImportList :checked").length;
            console.log('import selected click - select count: ' + selectCount.toString());
            if (selectCount === 0) return false;

            var contacts = [];

            $("#tblImportList :checked").each(function () {
                var lastName = $(this).closest('td').siblings('[data-type=lastname]').text();
                var firstName = $(this).closest('td').siblings('[data-type=firstname]').text();
                var email = $(this).closest('td').siblings('[data-type=email]').text();
                var phone1 = $(this).closest('td').siblings('[data-type=phone1]').text();
                var phone2 = $(this).closest('td').siblings('[data-type=phone2]').text();
                var c = {
                    lastName: lastName,
                    firstName: firstName,
                    email: email,
                    phone1: phone1,
                    phone2: phone2
                };
                contacts.push(c);
            });
            console.log(contacts);
            $.ajax({
                url: "/Account/ImportSelectedContacts",
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(contacts),
                async: true,
                cache: false,
                //dataType: "html",
                success: function success(results) {
                    var fff = 4;
                    console.log('import selected contacts results');
                    console.log(results);
                    $("#divImportResults").hide();
                    $("#divAddConnections").show();
                    $("#divConnectionList").empty().html(results).show();

                },
                error: function error(err) {
                    alert('error');
                }
            });
        },
        ImportContactsFileChanged: function () {
            if (!event.target.files || event.target.files.length === 0) return false;

            var fd = new FormData();
            console.log(EditAccount.ImportMode);
            console.log(event.target.files[0]);
            fd.append('file', event.target.files[0]);
            fd.append('format', EditAccount.ImportMode);
            console.log(fd.keys.length);
            console.log('posting this');
            console.log(fd);
            $.ajax({
                url: "/Account/LoadImportFile",
                type: 'POST',
                async: true,
                data: fd,
                contentType: false,
                processData: false,
                cache: false,
                success: function success(results) {

                    updateImportTable(results);

                },
                error: function error(err) {
                    alert('error');
                }
            });
        },
        CancelImportClicked: function () {
            $("#divConnectionList").show();
            $("#divAddConnections").show();
            $("#divImportResults").hide();
        },
        ImportGoogleClicked: function () {
            EditAccount.ImportMode = 'google';
            $("#fileInput").val('');
            $("#fileInput").click();
        },
        ImportOutlookClicked: function () {
            EditAccount.ImportMode = 'outlook';
            $("#fileInput").val('');
            $("#fileInput").click();
        },
        ConnectionTextChanged: function () {
            if ($(this).val() === '') {
                $(this).removeAttr('data-contact-id');
                $("#btnAddConnection").attr('disabled', true);
                $("#txtSelectConnection").removeClass('light-blue');
            }
        },
        AddConnectionClicked: function () {
            if (!$("#txtSelectConnection").has('[data-contact-id]')) return;
            var cid = Number($("#txtSelectConnection").attr('data-contact-id'));

            //check to see if the user already exsits
            if ($("#divConnectionList").find("div[data-user-id=" + cid + "]").length > 0) return;

            //active user id
            Loading.Start();
            $.ajax({
                url: "/Account/AddConnection/" + cid.toString(),
                type: 'POST',
                async: true,
                cache: false,
                dataType: "html",
                success: function success(results) {
                    Loading.Stop();
                    $("#divConnectionList").html(results);
                    $("#txtSelectConnection").val('');
                    $("#txtSelectConnection").removeAttr('data-contact-id');
                    $("#txtSelectConnection").removeClass('light-blue');
                },
                error: function error(err) {
                    Loading.Stop();
                    alert('error');
                }
            });
        },
        ImportListCheckedChanged: function () {
            var attr = $(this).attr('data-checked');

            var isChecked = attr !== undefined;

            $("#tblImportList tbody :checkbox").prop('checked', !isChecked);
            if (isChecked) {
                console.log('removing checked attr');
                $(this).removeAttr('data-checked');
            }
            else {
                console.log('adding data-checked');
                $(this).attr('data-checked', true);
            }
        },
        PhotoInputChanged: function () {
            if (this.files && this.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $("#divImg").css('background-image', 'url(' + e.target.result + ')');
                    //$("#imgNewPhoto").attr('src', e.target.result);
                    //$("#divNewPhoto").show();
                }
                reader.readAsDataURL(this.files[0]);
            }
            else {
                //$("#divNewPhoto").hide();
                $("#divImg").css('background-image', EditAccount.OriginalImageUrl);
            }
        },
        ChangePhotoClicked: function () {
            $("#inputNewPhoto").click();
        },
        ContactLinkClicked: function () {
            var id = Number($(this).attr('data-contact-id'));
            if ($(this).attr('data-action-type') === 'edit') {

            }
            else {
                $.ajax({
                    url: "/Account/Contacts/Delete/" + id,
                    type: 'DELETE',
                    dataType: "html",
                    async: true,
                    cache: false,
                    success: function success(results) {
                        console.log('delete success');
                        console.log(results);
                        $("#divContactList").html(results);
                    },
                    error: function error(err) {
                        alert('error');
                        console.log(err);
                    }
                });
            }
        }
    },
    ImportMode: null,
    OriginalImageUrl: null,
    UpdateImportTable: function (contacts) {
        $("#tblImportList tbody").empty();
        var tableHtml = '';
        for (var x = 0; x < contacts.length; x++) {
            var contact = contacts[x];
            if (contact.email !== null && contact.email !== '') {
                tableHtml += '<tr><td><input data-checked="true" type="checkbox" checked /></td>';
                tableHtml += '<td data-type="lastname">' + contact.lastName + '</td>';
                tableHtml += '<td data-type="firstname">' + contact.firstName + '</td>';
                tableHtml += '<td data-type="email">' + contact.email + '</td>';
                tableHtml += '<td data-type="phone1">' + contact.phone1 + '</td>';
                tableHtml += '<td data-type="phone2">' + contact.phone2 + '</td>';
                tableHtml += '</tr>';
            }


        }

        $("#tblImportList tbody").html(tableHtml);
        $("#divImportResults").show();
        $("#divConnectionList").hide();
        $("#divAddConnections").hide();
    },
    InitConnectionAutocomplete: function () {
        $("#txtSelectConnection").autocomplete({
            minLength: 3,
            autoFocus: true,
            delay: 300,
            select: function (e, i) {
                $("#txtSelectConnection").attr('data-contact-id', i.item.id);
                $("#txtSelectConnection").addClass('light-blue');
                $("#btnAddConnection").attr('disabled', false);
            },
            source: function (request, response) {
                //var userId = $("#Id").val();
                //var apiUrl = $("#rbContactsConnections").is(':checked') ? "/Users/SearchAllUsers/" + request.term : "/Users/SearchAutocomplete/" + request.term;
                var _response = response;
                $.ajax({
                    url: "/Users/SearchAllUsers/" + request.term,
                    type: 'GET',
                    //dataType: "application/json; charset=utf-8",
                    async: true,
                    cache: false,
                    success: function success(results) {
                        var output = [];
                        for (var x = 0; x < results.length; x++) {
                            var item = {
                                value: results[x].name,
                                label: results[x].name + ' (' + results[x].email + ')',
                                id: results[x].id
                            };
                            output.push(item);
                        }
                        _response(output);
                    },
                    error: function error(err) {
                        alert('error');
                    }
                });
            }
        });
    }
};

