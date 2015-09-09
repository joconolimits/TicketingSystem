// This Script is used for the Ajax filtering of the tickets/Index page.
$(function () {
    $('select').on('change', function () {
        var categoryId = $('#CategoryID option:selected').val();
        var statusId = $('#status option:selected').val();
        $.ajax({
            url: '/Tickets/FilterBy?categoryId=' + categoryId + '&statusId=' + statusId,
            contentType: 'application/html; charset=utf-8',
            type: 'GET',
            dataType: 'html'
        })
        .success(function (result) {
            $('#tickets').html(result);
        })
        .error(function (xhr, status) {
            alert(status);
        })
    });
});
