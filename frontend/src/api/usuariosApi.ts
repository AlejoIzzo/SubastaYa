import type { BilleteraDTO } from "../models/billeteraTypes";
import type { UsuarioDashboardDTO, UsuarioDTO } from "../models/usuarioTypes";
import { get } from "./client";

export async function getUsuarios() {
    return await get<UsuarioDTO[]>("/usuarios")
}

export async function getUsuario(id: number) {
    return await get<UsuarioDTO>(`/usuarios/${id}`)
}

type UsuarioDashboardQuery = {
    id: number, 
    paginacion: UsuarioDashboardPaginacion
}

export type UsuarioDashboardPaginacion = {
    paginaSubastas: number
    tamanioPaginaSubastas: number
    paginaParticipaciones: number
    tamanioPartipaciones: number
}
export async function getUsuarioDashboard({id, paginacion} : UsuarioDashboardQuery)  {
    const params = new URLSearchParams();
    
    params.set("paginaSubastas", paginacion.paginaSubastas.toString());
    params.set("tamanioSubastas", paginacion.tamanioPaginaSubastas.toString());
    params.set("paginaParticipaciones", paginacion.paginaParticipaciones.toString());
    params.set("tamanioParticipaciones", paginacion.tamanioPartipaciones.toString());

    return await get<UsuarioDashboardDTO>(`/usuarios/${id}/dashboard`, params)
}

export async function getSaldoDisponible(usuarioId: number) {
    return (await get<BilleteraDTO>(`/billeteras/usuario/${usuarioId}`)).saldoDisponible
}