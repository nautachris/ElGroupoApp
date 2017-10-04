$(document).ready(function () {


    GoogleContacts.UpdateTableCallback = updateImportTable;

    $(".links").on("click", function () {
        $(".links").removeClass('bold');
        $(this).addClass('bold');
        $(".row.tab").hide();
        $(".row." + $(this).attr('data-link-type')).show();

    });

    var originalImageUrl = $("#divImg").css('background-image');
    $("#btnAddNewContact").on("click", function () {
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
                alert('fuck me');
            }
        });


    });

    $("#divContactList").on("click", " #tblContacts a", function () {
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
                    alert('fuck me');
                    console.log(err);
                }
            });
        }
    });


    $("#divChangePhoto").on("click", function () {
        console.log('bears');
        $("#inputNewPhoto").click();
    });



    $("#inputNewPhoto").on("change", function () {
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
            $("#divImg").css('background-image', originalImageUrl);
        }

    });


    //connections
    $(".contact-search-rb").on("change", function () {
        if ($("#rbExistingContacts").is(':checked')) {
            $(".row.select-existing-contacts").show();
            $(".row.manual-search").hide();
        }
        else {
            $(".row.select-existing-contacts").hide();
            $(".row.manual-search").show();
        }
    });


    $("#tblImportList").on("click", "th a", function () {

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

    });
    $("#btnAddConnection").on('click', function () {
        if (!$("#txtSelectConnection").has('[data-contact-id]')) return;
        var cid = Number($("#txtSelectConnection").attr('data-contact-id'));
        //active user id
        var eid = Number($("#Id").val());
        var obj = { userId: cid, eventId: eid };

        $.ajax({
            url: "/Account/AddConnection/" + eid.toString() + '/Add/' + cid.toString(),
            type: 'POST',
            async: true,
            cache: false,
            dataType: "html",
            success: function success(results) {
                $("#divConnections").html(results);
                $("#txtSelectConnection").val('');
                $("#txtSelectConnection").removeAttr('data-contact-id')
            },
            error: function error(err) {
                alert('fuck me');
            }
        });
    });
    $("#txtSelectConnection").on("input", function () {
        if ($(this).val() === '') {
            $(this).removeAttr('data-contact-id');
            $("#txtSelectConnection").attr('disabled', true);
        }
    });
    $("#txtSelectConnection").autocomplete({
        minLength: 3,
        autoFocus: true,
        delay: 300,
        select: function (e, i) {
            $("#txtSelectConnection").attr('data-contact-id', i.item.id);
            $("#btnAddConnection").attr('disabled', false);
        },
        source: function (request, response) {
            var userId = $("#Id").val();
            var apiUrl = $("#rbContactsConnections").is(':checked') ? "/Users/SearchAutocompleteByUser/" + request.term : "/Users/SearchAutocomplete/" + request.term;
            var _response = response;
            $.ajax({
                url: apiUrl,
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
                    alert('fuck me');
                }
            });
        }
    });



    var importButtonClicked = null;

    $("#btnImportOutlook").on("click", function () {
        importButtonClicked = 'outlook';
        $("#fileInput").val('');
        $("#fileInput").click();
    });
    $("#btnImportGoogle").on("click", function () {
        importButtonClicked = 'google';
        $("#fileInput").val('');
        $("#fileInput").click();

    });
    $("#btnImportGoogleApi").on("click", function () {
    });
    $("#btnCancelImport").on("click", function () {
        $("#divConnectionList").show();
        $("#divAddConnections").show();
        $("#divImportResults").hide();

    });


    function updateImportTable(contacts) {
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
    }

    $("#fileInput").on("change", function (event) {
        if (!event.target.files || event.target.files.length === 0) return false;

        var fd = new FormData();
        console.log(importButtonClicked);
        console.log(event.target.files[0]);
        fd.append('file', event.target.files[0]);
        fd.append('format', importButtonClicked);
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
                alert('fuck me');
            }
        });

    });


    $("#btnImportSelectedContacts").on("click", function () {
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
                alert('fuck me');
            }
        });
    });

});

