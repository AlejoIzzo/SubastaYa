import { type BilleteraDTO, type DepositoResultadoDTO } from "../models/billeteraTypes";
import { get, post } from "./client";

export async function getBilletera(billeteraId: number) {
    return await get<BilleteraDTO>(`/billeteras/${billeteraId}`)
}

export async function postSaldo(billeteraId: number, monto: number) {
    return await post<BilleteraDTO>(`/billeteras/${billeteraId}/depositos`, {monto})
}