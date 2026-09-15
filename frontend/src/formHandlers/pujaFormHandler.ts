import { postPuja } from "../api/pujaApi";
import { getSaldoDisponible } from "../api/usuariosApi";
import type { CrearPujaDTO } from "../models/pujaTypes";
import type { SubastaDetalleDTO } from "../models/subastaTypes";
import type { UsuarioDTO } from "../models/usuarioTypes";
import { validatePujaForm } from "../validation/pujaValidation";
import { toggleErrorDisplay } from "../views/subastaView";

type Args = {
    getCurrentSubasta: () => Promise<SubastaDetalleDTO>
    getLoggedUsuario: () => Promise<UsuarioDTO>
    /**
     * Función a ejecutar una vez confirmada la creación de la puja, se pasa el nuevo estado de la subasta recibido del backend
     */
    onPujaCreated: (subastaUpdated: SubastaDetalleDTO) => Promise<void>
}

export function setupPujaForm({getCurrentSubasta, getLoggedUsuario, onPujaCreated} : Args) {
    const pujaForm = document.getElementById("puja-form") as HTMLFormElement
    pujaForm.addEventListener("submit", async (event) => {
        event.preventDefault()
    
        const formData = new FormData(pujaForm)
        
        const monto = Number(formData.get("monto"))

        // obtener objetos mediante getters para evitar stale objects
        const subasta = await getCurrentSubasta()
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
        try {
            const resultado = await postPuja(Number(subasta.id), crearPujaDTO)
            await onPujaCreated(resultado.subastaDetalle)
        } catch (err: any) {
            alert(`Ocurrió un error al registrar puja: ${err}`)
            console.error(err)
        }
    })
    
}