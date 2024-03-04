var reportPageLibrarian = (function () {
    function init($container) {
        $(document).on('change', '#genreSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#genre-personalized-row').show();
            } else {
                $('#genre-personalized-row').hide();
            }
        });
        $(document).on('change', '#bookSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#book-personalized-row').show();
            } else {
                $('#book-personalized-row').hide();
                var selectedBookTimeSpan = $(this).val();
            }
        });
    }
    return {
        init
    };
})();