import { getCategorias } from "../api/categoriasApi";
import { getSubastaCatalogo } from "../api/subastaApi";
import { renderHeader } from "../components/header";
import { setupFiltroForm } from "../formHandlers/catalogoFiltroFormHandler";
import { renderCategoriaSectionLoading, renderFilterSection, renderSubastaCatalogo, renderSubastaCatalogoLoading, updateTimers } from "../views/catalogoView";

async function init() {
    renderHeader(document.getElementById("header")!)
    
    renderSubastaCatalogoLoading()
    renderCategoriaSectionLoading()
    let subastas = await getSubastaCatalogo()
    const categorias = await getCategorias()
    
    renderSubastaCatalogo(subastas)
    renderFilterSection(categorias)
    
    setupFiltroForm({
        onSubastasUpdate: (subastasFiltered) => {
            subastas = subastasFiltered
            renderSubastaCatalogo(subastas)
        }
    })
    
    window.setInterval(updateTimers, 1000)
}

init()
