function goBack() {
    window.history.back();
}

function getSuggestion() {
    var searchString = $("#searchString").val();
    $.ajax({
        url: '/Search/GetSuggestions',
        type: 'POST',
        data: {
            searchString: searchString
        },
        success: function (response) {
            console.log(response);
            var suggestions = "Do you mean:";
            $.each(response, function (i, val) {
                suggestions += " " + val;
            });
            $('#suggestions').text(suggestions);           
        }
    });
}