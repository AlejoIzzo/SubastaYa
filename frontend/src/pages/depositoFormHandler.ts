import { postSaldo } from "../api/billeteraApi"
import type { BilleteraDTO } from "../models/billeteraTypes"

type Args = {
    getLoggedUsuarioBilletera: () => Promise<BilleteraDTO>
    onDepositoSubmit: (resultado: BilleteraDTO) => void
}

export function setupSubastaForm({getLoggedUsuarioBilletera, onDepositoSubmit} : Args) {
    const depositoForm = document.getElementById("deposito-form") as HTMLFormElement
    const montoField = document.getElementById("monto") as HTMLInputElement

    depositoForm.addEventListener("submit", async (event) => {
        event.preventDefault()
        const billetera = await getLoggedUsuarioBilletera()
        const monto = Number(montoField.value)

        try {
            const resultado = await postSaldo(billetera.id, monto)
            onDepositoSubmit(resultado)
        } catch (err: any) {
            alert(`Ocurrió un error cargar saldo: ${err}`)
            console.error(err)
        }
    })
    
}
