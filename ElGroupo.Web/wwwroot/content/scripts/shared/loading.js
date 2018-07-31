$(document).ajaxStart(function () {
    Loading.Start();

});
$(document).ajaxComplete(function () {
    Loading.Stop();
});

$(document).ready(function () {
    $('#btnStopLoad').on('click', function () {
        Loading.Stop();
    });
    $('#btnStartLoad').on('click', function () {
        Loading.Start();
    });
});


var Loading = {
    Start: function () {
        $("#divSpinner").show();
        //$("#fountainG div").addClass('fountainG');
        $('#divSpinLogo').addClass('loader');
        $("#main").css('opacity', 0.5);

    },
    Stop: function () {
        $("#divSpinner").hide();
        //$("#fountainG div").removeClass('fountainG');
        $('#divSpinLogo').removeClass('loader');
        $("#main").css('opacity', 1.0);
    }
};