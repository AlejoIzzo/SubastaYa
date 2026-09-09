const API_URL = "https://localhost:7282/api"



export async function get<T>(endpoint: string, params: URLSearchParams | null = null) : Promise<T> {
    const response = await fetch(`${API_URL}${endpoint}${params ? `?${params}` : ""}`)

    if (!response.ok) {
        throw new Error(`HTTP error: ${response.status}`);
    }
    return response.json()
}