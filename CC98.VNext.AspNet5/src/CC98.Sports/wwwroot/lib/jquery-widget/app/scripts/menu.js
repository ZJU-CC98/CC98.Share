'use strict';

var app = angular.module('menu', ['jqueryWidget']);

// Configuration for the jQueryUI slider widget

app.factory('menuConfig', function ($log) {

    this.widget = 'menu';

    this.default = undefined;

    this.attributes = ['disabled', 'icons', 'items', 'menus', 'position', 'role'];

    this.options = {};

    this.events = {
        blur: function (event, ui) {
            $log.info('Event: blur', event, ui);
        },
        create: function (event, ui) {
            $log.info('Event: create', event, ui);
        },
        focus: function (event, ui) {
            $log.info('Event: focus', event, ui);
        },
        select: function (event, ui) {
            $log.info('Event: select', event, ui);
        }
};

    return this;
});
