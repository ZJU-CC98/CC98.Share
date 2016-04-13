var JsonProcessor = function (element, options) {
    this.element = (element) ? element : $(document);
    this.options = jQuery.extend({}, this.defaults, options);
}

JsonProcessor.prototype = {
    defaults: {
        contentProcessor: function(){} //Content processor callback. This function will be applied to generated content.
    },
    filters: [
        'elements', //Process JSON on each element
        'execute', //Execute javascript string
        'event', //Fires an event
        'fields', //Sets the values of a list of fields
        'html', //Sets HTML value
        'message', //Alert a message
        'redirect', //Redirect to a URL in the same window
        'redirectBlank', //Redirect to a URL in a new window
        'remove', //Remove the element from the dom,
        'bsFieldError' //Sets a bootstrap style error on a field.
    ],
    elementFilters: [
        'attributes', //Set element attributes
        'html', //Sets HTML value
        'url', //Gets HTML via AJAX and sets HTML value
        'bsFieldError' //Sets a bootstrap style error on a field.
    ],
    properties: {}, //The parsed JSON

    /**
     * Parse the JSON then send it's properties to be filtered for action.
     *
     * @param json
     */
    processJson: function(json) {
        if (!json) {
            return;
        }
        properties = jQuery.parseJSON(json);
        this.process(properties);
    },

    /**
     * Filter properties for action.
     *
     * @param properties
     */
    process: function(properties) {
        var self = this;
        this.properties = properties;
        $.each(properties, function(property, value) {
            self.filterProperty(property, value);
        });
    },

    /**
     * Set a custom filters array.
     *
     * @param filters
     */
    setFilters: function(filters) {
        this.filters = filters;
    },

    /**
     * Get the current filters array. Useful if you want to add some filters to the exiting filters.
     *
     * @returns {*}
     */
    getFilters: function() {
        return this.filters;
    },

    /**
     * Check to see if the property is filterable. If so, filter it.
     *
     * @param property
     * @param value
     */
    filterProperty: function(property, value) {
        if($.inArray(property, this.filters) != -1) {
            this.callFilter(property, value);
        }
    },

    /**
     * Call the filter method.
     *
     * @param property
     * @param value
     */
    callFilter: function (property, value) {
        var filterMethodName = property + 'Filter';
        this[filterMethodName](value);
    },

    /***********************************
     *  Filter Methods
     ***********************************/

    /**
     * Sets an elements attributes
     *
     * Takes the following parameters from the json request:
     * addClass: The class to add.
     * removeClass: The class to remove.
     * setProperty: The property to set. Set property takes the following parameters from the json request:
     *      Takes the following property parameters:
     *      property: The property to set.
     *      value: The property value to set.
     *
     * @param attributes
     */
    attributesFilter: function (attributes) {
        if (attributes.addClass) {
            jQuery(this.properties.target).addClass(attributes.addClass);
        }
        if (attributes.removeClass) {
            jQuery(this.properties.target).removeClass(attributes.removeClass);
        }
        if (attributes.setProperty) {
            if (attributes.setProperty.property) {
                jQuery(this.properties.target).attr(attributes.setProperty.property, attributes.setProperty.value);
            }
        }
    },

    /**
     * Recursively call json processor on each element
     *
     * @param elements
     */
    elementsFilter: function(elements) {
        var self = this;
        $.each(elements, function(key, elementData) {
            self.processNewElement(elementData);
        });
    },

    /**
     * Fire an event.
     * Requires target.
     *
     * @param event
     */
    eventFilter: function(event) {
        if (!this.properties.target) {
            return;
        }
        $(this.properties.target).trigger(event);
    },
    /**
     * Execute javascript string.
     *
     * @param js
     */
    executeFilter: function(js) {
        eval(js);
    },

    processNewElement: function(elementData) {
        var processor = new JsonProcessor(this.element, this.options);
        processor.setFilters(this.elementFilters);
        processor.process(elementData);
    },

    /**
     * Set the values of a list of fields.
     *
     * @param fields
     */
    fieldsFilter: function(fields) {
        var self = this;
        $.each(fields, function(id, field) {
            jQuery(self.element).find(field.target).val(field.value).trigger('update');
        });
    },

    /**
     * Inject HTML into matching HTML elements.
     *
     * Must be included along with the HTML element in properties:
     * target (required): The selector to inject into.
     * append (bool): Add to top of container.
     * prepend (bool): Add to bottom of container. Append takes precedence over prepend.
     *
     * @param html
     */
    htmlFilter: function(html) {
        var self = this;
        if (this.properties.target) {
            jQuery(this.element).find(this.properties.target).each(function(key, element){
                if (self.properties.append) {
                    var injectedContent = jQuery(element).append(html);
                } else if (self.properties.prepend) {
                    var injectedContent = jQuery(element).prepend(html);
                } else {
                    var injectedContent = jQuery(element).html(html);
                }
                self.options.contentProcessor(injectedContent);
            });
        }
    },

    /**
     * Alert a message
     *
     * @param elements
     */
    messageFilter: function (value) {
        alert(value);
    },


    /**
     * Redirect to a URL in the same window.
     *
     * @param url
     */
    redirectFilter: function(url) {
        document.location.href = url;
    },

    /**
     * Redirect to a URL in a new window.
     *
     * @param url
     */
    redirectBlankFilter: function(url) {
        window.open(json.redirect_blank, '_blank');
    },

    /**
     * Remove matched elements from the DOM.
     *
     * @param elements
     */
    removeFilter: function(selector) {
        jQuery(this.element).find(selector).remove();
    },

    /**
     * Gets HTML via AJAX and sets HTML value
     *
     * @param elements
     */
    urlFilter: function(url) {
        var self = this;
        $(this.properties.target).load($url, function() {
            self.options.contentProcessor(this);
        });
    },

    /**
     * Sets a bootstrap style error on a field.
     *
     * @param elements
     */
    bsFieldErrorFilter: function(message) {
        var target = $(this.properties.target);
        var group = target.parents('.form-group').first();
        if (group) {
            group.addClass('has-error');
        }
        group.append('<p class="help-block">' + message + '</p>');
    }
};
