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
}

export function renderSubastaState(subasta: SubastaDetalleDTO, usuario: UsuarioDTO) {
    const timer = document.getElementById("timer")!
    const timerLabel = document.getElementById("timer-label")!
    const estadoLabel = document.getElementById("estado-label")!
    const pujaLiderLabel = document.getElementById("puja-lider-label")!
    const pujaLiderMonto = document.getElementById("puja-lider-monto")!
    const usuarioEstadoTag = document.getElementById("usuario-puja-estado")!
    const pujaLiderContainer = document.getElementById("puja-lider-container")!
    const subastaResultadoContainer = document.getElementById("subasta-resultado-container")!

    // Renderizar elementos que dependen de si hay al menos un puja
    if (subasta.pujaActual) {
        pujaLiderLabel.textContent = "PUJA LÍDER"
        pujaLiderMonto.textContent = `$ ${subasta.pujaActual.monto}`
        usuarioEstadoTag.classList.remove("hidden")
    } else {
        pujaLiderLabel.textContent = "PRECIO BASE"
        pujaLiderMonto.textContent = `$ ${subasta.precioBase}`
        usuarioEstadoTag.classList.add("hidden")
    }

    // Resetear elementos que dependen del estado
    estadoLabel.classList.add("hidden")
    timer.classList.remove("hidden")
    timerLabel.classList.remove("hidden")
    pujaLiderContainer.classList.remove("hidden")
    subastaResultadoContainer.classList.add("hidden")

    switch (subasta.estado) {
        case "ACTIVA": {
            timer.dataset.timerActivo = "true"
            timerLabel.textContent = "TIEMPO RESTANTE"

            if (usuario.id === subasta.vendedorId) {
                renderFormState("oculto")
            } else if (subasta.pujaActual?.compradorId === usuario.id) {
                renderFormState("deshabilitado", "No puedes pujar mientras seas el líder")
            } else {
                renderFormState("habilitado")
            }

            break;
        }

        case "PROGRAMADA": {
            timer.dataset.timerActivo = "true"
            timerLabel.textContent = "COMIENZA EN"

            if (usuario.id === subasta.vendedorId) {
                renderFormState("oculto")
            } else {
                renderFormState(
                    "deshabilitado",
                    "No puedes pujar hasta que la subasta comience"
                );
            }
  
            break;
        }

        case "FINALIZADA": {
            timer.dataset.timerActivo = "false";
            timer.classList.add("hidden");
            timerLabel.classList.add("hidden");

            estadoLabel.textContent = "SUBASTA FINALIZADA";
            estadoLabel.classList.remove("hidden");

            subastaResultadoContainer.classList.remove("hidden");
            pujaLiderContainer.classList.add("hidden");

            const subastaResultadoLabel = document.getElementById("subasta-resultado-label")!;
            const pujaGanadoraMonto = document.getElementById("puja-ganadora-monto")!;

            subastaResultadoLabel.textContent = subasta.pujaActual?.compradorId == usuario.id
                                                    ? "Ganaste la subasta por"
                                                    : "Se subastó por"

            pujaGanadoraMonto.textContent = `$ ${subasta.pujaActual?.monto ?? ""}`;

            renderFormState("oculto");

            break;
        }

        case "DESIERTA": {
            timer.dataset.timerActivo = "false";
            timer.classList.add("hidden");
            timerLabel.classList.add("hidden");

            estadoLabel.textContent = "SUBASTA DESIERTA";
            estadoLabel.classList.remove("hidden");

            subastaResultadoContainer.classList.remove("hidden");
            pujaLiderContainer.classList.add("hidden");

            const subastaResultadoLabel =
                document.getElementById("subasta-resultado-label")!;

            subastaResultadoLabel.textContent = "No hubo ninguna puja";

            renderFormState("oculto");

            break;
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
    
    if (pujas.length <= 0) {
        const p = document.createElement("p")
        p.textContent = "Aún no hay pujas para mostrar."
        p.style.alignSelf = "center"
        pujasListContainer.appendChild(p)
    }

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

    console.log(Math.floor((totalSegundos % 3600) / 60))
    if (remaining <= 0) {
        timer.dataset.timerActivo = "false"
        timer.classList.remove("danger-text")
        
        // timer.textContent = timer.dataset.estado == "ACTIVA" ? "Finalizando..." : "Comenzando..."
        return
    } 
    console.log(totalSegundos)
    if (totalSegundos <= 120) {
        timer.classList.add("danger-text")
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

export function renderSubastaNotFound() {
    const container = document.querySelector("main")!;

    container.innerHTML = `
        <section class="not-found">
            <p class="not-found-title">Subasta no encontrada</p>
            <p class="not-found-description">La subasta que estás buscando no existe o ya no está disponible.</p>
            <a href="/index.html" class="primary-text-button">Volver al catálogo</a>
        </section>
    `;
}