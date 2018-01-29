$(document).ready(function () {
    Dashboard.Init();

});
Dashboard = {
    Init: function () {
        $("div [data-event-link]").on("click", function () {
            
            var url = $(this).attr('data-event-link');
            console.log('data-event-link click' + url);
            window.location.href = url;
        });

        $("a.quick-response").on("click", function (evt) {
            evt.stopPropagation();
            console.log('quick response clicked');
        });
    },
    EventHandlers: {

    }
};