import type { PujaDTO } from "./pujaTypes"

export type UsuarioDTO = {
    id: number,
    nombre: string
    email: string
    fechaRegistro: Date
    billeteraId: number
}

export type UsuarioDashboardDTO = {
    usuarioEstadisticas: UsuarioEstadisticasDTO,
    usuarioSubastas: UsuarioSubastaDTO[],
    usuarioParticipacionSubastas: UsuarioParticipacionSubastaDTO[]
}

export type UsuarioEstadisticasDTO = {
    subastasActivas: number
    subastasGanadas: number
    totalPujado: number
    totalRecaudado: number
}

export type UsuarioSubastaDTO = {
    id: number 
    titulo: string 
    precioBase: number 
    pujaLider?: PujaDTO 
    cantidadPujas: number 
    urlImagen: string 
    fechaInicio: string
    fechaFin: string
    estado: string  
    vendedorId: number 
}

export type UsuarioParticipacionSubastaDTO = {
    id: number 
    titulo: string 
    precioBase: number 
    pujaLider: PujaDTO 
    ultimaPujaUsuario: PujaDTO
    urlImagen: string 
    fechaInicio: string
    fechaFin: string
    estado: string  
    vendedorId: number 
}