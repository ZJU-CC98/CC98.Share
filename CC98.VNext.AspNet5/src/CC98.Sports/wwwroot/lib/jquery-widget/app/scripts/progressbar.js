'use strict';

var app = angular.module('progressbar', ['jqueryWidget']);

// Configuration for the jQueryUI progressbar widget
app.factory('progressbarConfig', function ($log) {

    this.widget = 'progressbar';

    this.default = 'value';

    this.attributes = ['disabled', 'max', 'value'];

    this.options = { max: 100, value: 50 };

    this.events = {
        change: function (event, ui) {
            $log.info('Event: change', event, ui);
        },
        create: function (event, ui) {
            $log.info('Event: create', event, ui);
        },
        complete: function (event, ui) {
            $log.info('Event: slide', event, ui);
        }
    };

    this.binds = [
        { change: 'value' }
    ];

    this.render = function(viewValue, ngModel) {
        for (var key in viewValue) {
            if (viewValue.hasOwnProperty(key)) {
                if (viewValue[key] === '' || viewValue[key] === '0') {
                    if (key === 'step') {
                        // special case: step can't be empty or 0
                        viewValue[key] = '1';
                    }
                } else {
                    // slider wants numbers not strings
                    viewValue[key] = parseValue(viewValue[key]);
                }
            }
        }
    };

    return this;
});
