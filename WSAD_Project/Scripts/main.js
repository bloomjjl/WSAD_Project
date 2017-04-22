$(function () {



    // look up sessions for user's shopping cart
    var $shoppingCartSessionSearch = $('#ShoppingCartSessionSearch');
    var $sessionsList = $('#shopping_cart_session_search_list');

    $shoppingCartSessionSearch.keyup(function () {
        var searchUrl = '/api/sessionsearch?term=' + $shoppingCartSessionSearch.val();
        $.ajax({
            type: 'GET',
            url: searchUrl,
            success: function (sessions) {
                // clear list
                $sessionsList.empty();
                $sessionsList.hide();

                if ($shoppingCartSessionSearch.val() != null && $shoppingCartSessionSearch.val() != "")
                {
                    // display new list
                    $.each(sessions, function (i, session) {
                        $sessionsList.append('<a href="/ShoppingCart/AddSelectedSessionToOrder?sessionId=' + session.SessionId + '">' + session.SessionTitle + '</a><br/>');
                    });
                    $sessionsList.show();
                }
            }
        });
    });





});