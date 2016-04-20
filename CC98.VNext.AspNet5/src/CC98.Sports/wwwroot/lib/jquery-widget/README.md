# jquery-widget

Have you written a jQuery Widget but you don't have time to re-write the whole thing in angular?

Then you thought - I should be able to wrap it in an angular directive - right?

But - *wow* - it takes a lot of knowledge about the internals of angular to *properly* wrap your widget.

There must be a generic directive to wrap a jQueryUI widget right? Well none that I could find - so I wrote jquery-widget.

With this general-purpose angular directive you should be able to wrap **any** jQuery UIWidget written to the specs of
the [Widget Factory](http://api.jqueryui.com/jquery.widget/).

## Demos

Three standard widgets from jQueryUI - all use the same directive - just different factories to define them:
 
- [slider](http://plnkr.co/edit/9Pcn3q?p=preview)
- [spinner](http://plnkr.co/edit/dXOXnI?p=preview)
- [progressbar](http://plnkr.co/edit/hSDeSd?p=preview) 

The [index.html](https://github.com/jmcpeak/jquery-widget/blob/master/app/index.html) file has more working examples.

## Features

- Should work with any jQuery Widget written to the jQuery Widget [Factory Pattern](http://api.jqueryui.com/jquery.widget/)
- Written as a single general purpose directive - `<div jquery-widget="" config="yourFactoryHere">`
- Just need to write and pass in a config factory object - see [slider.js](https://github.com/jmcpeak/jquery-widget/blob/master/app/scripts/controllers/slider.js)

## Settings in the config factory

The only thing required to use the directive is the config attribute.
This is a factory object with all the settings unique to this widget.
You will define the name of the widget, the events and any options you
want to use as attributes here:
`<div jquery-widget config="sliderConfig" ng-model="model.foo"></div>`

```javascript
register a slider module
var app = angular.module('slider', ['jqueryWidget']);

register a factory against that module
app.factory('sliderConfig', function () {

// the name of the widget - $('.selector').slider()
// required
this.widget = 'slider';

// the default widget option
// optional
// will map to the model based on this
this.default = 'value';

// the list of html attributes you want to make available
// optional 
this.attributes = ['animate', 'disabled', 'max', 'min', 'orientation', 'range', 'step', 'value', 'values'];

// Any options you want to set on initialization
// optional
this.options = { min: 0, max: 10, step: 1 };

// Where you wire up the jQuery events that are raised
// optional
this.events = {
    change: function (event, ui) {
        $log.info('Event: change', event, ui);
    },
    slide: function (event, ui) {
        $log.info('Event: slide', event, ui);
    }
};

// An array of events that will get bound
// optional
// In this example, whenever the slide event is raised it will be bound to the model - here is where you specify
// that on the slide event you want ui.value used as the model value 
// http://api.jqueryui.com/slider/#event-slide
this.binds = [
    { slide: 'value' }
];

// the render callback
// optional
// if you need to change some values before they are put back into the widget - for instance the slider step
// can't be less than 1 or empty - do that here
this.render = function(viewValue, ngModel) {};
```

## Options
The model needs to start life as an object - ng-model="model.foo"

Then all options will just be available on the model, by name, as two way bindings:
```html
<input type="text" ng-model="model.foo.value">
```

## Methods
A method can be used two ways - with and without a value.

Without a value, it is just a string:
```html
<button ng-click="model.foo.method='pageUp'">pageUp</button>
```

With a value, it is an object where the key is the name of the method and the value is the value:
```html
<button ng-click="model.foo.method={pageUp: 10}">pageUp by 10</button>
```

## Events
Defined once in the config object

## Attributes

Don't need any fancy options - just want to make them available as attributes?

Set a list of them in the config object
```javascript
this.attributes = ['disabled'];
```
And boom - they are just available as bound attributes:

First define the directive
```html
<input jquery-widget config="sliderConfig" disabled="{{sDisabled}}" ng-model="model.foo">
```

Now just use it
```html
<input type="checkbox" value="{{sDisabled}}" ng-click="sDisabled = !sDisabled"/>
```

## Browser compatibility

TBD

## Installation using [Bower](http://bower.io/)

- `bower install jquery-widget`

## FAQ

TBD

