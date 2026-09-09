import { get } from "./client";
import { type SubastaCatalogoDTO } from "../models/subastaTypes";

export async function getSubastaCatalogo() {
    return await get<SubastaCatalogoDTO[]>('/subastas')
}