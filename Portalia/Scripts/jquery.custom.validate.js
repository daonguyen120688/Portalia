$.validator.addMethod("comparedate", function (value, element, params) {
    var curElement = $(element);
    var refElementValue = $('#' + curElement.attr('data-val-comparedate-refproperty')).val();
    if (value == null || value === "" || refElementValue === "")
        return true;

    var bits = value.match(/([0-9]+)/gi);
    var bit1s = refElementValue.match(/([0-9]+)/gi);

    var elementValue = new Date(parseInt(bits[2]), parseInt(bits[1])-1, parseInt(bits[0]), 0, 0, 0, 0);
    var refValue = new Date(parseInt(bit1s[2]), parseInt(bit1s[1])-1, parseInt(bit1s[0]), 0, 0, 0, 0);
    var operator = parseInt(curElement.attr('data-val-comparedate-operator'));
    var isTrue = true;

    switch (operator) {
        //Greater than
        case 1:
            isTrue= elementValue >= refValue;
            break;
        //Less than
        case 2:
            isTrue = elementValue <= refValue;
            break;
        case 3:
        default:
            isTrue = elementValue === refValue;
            break;
    }

    return isTrue;
},"Doit être supérieure à la date de début du contrat.");

$.validator.unobtrusive.adapters.addBool("comparedate");

$.validator.addMethod("requireiftrue", function (value, element, params) {
    var curElement = $(element);

    var refElementValue = $('#' + curElement.attr('data-val-requireiftrue-flagproperty')).val();

    if ((value == null || value === "") && refElementValue === "True")
        return false;
    return true;
}, "Ce champ est requis");
$.validator.unobtrusive.adapters.addBool("requireiftrue");

$.validator.addMethod(
    "date",
    function (value, element) {
        var bits = value.match(/([0-9]+)/gi);
        if (!bits)
            return this.optional(element) || false;
        //str = bits[1] + '/' + bits[0] + '/' + bits[2];
        return this.optional(element) || !/Invalid|NaN/.test(new Date(parseInt(bits[2]), parseInt(bits[1]), parseInt(bits[0]),0,0,0,0));
    },
    ""
);