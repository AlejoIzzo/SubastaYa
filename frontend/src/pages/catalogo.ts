import { getCategorias } from "../api/categoriasApi";
import { getSubastaCatalogo } from "../api/subastaApi";
import { renderHeader } from "../components/header";
import { createRadioButton } from "../components/radio-button";
import { createSubastaCard } from "../components/subastaCard";

const headerContainer = document.getElementById("header")!
renderHeader(headerContainer)

async function renderSubastaCatalogo(filtroFormData?: FormData) {
    const cardsContainer = document.querySelector(".catalogo-cards-container")!
    cardsContainer.replaceChildren()
    const subastas = await getSubastaCatalogo(filtroFormData)

    for (let s of subastas) {
        cardsContainer.appendChild(createSubastaCard(s))
    }
}

async function renderFilterSection(){
    const categoriasContainer = document.querySelector(".categorias-container")!
    const categorias = await getCategorias()

    const iconMap: any = {
        "Tecnología": "laptop-outline",
        "Coleccionables": "watch-outline",
        "Indumentaria": "shirt-outline",
        "Vehículos": "car-sport-outline",
    }
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

renderSubastaCatalogo()
renderFilterSection()

let filtrosFormData = new FormData()

// --- manejo de filtros ---

const filtrosForm = document.getElementById("filtros-form") as HTMLFormElement
const searchForm = document.getElementById("search-form") as HTMLFormElement

function updateFiltroFormData() {
    const filtroFormData = new FormData(filtrosForm);
    for (const [key, value] of filtroFormData.entries()) {
        if (value != null) {
            filtrosFormData.set(key, value.toString());
        }
    }
    
    const searchFormData = new FormData(searchForm);
    filtrosFormData.set("busqueda", searchFormData.get("busqueda") ?? "")
}

filtrosForm.addEventListener("submit", async (event) => {
    event.preventDefault()
    updateFiltroFormData()
    await renderSubastaCatalogo(filtrosFormData)
})

searchForm.addEventListener("submit", async (event) => {
    event.preventDefault()
    updateFiltroFormData()
    await renderSubastaCatalogo(filtrosFormData)
})

filtrosForm.addEventListener("reset", async (event) => {
    // esperar un frame a que browser resetee los valores del form, si no se espera los filtros quedan un estado por detras y se repite la query con los filtros activos
    requestAnimationFrame(async () => {
        updateFiltroFormData();
        await renderSubastaCatalogo(filtrosFormData);
    });
})

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