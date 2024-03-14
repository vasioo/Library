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
    var navBar = $('.navbar');

    $('#signIn').mouseenter(function () {
        signInContainer.css('display', 'block');
        navBar.css({
            'border-bottom': '1px solid #ccc',
            'box-shadow': '0 0 10px rgba(0, 0, 0, 0.7)'
        });
    });
    $(document).on('click', function (e) {
        if (!signInContainer.is(e.target) && signInContainer.has(e.target).length === 0) {
            signInContainer.css('display', 'none');
            navBar.css({
                'border-bottom': '',
                'box-shadow': ''
            });
            var passwordInput = $('#password');
            var type = passwordInput.attr('type') === 'password' ? 'text' : 'password';
            passwordInput.attr('type', type);
            $(this).toggleClass('fa-eye fa-eye-slash');

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
        showSwalMessage('Успех', 'Вие влезнахте успешно в профила си.', 'success', 3000);
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
                    showSwalMessage('Грешка', response.message, 'error', 3000);
                }
            },
            error: function () {
                showSwalMessage('Грешка', 'Възникна грешка.', 'error', 3000);
            }
        });
    });

});


$('.facebookLink').click(function () {
    $('.facebookInput').click();
});

$('.googleLink').click(function () {
    $('.googleInput').click();
});
$('#menu-open').change(function () {
    if ($(this).prop('checked')) {
        $('main').addClass('menu-opened');
        $('body').css('overflow-y', 'hidden');
        $('main > div:first').css('pointer-events', 'none');
    } else {
        $('main').removeClass('menu-opened');
        $('body').css('overflow-y', 'auto');
        $('main > div:first').css('pointer-events', 'all');
    }
});
$('main').on('click', function (event) {
    var $menuContainer = $('.menu');
    if (!$(event.target).closest($menuContainer).length && !$(event.target).is('.menu-open-button')) {
        $('#menu-open').prop('checked', false);
        $('main').removeClass('menu-opened');
        $('main > div:first').css('pointer-events', 'all');
        $('body').css('overflow-y', 'auto');

    }
});