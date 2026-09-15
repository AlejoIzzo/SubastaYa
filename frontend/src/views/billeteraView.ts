import { formatDateTime } from "../helpers/dateHelpers"
import type { BilleteraDTO } from "../models/billeteraTypes"
import type { TransaccionDTO } from "../models/transaccionTypes"

export function renderBilletera(billetera: BilleteraDTO)  {
    const saldoTotal = document.getElementById("saldo-total")!
    const saldoDisponible = document.getElementById("saldo-disponible")!
    const saldoRetenido = document.getElementById("saldo-retenido")!

    saldoTotal.textContent = `$ ${billetera.saldoTotal}`
    saldoDisponible.textContent = `$ ${billetera.saldoDisponible}`
    saldoRetenido.textContent = `$ ${billetera.saldoRetenido}`
}

export function renderTablaTransaccion(transacciones: TransaccionDTO[]) {
    const transaccionesTabla = document.getElementById("transacciones-tabla-body")!

    transaccionesTabla.replaceChildren()
    for (let t of transacciones) {
        transaccionesTabla.appendChild(createTransaccionFila(t))
    }
}

function createTransaccionFila(transaccion: TransaccionDTO) {
    const rowTemplate = document.getElementById("transaccion-row-template") as HTMLTemplateElement
    const clone = rowTemplate.content.cloneNode(true) as DocumentFragment

    const fecha = clone.querySelector(".row-fecha")!
    const descripcion = clone.querySelector(".row-descripcion")!
    const subasta = clone.querySelector(".row-subasta")!
    const monto = clone.querySelector(".row-monto")!

    fecha.textContent = formatDateTime(new Date(transaccion.fecha))
    monto.textContent = `$ ${transaccion.monto}`

    if (transaccion.tipo == "LIBERACION") {
        descripcion.textContent = `Liberación por puja superada`
        subasta.textContent = transaccion.subastaTitulo
    } else if (transaccion.tipo == "RETENCION") {
        descripcion.textContent = `Retención por puja`
        subasta.textContent = transaccion.subastaTitulo
    } else if (transaccion.tipo == "DEPOSITO") {
        descripcion.textContent = `Deposito de saldo`
        subasta.textContent = " - "
    }

    return clone
}

// export function updateTablaTransaccion(nuevaTransaccion: TransaccionDTO) {
//     const transaccionesTabla = document.getElementById("transacciones-tabla")!

//     transaccionesTabla.appendChild(createTransaccionFila(nuevaTransaccion))
// }

