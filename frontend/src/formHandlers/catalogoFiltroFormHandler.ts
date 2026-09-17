import { renderSubastaCatalogoLoading } from "../views/catalogoView"

type Args = {
    // onSubastasUpdate: (subastas: SubastaCatalogoDTO[]) => void
    onFiltrosAplicados: (filtrosFormData: FormData) => void
}

export function setupFiltroForm({onFiltrosAplicados}: Args) {
    const filtrosForm = document.getElementById("filtros-form") as HTMLFormElement
    const searchForm = document.getElementById("search-form") as HTMLFormElement
    
    let filtrosFormData = new FormData()

    function updateFiltroFormData() {
        // data de categoria, estado, precioMin y precioMax
        const filtroFormData = new FormData(filtrosForm);
        for (const [key, value] of filtroFormData.entries()) {
            if (value != null) {
                filtrosFormData.set(key, value.toString());
            }
        }
        console.log(filtroFormData)
        
        // data de la barra de busqueda
        const searchFormData = new FormData(searchForm);
        filtrosFormData.set("busqueda", searchFormData.get("busqueda") ?? "")
    }

    async function aplicarFiltros() {
        updateFiltroFormData()
        renderSubastaCatalogoLoading()
        
        // const subastas = await getSubastaCatalogo(filtrosFormData)
        // onSubastasUpdate(subastas)
        
        // volver a página 1 al aplicar filtros nuevos
        onFiltrosAplicados(filtrosFormData)
        // await cargarPagina(1);
    }
    
    filtrosForm.addEventListener("submit", async (event) => {
        event.preventDefault()
        await aplicarFiltros()
    })
    
    searchForm.addEventListener("submit", async (event) => {
        event.preventDefault()
        await aplicarFiltros()
    })
    
    filtrosForm.addEventListener("reset", async (event) => {
        // esperar un frame a que browser resetee los valores del form, si no se espera los filtros quedan un estado por detras y se repite la query con los filtros activos
        requestAnimationFrame(async () => {
            await aplicarFiltros()
            document.querySelector<HTMLInputElement>(".categorias-container input")!.checked = true
        });
    })

}
