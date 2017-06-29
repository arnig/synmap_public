$(document).ready(function () {

    $('#btnPostResult').on('click', function () {
        var htmlValues = $('#char').children();
        var charIndex = 0;

        // Check if all characters are hidden
        var allHidden = true;
        $.each(htmlValues, function (index, value) {
            var attr = $(this).attr('hidden');


            // For some browsers, `attr` is undefined; for others,
            // `attr` is false.  Check for both.
            if (typeof attr !== typeof undefined && attr !== false) {
                console.log('is hidden');
            }
            else {
                charIndex = index;
                allHidden = false;
            }
        });
        if (allHidden)
        {
            htmlValues.first().removeAttr('hidden');
            return;
        }

        // Some character is showing
        $.each(htmlValues, function (index, value) {
            var attr = $(this).attr('hidden');

            if (index === charIndex) {
                $(this).attr('hidden', true);
            }
            if (index === charIndex + 1) {
                $(this).attr('hidden', false);
            }
        });
        
        
        /*$.ajax({
                url: '/Alphabet/GetCharacters',
                data: values.serialize(),
                method: 'POST',
                success: function (responseData) {
                }
        });*/

        
    });

})

