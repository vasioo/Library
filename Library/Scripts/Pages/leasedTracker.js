var leasedTracker = (function () {
    function init($container) {
        $('#deleteBtn, #stopLeasingBtn, #leaseBookBtn, #rejectLeaseBtn').on('click', function (e) {
            e.preventDefault();
            var url = $(this).attr('href');
            var userLeasedId = $(this).data('userLeasedId');

            $.ajax({
                type: 'GET',
                url: url,
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });
    }
    return {
        init
    };
})();