const API_URL = "https://localhost:7282/api"


export async function get<T>(endpoint: string, params: URLSearchParams | null = null) : Promise<T> {
    const response = await fetch(`${API_URL}${endpoint}${params ? `?${params}` : ""}`)

    if (!response.ok) {
        throw new ApiError(response.status, await response.text());
        // throw new Error(`HTTP error: ${response.status}`);
    }
    return response.json()
}

export async function post<T>(endpoint: string, object: {}) : Promise<T> {
    const response = await fetch(`${API_URL}${endpoint}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(object)
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(
            error.detail ??
            error.message ??
            error.title ??
            "Ocurrió un error"
        );
    }

    return await response.json();
}

export class ApiError extends Error {
    public status: number
    constructor(status: number, message: string) {
        super(message);
        this.status = status
    }
}