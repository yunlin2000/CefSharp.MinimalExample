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
                alert('�зǷ��������ڻ�ȡ������Ϣ,��رշǷ���һ����������ȷǷ�����!');
            }
        };
        alws[l].onerror = function (A) {
            console.clear();
        };
    }
}
catch (a) {
}