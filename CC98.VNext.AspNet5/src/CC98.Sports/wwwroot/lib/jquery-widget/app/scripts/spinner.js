'use strict';

var app = angular.module('spinner', ['jqueryWidget']);

// Configuration for the jQueryUI slider widget
app.factory('spinnerConfig', function ($log) {

    this.widget = 'spinner';

    this.default = undefined;

    this.attributes = ['culture', 'disabled', 'icons', 'incremental', 'max', 'min', 'numberFormat', 'page', 'step'];

    this.options = { min: 0, max: 100, step: 1, page: 10 };

    this.events = {
        change: function (event, ui) {
            $log.info('Event: change', event, ui);
        },
        create: function (event, ui) {
            $log.info('Event: create', event, ui);
        },
        spin: function (event, ui) {
            $log.info('Event: spin', event, ui);
        },
        start: function (event, ui) {
            $log.info('Event: start', event, ui);
        },
        stop: function (event, ui) {
            $log.info('Event: stop', event, ui);
        }
    };

    this.binds = [
        { spin: 'value' }
    ];

    return this;
});
