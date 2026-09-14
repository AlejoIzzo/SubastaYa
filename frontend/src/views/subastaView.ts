import { createPujaCardElement } from "../components/pujaCard"
import type { PujaDTO } from "../models/pujaTypes"
import type { SubastaDetalleDTO } from "../models/subastaTypes"
import type { UsuarioDTO } from "../models/usuarioTypes"

export function renderSubasta(subasta: SubastaDetalleDTO)  {
    const titulo = document.getElementById("titulo")!
    const vendedorNombre = document.getElementById("vendedor-nombre")!
    const descripcion = document.getElementById("descripcion")!
    const img = document.getElementById("subasta-img")!
    const incrementoMinimo = document.getElementById("incremento-minimo")!

    titulo.textContent = subasta.titulo
    vendedorNombre.textContent = subasta.vendedorNombre
    descripcion.textContent = subasta.descripcion
    img.setAttribute("src", subasta.urlImagen)
    incrementoMinimo.textContent = String(subasta.incrementoMinimo)

    renderSubastaDependentState(subasta)
}

/**
 * Oculta, deshabilita o habilita el formulario de puja dependiendo si el usuario es vendedor, líder u otro
 */
export function renderUserDependentState(subasta: SubastaDetalleDTO, usuario: UsuarioDTO) {
    if (usuario.id === subasta.vendedorId) {
        renderFormState("oculto")
    } else if (subasta.pujaActual && usuario.id === subasta.pujaActual.compradorId) {
        renderFormState("deshabilitado", "No puedes pujar mientras seas el líder")
    } else {
        renderFormState("habilitado")
    }
}

/**
 * Renderiza elementos que dependen del estado de la subasta
 */
export function renderSubastaDependentState(subasta: SubastaDetalleDTO) {
    const subastaResultadoContainer = document.getElementById("subasta-resultado-container")!
    const pujaLiderContainer = document.getElementById("puja-lider-container")!

    const timer = document.getElementById("timer")!
    const timerLabel = document.getElementById("timer-label")!
    const estadoLabel = document.getElementById("estado-label")!
    const pujaLiderLabel = document.getElementById("puja-lider-label")!
    const pujaLiderMonto = document.getElementById("puja-lider-monto")!
    const usuarioEstadoTag = document.getElementById("usuario-puja-estado")!;

    if (subasta.pujaActual) {
        pujaLiderLabel.textContent = "PUJA LÍDER"
        pujaLiderMonto.textContent = `$ ${subasta.pujaActual.monto}`
        usuarioEstadoTag.classList.remove("hidden")
    } else {
        pujaLiderLabel.textContent = "PRECIO BASE"
        pujaLiderMonto.textContent = `$ ${subasta.precioBase}`
        usuarioEstadoTag.classList.add("hidden")
    }

    if (subasta.estado == "ACTIVA") {
        timer.dataset.timerActivo = "true"
        timerLabel.textContent = "TIEMPO RESTANTE"

        estadoLabel.classList.add("hidden")
    } else if (subasta.estado == "PROGRAMADA") {
        timer.dataset.timerActivo = "true"
        timerLabel.textContent = "COMIENZA EN"

        estadoLabel.classList.add("hidden")
        renderFormState("deshabilitado")
    } else { 
        // finalizada o desierta
        timer.dataset.timerActivo = "false"
        
        const subastaResultadoLabel = document.getElementById("subasta-resultado-label")!
        const pujaGanadoraMonto = document.getElementById("puja-ganadora-monto")!

        pujaLiderContainer.classList.add("hidden")
        subastaResultadoContainer.classList.remove("hidden")
            
        estadoLabel.classList.remove("hidden")
        timerLabel.classList.add("hidden")
        timer.classList.add("hidden")

        renderFormState("oculto")

        if (subasta.estado == "FINALIZADA") {
            estadoLabel.textContent = "SUBASTA FINALIZADA"
            subastaResultadoLabel.textContent = "Se subasto por"
            pujaGanadoraMonto.textContent = `$ ${String(subasta.pujaActual?.monto)}`
            
        } else if (subasta.estado == "DESIERTA") {
            estadoLabel.textContent = "SUBASTA DESIERTA"
            subastaResultadoLabel.textContent = "No hubo ningúna puja"
        }
    }

}

export function renderFormState(estado: "habilitado" | "deshabilitado" | "oculto", tooltipText?: string) {
    const pujaForm = document.getElementById("puja-form") as HTMLFormElement
    const formTooltipText = document.getElementById("form-tooltip-text") as HTMLFormElement
    
    switch (estado) {
        case "oculto":
            pujaForm.classList.add("hidden")
            pujaForm.classList.remove("disabled")
            formTooltipText.textContent = ""

            pujaForm.querySelectorAll<HTMLElement>("input, button").forEach(x => x.removeAttribute("disabled"))
            break;

        case "deshabilitado":
            pujaForm.classList.remove("hidden")
            pujaForm.classList.add("disabled")
            formTooltipText.textContent = tooltipText ?? ""

            pujaForm.querySelectorAll<HTMLElement>("input, button").forEach(x => x.setAttribute("disabled", "true"))
            break;
        case "habilitado": 
            pujaForm.classList.remove("hidden")
            pujaForm.classList.remove("disabled")
            formTooltipText.textContent = ""
            
            pujaForm.querySelectorAll<HTMLElement>("input, button").forEach(x => x.removeAttribute("disabled"))

            break;
    }
}

