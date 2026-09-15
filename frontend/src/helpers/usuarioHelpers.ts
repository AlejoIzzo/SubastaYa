import { getUsuario } from "../api/usuariosApi"

export async function getLoggedUsuario() { 
    const usuario = await getUsuario(Number(localStorage.getItem("usuarioId")))
    if (usuario == null) {
        throw new Error("Error al obtener usuario desde localStorage")
    }
    return usuario
}