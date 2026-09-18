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

function getPujaDate(fecha: Date): string {
    return fecha.toLocaleString();
}