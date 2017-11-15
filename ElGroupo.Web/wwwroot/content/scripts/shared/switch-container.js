$(document).ready(function () {

    SwitchContainer.bindClick();
    SwitchContainer.init();

    SwitchContainerMulti.bindClick();
    SwitchContainerMulti.init();
   



});

SwitchContainer = {
    bindClick: function () {
        $("html").on("click", ".switch-container > span", function () {

            //event-status



            var $parent = $(this).closest("div.switch-container");
            if ($parent.hasClass('ignore-switch')) return false;

            console.log('switch container click');

            $parent.find(".switch-selected").removeClass("switch-selected");
            $(this).addClass("switch-selected");

            if ($parent.attr('data-replace-element') && $(this).attr('data-replace-val')) {
                //this sets the model-bound hidden to what the user has clicked
                console.log('replace element: ' + $parent.attr('data-replace-element'));
                console.log('replace val: ' + $(this).attr('data-replace-val'));
                var $replaceEl = $("#" + $parent.attr('data-replace-element'));
                if ($replaceEl.attr('type') === 'checkbox') {
                    if ($(this).attr('data-replace-val') == 'true') {
                        $replaceEl.prop('checked', true);
                    }
                    else {
                        $replaceEl.prop('checked', false);
                    }
                }
                else {
                    $replaceEl.val($(this).attr('data-replace-val'));
                }
            }


        });
        $("html").on("click", ".switch-container > div", function (evt) {
            console.log(evt.defaultPrevented);
            console.log('switch container click');
            var $parent = $(this).closest("div.switch-container");
            $parent.find(".switch-selected").removeClass("switch-selected");
            $(this).addClass("switch-selected");


            if ($parent.attr('data-replace-element') && $(this).attr('data-replace-val')) {
                //this sets the model-bound hidden to what the user has clicked
                console.log('replace element: ' + $parent.attr('data-replace-element'));
                console.log('replace val: ' + $(this).attr('data-replace-val'));
                var $replaceEl = $("#" + $parent.attr('data-replace-element'));
                if ($replaceEl.attr('type') === 'checkbox') {
                    if ($(this).attr('data-replace-val') == 'true') {
                        $replaceEl.prop('checked', true);
                    }
                    else {
                        $replaceEl.prop('checked', false);
                    }
                }
                else {
                    $replaceEl.val($(this).attr('data-replace-val'));
                }
            }
        });
    },
    init: function (selector) {

        var sel = "div.switch-container[data-replace-element]";
        if (selector) sel = selector + " " + sel;

        $(sel).each(function () {
            //this sets the switch container UI based on the model data
            console.log('init replace element: ' + $(this).attr('data-replace-element'));
            var replaceElId = $(this).attr('data-replace-element');
            var $replaceEl = $("#" + replaceElId);
            console.log($replaceEl);
            $(this).children('span').removeClass('switch-selected');
            //if checkbox
            if ($replaceEl.attr('type') === 'checkbox') {
                console.log('replace elelment is checkbox');
                if ($replaceEl.is(':checked')) {
                    $(this).find('span[data-replace-val=true]').addClass('switch-selected');
                    $(this).find('span[data-replace-val=' + val + ']').click();
                }
                else {
                    $(this).find('span[data-replace-val=false]').addClass('switch-selected');
                    $(this).find('span[data-replace-val=' + val + ']').click();
                }
            }
            else {

                var val = $replaceEl.val();

                if (val) {
                    $(this).find('span[data-replace-val=' + val + ']').addClass('switch-selected');
                    $(this).find('span[data-replace-val=' + val + ']').click();
                }

            }

        });
    }

};

SwitchContainerMulti = {
    bindClick: function () {
        $("html").on("click", ".switch-container-multi > span", function () {

            if ($(this).hasClass('switch-selected')) $(this).removeClass('switch-selected');
            else $(this).addClass('switch-selected');


        });
        $("html").on("click", ".switch-container-multi > div", function () {

            if ($(this).hasClass('switch-selected')) $(this).removeClass('switch-selected');
            else $(this).addClass('switch-selected');
        });
    },
    init: function (selector) {


    }

};