'use strict';

angular.module('jqueryWidget', []).directive('jqueryWidget', function ($injector, $log) {
    return {
        require: 'ngModel',
        compile: function () {
            return function (scope, element, attrs, ngModel) {
                var widgetFn;
                var configFactory = {};
                var init = false;

                if (attrs.config && attrs.config !== '' && $injector.has(attrs.config)) {
                    configFactory = $injector.get(attrs.config);
                } else {
                    $log.error('You need to define a config attribute with the name of the factory: config="foo"');
                    return;
                }

                if (configFactory.widget) {
                    widgetFn = configFactory.widget;
                } else {
                    $log.error('You need to define the name of the jQuery widget to use: this.widget="slider"');
                    return;
                }

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

                // Late-bind to prevent compiler clobbering
                // This will get called when it is all properly loaded on the page
                scope.$evalAsync(function (scope) {
                    // convenience properties
                    var attributeOptions = {};

                    // Ensure the convenience attributes are passed as options if they're defined
                    // This avoids init ordering issues where the widget's initial state (eg handle
                    // position) is calculated using widget defaults
                    // Note the attributes will take precedence over any duplicates in options
                    angular.forEach(configFactory.attributes, function (attribute) {
                        if (angular.isDefined(attrs[attribute])) {
                            attributeOptions[attribute] = parseValue(attrs[attribute]);
                        }

                        // Create an observer for each html attribute
                        // $observe() is a method on the Attributes object, and as such, it can only be used to observe
                        // the value change of a DOM attributes. It is only used/called inside directives.
                        attrs.$observe(attribute, function (inputValue) {
                            if (inputValue) {
                                var value = inputValue;
                                try {
                                    value = JSON.parse(inputValue);
                                } catch (e) {
                                }
                                if (typeof(value) !== 'object') {
                                    value = parseValue(value);
                                }
                                element[widgetFn]('option', attribute, value);
                            }
                        });
                    });

                    angular.forEach(configFactory.binds, function (bind) {
                        var key = Object.keys(bind)[0];
                        var value = bind[key];
                        element.bind(key, function (event, ui) {
                            // If the value is an object (rather than a string or a number), we should make a copy of the object
                            // before passing it to $setViewValue. This is because ngModel does not perform a deep watch of
                            // objects, it only looks for a change of identity. If you only change the property of the object
                            // then ngModel will not realise that the object has changed and will not invoke the $parsers and
                            // $validators pipelines.
                            var modelValue = (ngModel.$modelValue) ? angular.copy(ngModel.$modelValue) : {};

                            modelValue[value] = ui[value];

                            if (!scope.$$phase) {
                                scope.$apply(function () {
                                    ngModel.$setViewValue(modelValue, event);
                                });
                            }
                        });
                    });

                    // After we have parsed option values, join in events and convenience attributes
                    var optionsAndEvents = angular.extend({}, configFactory.options, configFactory.events, attributeOptions);

                    // Now call the widget with all options and events
                    element[widgetFn](optionsAndEvents);

                    init = true;
                });

                //
                // $render() is only invoked if the values of $modelValue and $viewValue are actually different than
                // their previous value. If $modelValue or $viewValue are objects (rather than a string or number)
                // then $render() will not be invoked if you only change a property on the objects. (Well, it will be
                // invoked the first time, but not after that)
                //
                // How to get around this? Add a watch with a deep copy set to true
                //

                //scope.$watch(function() {
                //    return {
                //        width: element.width(),
                //        height: element.height()
                //    };
                //}, function(newVal, oldVal) {
                //    console.log(element.width() + ' | ' + element.height());
                //}, true);

                scope.$watch(attrs.ngModel, function (newVal, oldVal) {
                    if (ngModel.$viewValue !== undefined && init) {

                        var value = ngModel.$viewValue;
                        if (Object().toString() === ngModel.$viewValue) {
                            value = ngModel.$modelValue;
                        }

                        if (typeof(value) === 'object' && Object.keys(value).length) {
                            if (!value.method) {
                                ngModel.$render();
                            } else {
                                if ('object' === typeof(ngModel.$modelValue.method)) {
                                    var methodName = Object.keys(ngModel.$modelValue.method)[0];
                                    var methodValue = parseValue(ngModel.$modelValue.method[methodName]);
                                    element[widgetFn](methodName, methodValue);
                                } else {
                                    element[widgetFn](ngModel.$modelValue.method);
                                }

                                var copy = angular.copy(value);
                                delete copy.method;
                                ngModel.$setViewValue(copy);
                            }
                        } else if (configFactory.default) {
                            element[widgetFn]('option', configFactory.default, parseValue(ngModel.$viewValue));
                        }
                    }
                }, true);

                // Update widget from model value - this gives us two-way binding
                ngModel.$render = function () {
                    if (ngModel.$viewValue) {
                        var viewValue = angular.copy(ngModel.$viewValue);

                        // Did the caller define a render callback?
                        // This is there chance to validate the model
                        if (configFactory.render && typeof(configFactory.render) === 'function') {
                            configFactory.render(viewValue, this);
                        }

                        // set all options at one time
                        element[widgetFn]('option', viewValue);
                    }
                };
            };
        }
    };
});