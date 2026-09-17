import { getUsuarios } from "../api/usuariosApi"
import htmlTemplate from "./header.html?raw"
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("header-template") as HTMLTemplateElement

export function renderHeader(parentElement: HTMLElement) {
    const clone = template.content.cloneNode(true) as DocumentFragment
    
    const searchForm = clone.querySelector("#search-form") as HTMLFormElement
    const searchInput = clone.querySelector("#search-input") as HTMLInputElement

    const params = new URLSearchParams(window.location.search)
    searchInput.value = params.get("busqueda") ?? ""

    searchForm.addEventListener("submit", (event) => {
        event.preventDefault()

        const busqueda = searchInput.value.trim()

        const url = new URL("index.html", window.location.href)
        // si se busca desde el catalogo, conservar filtros aplicados
        if (window.location.pathname.endsWith("index.html")) {
            const currentParams = new URLSearchParams(window.location.search);

            currentParams.delete("busqueda");
            if (busqueda) {
                currentParams.set("busqueda", busqueda);
            }

            url.search = currentParams.toString();
        } else {
            // Desde cualquier otra página solamente llevamos la búsqueda al catálogo.
            if (busqueda) {
                url.searchParams.set("busqueda", busqueda);
            }
        }
        
        window.location.href = url.toString()
    })
    
    const usuarioSelect = clone.querySelector("#usuario-select") as HTMLSelectElement
    usuarioSelect.addEventListener("change", () => {
        const usuarioId = usuarioSelect.value

        sessionStorage.setItem("usuarioId", usuarioId) // sessionStorage guarda como string

        // disparar evento para poder refrescar usuario en páginas que lo necesiten
        document.dispatchEvent(
            new CustomEvent("usuarioChanged", {
                detail: { usuarioId }
            })
        );
    })

    cargarUsuarios(usuarioSelect)
    parentElement.appendChild(clone)
}

async function cargarUsuarios(usuarioSelect: HTMLSelectElement) {
    const usuarios = await getUsuarios()

    let loggedUserId = Number(sessionStorage.getItem("usuarioId") ?? -1)
    // si no hay usuario elegido en la session, elegir el primero
    if (loggedUserId === -1) {
        loggedUserId = usuarios?.[0].id
        sessionStorage.setItem("usuarioId", String(loggedUserId))
    }

    for (let usuario of usuarios) {
        const option = document.createElement("option");

        option.value = usuario.id.toString();
        option.textContent = usuario.nombre;

        if (usuario.id == loggedUserId)
            option.setAttribute("selected", "")

        usuarioSelect.appendChild(option);
    }
}