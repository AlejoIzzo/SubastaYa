import { get, post } from "./client";
import { type CrearSubastaDTO, type SubastaCatalogoDTO, type SubastaCreadaDTO, type SubastaDetalleDTO } from "../models/subastaTypes";

export async function getSubastaCatalogo(filtroFormData?: FormData) {
    // this should convert the filter form data into url params
    
    const params = new URLSearchParams();
    if (filtroFormData) {
        for (const [key, value] of filtroFormData.entries()) {
            if (value != null && value !== "all") {
                params.set(key, value.toString());
            }
        }
    }

    return await get<SubastaCatalogoDTO[]>('/subastas', params)
}

export async function getSubasta(id: number) {
    return await get<SubastaDetalleDTO>(`/subastas/${id}`)
}

export async function postSubasta(dto: CrearSubastaDTO) {
    return await post<SubastaCreadaDTO>(`/subastas`, dto)
}