$(document).load(function () {
    GoogleContacts.$SignInButton = $("#authorize-button");
    GoogleContacts.$SignOutButton = $("#signout-button");

    GoogleContacts.$SignInButton.on("click", GoogleContacts.HandleAuthClick);
    GoogleContacts.$SignOutButton.on("click", GoogleContacts.HandleSignoutClick);
});


var GoogleContacts = {
    $SignInButton: $("#authorize-button"),
    $SignOutButton: $("#signout-button"),
    ApiKey: null,
    ClientSecret: null,
    DISCOVERY_DOCS :["https://www.googleapis.com/discovery/v1/apis/people/v1/rest"],
    SCOPES : "https://www.googleapis.com/auth/contacts.readonly",
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

            GoogleContacts.$SignInButton.on("click", GoogleContacts.HandleAuthClick);
            GoogleContacts.$SignOutButton.on("click", GoogleContacts.HandleSignoutClick);
            // Listen for sign-in state changes.
            gapi.auth2.getAuthInstance().isSignedIn.listen(GoogleContacts.UpdateSigninStatus);

            // Handle the initial sign-in state.
            GoogleContacts.UpdateSigninStatus(gapi.auth2.getAuthInstance().isSignedIn.get());

        });
    },
    UpdateSigninStatus: function (signedIn) {
        if (signedIn) {
            GoogleContacts.$SignOutButton.show();
            GoogleContacts.$SignInButton.hide();
            GoogleContacts.LoadConnections();
        }
        else {
            GoogleContacts.$SignOutButton.hide();
            GoogleContacts.$SignInButton.show();
        }

    },
    HandleAuthClick: function (event) {
        gapi.auth2.getAuthInstance().signIn();
    },
    HandleSignoutClick: function (event) {
        gapi.auth2.getAuthInstance().signOut();
    },
    AppendConnection: function (text) {
        var pre = document.getElementById('content');
        var textContent = document.createTextNode(text + '\n');
        pre.appendChild(textContent);
    },
    LoadConnections: function() {
        gapi.client.people.people.connections.list({
            'resourceName': 'people/me',
            'pageSize': 50,
            'personFields': 'names,emailAddresses',
        }).then(function (response) {
            var connections = response.result.connections;
            GoogleContacts.AppendConnection('Connections:');

            if (connections.length > 0) {
                for (i = 0; i < connections.length; i++) {
                    console.log(connections[i]);
                    var person = connections[i];
                    if (person.names && person.names.length > 0) {
                        GoogleContacts.AppendConnection(person.names[0].displayName)
                    } else {
                        GoogleContacts.AppendConnection("No display name found for connection.");
                    }
                }
            } else {
                GoogleContacts.AppendConnection('No upcoming events found.');
            }
        });

    }


}

