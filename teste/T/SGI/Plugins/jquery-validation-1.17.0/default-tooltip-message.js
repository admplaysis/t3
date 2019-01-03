$.validator.setDefaults({
    errorClass: "has-error",
    validClass: "has-success",
    errorElement: "span",
    errorPlacement: function (error, element) {
        // Add the `help-block` class to the error element
        //error.addClass("help-block");
        //error.insertAfter(element);
        element.tooltip({
            title: error.text()
        });
    },
    highlight: function (element, errorClass, validClass) {
        $(element).parents(".form-group").addClass(errorClass).removeClass(validClass);
        $(element).tooltip('destroy');
    },
    unhighlight: function (element, errorClass, validClass) {
        $(element).parents(".form-group").addClass(validClass).removeClass(errorClass);
        $(element).tooltip('destroy');
    }
});