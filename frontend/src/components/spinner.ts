const htmlString = `<div class="spinner-container">
    <span class="spinner" aria-label="Cargando subastas"></span>
</div>`

function createSpinner() {
    const spinner = document.createElement("span")
    spinner.classList.add("spinner")
    spinner.setAttribute("aria-hidden", "true")

    return spinner
}

export function showLoading(container: Element) {
    const spinnerContainer = document.createElement("div")
    spinnerContainer.classList.add("spinner-container")
    
    const spinner = createSpinner()

    spinnerContainer.appendChild(spinner)

    container.replaceChildren(spinnerContainer)
}

export function showTableLoadingRow(tableBodyElement: HTMLElement, columnCount: number) {
    const row = document.createElement("tr")

    const cell = document.createElement("td")
    cell.colSpan = columnCount

    const spinnerContainer = document.createElement("div")
    spinnerContainer.classList.add("spinner-container")

    const spinner = createSpinner()
    spinnerContainer.appendChild(spinner)

    cell.appendChild(spinnerContainer)
    row.appendChild(cell)

    tableBodyElement.replaceChildren(row)
}

export function showButtonLoading(button: HTMLButtonElement, loadingMessage?: string) {
    button.disabled = true;
    button.setAttribute("aria-busy", "true");

    const spinnerContainer = document.createElement("div")
    spinnerContainer.classList.add("spinner-button")

    const spinner = createSpinner()
    spinnerContainer.appendChild(spinner)

    const textNode = loadingMessage ? document.createTextNode(loadingMessage) : ""

    button.replaceChildren(
        spinnerContainer,
        textNode
    );
}
export function showButtonReady(button: HTMLButtonElement, buttonText: string) {
    button.disabled = false;
    button.removeAttribute("aria-busy");
    button.textContent = buttonText;
}