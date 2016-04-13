/**
 * @preserve
 * Author: http://mrhenry.be
 *
 * How to use:
 *
 * var Ajaxform = require('./ajaxform'),
 *     formContainer = $('.parent-of-form');
 *
 * new Ajaxform(formContainer);
 *
 * Be sure that the form_container only contains 1 form
 */


'use strict';


/**
 * jQuery function
 *
 * @param  {Object} options
 *
 * @return {jQuery array}
 */
$.fn.ajaxform = function(options) {
  return this.each(function() {
    if ( !$.data(this, 'ajaxform') ) {
      $.data(this, 'ajaxform', new Ajaxform( $(this), options));
    }
  });
}


/**
 * @constructor
 *
 * @param {jQuery element}  element
 * @param {Object}          options
 *
 * @return {Ajaxform}
 */
function Ajaxform($element, options) {
  this.$element = $element;
  this.$form    = this.$element.find('form').first();
  this.$token   = $('<input type="hidden" name="token" value="' + options.token + '">');
  this.busy     = false;

  if ( options.callback === undefined || !$.isFunction(options.callback) ) {
    this.callback = function(){};

  } else {
    this.callback = options.callback;

  }

  this.init();
}


/**
 * Initialize
 */
Ajaxform.prototype.init = function() {
  var _this = this;

  // Append security token
  _this.$form.append( _this.$token );

  // Form submit event
  _this.$element.on('submit', 'form', $.proxy(this.onSubmit, this));
};


/**
 * Form submit handler
 *
 * @param  {jQuery event} e
 */
Ajaxform.prototype.onSubmit = function(e) {
  if ( this.busy ) {
    return;
  }

  e.preventDefault();

  // Act busy
  this.busy = true;
  this.$form.addClass('busy');

  // Make ajax request
  $.ajax({
    data:     this.$form.serialize(),
    error:    $.proxy(this.requestError, this),
    success:  $.proxy(this.requestSuccess, this),
    type:     this.$form.attr('method'),
    url:      this.$form.attr('action')
  });
};


/**
 * Request sucess handler
 *
 * @param  {String} responseData
 */
Ajaxform.prototype.requestSuccess = function(responseData) {
  var _this         = this,
      $responseData = Ajaxform.cleanResponse( responseData ),
      $newForm      = $responseData.find("form").first();

  // Stop acting busy
  _this.busy = false;
  _this.$form.removeClass('busy');

  // Replace old form with new form
  _this.$form.replaceWith($newForm);
  _this.$form = $newForm;

  // Execute callback
  _this.callback();
};


/**
 * Request error handler
 *
 * Show an alert message and
 * removes the busy state
 *
 * @param  {Object} jqXHR
 * @param  {String} textStatus
 * @param  {String} errorThrown
 */
Ajaxform.prototype.requestError = function(jqXHR, textStatus, errorThrown) {
  window.alert("Oops, something went wrong. Please try again.");

  // Stop acting busy
  this.busy = false;
  this.$form.removeClass('busy');
};


/*
 * Strip out script tags of jQuery ajax response data and
 * returns a traversable jQuery object
 *
 * @return [jQuery object]
 */
Ajaxform.cleanResponse = function(responseData) {
  var regexScript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi;
  return $('<div>').append(responseData.replace(regexScript, ''));
};
