!function($){var Checkbox=function(element,options){this.$element=$(element);this.options=$.extend({},$.fn.checkbox.defaults,options);this.$label=this.$element.parent();this.$icon=this.$label.find('i');this.$chk=this.$label.find('input[type=checkbox]');this.setState(this.$chk);this.$chk.on('change',$.proxy(this.itemchecked,this));};Checkbox.prototype={constructor:Checkbox,setState:function($chk){var checked=$chk.is(':checked');var disabled=$chk.is(':disabled');this.$icon.removeClass('checked').removeClass('disabled');if(checked===true){this.$icon.addClass('checked');}
if(disabled===true){this.$icon.addClass('disabled');}},enable:function(){this.$chk.attr('disabled',false);this.$icon.removeClass('disabled');},disable:function(){this.$chk.attr('disabled',true);this.$icon.addClass('disabled');},toggle:function(){this.$chk.click();},itemchecked:function(e){var chk=$(e.target);this.setState(chk);}};$.fn.checkbox=function(option,value){var methodReturn;var $set=this.each(function(){var $this=$(this);var data=$this.data('checkbox');var options=typeof option==='object'&&option;if(!data)$this.data('checkbox',(data=new Checkbox(this,options)));if(typeof option==='string')methodReturn=data[option](value);});return(methodReturn===undefined)?$set:methodReturn;};$.fn.checkbox.defaults={};$.fn.checkbox.Constructor=Checkbox;$(document).on('click','.checkbox-custom > input[type=checkbox]',function(e){var $this=$(e.target);if($this.data('checkbox'))return;$this.checkbox($this.data());});}(window.jQuery);