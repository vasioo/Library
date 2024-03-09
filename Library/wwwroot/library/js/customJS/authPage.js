const wrapper = document.querySelector('.wrapper');
const registerLink = document.querySelector('.register-link');
const loginLink = document.querySelector('.login-link');

registerLink.onclick = () => {
    wrapper.classList.add('active');
}

loginLink.onclick = () => {
    wrapper.classList.remove('active');
}

$('#register-expose-password').click(function () {
    var passwordInput = $('#register-password');
    var eyeIcon = $('#register-expose-password');

    if (passwordInput.attr('type') === 'password') {
        passwordInput.attr('type', 'text');
        eyeIcon.removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
        passwordInput.attr('type', 'password');
        eyeIcon.removeClass('fa-eye-slash').addClass('fa-eye');
    }
});

$('#login-expose-password').click(function () {
    var passwordInput = $('#login-password');
    var eyeIcon = $('#login-expose-password');

    if (passwordInput.attr('type') === 'password') {
        passwordInput.attr('type', 'text');
        eyeIcon.removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
        passwordInput.attr('type', 'password');
        eyeIcon.removeClass('fa-eye-slash').addClass('fa-eye');
    }
});

$(document).ready(function () {
    $('input').on('input', function () {
        $(this).closest('.input-box').removeClass('invalid');
    });
    $('#LoginEmail').on('input', function () {
        var emailLabel = $(this).closest('.input-box').find('label');
        emailLabel.text('Имейл');
    });
    $('#RegisterEmail').on('input', function () {
        var emailLabel = $(this).closest('.input-box').find('label');
        emailLabel.text('Имейл');
    });
    $('#RegisterUsername').on('input', function () {
        var emailLabel = $(this).closest('.input-box').find('label');
        emailLabel.text('Потребителско име');
    });
    $('#register-password').on('input', function () {
        var registerLabel = $(this).closest('.input-box').find('label');
        registerLabel.text('Парола');
    });
    $('#loginForm').submit(function (event) {
        event.preventDefault();

        var emailInput = $('#LoginEmail');
        var email = emailInput.val();


        if (!validateEmail(email)) {
            emailInput.closest('.input-box').addClass('invalid');
            var label = emailInput.closest('.input-box').find('label');
            label.text('Невалиден Имейл');
            return false;

        }

        var formData = $(this).serialize();

        $.ajax({
            url: '/Account/Login',
            type: 'POST',
            data: formData,
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message,
                        timer: 3000,
                        timerProgressBar: true,
                        showConfirmButton: false
                    }).then(function () {
                        window.location.href = '/';
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message
                    });
                    $('#loginForm input').each(function () {
                        $(this).closest('.input-box').addClass('invalid');
                    });

                }
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    });
    $('#registerForm').submit(function (event) {
        event.preventDefault();

        var emailInput = $('#RegisterEmail');
        var email = emailInput.val();

        if (!validateEmail(email)) {
            emailInput.closest('.input-box').addClass('invalid');
            var label = emailInput.closest('.input-box').find('label');
            label.text('Невалиден Имейл');
            return false;
        }

        var passwordInput = $('#register-password');
        var currpassword = passwordInput.val();

        if (!validatePassword(currpassword)) {
            passwordInput.closest('.input-box').addClass('invalid');
            var label = passwordInput.closest('.input-box').find('label');
            Swal.fire({
                icon: 'error',
                title: 'Грешка',
                html: '<div class="swal-text-wrapper row px-5 justify-content-start"><p class="row justify-content-start h5 fw-bold">Паролата трябва да съдържа:</p><ul class="text-start"><li>8 елемента общо</li><li>1 главна буква</li><li>1 малка буква</li><li>1 цифра</li></ul></div>',
                confirmButtonText: 'OK',
                customClass: {
                    htmlContainer: 'text-left'
                }
            });
            label.text('Невалидна Парола');
            return false;
        }

        var formData = $(this).serialize();

        $.ajax({
            url: '/Account/Register',
            type: 'POST',
            data: formData,
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'info',
                        title: 'Потвърждение',
                        text: response.message,
                        showConfirmButton: false,
                        allowOutsideClick: false
                    });
                    
                } else {
                    if (response.hasOwnProperty('usernameRelated') && response.usernameRelated === true) {
                        var usernameInput = $('#RegisterUsername');
                        var inputBox = usernameInput.closest('.input-box');
                        inputBox.addClass('invalid');
                        var label = inputBox.find('label');
                        label.text('Това потребителско име вече съществува');
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: response.message
                        });
                        $('#registerForm input').each(function () {
                            $(this).closest('.input-box').addClass('invalid');
                        });
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    });

});
function validateEmail(email) {
    var re = /\S+@\S+\.\S+/;
    return re.test(email);
}
function validatePassword(password) {
    var hasUpperCase = /[A-Z]/.test(password);
    var hasLowerCase = /[a-z]/.test(password);
    var hasDigits = /\d/.test(password);
    var isValidLength = password.length >= 8;

    return isValidLength && hasUpperCase && hasLowerCase && hasDigits;
}
