$("form").validate({
    rules: {
        confirmPassword: {
            equalTo: "#Password"
        }
    },
    messages:{
        equalTo: "Please enter the same password as above"
    } 
});
