import { renderSubastaCatalogoLoading } from "../views/catalogoView"

type Args = {
    onFiltrosAplicados: (filtrosFormData: FormData) => void
}

export function setupFiltroForm({onFiltrosAplicados}: Args) {
    const filtrosForm = document.getElementById("filtros-form") as HTMLFormElement
    
    let filtrosFormData = new FormData()

    function cargarFiltrosDesdeUrl() {
        const params = new URLSearchParams(window.location.search);

        filtrosFormData = new FormData();

        for (const [key, value] of params.entries()) {
            if (value !== "") {
                filtrosFormData.set(key, value);
            }
        }
    }

    function sincronizarFormConUrl() {
        const params = new URLSearchParams(window.location.search);

        // categoria
        const categoriaId = params.get("categoriaId");
        if (categoriaId) {
            const categoriaInput = filtrosForm.querySelector<HTMLInputElement>(
                `input[name="categoriaId"][value="${CSS.escape(categoriaId)}"]`
            );

            if (categoriaInput) {
                categoriaInput.checked = true;
            }
        }

        // estado
        const estado = params.get("estado");
        if (estado) {
            const estadoInput = filtrosForm.querySelector<HTMLInputElement>(
                `input[name="estado"][value="${CSS.escape(estado)}"]`
            );

            if (estadoInput) {
                estadoInput.checked = true;
            }
        }

        // precio mínimo
        const precioMin = params.get("precioMin");
        const precioMinInput = filtrosForm.querySelector<HTMLInputElement>(
            '[name="precioMin"]'
        );

        if (precioMinInput) {
            precioMinInput.value = precioMin ?? "";
        }

        // precio máximo
        const precioMax = params.get("precioMax");
        const precioMaxInput = filtrosForm.querySelector<HTMLInputElement>(
            '[name="precioMax"]'
        );

        if (precioMaxInput) {
            precioMaxInput.value = precioMax ?? "";
        }
    }

    function actualizarFiltrosDesdeForm() {
        const formData = new FormData(filtrosForm);

        filtrosFormData = new FormData();

        const busqueda = new URLSearchParams(window.location.search).get("busqueda")
        if (busqueda) {
            filtrosFormData.set("busqueda", busqueda)
        }
        for (const [key, value] of formData.entries()) {
            const stringValue = value.toString().trim();

            // "all" significa que no queremos filtrar por ese campo
            if (stringValue !== "" && stringValue !== "all") {
                filtrosFormData.set(key, stringValue);
            }
        }
    }
    function actualizarUrl() {
        const url = new URL(window.location.href);

        // Solamente reemplazamos los parámetros que maneja el catálogo.
        url.searchParams.delete("categoriaId");
        url.searchParams.delete("estado");
        url.searchParams.delete("precioMin");
        url.searchParams.delete("precioMax");

        for (const [key, value] of filtrosFormData.entries()) {
            if (key === "busqueda") {
                continue;
            }

            url.searchParams.set(key, value.toString());
        }

        window.history.pushState({}, "", url);
    }
    
    async function aplicarFiltros() {
        actualizarFiltrosDesdeForm()
        actualizarUrl()

        renderSubastaCatalogoLoading()

        onFiltrosAplicados(filtrosFormData)
    }
    
    cargarFiltrosDesdeUrl();
    sincronizarFormConUrl();
    
    filtrosForm.addEventListener("submit", async (event) => {
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

    return filtrosFormData

}
