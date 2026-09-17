import { createRadioButton } from "../components/radio-button"
import { showLoading } from "../components/spinner"
import { createSubastaCard } from "../components/subastaCard"
import type { categoriaDTO } from "../models/categoriaTypes"
import type { SubastaCatalogoDTO } from "../models/subastaTypes"

const cardsContainer = document.querySelector(".catalogo-cards-container")!
export function renderSubastaCatalogoLoading() {
    showLoading(cardsContainer)
}
export function renderSubastaCatalogo(subastas: SubastaCatalogoDTO[]) {
    cardsContainer.replaceChildren()

    if (subastas.length <= 0) {
        const p = document.createElement("p")
        p.textContent = "No hay subastas para mostrar, pruebe ajustar los filtros o la busqueda."
        p.style.margin = "auto auto"
        cardsContainer.appendChild(p)
    }

    for (let s of subastas) {
        cardsContainer.appendChild(createSubastaCard(s))
    }
}

const categoriasContainer = document.querySelector(".categorias-container")!
export function renderCategoriaSectionLoading() {
    showLoading(categoriasContainer)
}
export function renderFilterSection(categorias: categoriaDTO[]){

    categoriasContainer.replaceChildren()

    const iconMap: any = {
        "Tecnología": "laptop-outline",
        "Coleccionables": "watch-outline",
        "Indumentaria": "shirt-outline",
        "Vehículos": "car-sport-outline",
    }

    categoriasContainer.appendChild(createRadioButton({
        inputFormName: "categoriaId",
        inputFormValue: "all",
        icon: "cube-outline",
        label: "Todas",
        checked: true
    }))

    for (let c of categorias) {
        categoriasContainer.appendChild(createRadioButton({
            inputFormName: "categoriaId",
            inputFormValue: c.id,
            icon: iconMap[c.nombre],
            label: c.nombre
        }))
    }

    const estadosContainer = document.querySelector(".estados-container")!
    const estados = ["Activa", "Finalizada", "Programada"]
    
    for (let e of estados) {
        estadosContainer.appendChild(createRadioButton({
            inputFormName: "estado",
            inputFormValue: e,
            label: e
        }))
    }
}

export function updateTimers() {
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