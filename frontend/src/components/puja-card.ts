import type { PujaDTO } from "../models/pujaTypes"
import htmlTemplate from "./puja-card.html?raw" 
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("template") as HTMLTemplateElement

export function createPujaCardElement(dto: PujaDTO) {
    const clone = template.content.cloneNode(true) as DocumentFragment

    // const container = clone.querySelector('.puja-card')! as HTMLElement
    const pujaComprador = clone.querySelector('.puja-comprador')!
    const pujaFecha = clone.querySelector('.puja-fecha')!
    const pujaMonto = clone.querySelector('.puja-monto')!
    pujaComprador.textContent = dto.compradorNombre
    pujaFecha.textContent = `Hace ${getPujaDate(new Date(dto.fecha))}`
    pujaMonto.textContent = `$ ${String(dto.monto)}`

    return clone
}

function getPujaDate(fecha: Date) : string {
    const diff = Date.now() - fecha.getTime() 
    const totalSegundos = Math.floor(diff / 1000);

    const dias = Math.floor(totalSegundos / 86400);
    if (dias > 0)
        return `${dias} ${dias > 1 ? "días" : "día"}`

    const horas = Math.floor((totalSegundos % 86400) / 3600)
    if (horas > 0) 
        return `${horas} ${horas > 1 ? "horas" : "hora"}`

    const minutos = Math.floor((totalSegundos % 3600) / 60)
    if (minutos > 0)
        return `${minutos} ${minutos > 1 ? "minutos" : "minuto"}`

    const segundos = String(totalSegundos % 60).padStart(2, "0");
    return `${segundos} segundos`
    
}