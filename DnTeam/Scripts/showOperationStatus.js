function showOk(id) {
    var tmpId = 'Res' + id;
    var theSpan = $('#' + tmpId);
    if (theSpan.length > 0) { theSpan.remove(); }
    
    var ok = '<span class="resultOk" id="' + tmpId + '">Saved!</span>';
    $('#' + id).after(ok);
    $('#' + tmpId).delay(3600).fadeOut('fast', function () {
        $('#' + tmpId).remove();
    });
}

function showError(id, desc) {
    var tmpId = 'Res' + id;
    var theSpan = $('#' + tmpId);
    if (theSpan.length > 0) { theSpan.remove(); }
    
    var ok = '<span class="resultError" id="' + tmpId + '"><b>Error:</b> '+ desc +'</span>';
    $('#' + id).after(ok);
    $('#' + tmpId).delay(6200).fadeOut('fast', function () {
        $('#' + tmpId).remove();
    });
}