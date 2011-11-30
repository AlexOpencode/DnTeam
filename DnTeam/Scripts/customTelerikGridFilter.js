(function ($) {

    var theGrid;
    var filterTimeout;
    var filterTimeoutTime = 1000;
    var mouseIsInside = false;
    var postUrl;
    var gridInsatance;

    var methods = {
        init: function (options) {

            return this.each(function () {
                postUrl = options.postUrl;
                gridInsatance = options.getGridInstance;
                theGrid = $(this);
                //Add filter buttons to table headers
                $.each(theGrid.find("th.t-header"), function () {

                    //Validate if column is filtarable
                    if (isFilterable($(this))) {
                        var nameId = $(this).text().replace(" ", "");
                        $(this).append("<div id=\"F" + nameId + "\" class=\"gridFilter t-grid-filter\"><span class=\"t-icon t-filter\">Filter</span></div>");
                        $("body").append("<div id=\"divF" + nameId + "\" class=\"filterDiv\"><input type=\"text\" class=\"filterSearchBox\" /><ul class=\"filterDefined\"></ul><ul class=\"filterOffer\"></ul><div class=\"filterAction\"></div></div>");
                    }
                });

                //Bind hover on filter
                $(".gridFilter").hover(
                    function () {
                        $(this).addClass("t-state-hover");
                    },
                    function () {
                        $(this).removeClass("t-state-hover");
                    });
                $('.filterDiv').hover(function () {
                    mouseIsInside = true;
                }, function () {
                    mouseIsInside = false;
                });
                $(document).mouseup(function () {
                    if (!mouseIsInside) {
                        $('.filterDiv').hide();
                        filterTimeoutTime = 1000;

                        $(".filterSearchBox").val(""); //clear search box
                        $("ul.filterOffer li").remove(); //delete offers
                        if ($("ul.filterDefined li").length <= 0) {
                            $("div.filterAction a").remove(); //delete buttons if there are no defined filters
                        }
                    }
                });

                //Bind filter form
                $(".gridFilter").click(function () {

                    var filterDiv = $("#div" + $(this).attr('id'));

                    clearTimeout(filterTimeout); //clear timeout

                    var th = $(this).offset();
                    filterDiv.css({
                        top: th.top + 3,
                        left: th.left - 233
                    }).show(); //Open filter form

                    filterDiv.children(".filterSearchBox").focus(); //set focus to searh box
                });

                //Update lookup values handler
                $(".filterSearchBox").keypress(function () {
                    var searchBox = $(this);
                    var query = searchBox.val();
                    clearTimeout(filterTimeout);

                    filterTimeoutTime = filterTimeoutTime - query.length * filterTimeoutTime / 6; //reduse time out time after each input while searching

                    filterTimeout = setTimeout(function () {

                        var filterDiv = searchBox.closest("div");
                        var columnName = filterDiv.attr("id").substr(4, filterDiv.attr("id").length);

                        $.post(postUrl, { column: columnName, query: query }, function (data) {
                            var filterOffer = filterDiv.find("ul.filterOffer");
                            filterOffer.find("li").remove();
                            var values = getDefinedFilterValues(filterDiv);

                            $.each(data, function (index, value) {
                                if (jQuery.inArray(value, values) < 0) { //add filter offers if not mentioned
                                    filterOffer.append("<li><input type=\"checkbox\" name=\"filterValues\" value=\"" + value + "\" />" + value + "</li>");
                                }
                            });

                            if (data.length > 0 && $("#apply" + columnName).length < 1) {

                                filterDiv.find("div.filterAction").append("<a id=\"apply" + columnName + "\" href=\"#\" class=\"t-button t-button-icontext\"><span class=\"t-icon t-filter\"></span><label>Apply Filter</label></a>");
                                $("#apply" + columnName).click(function (e) {
                                    e.preventDefault();
                                    applyFilter(columnName);
                                    $('#F' + columnName).addClass('t-active-filter'); //set filter icon
                                });
                            }

                            var ss = filterDiv.find("ul.filterDefined li").length;

                            if (data.length <= 0 && filterDiv.find("ul.filterDefined li").length <= 0) {
                                $("#apply" + columnName).remove();
                            }

                        }, "json");
                    }, filterTimeoutTime);
                });
            });
        }
    };

    $.fn.customTelerikGridFilter = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.setCustomTelerikGridFilter');
        }

    };

    function isFilterable(th) {
        var name = th.children("a").text();
        return (name.length > 0) ? true : false;
    }

    function applyFilter(columnName) {
        //get grid instanse
        var grid = gridInsatance();


        //add just selected values to the filter
        $('#divF' + columnName).find("ul.filterOffer li input:checkbox[name=filterValues]:checked").each(function () {
            $('#divF' + columnName).find("ul.filterDefined").append($(this).closest("li"));
        });

        //Get defined filters and create query url
        var filterString = "";
        $("ul.filterOffer").each(function () {
            var div = $(this).closest("div");
            var values = getDefinedFilterValues(div);
            removeUncheckedDefinedFilterValues(div);

            if (values.length > 0) {
                filterString += "&filterQuery=" + div.attr("id").substring(4, div.attr("id").length) + "~" + values;
                var actionDiv = div.find("div.filterAction");
                if (actionDiv.find("a.clearFilter").length < 1) { //append Clear button if absent
                    actionDiv.prepend("<a id=\"clear" + columnName + "\" href=\"#\" class=\"t-button t-button-icontext clearFilter\"><span class=\"t-icon t-clear-filter\"></span><label>Clear</label></a>");
                    $("#clear" + columnName).click(function (e) {
                        e.preventDefault();
                        clearFilter(div, columnName);
                    });
                }
            }

        });

        var selectUrl = grid.ajax.selectUrl;
        if (selectUrl.indexOf("?") > 0) {
            selectUrl = selectUrl.substring(0, selectUrl.indexOf("?"));
        }
        
        grid.ajax.selectUrl = selectUrl + "?filterQuery=none" + filterString;
        grid.ajaxRequest();

        //hide filter
        $('#divF' + columnName).hide();
    }

    function getDefinedFilterValues(filterDiv) {
        var values = new Array();

        filterDiv.find("ul.filterDefined li input:checkbox[name=filterValues]:checked").each(function () {
            values.push($(this).val());
        });

        return values;
    }

    function removeUncheckedDefinedFilterValues(filterDiv) {
        filterDiv.find("ul.filterDefined li input:checkbox[name=filterValues]:not(:checked)").each(function () {
            $(this).closest("li").remove();
        });

        if (filterDiv.find("ul.filterDefined li").length <= 0) {
            removeFilterIcon(filterDiv);
        };
    }

    function removeFilterIcon(filterDiv) {
        $(filterDiv).removeClass("t-active-filter"); //remove grid icon
    }

    function clearFilter(filterDiv, columnName) {
        filterDiv.find("ul li").remove(); //clear all checkboxes
        filterDiv.find("div a").remove(); //remove buttons
        removeFilterIcon($("#F" + columnName)); //remove filter-active icon
        applyFilter(columnName); //update grid values
    }


})(jQuery);