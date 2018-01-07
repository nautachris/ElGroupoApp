var Confirm = function (message, callbackYes, callbackNo) {
    var cbYes = callbackYes;
    var cbNo = callbackNo;
    //$("#main").hide();
    $("#main").css('opacity', 0.2);
    $("#divConfirm").show();
    $("#spanConfirmationMessage").text(message);
    $("#btnConfirmYes").on("click", function () {
        $("#main").css('opacity', 1);
        $("#divConfirm").hide();
        $("#btnConfirmYes").off("click");
        $("#btnConfirmNo").off("click");
        if (cbYes) cbYes();
    });
    $("#btnConfirmNo").on("click", function () {

        //$("#main").show();
        $("#main").css('opacity', 1);
        $("#divConfirm").hide();
        $("#btnConfirmYes").off("click");
        $("#btnConfirmNo").off("click");
        if (cbNo) cbNo();
    });
}

var MessageDialog = function (message) {
    $("#divConfirm").hide();
    $("#main").css('opacity', 0.2);
    $("#divMessageDialog").show();
    $("#spanMessageDialog").text(message);
    $("#btnMessageDialogOK").on("click", function () {
        $("#main").css('opacity', 1);
        $("#divMessageDialog").hide();
        $("#btnMessageDialogOK").off("click");
    });
};

var ConfirmRecurrence = function (message, callbackYesRecurrence, callbackNoRecurrence) {
    var cbYes = callbackYesRecurrence;
    var cbNo = callbackNoRecurrence;
    $("#main").hide();
    $("#divConfirmRecurrence").show();
    $("#spanConfirmationMessageRecurrence").text(message);
    $("#btnConfirmRecurrenceYes").on("click", function () {
        $("#divConfirmRecurrence").hide();
        $("#main").show();
        $("#btnConfirmRecurrenceYes").off("click");
        $("#btnConfirmRecurrenceNo").off("click");
        $("#btnConfirmRecurrenceCancel").off("click");
        if (cbYes) cbYes();
    });
    $("#btnConfirmRecurrenceNo").on("click", function () {

        $("#main").show();
        $("#divConfirmRecurrence").hide();
        $("#btnConfirmRecurrenceYes").off("click");
        $("#btnConfirmRecurrenceNo").off("click");
        $("#btnConfirmRecurrenceCancel").off("click");
        if (cbNo) cbNo();
    });
    $("#btnConfirmRecurrenceCancel").on("click", function () {

        $("#main").show();
        $("#divConfirmRecurrence").hide();
        $("#btnConfirmRecurrenceYes").off("click");
        $("#btnConfirmRecurrenceNo").off("click");
        $("#btnConfirmRecurrenceCancel").off("click");
    });
}