// This script is used for the Ajax ordering of tickets for moderators and super admins
$(function () {
    $('select').on('change', function () {
        var selectedVal = $('#filterBy option:selected').val();
        var ordering = $('#ordering option:selected').val();
        $.ajax({
            url: '/Moderator/OrderBy?x=' + selectedVal + '&ord=' + ordering,
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