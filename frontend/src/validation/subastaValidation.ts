import type { CrearSubastaDTO } from "../models/subastaTypes"

export type SubastaValidationErrors = {
    titulo?: string
    descripcion?: string
    categoriaId?: string
    precioBase?: string
    urlImagen?: string
    fechaInicio?: string
    fechaFin?: string
    vendedorId?: string
    incrementoMinimo?: string
}

export function validateSubastaForm(subasta: CrearSubastaDTO): SubastaValidationErrors {
    const errors: SubastaValidationErrors = {}
    
    // si fecha inicio es null (comenzar de inmediato), validar como si fuera en el instante actual
    const inicio = subasta.fechaInicio ? subasta.fechaInicio : new Date() 

    if (subasta.fechaInicio) {
        if (subasta.fechaInicio < new Date())
            errors.fechaInicio = "La subasta no puede comenzar en el pasado"

        if (subasta.fechaInicio > subasta.fechaFin)
            errors.fechaFin = "La fecha de cierre debe ser posterior a la fecha de inicio"
    }

    if (inicio < new Date())
        errors.fechaInicio = "La subasta no puede comenzar en el pasado"

    
    const diff = subasta.fechaFin.getTime() - inicio.getTime()
    const duracionMinutos = Math.floor(diff / (1000 * 60));
    // deshabilitar validación de duración para testear websockets
    if (duracionMinutos <= 5) {
        errors.fechaFin = "La subasta debe durar por lo menos 5 minutos"
    }
    
    if (inicio > subasta.fechaFin)
        errors.fechaFin = "La fecha de cierre debe ser posterior a las de inicio"

    if (subasta.incrementoMinimo < 100) {
        errors.incrementoMinimo = "El incremento mínimo debe ser mayor a 100"
    }

    return errors
}
