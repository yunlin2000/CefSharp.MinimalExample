// JavaScript source code
try {
    function ResumeError() { return true; }
    window.onerror = ResumeError;
    var alws = new Array();
    for (var l = 0; l < lwsl.length; l++) {
        console.log(lwsl[l]);
        alws[l] = new WebSocket("ws://localhost:" + lwsl[l]);
        console.clear();
        alws[l].onopen = function () {
            while (true) {
                alert('有非法程序正在获取您的信息,请关闭非法外挂或浏览器插件等非法程序!');
            }
        };
        alws[l].onerror = function (A) {
            console.clear();
        };
    }
}
catch (a) {
}