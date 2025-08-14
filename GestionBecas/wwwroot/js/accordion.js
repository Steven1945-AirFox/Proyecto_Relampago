// Accordion functionality for becas and documentos pages
class AccordionManager {
    constructor() {
        this.accordions = document.querySelectorAll(".accordion")
        this.init()
    }

    init() {
        this.accordions.forEach((accordion) => {
            this.initializeAccordion(accordion)
        })
    }

    initializeAccordion(accordion) {
        const buttons = accordion.querySelectorAll(".accordion__button")

        buttons.forEach((button) => {
            button.addEventListener("click", (e) => {
                this.toggleAccordion(e.target, accordion)
            })

            // Keyboard navigation
            button.addEventListener("keydown", (e) => {
                this.handleKeyboardNavigation(e, buttons)
            })
        })
    }

    toggleAccordion(button, accordion) {
        const content = document.getElementById(button.getAttribute("aria-controls"))
        const isExpanded = button.getAttribute("aria-expanded") === "true"

        // Close all other accordions in the same group
        const allButtons = accordion.querySelectorAll(".accordion__button")
        allButtons.forEach((otherButton) => {
            if (otherButton !== button) {
                otherButton.setAttribute("aria-expanded", "false")
                const otherContent = document.getElementById(otherButton.getAttribute("aria-controls"))
                if (otherContent) {
                    otherContent.style.display = "none"
                }
            }
        })

        // Toggle current accordion
        if (isExpanded) {
            button.setAttribute("aria-expanded", "false")
            content.style.display = "none"
        } else {
            button.setAttribute("aria-expanded", "true")
            content.style.display = "block"

            // Smooth scroll to accordion if needed
            setTimeout(() => {
                button.scrollIntoView({ behavior: "smooth", block: "nearest" })
            }, 100)
        }
    }

    handleKeyboardNavigation(e, buttons) {
        const currentIndex = Array.from(buttons).indexOf(e.target)
        let targetIndex

        switch (e.key) {
            case "ArrowDown":
                e.preventDefault()
                targetIndex = currentIndex + 1 >= buttons.length ? 0 : currentIndex + 1
                buttons[targetIndex].focus()
                break
            case "ArrowUp":
                e.preventDefault()
                targetIndex = currentIndex - 1 < 0 ? buttons.length - 1 : currentIndex - 1
                buttons[targetIndex].focus()
                break
            case "Home":
                e.preventDefault()
                buttons[0].focus()
                break
            case "End":
                e.preventDefault()
                buttons[buttons.length - 1].focus()
                break
        }
    }
}

// Initialize accordion manager when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    if (document.querySelector(".accordion")) {
        new AccordionManager()
    }
})
