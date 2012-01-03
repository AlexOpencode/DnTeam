(function ($) {

    $.fn.gridFilter = function (options) {
        var filterTimeout;
        var filterTimeoutTime = 1000;
        var theGrid = this;
        var postUrl = options.postUrl;
        var gridInsatance = options.gridInstance;
        var btnClearFilter = options.btnClearFilter;
        btnClearFilter.hide();

        var headRow = this.find("thead.t-grid-header").find("tr");
        var filterLine = "<tr>";
        $.each(headRow.children("th"), function () {
            var filterable = $(this)[0].attributes.getNamedItem("filterable-column");
            filterLine += '<td style=" border: none;margin-left:-2px;">';
            if (filterable) {
                filterLine += '<div><input type="filter" name="' + filterable.value + '"/></div>';
            }
            filterLine += "</td>";
        });
        filterLine += "</tr >";
        headRow.after(filterLine);

        this.find("input[type=filter]").keyup(function () {

            clearTimeout(filterTimeout);

            //reduce timeout time after each input while searching
            filterTimeoutTime = filterTimeoutTime - $(this).val().length * filterTimeoutTime / 6;

            filterTimeout = setTimeout(function () {

                applyFilter(gridInsatance, theGrid, postUrl, btnClearFilter, function () {
                    filterTimeoutTime = 1000;
                });

            }, filterTimeoutTime);
        });

        //bind Clear filter event
        btnClearFilter.click(function () {
            removeFilter(gridInsatance, theGrid, postUrl, btnClearFilter, function () {
                filterTimeoutTime = 1000;
            });
        });
    };

    //Clears filter inputs and rebinds the grid
    function removeFilter(gridInsatance, theGrid, postUrl, btnClearFilter) {
        $.each(theGrid.find("input[type=filter]"), function () {
            $(this).val("");
        });

        applyFilter(gridInsatance, theGrid, postUrl, btnClearFilter);
    }

    function applyFilter(gridInsatance, theGrid, postUrl, btnClearFilter) {
        var filterString = "";
        var grid = gridInsatance();

        $.each(theGrid.find("input[type=filter]"), function () {
            var value = $(this).val();
            if (value.length > 0) {
                filterString += "&filterQuery=" + $(this).attr('name') + "~" + value;
            }
        });

        var selectUrl = grid.ajax.selectUrl;

        //remove old query
        if (selectUrl.indexOf("?filterQuery") > 0) {
            selectUrl = selectUrl.substring(0, selectUrl.indexOf("?filterQuery"));
        } else if (selectUrl.indexOf("&filterQuery") > 0) {
            selectUrl = selectUrl.substring(0, selectUrl.indexOf("&filterQuery"));
        }


        var values = "";
        if (filterString.length > 0) {
            btnClearFilter.show();
            if (postUrl.indexOf("?") > 0) {
                values = filterString;
            } else {
                values = "?" + filterString.substring(1);
            }
        } else {
            btnClearFilter.hide();
        }

        //set filter string
        grid.ajax.selectUrl = selectUrl + values;

        //rebind grid
        grid.ajaxRequest();
    }

})(jQuery);
