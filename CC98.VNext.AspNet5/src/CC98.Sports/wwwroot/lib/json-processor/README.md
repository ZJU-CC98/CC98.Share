JSON Processor
==============

> Note: JSON Processor is still in development, but everything should be working!

Parses JSON to automagically handle common post-request actions like adding/removing elements, altering element properties, injecting HTML, redirecting to a new page, and making new AJAX requests. JSON Processor is based on jQuery.

##How it works
You pass a JSON response to JSON Processor and it will parse the JSON looking for predefined filter keys. If it finds one, it will take whatever action is tied to that filter.

##A basic example
Let's say after an AJAX form request, you want to add a success message above the form. You would return the following from the AJAX Form's submit request:
    
    {
        "elements": {
            "0": {
                "target": "#message",
                "html":"<b>Thanks! </b> We've received your submission!"
            }
        }
    }
    

Then you would instantiate a new JsonProcessor instance in your $.ajax success function and pass the resulting JSON to it:

    $.ajax({
           type: 'POST',
           url: 'request/url',
           data: $('#form').serialize(),
           success: function(json)
           {
               var processor = new JsonProcessor(); 
               processor.processJson(json);
           }
         });
         
##Getting Started
After including jQuery and jquery.json-processor.js, JSON Processor will be available by: 

    var processor = new JsonProcessor(); 
    processor.processJson(json);
    
##Installing via Bower

    bower install json-processor

##Filters
###elements
Process JSON on each element. The elements filter has it's own sub filters that are applied to the target element.
    
####Element Filters
**attributes**<br />
Set element attributes:

    {
        "elements": {
            "0": {
                "target": "#content",
                "attributes": {
                    "addClass": "new-css-class",
                    "removeClass": "old-css-class",
                    "setProperty": {
                        "property": "style",
                        "value": "background-color: red;"
                    }
                }
            }
        }
    }
    
Attributes options:

- **addClass**: The class to add.
- **removeClass**: The class to remove.
- **setProperty**: The property to set. Set property takes the following parameters: 
    - **property**: The property to set. 
    - **value**: The property value to set.
     
**html**<br />
Sets HTML value:

    {
        "elements": {
            "0": {
                "target": "#target-element",
                "html":"<div>Inject this div into #target-element.</div>"
            }
        }
    }
    
**url**<br />
Gets HTML via AJAX and sets HTML value:

    {'elements': {
        0: {
            'target': '#content',
            'url': '/ajax'
        }
    }
    
See top level HTML filter entry below for options.
    
###execute
Execute javascript string:

    {
        "execute": "alert('a message')"
    }

###event
Fires an event:

    {
        "target": "#my-input",
        "event": "change"
    }
    
###fields
Sets the values of a list of fields:

    {
        "fields": {
        "0": {
                "target":"#my-input",
                "value":"The new value!"
            }
        }
    }


###html
Sets HTML value:

    {
        "target": "#target-element",
        "html":"<div>Inject this div into #target-element.</div>"
        "prepend": true
    }
    
Html Options:

- **prepend** (bool): Retain current content and inject html content to the top of the target.
- **append** (bool): Retain current content and inject html content to the bottom of the target.

###message
Alert a message:

    {
        "message": "This will be alerted!"
    }
    
###redirect 
Redirect to a URL in the same window:

    {
        "redirect": "/newurl/"
    }

###redirectBlank
Redirect to a URL in a new window:

    {
        "redirectBlank": "/newurl/"
    }

###remove 
Remove from the dom:

    {
        "remove": "#remove-me"
    }

## Limit targeting
JSON processor will apply any DOM modifications to the entire document by default. To limit targeting to an element and it's descendants, pass an element when instantiating JsonProcessor.

    var processor = new JsonProcessor($('#target'));
    
## Options
The only option is a contentProcessor, a callback function to execute on any generated content. 

    var processor = new JsonProcessor(null,
        {
            'contentProcessor:': processContent
        });

##Looking for a MooTools version? 
This package was inspired by the JsonProcessor component of jdavidbakr's excellent MooTools package PopupForm: https://github.com/jdavidbakr/PopupForm
