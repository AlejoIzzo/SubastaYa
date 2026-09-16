import { getBilletera } from "../api/billeteraApi"
import { getUsuario, getUsuarioDashboard } from "../api/usuariosApi"

export async function getLoggedUsuario() { 
    const usuario = await getUsuario(Number(localStorage.getItem("usuarioId")))
    if (usuario == null) {
        throw new Error("Error al obtener usuario desde localStorage")
    }
    return usuario
}

export async function getLoggedUsuarioBilletera() {
    const loggedUsuario = await getLoggedUsuario()
    return await getBilletera(loggedUsuario.billeteraId)
}

export async function getLoggedUsuarioDashboard() {
    const loggedUsuario = await getLoggedUsuario()
    return await getUsuarioDashboard(loggedUsuario.id)
}