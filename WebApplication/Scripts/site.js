﻿$(document).ready(function () {
    var keyValues = [];
    var attemptNumber = 1;

    //TODO: Add attemptNumber

    $('#btnPostResult').on('click', function () {
        var htmlValues = $('#char').children();
        var charIndex = 0;

        // Check if all characters are hidden
        var allHidden = true;
        $.each(htmlValues, function (index, value) {
            var attr = $(this).attr('hidden');

            // For some browsers, `attr` is undefined; for others,
            // `attr` is false.  Check for both.
            if (typeof attr === typeof undefined || attr === false) {
                charIndex = index;
                allHidden = false;
            }
        });
        if (allHidden) {
            htmlValues.first().removeAttr('hidden');
            return;
        }


        // Hide current char and show next one.
        // Grab value of current character and color picker.
        $.each(htmlValues, function (index, value) {

            if (index === charIndex) {
                var ascii = parseInt( $(this).attr('title') );
                var colorValue = colorWheel.color.rgb;

                var pair = { Ascii: ascii, R: colorValue.r, G: colorValue.g, B: colorValue.b };

                keyValues.push(pair);

                $(this).attr('hidden', true);
            }
            else if (index === charIndex + 1) {
                $(this).attr('hidden', false);
            }
        });


        // Reached the end of alphabet
        if (charIndex === htmlValues.length - 1) {
            $.ajax({
                url: '/Alphabet/AlphabetResult',
                data: {
                    'viewModel': { 'results': keyValues, 'attemptNumber': attemptNumber }
                },
                method: 'POST',
                success: function (responseData) {
                    keyValues.length = 0;
                }
            });

            attemptNumber++;
        }
        
        if (attemptNumber > 3) {
            window.location = '../../';
        }

    });

});

