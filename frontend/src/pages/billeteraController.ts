import { renderHeader } from "../components/header"
import { getLoggedUsuarioBilletera } from "../helpers/usuarioHelpers"
import type { BilleteraDTO } from "../models/billeteraTypes"
import { renderBilletera, renderTablaTransaccion } from "../views/billeteraView"
import { setupSubastaForm } from "./depositoFormHandler"

async function init() {
    renderHeader(document.getElementById("header")!)
    
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
        billetera = await getLoggedUsuarioBilletera()

        renderBilletera(billetera)
        renderTablaTransaccion(billetera.transacciones)
    });
}

init()

