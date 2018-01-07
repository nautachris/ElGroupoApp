///<reference path="../../../../node_modules/@types/jquery/index.d.ts"/>

$(document).ready(() => {
    Bears.Init();
    let p = 5;
    let g = 5;
    let ffff = 5;
});

export class Bears {
    static Init() {
        $('.log-in').on('click', Bears.EventHandlers.LoginClicked);

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