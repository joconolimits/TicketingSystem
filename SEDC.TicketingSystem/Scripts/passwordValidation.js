$(function () {
    $("#passForm").validate({
        rules: {
            confirmPassword: {
                equalTo: "#passwordReg"
            }
        },
        messages: {
            equalTo: "Please enter the same password as above"
        }
    });

})