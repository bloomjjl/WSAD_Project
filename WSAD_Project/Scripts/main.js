$(function () {

    var sessions = $('sessions');

    $.ajax({
        type: 'GET',
        url: '/api/sessionsearch?term=',
        success: function (sessions) {
            console.log('Success', sessions);
            $.each(sessions, function (i, session) {
                sessions.append('<li>')
            });
        }
    });
});