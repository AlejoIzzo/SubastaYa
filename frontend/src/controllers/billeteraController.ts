import { getTransacciones } from "../api/billeteraApi"
import { renderHeader } from "../components/header"
import { getLoggedUsuarioBilletera } from "../helpers/usuarioHelpers"
import type { BilleteraDTO } from "../models/billeteraTypes"
import { renderBilletera, renderBilleteraLoading, renderTablaTransaccion, renderTablaTransaccionLoading } from "../views/billeteraView"
import { setupSubastaForm } from "./depositoFormHandler"

async function init() {
    renderHeader(document.getElementById("header")!)
    
    renderBilleteraLoading()
    renderTablaTransaccionLoading()
    let billetera = await getLoggedUsuarioBilletera()
    const tamanioPagina = 9

    async function cargarPaginaTransacciones(pagina: number) {
        let transaccionesPaged = await getTransacciones(billetera.id, pagina, tamanioPagina)
        renderTablaTransaccion({
            transaccionesPaged,
            onAnterior: () => cargarPaginaTransacciones(transaccionesPaged.paginaActual - 1),
            onSiguiente: () => cargarPaginaTransacciones(transaccionesPaged.paginaActual + 1),
        })
    }

    renderBilletera(billetera)  
    cargarPaginaTransacciones(1)

    setupSubastaForm({
        getLoggedUsuarioBilletera,
        onDepositoSubmit: (resultado: BilleteraDTO) => {

            billetera = resultado
            renderBilletera(billetera)
            cargarPaginaTransacciones(1)
        }
    })

    // actualizar variable y elementos al cambiar de usuario
    document.addEventListener("usuarioChanged", async () => {
        renderBilleteraLoading()
        renderTablaTransaccionLoading()
        billetera = await getLoggedUsuarioBilletera()
        renderBilletera(billetera)
        cargarPaginaTransacciones(1)
    });
}

init()

