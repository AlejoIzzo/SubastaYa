import type { PaginatedResultDTO } from "../models/paginationType"
import type { SubastaCatalogoDTO } from "../models/subastaTypes"

type RenderPaginacionArgs = {
    paginacionNavId: string
    resultado: PaginatedResultDTO<SubastaCatalogoDTO>
    onSiguiente: () => void
    onAnterior: () => void
}
export function renderPaginacion({paginacionNavId = "paginacion", resultado, onSiguiente, onAnterior}: RenderPaginacionArgs) {
    const container = document.getElementById(paginacionNavId)!;
    if (!container)
        throw new Error("No se encontró nav container con ID: " + paginacionNavId)

    container.innerHTML = "";

    if (resultado.totalPaginas <= 1) {
        return;
    }

    const anterior = document.createElement("button");
    const anteriorIcon = document.createElement("ion-icon")

    anteriorIcon.setAttribute("name", "chevron-back-outline")
    anterior.appendChild(anteriorIcon)
    anterior.appendChild(document.createTextNode("Anterior"))
    anterior.disabled = !resultado.tienePaginaAnterior;

    const pagina = document.createElement("span");
    pagina.textContent =
    `${resultado.paginaActual} / ${resultado.totalPaginas}`;
    
    const siguiente = document.createElement("button");
    const siguienteIcon = document.createElement("ion-icon")
    
    siguiente.appendChild(document.createTextNode("Siguiente"))
    siguiente.disabled = !resultado.tienePaginaSiguiente;
    siguienteIcon.setAttribute("name", "chevron-forward-outline")
    siguiente.appendChild(siguienteIcon)

    container.append(anterior, pagina, siguiente);

    anterior.addEventListener("click", () => {
        onAnterior()
    });

    siguiente.addEventListener("click", () => {
        onSiguiente()
    });
}