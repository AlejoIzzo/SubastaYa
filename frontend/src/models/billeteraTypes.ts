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