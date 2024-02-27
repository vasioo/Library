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