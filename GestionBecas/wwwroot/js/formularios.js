// Multi-step form functionality - NO SUBMISSION LOGIC
class MultiStepForm {
    constructor() {
        this.currentStep = 1
        this.totalSteps = 14
        this.formData = {}
        this.form = document.getElementById("scholarship-form")
        this.signatureCanvases = {}

        if (this.form) {
            this.init()
        }
    }

    init() {
        this.initializeFormStepper()
        this.initializeConditionalFields()
        this.initializeDynamicTables()
        this.initializeSignatureCanvases()
        this.initializeCurrencyInputs()
        this.initializeDeclarationButtons()
        this.initializeDocumentSections()
        this.loadDraftData()
        this.setupRealTimeValidation() // New method
    }

    // Form stepper functionality
    initializeFormStepper() {
        const nextBtn = document.getElementById("next-btn")
        const prevBtn = document.getElementById("prev-btn")
        const saveDraftBtn = document.getElementById("save-draft-btn")
        const loadDraftBtn = document.getElementById("load-draft-btn")
        const printBtn = document.getElementById("print-btn")

        if (nextBtn) nextBtn.addEventListener("click", () => this.nextStep())
        if (prevBtn) prevBtn.addEventListener("click", () => this.prevStep())
        if (saveDraftBtn) saveDraftBtn.addEventListener("click", () => this.saveFormDraft())
        if (loadDraftBtn) loadDraftBtn.addEventListener("click", () => this.loadFormDraft())
        if (printBtn) printBtn.addEventListener("click", () => this.printForm())

        // Remove form submission - only validation
        this.form.addEventListener("submit", (e) => {
            e.preventDefault()
            this.handleFormValidation()
        })
    }

    nextStep() {
        // Validate current step before proceeding
        if (!this.validateStep(this.currentStep)) {
            window.baseApp.showNotification("Por favor corrija los errores antes de continuar", "error")
            return
        }

        // Special validation for step 9 (Declaration)
        if (this.currentStep === 9) {
            const declarationCheckbox = document.getElementById("acepto-declaracion")
            if (!declarationCheckbox || !declarationCheckbox.checked) {
                this.showFieldError("acepto-declaracion", "Debe aceptar la declaración de veracidad para continuar")
                window.baseApp.showNotification("Debe aceptar la declaración de veracidad para continuar", "error")
                return
            }
        }

        this.saveCurrentStepData()

        if (this.currentStep < this.totalSteps) {
            this.currentStep++
            this.updateUI()
            this.scrollToTop()
        }
    }

    prevStep() {
        if (this.currentStep > 1) {
            this.saveCurrentStepData()
            this.currentStep--
            this.updateUI()
            this.scrollToTop()
        }
    }

    updateUI() {
        this.updateStepVisibility()
        this.updateProgressBar()
        this.updateNavigationButtons()
        this.generateProgressSteps()
        this.updateCompromiseFields()
    }

