//form tracking changes extension method.
//to use: call initTrackingChanges() with form jquery object exp: $("#myForm").initTrackingChanges();
//when save or submit form call clearTrackChanges() to reset all exp: $("#myForm").clearTrackChanges();
$.fn.extend({
    setCurrentVal : function ()
    {
        $(":input", this).each(function () {
            var input = $(this);
            input.data('current-val', input.val());
        });
    },
    initTrackingChanges: function () {
        this.setCurrentVal();
        formHasChanged = false;
        $(":input", this).change(function () {
            var input = $(this);
            if (input.val() !== input.data('current-val')) {
                formHasChanged = true;
            }
        });
        $(window).on("beforeunload", function () {
            if (formHasChanged) {
                return "Are you sure? You didn't finish the form!";
            }
        });
    },
    //call this method while submit form.
    clearTrackChanges: function () {
        formHasChanged = false;
        this.setCurrentVal();
    }
});