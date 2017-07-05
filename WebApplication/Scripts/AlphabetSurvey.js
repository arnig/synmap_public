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
