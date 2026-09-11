import { getSubasta } from "../api/subastaApi";
import { postPuja, getPujas } from "../api/pujaApi";
import { renderHeader } from "../components/header";
import { createPujaCardElement } from "../components/puja-card";
import type { CrearPujaDTO } from "../models/pujaTypes";

const params = new URLSearchParams(window.location.search);
const subastaId = params.get("id");
const subasta = await getSubasta(Number(subastaId))

async function renderSubasta()  {
    const titulo = document.getElementById("titulo")!
    const vendedorNombre = document.getElementById("vendedor-nombre")!
    const descripcion = document.getElementById("descripcion")!
    const img = document.getElementById("subasta-img")!
    const timer = document.getElementById("timer")!
    const pujaLiderMonto = document.getElementById("puja-lider-monto")!
    const incrementoMinimo = document.getElementById("incremento-minimo")!

    titulo.textContent = subasta.titulo
    vendedorNombre.textContent = subasta.vendedorNombre
    descripcion.textContent = subasta.descripcion
    img.setAttribute("src", subasta.urlImagen)
    incrementoMinimo.textContent = String(subasta.incrementoMinimo)
    pujaLiderMonto.textContent = `$${subasta.pujaActual}`
    
    renderPujaList()

    sugerirPuja(subasta.pujaActual, subasta.incrementoMinimo)
}

async function renderPujaList() {
    const pujas = await getPujas(Number(subastaId))

    const pujasListContainer = document.getElementById("puja-list")!
    pujasListContainer.replaceChildren()
    
    for (let puja of pujas) {
        pujasListContainer.appendChild(createPujaCardElement(puja))
    }
}

function sugerirPuja(pujaLider: number, incrementoMinimo: number) {
    const pujaInput = document.getElementById("puja-input") as HTMLInputElement
    pujaInput.value = String(pujaLider + incrementoMinimo)
}

const headerContainer = document.getElementById("header")!
renderHeader(headerContainer)
renderSubasta()

const pujaForm = document.getElementById("puja-form") as HTMLFormElement
pujaForm.addEventListener("submit", async (event) => {
    event.preventDefault()

    const formData = new FormData(pujaForm)
    
    const crearPujaDTO: CrearPujaDTO = {
        compradorId: 2, // hardcoded for the moment
        monto: Number(formData.get("monto"))
    }
    try {
        const resultado = await postPuja(Number(subastaId), crearPujaDTO)
        
        renderPujaList()
    } catch (err: any) {
        alert(`Ocurrió un error al registrar puja: ${err}`)
        console.error(err)
    }
})