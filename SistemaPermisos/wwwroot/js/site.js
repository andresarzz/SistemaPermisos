// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Example for displaying TempData messages as alerts
document.addEventListener("DOMContentLoaded", () => {
    const successMessage = '@TempData["SuccessMessage"]'
    const errorMessage = '@TempData["ErrorMessage"]'

    if (successMessage && successMessage !== "") {
        alert(successMessage) // Or use a more sophisticated notification system
    }
    if (errorMessage && errorMessage !== "") {
        alert(errorMessage) // Or use a more sophisticated notification system
    }
})

// Example for handling file input display
document
    .querySelectorAll(".custom-file-input")
    .forEach((input) => {
        input.addEventListener("change", (e) => {
            var fileName = e.target.files[0].name
            var nextSibling = e.target.nextElementSibling
            if (nextSibling && nextSibling.classList.contains("custom-file-label")) {
                nextSibling.innerText = fileName
            }
        })
    })

    // Example for date pickers (if using a library like jQuery UI or flatpickr)
    // $(function () {
    //     $(".datepicker").datepicker({
    //         dateFormat: "yy-mm-dd"
    //     });
    // });

    // Example for time pickers (if using a library)
    // $(function () {
    //     $(".timepicker").timepicker({
    //         timeFormat: 'H:i',
    //         interval: 15,
    //         minTime: '08:00',
    //         maxTime: '18:00',
    //         defaultTime: '9',
    //         startTime: '08:00',
    //         dynamic: false,
    //         dropdown: true,
    //         scrollbar: true
    //     });
    // });

    // General form validation feedback (if not using client-side validation frameworks)
    ; (() => {
        window.addEventListener(
            "load",
            () => {
                var forms = document.getElementsByClassName("needs-validation")
                var validation = Array.prototype.filter.call(forms, (form) => {
                    form.addEventListener(
                        "submit",
                        (event) => {
                            if (form.checkValidity() === false) {
                                event.preventDefault()
                                event.stopPropagation()
                            }
                            form.classList.add("was-validated")
                        },
                        false,
                    )
                })
            },
            false,
        )
    })()
