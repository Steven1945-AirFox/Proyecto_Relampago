// Global variables
let currentStep = 1
const totalSteps = 14 // Total number of form steps
let formData = {}

// DOM Content Loaded
document.addEventListener("DOMContentLoaded", () => {
    initializeApp()
})

// Initialize Application
function initializeApp() {
    initializeNavigation()
    initializeAccordions()
    initializeForms()
    initializeFormStepper()
    initializeValidation()
    initializeCurrencyInputs()
    initializeConditionalFields()
    loadFormData()
}

// Navigation
function initializeNavigation() {
    const navToggle = document.querySelector(".nav__toggle")
    const navMenu = document.querySelector(".nav__menu")

    if (navToggle && navMenu) {
        navToggle.addEventListener("click", () => {
            const isOpen = navMenu.classList.contains("nav__menu--open")

            if (isOpen) {
                navMenu.classList.remove("nav__menu--open")
                navToggle.setAttribute("aria-expanded", "false")
            } else {
                navMenu.classList.add("nav__menu--open")
                navToggle.setAttribute("aria-expanded", "true")
            }
        })

        // Close menu when clicking outside
        document.addEventListener("click", (e) => {
            if (!navToggle.contains(e.target) && !navMenu.contains(e.target)) {
                navMenu.classList.remove("nav__menu--open")
                navToggle.setAttribute("aria-expanded", "false")
            }
        })
    }
}

// Accordions
function initializeAccordions() {
    const accordionButtons = document.querySelectorAll(".accordion__button")

    accordionButtons.forEach((button) => {
        button.addEventListener("click", function () {
            const content = document.getElementById(this.getAttribute("aria-controls"))
            const isExpanded = this.getAttribute("aria-expanded") === "true"

            // Close all other accordions
            accordionButtons.forEach((otherButton) => {
                if (otherButton !== this) {
                    otherButton.setAttribute("aria-expanded", "false")
                    const otherContent = document.getElementById(otherButton.getAttribute("aria-controls"))
                    if (otherContent) {
                        otherContent.style.display = "none"
                    }
                }
            })

            // Toggle current accordion
            if (isExpanded) {
                this.setAttribute("aria-expanded", "false")
                content.style.display = "none"
            } else {
                this.setAttribute("aria-expanded", "true")
                content.style.display = "block"
            }
        })
    })
}

// Forms
function initializeForms() {
    // Contact form
    const contactForm = document.getElementById("contact-form")
    if (contactForm) {
        contactForm.addEventListener("submit", handleContactFormSubmit)
    }

    // Scholarship form
    const scholarshipForm = document.getElementById("scholarship-form")
    if (scholarshipForm) {
        scholarshipForm.addEventListener("submit", handleScholarshipFormSubmit)
    }

    // Age calculation
    const birthDateInput = document.getElementById("fecha-nacimiento")
    const ageInput = document.getElementById("edad")

    if (birthDateInput && ageInput) {
        birthDateInput.addEventListener("change", function () {
            const birthDate = new Date(this.value)
            const today = new Date()
            let age = today.getFullYear() - birthDate.getFullYear()
            const monthDiff = today.getMonth() - birthDate.getMonth()

            if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
                age--
            }

            ageInput.value = age >= 0 ? age : ""
        })
    }
}

// Form Stepper
function initializeFormStepper() {
    const nextBtn = document.getElementById("next-btn")
    const prevBtn = document.getElementById("prev-btn")
    const submitBtn = document.getElementById("submit-btn")
    const saveDraftBtn = document.getElementById("save-draft-btn")
    const loadDraftBtn = document.getElementById("load-draft-btn")
    const printBtn = document.getElementById("print-btn")

    if (nextBtn) {
        nextBtn.addEventListener("click", nextStep)
    }

    if (prevBtn) {
        prevBtn.addEventListener("click", prevStep)
    }

    if (saveDraftBtn) {
        saveDraftBtn.addEventListener("click", saveFormDraft)
    }

    if (loadDraftBtn) {
        loadDraftBtn.addEventListener("click", loadFormDraft)
    }

    if (printBtn) {
        printBtn.addEventListener("click", printForm)
    }

    updateProgressBar()
    updateStepVisibility()
    generateProgressSteps()
}

