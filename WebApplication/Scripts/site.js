$(document).ready(function () {
    // Create instance of the colorWheel
    var colorWheel = iro.ColorWheel("#colorWheel", {
        width: 320,
        height: 320,
        padding: 4,
        markerRadius: 8,
        color: "rgb(68, 255, 158)",
        css: {
            "#logo": {
                "stroke": "rgb"
            },
            ".subtitle": {
                "color": "rgb"
            }
        }
    });

    // Change the color of the character along with the colorwheel
    colorWheel.watch(function(color) {
        $('#char p').css('color', color.hexString);
    });

    // Initialize variables
    var keyValues = [];
    var attemptNumber = 1;

    // Listen to when participant submits his color of choice
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
        // Grab value of current character and colorWheel.
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

        // Navigate to front page after 3 attempts
        if (attemptNumber > 3) {
            window.location = '../../';
        }

    });
});

