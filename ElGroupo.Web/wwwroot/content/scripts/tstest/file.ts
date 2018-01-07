///<reference path="../../../../node_modules/@types/jquery/index.d.ts"/>

$(document).ready(() => {
    Login.Init();
    let p = 5;
    let g = 5;
});

export class Login {
    static Init() {
        $('.log-in').on('click', Login.EventHandlers.LoginClicked);

    }

    static EventHandlers = {
        LoginClicked() {
            $(".sign-up").removeClass("bold");
            $(".log-in").addClass("bold");
            $("#divSignUp").hide();
            $("#divLogin").show();
        },
        SignUpClicked() {
            $(".sign-up").addClass("bold");
            $(".log-in").removeClass("bold");
            $("#divSignUp").show();
            $("#divLogin").hide();
        }

    };

}