$(document).ready(function () {
    Login.Init();

});

Login = {
    Init: function () {
        $(".log-in").on("click", Login.EventHandlers.LoginClicked);
        $(".sign-up").on("click", Login.EventHandlers.SignUpClicked);
    },
    EventHandlers: {
        LoginClicked: function () {
            $(".sign-up").removeClass("bold");
            $(".log-in").addClass("bold");
            $("#divSignUp").hide();
            $("#divLogin").show();
        },
        SignUpClicked: function () {
            $(".sign-up").addClass("bold");
            $(".log-in").removeClass("bold");
            $("#divSignUp").show();
            $("#divLogin").hide();
        }
    }
};