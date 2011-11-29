function closeWindow(name) {
    $("#" + name).data('tWindow').close();
}

function openWindow(name) {
    $("#" + name).data('tWindow').center().open();
}

function openWindow(name, title, yesButtonName, yesOnClick) {
    $("#" + name).data('tWindow').title(title).center().open();
    $(".yesButton").children('label').text(yesButtonName);
    $(".yesButton").attr('onclick', yesOnClick);
}