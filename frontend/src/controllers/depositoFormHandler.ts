import { postSaldo } from "../api/billeteraApi"
import { showButtonLoading, showButtonReady } from "../components/spinner"
import { showToast } from "../components/toast"
import type { BilleteraDTO } from "../models/billeteraTypes"

type Args = {
    getLoggedUsuarioBilletera: () => Promise<BilleteraDTO>
    onDepositoSubmit: (resultado: BilleteraDTO) => void
}

export function setupSubastaForm({getLoggedUsuarioBilletera, onDepositoSubmit} : Args) {
    const depositoForm = document.getElementById("deposito-form") as HTMLFormElement
    const montoField = document.getElementById("monto") as HTMLInputElement
    const submitButton = document.getElementById("submit-button") as HTMLButtonElement

    depositoForm.addEventListener("submit", async (event) => {
        event.preventDefault()
        const billetera = await getLoggedUsuarioBilletera()
        const monto = Number(montoField.value)

        showButtonLoading(submitButton)
        try {
            const resultado = await postSaldo(billetera.id, monto)
            showToast(`Se depositarón $ ${monto} en tu cuenta`, "success")
            onDepositoSubmit(resultado)
        } catch (err: any) {
            showToast(err, "error")
            console.error(err)
        } finally {
            showButtonReady(submitButton, "Depositar")
        }

    })
    
}
