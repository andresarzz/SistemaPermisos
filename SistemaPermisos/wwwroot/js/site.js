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
function previewImage(input, previewId, placeholderId) {
    const preview = document.getElementById(previewId)
    const placeholder = placeholderId ? document.getElementById(placeholderId) : null

    if (preview && input.files && input.files[0]) {
        const reader = new FileReader()

        reader.onload = (e) => {
            preview.src = e.target.result
            preview.style.display = "block"
            if (placeholder) {
                placeholder.style.display = "none"
            }
        }

        reader.readAsDataURL(input.files[0])
    }
}

// Inicializar componentes cuando el DOM esté cargado
document.addEventListener("DOMContentLoaded", () => {
    // Inicializar tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    tooltipTriggerList.forEach((tooltipTriggerEl) => {
        new bootstrap.Tooltip(tooltipTriggerEl)
    })

    // Añadir listeners para previsualización de imágenes
    const imageInputs = document.querySelectorAll(".image-upload")
    imageInputs.forEach((input) => {
        const previewId = input.getAttribute("data-preview")
        const placeholderId = input.getAttribute("data-placeholder")
        if (previewId) {
            input.addEventListener("change", function () {
                previewImage(this, previewId, placeholderId)
            })
        }
    })

    // Inicializar dropdowns de Bootstrap
    var dropdownElementList = [].slice.call(document.querySelectorAll(".dropdown-toggle"))
    dropdownElementList.forEach((dropdownToggleEl) => {
        new bootstrap.Dropdown(dropdownToggleEl)
    })

    // Cerrar alertas automáticamente después de 5 segundos
    const alerts = document.querySelectorAll(".alert:not(.alert-permanent)")
    alerts.forEach((alert) => {
        setTimeout(() => {
            const closeButton = alert.querySelector(".btn-close")
            if (closeButton) {
                closeButton.click()
            } else {
                alert.classList.add("fade")
                setTimeout(() => {
                    alert.style.display = "none"
                }, 150)
            }
        }, 5000)
    })
})

