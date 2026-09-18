import { capitalize, formatNumber } from "../helpers/stringHelpers"
import type { SubastaCatalogoDTO } from "../models/subastaTypes"

import htmlTemplate from "./subasta-card.html?raw" 
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("subasta-card-template") as HTMLTemplateElement

export function createSubastaCard(dto: SubastaCatalogoDTO) {
    const clone = template.content.cloneNode(true) as DocumentFragment

    const container = clone.querySelector(".card-container")! as HTMLElement
    const categoria = clone.querySelector(".categoria")!
    const titulo = clone.querySelector(".titulo")!
    const pujaActualLabel = clone.querySelector(".puja-actual-label")!
    const pujaActualMonto = clone.querySelector(".puja-actual-monto")!
    const pujaContador = clone.querySelector(".puja-contador")!
    const img = clone.querySelector("#img")! as HTMLImageElement
    const tiempoLabel = clone.querySelector(".tiempo-label")! 
    const tiempo = clone.querySelector(".tiempo")! as HTMLElement

    container.dataset.id = String(dto.id)
    container.setAttribute("href", `subasta.html?id=${dto.id}`)

    categoria.textContent = dto.categoriaNombre
    titulo.textContent = dto.titulo
    if (dto.estado == "ACTIVA" || dto.estado == "PROGRAMADA") {
        pujaActualLabel.textContent = dto.cantidadPujas == 0 ? "Precio base" : "Puja líder actual"
    } else {
        pujaActualLabel.textContent = dto.cantidadPujas == 0 ? "Precio base" : "Puja ganadora"
    }
    pujaActualMonto.textContent = "$ " + formatNumber(dto.pujaActual)
    
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
        tiempoLabel.textContent = capitalize(dto.estado)
        tiempo.dataset.timerActivo = "false"
    }

    return clone
}


