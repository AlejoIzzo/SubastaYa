import { type categoriaDTO } from "../models/categoriaTypes";
import { get } from "./client";

export async function getCategorias() {
    return await get<categoriaDTO[]>("/categorias");
}