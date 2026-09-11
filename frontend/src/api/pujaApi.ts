import type { CrearPujaDTO, PujaDTO, PujaResultadoDTO } from "../models/pujaTypes"
import { get, post } from "./client"

export async function getPujas(subastaId: number) {
    return await get<PujaDTO[]>(`/subastas/${subastaId}/pujas`)
}

export async function postPuja(subastaId: number, dto: CrearPujaDTO) {
    return await post<PujaResultadoDTO>(`/subastas/${subastaId}/pujas`, dto)
}