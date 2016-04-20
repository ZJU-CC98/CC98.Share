var AjaxForm = new Class({
    options: {
        dataType: 'text', //xml, json, script, html
        target: null, //If html datatype, selector to inject into
        callback: function(){}, //Fires after the request.
        context: null, //The first element that matches this selector will be a parameter of the callback function.
        complete: function(){}, //Fires on request completion.
        beforeSend: function(){} //Fires before request.
    },
    form: null,
    action: null,
    processor: null,
    method: null,
    context: null,

    initialize: function (form, options) {
        this.options = jQuery.extend({}, this.options, options);
        this.form = form;
        this.action = jQuery(this.form).attr('action');
        if (!this.action) {
            return;
        }
        this.method = jQuery(this.form).attr('method');
        if (this.options.context) {
            this.context = $(this.options.context).first();
        }

        this.processor = new JsonProcessor();

        var self = this;
        jQuery(this.form).submit(function(event) {
            self.submit(event);
        });
    },

    submit: function (event) {
        var self = this;
        event.preventDefault();
        $.ajax({
            url: this.action,
            type: this.method,
            dataType: this.options.dataType,
            context: this.context,
            data: jQuery(this.form).serialize(),
            beforeSend: this.options.beforeSend,
            complete: this.options.complete,
            error: function(xhr, type, exception) {
                self.error(xhr, type, exception)
            },
            success: function(data, status, xhr) {
                self.success(data, status, xhr)
            }
        }).done(this.options.callback);
    },

    error: function (xhr, type, exception) {
        this.processor.processJson(
            JSON.stringify({'message': this.options.errorMessage})
        )
    },

    success: function (data, status, xhr) {
        if (this.options.dataType == 'html') {
            this.injectHtml(data);
        } else {
            this.processor.processJson(data);
        }
    },

    injectHtml: function (html) {
        if (this.options.target) {
            $(this.options.target).html(html);
        }
    }
});