function nextStep() {
    if (validateCurrentStep()) {
        saveCurrentStepData()
        if (currentStep < totalSteps) {
            currentStep++
            updateStepVisibility()
            updateProgressBar()
            updateNavigationButtons()
            scrollToTop()
        }
    }
}

function prevStep() {
    if (currentStep > 1) {
        saveCurrentStepData()
        currentStep--
        updateStepVisibility()
        updateProgressBar()
        updateNavigationButtons()
        scrollToTop()
    }
}

function updateStepVisibility() {
    const steps = document.querySelectorAll(".form-step")
    steps.forEach((step, index) => {
        if (index + 1 === currentStep) {
            step.style.display = "block"
            // Focus management for accessibility
            const firstInput = step.querySelector("input, select, textarea")
            if (firstInput) {
                setTimeout(() => firstInput.focus(), 100)
            }
        } else {
            step.style.display = "none"
        }
    })
}

function updateProgressBar() {
    const progressFill = document.getElementById("progress-fill")
    if (progressFill) {
        const percentage = (currentStep / totalSteps) * 100
        progressFill.style.width = percentage + "%"
    }
}

function updateNavigationButtons() {
    const nextBtn = document.getElementById("next-btn")
    const prevBtn = document.getElementById("prev-btn")
    const submitBtn = document.getElementById("submit-btn")

    if (prevBtn) {
        prevBtn.style.display = currentStep > 1 ? "inline-flex" : "none"
    }

    if (currentStep === totalSteps) {
        if (nextBtn) nextBtn.style.display = "none"
        if (submitBtn) submitBtn.style.display = "inline-flex"
    } else {
        if (nextBtn) nextBtn.style.display = "inline-flex"
        if (submitBtn) submitBtn.style.display = "none"
    }
}

function generateProgressSteps() {
    const progressSteps = document.getElementById("progress-steps")
    if (!progressSteps) return

    const stepNames = [
        "Identificación",
        "Datos Personales",
        "Info Académica",
        "Fuente Ingresos",
        "Bienes",
        "Vivienda",
        "Ingresos Familiares",
        "Egresos",
        "Declaración",
        "Puntos Relevantes",
        "Carta Compromiso",
        "Resolución",
        "Documentos",
        "Declaraciones",
    ]

    progressSteps.innerHTML = ""
    stepNames.forEach((name, index) => {
        const stepElement = document.createElement("div")
        stepElement.className = "progress-step"
        stepElement.textContent = name

        if (index + 1 === currentStep) {
            stepElement.classList.add("progress-step--active")
        } else if (index + 1 < currentStep) {
            stepElement.classList.add("progress-step--completed")
        }

        progressSteps.appendChild(stepElement)
    })
}

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: "smooth" })
}

// Validation
function initializeValidation() {
    // Real-time validation
    const inputs = document.querySelectorAll(".form-input, .form-select, .form-textarea")
    inputs.forEach((input) => {
        input.addEventListener("blur", function () {
            validateField(this)
        })

        input.addEventListener("input", function () {
            clearFieldError(this)
        })
    })

    // Cedula formatting and validation
    const cedulaInput = document.getElementById("cedula")
    if (cedulaInput) {
        cedulaInput.addEventListener("input", formatCedula)
        cedulaInput.addEventListener("blur", validateCedula)
    }
}

function validateField(field) {
    const value = field.value.trim()
    const fieldName = field.name
    let isValid = true
    let errorMessage = ""

    // Required field validation
    if (field.hasAttribute("required") && !value) {
        isValid = false
        errorMessage = "Este campo es obligatorio"
    }

    // Email validation
    if (field.type === "email" && value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(value)) {
            isValid = false
            errorMessage = "Ingrese un correo electrónico válido"
        }
    }

    // Phone validation
    if (field.type === "tel" && value) {
        const phoneRegex = /^[0-9\-\s+$$$$]+$/
        if (!phoneRegex.test(value) || value.length < 8) {
            isValid = false
            errorMessage = "Ingrese un número de teléfono válido"
        }
    }

    // Date validation
    if (field.type === "date" && value) {
        const date = new Date(value)
        const today = new Date()
        if (date > today) {
            isValid = false
            errorMessage = "La fecha no puede ser futura"
        }
    }

    displayFieldError(field, isValid ? "" : errorMessage)
    return isValid
}

