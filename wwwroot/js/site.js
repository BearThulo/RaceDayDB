
document.addEventListener("DOMContentLoaded", function () {

    // Mobile navigation
    const menuButton = document.querySelector(".nav-toggle");
    const navigation = document.querySelector(".nav-links");

    if (menuButton && navigation) {

        menuButton.addEventListener("click", function () {

            const isOpen = navigation.classList.toggle("open");

            menuButton.setAttribute(
                "aria-expanded",
                isOpen.toString()
            );
        });
    }

    // Prevent accidental double submissions
    const forms = document.querySelectorAll(
        "form[data-disable-on-submit]"
    );

    forms.forEach(function (form) {

        form.addEventListener("submit", function () {

            const button = form.querySelector(
                'button[type="submit"]'
            );

            if (button) {
                button.disabled = true;
                button.textContent = "Please wait...";
            }
        });
    });

});
