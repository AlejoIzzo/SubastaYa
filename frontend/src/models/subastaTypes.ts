import type { PujaDTO } from "./pujaTypes"

export type SubastaCatalogoDTO = {
    id: number,
    titulo: string,
    descripcion: string,
    precioBase: number,
    pujaActual: number,
    cantidadPujas: number,
    urlImagen: string,
    fechaInicio: Date, 
    fechaFin: Date, 
    estado: string,
    categoriaId: number,
    categoriaNombre: string,
    vendedorNombre: string,
}
export type SubastaDetalleDTO = {
    id: number 
    titulo: string 
    descripcion: string 
    precioBase: number 
    pujaActual?: PujaDTO 
    cantidadPujas: number 
    urlImagen: string 
    fechaInicio: Date 
    fechaFin: Date 
    estado: string  
    vendedorNombre: string 
    vendedorId: number 
    ultimasPujas: PujaDTO[]
    incrementoMinimo: number 
    categoriaNombre: string 
}

export type CrearSubastaDTO = {
    titulo: string
    descripcion: string
    categoriaId: number
    precioBase: number
    urlImagen: string
    fechaInicio?: Date
    fechaFin: Date
    vendedorId: number
    incrementoMinimo: number
}