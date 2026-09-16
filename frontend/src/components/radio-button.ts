import htmlTemplate from "./radio-button.html?raw" 
const template = new DOMParser().parseFromString(htmlTemplate, "text/html").getElementById("radio-button-template") as HTMLTemplateElement

type Args = {
    inputFormName: string, 
    inputFormValue: string | number, 
    label?: string, 
    icon?: string,
    checked?: boolean
}

export function createRadioButton({inputFormName, inputFormValue, label, icon, checked}: Args) {
    const clone = template.content.cloneNode(true) as DocumentFragment
    
    // const buttonEl = clone.querySelector(".radio-button")! as HTMLElement
    const radioInputEl = clone.querySelector(".radio-input")! as HTMLInputElement
    const iconEl = clone.querySelector(".radio-button-icon")! as HTMLElement
    const labelEl = clone.querySelector(".radio-button-text")!

    radioInputEl.setAttribute('name', inputFormName)
    radioInputEl.setAttribute('value', String(inputFormValue))

    if (!icon)
        iconEl.style.display = "none"
    else
        iconEl.setAttribute("name", icon)

    if (label)
        labelEl.textContent = label

    if (checked)
        radioInputEl.checked = checked

    return clone
}
