﻿$(document).ready(function () {

    // Initialize variables
    var keyValues = [];
    var attemptNumber = 1;

    $('#btnNoColor, #btnPostResult').on('click', function () {
        var noColor = $(this).attr('id') === "btnNoColor";

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
            InitiateCharList(htmlValues);

            return;
        }
        
        $.each(htmlValues, function (index, value) {

            if (index === charIndex) {
                var colorValue = colorWheel.color.rgb;
                var ascii = parseInt($(this).attr('title'));

                var pair;
                if (noColor) {
                    pair = { Ascii: ascii, AttemptNumber: attemptNumber, R: null, G: null, B: null };
                }
                else {
                    pair = { Ascii: ascii, AttemptNumber: attemptNumber, R: colorValue.r, G: colorValue.g, B: colorValue.b };
                }
                
                keyValues.push(pair);

                //Randomize color value
                var newH = Math.floor(Math.random() * 360);
                var newS = Math.floor(Math.random() * 100);

                colorWheel.color.hsv = { h: newH, s: newS, v: 100 };

                $(this).attr('hidden', true);
            }
            else if (index === charIndex + 1) {
                $(this).attr('hidden', false);
            }
        });
        
        // Reached the end of alphabet
        if (charIndex === htmlValues.length - 1) {
            
            // Navigate to result page after 3 attempts
            if (++attemptNumber > 3) {
                $(this).parent().remove();

                $.ajax({
                    url: '/Alphabet/AlphabetResult',
                    data: {
                        'viewModel': { 'results': keyValues }
                    },
                    method: 'POST',
                    success: function (responseData) {
                        //TODO: wait for response before routing to result page
                        window.location = '../Result';
                    },
                    error: function (responseData) {
                        //Deal with errors if any
                        window.location = '../..';
                    }
                });

            }
            else {
                InitiateCharList(htmlValues);
            }
        }
        
    });

    $('#btnIdentificationSkip').on('click', function () {
        $('#alphabetIdentification').attr('hidden', true);
        $('#alphabetSurvey').attr('hidden', false);
    });

    $('#btnIdentification').on('click', function () {

        $.ajax({
            url: '/Alphabet/SurveyInformation',
            data: {
                'viewModel': {
                    'AnonCode': $('#Code').val(),
                    'Email': $('#Email').val(),
                    'AnonAge': $('#Age').val() 
                }
            },
            method: 'POST',
            success: function (responseData) {
                keyValues.length = 0;
            }
        });

        $('#alphabetIdentification').attr('hidden', true);
        $('#alphabetSurvey').attr('hidden', false);
    });

    // Listen to when participant submits his color of choice
    $('#btnDownloadAB').on('click', function () {
        $.ajax({
            url: '/Alphabet/DownloadSingle',
            data: {
                'id': 1
            },
            method: 'POST',
            success: function (responseData) {
                var parsedData = JSON.parse(responseData);
                var jsonObject = JSON.stringify(parsedData);
                var csv = ConvertToCSV(jsonObject);
                
                a = document.createElement('a');
                a.download = "AlphabetResults.csv";
                a.href = 'data:text/csv;charset=utf-8,' + escape(csv); //Actual content

                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
            }
        });
    });
});

function InitiateCharList(htmlValues) {
    var charList = [];

    //Randomize List
    $.each(htmlValues, function (index, value) {
        charList.push({
            title: $(this).attr('title'),
            text: $(this).text()
        });
    });

    shuffle(charList);

    $.each(htmlValues, function (index, value) {
        $(this).attr('title', charList[index].title);
        $(this).text(charList[index].text);
    });

    htmlValues.first().removeAttr('hidden');
}

function ConvertToCSV(objArray) {
    var array = typeof objArray !== 'object' ? JSON.parse(objArray) : objArray;
    var header = '';
    var rows = '';

    for (var title in array[0]) {
        if (header !== '') header += ';';

        header += title;
    }

    rows += header + '\r\n';

    for (var i = 0; i < array.length; i++) {
        var line = '';
        for (var index in array[i]) {
            if (line !== '') line += ';';

            line += array[i][index];
        }

        rows += line + '\r\n';
    }

    return rows;
}