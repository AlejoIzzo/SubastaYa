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
    const cardStatusTag = clone.querySelector(".card-status-tag")! as HTMLElement
    const cardStatusTagText = clone.querySelector(".card-status-tag-text")! as HTMLElement
    const cardStatusTagIcon = clone.querySelector(".card-status-tag-icon")! as HTMLElement

    container.dataset.id = String(dto.id)
    container.setAttribute("href", `subasta.html?id=${dto.id}`)

    categoria.textContent = dto.categoriaNombre
    titulo.textContent = dto.titulo
    if (dto.estado == "ACTIVA" || dto.estado == "PROGRAMADA") {
        pujaActualLabel.textContent = dto.cantidadPujas == 0 ? "Precio base" : "Puja líder"
    } else {
        pujaActualLabel.textContent = dto.cantidadPujas == 0 ? "Precio base" : "Puja ganadora"
    }
    pujaActualMonto.textContent = "$ " + formatNumber(dto.pujaActual)
    
    const imgFallbackUrl = "./src/assets/img-fallback.jpg"
    img.src = dto.urlImagen
    img.addEventListener('error', (event: Event) => {
        const target = event.target as HTMLImageElement;
        // Prevent infinite loop if fallback also fails
        target.onerror = null; 
        target.src = imgFallbackUrl;
        target.classList.add("img-error");
        target.title = "Imagen no encontrada"
    });
    
    if (dto.cantidadPujas > 0)
        pujaContador.textContent = ` (${dto.cantidadPujas} ${dto.cantidadPujas > 1 ? "Pujas)" : "Puja)"}`

    // guardar data en el elemento del timer para poder acceder en la función updateTimers()
    tiempo.dataset.estado = dto.estado
    tiempo.dataset.inicio = String(dto.fechaInicio)
    tiempo.dataset.fin = String(dto.fechaFin)
    tiempo.dataset.timerActivo = "true"

    if (dto.estado == "ACTIVA") {
        container.classList.add("card-activa")
        tiempoLabel.textContent = "Termina en"

        cardStatusTag.classList.add("tag-activa")
        cardStatusTagText.textContent = "ACTIVA"
        cardStatusTagIcon.setAttribute("name", "ellipse")
    } else if (dto.estado == "PROGRAMADA") {
        container.classList.add("card-programada")
        tiempoLabel.textContent = "Comienza en"

        cardStatusTag.classList.add("tag-programada")
        cardStatusTagText.textContent = "PROGRAMADA"
        cardStatusTagIcon.setAttribute("name", "calendar-outline")
    } else if (dto.estado == "FINALIZADA") {
        container.classList.add("card-finalizada")
        tiempo.dataset.timerActivo = "false"

        cardStatusTag.classList.add("tag-finalizada")
        cardStatusTagText.textContent = "FINALIZADA"
        cardStatusTagIcon.setAttribute("name", "trophy-outline")
    } else if (dto.estado == "DESIERTA") {
        container.classList.add("card-desierta")
        tiempo.dataset.timerActivo = "false"
        
        cardStatusTag.classList.add("tag-desierta")
        cardStatusTagText.textContent = "DESIERTA"
        cardStatusTagIcon.setAttribute("name", "remove-circle-outline")
    }

    return clone
}


