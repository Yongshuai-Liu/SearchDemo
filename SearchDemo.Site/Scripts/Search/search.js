$(document).ready(function () {
    $('#searchResultDataTable').DataTable();
    $('#searchString').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Search/SpellCheck",
                type: "POST",
                dataType: "json",
                data: {
                    searchString: request.term
                },
                success: function (data) {
                    if (data.length > 0) {
                        response($.map(data, function (item) {
                            return {
                                label: item,
                                value: item,
                            }
                        }))
                    }
                }
            })
        },
        close: function (event, ui) {
        },
        minLength: 2,
        focus: function () {
        },
        select: function () { },
        scroll: !0
    });

});

function goBack() {
    window.history.back();
}

function getSuggestion() {
    var searchString = $('#searchString').val();
    $.ajax({
        url: '/Search/SpellCheck',
        type: 'POST',
        data: {
            searchString: searchString
        },
        success: function (response) {
            //if there's suggest words, show it
            if (response.length > 0) {
                var suggestions = "Do you meant to search: ";
                $.each(response, function (i, val) {
                    suggestions += " " + val;
                });
                $('#suggestions').text(suggestions);
            }
                //otherwise, reset suggestion tip
            else {
                $('#suggestions').text("");
            }
        }
    });
}

