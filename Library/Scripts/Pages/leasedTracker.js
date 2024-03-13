var leasedTracker = (function () {
    function init($container) {
        $('#deleteBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/DeleteUserLeasedEntity",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    commonFuncs.endLoader();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            }); commonFuncs.endLoader();
        });

        $('#stopLeasingBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/StopLeasing",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    commonFuncs.endLoader();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            }); commonFuncs.endLoader();
        });

        $('#leaseBookBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: true },
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire("Успех", response.message, "success");
                        location.reload();
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire("Грешка", response.message, "error");
                    }
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            }); commonFuncs.endLoader();
        });

        $('#rejectLeaseBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: false },
                success: function (response) {
                    commonFuncs.endLoader();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            }); commonFuncs.endLoader();
        }); 
    }
    return {
        init
    };
})();