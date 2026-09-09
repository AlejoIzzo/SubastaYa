import { getCategorias } from "../api/categoriasApi";
import { getSubastaCatalogo } from "../api/subastaApi";
import { createCategoriaElement } from "../components/categoria";
import { renderHeader } from "../components/header";
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

async function renderCategoriaList(){
    const categoriasContainer = document.querySelector(".categorias-container")!

    const categorias = await getCategorias()
    for (let c of categorias) {
        categoriasContainer.appendChild(createCategoriaElement(c))
    }
}

renderSubastaCatalogo()
renderCategoriaList()

let filtrosFormData = new FormData()

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