function validateCedula() {
    const cedulaInput = document.getElementById("cedula")
    if (!cedulaInput) return true

    const cedula = cedulaInput.value.replace(/\D/g, "")
    let isValid = true
    let errorMessage = ""

    if (cedula.length !== 9) {
        isValid = false
        errorMessage = "La cédula debe tener 9 dígitos"
    } else {
        // Basic Costa Rican cedula validation
        const firstDigit = Number.parseInt(cedula[0])
        if (firstDigit < 1 || firstDigit > 9) {
            isValid = false
            errorMessage = "Formato de cédula inválido"
        }
    }

    displayFieldError(cedulaInput, errorMessage)
    return isValid
}

function formatCedula() {
    const cedulaInput = document.getElementById("cedula")
    if (!cedulaInput) return

    let value = cedulaInput.value.replace(/\D/g, "")

    if (value.length >= 1) {
        value = value.substring(0, 9)
        if (value.length > 1) {
            value = value[0] + "-" + value.substring(1)
        }
        if (value.length > 6) {
            value = value.substring(0, 6) + "-" + value.substring(6)
        }
    }

    cedulaInput.value = value
}

function validateCurrentStep() {
    const currentStepElement = document.getElementById(`step-${currentStep}`)
    if (!currentStepElement) return true

    const requiredFields = currentStepElement.querySelectorAll("[required]")
    let isValid = true

    requiredFields.forEach((field) => {
        if (!validateField(field)) {
            isValid = false
        }
    })

    if (!isValid) {
        showNotification("Por favor, complete todos los campos obligatorios", "error")
    }

    return isValid
}

function displayFieldError(field, message) {
    const errorElement = document.getElementById(field.id + "-error")
    if (errorElement) {
        errorElement.textContent = message
        errorElement.setAttribute("aria-live", "polite")
    }

    if (message) {
        field.classList.add("form-input--error")
        field.setAttribute("aria-invalid", "true")
        field.setAttribute("aria-describedby", field.id + "-error")
    } else {
        field.classList.remove("form-input--error")
        field.setAttribute("aria-invalid", "false")
        field.removeAttribute("aria-describedby")
    }
}

function clearFieldError(field) {
    displayFieldError(field, "")
}

// Currency Inputs
function initializeCurrencyInputs() {
    const currencyInputs = document.querySelectorAll(".currency-input")

    currencyInputs.forEach((input) => {
        input.addEventListener("blur", function () {
            formatCurrency(this)
        })

        input.addEventListener("focus", function () {
            // Remove formatting for editing
            this.value = this.value.replace(/[₡,\s]/g, "")
        })
    })
}

function formatCurrency(input) {
    const value = input.value.replace(/[^\d]/g, "")

    if (value) {
        // Format as Costa Rican currency
        const formatter = new Intl.NumberFormat("es-CR", {
            style: "currency",
            currency: "CRC",
            minimumFractionDigits: 0,
            maximumFractionDigits: 0,
        })

        input.value = formatter.format(Number.parseInt(value))
    }
}

// Conditional Fields
function initializeConditionalFields() {
    // Student condition fields
    const condicionRadios = document.querySelectorAll('input[name="condicion"]')
    condicionRadios.forEach((radio) => {
        radio.addEventListener("change", toggleCondicionFields)
    })

    // Different address during studies
    const direccionDiferente = document.getElementById("direccion-diferente")
    if (direccionDiferente) {
        direccionDiferente.addEventListener("change", function () {
            const section = document.getElementById("direccion-estudios-section")
            if (section) {
                section.style.display = this.checked ? "block" : "none"
            }
        })
    }
}

