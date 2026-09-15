import type { TransaccionDTO } from "./transaccionTypes"

export type BilleteraDTO = {
    id: number,
    usuarioId: number,
    usuarioNombre: string,
    saldoTotal: number,
    saldoRetenido: number,
    saldoDisponible: number,
    transacciones: TransaccionDTO[]
}

export type CargarSaldoDTO = {
    monto: number
}

export type DepositoResultadoDTO = {
    saldoTotal: number,
    saldoDisponible: number,
    saldoRetenido: number,
    transaccion: TransaccionDTO
}