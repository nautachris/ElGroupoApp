$(document).ready(function () {

    $(".links").on("click", function () {
        $(".links").removeClass('bold');
        $(this).addClass('bold');
        $(".row.tab").hide();
        $(".row." + $(this).attr('data-link-type')).show();

    });

    var originalImageUrl = $("#divImg").css('background-image');
    $("#btnAddNewContact").on("click", function () {
        if ($("#selContactType").val() === '') return false;
        if ($("#txtContactValue").val() === '') return false;


        var model = {
            ContactTypeId: Number($("#selContactType").val()),
            Value: $("#txtContactValue").val()
        };

        $.ajax({
            url: "/Account/Contacts/Create",
            type: 'POST',
            dataType: "html",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            async: true,
            cache: false,
            success: function success(results) {
                $("#divContactList").html(results);
            },
            error: function error(err) {
                alert('fuck me');
            }
        });


    });

    $("#divContactList").on("click", " #tblContacts a", function () {
        var id = Number($(this).attr('data-contact-id'));
        if ($(this).attr('data-action-type') === 'edit') {

        }
        else {
            $.ajax({
                url: "/Account/Contacts/Delete/" + id,
                type: 'DELETE',
                dataType: "html",
                async: true,
                cache: false,
                success: function success(results) {
                    console.log('delete success');
                    console.log(results);
                    $("#divContactList").html(results);
                },
                error: function error(err) {
                    alert('fuck me');
                    console.log(err);
                }
            });
        }
    });


    $("#divChangePhoto").on("click", function () {
        console.log('bears');
        $("#inputNewPhoto").click();
    });



    $("#inputNewPhoto").on("change", function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $("#divImg").css('background-image', 'url(' + e.target.result + ')');
                //$("#imgNewPhoto").attr('src', e.target.result);
                //$("#divNewPhoto").show();
            }
            reader.readAsDataURL(this.files[0]);
        }
        else {
            //$("#divNewPhoto").hide();
            $("#divImg").css('background-image', originalImageUrl);
        }

    });



});