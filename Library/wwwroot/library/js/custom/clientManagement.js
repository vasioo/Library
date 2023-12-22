var clientManagement = (function () {
    $('#toggleAdminsAndWorkers').change(function () {
        $('#staffManagementForm').submit();
    });
    return {
        init
    };
})();