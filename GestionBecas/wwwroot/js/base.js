
class BaseApp {
    constructor() {
        this.init()
    }

    init() {
        this.initializeNavigation()
        
        this.initializeAccessibility()
    }

 
    initializeNavigation() {
        const navToggle = document.querySelector(".nav__toggle")
        const navMenu = document.querySelector(".nav__menu")

        if (navToggle && navMenu) {
            navToggle.addEventListener("click", () => {
                const isOpen = navMenu.classList.contains("nav__menu--open")

                if (isOpen) {
                    this.closeNavMenu(navToggle, navMenu)
                } else {
                    this.openNavMenu(navToggle, navMenu)
                }
            })

           
            document.addEventListener("click", (e) => {
                if (!navToggle.contains(e.target) && !navMenu.contains(e.target)) {
                    this.closeNavMenu(navToggle, navMenu)
                }
            })

            
            document.addEventListener("keydown", (e) => {
                if (e.key === "Escape" && navMenu.classList.contains("nav__menu--open")) {
                    this.closeNavMenu(navToggle, navMenu)
                    navToggle.focus()
                }
            })
        }
    }

    openNavMenu(toggle, menu) {
        menu.classList.add("nav__menu--open")
        toggle.setAttribute("aria-expanded", "true")
    }

    closeNavMenu(toggle, menu) {
        menu.classList.remove("nav__menu--open")
        toggle.setAttribute("aria-expanded", "false")
    }

    
    showNotification(message, type = "info") {
        const notification = document.createElement("div")
        notification.className = `notification notification--${type}`
        notification.innerHTML = `
      <span>${message}</span>
      <button class="notification__close" aria-label="Cerrar notificación">&times;</button>
    `

        document.body.appendChild(notification)

        const closeBtn = notification.querySelector(".notification__close")
        closeBtn.addEventListener("click", () => {
            notification.remove()
        })

       
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove()
            }
        }, 5000)
    }

    
    initializeAccessibility() {
        
        this.addSkipLink()

        
        this.manageFocus()
    }

    addSkipLink() {
        const skipLink = document.createElement("a")
        skipLink.href = "#main-content"
        skipLink.textContent = "Saltar al contenido principal"
        skipLink.className = "sr-only"
        skipLink.style.cssText = `
      position: absolute;
      top: -40px;
      left: 6px;
      background: var(--color-primary);
      color: white;
      padding: 8px;
      text-decoration: none;
      border-radius: 4px;
      z-index: 1000;
      transition: top 0.3s;
    `

        skipLink.addEventListener("focus", () => {
            skipLink.style.top = "6px"
            skipLink.classList.remove("sr-only")
        })

        skipLink.addEventListener("blur", () => {
            skipLink.style.top = "-40px"
            skipLink.classList.add("sr-only")
        })

        document.body.insertBefore(skipLink, document.body.firstChild)
    }

    manageFocus() {
        
        document.addEventListener("keydown", (e) => {
            if (e.key === "Tab") {
                document.body.classList.add("keyboard-navigation")
            }
        })

        document.addEventListener("mousedown", () => {
            document.body.classList.remove("keyboard-navigation")
        })
    }

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
}


document.addEventListener("DOMContentLoaded", () => {
    window.baseApp = new BaseApp()
})
