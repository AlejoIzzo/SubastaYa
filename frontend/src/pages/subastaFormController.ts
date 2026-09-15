import { getCategorias } from "../api/categoriasApi"
import { renderHeader } from "../components/header"
import { setupSubastaForm } from "../formHandlers/subastaFormHandler"
import { getLoggedUsuario } from "../helpers/usuarioHelpers"
import type { categoriaDTO } from "../models/categoriaTypes"
import { renderCategoriaOptions } from "../views/subastaFormView"

async function init() {
    let usuario = await getLoggedUsuario()
    let categorias = await getCategorias()

    renderHeader(document.getElementById("header")!)

    renderCategoriaOptions(categorias)

    setupSubastaForm({
        getLoggedUsuario
    })

    document.getElementById("cancel-button")!.addEventListener("click", () => {
        window.history.back();
    });
}

init()