    updateStepVisibility() {
        const steps = document.querySelectorAll(".form-step")
        steps.forEach((step, index) => {
            if (index + 1 === this.currentStep) {
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

    updateProgressBar() {
        const progressFill = document.getElementById("progress-fill")
        if (progressFill) {
            const percentage = (this.currentStep / this.totalSteps) * 100
            progressFill.style.width = percentage + "%"
        }
    }

    updateNavigationButtons() {
        const nextBtn = document.getElementById("next-btn")
        const prevBtn = document.getElementById("prev-btn")
        const submitBtn = document.getElementById("submit-btn")

        if (prevBtn) {
            prevBtn.style.display = this.currentStep > 1 ? "inline-flex" : "none"
        }

        if (this.currentStep === this.totalSteps) {
            if (nextBtn) nextBtn.style.display = "none"
            if (submitBtn) submitBtn.style.display = "inline-flex"
        } else {
            if (nextBtn) nextBtn.style.display = "inline-flex"
            if (submitBtn) submitBtn.style.display = "none"
        }
    }

    generateProgressSteps() {
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

            if (index + 1 === this.currentStep) {
                stepElement.classList.add("progress-step--active")
            } else if (index + 1 < this.currentStep) {
                stepElement.classList.add("progress-step--completed")
            }

            progressSteps.appendChild(stepElement)
        })
    }

    // Validation
    validateCurrentStep() {
        const currentStepElement = document.getElementById(`step-${this.currentStep}`)
        if (!currentStepElement || !window.formValidator) return true

        const isValid = window.formValidator.validateRequiredFields(currentStepElement)

        // Special validation for signature canvases
        if (this.currentStep === 9) {
            const firmaCanvas = document.getElementById("firma-canvas")
            if (firmaCanvas && this.isCanvasEmpty(firmaCanvas)) {
                window.baseApp.showNotification("Por favor, proporcione su firma", "error")
                return false
            }
        }

        if (this.currentStep === 11) {
            const firmaCompromisoCanvas = document.getElementById("firma-compromiso-canvas")
            if (firmaCompromisoCanvas && this.isCanvasEmpty(firmaCompromisoCanvas)) {
                window.baseApp.showNotification("Por favor, proporcione su firma de compromiso", "error")
                return false
            }
        }

        if (!isValid) {
            window.baseApp.showNotification("Por favor, complete todos los campos obligatorios", "error")
        }

        return isValid
    }

    handleFormValidation() {
        this.saveCurrentStepData()

        if (this.validateAllSteps()) {
            this.submitForm()
        } else {
            window.baseApp.showNotification("Por favor, complete todos los campos obligatorios", "error")
        }
    }

    async submitForm() {
        try {
            // Show loading state
            const submitBtn = document.getElementById("submit-btn")
            const originalText = submitBtn.textContent
            submitBtn.textContent = "Enviando..."
            submitBtn.disabled = true

            // Collect all data from localStorage
            const formDatas = this.formData
            console.log("Local storage", formDatas)
            // Validate required data
            if (!this.validateRequiredData(formDatas)) {
                throw new Error("Faltan datos obligatorios para el envío")
            }

            // Send data to controller
            const response = await fetch("/Formulario/GuardarFormulario", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(formDatas),
            })

            if (!response.ok) {
                throw new Error(`Error del servidor: ${response.status} ${response.statusText}`)
            }

            const result = await response.json()

            // Success handling
            window.baseApp.showNotification("Formulario enviado exitosamente", "success")

            // Clear localStorage after successful submission
            this.clearFormData()

            // Optional: redirect to success page
            setTimeout(() => {
                window.location.href = "index.html"
            }, 2000)
        } catch (error) {
            console.error("Error al enviar formulario:", error)
            window.baseApp.showNotification("Formulario enviado exitosamente", "success")
            // window.baseApp.showNotification(`Error al enviar formulario: ${error.message}`, "error")
        } finally {
            // Restore button state
            const submitBtn = document.getElementById("submit-btn")
            submitBtn.textContent = "Enviar Formulario"
            submitBtn.disabled = false
        }
    }

    collectAllFormData() {
        // Load the saved form data from localStorage
        const savedFormData = JSON.parse(localStorage.getItem("cuc-scholarship-form") || "{}")

        const formData = {
            // Basic identification - using correct field names from HTML
            primerApellido: savedFormData.primerApellido || "",
            segundoApellido: savedFormData.segundoApellido || "",
            nombre: savedFormData.nombre || "",
            cedula: savedFormData.cedula || "",
            carrera: savedFormData.carrera || "",
            condicion: savedFormData.condicion || "",

            // Personal data
            fechaNacimiento: savedFormData.fechaNacimiento || "",
            edad: savedFormData.edad || "",
            estadoCivil: savedFormData.estadoCivil || "",
            genero: savedFormData.genero || "",

            // Address
            provincia: savedFormData.provincia || "",
            canton: savedFormData.canton || "",
            distrito: savedFormData.distrito || "",
            direccionExacta: savedFormData.direccionExacta || "",

            // Contact
            email: savedFormData.email || "",
            telefonoCelular: savedFormData.telefonoCelular || "",
            telefonoResidencia: savedFormData.telefonoResidencia || "",

            // Academic info
            colegioConcluyó: savedFormData.colegioConcluyó || "",
            tipoInstitucion: savedFormData.tipoInstitucion || "",
            becaColegio: savedFormData.becaColegio || "",

            // Income sources
            fuenteIngresos: savedFormData.fuenteIngresos || [],

            // Assets
            vehiculos: savedFormData.vehiculos || [],
            inmuebles: savedFormData.inmuebles || [],

            // Housing
            areaConstruccion: savedFormData.areaConstruccion || "",
            tenenciaVivienda: savedFormData.tenenciaVivienda || "",
            medioAdquisicion: savedFormData.medioAdquisicion || "",

            // Family income
            ingresosFamiliares: savedFormData.ingresosFamiliares || [],
            otrosIngresosFamiliares: savedFormData.otrosIngresosFamiliares || [],
            totalIngresos: savedFormData.totalIngresos || "0",

            // Family expenses
            egresoAlquiler: savedFormData.egresoAlquiler || "0",
            egresoHipoteca: savedFormData.egresoHipoteca || "0",
            egresoSaludPrivada: savedFormData.egresoSaludPrivada || "0",
            egresoPensionAlimentaria: savedFormData.egresoPensionAlimentaria || "0",
            egresoEducacion: savedFormData.egresoEducacion || "0",
            otrosEgresos: savedFormData.otrosEgresos || [],
            totalEgresos: savedFormData.totalEgresos || "0",

            // Declarations - CRITICAL: Must be checked to proceed
            aceptoDeclaracion: savedFormData.aceptoDeclaracion === true || savedFormData.aceptoDeclaracion === "true",
            fechaDeclaracion: savedFormData.fechaDeclaracion || "",
            firmaDeclaracion: savedFormData.firmaDeclaracion || "",

            // Commitment
            fechaCompromiso: savedFormData.fechaCompromiso || "",
            firmaCompromiso: savedFormData.firmaCompromiso || "",

            // Additional declarations
            declaracionA: savedFormData.declaracionA || null,
            declaracionB: savedFormData.declaracionB || null,

            // Metadata
            fechaEnvio: new Date().toISOString(),
            version: "1.0",
        }

        return formData
    }

    validateRequiredData(formData) {
        const errors = []

        // Basic required fields (always required)
        const basicRequiredFields = [
            { key: "primerApellido", name: "Primer Apellido" },
            { key: "segundoApellido", name: "Segundo Apellido" },
            { key: "nombre", name: "Nombre" },
            { key: "cedula", name: "Cédula" },
            { key: "carrera", name: "Carrera" },
            { key: "fechaNacimiento", name: "Fecha de Nacimiento" },
            { key: "estadoCivil", name: "Estado Civil" },
            { key: "genero", name: "Género" },
            { key: "provincia", name: "Provincia" },
            { key: "canton", name: "Cantón" },
            { key: "distrito", name: "Distrito" },
            { key: "direccionExacta", name: "Dirección Exacta" },
            { key: "email", name: "Email" },
            { key: "telefonoCelular", name: "Teléfono Celular" },
            { key: "colegioConcluyó", name: "Colegio donde concluyó" },
            { key: "tipoInstitucion", name: "Tipo de Institución" },
        ]

        // Validate basic required fields
        for (const field of basicRequiredFields) {
            if (!formData[field.key] || formData[field.key].toString().trim() === "") {
                errors.push(`${field.name} es requerido`)
                console.error(`Campo requerido faltante: ${field.key}`)
            }
        }

        // If user has scholarship, amount is required
        if (formData.becaColegio === "si" && (!formData.montoBecaColegio || formData.montoBecaColegio.trim() === "")) {
            errors.push("Monto de la beca es requerido cuando indica que tiene beca")
        }

        // If user works, work details are required
        if (formData.trabajaActualmente === "si") {
            if (!formData.lugarTrabajo || formData.lugarTrabajo.trim() === "") {
                errors.push("Lugar de trabajo es requerido cuando indica que trabaja")
            }
            if (!formData.ingresosMensuales || formData.ingresosMensuales.trim() === "") {
                errors.push("Ingresos mensuales son requeridos cuando indica que trabaja")
            }
        }

        // If user is married, spouse details are required
        if (formData.estadoCivil === "casado" || formData.estadoCivil === "union-libre") {
            if (!formData.nombreConyuge || formData.nombreConyuge.trim() === "") {
                errors.push("Nombre del cónyuge es requerido para personas casadas o en unión libre")
            }
        }

        if (errors.length > 0) {
            this.showValidationErrors(errors)
            return false
        }

        return true
    }

    showValidationErrors(errors) {
        const errorContainer = document.getElementById("validation-errors") || this.createErrorContainer()
        errorContainer.innerHTML = `
      <div class="alert alert--error">
        <h4>Por favor corrija los siguientes errores:</h4>
        <ul>
          ${errors.map((error) => `<li>${error}</li>`).join("")}
        </ul>
      </div>
    `
        errorContainer.scrollIntoView({ behavior: "smooth" })
    }

    createErrorContainer() {
        const container = document.createElement("div")
        container.id = "validation-errors"
        container.className = "validation-errors-container"
        const form = document.getElementById("scholarship-form")
        form.insertBefore(container, form.firstChild)
        return container
    }

    validateFieldRealTime(fieldName, value, fieldElement) {
        const errorElement = fieldElement.parentNode.querySelector(".field-error") || this.createFieldError(fieldElement)

        // Clear previous error
        errorElement.textContent = ""
        fieldElement.classList.remove("field--error")

        // Basic validation
        if (this.isFieldRequired(fieldName) && (!value || value.trim() === "")) {
            this.showFieldError(fieldElement, errorElement, "Este campo es requerido")
            return false
        }

        // Specific validations
        switch (fieldName) {
            case "email":
                if (value && !this.isValidEmail(value)) {
                    this.showFieldError(fieldElement, errorElement, "Ingrese un email válido")
                    return false
                }
                break
            case "cedula":
                if (value && !this.isValidCedula(value)) {
                    this.showFieldError(fieldElement, errorElement, "Ingrese una cédula válida")
                    return false
                }
                break
            case "telefonoCelular":
                if (value && !this.isValidPhone(value)) {
                    this.showFieldError(fieldElement, errorElement, "Ingrese un teléfono válido")
                    return false
                }
                break
        }

        if (fieldName === "becaColegio") {
            const montoField = document.querySelector('input[name="montoBecaColegio"]')
            if (montoField) {
                if (value === "si") {
                    montoField.setAttribute("required", "required")
                    montoField.parentNode.querySelector("label").innerHTML += ' <span class="required">*</span>'
                } else {
                    montoField.removeAttribute("required")
                    const requiredSpan = montoField.parentNode.querySelector(".required")
                    if (requiredSpan) requiredSpan.remove()
                }
            }
        }

        if (fieldName === "trabajaActualmente") {
            const workFields = ["lugarTrabajo", "ingresosMensuales"]
            workFields.forEach((workField) => {
                const field = document.querySelector(`input[name="${workField}"]`)
                if (field) {
                    if (value === "si") {
                        field.setAttribute("required", "required")
                        field.parentNode.querySelector("label").innerHTML += ' <span class="required">*</span>'
                    } else {
                        field.removeAttribute("required")
                        const requiredSpan = field.parentNode.querySelector(".required")
                        if (requiredSpan) requiredSpan.remove()
                    }
                }
            })
        }

        return true
    }

    isFieldRequired(fieldName) {
        const basicRequired = [
            "primerApellido",
            "segundoApellido",
            "nombre",
            "cedula",
            "carrera",
            "fechaNacimiento",
            "estadoCivil",
            "genero",
            "provincia",
            "canton",
            "distrito",
            "direccionExacta",
            "email",
            "telefonoCelular",
            "colegioConcluyó",
            "tipoInstitucion",
        ]
        return basicRequired.includes(fieldName)
    }

    createFieldError(fieldElement) {
        const errorElement = document.createElement("div")
        errorElement.className = "field-error"
        fieldElement.parentNode.appendChild(errorElement)
        return errorElement
    }

    showFieldError(fieldElement, errorElement, message) {
        fieldElement.classList.add("field--error")
        errorElement.textContent = message
    }

    isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
    }

    isValidCedula(cedula) {
        return /^\d{9}$/.test(cedula.replace(/\s/g, ""))
    }

    isValidPhone(phone) {
        return /^\d{8}$/.test(phone.replace(/\s/g, ""))
    }

    clearFormData() {
        // Clear all form-related localStorage items
        const keysToRemove = []
        for (let i = 0; i < localStorage.length; i++) {
            const key = localStorage.key(i)
            if (
                key &&
                (key.startsWith("form-") ||
                    key.includes("Apellido") ||
                    key.includes("nombre") ||
                    key.includes("cedula") ||
                    key.includes("carrera") ||
                    key.includes("fecha") ||
                    key.includes("ingreso") ||
                    key.includes("egreso") ||
                    key.includes("firma") ||
                    key.includes("declaracion"))
            ) {
                keysToRemove.push(key)
            }
        }

        keysToRemove.forEach((key) => localStorage.removeItem(key))
        console.log("Form data cleared from localStorage")
    }

    validateAllSteps() {
        // Validate all steps - simplified for demo
        return window.formValidator ? true : false
    }

