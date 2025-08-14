
class FormValidator {
    constructor() {
        this.validators = {
            required: this.validateRequired,
            email: this.validateEmail,
            phone: this.validatePhone,
            cedula: this.validateCedula,
            date: this.validateDate,
            minLength: this.validateMinLength,
            maxLength: this.validateMaxLength,
            pattern: this.validatePattern,
        }
        this.init()
    }

    init() {
        this.initializeValidation()
        this.initializeCedulaFormatting()
        this.initializeCurrencyInputs()
        this.initializeAgeCalculation()
    }

    
    initializeValidation() {
        const inputs = document.querySelectorAll(".form-input, .form-select, .form-textarea")

        inputs.forEach((input) => {
            input.addEventListener("blur", () => {
                this.validateField(input)
            })

            input.addEventListener("input", () => {
                this.clearFieldError(input)
            })
        })
    }

   
    validateField(field) {
        const value = field.value.trim()
        const rules = this.getValidationRules(field)
        let isValid = true
        let errorMessage = ""

        for (const rule of rules) {
            const result = this.validators[rule.type](value, rule.params, field)
            if (!result.isValid) {
                isValid = false
                errorMessage = result.message
                break
            }
        }

        this.displayFieldError(field, errorMessage)
        return isValid
    }

    
    getValidationRules(field) {
        const rules = []

        
        if (field.hasAttribute("required")) {
            rules.push({ type: "required" })
        }

       
        switch (field.type) {
            case "email":
                if (field.value) rules.push({ type: "email" })
                break
            case "tel":
                if (field.value) rules.push({ type: "phone" })
                break
            case "date":
                if (field.value) rules.push({ type: "date" })
                break
        }

      
        if (field.id === "cedula" && field.value) {
            rules.push({ type: "cedula" })
        }

        
        if (field.hasAttribute("minlength")) {
            rules.push({
                type: "minLength",
                params: Number.parseInt(field.getAttribute("minlength")),
            })
        }

        if (field.hasAttribute("maxlength")) {
            rules.push({
                type: "maxLength",
                params: Number.parseInt(field.getAttribute("maxlength")),
            })
        }

     
        if (field.hasAttribute("pattern")) {
            rules.push({
                type: "pattern",
                params: field.getAttribute("pattern"),
            })
        }

        return rules
    }

    
    validateRequired(value) {
        return {
            isValid: value.length > 0,
            message: "Este campo es obligatorio",
        }
    }

    validateEmail(value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        return {
            isValid: emailRegex.test(value),
            message: "Ingrese un correo electrónico válido",
        }
    }

    validatePhone(value) {
        const phoneRegex = /^[0-9\-\s+()]+$/
        const cleanPhone = value.replace(/\D/g, "")
        return {
            isValid: phoneRegex.test(value) && cleanPhone.length >= 8,
            message: "Ingrese un número de teléfono válido (mínimo 8 dígitos)",
        }
    }

    validateCedula(value) {
        const cedula = value.replace(/\D/g, "")

        if (cedula.length !== 9) {
            return {
                isValid: false,
                message: "La cédula debe tener 9 dígitos",
            }
        }

        const firstDigit = Number.parseInt(cedula[0])
        if (firstDigit < 1 || firstDigit > 9) {
            return {
                isValid: false,
                message: "Formato de cédula inválido",
            }
        }

        return { isValid: true, message: "" }
    }

    validateDate(value) {
        const date = new Date(value)
        const today = new Date()

        if (isNaN(date.getTime())) {
            return {
                isValid: false,
                message: "Fecha inválida",
            }
        }

        if (date > today) {
            return {
                isValid: false,
                message: "La fecha no puede ser futura",
            }
        }

        return { isValid: true, message: "" }
    }

    validateMinLength(value, minLength) {
        return {
            isValid: value.length >= minLength,
            message: `Debe tener al menos ${minLength} caracteres`,
        }
    }

    validateMaxLength(value, maxLength) {
        return {
            isValid: value.length <= maxLength,
            message: `No puede exceder ${maxLength} caracteres`,
        }
    }

    validatePattern(value, pattern) {
        const regex = new RegExp(pattern)
        return {
            isValid: regex.test(value),
            message: "Formato inválido",
        }
    }

    // 
    displayFieldError(field, message) {
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

    clearFieldError(field) {
        this.displayFieldError(field, "")
    }

    // fromato Cedula 
    initializeCedulaFormatting() {
        const cedulaInput = document.getElementById("cedula")
        if (cedulaInput) {
            cedulaInput.addEventListener("input", () => {
                this.formatCedula(cedulaInput)
            })
        }
    }

    formatCedula(input) {
        let value = input.value.replace(/\D/g, "")

        if (value.length >= 1) {
            value = value.substring(0, 9)
            if (value.length > 1) {
                value = value[0] + "-" + value.substring(1)
            }
            if (value.length > 6) {
                value = value.substring(0, 6) + "-" + value.substring(6)
            }
        }

        input.value = value
    }

   
    initializeCurrencyInputs() {
        const currencyInputs = document.querySelectorAll(".currency-input")

        currencyInputs.forEach((input) => {
            input.addEventListener("blur", () => {
                this.formatCurrency(input)
            })

            input.addEventListener("focus", () => {
                
                input.value = input.value.replace(/[₡,\s]/g, "")
            })
        })
    }

    formatCurrency(input) {
        const value = input.value.replace(/[^\d]/g, "")

        if (value) {
            const formatter = new Intl.NumberFormat("es-CR", {
                style: "currency",
                currency: "CRC",
                minimumFractionDigits: 0,
                maximumFractionDigits: 0,
            })

            input.value = formatter.format(Number.parseInt(value))
        }
    }

    
    initializeAgeCalculation() {
        const birthDateInput = document.getElementById("fecha-nacimiento")
        const ageInput = document.getElementById("edad")

        if (birthDateInput && ageInput) {
            birthDateInput.addEventListener("change", () => {
                const birthDate = new Date(birthDateInput.value)
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

    
    validateContainer(container) {
        const fields = container.querySelectorAll(".form-input, .form-select, .form-textarea")
        let isValid = true

        fields.forEach((field) => {
            if (!this.validateField(field)) {
                isValid = false
            }
        })

        return isValid
    }


    validateRequiredFields(container) {
        const requiredFields = container.querySelectorAll("[required]")
        let isValid = true

        requiredFields.forEach((field) => {
            if (!this.validateField(field)) {
                isValid = false
            }
        })

        return isValid
    }
}


document.addEventListener("DOMContentLoaded", () => {
    if (document.querySelector("form")) {
        window.formValidator = new FormValidator()
    }
})
