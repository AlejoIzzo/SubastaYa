import { formatNumber } from "../helpers/stringHelpers"
import type { PujaDTO } from "../models/pujaTypes"
import type { UsuarioDTO } from "../models/usuarioTypes"
import htmlTemplate from "./puja-card.html?raw" 
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("template") as HTMLTemplateElement

export function createPujaCardElement(dto: PujaDTO, usuario: UsuarioDTO) {
    const clone = template.content.cloneNode(true) as DocumentFragment
    
    // const container = clone.querySelector('.puja-card')! as HTMLElement
    const pujaContainer = clone.querySelector('.puja-card')! as HTMLElement
    const pujaComprador = clone.querySelector('.puja-comprador-nombre')!
    const pujaFecha = clone.querySelector('.puja-fecha')! as HTMLElement
    const pujaMonto = clone.querySelector('.puja-monto')!

    pujaComprador.textContent = dto.compradorNombre
    pujaFecha.textContent = `${getPujaDate(new Date(dto.fecha))}`
    pujaMonto.textContent = `$ ${formatNumber(dto.monto)}`
    
    pujaFecha.dataset.fecha = dto.fecha
    pujaContainer.dataset.compradorId = String(dto.compradorId)
    if (dto.compradorId === usuario.id)
        pujaContainer.classList.add('of-user')

    return clone
}

export function refreshUserPujas(usuario: UsuarioDTO) {
    const pujaContainers = document.querySelectorAll('.puja-card')!

    pujaContainers.forEach((x: any) => {
        if (Number(x.dataset.compradorId) === usuario.id)
            x.classList.add("of-user")
        else 
            x.classList.remove("of-user")
    })
}

export function refreshPujasDates() {
    const pujaFecha = document.querySelectorAll(".puja-fecha")!

    pujaFecha.forEach((el: any) => {
        el.textContent = getPujaDate(new Date(el.dataset.fecha))
    })
}

function getPujaDate(fecha: Date) : string {
    const diff = Date.now() - fecha.getTime();
    const totalSegundos = Math.floor(diff / 1000);

    const dias = Math.floor(totalSegundos / 86400);
    if (dias > 0)
        return `Hace ${dias} ${dias > 1 ? "días" : "día"}`;

    const horas = Math.floor(totalSegundos / 3600);
    if (horas > 0)
        return `Hace ${horas} ${horas > 1 ? "horas" : "hora"}`;

    const minutos = Math.floor(totalSegundos / 60);
    if (minutos > 0)
        return `Hace ${minutos} ${minutos > 1 ? "minutos" : "minuto"}`;
    
    return `Hace unos segundos`
}