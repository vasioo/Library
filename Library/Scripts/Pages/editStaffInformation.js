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
            var formData = {
                Id: $('#Id').val(),
                Salary: $('#Salary').val(),
                Position: $('#Position').val(),

            };
            $.post({
                url: '/Admin/EditInfo',
                data: formData,
                beforeSend: function (xhr) {
                    var token = $('input[name="__RequestVerificationToken"]').val();
                    xhr.setRequestHeader("RequestVerificationToken", token);
                },
                success: function (response) {
                    if (response.status) {
                        Swal.fire("Успех", response.message, "success");
                    } else {
                        Swal.fire("Грешка", response.errors, "error");
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire("Грешка", xhr.responseText, "error");
                }
            });

        });

    }
    return {
        init
    };
})();