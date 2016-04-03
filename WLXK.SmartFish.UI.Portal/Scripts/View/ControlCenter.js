
$(function () {
    $(".list").ligerAccordion({ height: 200 });
    //设置水质
    $("#btnsetshuizi").click(function () {
        SetTurbidity();
    })
    //设置氧气
    $("#btnsetyangqi").click(function () {
        Setyangqi();
    })
    //设置温度
    $("#btnSetTemp").click(function () {
        SetTemp();
    })
    //开始排水
    $("#startPaishui").click(function () {
        StartPaishui();
    })
    //停止排水
    $("#endPaishui").click(function () {
        EndPaishui();
    })

    //开始进水
    $("#startJinshui").click(function () {
        StartJinshui();
    })
    //停止进水
    $("#endJinshui").click(function () {
        EndJinshui();
    })

    //增加氧气
    $("#addyangqi").click(function () {
        addyangqi();
    })
    //降低氧气
    $("#subyangqi").click(function () {
        subyangqi();
    })
    //增加温度
    $("#addwendu").click(function () {
        addwendu();
    })

    //降低温度
    $("#subwendu").click(function () {
        subwendu();
    })
    //打开1号灯
    $("#openfirst").click(function () {
        openfirst();
    })
    //关闭1号灯
    $("#closefirst").click(function () {
        closefirst();
    })

    //打开2号灯
    $("#opensecond").click(function () {
        opensecond();
    })
    //关闭2号灯
    $("#closesecond").click(function () {
        closesecond();
    })
});

//关闭二号灯
function closesecond() {
    $.post("/Home/OpenLight", { 'type': 0, 'num': 2 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}

//开启二号灯
function opensecond() {
    $.post("/Home/OpenLight", { 'type': 1, 'num': 2}, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}


//关闭一号灯
function closefirst() {
    $.post("/Home/OpenLight", { 'type': 0, 'num': 1 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}

//开启一号灯
function openfirst() {
    $.post("/Home/OpenLight", { 'type': 1,'num':1 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}

//降低温度
function subwendu() {
    $.post("/Home/WenDu", { 'type': 0 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}

//增加温度
function addwendu() {
    $.post("/Home/WenDu", { 'type': 1 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}

//减少氧气
function subyangqi() {
    $.post("/Home/YangQi", { 'type': 0 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}
//增加氧气
function addyangqi() {
    $.post("/Home/YangQi", { 'type': 1 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}
//开始排水
function StartPaishui() {
    $.post("/Home/PaiShui", { 'type': 1 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}
//停止排水
function EndPaishui() {
    $.post("/Home/PaiShui", { 'type': 0 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}

//开始进水
function StartJinshui() {
    $.post("/Home/JinShui", { 'type': 1 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}
//停止进水
function EndJinshui() {
    $.post("/Home/JinShui", { 'type': 0 }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}


//设置水质
function SetTurbidity() {
    $.post("/Home/SetTurbidity", { value: $("#Turbidity").val() }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}
//设置氧气
function Setyangqi() {
    $.post("/Home/SetYangQi", { value: $("#yangqi").val() }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}
//设置温度范围
function SetTemp() {
    $.post("/Home/SetTemp", { value1: $("#temp1").val(), value2: $("#temp2").val() }, function (data) {
        $.ligerDialog.tip({ title: '提示信息', content: data });
    })
}