function toggleCondicionFields() {
    const selectedValue = document.querySelector('input[name="condicion"]:checked')?.value
    const becaAnteriorFields = document.getElementById("beca-anterior-fields")
    const cuatrimestreAnteriorFields = document.getElementById("cuatrimestre-anterior-fields")
    const anoAnteriorFields = document.getElementById("ano-anterior-fields")

    if (selectedValue === "regular-ya-tuvo") {
        if (becaAnteriorFields) becaAnteriorFields.style.display = "block"
        if (cuatrimestreAnteriorFields) cuatrimestreAnteriorFields.style.display = "block"
        if (anoAnteriorFields) anoAnteriorFields.style.display = "block"
    } else {
        if (becaAnteriorFields) becaAnteriorFields.style.display = "none"
        if (cuatrimestreAnteriorFields) cuatrimestreAnteriorFields.style.display = "none"
        if (anoAnteriorFields) anoAnteriorFields.style.display = "none"
    }
}

// Form Data Management
function saveCurrentStepData() {
    const currentStepElement = document.getElementById(`step-${currentStep}`)
    if (!currentStepElement) return

    const formElements = currentStepElement.querySelectorAll("input, select, textarea")

    formElements.forEach((element) => {
        if (element.type === "radio" || element.type === "checkbox") {
            if (element.checked) {
                formData[element.name] = element.value
            }
        } else {
            formData[element.name] = element.value
        }
    })
}

function loadFormData() {
    // Load from localStorage if available
    const savedData = localStorage.getItem("cuc-scholarship-form")
    if (savedData) {
        try {
            formData = JSON.parse(savedData)
            populateFormFields()
        } catch (e) {
            console.error("Error loading form data:", e)
        }
    }
}

function populateFormFields() {
    Object.keys(formData).forEach((fieldName) => {
        const field = document.querySelector(`[name="${fieldName}"]`)
        if (field) {
            if (field.type === "radio" || field.type === "checkbox") {
                if (field.value === formData[fieldName]) {
                    field.checked = true
                }
            } else {
                field.value = formData[fieldName]
            }
        }
    })

    // Trigger conditional field updates
    toggleCondicionFields()
}

function saveFormDraft() {
    saveCurrentStepData()
    localStorage.setItem("cuc-scholarship-form", JSON.stringify(formData))
    localStorage.setItem("cuc-scholarship-form-step", currentStep.toString())
    showNotification("Borrador guardado exitosamente", "success")
}

function loadFormDraft() {
    const savedStep = localStorage.getItem("cuc-scholarship-form-step")
    if (savedStep) {
        currentStep = Number.parseInt(savedStep)
        updateStepVisibility()
        updateProgressBar()
        updateNavigationButtons()
        generateProgressSteps()
    }

    loadFormData()
    showNotification("Borrador cargado exitosamente", "success")
}

// Form Submission
function handleContactFormSubmit(e) {
    e.preventDefault()

    const formData = new FormData(e.target)
    const data = Object.fromEntries(formData)

    // Validate form
    let isValid = true
    const requiredFields = e.target.querySelectorAll("[required]")

    requiredFields.forEach((field) => {
        if (!validateField(field)) {
            isValid = false
        }
    })

    if (isValid) {
        // Simulate form submission
        setTimeout(() => {
            document.getElementById("contact-form").style.display = "none"
            document.getElementById("contact-success").style.display = "block"
        }, 1000)

        showNotification("Enviando mensaje...", "info")
    }
}

function handleScholarshipFormSubmit(e) {
    e.preventDefault()

    saveCurrentStepData()

    // Validate all required fields
    if (validateAllSteps()) {
        // Simulate form submission
        showNotification("Formulario enviado exitosamente", "success")

        // Clear localStorage
        localStorage.removeItem("cuc-scholarship-form")
        localStorage.removeItem("cuc-scholarship-form-step")

        // Redirect or show success message
        setTimeout(() => {
            window.location.href = "index.html"
        }, 2000)
    } else {
        showNotification("Por favor, complete todos los campos obligatorios", "error")
    }
}

function validateAllSteps() {
    // This would validate all steps - simplified for demo
    return true
}

