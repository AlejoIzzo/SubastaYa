export type PujaDTO = {
    id: number
    subastaId: number
    subastaTitulo: string
    compradorId: number
    compradorNombre: string
    monto: number
    fecha: Date
}

export type CrearPujaDTO = {
    compradorId: number
    monto: number
}

export type PujaResultadoDTO = {
    id: number
    subastaId: number
    compradorId: number
    compradorNombre : string
    monto: number
    fecha: Date
    antiSnipingActivado : boolean
    fechaFinSubasta: Date
    mensaje: string
}