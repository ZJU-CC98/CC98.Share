var parseValue = function (value) {
    var retVal;

    // Is it a boolean?
    if (isNaN(value) || value === '') {
        if (typeof(value) !== 'object') {
            if (value.toLowerCase() === 'true') {
                retVal = true;
            } else if (value.toLowerCase() === 'false') {
                retVal = false;
            } else {
                retVal = value;
            }
        } else {
            retVal = '';
        }
    } else {
        // Is a boolean or number
        retVal = (typeof(value) === 'boolean') ? value : parseInt(value);
    }

    return retVal;
};