$(document).ready(function () {

    $("#btnTerms").on("click", function () {
        $("form > div").hide();
        $("form > div.terms-of-service").show();
    });
    $("#btnBack").on("click", function () {
        $("form > div").show();
        $("form > div.terms-of-service").hide();
    });

    $("#inputPhoto").on("change", function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                console.log(e.target.result);
                if (e.target.result) {
                    $("#imgPhoto").attr('src', e.target.result);
                    $("div.img-photo").show();
                }
                else {
                    $("div.img-photo").hide();
                }

            }
            reader.readAsDataURL(this.files[0]);
        }
        else {
            $("div.img-photo").hide();
        }

    });

});