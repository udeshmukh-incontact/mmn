function SetMessagesOnKendoGrid(gridId, noRecordsMessage, largeRecordsMessage, largeRecordsLimit) {

    var grid = $(gridId).data('kendoGrid');

    var dataMessageId = $(gridId).attr('id') + '-dataMessage';

    if (grid.dataSource.total() === 0) {
        var colCount = grid.columns.length;
        grid.tbody.append('<tr class="kendo-data-row"><td colspan="' + colCount + '" style="text-align:center"><b>' + noRecordsMessage + '</b></td></tr>');
        $('#' + dataMessageId).remove();
    }
    else if (largeRecordsLimit !== undefined && grid.dataSource.total() >= largeRecordsLimit) {
        hideFooter(gridId);
        if($('#' + dataMessageId).size() === 0)
            $(gridId).parent().before('<div class="info-message clear-both" style="margin-top:.5em;" id="' + dataMessageId + '">' + largeRecordsMessage + '</div>');
    }
    else {
        $('#' + dataMessageId).remove();
    }
}

function hideFooter(gridId) {
    var footer = $(gridId).find('.k-grid-footer');

    if (footer.is(':visible')) {
        var gridContent = $(gridId).find('.k-grid-content');
        var contentHeight = gridContent.height();
        gridContent.height(contentHeight + footer.height());
        footer.hide();
    }
}