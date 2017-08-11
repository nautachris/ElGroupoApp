$(document).ready(function () {

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

    $("#tblContacts").on("click", "a", function () {
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
                    $("#divContactList").html(results);
                },
                error: function error(err) {
                    alert('fuck me');
                }
            });
        }
    });

    $("#inputNewPhoto").on("change", function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $("#imgNewPhoto").attr('src', e.target.result);
                $("#divNewPhoto").show();
            }
            reader.readAsDataURL(this.files[0]);
        }
        else {
            $("#divNewPhoto").hide();
        }

    });

});