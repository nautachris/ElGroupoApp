﻿var Ajax = {
    Get: function (p1, p2, p3) {
        //p1 is url, p2 is datatype, p3 content type
        var url = null;
        var dataType = null;
        var contentType = null;
        if (typeof (p1) === 'object') {
            dataType = p1.hasOwnProperty('dataType') ? p1.dataType : 'html';
            contentType = p1.hasOwnProperty('contentType') ? p1.contentType : 'application/json';
            url = p1.url;
        }
        else {
            url = p1;
            dataType = (p2 !== undefined) ? p2 : 'html';
            contentType = (p3 !== undefined) ? p3 : 'html';
        }

        return $.ajax({
            url: url,
            type: 'GET',
            contentType: "application/json",
            async: true,
            cache: false,
            dataType: dataType,
            error: function error(err) {
                console.log('in ajax.js error handler');
                console.log(JSON.parse(err.responseText));
                //Loading.Stop();
                MessageDialog(err.message);
            }
        });
    },
    //Post: function (options, success) {
    //    console.log('in ajax post');
    //    var dataType = options.hasOwnProperty('dataType') ? options.dataType : 'html';
    //    var contentType = options.hasOwnProperty('contentType') ? options.contentType : 'application/json';
    //    var data = options.hasOwnProperty('data') ? options.data instanceof String ? options.data : JSON.stringify(options.data) : null;
    //    var _success = success;
    //    Loading.Start();
    //    $.ajax({
    //        url: options.url,
    //        type: 'POST',
    //        contentType: "application/json",
    //        async: true,
    //        cache: false,
    //        dataType: dataType,
    //        data: data,
    //        success: function success(results) {
    //            Loading.Stop();
    //            console.log('ajax.post calling callback');
    //            _success(results);
    //        },
    //        error: function error(err) {
    //            Loading.Stop();
    //            ErrorHandler.Show(err);
    //        }
    //    });
    //}
    Post: function (p1, p2, p3, p4) {
        console.log('in post');
        console.log(p1);
        console.log(p2);
        console.log(p3);
        console.log(p4);
        //p1 is url, p2 is data, p3 is datatype, p4 content type
        var url = null;
        var dataType = null;
        var contentType = null;
        var data = null;
        if (typeof (p1) === 'object') {
            dataType = p1.hasOwnProperty('dataType') ? p1.dataType : 'html';
            contentType = p1.hasOwnProperty('contentType') ? p1.contentType : 'application/json';
            data = p1.hasOwnProperty('data') ? typeof (p1.data) === 'string' ? p1.data : JSON.stringify(p1.data) : null;
            url = p1.url;
        }
        else {
            url = p1;
            data = typeof (p2) === 'string' ? JSON.stringify(p2) : p2;
            dataType = (p3 !== undefined) ? p3 : 'html';
            contentType = (p4 !== undefined) ? p4 : 'html';
        }
        console.log('url');
        console.log(url);
        console.log('data');
        console.log(data);
        console.log('datatype');
        console.log(dataType);
        console.log('contentType');
        console.log(contentType);
        return $.ajax({
            url: url,
            type: 'POST',
            contentType: "application/json",
            async: true,
            cache: false,
            dataType: dataType,
            data: data,
            error: function error(err) {
                console.log('in ajax.js error handler');
                console.log(JSON.parse(err.responseText));
                //Loading.Stop();
                MessageDialog(err.message);
            }
        });
    }

}; 