import { capitalize } from "../helpers/stringHelpers"
import type { UsuarioDashboardDTO, UsuarioParticipacionSubastaDTO, UsuarioSubastaDTO } from "../models/usuarioTypes"

export function renderUsuarioDashboard(UsuarioDashboard: UsuarioDashboardDTO)  {
    const subastasActivas = document.getElementById("subastas-activas")!
    const subastasGanadas = document.getElementById("subastas-ganadas")!
    const totalRecaudado = document.getElementById("total-recaudado")!
    const totalPujado = document.getElementById("total-pujado")!
    
    subastasGanadas.textContent = `${UsuarioDashboard.usuarioEstadisticas.subastasGanadas}`
    subastasActivas.textContent = `${UsuarioDashboard.usuarioEstadisticas.subastasActivas}`
    totalRecaudado.textContent = `$ ${UsuarioDashboard.usuarioEstadisticas.totalRecaudado}`
    totalPujado.textContent = `$ ${UsuarioDashboard.usuarioEstadisticas.totalPujado}`
}

export function renderMisSubastasTabla(usuarioSubatas: UsuarioSubastaDTO[]) {
    const misSubastasTabla = document.getElementById("mis-subastas-tabla-body")!

    misSubastasTabla.replaceChildren()
    for (let u of usuarioSubatas) {
        misSubastasTabla.appendChild(createMisSubastasFila(u))
    }
}

export function renderMisPujasTabla(usuarioParticipacionSubastas: UsuarioParticipacionSubastaDTO[]) {
    const misPujasTabla = document.getElementById("mis-pujas-tabla-body")!

    misPujasTabla.replaceChildren()
    for (let u of usuarioParticipacionSubastas) {
        misPujasTabla.appendChild(createMisPujasFila(u))
    }
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
                                ? String(usuarioSubastas.pujaLider.monto)
                                : " - "
    cantidadPujas.textContent = usuarioSubastas.cantidadPujas.toString()
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
    pujaLider.textContent = usuarioParticipacionSubastas.pujaLider.monto.toString()
    usuarioUltimaPuja.textContent = usuarioParticipacionSubastas.ultimaPujaUsuario.monto.toString()
    
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
