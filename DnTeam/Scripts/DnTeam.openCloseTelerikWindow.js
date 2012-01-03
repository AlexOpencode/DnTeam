function closeWindow(name) {
    $("#" + name).data('tWindow').close();
}

function openWindow(name, title, yesButtonName, yesOnClick) {
    var wnd = $("#" + name).data('tWindow');
    if (title) { wnd.title(title); }
    if (yesButtonName) { $(".yesButton").children('label').text(yesButtonName); }
    if (yesOnClick) { $(".yesButton").attr('onclick', yesOnClick); }
    wnd.center().open();
}