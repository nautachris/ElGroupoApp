$(document).load(function () {
    GoogleContacts.Init();
});

var GoogleContacts = {
    Init: function () {
        GoogleContacts.$SignInButton = $("#btnImportGoogleApi");
        //GoogleContacts.$SignOutButton = $("#signout-button");
    },
    UpdateTableCallback: null,
    $SignInButton: null,
    $SignOutButton: null,
    ApiKey: null,
    ClientSecret: null,
    DISCOVERY_DOCS: ["https://www.googleapis.com/discovery/v1/apis/people/v1/rest"],
    SCOPES: "https://www.googleapis.com/auth/contacts.readonly",
    HandleClientLoad: function (apiKey, clientSecret) {
        GoogleContacts.$SignInButton = $("#btnImportGoogleApi");
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