// Contact form validation - NO SUBMISSION LOGIC
class ContactForm {
    constructor() {
        this.form = document.getElementById("contact-form")
        if (this.form) {
            this.init()
        }
    }

    init() {
        this.form.addEventListener("submit", (e) => {
            e.preventDefault()
            this.handleFormValidation()
        })

        this.form.addEventListener("reset", () => {
            this.clearAllErrors()
        })
    }

    handleFormValidation() {
        if (!window.formValidator) return

        const isValid = window.formValidator.validateContainer(this.form)

        if (isValid) {
            window.baseApp.showNotification("Formulario validado correctamente. Listo para envío.", "success")
            console.log("Contact form data ready for submission:", this.getFormData())

            // Show success message
            this.showSuccessMessage()
        } else {
            window.baseApp.showNotification("Por favor, corrija los errores en el formulario", "error")
        }
    }

    getFormData() {
        const formData = new FormData(this.form)
        return Object.fromEntries(formData)
    }

    showSuccessMessage() {
        this.form.style.display = "none"
        const successElement = document.getElementById("contact-success")
        if (successElement) {
            successElement.style.display = "block"
        }
    }

    clearAllErrors() {
        const errorElements = this.form.querySelectorAll(".form-error")
        errorElements.forEach((error) => {
            error.textContent = ""
        })

        const inputElements = this.form.querySelectorAll(".form-input, .form-select, .form-textarea")
        inputElements.forEach((input) => {
            input.classList.remove("form-input--error")
            input.setAttribute("aria-invalid", "false")
            input.removeAttribute("aria-describedby")
        })
    }
}

// Initialize contact form when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    if (document.getElementById("contact-form")) {
        new ContactForm()
    }
})
