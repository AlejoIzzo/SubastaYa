import { getSubasta } from "../api/subastaApi";
import { renderHeader } from "../components/header";
import { getSaldoDisponible } from "../api/usuariosApi";
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
    toggleErrorDisplay, 
    updateTimer,
    renderSubastaState, 
} from "../views/subastaView";
import { refreshPujasDates, refreshUserPujas } from "../components/pujaCard";
import { getLoggedUsuario } from "../helpers/usuarioHelpers";
import * as signalR from "@microsoft/signalr";
import type { PujaResultadoDTO } from "../models/pujaTypes";
import { showLoading } from "../components/spinner";
import { showToast } from "../components/toast";

const params = new URLSearchParams(window.location.search);
const subastaId = params.get("id");
if (!subastaId)
    throw new Error("Error al obtener ID de subasta desde url")

async function init() {
    const pujaListContainer = document.getElementById("puja-list")!
    showLoading(pujaListContainer)
    
    let subasta = await getSubasta(Number(subastaId))
    let usuario = await getLoggedUsuario()
    
    const headerContainer = document.getElementById("header")!
    renderHeader(headerContainer)

    renderSubasta(subasta)
    renderSubastaState(subasta, usuario)
    renderPujaList(subasta.ultimasPujas, usuario)

    setSaldoUsuario(await getSaldoDisponible(usuario.id))
    updatePujaEstadoTag(subasta, usuario)
    sugerirPuja(subasta.pujaActual!, subasta.incrementoMinimo)

    function applySubastaUpdate(updated: SubastaDetalleDTO) {
        showLoading(pujaListContainer)
        subasta = updated;

        if (updated.pujaActual) {
            updatePujaLider(updated.pujaActual!.monto);
            sugerirPuja(updated.pujaActual!, updated.incrementoMinimo);
        }

        renderPujaList(updated.ultimasPujas, usuario);
        updatePujaEstadoTag(updated, usuario);
        renderSubastaState(subasta, usuario)
    }
    function getCurrentSubasta() {
       return subasta
    }

    setupPujaForm({
        getCurrentSubasta,
        getLoggedUsuario,
        onPujaCreated: async (subastaUpdated: SubastaDetalleDTO) => {
            applySubastaUpdate(subastaUpdated)

            setSaldoUsuario(await getSaldoDisponible(usuario.id))
        }
    })

    // actualizar variable y elementos al cambiar de usuario
    document.addEventListener("usuarioChanged", async () => {
        usuario = await getLoggedUsuario()

        setSaldoUsuario(await getSaldoDisponible(usuario.id))
        updatePujaEstadoTag(subasta, usuario)
        renderSubastaState(subasta, usuario)
        refreshUserPujas(usuario)
    });
    
    // eventos a los que escuchar para mostrar errores (además de form submit)
    const montoInput = document.getElementById("puja-input") as HTMLInputElement
    montoInput.addEventListener("input", async () => {
        const monto = Number(montoInput.value)
        const usuarioSaldoDisponible = await getSaldoDisponible(usuario.id)

        const errorMesage = validatePujaForm({
            monto,
            usuarioSaldoDisponible,
            usuario,
            subasta
        })

        toggleErrorDisplay(errorMesage)
    })

    // timer de subasta
    window.setInterval(() => updateTimer(subasta), 1000)

    // "hace x" de pujas
    window.setInterval(() => refreshPujasDates(), 60000)

    // --- websockets config ---

    // conexión hacia el backend
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7282/hubs/subastas")
        .withAutomaticReconnect()
        .build();

    // escuchar nuevas ofertas de otros usuarios en vivo
    connection.on("RecibirPuja", (pujaResultado: PujaResultadoDTO) => {
        applySubastaUpdate(pujaResultado.subastaDetalle)

        // Si se extendió por anti-sniping:
        if (pujaResultado.antiSnipingActivado) {
            showToast("¡Tiempo extendido por Anti-Sniping (+2 min)!", "info")
        }
    });

    // escuchar cuando el Background Worker incia la subasta
    connection.on("SubastaIniciada", (subsataUpdated: SubastaDetalleDTO) => {
        applySubastaUpdate(subsataUpdated)
        showToast("¡La subasta comenzó!", "info")
    });

    // escuchar cuando el Background Worker cierra la subasta
    connection.on("SubastaFinalizada", (subsataUpdated: SubastaDetalleDTO) => {
        applySubastaUpdate(subsataUpdated)
        showToast("¡La subasta ha finalizado!", "info")
    });

    // Iniciar y unirse a la sala de esta subasta
    await connection.start();
    await connection.invoke("UnirseASubasta", Number(subastaId));
}

init()