"use strict";
///<reference path="../../../../node_modules/@types/jquery/index.d.ts"/>
Object.defineProperty(exports, "__esModule", { value: true });
$(document).ready(function () {
    Bears.Init();
    var p = 5;
    var g = 5;
    var ffff = 5;
});
var Bears = /** @class */ (function () {
    function Bears() {
    }
    Bears.Init = function () {
        $('.log-in').on('click', Bears.EventHandlers.LoginClicked);
    };
    Bears.EventHandlers = {
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
    return Bears;
}());
exports.Bears = Bears;
//# sourceMappingURL=file1.js.map