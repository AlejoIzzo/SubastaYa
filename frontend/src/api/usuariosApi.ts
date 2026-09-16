import type { BilleteraDTO } from "../models/billeteraTypes";
import type { UsuarioDashboardDTO, UsuarioDTO } from "../models/usuarioTypes";
import { get } from "./client";

export async function getUsuarios() {
    return await get<UsuarioDTO[]>("/usuarios")
}

export async function getUsuario(id: number) {
    return await get<UsuarioDTO>(`/usuarios/${id}`)
}

export async function getUsuarioDashboard(id: number) {
    return await get<UsuarioDashboardDTO>(`/usuarios/${id}/dashboard`)
}

export async function getSaldoDisponible(usuarioId: number) {
    return (await get<BilleteraDTO>(`/billeteras/usuario/${usuarioId}`)).saldoDisponible
}