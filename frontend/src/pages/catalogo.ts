import { getCategorias } from "../api/categorias";
import { getSubastaCatalogo } from "../api/subasta";
import { renderHeader } from "../components/header";
import { createSubastaCard } from "../components/subastaCard";
import type { categoriaDTO } from "../models/categoriaTypes";
import { type SubastaCatalogoDTO } from "../models/subastaTypes";

const headerContainer = document.getElementById("header")!
renderHeader(headerContainer)

async function renderSubastaCatalogo() {
    const cardsContainer = document.querySelector(".catalogo-cards-container")!
    const subastas = await getSubastaCatalogo()

    for (let s of subastas) {
        cardsContainer.appendChild(createSubastaCard(s))
    }
}

renderSubastaCatalogo()

async function renderCategoriaList(){
    const categoriasContainer = document.querySelector(".categorias-container")!

    const categorias = await getCategorias()
    for (let c of categorias) {
        categoriasContainer.appendChild(createCategoriaElement(c))
    }
}

function createCategoriaElement(dto: categoriaDTO) {
    const template = document.getElementById("categoria-template")! as HTMLTemplateElement

    const clone = template.content.cloneNode(true) as DocumentFragment

    const container = clone.querySelector('.categoria-container')! as HTMLElement
    const categoriaNombre = clone.querySelector('.categoria-nombre')!

    container.dataset.id = String(dto.id)
    categoriaNombre.textContent = dto.nombre

    return clone
}

renderCategoriaList()