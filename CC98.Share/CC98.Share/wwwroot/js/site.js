// Write your Javascript code.
$(function () {
	var barSize = '@fileSizeNow';
	barSize = barSize.concat('px');
	$('.progress-bar').css('width', barSize);

	$('.change-size').each(function (index, element) {

		var unit = 'B';

		var size = parseInt($(element).text());

		if (size > 1099511627776) {
			unit = 'TB';
			size = size * 1.00 / 1099511627776;
		}
		if (size > 1073741824) {
			size = size * 1.00 / 1073741824;
			unit = 'GB';
		}
		if (size > 1048576) {
			size = size / 1048576;
			unit = 'MB';
		}
		if (size > 1024) {
			size = size / 1024;
			unit = 'KB';
		}
		size = size.toFixed(2);
		var finalSize = String(size) + unit;
		$(element).text(finalSize);

	});
});