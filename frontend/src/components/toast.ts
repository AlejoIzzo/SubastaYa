type ToastType = "success" | "error" | "info";

export function showToast(message: string, type: ToastType = "info") {
    const toast = document.createElement("div");
    toast.className = `toast toast-${type}`;

    const typeIconMap = {
        success: "checkmark-circle-outline",
        error: "warning-outline",
        info: "alert-circle-outline"
    }
    const icon = document.createElement("ion-icon");
    icon.setAttribute("name", typeIconMap[type]);

    const messageElement = document.createElement("span");
    messageElement.textContent = message;

    toast.appendChild(icon);
    toast.appendChild(messageElement);

    const container = getToastContainer();
    container.appendChild(toast);

    requestAnimationFrame(() => {
        toast.classList.add("show");
    });

    setTimeout(() => {
        toast.classList.remove("show");

        toast.addEventListener("transitionend", () => {
            toast.remove();
        }, { once: true });
    }, 6000);
}

function getToastContainer(): HTMLElement {
    let container = document.getElementById("toast-container");

    if (!container) {
        container = document.createElement("div");
        container.id = "toast-container";
        document.body.appendChild(container);
    }

    return container;
}