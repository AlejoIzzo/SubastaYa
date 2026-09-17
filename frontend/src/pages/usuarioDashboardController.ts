import { renderHeader } from "../components/header"
import { getLoggedUsuarioDashboard } from "../helpers/usuarioHelpers"
import { renderMisPujasTabla, renderMisSubastasTabla, renderTablasLoading, renderUsuarioDashboard, renderUsuarioDashboardLoading, updateRowTimers } from "../views/usuarioDashboardView"

async function init() {
    renderHeader(document.getElementById("header")!)

    renderUsuarioDashboardLoading()
    renderTablasLoading()
    let UsuarioDashboard = await getLoggedUsuarioDashboard()
    
    function renderAll() {
        renderUsuarioDashboard(UsuarioDashboard)
        renderMisSubastasTabla(UsuarioDashboard.usuarioSubastas)
        renderMisPujasTabla(UsuarioDashboard.usuarioParticipacionSubastas)   
    }

    const misSubastasTabla = document.getElementById("mis-subastas-tabla")!
    const misPujasTabla = document.getElementById("mis-pujas-tabla")!

    const misSubastasTabButton = document.getElementById("mis-subastas-tab-button")!
    const misPujasTabButton = document.getElementById("mis-pujas-tab-button")!
    
    misSubastasTabla.dataset.visible = "true"
    renderAll()

    misSubastasTabButton.addEventListener("click", () => {
        misSubastasTabla.classList.remove("hidden")
        misPujasTabla.classList.add("hidden")
        
        misSubastasTabla.dataset.visible = "true"
        misPujasTabla.dataset.visible = "false"
        
        misSubastasTabButton.classList.add("selected")
        misPujasTabButton.classList.remove("selected")

        renderAll()
    })
    misPujasTabButton.addEventListener("click", () => {
        misSubastasTabla.classList.add("hidden")
        misPujasTabla.classList.remove("hidden")
        
        misSubastasTabla.dataset.visible = "false"
        misPujasTabla.dataset.visible = "true"

        misSubastasTabButton.classList.remove("selected")
        misPujasTabButton.classList.add("selected")
        renderAll()
    })

    document.addEventListener("usuarioChanged", async () => {
        renderUsuarioDashboardLoading()
        renderTablasLoading()

        UsuarioDashboard = await getLoggedUsuarioDashboard()

        renderAll()
    });

    window.setInterval((updateRowTimers), 1000)
}

init()