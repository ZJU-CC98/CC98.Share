'use strict';

var app = angular.module('slider', ['jqueryWidget']);

// Configuration for the jQueryUI slider widget

app.factory('sliderConfig', function ($log) {

    this.widget = 'slider';

    this.default = 'value';

    this.attributes = ['animate', 'disabled', 'max', 'min', 'orientation', 'range', 'step', 'value', 'values'];

    this.options = { min: 0, max: 10, step: 1 };

    this.events = {
        change: function (event, ui) {
            $log.info('Event: change', event, ui);
        },
        create: function (event, ui) {
            $log.info('Event: create', event, ui);
        },
        slide: function (event, ui) {
            $log.info('Event: slide', event, ui);
        },
        start: function (event, ui) {
            $log.info('Event: start', event, ui);
        },
        stop: function (event, ui) {
            $log.info('Event: stop', event, ui);
        }
    };

    this.binds = [
        { slide: 'value' }
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
