import { postSubasta } from "../api/subastaApi";
import { showButtonLoading, showButtonReady } from "../components/spinner";
import { showToast } from "../components/toast";
import { formatLocalDateTime } from "../helpers/dateHelpers";
import type { CrearSubastaDTO, SubastaCreadaDTO } from "../models/subastaTypes";
import type { UsuarioDTO } from "../models/usuarioTypes";
import { validateSubastaForm, type SubastaValidationErrors } from "../validation/subastaValidation";

type Args = {
    getLoggedUsuario: () => Promise<UsuarioDTO>
    onSubastaCreated: (subasta: SubastaCreadaDTO) => void
}

export function setupSubastaForm({getLoggedUsuario, onSubastaCreated} : Args) {
    const comenzarInmediatoCheckbox = document.getElementById("comenzarInmediato") as HTMLInputElement
    
    const fechaInicio = document.getElementById("fechaInicio") as HTMLInputElement
    const fechaFin = document.getElementById("fechaFin") as HTMLInputElement
    fechaInicio.setAttribute("min", formatLocalDateTime(new Date()))
    fechaFin.setAttribute("min", formatLocalDateTime(new Date()))
    
    comenzarInmediatoCheckbox.addEventListener("change", () => {
        if (comenzarInmediatoCheckbox.checked) {
            fechaInicio.removeAttribute("required")
            fechaInicio.disabled = true
        } else {
            fechaInicio.setAttribute("required", "")
            fechaInicio.disabled = false
        }
    })

    const subastaForm = document.getElementById("subasta-form") as HTMLFormElement
    const submitButton = document.getElementById("submit-button") as HTMLButtonElement

    subastaForm.addEventListener("submit", async (event) => {
        event.preventDefault()

        const formData = new FormData(subastaForm)
        
        // obtener objetos mediante getters para evitar stale objects
        const usuario = await getLoggedUsuario()

        const comenzarInmediato = formData.get("comenzarInmediato") == "on" ? true : false
        const fechaInicio: Date | null = comenzarInmediato == false
                                ? new Date(formData.get("fechaInicio")!.toString())
                                : null

        const crearSubastaDTO: CrearSubastaDTO = {
            vendedorId: usuario.id,
            fechaInicio: fechaInicio,
            
            titulo: formData.get("titulo")!.toString(),
            descripcion: formData.get("descripcion")!.toString(),
            categoriaId: Number(formData.get("categoriaId")),
            precioBase: Number(formData.get("precioBase")),
            urlImagen: formData.get("urlImagen")!.toString(),
            fechaFin: new Date(formData.get("fechaFin")!.toString()),
            incrementoMinimo: Number(formData.get("incrementoMinimo")),
        };
        
        clearValidationErrors()
        const errors = validateSubastaForm(crearSubastaDTO)
        if (Object.keys(errors).length > 0) {
            showValidationErrors(errors);
            return
        }

        showButtonLoading(submitButton, "Publicando...");
        try {
            const resultado = await postSubasta(crearSubastaDTO)
            onSubastaCreated(resultado)
        } catch (err: any) {
            showToast(err.message, "error")
            console.error(err)
            showButtonReady(submitButton, "Publicar");
        }
    })
    
}

function showValidationErrors(errors: SubastaValidationErrors): void {
    for (const [field, message] of Object.entries(errors)) {
        showFieldError(field, message);
    }
}

function showFieldError(fieldName: string, error: string) {
    const field = document.getElementById(fieldName)!
    const errorTextEl = document.getElementById(`${fieldName}Error`)!

    errorTextEl.textContent = error
    field.classList.add("error")
    errorTextEl.classList.remove("hidden")
}

function clearValidationErrors() {
    ["titulo", "descripcion", "categoriaId", "urlImagen", "precioBase", "incrementoMinimo", "fechaInicio", "fechaFin"].forEach(x => {
        clearFieldError(x)
    })
}

function clearFieldError(fieldName: string) {
    const field = document.getElementById(fieldName)!
    const errorTextEl = document.getElementById(`${fieldName}Error`)!

    field.classList.remove("error")
    errorTextEl.classList.add("hidden");
    errorTextEl.textContent = "";
}