function showOk(element) {
    var id = element.attr("id");
    id = (id == null) ? new Date().getTime() : id;
    
    var tmpId = 'Res' + id;
    var theSpan = $('#' + tmpId);
    if (theSpan.length > 0) { theSpan.remove(); }

    var ok = '<span class="resultOk" id="' + tmpId + '">@(Resources.Labels.Operation_Status_Saved)</span>';
    element.after(ok);
    $('#' + tmpId).delay(3600).fadeOut('fast', function () {
        $('#' + tmpId).remove();
    });
}

function showError(id, desc, wnd) {
    var tmpId = 'Res' + id.attr("id");
    var theSpan = $('#' + tmpId);
    
    if (theSpan.length > 0) { theSpan.remove(); }

    var ok = '<span class="resultError" id="' + tmpId + '">' + desc + '</span>';
    id.after(ok);

    if (wnd) { //resize window to suit contents
        var window = $("#" + wnd).data("tWindow");
        $(window.element).find(".t-window-content").css("height", "auto");
    }
    
    $('#' + tmpId).delay(6200).fadeOut('fast', function () {
        $('#' + tmpId).remove();
    });
}