import { type BilleteraDTO } from "../models/billeteraTypes";
import { type PaginatedResultDTO } from "../models/paginationType";
import type { TransaccionDTO } from "../models/transaccionTypes";
import { get, post } from "./client";

export async function getBilletera(billeteraId: number) {
    return await get<BilleteraDTO>(`/billeteras/${billeteraId}`)
}

export async function postSaldo(billeteraId: number, monto: number) {
    return await post<BilleteraDTO>(`/billeteras/${billeteraId}/depositos`, {monto})
}

export async function getTransacciones(billeteraId: number, pagina: number, tamanioPagina: number) {
    const params = new URLSearchParams();
    
    params.set("pagina", pagina.toString());
    params.set("tamanioPagina", tamanioPagina.toString());

    return await get<PaginatedResultDTO<TransaccionDTO>>(`/billeteras/${billeteraId}/transacciones`, params)
}