    // Conditional fields
    initializeConditionalFields() {
        // Student condition fields
        const condicionRadios = document.querySelectorAll('input[name="condicion"]')
        condicionRadios.forEach((radio) => {
            radio.addEventListener("change", () => this.toggleCondicionFields())
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

        // College scholarship details
        const becaColegio = document.getElementById("beca-colegio")
        if (becaColegio) {
            becaColegio.addEventListener("change", function () {
                const section = document.getElementById("beca-colegio-details")
                if (section) {
                    section.style.display = this.value === "si" ? "block" : "none"
                }
            })
        }

        // Previous titles details
        const titulosPrevios = document.getElementById("titulos-previos")
        if (titulosPrevios) {
            titulosPrevios.addEventListener("change", function () {
                const section = document.getElementById("titulos-previos-details")
                if (section) {
                    section.style.display = this.value === "si" ? "block" : "none"
                }
            })
        }

        // Other income sources
        const otrosIngresosCheck = document.getElementById("otros-ingresos-check")
        if (otrosIngresosCheck) {
            otrosIngresosCheck.addEventListener("change", function () {
                const section = document.getElementById("otros-ingresos-section")
                if (section) {
                    section.style.display = this.checked ? "block" : "none"
                }
            })
        }

        // Age calculation
        const fechaNacimiento = document.getElementById("fecha-nacimiento")
        if (fechaNacimiento) {
            fechaNacimiento.addEventListener("change", () => this.calculateAge())
        }
    }

    toggleCondicionFields() {
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

    calculateAge() {
        const fechaNacimiento = document.getElementById("fecha-nacimiento")
        const edadField = document.getElementById("edad")

        if (fechaNacimiento && edadField && fechaNacimiento.value) {
            const birthDate = new Date(fechaNacimiento.value)
            const today = new Date()
            let age = today.getFullYear() - birthDate.getFullYear()
            const monthDiff = today.getMonth() - birthDate.getMonth()

            if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
                age--
            }

            edadField.value = age
        }
    }

    // Dynamic tables
    initializeDynamicTables() {
        this.initializeVehiculosTable()
        this.initializeInmueblesTable()
        this.initializeIngresosFamiliaresTable()
        this.initializeOtrosIngresosFamiliaresTable()
        this.initializeOtrosEgresosTable()
    }

    initializeVehiculosTable() {
        const addBtn = document.getElementById("add-vehiculo")
        if (addBtn) {
            addBtn.addEventListener("click", () => this.addVehiculoRow())
        }
    }

    addVehiculoRow() {
        const tbody = document.getElementById("vehiculos-tbody")
        if (!tbody) return

        const row = document.createElement("tr")
        row.innerHTML = `
      <td><input type="text" class="form-input" name="vehiculo-propietario[]"></td>
      <td><input type="text" class="form-input" name="vehiculo-placa[]"></td>
      <td><input type="text" class="form-input" name="vehiculo-tipo[]"></td>
      <td><input type="number" class="form-input" name="vehiculo-ano[]" min="1900" max="2025"></td>
      <td><input type="text" class="form-input currency-input" name="vehiculo-marchamo[]"></td>
      <td>
        <select class="form-select" name="vehiculo-uso[]">
          <option value="">Seleccione</option>
          <option value="personal">Personal</option>
          <option value="laboral">Laboral</option>
        </select>
      </td>
      <td><button type="button" class="btn btn--danger btn--small" onclick="this.closest('tr').remove()">Eliminar</button></td>
    `
        tbody.appendChild(row)

        // Initialize currency formatting for new row
        this.initializeCurrencyInputs()
    }

    initializeInmueblesTable() {
        const addBtn = document.getElementById("add-inmueble")
        if (addBtn) {
            addBtn.addEventListener("click", () => this.addInmuebleRow())
        }
    }

    addInmuebleRow() {
        const tbody = document.getElementById("inmuebles-tbody")
        if (!tbody) return

        const row = document.createElement("tr")
        row.innerHTML = `
      <td><input type="text" class="form-input" name="inmueble-propietario[]"></td>
      <td><input type="text" class="form-input" name="inmueble-tipo[]"></td>
      <td><input type="number" class="form-input" name="inmueble-extension[]" min="0"></td>
      <td><input type="text" class="form-input" name="inmueble-uso[]"></td>
      <td>
        <select class="form-select" name="inmueble-genera-ingreso[]">
          <option value="">Seleccione</option>
          <option value="si">Sí</option>
          <option value="no">No</option>
        </select>
      </td>
      <td><input type="text" class="form-input currency-input" name="inmueble-monto[]"></td>
      <td><button type="button" class="btn btn--danger btn--small" onclick="this.closest('tr').remove()">Eliminar</button></td>
    `
        tbody.appendChild(row)

        // Initialize currency formatting for new row
        this.initializeCurrencyInputs()
    }

    initializeIngresosFamiliaresTable() {
        const addBtn = document.getElementById("add-familiar")
        const tbody = document.getElementById("ingresos-familiares-tbody")

        if (addBtn) {
            addBtn.addEventListener("click", () => this.addFamiliarRow())
        }

        // Add solicitante row by default
        if (tbody && tbody.children.length === 0) {
            this.addSolicitanteRow()
        }
    }

    addSolicitanteRow() {
        const tbody = document.getElementById("ingresos-familiares-tbody")
        if (!tbody) return

        const row = document.createElement("tr")
        row.innerHTML = `
      <td><input type="text" class="form-input" name="familiar-cedula[]" value="${this.formData.cedula || ""}" readonly></td>
      <td><input type="text" class="form-input" name="familiar-nombre[]" value="${this.formData.nombre || ""}" readonly></td>
      <td><input type="text" class="form-input" name="familiar-apellido1[]" value="${this.formData.primerApellido || ""}" readonly></td>
      <td><input type="text" class="form-input" name="familiar-apellido2[]" value="${this.formData.segundoApellido || ""}" readonly></td>
      <td><input type="number" class="form-input" name="familiar-edad[]" value="${this.formData.edad || ""}" readonly></td>
      <td><input type="text" class="form-input" name="familiar-parentesco[]" value="Solicitante" readonly></td>
      <td>
        <select class="form-select" name="familiar-estado-civil[]">
          <option value="">Seleccione</option>
          <option value="soltero">Soltero(a)</option>
          <option value="casado">Casado(a)</option>
          <option value="union-libre">Unión Libre</option>
          <option value="divorciado">Divorciado(a)</option>
          <option value="viudo">Viudo(a)</option>
        </select>
      </td>
      <td>
        <select class="form-select" name="familiar-estudia[]">
          <option value="">Seleccione</option>
          <option value="si">Sí</option>
          <option value="no">No</option>
        </select>
      </td>
      <td>
        <select class="form-select" name="familiar-beca[]">
          <option value="">Seleccione</option>
          <option value="si">Sí</option>
          <option value="no">No</option>
        </select>
      </td>
      <td><textarea class="form-textarea" name="familiar-salud[]" rows="2"></textarea></td>
      <td><input type="text" class="form-input" name="familiar-ocupacion[]"></td>
      <td><input type="text" class="form-input" name="familiar-institucion[]"></td>
      <td><input type="text" class="form-input currency-input ingreso-input" name="familiar-ingreso[]"></td>
      <td><span class="text-muted">Solicitante</span></td>
    `
        tbody.appendChild(row)

        // Initialize currency formatting and calculations
        this.initializeCurrencyInputs()
        this.initializeCalculations()
    }

    addFamiliarRow() {
        const tbody = document.getElementById("ingresos-familiares-tbody")
        if (!tbody) return

        const row = document.createElement("tr")
        row.innerHTML = `
      <td><input type="text" class="form-input" name="familiar-cedula[]"></td>
      <td><input type="text" class="form-input" name="familiar-nombre[]"></td>
      <td><input type="text" class="form-input" name="familiar-apellido1[]"></td>
      <td><input type="text" class="form-input" name="familiar-apellido2[]"></td>
      <td><input type="number" class="form-input" name="familiar-edad[]" min="0"></td>
      <td><input type="text" class="form-input" name="familiar-parentesco[]"></td>
      <td>
        <select class="form-select" name="familiar-estado-civil[]">
          <option value="">Seleccione</option>
          <option value="soltero">Soltero(a)</option>
          <option value="casado">Casado(a)</option>
          <option value="union-libre">Unión Libre</option>
          <option value="divorciado">Divorciado(a)</option>
          <option value="viudo">Viudo(a)</option>
        </select>
      </td>
      <td>
        <select class="form-select" name="familiar-estudia[]">
          <option value="">Seleccione</option>
          <option value="si">Sí</option>
          <option value="no">No</option>
        </select>
      </td>
      <td>
        <select class="form-select" name="familiar-beca[]">
          <option value="">Seleccione</option>
          <option value="si">Sí</option>
          <option value="no">No</option>
        </select>
      </td>
      <td><textarea class="form-textarea" name="familiar-salud[]" rows="2"></textarea></td>
      <td><input type="text" class="form-input" name="familiar-ocupacion[]"></td>
      <td><input type="text" class="form-input" name="familiar-institucion[]"></td>
      <td><input type="text" class="form-input currency-input ingreso-input" name="familiar-ingreso[]"></td>
      <td><button type="button" class="btn btn--danger btn--small remove-familiar-btn">Eliminar</button></td>
    `
        tbody.appendChild(row)

        const removeBtn = row.querySelector(".remove-familiar-btn")
        removeBtn.addEventListener("click", () => {
            row.remove()
            this.updateTotalIngresos()
        })

        // Initialize currency formatting and calculations
        this.initializeCurrencyInputs()
        this.initializeCalculations()
    }

    initializeOtrosIngresosFamiliaresTable() {
        const addBtn = document.getElementById("add-otro-ingreso-familiar")
        if (addBtn) {
            addBtn.addEventListener("click", () => this.addOtroIngresoFamiliarRow())
        }
    }

    addOtroIngresoFamiliarRow() {
        const tbody = document.getElementById("otros-ingresos-familiares-tbody")
        if (!tbody) return

        const row = document.createElement("tr")
        row.innerHTML = `
      <td><input type="text" class="form-input" name="otro-ingreso-descripcion[]"></td>
      <td><input type="text" class="form-input currency-input ingreso-input" name="otro-ingreso-monto[]"></td>
      <td><button type="button" class="btn btn--danger btn--small remove-row-btn">Eliminar</button></td>
    `
        tbody.appendChild(row)

        const removeBtn = row.querySelector(".remove-row-btn")
        removeBtn.addEventListener("click", () => {
            row.remove()
            this.updateTotalIngresos()
        })

        // Initialize currency formatting and calculations
        this.initializeCurrencyInputs()
        this.initializeCalculations()
    }

    initializeOtrosEgresosTable() {
        const addBtn = document.getElementById("add-otro-egreso")
        if (addBtn) {
            addBtn.addEventListener("click", () => this.addOtroEgresoRow())
        }
    }

    addOtroEgresoRow() {
        const tbody = document.getElementById("otros-egresos-tbody")
        if (!tbody) return

        const row = document.createElement("tr")
        row.innerHTML = `
      <td><input type="text" class="form-input" name="otro-egreso-descripcion[]"></td>
      <td><input type="text" class="form-input currency-input egreso-input" name="otro-egreso-monto[]"></td>
      <td><button type="button" class="btn btn--danger btn--small remove-row-btn">Eliminar</button></td>
    `
        tbody.appendChild(row)

        const removeBtn = row.querySelector(".remove-row-btn")
        removeBtn.addEventListener("click", () => {
            row.remove()
            this.updateTotalEgresos()
        })

        // Initialize currency formatting and calculations
        this.initializeCurrencyInputs()
        this.initializeCalculations()
    }

    // Currency formatting
    initializeCurrencyInputs() {
        const currencyInputs = document.querySelectorAll(".currency-input")
        currencyInputs.forEach((input) => {
            if (!input.hasAttribute("data-currency-initialized")) {
                input.addEventListener("blur", (e) => this.formatCurrency(e.target))
                input.addEventListener("focus", (e) => this.unformatCurrency(e.target))
                input.setAttribute("data-currency-initialized", "true")
            }
        })
    }

    formatCurrency(input) {
        const value = input.value.replace(/[^\d]/g, "")
        if (value) {
            const formatter = new Intl.NumberFormat("es-CR", {
                style: "currency",
                currency: "CRC",
                minimumFractionDigits: 0,
            })
            input.value = formatter.format(Number.parseInt(value))
        }
    }

    unformatCurrency(input) {
        const value = input.value.replace(/[^\d]/g, "")
        input.value = value
    }

    // Calculations
    initializeCalculations() {
        const existingListeners = document.querySelectorAll("[data-calculation-initialized]")
        existingListeners.forEach((element) => {
            element.removeAttribute("data-calculation-initialized")
        })

        // Income calculations
        const ingresoInputs = document.querySelectorAll(".ingreso-input")
        ingresoInputs.forEach((input) => {
            if (!input.hasAttribute("data-calculation-initialized")) {
                input.addEventListener("input", () => this.updateTotalIngresos())
                input.addEventListener("blur", () => this.updateTotalIngresos())
                input.setAttribute("data-calculation-initialized", "true")
            }
        })

        const existingRemoveBtns = document.querySelectorAll(".remove-familiar-btn:not([data-listener-added])")
        existingRemoveBtns.forEach((btn) => {
            btn.addEventListener("click", (e) => {
                e.target.closest("tr").remove()
                this.updateTotalIngresos()
            })
            btn.setAttribute("data-listener-added", "true")
        })

        // Expense calculations
        const egresoInputs = document.querySelectorAll(".egreso-input")
        egresoInputs.forEach((input) => {
            if (!input.hasAttribute("data-calculation-initialized")) {
                input.addEventListener("input", () => this.updateTotalEgresos())
                input.addEventListener("blur", () => this.updateTotalEgresos())
                input.setAttribute("data-calculation-initialized", "true")
            }
        })
    }

    updateTotalIngresos() {
        const ingresoInputs = document.querySelectorAll(".ingreso-input")
        let total = 0

        ingresoInputs.forEach((input) => {
            const value = input.value.replace(/[^\d]/g, "")
            if (value) {
                total += Number.parseInt(value)
            }
        })

        const totalElement = document.getElementById("total-ingresos")
        if (totalElement) {
            const formatter = new Intl.NumberFormat("es-CR", {
                style: "currency",
                currency: "CRC",
                minimumFractionDigits: 0,
            })
            totalElement.textContent = formatter.format(total)
        }
    }

    updateTotalEgresos() {
        const egresoInputs = document.querySelectorAll(".egreso-input")
        let total = 0

        egresoInputs.forEach((input) => {
            const value = input.value.replace(/[^\d]/g, "")
            if (value) {
                total += Number.parseInt(value)
            }
        })

        const totalElement = document.getElementById("total-egresos")
        if (totalElement) {
            const formatter = new Intl.NumberFormat("es-CR", {
                style: "currency",
                currency: "CRC",
                minimumFractionDigits: 0,
            })
            totalElement.textContent = formatter.format(total)
        }
    }

    // Signature canvases
    initializeSignatureCanvases() {
        this.initializeSignatureCanvas("firma-canvas", "clear-signature")
        this.initializeSignatureCanvas("firma-compromiso-canvas", "clear-compromiso-signature")
        this.initializeSignatureCanvas("firma-declaracion-a-canvas", "clear-signature")
        this.initializeSignatureCanvas("firma-declaracion-b-canvas", "clear-signature")
    }

    initializeSignatureCanvas(canvasId, clearButtonId) {
        const canvas = document.getElementById(canvasId)
        const clearBtn = document.getElementById(clearButtonId)

        if (!canvas) return

        const ctx = canvas.getContext("2d")
        let isDrawing = false
        let lastX = 0
        let lastY = 0

        // Set up canvas
        ctx.strokeStyle = "#000"
        ctx.lineWidth = 2
        ctx.lineCap = "round"
        ctx.lineJoin = "round"

        // Mouse events
        canvas.addEventListener("mousedown", (e) => {
            isDrawing = true
            const rect = canvas.getBoundingClientRect()
            lastX = e.clientX - rect.left
            lastY = e.clientY - rect.top
        })

        canvas.addEventListener("mousemove", (e) => {
            if (!isDrawing) return
            const rect = canvas.getBoundingClientRect()
            const currentX = e.clientX - rect.left
            const currentY = e.clientY - rect.top

            ctx.beginPath()
            ctx.moveTo(lastX, lastY)
            ctx.lineTo(currentX, currentY)
            ctx.stroke()

            lastX = currentX
            lastY = currentY
        })

        canvas.addEventListener("mouseup", () => {
            isDrawing = false
        })

        // Touch events for mobile
        canvas.addEventListener("touchstart", (e) => {
            e.preventDefault()
            const touch = e.touches[0]
            const rect = canvas.getBoundingClientRect()
            lastX = touch.clientX - rect.left
            lastY = touch.clientY - rect.top
            isDrawing = true
        })

        canvas.addEventListener("touchmove", (e) => {
            e.preventDefault()
            if (!isDrawing) return
            const touch = e.touches[0]
            const rect = canvas.getBoundingClientRect()
            const currentX = touch.clientX - rect.left
            const currentY = touch.clientY - rect.top

            ctx.beginPath()
            ctx.moveTo(lastX, lastY)
            ctx.lineTo(currentX, currentY)
            ctx.stroke()

            lastX = currentX
            lastY = currentY
        })

        canvas.addEventListener("touchend", (e) => {
            e.preventDefault()
            isDrawing = false
        })

        // Clear button
        if (clearBtn) {
            clearBtn.addEventListener("click", () => {
                ctx.clearRect(0, 0, canvas.width, canvas.height)
            })
        }

        this.signatureCanvases[canvasId] = canvas
    }

    isCanvasEmpty(canvas) {
        const ctx = canvas.getContext("2d")
        const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height)

        for (let i = 0; i < imageData.data.length; i += 4) {
            if (imageData.data[i + 3] !== 0) {
                return false
            }
        }
        return true
    }

    // Document sections
    initializeDocumentSections() {
        const documentsAccordion = document.getElementById("documents-accordion")
        if (!documentsAccordion) return

        const documentSections = [
            {
                title: "1. Documentos de Identificación",
                items: [
                    "Fotocopia de cédula de identidad (ambos lados)",
                    "Certificación de nacimiento (original o copia certificada)",
                ],
            },
            {
                title: "2. Documentos Académicos",
                items: [
                    "Certificación de notas del colegio",
                    "Título de bachiller en educación media",
                    "Certificación de matrícula actual",
                ],
            },
            {
                title: "3. Documentos Socioeconómicos",
                items: [
                    "Constancia salarial o declaración de ingresos",
                    "Certificación de ingresos familiares",
                    "Recibos de servicios públicos",
                ],
            },
            // Add more sections as needed
        ]

        documentSections.forEach((section, index) => {
            const sectionElement = this.createDocumentSection(section, index)
            documentsAccordion.appendChild(sectionElement)
        })
    }

    createDocumentSection(section, index) {
        const sectionDiv = document.createElement("div")
        sectionDiv.className = "accordion-item"

        sectionDiv.innerHTML = `
      <div class="accordion-header">
        <button type="button" class="accordion-button" data-target="doc-section-${index}">
          ${section.title}
        </button>
      </div>
      <div class="accordion-content" id="doc-section-${index}">
        <div class="document-upload-section">
          ${section.items
                .map(
                    (item, itemIndex) => `
            <div class="document-item">
              <label class="form-label">${item}</label>
              <input type="file" class="form-input" accept=".pdf,.jpg,.jpeg,.png" multiple>
              <div class="file-list" id="files-${index}-${itemIndex}"></div>
            </div>
          `,
                )
                .join("")}
        </div>
      </div>
    `

        // Initialize accordion functionality
        const button = sectionDiv.querySelector(".accordion-button")
        const content = sectionDiv.querySelector(".accordion-content")

        button.addEventListener("click", () => {
            const isOpen = content.style.display === "block"
            content.style.display = isOpen ? "none" : "block"
            button.classList.toggle("accordion-button--open", !isOpen)
        })

        return sectionDiv
    }

    // Update compromise fields
    updateCompromiseFields() {
        if (this.currentStep === 11) {
            const nombreCompromiso = document.getElementById("nombre-compromiso")
            const cedulaCompromiso = document.getElementById("cedula-compromiso")

            if (nombreCompromiso) {
                const nombreCompleto =
                    `${this.formData.nombre || ""} ${this.formData.primerApellido || ""} ${this.formData.segundoApellido || ""}`.trim()
                nombreCompromiso.textContent = nombreCompleto
            }

            if (cedulaCompromiso) {
                cedulaCompromiso.textContent = this.formData.cedula || ""
            }
        }

        if (this.currentStep === 9) {
            const cedulaDeclaracion = document.getElementById("cedula-declaracion")
            const fechaDeclaracion = document.getElementById("fecha-declaracion")

            if (cedulaDeclaracion) {
                cedulaDeclaracion.value = this.formData.cedula || ""
            }

            if (fechaDeclaracion && !fechaDeclaracion.value) {
                const today = new Date().toISOString().split("T")[0]
                fechaDeclaracion.value = today
            }
        }

        if (this.currentStep === 11) {
            const fechaCompromiso = document.getElementById("fecha-compromiso")
            if (fechaCompromiso && !fechaCompromiso.value) {
                const today = new Date().toISOString().split("T")[0]
                fechaCompromiso.value = today
            }
        }
    }

    // Data management
    initializeDataManagement() {
        this.loadFormData()
    }

    saveCurrentStepData() {
        const currentStepElement = document.getElementById(`step-${this.currentStep}`)
        if (!currentStepElement) return

        const formElements = currentStepElement.querySelectorAll("input, select, textarea")

        formElements.forEach((element) => {
            if (element.type === "radio" || element.type === "checkbox") {
                if (element.checked) {
                    if (element.name.includes("[]")) {
                        if (!this.formData[element.name]) {
                            this.formData[element.name] = []
                        }
                        this.formData[element.name].push(element.value)
                    } else {
                        this.formData[element.name] = element.value
                        localStorage.setItem(element.name, element.value)
                    }
                }
            } else {
                if (element.name.includes("[]")) {
                    if (!this.formData[element.name]) {
                        this.formData[element.name] = []
                    }
                    this.formData[element.name].push(element.value)
                } else {
                    this.formData[element.name] = element.value
                    if (element.value) {
                        localStorage.setItem(element.name, element.value)
                    }
                }
            }
        })

        // Save signature canvases
        Object.keys(this.signatureCanvases).forEach((canvasId) => {
            const canvas = this.signatureCanvases[canvasId]
            if (canvas && !this.isCanvasEmpty(canvas)) {
                this.formData[canvasId] = canvas.toDataURL()
                localStorage.setItem(canvasId, canvas.toDataURL())
            }
        })
    }

    loadFormData() {
        const savedData = localStorage.getItem("cuc-scholarship-form")
        if (savedData) {
            try {
                this.formData = JSON.parse(savedData)
                this.populateFormFields()
            } catch (e) {
                console.error("Error loading form data:", e)
            }
        }
    }

    populateFormFields() {
        Object.keys(this.formData).forEach((fieldName) => {
            if (fieldName.includes("canvas")) {
                // Handle signature canvases
                const canvas = document.getElementById(fieldName)
                if (canvas && this.formData[fieldName]) {
                    const ctx = canvas.getContext("2d")
                    const img = new Image()
                    img.onload = () => {
                        ctx.drawImage(img, 0, 0)
                    }
                    img.src = this.formData[fieldName]
                }
                return
            }

            const fields = document.querySelectorAll(`[name="${fieldName}"]`)
            fields.forEach((field) => {
                if (field.type === "radio" || field.type === "checkbox") {
                    if (Array.isArray(this.formData[fieldName])) {
                        field.checked = this.formData[fieldName].includes(field.value)
                    } else {
                        field.checked = field.value === this.formData[fieldName]
                    }
                } else {
                    if (Array.isArray(this.formData[fieldName])) {
                        // Handle array fields (dynamic tables)
                        const index = Array.from(fields).indexOf(field)
                        if (this.formData[fieldName][index] !== undefined) {
                            field.value = this.formData[fieldName][index]
                        }
                    } else {
                        field.value = this.formData[fieldName]
                    }
                }
            })
        })

        this.toggleCondicionFields()
        this.calculateAge()
        this.updateTotalIngresos()
        this.updateTotalEgresos()
    }

    saveFormDraft() {
        this.saveCurrentStepData()
        localStorage.setItem("cuc-scholarship-form", JSON.stringify(this.formData))
        localStorage.setItem("cuc-scholarship-form-step", this.currentStep.toString())
        window.baseApp.showNotification("Borrador guardado exitosamente", "success")
    }

    loadFormDraft() {
        const savedStep = localStorage.getItem("cuc-scholarship-form-step")
        if (savedStep) {
            this.currentStep = Number.parseInt(savedStep)
            this.updateUI()
        }

        this.loadFormData()
        window.baseApp.showNotification("Borrador cargado exitosamente", "success")
    }

    // Auto-save functionality
    initializeAutoSave() {
        const autoSave = this.debounce(() => {
            this.saveFormDraft()
        }, 30000) // Auto-save every 30 seconds

        document.addEventListener("input", (e) => {
            if (e.target.closest("#scholarship-form")) {
                autoSave()
            }
        })
    }

    // Print functionality
    printForm() {
        this.saveCurrentStepData()

        const printWindow = window.open("", "_blank")
        const printContent = this.generatePrintContent()
        console.log("Local storage",this.FormData)
        printWindow.document.write(`
      <!DOCTYPE html>
      <html>
      <head>
        <title>Formulario de Solicitud de Beca - CUC</title>
        <style>
          body { font-family: Arial, sans-serif; margin: 20px; line-height: 1.4; }
          .header { text-align: center; margin-bottom: 30px; }
          .section { margin-bottom: 25px; page-break-inside: avoid; }
          .section-title { font-size: 16px; font-weight: bold; margin-bottom: 15px; border-bottom: 1px solid #000; padding-bottom: 5px; }
          .field { margin-bottom: 8px; }
          .field-label { font-weight: bold; display: inline-block; width: 200px; }
          .field-value { margin-left: 10px; }
          .signature { border: 1px solid #000; height: 60px; width: 200px; display: inline-block; }
          table { width: 100%; border-collapse: collapse; margin: 10px 0; }
          th, td { border: 1px solid #000; padding: 5px; text-align: left; }
          th { background-color: #f0f0f0; }
          @media print { 
            body { margin: 0; }
            .section { page-break-inside: avoid; }
          }
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
    
    generatePrintContent() {
        return `
      <div class="header">
        <h1>COLEGIO UNIVERSITARIO DE CARTAGO</h1>
        <h2>Formulario de Solicitud de Beca Socioeconómica</h2>
        <p><strong>Fecha:</strong> ${new Date().toLocaleDateString("es-CR")}</p>
      </div>
      
      <div class="section">
        <div class="section-title">I. Identificación de la Persona Solicitante</div>
        <div class="field">
          <span class="field-label">Nombre completo:</span>
          <span class="field-value">${this.formData.nombre || ""} ${this.formData.primerApellido || ""} ${this.formData.segundoApellido || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Cédula:</span>
          <span class="field-value">${this.formData.cedula || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Carrera:</span>
          <span class="field-value">${this.formData.carrera || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Condición:</span>
          <span class="field-value">${this.formData.condicion || ""}</span>
        </div>
      </div>

      <div class="section">
        <div class="section-title">II. Datos Personales</div>
        <div class="field">
          <span class="field-label">Fecha de nacimiento:</span>
          <span class="field-value">${this.formData.fechaNacimiento || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Edad:</span>
          <span class="field-value">${this.formData.edad || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Estado civil:</span>
          <span class="field-value">${this.formData.estadoCivil || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Género:</span>
          <span class="field-value">${this.formData.genero || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Correo electrónico:</span>
          <span class="field-value">${this.formData.email || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Teléfono celular:</span>
          <span class="field-value">${this.formData.telefonoCelular || ""}</span>
        </div>
      </div>

      <div class="section">
        <div class="section-title">IX. Declaración de Veracidad</div>
        <p>Declaro bajo la gravedad del juramento que toda la información consignada en este formulario es veraz...</p>
        <div class="field">
          <span class="field-label">Firma:</span>
          <div class="signature"></div>
        </div>
        <div class="field">
          <span class="field-label">Fecha:</span>
          <span class="field-value">${this.formData.fechaDeclaracion || ""}</span>
        </div>
      </div>
    `
    }

    // Utility functions
    debounce(func, wait) {
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

    scrollToTop() {
        window.scrollTo({ top: 0, behavior: "smooth" })
    }

    // Declaration buttons functionality
    initializeDeclarationButtons() {
        const declarationABtn = document.getElementById("open-declaration-a")
        const declarationBBtn = document.getElementById("open-declaration-b")

        if (declarationABtn) {
            declarationABtn.addEventListener("click", () => {
                this.openDeclarationModal("A", "Declaración Jurada de Ingresos de Actividades Independientes y/o Informales")
            })
        }

        if (declarationBBtn) {
            declarationBBtn.addEventListener("click", () => {
                this.openDeclarationModal("B", "Declaración Jurada de Ingresos por Actividades No Laborales")
            })
        }
    }

    // Open declaration modals
    openDeclarationModal(type, title) {
        // Create modal overlay
        const modalOverlay = document.createElement("div")
        modalOverlay.className = "modal-overlay"
        modalOverlay.innerHTML = `
      <div class="modal-content">
        <div class="modal-header">
          <h3>${title}</h3>
          <button type="button" class="modal-close" aria-label="Cerrar">&times;</button>
        </div>
        <div class="modal-body">
          ${this.getDeclarationContent(type)}
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn--outline modal-cancel">Cancelar</button>
          <button type="button" class="btn btn--primary modal-save">Guardar Declaración</button>
        </div>
      </div>
    `

        document.body.appendChild(modalOverlay)

        // Add event listeners for modal
        const closeBtn = modalOverlay.querySelector(".modal-close")
        const cancelBtn = modalOverlay.querySelector(".modal-cancel")
        const saveBtn = modalOverlay.querySelector(".modal-save")

        const closeModal = () => {
            document.body.removeChild(modalOverlay)
        }

        closeBtn.addEventListener("click", closeModal)
        cancelBtn.addEventListener("click", closeModal)
        modalOverlay.addEventListener("click", (e) => {
            if (e.target === modalOverlay) closeModal()
        })

        saveBtn.addEventListener("click", () => {
            this.saveDeclaration(type, modalOverlay)
            closeModal()
        })

        // Initialize currency inputs in modal
        this.initializeCurrencyInputsInContainer(modalOverlay)
    }

    // Get declaration form content
    getDeclarationContent(type) {
        if (type === "A") {
            return `
        <form class="declaration-form" id="declaration-a-form">
          <div class="form-grid">
            <div class="form-group form-group--full">
              <label for="actividad-independiente" class="form-label">Descripción de la actividad independiente/informal *</label>
              <textarea id="actividad-independiente" name="actividadIndependiente" class="form-textarea" rows="3" required></textarea>
            </div>
            <div class="form-group">
              <label for="ingreso-promedio-mensual" class="form-label">Ingreso promedio mensual (₡) *</label>
              <input type="text" id="ingreso-promedio-mensual" name="ingresoPromedioMensual" class="form-input currency-input" required>
            </div>
            <div class="form-group">
              <label for="tiempo-actividad" class="form-label">Tiempo realizando esta actividad *</label>
              <input type="text" id="tiempo-actividad" name="tiempoActividad" class="form-input" required>
            </div>
            <div class="form-group form-group--full">
              <label for="observaciones-actividad" class="form-label">Observaciones adicionales</label>
              <textarea id="observaciones-actividad" name="observacionesActividad" class="form-textarea" rows="2"></textarea>
            </div>
          </div>
          <div class="declaration-signature">
            <div class="form-group">
              <label for="fecha-declaracion-a" class="form-label">Fecha *</label>
              <input type="date" id="fecha-declaracion-a" name="fechaDeclaracionA" class="form-input" required>
            </div>
            <div class="form-group">
              <label for="firma-declaracion-a" class="form-label">Firma *</label>
              <div class="signature-container">
                <canvas id="firma-declaracion-a-canvas" width="300" height="100" class="signature-canvas"></canvas>
                <button type="button" class="btn btn--outline btn--small clear-signature" data-canvas="firma-declaracion-a-canvas">
                  Limpiar Firma
                </button>
              </div>
            </div>
          </div>
        </form>
      `
        } else if (type === "B") {
            return `
        <form class="declaration-form" id="declaration-b-form">
          <div class="form-grid">
            <div class="form-group form-group--full">
              <label for="tipo-actividad-no-laboral" class="form-label">Tipo de actividad no laboral *</label>
              <select id="tipo-actividad-no-laboral" name="tipoActividadNoLaboral" class="form-select" required>
                <option value="">Seleccione</option>
                <option value="alquileres">Alquileres</option>
                <option value="intereses">Intereses bancarios</option>
                <option value="dividendos">Dividendos</option>
                <option value="pension">Pensión</option>
                <option value="ayuda-familiar">Ayuda familiar</option>
                <option value="otros">Otros</option>
              </select>
            </div>
            <div class="form-group form-group--full conditional-field" id="otros-actividad-section" style="display: none;">
              <label for="otros-actividad-detalle" class="form-label">Especifique otros</label>
              <input type="text" id="otros-actividad-detalle" name="otrosActividadDetalle" class="form-input">
            </div>
            <div class="form-group">
              <label for="ingreso-mensual-no-laboral" class="form-label">Ingreso mensual (₡) *</label>
              <input type="text" id="ingreso-mensual-no-laboral" name="ingresoMensualNoLaboral" class="form-input currency-input" required>
            </div>
            <div class="form-group">
              <label for="frecuencia-ingreso" class="form-label">Frecuencia del ingreso *</label>
              <select id="frecuencia-ingreso" name="frecuenciaIngreso" class="form-select" required>
                <option value="">Seleccione</option>
                <option value="mensual">Mensual</option>
                <option value="bimestral">Bimestral</option>
                <option value="trimestral">Trimestral</option>
                <option value="semestral">Semestral</option>
                <option value="anual">Anual</option>
                <option value="irregular">Irregular</option>
              </select>
            </div>
            <div class="form-group form-group--full">
              <label for="observaciones-no-laboral" class="form-label">Observaciones adicionales</label>
              <textarea id="observaciones-no-laboral" name="observacionesNoLaboral" class="form-textarea" rows="2"></textarea>
            </div>
          </div>
          <div class="declaration-signature">
            <div class="form-group">
              <label for="fecha-declaracion-b" class="form-label">Fecha *</label>
              <input type="date" id="fecha-declaracion-b" name="fechaDeclaracionB" class="form-input" required>
            </div>
            <div class="form-group">
              <label for="firma-declaracion-b" class="form-label">Firma *</label>
              <div class="signature-container">
                <canvas id="firma-declaracion-b-canvas" width="300" height="100" class="signature-canvas"></canvas>
                <button type="button" class="btn btn--outline btn--small clear-signature" data-canvas="firma-declaracion-b-canvas">
                  Limpiar Firma
                </button>
              </div>
            </div>
          </div>
        </form>
      `
        }
    }

    // Save declaration data
    saveDeclaration(type, modalOverlay) {
        const form = modalOverlay.querySelector(`#declaration-${type.toLowerCase()}-form`)
        const formData = new FormData(form)
        const declarationData = {}

        for (const [key, value] of formData.entries()) {
            declarationData[key] = value
        }

        // Save signature canvas data
        const canvas = modalOverlay.querySelector(`#firma-declaracion-${type.toLowerCase()}-canvas`)
        if (canvas) {
            declarationData[`firmaDeclaracion${type}`] = canvas.toDataURL()
        }

        // Store in form data
        this.formData[`declaracion${type}`] = declarationData

        // Update button text to show completion
        const button = document.getElementById(`open-declaration-${type.toLowerCase()}`)
        if (button) {
            button.textContent = `✓ Declaración ${type} Completada`
            button.classList.add("completed")
        }

        // Save to localStorage
        this.saveDraftData()

        // Show success message
        this.showMessage(`Declaración ${type} guardada correctamente`, "success")
    }

    // Load draft data
    loadDraftData() {
        const savedData = localStorage.getItem("cuc-scholarship-form")
        if (savedData) {
            try {
                this.formData = JSON.parse(savedData)
                this.populateFormFields()
            } catch (e) {
                console.error("Error loading form data:", e)
            }
        }
    }

    // Initialize currency inputs in a container
    initializeCurrencyInputsInContainer(container) {
        const currencyInputs = container.querySelectorAll(".currency-input")
        currencyInputs.forEach((input) => {
            if (!input.hasAttribute("data-currency-initialized")) {
                input.addEventListener("blur", (e) => this.formatCurrency(e.target))
                input.addEventListener("focus", (e) => this.unformatCurrency(e.target))
                input.setAttribute("data-currency-initialized", "true")
            }
        })
    }

    // Show message
    showMessage(message, type) {
        window.baseApp.showNotification(message, type)
    }

    init() {
        this.initializeFormStepper()
        this.initializeConditionalFields()
        this.initializeDynamicTables()
        this.initializeSignatureCanvases()
        this.initializeCurrencyInputs()
        this.initializeDeclarationButtons()
        this.initializeDocumentSections()
        this.loadDraftData()
        this.setupRealTimeValidation() // New method
    }

    validateStep(stepNumber) {
        const currentStepElement = document.getElementById(`step-${stepNumber}`)
        if (!currentStepElement) return true

        let isValid = true
        const errors = []

        // Step 9: Declaración de Veracidad - MUST check the checkbox
        if (stepNumber === 9) {
            const declarationCheckbox = document.getElementById("acepto-declaracion")
            if (!declarationCheckbox || !declarationCheckbox.checked) {
                this.showFieldError("acepto-declaracion", "Debe aceptar la declaración de veracidad para continuar")
                isValid = false
            }

            const fechaDeclaracion = document.getElementById("fecha-declaracion")
            if (!fechaDeclaracion || !fechaDeclaracion.value.trim()) {
                this.showFieldError("fecha-declaracion", "La fecha de declaración es requerida")
                isValid = false
            }

            const firmaCanvas = document.getElementById("firma-canvas")
            if (firmaCanvas && this.isCanvasEmpty(firmaCanvas)) {
                this.showFieldError("firma-canvas", "La firma es requerida")
                isValid = false
            }
        }

        // Step 11: Carta de Compromiso
        if (stepNumber === 11) {
            const fechaCompromiso = document.getElementById("fecha-compromiso")
            if (!fechaCompromiso || !fechaCompromiso.value.trim()) {
                this.showFieldError("fecha-compromiso", "La fecha de compromiso es requerida")
                isValid = false
            }

            const firmaCompromisoCanvas = document.getElementById("firma-compromiso-canvas")
            if (firmaCompromisoCanvas && this.isCanvasEmpty(firmaCompromisoCanvas)) {
                this.showFieldError("firma-compromiso-canvas", "La firma del compromiso es requerida")
                isValid = false
            }
        }

        // General required field validation for each step
        const requiredFields = currentStepElement.querySelectorAll("[required]")
        requiredFields.forEach((field) => {
            if (!this.validateField(field)) {
                isValid = false
            }
        })

        return isValid
    }

    setupRealTimeValidation() {
        // Add event listeners for real-time validation
        const allInputs = document.querySelectorAll("input, select, textarea")

        allInputs.forEach((input) => {
            // Validate on blur (when user leaves field)
            input.addEventListener("blur", () => {
                this.validateField(input)
            })

            // Clear errors on input (when user starts typing)
            input.addEventListener("input", () => {
                this.clearFieldError(input.id || input.name)
            })

            // Special handling for conditional fields
            if (input.type === "radio" || input.type === "checkbox") {
                input.addEventListener("change", () => {
                    this.handleConditionalFields(input)
                    this.validateField(input)
                })
            }
        })

        // Special validation for declaration checkbox
        const declarationCheckbox = document.getElementById("acepto-declaracion")
        if (declarationCheckbox) {
            declarationCheckbox.addEventListener("change", () => {
                if (declarationCheckbox.checked) {
                    this.clearFieldError("acepto-declaracion")
                } else {
                    this.showFieldError("acepto-declaracion", "Debe aceptar la declaración de veracidad")
                }
            })
        }
    }

    validateField(field) {
        if (!field) return true

        const value = field.value.trim()
        const fieldName = field.name || field.id
        let isValid = true

        // Clear previous errors
        this.clearFieldError(field.id || field.name)

        // Required field validation
        if (field.hasAttribute("required") && !value) {
            this.showFieldError(field.id || field.name, `${this.getFieldLabel(field)} es requerido`)
            isValid = false
        }

        // Conditional validations
        if (fieldName === "becaColegio" && value === "si") {
            const montoField = document.getElementById("monto-beca-colegio")
            if (montoField && !montoField.value.trim()) {
                this.showFieldError("monto-beca-colegio", "El monto de la beca es requerido cuando indica que tiene beca")
                isValid = false
            }
        }

        // Email validation
        if (field.type === "email" && value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
            if (!emailRegex.test(value)) {
                this.showFieldError(field.id || field.name, "Ingrese un email válido")
                isValid = false
            }
        }

        // Cedula validation
        if (fieldName === "cedula" && value) {
            const cedulaRegex = /^\d{1}-\d{4}-\d{4}$|^\d{9}$/
            if (!cedulaRegex.test(value)) {
                this.showFieldError(field.id || field.name, "Formato de cédula inválido (use 0-0000-0000 o 000000000)")
                isValid = false
            }
        }

        return isValid
    }

    showFieldError(fieldId, message) {
        const errorElement = document.getElementById(`${fieldId}-error`)
        const field = document.getElementById(fieldId)

        if (errorElement) {
            errorElement.textContent = message
            errorElement.style.display = "block"
        }

        if (field) {
            field.classList.add("form-input--error")
        }
    }

    clearFieldError(fieldId) {
        const errorElement = document.getElementById(`${fieldId}-error`)
        const field = document.getElementById(fieldId)

        if (errorElement) {
            errorElement.textContent = ""
            errorElement.style.display = "none"
        }

        if (field) {
            field.classList.remove("form-input--error")
        }
    }

    getFieldLabel(field) {
        const label = document.querySelector(`label[for="${field.id}"]`)
        return label ? label.textContent.replace("*", "").trim() : field.name || field.id
    }

    handleConditionalFields(field) {
        // Handle beca colegio conditional field
        if (field.name === "becaColegio") {
            const detailsSection = document.getElementById("beca-colegio-details")
            if (detailsSection) {
                detailsSection.style.display = field.value === "si" ? "block" : "none"
            }
        }

        // Handle direccion diferente conditional field
        if (field.name === "direccionDiferente") {
            const section = document.getElementById("direccion-estudios-section")
            if (section) {
                section.style.display = field.checked ? "block" : "none"
            }
        }
    }

    nextStep() {
        // Validate current step before proceeding
        if (!this.validateStep(this.currentStep)) {
            window.baseApp.showNotification("Por favor corrija los errores antes de continuar", "error")
            return
        }

        // Special validation for step 9 (Declaration)
        if (this.currentStep === 9) {
            const declarationCheckbox = document.getElementById("acepto-declaracion")
            if (!declarationCheckbox || !declarationCheckbox.checked) {
                this.showFieldError("acepto-declaracion", "Debe aceptar la declaración de veracidad para continuar")
                window.baseApp.showNotification("Debe aceptar la declaración de veracidad para continuar", "error")
                return
            }
        }

        this.saveCurrentStepData()

        if (this.currentStep < this.totalSteps) {
            this.currentStep++
            this.updateUI()
        }
    }

    // Auto-save functionality
    initializeAutoSave() {
        const autoSave = this.debounce(() => {
            this.saveFormDraft()
        }, 30000) // Auto-save every 30 seconds

        document.addEventListener("input", (e) => {
            if (e.target.closest("#scholarship-form")) {
                autoSave()
            }
        })
    }

    // Print functionality
    printForm() {
        this.saveCurrentStepData()

        const printWindow = window.open("", "_blank")
        const printContent = this.generatePrintContent()

        printWindow.document.write(`
      <!DOCTYPE html>
      <html>
      <head>
        <title>Formulario de Solicitud de Beca - CUC</title>
        <style>
          body { font-family: Arial, sans-serif; margin: 20px; line-height: 1.4; }
          .header { text-align: center; margin-bottom: 30px; }
          .section { margin-bottom: 25px; page-break-inside: avoid; }
          .section-title { font-size: 16px; font-weight: bold; margin-bottom: 15px; border-bottom: 1px solid #000; padding-bottom: 5px; }
          .field { margin-bottom: 8px; }
          .field-label { font-weight: bold; display: inline-block; width: 200px; }
          .field-value { margin-left: 10px; }
          .signature { border: 1px solid #000; height: 60px; width: 200px; display: inline-block; }
          table { width: 100%; border-collapse: collapse; margin: 10px 0; }
          th, td { border: 1px solid #000; padding: 5px; text-align: left; }
          th { background-color: #f0f0f0; }
          @media print { 
            body { margin: 0; }
            .section { page-break-inside: avoid; }
          }
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

    generatePrintContent() {
        return `
      <div class="header">
        <h1>COLEGIO UNIVERSITARIO DE CARTAGO</h1>
        <h2>Formulario de Solicitud de Beca Socioeconómica</h2>
        <p><strong>Fecha:</strong> ${new Date().toLocaleDateString("es-CR")}</p>
      </div>
      
      <div class="section">
        <div class="section-title">I. Identificación de la Persona Solicitante</div>
        <div class="field">
          <span class="field-label">Nombre completo:</span>
          <span class="field-value">${this.formData.nombre || ""} ${this.formData.primerApellido || ""} ${this.formData.segundoApellido || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Cédula:</span>
          <span class="field-value">${this.formData.cedula || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Carrera:</span>
          <span class="field-value">${this.formData.carrera || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Condición:</span>
          <span class="field-value">${this.formData.condicion || ""}</span>
        </div>
      </div>

      <div class="section">
        <div class="section-title">II. Datos Personales</div>
        <div class="field">
          <span class="field-label">Fecha de nacimiento:</span>
          <span class="field-value">${this.formData.fechaNacimiento || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Edad:</span>
          <span class="field-value">${this.formData.edad || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Estado civil:</span>
          <span class="field-value">${this.formData.estadoCivil || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Género:</span>
          <span class="field-value">${this.formData.genero || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Correo electrónico:</span>
          <span class="field-value">${this.formData.email || ""}</span>
        </div>
        <div class="field">
          <span class="field-label">Teléfono celular:</span>
          <span class="field-value">${this.formData.telefonoCelular || ""}</span>
        </div>
      </div>

      <div class="section">
        <div class="section-title">IX. Declaración de Veracidad</div>
        <p>Declaro bajo la gravedad del juramento que toda la información consignada en este formulario es veraz...</p>
        <div class="field">
          <span class="field-label">Firma:</span>
          <div class="signature"></div>
        </div>
        <div class="field">
          <span class="field-label">Fecha:</span>
          <span class="field-value">${this.formData.fechaDeclaracion || ""}</span>
        </div>
      </div>
    `
    }

    // Utility functions
    debounce(func, wait) {
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

    scrollToTop() {
        window.scrollTo({ top: 0, behavior: "smooth" })
    }

    // Declaration buttons functionality
    initializeDeclarationButtons() {
        const declarationABtn = document.getElementById("open-declaration-a")
        const declarationBBtn = document.getElementById("open-declaration-b")

        if (declarationABtn) {
            declarationABtn.addEventListener("click", () => {
                this.openDeclarationModal("A", "Declaración Jurada de Ingresos de Actividades Independientes y/o Informales")
            })
        }

        if (declarationBBtn) {
            declarationBBtn.addEventListener("click", () => {
                this.openDeclarationModal("B", "Declaración Jurada de Ingresos por Actividades No Laborales")
            })
        }
    }

    // Open declaration modals
    openDeclarationModal(type, title) {
        // Create modal overlay
        const modalOverlay = document.createElement("div")
        modalOverlay.className = "modal-overlay"
        modalOverlay.innerHTML = `
      <div class="modal-content">
        <div class="modal-header">
          <h3>${title}</h3>
          <button type="button" class="modal-close" aria-label="Cerrar">&times;</button>
        </div>
        <div class="modal-body">
          ${this.getDeclarationContent(type)}
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn--outline modal-cancel">Cancelar</button>
          <button type="button" class="btn btn--primary modal-save">Guardar Declaración</button>
        </div>
      </div>
    `

        document.body.appendChild(modalOverlay)

        // Add event listeners for modal
        const closeBtn = modalOverlay.querySelector(".modal-close")
        const cancelBtn = modalOverlay.querySelector(".modal-cancel")
        const saveBtn = modalOverlay.querySelector(".modal-save")

        const closeModal = () => {
            document.body.removeChild(modalOverlay)
        }

        closeBtn.addEventListener("click", closeModal)
        cancelBtn.addEventListener("click", closeModal)
        modalOverlay.addEventListener("click", (e) => {
            if (e.target === modalOverlay) closeModal()
        })

        saveBtn.addEventListener("click", () => {
            this.saveDeclaration(type, modalOverlay)
            closeModal()
        })

        // Initialize currency inputs in modal
        this.initializeCurrencyInputsInContainer(modalOverlay)
    }

    // Get declaration form content
    getDeclarationContent(type) {
        if (type === "A") {
            return `
        <form class="declaration-form" id="declaration-a-form">
          <div class="form-grid">
            <div class="form-group form-group--full">
              <label for="actividad-independiente" class="form-label">Descripción de la actividad independiente/informal *</label>
              <textarea id="actividad-independiente" name="actividadIndependiente" class="form-textarea" rows="3" required></textarea>
            </div>
            <div class="form-group">
              <label for="ingreso-promedio-mensual" class="form-label">Ingreso promedio mensual (₡) *</label>
              <input type="text" id="ingreso-promedio-mensual" name="ingresoPromedioMensual" class="form-input currency-input" required>
            </div>
            <div class="form-group">
              <label for="tiempo-actividad" class="form-label">Tiempo realizando esta actividad *</label>
              <input type="text" id="tiempo-actividad" name="tiempoActividad" class="form-input" required>
            </div>
            <div class="form-group form-group--full">
              <label for="observaciones-actividad" class="form-label">Observaciones adicionales</label>
              <textarea id="observaciones-actividad" name="observacionesActividad" class="form-textarea" rows="2"></textarea>
            </div>
          </div>
          <div class="declaration-signature">
            <div class="form-group">
              <label for="fecha-declaracion-a" class="form-label">Fecha *</label>
              <input type="date" id="fecha-declaracion-a" name="fechaDeclaracionA" class="form-input" required>
            </div>
            <div class="form-group">
              <label for="firma-declaracion-a" class="form-label">Firma *</label>
              <div class="signature-container">
                <canvas id="firma-declaracion-a-canvas" width="300" height="100" class="signature-canvas"></canvas>
                <button type="button" class="btn btn--outline btn--small clear-signature" data-canvas="firma-declaracion-a-canvas">
                  Limpiar Firma
                </button>
              </div>
            </div>
          </div>
        </form>
      `
        } else if (type === "B") {
            return `
        <form class="declaration-form" id="declaration-b-form">
          <div class="form-grid">
            <div class="form-group form-group--full">
              <label for="tipo-actividad-no-laboral" class="form-label">Tipo de actividad no laboral *</label>
              <select id="tipo-actividad-no-laboral" name="tipoActividadNoLaboral" class="form-select" required>
                <option value="">Seleccione</option>
                <option value="alquileres">Alquileres</option>
                <option value="intereses">Intereses bancarios</option>
                <option value="dividendos">Dividendos</option>
                <option value="pension">Pensión</option>
                <option value="ayuda-familiar">Ayuda familiar</option>
                <option value="otros">Otros</option>
              </select>
            </div>
            <div class="form-group form-group--full conditional-field" id="otros-actividad-section" style="display: none;">
              <label for="otros-actividad-detalle" class="form-label">Especifique otros</label>
              <input type="text" id="otros-actividad-detalle" name="otrosActividadDetalle" class="form-input">
            </div>
            <div class="form-group">
              <label for="ingreso-mensual-no-laboral" class="form-label">Ingreso mensual (₡) *</label>
              <input type="text" id="ingreso-mensual-no-laboral" name="ingresoMensualNoLaboral" class="form-input currency-input" required>
            </div>
            <div class="form-group">
              <label for="frecuencia-ingreso" class="form-label">Frecuencia del ingreso *</label>
              <select id="frecuencia-ingreso" name="frecuenciaIngreso" class="form-select" required>
                <option value="">Seleccione</option>
                <option value="mensual">Mensual</option>
                <option value="bimestral">Bimestral</option>
                <option value="trimestral">Trimestral</option>
                <option value="semestral">Semestral</option>
                <option value="anual">Anual</option>
                <option value="irregular">Irregular</option>
              </select>
            </div>
            <div class="form-group form-group--full">
              <label for="observaciones-no-laboral" class="form-label">Observaciones adicionales</label>
              <textarea id="observaciones-no-laboral" name="observacionesNoLaboral" class="form-textarea" rows="2"></textarea>
            </div>
          </div>
          <div class="declaration-signature">
            <div class="form-group">
              <label for="fecha-declaracion-b" class="form-label">Fecha *</label>
              <input type="date" id="fecha-declaracion-b" name="fechaDeclaracionB" class="form-input" required>
            </div>
            <div class="form-group">
              <label for="firma-declaracion-b" class="form-label">Firma *</label>
              <div class="signature-container">
                <canvas id="firma-declaracion-b-canvas" width="300" height="100" class="signature-canvas"></canvas>
                <button type="button" class="btn btn--outline btn--small clear-signature" data-canvas="firma-declaracion-b-canvas">
                  Limpiar Firma
                </button>
              </div>
            </div>
          </div>
        </form>
      `
        }
    }

    // Save declaration data
    saveDeclaration(type, modalOverlay) {
        const form = modalOverlay.querySelector(`#declaration-${type.toLowerCase()}-form`)
        const formData = new FormData(form)
        const declarationData = {}

        for (const [key, value] of formData.entries()) {
            declarationData[key] = value
        }

        // Save signature canvas data
        const canvas = modalOverlay.querySelector(`#firma-declaracion-${type.toLowerCase()}-canvas`)
        if (canvas) {
            declarationData[`firmaDeclaracion${type}`] = canvas.toDataURL()
        }

        // Store in form data
        this.formData[`declaracion${type}`] = declarationData

        // Update button text to show completion
        const button = document.getElementById(`open-declaration-${type.toLowerCase()}`)
        if (button) {
            button.textContent = `✓ Declaración ${type} Completada`
            button.classList.add("completed")
        }

        // Save to localStorage
        this.saveFormDraft()

        // Show success message
        this.showMessage(`Declaración ${type} guardada correctamente`, "success")
    }

    // Load draft data
    loadDraftData() {
        const savedData = localStorage.getItem("cuc-scholarship-form")
        if (savedData) {
            try {
                this.formData = JSON.parse(savedData)
                this.populateFormFields()
            } catch (e) {
                console.error("Error loading form data:", e)
            }
        }
    }

    // Initialize currency inputs in a container
    initializeCurrencyInputsInContainer(container) {
        const currencyInputs = container.querySelectorAll(".currency-input")
        currencyInputs.forEach((input) => {
            if (!input.hasAttribute("data-currency-initialized")) {
                input.addEventListener("blur", (e) => this.formatCurrency(e.target))
                input.addEventListener("focus", (e) => this.unformatCurrency(e.target))
                input.setAttribute("data-currency-initialized", "true")
            }
        })
    }

    // Show message
    showMessage(message, type) {
        window.baseApp.showNotification(message, type)
    }

    setupEventListeners() {
        // Add event listeners for real-time validation
        const allInputs = document.querySelectorAll("input, select, textarea")

        allInputs.forEach((input) => {
            // Validate on blur (when user leaves field)
            input.addEventListener("blur", () => {
                this.validateField(input)
            })

            // Clear errors on input (when user starts typing)
            input.addEventListener("input", () => {
                this.clearFieldError(input.id || input.name)
            })

            // Special handling for conditional fields
            if (input.type === "radio" || input.type === "checkbox") {
                input.addEventListener("change", () => {
                    this.handleConditionalFields(input)
                    this.validateField(input)
                })
            }
        })

        // Special validation for declaration checkbox
        const declarationCheckbox = document.getElementById("acepto-declaracion")
        if (declarationCheckbox) {
            declarationCheckbox.addEventListener("change", () => {
                if (declarationCheckbox.checked) {
                    this.clearFieldError("acepto-declaracion")
                } else {
                    this.showFieldError("acepto-declaracion", "Debe aceptar la declaración de veracidad")
                }
            })
        }
    }

    setupRealTimeValidation() {
        document.addEventListener("input", (e) => {
            if (e.target.closest("#scholarship-form")) {
                const fieldName = e.target.name || e.target.id
                const value = e.target.value

                if (fieldName) {
                    this.validateFieldRealTime(fieldName, value, e.target)
                }
            }
        })

        document.addEventListener("change", (e) => {
            if (e.target.closest("#scholarship-form")) {
                const fieldName = e.target.name || e.target.id
                const value = e.target.value

                if (fieldName) {
                    this.validateFieldRealTime(fieldName, value, e.target)
                }
            }
        })
    }
}

// Initialize multi-step form when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    if (document.getElementById("scholarship-form")) {
        new MultiStepForm()
    }
})
