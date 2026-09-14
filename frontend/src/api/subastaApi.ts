import { get } from "./client";
import { type SubastaCatalogoDTO, type SubastaDetalleDTO } from "../models/subastaTypes";

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