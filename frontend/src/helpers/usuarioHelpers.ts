import { getBilletera } from "../api/billeteraApi"
import { getUsuario } from "../api/usuariosApi"

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