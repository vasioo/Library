var manageMemberships = (function () {
    function init($container) {

        $container.on('click', '.addMembershipBtn', function () {
            $('#addMembershipModal').modal('show');
        });

        $container.on('click', '.editBtn', function () {
            var membershipId = $(this).data('id');
            var membershipName = $(this).data('name');
            var membershipStarterPoints = $(this).data('start');
            var membershipExitPoints = $(this).data('end');

            $('#membershipId').val(membershipId);
            $('#editMembershipName').val(membershipName);
            $('#editStarterNeededPoints').val(membershipStarterPoints);
            $('#editNeededAmountOfPoints').val(membershipExitPoints);

            $('#editMembershipModal').modal('show');
        });

        $container.on('click', '.deleteBtn', function () {
            var membershipItemId = $(this).data('id');
            Swal.fire({
                title: 'Сигурни ли сте че искате да изтриете членството ?',
                text: "След натискането на 'Потвърждавам' няма да можете да върнете членството!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Потвърждавам!'
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire({
                        title: 'Накъде искате да преместите съществуващите елементи в даденото членство?',
                        text:'Нагоре(в горната категория по точки), Надолу(в по-долната категория по точки)',
                        icon: 'question',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Нагоре',
                        cancelButtonText: 'Надолу'
                    }).then((transferResult) => {
                        var isUpperConfirmed = transferResult.isConfirmed;
                        $.ajax({
                            url: '/Admin/DeleteMembership',
                            type: 'POST',
                            data: { id: membershipItemId, upper: isUpperConfirmed },
                            success: function (response) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Успех',
                                    text: response.message,
                                    showConfirmButton: false,
                                    timer: 3000
                                }).then((result) => {
                                    location.reload();
                                });
                            },
                            error: function (xhr, status, error) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Грешка',
                                    text: 'Възникна грешка. Опитайте отново'
                                });
                            }
                        });
                    });
                }
            });
        });


        $('#addMembershipForm').submit(function (event) {
            event.preventDefault();

            var membershipName = $('#addMembershipName').val();
            var starterNeededPoints = parseInt($('#addStarterNeededPoints').val());
            var neededAmountOfPoints = parseInt($('#addNeededAmountOfPoints').val());

            if (membershipName === '' || isNaN(starterNeededPoints) || isNaN(neededAmountOfPoints) ||
                starterNeededPoints <0 || neededAmountOfPoints <= starterNeededPoints || neededAmountOfPoints < 0) {
                Swal.fire({
                    icon: 'error',
                    title: 'Грешка',
                    text: 'Моля въведете данните с адекватни стойности.'
                });
                return;
            }

            var formData = $(this).serialize();

            $.ajax({
                url: '/Admin/AddMembership',
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.status) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Успех',
                            text: response.message,
                            showConfirmButton: false, 
                            timer: 3000 
                        }).then((result) => {
                            location.reload();
                        });
                    
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Грешка',
                            text: response.message
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка',
                        text: 'Възникна грешка. Опитайте пак.'
                    });
                }
            });
        });

        $('#editMembershipForm').submit(function (event) {
            event.preventDefault();
            var membershipName = $('#editMembershipName').val().trim();
            var starterNeededPoints = parseInt($('#editStarterNeededPoints').val().trim());
            var neededAmountOfPoints = parseInt($('#editNeededAmountOfPoints').val().trim());

            // Check if input fields are valid
            if (membershipName === '' || isNaN(starterNeededPoints) || isNaN(neededAmountOfPoints) ||
                starterNeededPoints <= 0 || neededAmountOfPoints <= starterNeededPoints || neededAmountOfPoints < 0) {
                Swal.fire({

                    icon: 'error',
                    title: 'Грешка',
                    text: 'Моля въведете данните с адекватни стойности.'
                });
                return;
            }

            var formData = $(this).serialize();

            $.ajax({
                url: '/Admin/EditMembership',
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.status) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Успех',
                            text: response.message,
                            showConfirmButton: false,
                            timer: 3000
                        }).then((result) => {
                            location.reload();
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Грешка',
                            text: response.message
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка',
                        text: 'Възникна грешка. Опитайте пак.'
                    });
                }
            });
        });

    }
    return {
        init
    };
})();
