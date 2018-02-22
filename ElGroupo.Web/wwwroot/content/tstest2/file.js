"use strict";
///<reference path="../../../../node_modules/@types/jquery/index.d.ts"/>
Object.defineProperty(exports, "__esModule", { value: true });
$(document).ready(function () {
    Login.Init();
    var p = 5;
    var g = 5;
});
var Login = /** @class */ (function () {
    function Login() {
    }
    Login.Init = function () {
        $('.log-in').on('click', Login.EventHandlers.LoginClicked);
    };
    Login.EventHandlers = {
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
    };
    return Login;
}());
exports.Login = Login;
//# sourceMappingURL=file.js.map