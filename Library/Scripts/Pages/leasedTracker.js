var leasedTracker = (function () {
    function init($container) {
        $('#deleteBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/DeleteUserLeasedEntity",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });

        $('#stopLeasingBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/StopLeasing",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });

        $('#leaseBookBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: true },
                success: function (response) {
                    if (response.status) {
                        Swal.fire("Успех", response.message, "success");
                    } else {
                        Swal.fire("Грешка", response.message, "error");
                    }
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });

        $('#rejectLeaseBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: false },
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