export function updatePujaLider(pujaActual: number) {
    const pujaLiderMonto = document.getElementById("puja-lider-monto")!

    pujaLiderMonto.textContent = `$ ${pujaActual}`
}

export function setSaldoUsuario(saldoDisponible: number) {
    const usuarioSaldo = document.getElementById("usuario-saldo")!
    usuarioSaldo.textContent = String(saldoDisponible)
}

export function renderPujaList(pujas: PujaDTO[], usuario: UsuarioDTO) {
    const pujasListContainer = document.getElementById("puja-list")!
    pujasListContainer.replaceChildren()
    
    for (let puja of pujas) {
        pujasListContainer.appendChild(createPujaCardElement(puja, usuario))
    }
}

export function sugerirPuja(pujaLider: PujaDTO, incrementoMinimo: number) {
    const montoInput = document.getElementById("puja-input") as HTMLInputElement
    if (pujaLider)
        montoInput.value = String(pujaLider.monto + incrementoMinimo)
}

export function updatePujaEstadoTag(subasta: SubastaDetalleDTO, usuario: UsuarioDTO) {
    const tag = document.getElementById("usuario-puja-estado")!;
    const icon = tag.querySelector<HTMLElement>("#puja-estado-icon")!;
    const text = tag.querySelector<HTMLParagraphElement>("#puja-estado-texto")!;

    const estados: any = {
        lider: {
            className: "lider",
            icon: "trophy-outline",
            text: "Estás liderando"
        },
        superado: {
            className: "superado",
            icon: "warning-outline",
            text: "Tu puja fue superada"
        },
        otroLider: {
            className: "otro-lider",
            icon: "information-circle-outline",
            text: `"${subasta.pujaActual?.compradorNombre}" esta liderando`
        },
        sinPujas: {
            className: "sin-pujas",
            icon: "information-circle-outline",
            text: "Aún no hay pujas"
        }
    };

    const estado = estados[getPujaEstado(subasta, usuario)]
    tag.classList.remove("lider", "superado", "otro-lider", "sin-pujas");
    tag.classList.add(estado.className);

    icon.setAttribute("name", estado.icon);
    text.textContent = estado.text;
}

function getPujaEstado(subasta: SubastaDetalleDTO, usuario: UsuarioDTO) {
    if (subasta.ultimasPujas.length == 0) {
        return 'sinPujas'
    }
    
    if (subasta.pujaActual && subasta.pujaActual.compradorId === usuario.id) {
        return "lider"
    }
    else if (subasta.ultimasPujas.find(x => x.compradorId === usuario.id)) {
        // usuario no es líder pero esta en pujas recientes
        return "superado"
    } 
    else {
        return "otroLider"
    }
}

export function toggleErrorDisplay(error: string | null) {
    const errorContainer = document.querySelector(".form-error-container") as HTMLFormElement
    const errorMessageEl = document.querySelector(".form-error-message") as HTMLFormElement
    
    if (error) {
        errorContainer.classList.remove("hidden")
        errorMessageEl.textContent = error
    } else {
        errorContainer.classList.add("hidden")
        errorMessageEl.textContent = ""
    }
}

export function updateTimer(subasta: SubastaDetalleDTO) {    
    const timer = document.getElementById("timer")!
    if (timer.dataset.timerActivo == "false")
        return

    // const timerLabel = document.getElementById("timer-label")!

    const objetivo = subasta.estado == "ACTIVA" 
                        ? new Date(String(subasta.fechaFin)).getTime() 
                        : new Date(String(subasta.fechaInicio)).getTime();
    
    const remaining = objetivo - Date.now();
    const totalSegundos = Math.floor(remaining / 1000);

    if (remaining <= 0) {
        timer.dataset.timerActivo = "false"
        
        timer.textContent = timer.dataset.estado == "ACTIVA" ? "Finalizada" : "Comenzando..."
        return
    }

    const dias = Math.floor(totalSegundos / 86400);
    const horas = String(Math.floor((totalSegundos % 86400) / 3600)).padStart(2, "0");
    const minutos = String(Math.floor((totalSegundos % 3600) / 60)).padStart(2, "0");
    const segundos = String(totalSegundos % 60).padStart(2, "0");

    const timerText = dias > 0 
                ? `${dias}d ${horas}:${minutos}:${segundos}` 
                : `${horas}:${minutos}:${segundos}` 
    
    timer.textContent = timerText
}