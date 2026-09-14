import { getUsuarios } from "../api/usuariosApi"
import htmlTemplate from "./header.html?raw"
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("header-template") as HTMLTemplateElement

const clone = template.content.cloneNode(true) as DocumentFragment
export function renderHeader(parentElement: HTMLElement) {
    parentElement.appendChild(clone)
}

// const headerElement = document.getElementById("header")!
// console.log(headerElement)
const usuarioSelect = clone.querySelector("#usuario-select") as HTMLSelectElement
// console.log(usuarioSelect)
async function cargarUsuarios() {
    const usuarios = await getUsuarios()

    let loggedUserId = Number(localStorage.getItem("usuarioId"))
    if (loggedUserId == null) {
        loggedUserId = usuarios?.[0].id
        localStorage.setItem("usuarioId", String(loggedUserId))
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

cargarUsuarios()

usuarioSelect.addEventListener("change", () => {
    const usuarioId = usuarioSelect.value

    localStorage.setItem("usuarioId", usuarioId) // localStorage guarda como string

    // disparar evento para poder refrescar usuario en páginas que lo necesiten
    document.dispatchEvent(
        new CustomEvent("usuarioChanged", {
            detail: { usuarioId }
        })
    );
})