var GoogleContacts = {

    UpdateTableCallback: null,
    $SignInButton: $("#btnImportGoogleApi"),
    $SignOutButton: $("#signout-button"),
    ApiKey: null,
    ClientSecret: null,
    DISCOVERY_DOCS: ["https://www.googleapis.com/discovery/v1/apis/people/v1/rest"],
    SCOPES: "https://www.googleapis.com/auth/contacts.readonly",
    HandleClientLoad: function (apiKey, clientSecret) {
        GoogleContacts.ApiKey = apiKey;
        GoogleContacts.ClientSecret = clientSecret;
        gapi.load('client:auth2', GoogleContacts.InitClient);

    },
    InitClient() {
        gapi.client.init({
            apiKey: GoogleContacts.ApiKey,
            clientId: GoogleContacts.ClientSecret,
            discoveryDocs: GoogleContacts.DISCOVERY_DOCS,
            scope: GoogleContacts.SCOPES
        }).then(function () {

            gapi.auth2.getAuthInstance().signOut();
            GoogleContacts.$SignInButton.on("click", GoogleContacts.HandleAuthClick);
            //GoogleContacts.$SignOutButton.on("click", GoogleContacts.HandleSignoutClick);
            // Listen for sign-in state changes.
            gapi.auth2.getAuthInstance().isSignedIn.listen(function (signedIn) {
                console.log('signed in: ' + signedIn);
                if (signedIn) {
                    GoogleContacts.LoadConnections();
                }
            });
            // Handle the initial sign-in state.
            //GoogleContacts.UpdateSigninStatus(gapi.auth2.getAuthInstance().isSignedIn.get());

        });
    },
    UpdateSigninStatus: function (signedIn) {
        console.log('in updatesigninstatus ' + signedIn);
        if (signedIn) {
            //GoogleContacts.$SignOutButton.show();
            //GoogleContacts.$SignInButton.hide();
            GoogleContacts.LoadConnections();
        }
        else {
            //GoogleContacts.$SignOutButton.hide();
            //GoogleContacts.$SignInButton.show();
        }

    },
    HandleAuthClick: function (event) {
        console.log('handleauthclick');
        if (gapi.auth2.getAuthInstance().isSignedIn.get() === true) {
            console.log('signed in');
            GoogleContacts.LoadConnections();

            //GoogleContacts.LoadConnections();
        }
        else {
            gapi.auth2.getAuthInstance().signIn(function (evt) {
                console.log('signin callback A');
                console.log(evt);
            });



        }
        //gapi.auth2.getAuthInstance().isSignedIn.listen(GoogleContacts.UpdateSigninStatus);

    },
    HandleSignoutClick: function (event) {
        gapi.auth2.getAuthInstance().signOut();
    },
    AppendConnection: function (text) {
        var pre = document.getElementById('content');
        var textContent = document.createTextNode(text + '\n');
        pre.appendChild(textContent);
    },
    LoadConnections: function () {

        gapi.client.people.people.connections.list({
            'resourceName': 'people/me',
            'pageSize': 500,
            'personFields': 'names,emailAddresses,phoneNumbers',
        }).then(function (response) {
            var contacts = [];
            var connections = response.result.connections;

            for (i = 0; i < connections.length; i++) {
                var firstName = '';
                var lastName = '';
                var email = '';
                var phone1 = ''
                var phone2 = '';
                var person = connections[i];
                if (person.names && person.names.length > 0) {
                    firstName = person.names[0].givenName;
                    lastName = person.names[0].familyName;
                    //GoogleContacts.AppendConnection(person.names[0].displayName)
                }
                if (person.phoneNumbers && person.phoneNumbers.length === 1) {
                    phone1 = person.phoneNumbers[0].value;
                }
                if (person.phoneNumbers && person.phoneNumbers.length === 2) {
                    phone2 = person.phoneNumbers[1].value;
                }
                if (person.emailAddresses && person.emailAddresses.length === 1) {
                    email = person.emailAddresses[0].value;
                }

                var contact = {
                    firstName: firstName,
                    lastName: lastName,
                    email: email,
                    phone1: phone1,
                    phone2: phone2
                };
                contacts.push(contact);
            }
            console.log(contacts);
            GoogleContacts.UpdateTableCallback(contacts);

        });

    }


}