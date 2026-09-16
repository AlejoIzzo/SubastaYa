import { renderHeader } from "../components/header"
import { getLoggedUsuarioDashboard } from "../helpers/usuarioHelpers"
import { renderMisPujasTabla, renderMisSubastasTabla, renderUsuarioDashboard, updateRowTimers } from "../views/usuarioDashboardView"

async function init() {
    renderHeader(document.getElementById("header")!)
    
    let UsuarioDashboard = await getLoggedUsuarioDashboard()

    renderUsuarioDashboard(UsuarioDashboard)
    renderMisSubastasTabla(UsuarioDashboard.usuarioSubastas)
    renderMisPujasTabla(UsuarioDashboard.usuarioParticipacionSubastas)
    
    const misSubastasTabla = document.getElementById("mis-subastas-tabla")!
    const misPujasTabla = document.getElementById("mis-pujas-tabla")!

    const misSubastasTabButton = document.getElementById("mis-subastas-tab-button")!
    const misPujasTabButton = document.getElementById("mis-pujas-tab-button")!
    
    misSubastasTabButton.addEventListener("click", () => {
        misSubastasTabla.classList.remove("hidden")
        misPujasTabla.classList.add("hidden")

        misSubastasTabButton.classList.add("selected")
        misPujasTabButton.classList.remove("selected")
    })
    misPujasTabButton.addEventListener("click", () => {
        misSubastasTabla.classList.add("hidden")
        misPujasTabla.classList.remove("hidden")

        misSubastasTabButton.classList.remove("selected")
        misPujasTabButton.classList.add("selected")
    })

    document.addEventListener("usuarioChanged", async () => {
        UsuarioDashboard = await getLoggedUsuarioDashboard()

        renderUsuarioDashboard(UsuarioDashboard)
        renderMisSubastasTabla(UsuarioDashboard.usuarioSubastas)
        renderMisPujasTabla(UsuarioDashboard.usuarioParticipacionSubastas)
    });

    window.setInterval((updateRowTimers), 1000)
}

init()

