import { getBilletera } from "../api/billeteraApi"
import { getUsuario, getUsuarioDashboard, getUsuarios, type UsuarioDashboardPaginacion } from "../api/usuariosApi"

export async function getLoggedUsuario() { 
    const loggedUserId = Number(sessionStorage.getItem("usuarioId") ?? -1)

    let usuario;
    if (loggedUserId === -1) {
        const usuarios = await getUsuarios()
        usuario = await getUsuario(usuarios[0].id)
    } else {
        usuario = await getUsuario(loggedUserId)
    }

    return usuario
}

export async function getLoggedUsuarioBilletera() {
    const loggedUsuario = await getLoggedUsuario()
    return await getBilletera(loggedUsuario.billeteraId)
}

export async function getLoggedUsuarioDashboard({
    paginaSubastas,
    tamanioPaginaSubastas,
    paginaParticipaciones,
    tamanioPartipaciones
}: UsuarioDashboardPaginacion) {
    const loggedUsuario = await getLoggedUsuario()

    return await getUsuarioDashboard({
        id: loggedUsuario.id, 
        paginacion: {
            paginaSubastas,
            tamanioPaginaSubastas,
            paginaParticipaciones,
            tamanioPartipaciones,
        }
    })
}