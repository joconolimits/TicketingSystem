// This script is used for the Ajax filtering of tickets for moderators and super admins
$(function () {
    $('select').on('change', function () {
        var categoryId = $('#CategoryID option:selected').val();
        var statusId = $('#status option:selected').val();
        var key = $('#key').val();
        $.ajax({
            url: '/Moderator/FilterBy?categoryId=' + categoryId + '&statusId=' + statusId +'&key=' +key,
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