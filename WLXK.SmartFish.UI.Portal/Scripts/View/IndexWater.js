$(function () {
    $(".list").ligerAccordion({ height: 220 });

    LoadValues();

    setInterval(LoadValues, 10000);

});
function LoadValues() {
    $.post("/Home/GetAllValues", {}, function (data) {
        if (data != undefined) {
            var object = eval("[" + data + "]");

            var wenduId = $("#shuiwensel option:selected").text();

            if (wenduId == "1") {
                changeValues("#shuiwen", object[0].FishTemperate);
            }
            else if (wenduId == "2") {
                changeValues("#shuiwen", object[0].FishTemperate2);
            }

            changeValues("#rongjieyang", object[0].Oxygen);
            changeValues("#phvalues", object[0].PhValues);
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