import type { categoriaDTO } from "../models/categoriaTypes"

import htmlTemplate from "./categoria.html?raw" 
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("categoria-template") as HTMLTemplateElement

export function createCategoriaElement(dto: categoriaDTO) {
    const iconMap: any = {
        "Tecnología": "laptop-outline",
        "Coleccionables": "watch-outline",
        "Indumentaria": "shirt-outline",
        "Vehículos": "car-sport-outline",
    }

    const clone = template.content.cloneNode(true) as DocumentFragment

    const container = clone.querySelector('.categoria-container')! as HTMLElement
    const categoriaNombre = clone.querySelector('.categoria-nombre')!
    const categoriaIcon = clone.querySelector('.categoria-icon')! as HTMLElement
    const checkbox = clone.querySelector('input')! as HTMLElement

    container.dataset.id = String(dto.id)
    categoriaNombre.textContent = dto.nombre
    checkbox.setAttribute("value", String(dto.id)) // setear id en input para recuperar con formData()
    
    categoriaIcon.setAttribute("name",  iconMap?.[dto.nombre] ?? "cube-outline")
    
    return clone
}