// Función para mostrar/ocultar contraseña
function togglePasswordVisibility(inputId, toggleId) {
    const passwordInput = document.getElementById(inputId)
    const toggleIcon = document.getElementById(toggleId)

    if (passwordInput && toggleIcon) {
        if (passwordInput.type === "password") {
            passwordInput.type = "text"
            toggleIcon.classList.remove("fa-eye")
            toggleIcon.classList.add("fa-eye-slash")
        } else {
            passwordInput.type = "password"
            toggleIcon.classList.remove("fa-eye-slash")
            toggleIcon.classList.add("fa-eye")
        }
    }
}

// Función para previsualizar imágenes antes de subirlas
function previewImage(input, previewId) {
    const preview = document.getElementById(previewId)

    if (preview && input.files && input.files[0]) {
        const reader = new FileReader()

        reader.onload = (e) => {
            preview.src = e.target.result
            preview.style.display = "block"
        }

        reader.readAsDataURL(input.files[0])
    }
}

// Inicializar tooltips de Bootstrap
document.addEventListener("DOMContentLoaded", () => {
    // Inicializar tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map((tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl))

    // Añadir listeners para previsualización de imágenes
    const imageInputs = document.querySelectorAll(".image-upload")
    imageInputs.forEach((input) => {
        const previewId = input.getAttribute("data-preview")
        if (previewId) {
            input.addEventListener("change", function () {
                previewImage(this, previewId)
            })
        }
    })
})

