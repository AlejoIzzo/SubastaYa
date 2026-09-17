import { renderPaginacion } from "../components/paginacionNav"
import { showLoading, showTableLoadingRow } from "../components/spinner"
import { capitalize, formatNumber } from "../helpers/stringHelpers"
import type { PaginatedResultDTO } from "../models/paginationType"
import type { UsuarioDashboardDTO, UsuarioParticipacionSubastaDTO, UsuarioSubastaDTO } from "../models/usuarioTypes"

const subastasActivas = document.getElementById("subastas-activas")!
const subastasGanadas = document.getElementById("subastas-ganadas")!
const totalRecaudado = document.getElementById("total-recaudado")!
const totalPujado = document.getElementById("total-pujado")!

export function renderUsuarioDashboardLoading()  {
    showLoading(subastasGanadas)
    showLoading(subastasActivas)
    showLoading(totalRecaudado)
    showLoading(totalPujado)
}
export function renderUsuarioDashboard(UsuarioDashboard: UsuarioDashboardDTO)  {
    subastasGanadas.textContent = `${formatNumber(UsuarioDashboard.usuarioEstadisticas.subastasGanadas)}`
    subastasActivas.textContent = `${formatNumber(UsuarioDashboard.usuarioEstadisticas.subastasActivas)}`
    totalRecaudado.textContent = `$ ${formatNumber(UsuarioDashboard.usuarioEstadisticas.totalRecaudado)}`
    totalPujado.textContent = `$ ${formatNumber(UsuarioDashboard.usuarioEstadisticas.totalPujado)}`
}

const misSubastasTabla = document.getElementById("mis-subastas-tabla")!
const misPujasTabla = document.getElementById("mis-pujas-tabla")!

const misSubastasTablaBody = document.getElementById("mis-subastas-tabla-body")!
const misPujasTablaBody = document.getElementById("mis-pujas-tabla-body")!

const subastasEmptyMessage = document.querySelector("#subastas-empty-message")!
const pujasEmptyMessage = document.querySelector("#pujas-empty-message")!

export function renderTablasLoading() {
    showTableLoadingRow(misSubastasTablaBody, 5)
    showTableLoadingRow(misPujasTablaBody, 5)
}
type RenderMisSubastasTablaArgs = {
    resultado: PaginatedResultDTO<UsuarioSubastaDTO>, 
    onSiguiente: () => void, 
    onAnterior: () => void
}
export function renderMisSubastasTabla({resultado, onSiguiente, onAnterior}: RenderMisSubastasTablaArgs) {
    const tablaHead = document.querySelector("#mis-subastas-tabla thead")!
    
    const usuarioSubastas = resultado.items
    if (misSubastasTabla.dataset.visible == "true" && usuarioSubastas.length <= 0) {
        subastasEmptyMessage.classList.remove("hidden")
        tablaHead.classList.add("hidden")
    } else {
        subastasEmptyMessage.classList.add("hidden")
        tablaHead.classList.remove("hidden")
    }
    misSubastasTablaBody.replaceChildren()
    for (let u of usuarioSubastas) {
        misSubastasTablaBody.appendChild(createMisSubastasFila(u))
    }

    renderPaginacion({
        paginacionNavId: "mis-subastas-paginacion",
        resultado: resultado,
        onSiguiente,
        onAnterior
    })
}
type RenderMisPujasTablaArgs = {
    resultado: PaginatedResultDTO<UsuarioParticipacionSubastaDTO>, 
    onSiguiente: () => void, 
    onAnterior: () => void
}
export function renderMisPujasTabla({resultado, onSiguiente, onAnterior}: RenderMisPujasTablaArgs) {
    const tablaHead = document.querySelector("#mis-pujas-tabla thead")!

    const usuarioParticipacionSubastas = resultado.items
    if (misPujasTabla.dataset.visible == "true" && usuarioParticipacionSubastas.length <= 0) {
        pujasEmptyMessage.classList.remove("hidden")
        tablaHead.classList.add("hidden")
    } else {
        pujasEmptyMessage.classList.add("hidden")
        tablaHead.classList.remove("hidden")
    }
    misPujasTablaBody.replaceChildren()
    for (let u of usuarioParticipacionSubastas) {
        misPujasTablaBody.appendChild(createMisPujasFila(u))
    }

    renderPaginacion({
        paginacionNavId: "mis-pujas-paginacion",
        resultado: resultado,
        onSiguiente,
        onAnterior
    })
}

