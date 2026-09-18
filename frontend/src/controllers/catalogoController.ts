import { getCategorias } from "../api/categoriasApi";
import { getSubastaCatalogo } from "../api/subastaApi";
import { renderHeader } from "../components/header";
import { setupFiltroForm } from "../formHandlers/catalogoFiltroFormHandler";
import { renderCategoriaSectionLoading, renderFilterSection, renderSubastaCatalogo, renderSubastaCatalogoLoading, updateTimers } from "../views/catalogoView";

async function init() {
    // const params = new URLSearchParams(window.location.search)
    // const busqueda = params.get("busqueda") ?? ""

    renderHeader(document.getElementById("header")!)
    
    renderSubastaCatalogoLoading()
    renderCategoriaSectionLoading()
    const categorias = await getCategorias()
    
    let filtrosFormData = new FormData();
    // if (busqueda) {
    //     filtrosFormData.set("busqueda", busqueda)
    // }

    let paginaActual = 1;
    const tamanioPagina = 12;

    async function cargarPagina(pagina: number) {
        renderSubastaCatalogoLoading();

        const resultado = await getSubastaCatalogo(
            filtrosFormData,
            pagina,
            tamanioPagina ?? 12
        );

        paginaActual = resultado.paginaActual;

        renderSubastaCatalogo({
            resultado: resultado,
            onAnterior: () => cargarPagina(resultado.paginaActual - 1),
            onSiguiente: () => cargarPagina(resultado.paginaActual + 1)
        });

        return resultado
    }
    
    renderFilterSection(categorias)
    
    filtrosFormData = setupFiltroForm({
        onFiltrosAplicados: async (formData) => {
            filtrosFormData = formData

            // Cuando cambia un filtro, volver a página 1
            await cargarPagina(1)
        }
    })
    let subastasPaginado = await cargarPagina(1)
    
    window.setInterval(updateTimers, 1000)
}

init()

