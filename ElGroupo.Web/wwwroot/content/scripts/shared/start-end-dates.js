


StartEndDates = {
    $startHour: null,
    $startDate: null,
    $startMin: null,
    $startAmPm: null,
    $endHour: null,
    $endDate: null,
    $endMin: null,
    $endAmPm: null,
    GetStartDate: function () {
        console.log('getstartdate');
        var date = this.$startDate.datepicker('getDate');
        if (date === null) return null;
        console.log(date);
        var hour = Number(this.$startHour.val());
        var min = Number(this.$startMin.val());
        if (hour === 12 && this.$startAmPm.val() === 'AM') hour = 0;
        else if (this.$startAmPm.val() === 'PM' && hour !== 12) hour += 12;
        date.setHours(hour, min);
        return date;
    },
    GetEndDate: function () {

        var date = this.$endDate.datepicker('getDate');
        var hour = Number(this.$endHour.val());
        var min = Number(this.$endMin.val());
        if (hour === 12 && this.$endAmPm.val() === 'AM') hour = 0;
        else if (this.$endAmPm.val() === 'PM' && hour !== 12) hour += 12;
        date.setHours(hour, min);
        return date;
    },
    SyncDates: function (changed) {
        console.log('syncdates - ' + changed + ' was changed');
        var startDate = StartEndDates.GetStartDate();
        var endDate = StartEndDates.GetEndDate();

        if (changed === 'start') {
            //update end 
            if (endDate < startDate) StartEndDates.SetEndDate(startDate);
        }
        else {
            if (endDate < startDate) StartEndDates.SetStartDate(endDate);
        }


    },
    SetEndDate: function (date) {
        StartEndDates.$endDate.datepicker('setDate', date);
        StartEndDates.$endMin.val(date.getMinutes());
        var hour = date.getHours();
        if (hour === 0) {
            StartEndDates.$endAmPm.val('AM');
            StartEndDates.$endHour.val(12);
        }
        else if (hour > 0 && hour <= 11) {
            StartEndDates.$endAmPm.val('AM');
            StartEndDates.$endHour.val(hour);
        }
        else if (hour === 12) {
            StartEndDates.$endAmPm.val('PM');
            StartEndDates.$endHour.val(hour);
        }
        else {
            StartEndDates.$endAmPm.val('PM');
            StartEndDates.$endHour.val(hour - 12);
        }
    },
    SetStartDate: function (date) {
        StartEndDates.$startDate.datepicker('setDate', date);
        StartEndDates.$startMin.val(date.getMinutes());
        var hour = date.getHours();
        if (hour === 0) {
            StartEndDates.$startAmPm.val('AM');
            StartEndDates.$startHour.val(12);
        }
        else if (hour > 0 && hour <= 11) {
            StartEndDates.$startAmPm.val('AM');
            StartEndDates.$startHour.val(hour);
        }
        else if (hour === 12) {
            StartEndDates.$startAmPm.val('PM');
            StartEndDates.$startHour.val(hour);
        }
        else {
            StartEndDates.$startAmPm.val('PM');
            StartEndDates.$startHour.val(hour - 12);
        }

    },

    Init: function (container) {
        this.$startDate = $(container + ' input[data-start-date]');
        this.$startHour = $(container + ' select[data-start-hour]');
        this.$startMin = $(container + ' select[data-start-min]');
        this.$startAmPm = $(container + ' select[data-start-am-pm]');
        this.$endDate = $(container + ' input[data-end-date]');
        this.$endHour = $(container + ' select[data-end-hour]');
        this.$endMin = $(container + ' select[data-end-min]');
        this.$endAmPm = $(container + ' select[data-end-am-pm]');


        this.$startDate.on('change', function () {
            console.log('start date changed');
            StartEndDates.SyncDates('start')

        });
        this.$startHour.on('change', function () {
            StartEndDates.SyncDates('start')

        });
        this.$startMin.on('change', function () {
            StartEndDates.SyncDates('start')
        });
        this.$startAmPm.on('change', function () {
            StartEndDates.SyncDates('start')

        });
        this.$endDate.on('change', function () {
            StartEndDates.SyncDates('end')

        });
        this.$endHour.on('change', function () {
            StartEndDates.SyncDates('end')

        });
        this.$endMin.on('change', function () {
            StartEndDates.SyncDates('end')
        });
        this.$endAmPm.on('change', function () {
            StartEndDates.SyncDates('end')
        });
    }

}