function createMisSubastasFila(usuarioSubastas: UsuarioSubastaDTO) {
    const rowTemplate = document.getElementById("mis-subastas-row-template") as HTMLTemplateElement
    const clone = rowTemplate.content.cloneNode(true) as DocumentFragment

    const subastaNombre = clone.querySelector(".row-subasta-nombre")!
    const pujaLider = clone.querySelector(".row-puja-lider")!
    const cantidadPujas = clone.querySelector(".row-cantidad-pujas")!
    const subastaEstado = clone.querySelector(".row-subasta-estado")!
    const timer = clone.querySelector(".row-timer") as HTMLElement


    subastaNombre.textContent = usuarioSubastas.titulo
    pujaLider.textContent = usuarioSubastas.pujaLider 
                                ? formatNumber(usuarioSubastas.pujaLider.monto)
                                : " - "
    cantidadPujas.textContent = formatNumber(usuarioSubastas.cantidadPujas)
    subastaEstado.textContent = capitalize(usuarioSubastas.estado)

    if (usuarioSubastas.estado == "ACTIVA") {
        timer.textContent = "00:00:00"
        timer.dataset.activo = "true"
        timer.dataset.inicio = usuarioSubastas.fechaFin
        timer.dataset.fin = usuarioSubastas.fechaFin
    } else {
        timer.textContent = "Finalizada"
        timer.dataset.activo = "false"
    }
    return clone
}

function createMisPujasFila(usuarioParticipacionSubastas: UsuarioParticipacionSubastaDTO) {
    const rowTemplate = document.getElementById("mis-pujas-row-template") as HTMLTemplateElement
    const clone = rowTemplate.content.cloneNode(true) as DocumentFragment

    const subastaNombre = clone.querySelector(".row-subasta-nombre")!
    const pujaLider = clone.querySelector(".row-puja-lider")!
    const usuarioUltimaPuja = clone.querySelector(".row-puja-usuario")!
    const usuarioEstado = clone.querySelector(".row-usuario-estado")!
    const timer = clone.querySelector(".row-timer")! as HTMLElement

    subastaNombre.textContent = usuarioParticipacionSubastas.titulo
    pujaLider.textContent = formatNumber(usuarioParticipacionSubastas.pujaLider.monto)
    usuarioUltimaPuja.textContent = formatNumber(usuarioParticipacionSubastas.ultimaPujaUsuario.monto)
    
    if (usuarioParticipacionSubastas.ultimaPujaUsuario.compradorId === usuarioParticipacionSubastas.pujaLider.compradorId) {
        usuarioEstado.textContent = usuarioParticipacionSubastas.estado == "ACTIVA"
                                        ? "Líderando"
                                        : "Ganador"
    } else {
        usuarioEstado.textContent = usuarioParticipacionSubastas.estado == "ACTIVA"
                                        ? "Superado"
                                        : "Perdedor"
    }

    if (usuarioParticipacionSubastas.estado == "ACTIVA") {
        timer.textContent = "00:00:00"
        timer.dataset.activo = "true"
        timer.dataset.inicio = usuarioParticipacionSubastas.fechaFin
        timer.dataset.fin = usuarioParticipacionSubastas.fechaFin
    } else {
        timer.textContent = "Finalizada"
        timer.dataset.activo = "false"
    }
    
    return clone
}

export function updateRowTimers() {
    const timers = document.querySelectorAll<HTMLElement>('.row-timer[data-activo="true"]')
    for (let timer of timers) {
        const objetivo = timer.dataset.estado == "ACTIVA" 
                            ? new Date(timer.dataset.fin as string).getTime() 
                            : new Date(timer.dataset.inicio as string).getTime();
        const remaining = objetivo - Date.now();

        const totalSegundos = Math.floor(remaining / 1000);

        if (remaining <= 0) {
            timer.dataset.timerActivo = "false"
            
            timer.textContent = timer.dataset.estado == "ACTIVA" ? "Finalizada" : "Comenzando..."
            continue
        }

        const dias = Math.floor(totalSegundos / 86400);
        const horas = String(Math.floor((totalSegundos % 86400) / 3600)).padStart(2, "0");
        const minutos = String(Math.floor((totalSegundos % 3600) / 60)).padStart(2, "0");
        const segundos = String(totalSegundos % 60).padStart(2, "0");

        const timerText = dias > 0 
                    ? `${dias}d ${horas}:${minutos}:${segundos}` 
                    : `${horas}:${minutos}:${segundos}` 
        
        timer.textContent = timerText
    }
}
