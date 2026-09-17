import { renderHeader } from "../components/header"
import { getLoggedUsuarioDashboard } from "../helpers/usuarioHelpers"
import { renderMisPujasTabla, renderMisSubastasTabla, renderTablasLoading, renderUsuarioDashboard, renderUsuarioDashboardLoading, updateRowTimers } from "../views/usuarioDashboardView"

async function init() {
    renderHeader(document.getElementById("header")!)

    renderUsuarioDashboardLoading()
    renderTablasLoading()
    // let UsuarioDashboard = await getLoggedUsuarioDashboard()
    const tamanioPagina = 9

    async function cargarPagina(pagina: number) {
        let UsuarioDashboard = await getLoggedUsuarioDashboard({
            paginaSubastas: pagina,
            tamanioPaginaSubastas: tamanioPagina,
            paginaParticipaciones: pagina,
            tamanioPartipaciones: tamanioPagina
        })

        renderUsuarioDashboard(UsuarioDashboard)
        renderMisSubastasTabla({
            resultado: UsuarioDashboard.usuarioSubastas,
            onAnterior: () => cargarPagina(UsuarioDashboard.usuarioSubastas.paginaActual - 1),
            onSiguiente: () => cargarPagina(UsuarioDashboard.usuarioSubastas.paginaActual + 1)
        })
        renderMisPujasTabla({
            resultado: UsuarioDashboard.usuarioParticipacionSubastas,
            onAnterior: () => cargarPagina(UsuarioDashboard.usuarioParticipacionSubastas.paginaActual - 1),
            onSiguiente: () => cargarPagina(UsuarioDashboard.usuarioParticipacionSubastas.paginaActual + 1)
        })  

        return UsuarioDashboard
    }

    let UsuarioDashboard = await cargarPagina(1)

    // function renderAll() {
    //     renderUsuarioDashboard(UsuarioDashboard)
    //     renderMisSubastasTabla({
    //         resultado: UsuarioDashboard.usuarioSubastas,
    //         onSiguiente: () => {},
    //         onAnterior: () => {}
    //     })
    //     renderMisPujasTabla({
    //         resultado: UsuarioDashboard.usuarioParticipacionSubastas,
    //         onSiguiente: () => {},
    //         onAnterior: () => {}
    //     })
    // }

    const misSubastasTabla = document.getElementById("mis-subastas-tabla")!
    const misPujasTabla = document.getElementById("mis-pujas-tabla")!

    const misSubastasTabButton = document.getElementById("mis-subastas-tab-button")!
    const misPujasTabButton = document.getElementById("mis-pujas-tab-button")!
    
    const misSubastasNav = document.getElementById("mis-subastas-paginacion")!
    const misPujasNav = document.getElementById("mis-pujas-paginacion")!
    misSubastasTabla.dataset.visible = "true"
    // renderAll()

    misSubastasTabButton.addEventListener("click", () => {
        misSubastasTabla.classList.remove("hidden")
        misPujasTabla.classList.add("hidden")
        
        misSubastasTabla.dataset.visible = "true"
        misPujasTabla.dataset.visible = "false"

        misSubastasNav.classList.remove("hidden")
        misPujasNav.classList.add("hidden")
        
        misSubastasTabButton.classList.add("selected")
        misPujasTabButton.classList.remove("selected")
        cargarPagina(1)
        
        // renderAll()
    })
    misPujasTabButton.addEventListener("click", () => {
        misSubastasTabla.classList.add("hidden")
        misPujasTabla.classList.remove("hidden")
        
        misSubastasTabla.dataset.visible = "false"
        misPujasTabla.dataset.visible = "true"

        misSubastasNav.classList.add("hidden")
        misPujasNav.classList.remove("hidden")

        misSubastasTabButton.classList.remove("selected")
        misPujasTabButton.classList.add("selected")
        // renderAll()
        cargarPagina(1)
        
    })

    document.addEventListener("usuarioChanged", async () => {
        renderUsuarioDashboardLoading()
        renderTablasLoading()

        UsuarioDashboard = await cargarPagina(1)
        // UsuarioDashboard = await getLoggedUsuarioDashboard()

        // renderAll()
    });

    window.setInterval((updateRowTimers), 1000)
}

init()