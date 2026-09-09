import type { SubastaCatalogoDTO } from "../models/subastaTypes"

import htmlTemplate from "./subasta-card.html?raw" 
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("subasta-card-template") as HTMLTemplateElement

export function createSubastaCard(dto: SubastaCatalogoDTO) {
    const clone = template.content.cloneNode(true) as DocumentFragment

    const container = clone.querySelector(".card-container")! as HTMLElement
    const categoria = clone.querySelector(".categoria")!
    const titulo = clone.querySelector(".titulo")!
    const pujaActualMonto = clone.querySelector(".puja-actual-monto")!
    const pujaContador = clone.querySelector(".puja-contador")!
    const img = clone.querySelector("#img")! as HTMLImageElement
    const tiempoLabel = clone.querySelector(".tiempo-label")! 
    const tiempo = clone.querySelector(".tiempo")! as HTMLElement

    container.dataset.id = String(dto.id)

    categoria.textContent = dto.categoriaNombre
    titulo.textContent = dto.titulo
    pujaActualMonto.textContent = "$ " + dto.pujaActual
    img.src = dto.urlImagen
    
    if (dto.cantidadPujas > 0)
        pujaContador.textContent = `${dto.cantidadPujas} ${dto.cantidadPujas > 1 ? "Pujas" : "Puja"}`
    else
        pujaContador.textContent = "Sin pujas"

    
    // guardar data en el elemento del timer para poder acceder en la función updateTimers()
    tiempo.dataset.estado = dto.estado
    tiempo.dataset.inicio = String(dto.fechaInicio)
    tiempo.dataset.fin = String(dto.fechaFin)
    tiempo.dataset.timerActivo = "true"

    if (dto.estado == "ACTIVA") {
        tiempoLabel.textContent = "Termina en"
    }
    else if (dto.estado == "PROGRAMADA") {
        tiempoLabel.textContent = "Comienza en"
    }
    else if (dto.estado == "FINALIZADA" || dto.estado == "DESIERTA") {
        tiempoLabel.textContent = "Finalizada"
        tiempo.dataset.timerActivo = "false"
    }

    return clone
}


function updateTimers() {
    const timers = document.querySelectorAll<HTMLElement>('.tiempo[data-timer-activo="true"]')

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

window.setInterval(updateTimers, 1000)