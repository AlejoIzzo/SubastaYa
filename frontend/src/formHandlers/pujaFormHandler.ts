import { postPuja } from "../api/pujaApi";
import { getSaldoDisponible } from "../api/usuariosApi";
import { showButtonLoading, showButtonReady } from "../components/spinner";
import { showToast } from "../components/toast";
import type { CrearPujaDTO } from "../models/pujaTypes";
import type { SubastaDetalleDTO } from "../models/subastaTypes";
import type { UsuarioDTO } from "../models/usuarioTypes";
import { validatePujaForm } from "../validation/pujaValidation";
import { toggleErrorDisplay } from "../views/subastaView";

type Args = {
    getCurrentSubasta: () => SubastaDetalleDTO
    getLoggedUsuario: () => Promise<UsuarioDTO>
    /**
     * Función a ejecutar una vez confirmada la creación de la puja, se pasa el nuevo estado de la subasta recibido del backend
     */
    onPujaCreated: (subastaUpdated: SubastaDetalleDTO) => Promise<void>
}

export function setupPujaForm({getCurrentSubasta, getLoggedUsuario, onPujaCreated} : Args) {
    const pujaForm = document.getElementById("puja-form") as HTMLFormElement
    const submitButton = document.getElementById("submit-button") as HTMLButtonElement
    pujaForm.addEventListener("submit", async (event) => {
        event.preventDefault()
    
        const formData = new FormData(pujaForm)
        
        const monto = Number(formData.get("monto"))

        // obtener objetos mediante getters para evitar stale objects
        const subasta = getCurrentSubasta()
        const usuario = await getLoggedUsuario()
        
        const usuarioSaldoDisponible = await getSaldoDisponible(usuario.id)
        const error = validatePujaForm({
            usuario,
            usuarioSaldoDisponible,
            subasta,
            monto, 
        })

        toggleErrorDisplay(error)
        if (error)
            return

        const crearPujaDTO: CrearPujaDTO = {
            compradorId: usuario.id,
            monto: monto
        }
        
        showButtonLoading(submitButton)
        
        try {
            const resultado = await postPuja(Number(subasta.id), crearPujaDTO)
            showToast("Puja realizada correctamente", "success")
            await onPujaCreated(resultado.subastaDetalle)
        } catch (err: any) {
            showToast(err.message, "error")
            console.error(err)
        } finally { 
            showButtonReady(submitButton, "Pujar");
        }
    })
    
}