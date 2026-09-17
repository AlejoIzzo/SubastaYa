import { get, post } from "./client";
import { type CrearSubastaDTO, type SubastaCatalogoDTO, type SubastaCreadaDTO, type SubastaDetalleDTO } from "../models/subastaTypes";
import type { PaginatedResultDTO } from "../models/paginationType";

export async function getSubastaCatalogo(
    filtroFormData?: FormData,
    pagina: number = 1, 
    tamanioPagina: number = 12
) {
    // this should convert the filter form data into url params
    
    const params = new URLSearchParams();
    if (filtroFormData) {
        for (const [key, value] of filtroFormData.entries()) {
            if (value != null && value !== "all") {
                params.set(key, value.toString());
            }
        }
    }

    params.set("pagina", pagina.toString());
    params.set("tamanioPagina", tamanioPagina.toString());

    return await get<PaginatedResultDTO<SubastaCatalogoDTO>>('/subastas', params)
}

export async function getSubasta(id: number) {
    return await get<SubastaDetalleDTO>(`/subastas/${id}`)
}

export async function postSubasta(dto: CrearSubastaDTO) {
    return await post<SubastaCreadaDTO>(`/subastas`, dto)
}