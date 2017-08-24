$(document).ready(function () {
    $(".sign-up").on("click", function () {
        $(".sign-up").addClass("bold");
        $(".log-in").removeClass("bold");
        $("#divSignUp").show();
        $("#divLogin").hide();
    });
    $(".log-in").on("click", function () {
        $(".sign-up").removeClass("bold");
        $(".log-in").addClass("bold");
        $("#divSignUp").hide();
        $("#divLogin").show();
    });

});