// Print Functionality
function printForm() {
    // Save current data
    saveCurrentStepData()

    // Create print-friendly version
    const printWindow = window.open("", "_blank")
    const printContent = generatePrintContent()

    printWindow.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Formulario de Solicitud de Beca - CUC</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                .header { text-align: center; margin-bottom: 30px; }
                .section { margin-bottom: 25px; page-break-inside: avoid; }
                .section-title { font-size: 18px; font-weight: bold; margin-bottom: 15px; border-bottom: 1px solid #000; }
                .field { margin-bottom: 10px; }
                .field-label { font-weight: bold; }
                .field-value { margin-left: 10px; }
                table { width: 100%; border-collapse: collapse; margin-top: 10px; }
                th, td { border: 1px solid #000; padding: 8px; text-align: left; }
                th { background-color: #f0f0f0; }
            </style>
        </head>
        <body>
            ${printContent}
        </body>
        </html>
    `)

    printWindow.document.close()
    printWindow.print()
}

function generatePrintContent() {
    return `
        <div class="header">
            <h1>COLEGIO UNIVERSITARIO DE CARTAGO</h1>
            <h2>Formulario de Solicitud de Beca Socioeconómica</h2>
        </div>
        
        <div class="section">
            <div class="section-title">I. Identificación de la Persona Solicitante</div>
            <div class="field">
                <span class="field-label">Nombre completo:</span>
                <span class="field-value">${formData.nombre || ""} ${formData.primerApellido || ""} ${formData.segundoApellido || ""}</span>
            </div>
            <div class="field">
                <span class="field-label">Cédula:</span>
                <span class="field-value">${formData.cedula || ""}</span>
            </div>
            <div class="field">
                <span class="field-label">Carrera:</span>
                <span class="field-value">${formData.carrera || ""}</span>
            </div>
        </div>
        
        <!-- Additional sections would be generated here -->
    `
}

// Notifications
function showNotification(message, type = "info") {
    // Create notification element
    const notification = document.createElement("div")
    notification.className = `notification notification--${type}`
    notification.innerHTML = `
        <span>${message}</span>
        <button class="notification__close" aria-label="Cerrar notificación">&times;</button>
    `

    // Add styles
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        border-radius: 5px;
        color: white;
        font-weight: 500;
        z-index: 1000;
        max-width: 300px;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 10px;
    `

    // Set background color based on type
    const colors = {
        success: "#10B981",
        error: "#EF4444",
        warning: "#F59E0B",
        info: "#3B82F6",
    }

    notification.style.backgroundColor = colors[type] || colors.info

    // Add to page
    document.body.appendChild(notification)

    // Close button functionality
    const closeBtn = notification.querySelector(".notification__close")
    closeBtn.style.cssText = `
        background: none;
        border: none;
        color: white;
        font-size: 20px;
        cursor: pointer;
        padding: 0;
        margin-left: 10px;
    `

    closeBtn.addEventListener("click", () => {
        notification.remove()
    })

    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove()
        }
    }, 5000)
}

// Utility Functions
function debounce(func, wait) {
    let timeout
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout)
            func(...args)
        }
        clearTimeout(timeout)
        timeout = setTimeout(later, wait)
    }
}

// Auto-save functionality
const autoSave = debounce(() => {
    if (document.getElementById("scholarship-form")) {
        saveFormDraft()
    }
}, 30000) // Auto-save every 30 seconds

// Add auto-save to form inputs
document.addEventListener("input", (e) => {
    if (e.target.closest("#scholarship-form")) {
        autoSave()
    }
})

// Accessibility improvements
document.addEventListener("keydown", (e) => {
    // Escape key closes mobile menu
    if (e.key === "Escape") {
        const navMenu = document.querySelector(".nav__menu")
        const navToggle = document.querySelector(".nav__toggle")

        if (navMenu && navMenu.classList.contains("nav__menu--open")) {
            navMenu.classList.remove("nav__menu--open")
            navToggle.setAttribute("aria-expanded", "false")
            navToggle.focus()
        }
    }
})

// Error handling
window.addEventListener("error", (e) => {
    console.error("Application error:", e.error)
    showNotification("Ha ocurrido un error. Por favor, recargue la página.", "error")
})

// Service worker registration (for offline functionality)
if ("serviceWorker" in navigator) {
    window.addEventListener("load", () => {
        // Service worker would be registered here for offline functionality
        // navigator.serviceWorker.register('/sw.js');
    })
}
