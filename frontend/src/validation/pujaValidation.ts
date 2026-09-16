import type { SubastaDetalleDTO } from "../models/subastaTypes"
import type { UsuarioDTO } from "../models/usuarioTypes"

type Args = {
    monto: number
    usuarioSaldoDisponible: number
    usuario: UsuarioDTO
    subasta: SubastaDetalleDTO
}

export function validatePujaForm({
    monto,
    usuarioSaldoDisponible,
    usuario,
    subasta,
} : Args) {
    
    if (usuario.id == subasta.vendedorId) {
        return `No puedes pujar en tu propia subasta`
    }

    if (subasta.pujaActual && usuario.id === subasta.pujaActual.compradorId) {
        return `Ya eres el líder, no puedes pujar contra ti mismo`
    }

    if (monto > usuarioSaldoDisponible) {
        return `El monto ingresado excede tu saldo disponible`
    }

    if (subasta.pujaActual && monto < subasta.pujaActual.monto) {
        return "El monto debe ser mayor que la puja líder actual"
    }

    if (subasta.pujaActual && monto < subasta.pujaActual.monto + subasta.incrementoMinimo) {
        return `El monto debe superar a la puja líder por mínimo $ ${subasta.incrementoMinimo}`
    }

    if (subasta.pujaActual == null && monto < subasta.precioBase + subasta.incrementoMinimo) {
        return `El monto debe superar al precio base por mínimo $ ${subasta.incrementoMinimo}`
    }

    if (Number.isNaN(monto)) {
        return "El monto debe ser un número"
    }

    return null
}
