# SubastaYa - Plataforma de Subastas en Tiempo Real

**CÃ¡tedra:** Proyecto de Software - 2026 - Segundo Cuatrimestre  
**Integrantes:** Izzo, Alejo - Villa, Javier  
**Repositorio:** [SubastaYa en GitHub](https://github.com/AlejoIzzo/SubastaYa)

---

##  DescripciÃ³n del Proyecto

**SubastaYa** es una soluciÃ³n web integral (Backend RESTful, Frontend reactivo y Base de Datos relacional) diseÃ±ada para compras y ventas competitivas en lÃ­nea en tiempo real. La plataforma resuelve los dos desafÃ­os mÃ¡s crÃ­ticos de los sistemas de subastas tradicionales:

1. **GarantÃ­a y Solvencia EconÃ³mica (Escrow):** Toda puja congela automÃ¡ticamente fondos reales (`SaldoRetenido`) de la billetera virtual del comprador. Si otro usuario supera la oferta, los fondos del usuario anterior se liberan inmediatamente en un bloque transaccional atÃ³mico.
2. **Juego Limpio (Anti-Sniping):** Si entra una oferta vÃ¡lida en los Ãºltimos 60 segundos antes del vencimiento, la subasta se extiende automÃ¡ticamente por 2 minutos adicionales.
3. **LiquidaciÃ³n y Ciclo de Vida Automatizado (Background Worker):** Un worker en segundo plano monitorea periÃ³dicamente el vencimiento de subastas, liquidando los fondos al vendedor si hubo ganador o declarÃ¡ndola desierta si no hubo ofertas.
4. **AuditorÃ­a Inmutable de Eventos (Audit Log):** Registro inmutable en base de datos de extensiones de tiempo, cierres automÃ¡ticos, depÃ³sitos y rechazos de ofertas (por concurrencia o reglas de negocio).
5. **Tiempo Real (SignalR WebSockets):** Salas en vivo que transmiten inmediatamente las nuevas pujas lÃ­deres y las extensiones de tiempo a todos los participantes conectados sin necesidad de recargar la pÃ¡gina.

---

##  Arquitectura y TecnologÃ­as

### Backend (.NET 8 - Clean Architecture)
- **SubastaYa.Domain:** Entidades (`Subasta`, `Billetera`, `Puja`, `Usuario`, `AuditoriaLog`, `TransaccionLedger`), Value Objects y tokens de concurrencia (`Version` / `[Timestamp]`).
- **SubastaYa.Application:** DTOs (`PagedResultDTO<T>`, etc.), Interfaces y Casos de Uso/Servicios de Negocio.
- **SubastaYa.Infrastructure:** Contexto EF Core (`SubastaYaContext`), Repositorios con transacciones ACID y migraciones Code-First.
- **SubastaYa.API:** Controladores RESTful bajo convenciÃ³n de sustantivos plurales, SignalR Hubs (`/hubs/subastas`), Background Worker (`SubastaClosingWorker`) y documentaciÃ³n OpenAPI con Swagger.

### Frontend
- **SPA Moderna:** TypeScript + HTML5 / CSS3 modular, empaquetado con **Vite**.
- **Cliente SignalR:** SincronizaciÃ³n en vivo de salas de subasta.
- **PaginaciÃ³n Server-Side:** Consumo eficiente de `Skip` y `Take` desde SQL Server.

### Base de Datos
- **Motor:** SQL Server / LocalDB.
- **Estrategia:** Code-First con Entity Framework Core.

---

##  Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18 o superior)](https://nodejs.org/) y npm
- [SQL Server](https://www.microsoft.com/sql-server/) o **SQL Server Express / LocalDB** (incluido habitualmente con Visual Studio)

---

##  InstalaciÃ³n y Puesta en Marcha

### 1. Clonar el Repositorio
```bash
git clone https://github.com/AlejoIzzo/SubastaYa.git
cd SubastaYa
```

### 2. Configurar y Levantar el Backend

1. Ingresar a la carpeta de configuraciÃ³n del backend:
   ```bash
   cd backend/SubastaYa.API
   ```
2. Verificar la cadena de conexiÃ³n en `appsettings.json` (por defecto apunta a LocalDB):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SubastaYaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
3. Restaurar dependencias del proyecto:
   ```bash
   dotnet restore
   ```
4. Aplicar las migraciones Code-First y cargar los datos semilla (Seed Data):
   ```bash
   dotnet ef database update --project ../SubastaYa.Infrastructure --startup-project .
   ```
   *(Si no tenÃ©s instalada la herramienta CLI de EF Core globalmente, podÃ©s instalarla con: `dotnet tool install --global dotnet-ef`)*
5. Iniciar la API Backend:
   ```bash
   dotnet run --launch-profile https
   ```
   - **API escuchando en:** `https://localhost:7282`
   - **DocumentaciÃ³n Swagger UI:** `https://localhost:7282/swagger`
   - **SignalR Hub:** `https://localhost:7282/hubs/subastas`

### 3. Levantar el Frontend

En una nueva terminal:
```bash
cd frontend
npm install
npm run dev
```
- **AplicaciÃ³n Web accesible en:** `http://localhost:5173`

---

##  Datos Semilla para Pruebas (Seed Data)

El sistema precarga automÃ¡ticamente las cuentas requeridas por la cÃ¡tedra para evaluar todos los flujos:

| Usuario | Email | Saldo Total | Saldo Retenido | Saldo Disponible | Rol / PropÃ³sito |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Vendedor Test** | `vendedor@test.com` | $0 | $0 | $0 | Publicador de subastas |
| **Comprador 1** | `comprador1@test.com` | $150.000 | $45.000 | $105.000 | Postor lÃ­der en subasta activa |
| **Comprador 2** | `comprador2@test.com` | $200.000 | $0 | $200.000 | Postor habilitado para superar ofertas |
| **Sin Fondos** | `sinfondos@test.com` | $500 | $0 | $500 | Para probar rechazo por saldo insuficiente |

### Subastas de Prueba Precargadas:
1. **Activa estÃ¡ndar (Id: 1):** Activa con ventana corta de cierre (~25 minutos) y lÃ­der inicial en $45.000 (Comprador 1).
2. **Activa crÃ­tica (Id: 2):** Activa con cierre inminente en menos de 2 minutos (~90 segundos). Permite verificar en vivo la alerta de urgencia visual (< 60 segundos en color rojo) y la extensiÃ³n dinÃ¡mica automÃ¡tica Anti-Sniping (+2 minutos) al recibir una oferta en tiempo real sin requerir creaciÃ³n manual.
3. **PrÃ³xima (Id: 3):** Programada a futuro (+24 horas, pujas bloqueadas).
4. **Vencida con Ganador (Id: 4):** Vencida con oferta ganadora (para verificar liquidaciÃ³n del Worker).
5. **Vencida Desierta (Id: 5):** Vencida sin ofertas (para verificar pase a DESIERTA).

---

## ðŸ›¡ï¸ Prueba de Concurrencia Optimista (Stress Test - HTTP 409 Conflict)

### Fundamento TeÃ³rico
Para evitar condiciones de carrera (Race Conditions) como la pÃ©rdida de ofertas simultÃ¡neas o inconsistencias en los saldos en garantÃ­a, las entidades `Subasta` y `Billetera` cuentan con una propiedad de control de versiÃ³n:
```csharp
[Timestamp]
public byte[] Version { get; set; }
```
Cuando dos usuarios intentan pujar sobre la misma subasta en el mismo milisegundo:
1. Ambas peticiones leen el mismo token de versiÃ³n inicial.
2. La primera peticiÃ³n en ser procesada actualiza la subasta, incrementa la versiÃ³n y hace `COMMIT` atÃ³mico (retornando `201 Created`).
3. La segunda peticiÃ³n intenta actualizar con una versiÃ³n obsoleta. Entity Framework Core detecta la colisiÃ³n (`DbUpdateConcurrencyException`), aborta la transacciÃ³n y el sistema responde explÃ­citamente con **`HTTP 409 Conflict`**, registrando el intento en la tabla de auditorÃ­a inmutable.

### Script de Prueba (EjecuciÃ³n en Paralelo)

PodÃ©s ejecutar este script en **PowerShell** con la API en ejecuciÃ³n para disparar dos ofertas idÃ©nticas en el mismo instante y comprobar el rechazo por concurrencia:

```powershell
# Disparo simultÃ¡neo de 2 peticiones idÃ©nticas de puja sobre la Subasta 1
# (Comprador 2 con $200.000 de saldo disponible superando la oferta de $45.000)
$task1 = [System.Threading.Tasks.Task]::Run({
    curl.exe -k -s -w "`nStatus: %{http_code}`n" -X POST "https://localhost:7282/api/subastas/1/pujas" -H "Content-Type: application/json" -d '{\"compradorId\": 3, \"monto\": 55000}'
})
$task2 = [System.Threading.Tasks.Task]::Run({
    curl.exe -k -s -w "`nStatus: %{http_code}`n" -X POST "https://localhost:7282/api/subastas/1/pujas" -H "Content-Type: application/json" -d '{\"compradorId\": 3, \"monto\": 55000}'
})

[System.Threading.Tasks.Task]::WaitAll($task1, $task2)
```

**Resultado esperado:**
- Una de las peticiones retorna: `HTTP 201 Created` (Puja registrada exitosamente).
- La otra peticiÃ³n colisiona y retorna: `HTTP 409 Conflict` con el mensaje:
  ```json
  {
    "status": 409,
    "error": "Conflict",
    "message": "La subasta fue modificada por otra oferta en este mismo instante. Por favor, actualice la vista e intente nuevamente."
  }
  ```

---

## ðŸ“¡ CÃ³digos de Estado HTTP en `POST /api/subastas/{id}/pujas`

El endpoint de ofertas implementa cÃ³digos de estado HTTP diferenciados de acuerdo con la especificaciÃ³n y las reglas de negocio del sistema:

| CÃ³digo HTTP | Estado | Caso de Uso / Regla de Negocio |
| :--- | :--- | :--- |
| **`201 Created`** | Created | Oferta vÃ¡lida persistida atÃ³micamente y nuevo lÃ­der registrado. |
| **`400 Bad Request`** | Bad Request | Oferta invÃ¡lida: monto menor al mÃ­nimo requerido (`RUN-06`), subasta no activa o fuera de vigencia. |
| **`404 Not Found`** | Not Found | Subasta o usuario comprador inexistente. |
| **`409 Conflict`** | Conflict | ColisiÃ³n por concurrencia optimista (`DbUpdateConcurrencyException`). |
| **`422 Unprocessable Entity`** | Unprocessable Entity | PeticiÃ³n bien formada que viola la regla de solvencia: saldo disponible insuficiente en la billetera virtual (`RUN-05`). |

---

##  Endpoints Principales de la API REST

| MÃ©todo | Endpoint | DescripciÃ³n |
| :--- | :--- | :--- |
| `GET` | `/api/subastas` | CatÃ¡logo con paginaciÃ³n server-side (`pagina`, `tamanioPagina`) y filtros (`categoriaId`, `estado`, `precioMin`, etc.). |
| `GET` | `/api/subastas/{id}` | Detalle completo de una subasta y sus Ãºltimas ofertas. |
| `POST` | `/api/subastas` | PublicaciÃ³n de nueva subasta (vendedor). |
| `POST` | `/api/subastas/{id}/pujas` | EnvÃ­o de oferta con validaciÃ³n de fondos, anti-sniping y concurrencia. |
| `GET` | `/api/billeteras/{id}` | Consulta de saldo (Total, Retenido, Disponible). |
| `GET` | `/api/billeteras/{id}/transacciones` | Historial de movimientos paginado (DepÃ³sitos, Retenciones, Pagos, Cobros). |
| `POST` | `/api/billeteras/{id}/depositos` | Carga de saldo simulada. |
| `GET` | `/api/usuarios/{id}/dashboard` | Panel de usuario: estadÃ­sticas, subastas y participaciones paginadas. |
| `GET` | `/api/auditorias` | Consulta de la pista de auditorÃ­a inmutable. |
