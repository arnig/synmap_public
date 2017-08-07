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
colorWheel.watch(function (color) {
    $('#char p').css('color', color.hexString);
});

function shuffle(array) {
    var currentIndex = array.length, temporaryValue, randomIndex;

    // While there remain elements to shuffle...
    while (0 !== currentIndex) {

        // Pick a remaining element...
        randomIndex = Math.floor(Math.random() * currentIndex);
        currentIndex -= 1;

        // And swap it with the current element.
        temporaryValue = array[currentIndex];
        array[currentIndex] = array[randomIndex];
        array[randomIndex] = temporaryValue;
    }

    return array;
}


//Customize brightness with arrow keys
document.onkeydown = function (e) {
    e = e || window.event;
    // use e.keyCode

    var HSV = colorWheel.color.hsv;

    if (e.keyCode === 37) {
        if (HSV.v > 0) {
            colorWheel.color.hsv = { h: HSV.h, s: HSV.s, v: HSV.v - 1 };
        }
    }
    else if (e.keyCode === 39) {
        if (HSV.v < 100) {
            colorWheel.color.hsv = { h: HSV.h, s: HSV.s, v: HSV.v + 1 };
        }
    }
};