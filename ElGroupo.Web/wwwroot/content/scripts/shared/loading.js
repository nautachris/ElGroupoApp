$(document).ajaxStart(function () {
    Loading.Start();

});
$(document).ajaxComplete(function () {
    Loading.Stop();
});


var Loading = {
    Start: function () {
        $("#divSpinner").show();
        $("#fountainG div").addClass('fountainG');
        $("#main").css('opacity', 0.5);

    },
    Stop: function () {
        $("#divSpinner").hide();
        $("#fountainG div").removeClass('fountainG');
        $("#main").css('opacity', 1.0);
    }
};