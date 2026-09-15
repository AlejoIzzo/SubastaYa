import { getSubasta } from "../api/subastaApi";
import { renderHeader } from "../components/header";
import { getSaldoDisponible, getUsuario } from "../api/usuariosApi";
import { validatePujaForm } from "../validation/pujaValidation";
import { setupPujaForm } from "../formHandlers/pujaFormHandler";
import type { SubastaDetalleDTO } from "../models/subastaTypes";
import { 
    updatePujaLider, 
    setSaldoUsuario, 
    renderPujaList, 
    renderSubasta, 
    sugerirPuja, 
    updatePujaEstadoTag, 
    renderUserDependentState, 
    toggleErrorDisplay, 
    renderSubastaDependentState, 
    updateTimer 
} from "../views/subastaView";
import { refreshPujasDates, refreshUserPujas } from "../components/pujaCard";
import { getLoggedUsuario } from "../helpers/usuarioHelpers";

const params = new URLSearchParams(window.location.search);
const subastaId = params.get("id");
if (!subastaId)
    throw new Error("Error al obtener ID de subasta desde url")

async function init() {
    let subasta = await getSubasta(Number(subastaId))
    let usuario = await getLoggedUsuario()
    
    const headerContainer = document.getElementById("header")!
    renderHeader(headerContainer)

    renderUserDependentState(subasta, usuario)
    renderSubasta(subasta)
    renderPujaList(subasta.ultimasPujas, usuario)

    setSaldoUsuario(await getSaldoDisponible(usuario.id))
    updatePujaEstadoTag(subasta, usuario)
    sugerirPuja(subasta.pujaActual!, subasta.incrementoMinimo)

    setupPujaForm({
        getCurrentSubasta,
        getLoggedUsuario,
        onPujaCreated: async (subastaUpdated: SubastaDetalleDTO) => {
            subasta = subastaUpdated

            updatePujaLider(subasta.pujaActual!.monto)
            setSaldoUsuario(await getSaldoDisponible(usuario.id))
            renderPujaList(subasta.ultimasPujas, usuario)
            updatePujaEstadoTag(subasta, usuario)
            renderUserDependentState(subasta, usuario)
            sugerirPuja(subasta.pujaActual!, subasta.incrementoMinimo)
        }
    })

    // actualizar variable y elementos al cambiar de usuario
    document.addEventListener("usuarioChanged", async () => {
        usuario = await getLoggedUsuario()

        setSaldoUsuario(await getSaldoDisponible(usuario.id))
        updatePujaEstadoTag(subasta, usuario)
        renderUserDependentState(subasta, usuario)
        renderSubastaDependentState(subasta)
        refreshUserPujas(usuario)
    });
    
    // eventos a los que escuchar para mostrar errores (además de form submit)
    const montoInput = document.getElementById("puja-input") as HTMLInputElement
    ["input", "focus"].forEach(evento => montoInput.addEventListener(evento, async () => {
        const monto = Number(montoInput.value)
        const usuarioSaldoDisponible = await getSaldoDisponible(usuario.id)

        const errorMesage = validatePujaForm({
            monto,
            usuarioSaldoDisponible,
            usuario,
            subasta
        })

        toggleErrorDisplay(errorMesage)
    }))

    // timer de subasta
    window.setInterval(() => updateTimer(subasta), 1000)

    // "hace x" de pujas
    window.setInterval(() => refreshPujasDates(), 60000)
}

init()

// helpers


async function getCurrentSubasta() {
    return await getSubasta(Number(subastaId))
}