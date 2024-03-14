var editStaffInformation = (function () {
    function init($container) {
        $('#Position').change(function () {
            if ($(this).val() === 'Потребител') {
                $('#Salary').prop('disabled', true).val('');
            } else {
                $('#Salary').prop('disabled', false);
            }
        });
        $('#submitButton').click(function () {
            commonFuncs.startLoader();
            var formData = {
                Id: $('#Id').val(),
                Salary: $('#Salary').val(),
                Position: $('#Position').val(),
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            };

            $.ajax({
                type: "POST",
                url: '/Admin/EditInfo',
                data: formData,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("RequestVerificationToken", formData.__RequestVerificationToken);
                },
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire("Успех", response.message, "success");
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire("Грешка", response.errors, "error");
                    }
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    Swal.fire("Грешка", xhr.responseText, "error");
                }
            });
        });
    }
    return {
        init
    };
})();