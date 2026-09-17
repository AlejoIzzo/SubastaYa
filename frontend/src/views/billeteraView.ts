import { renderPaginacion } from "../components/paginacionNav"
import { showLoading, showTableLoadingRow } from "../components/spinner"
import { formatDateTime } from "../helpers/dateHelpers"
import { formatNumber } from "../helpers/stringHelpers"
import type { BilleteraDTO } from "../models/billeteraTypes"
import type { PaginatedResultDTO } from "../models/paginationType"
import type { TransaccionDTO } from "../models/transaccionTypes"

const saldoTotal = document.getElementById("saldo-total")!
const saldoDisponible = document.getElementById("saldo-disponible")!
const saldoRetenido = document.getElementById("saldo-retenido")!

export function renderBilleteraLoading() {
    showLoading(saldoTotal)
    showLoading(saldoDisponible)
    showLoading(saldoRetenido)
}
export function renderBilletera(billetera: BilleteraDTO)  {
    saldoTotal.textContent = `$ ${formatNumber(billetera.saldoTotal)}`
    saldoDisponible.textContent = `$ ${formatNumber(billetera.saldoDisponible)}`
    saldoRetenido.textContent = `$ ${formatNumber(billetera.saldoRetenido)}`
}

const transaccionesTabla = document.getElementById("transacciones-tabla-body")!
export function renderTablaTransaccionLoading() {
    showTableLoadingRow(transaccionesTabla, 4)
}

type RenderTablaTransaccionArgs = {
    transaccionesPaged: PaginatedResultDTO<TransaccionDTO>,
    onSiguiente: () => void 
    onAnterior: () => void
}
export function renderTablaTransaccion({transaccionesPaged, onSiguiente, onAnterior}: RenderTablaTransaccionArgs) {
    transaccionesTabla.replaceChildren()
    
    const emptyMessage = document.querySelector(".empty-message")!
    const tablaHead = document.querySelector(".tabla thead")!
    
    const transacciones = transaccionesPaged.items

    if (transacciones.length <= 0) {
        emptyMessage.classList.remove("hidden")
        tablaHead.classList.add("hidden")
    } else {
        emptyMessage.classList.add("hidden")
        tablaHead.classList.remove("hidden")
    }

    for (let t of transacciones) {
        transaccionesTabla.appendChild(createTransaccionFila(t))
    }

    renderPaginacion({
        paginacionNavId: "paginacion",
        resultado: transaccionesPaged,
        onSiguiente: onSiguiente,
        onAnterior: onAnterior,
    })
}

function createTransaccionFila(transaccion: TransaccionDTO) {
    const rowTemplate = document.getElementById("transaccion-row-template") as HTMLTemplateElement
    const clone = rowTemplate.content.cloneNode(true) as DocumentFragment

    const fecha = clone.querySelector(".row-fecha")!
    const descripcion = clone.querySelector(".row-descripcion")!
    const subasta = clone.querySelector(".row-subasta")!
    const monto = clone.querySelector(".row-monto")!

    fecha.textContent = formatDateTime(new Date(transaccion.fecha))
    monto.textContent = `$ ${formatNumber(transaccion.monto)}`

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