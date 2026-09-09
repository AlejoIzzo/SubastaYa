import htmlTemplate from "./header.html?raw"

export function renderHeader(parentElement: HTMLElement) {
    parentElement.innerHTML = htmlTemplate
}