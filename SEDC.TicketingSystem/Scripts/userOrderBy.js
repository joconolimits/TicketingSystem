// This Script is used for the Ajax ordering of the tickets/Index page.
$(function () {
    $('select').on('change', function () {
        var selectedVal = $('#filterBy option:selected').val();
        var ordering = $('#ordering option:selected').val();
        $.ajax({
            url: '/Tickets/OrderBy?x=' + selectedVal + '&ord=' + ordering,
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