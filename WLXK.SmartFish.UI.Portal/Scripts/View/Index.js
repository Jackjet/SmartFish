$(function () {
    $(".list").ligerAccordion({ height: 220 });

    LoadValues();

    setInterval(LoadValues, 10000);
});
function LoadValues() {
    $.post("/Home/GetAllValues", {}, function (data) {
        if (data != undefined) {
            var object = eval("[" + data + "]");

            var wenduId = $("#wendusel option:selected").text();

            if (wenduId == "1") {
                changeValues("#sheshidu", object[0].EnviroTemperate);
            }
            else if (wenduId == "2") {
                changeValues("#sheshidu", object[0].EnviroTemperate2);
            }

            var shiduId = $("#shidusel option:selected").text();
            if (shiduId == "1") {
                changeValues("#shidu", object[0].EnviroHumidity);
            }
            else if (shiduId == "2") {
                changeValues("#shidu", object[0].EnviroHumidity2);
            }

            var guangzhaoId = $("#guangzhaosel option:selected").text();
            if (guangzhaoId == "1") {
                changeValues("#guangzhao", object[0].SunValue);
            }
            else if (guangzhaoId == "2") {
                changeValues("#guangzhao", object[0].SunValue2);
            }
        }
    })
}
function changeValues(m, v) {
    if (v > 0) {
        $(m).animate({ top: '+=20px', opacity: '0' }, "slow", function () {
            $(m).text(v);
        }).animate({ top: '-=40px' }, "slow").animate({ top: '+=20px', opacity: '1' }, "slow");
    }
}