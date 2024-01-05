var prevScrollpos = window.pageYOffset;
var navbar = document.getElementById("navbar");

window.onscroll = function () {
    var currentScrollPos = window.pageYOffset;

    if (prevScrollpos > currentScrollPos) {
        navbar.classList.add("sticky");
        navbar.classList.remove("navbar-hidden");
    } else {
        navbar.classList.remove("sticky");
        navbar.classList.add("navbar-hidden");
    }

    prevScrollpos = currentScrollPos;
}


$(document).ready(function () {
    var signInContainer = $('.sign-in-container');
    var signUpContainer = $('.sign-up-container');
    var navBar = $('.navbar');

    $('#signIn').mouseenter(function () {
        signInContainer.css('display', 'block');
        navBar.css({
            'border-bottom': '1px solid #ccc',
            'box-shadow': '0 0 10px rgba(0, 0, 0, 0.7)'
        });
    });
    $(document).on('click', function (e) {
        if (!signInContainer.is(e.target) && signInContainer.has(e.target).length === 0 &&
            !signUpContainer.is(e.target) && signUpContainer.has(e.target).length === 0) {
            signInContainer.css('display', 'none');
            signUpContainer.css('display', 'none');
            navBar.css({
                'border-bottom': '',
                'box-shadow': ''
            });
            var passwordInput = $('#password');
            var type = passwordInput.attr('type') === 'password' ? 'text' : 'password';
            passwordInput.attr('type', type);
            $(this).toggleClass('fa-eye fa-eye-slash');

            var passwordSignUpInput = $('#SignUpPassword');
            var SignUptype = passwordSignUpInput.attr('type') === 'password' ? 'text' : 'password';
            passwordSignUpInput.attr('type', type);
            $(this).toggleClass('fa-eye fa-eye-slash');
        }
    });

});

$(document).ready(function () {
    $('#togglePassword').click(function (e) {
        e.preventDefault();
        var passwordInput = $('#password');
        var type = passwordInput.attr('type') === 'password' ? 'text' : 'password';
        passwordInput.attr('type', type);
        $(this).toggleClass('fa-eye fa-eye-slash');
    });
    $('#toggleSignUpPassword').click(function (e) {
        e.preventDefault();
        var passwordInput = $('#SignUpPassword');
        var type = passwordInput.attr('type') === 'password' ? 'text' : 'password';
        passwordInput.attr('type', type);
        $(this).toggleClass('fa-eye fa-eye-slash');
    });
});

$(document).ready(function () {
    var signUpContainer = $('.sign-up-container');
    var signInContainer = $('.sign-in-container');

    $('#open-sign-up-container').click(function () {
        signUpContainer.css('display', 'block');
        signInContainer.css('display', 'none');
    });

    $('#open-sign-in-container').click(function () {
        signInContainer.css('display', 'block');
        signUpContainer.css('display', 'none');
    });
});

$(document).ready(function () {
    function showSwalMessage(title, text, icon, timer) {
        Swal.fire({
            title: title,
            text: text,
            icon: icon,
            timer: timer,
            showConfirmButton: false
        }).then(function () {
            location.reload(); 
        });
    }

    function showSuccessLoginMessage() {
        showSwalMessage('Success', 'You have been successfully logged in.', 'success', 3000);
    }

    $('#signInForm').submit(function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    showSuccessLoginMessage();
                } else {
                    showSwalMessage('Error', response.message, 'error', 3000);
                }
            },
            error: function () {
                showSwalMessage('Error', 'An error occurred.', 'error', 3000);
            }
        });
    });

    $('#signUpForm').submit(function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    showSwalMessage('Success', 'You have successfully registered.', 'success', 1500);
                } else {
                    showSwalMessage('Error', response.message, 'error', 1500);
                }
            },
            error: function () {
                showSwalMessage('Error', 'An error occurred.', 'error', 1500);
            }
        });
    });
});