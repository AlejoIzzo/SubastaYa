import { renderHeader } from "../components/header"
import { showLoading, showTableLoadingRow } from "../components/spinner"
import { getLoggedUsuarioBilletera } from "../helpers/usuarioHelpers"
import type { BilleteraDTO } from "../models/billeteraTypes"
import { renderBilletera, renderBilleteraLoading, renderTablaTransaccion, renderTablaTransaccionLoading } from "../views/billeteraView"
import { setupSubastaForm } from "./depositoFormHandler"

async function init() {
    renderHeader(document.getElementById("header")!)
    
    renderBilleteraLoading()
    renderTablaTransaccionLoading()
    let billetera = await getLoggedUsuarioBilletera()
    
    renderBilletera(billetera)
    renderTablaTransaccion(billetera.transacciones)

    setupSubastaForm({
        getLoggedUsuarioBilletera,
        onDepositoSubmit: (resultado: BilleteraDTO) => {

            billetera = resultado
            renderBilletera(billetera)
            renderTablaTransaccion(billetera.transacciones)
        }
    })

    // actualizar variable y elementos al cambiar de usuario
    document.addEventListener("usuarioChanged", async () => {
        renderBilleteraLoading()
        renderTablaTransaccionLoading()
        billetera = await getLoggedUsuarioBilletera()
        renderBilletera(billetera)
        renderTablaTransaccion(billetera.transacciones)
    });
}

init()

