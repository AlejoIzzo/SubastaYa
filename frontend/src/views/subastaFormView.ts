import type { categoriaDTO } from "../models/categoriaTypes";

export function renderCategoriaOptions(categorias: categoriaDTO[]) {
    const categoriaSelect = document.getElementById("categoriaId") as HTMLSelectElement

    for (let c of categorias) {
        const option = document.createElement("option");

        option.value = c.id.toString();
        option.textContent = c.nombre;
        
        categoriaSelect.appendChild(option);
    }
}
