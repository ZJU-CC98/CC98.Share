# jQuery Ajax Form
Ajax based form processing. Parses JSON responses for common actions like redirects, DOM updates or alerts. Requires [class.js](https://classjs.readthedocs.org/en/latest/) which is included in the bower_components folder if you're not using Bower.

Json responses are parsed with [Json Processor](https://github.com/incraigulous/json-processor), allowing you to manipulate dom elements via keyword hooks in ajax responses. Check out the [Json Processor Readme](https://github.com/incraigulous/json-processor) for documentation.

##Demo
Check out the demo here: [https://cdn.rawgit.com/incraigulous/jquery-ajax-form/master/demo.html](https://cdn.rawgit.com/incraigulous/jquery-ajax-form/master/demo.html)

##How to use it
Instantiate a new AjaxForm class and pass in your element. You can also pass an options object as your second parameter but this is optional.

````javascript
        //All options are optional.
        new AjaxForm($('.ajax-form'), {
            dataType: 'text', //xml, json, script, html
            target: null, //If html datatype, selector to inject into
            callback: function(){}, //Fires after the request.
            context: null, //The first element that matches this selector will be a parameter of the callback function.
            complete: function(){}, //Fires on request completion.
            beforeSend: function(){} //Fires before request.
        });
````

##How to install it
Download or clone it from directly from Github or install via Bower:
````
bower install jquery-ajax-form
````
