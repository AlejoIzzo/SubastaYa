# SubastaYa - Plataforma de Subastas en Tiempo Real

**Cátedra:** Proyecto de Software - 2026 - Segundo Cuatrimestre  
**Integrantes:** Izzo, Alejo - Villa, Javier  
**Repositorio:** [SubastaYa en GitHub](https://github.com/AlejoIzzo/SubastaYa)

---

##  Descripción del Proyecto

**SubastaYa** es una solución web integral (Backend RESTful, Frontend reactivo y Base de Datos relacional) diseñada para compras y ventas competitivas en línea en tiempo real. La plataforma resuelve los dos desafíos más críticos de los sistemas de subastas tradicionales:

1. **Garantía y Solvencia Económica (Escrow):** Toda puja congela automáticamente fondos reales (`SaldoRetenido`) de la billetera virtual del comprador. Si otro usuario supera la oferta, los fondos del usuario anterior se liberan inmediatamente en un bloque transaccional atómico.
2. **Juego Limpio (Anti-Sniping):** Si entra una oferta válida en los últimos 60 segundos antes del vencimiento, la subasta se extiende automáticamente por 2 minutos adicionales.
3. **Liquidación y Ciclo de Vida Automatizado (Background Worker):** Un worker en segundo plano monitorea periódicamente el vencimiento de subastas, liquidando los fondos al vendedor si hubo ganador o declarándola desierta si no hubo ofertas.
4. **Auditoría Inmutable de Eventos (Audit Log):** Registro inmutable en base de datos de extensiones de tiempo, cierres automáticos, depósitos y rechazos de ofertas (por concurrencia o reglas de negocio).
5. **Tiempo Real (SignalR WebSockets):** Salas en vivo que transmiten inmediatamente las nuevas pujas líderes y las extensiones de tiempo a todos los participantes conectados sin necesidad de recargar la página.

---

##  Arquitectura y Tecnologías

### Backend (.NET 8 - Clean Architecture)
- **SubastaYa.Domain:** Entidades (`Subasta`, `Billetera`, `Puja`, `Usuario`, `AuditoriaLog`, `TransaccionLedger`), Value Objects y tokens de concurrencia (`Version` / `[Timestamp]`).
- **SubastaYa.Application:** DTOs (`PagedResultDTO<T>`, etc.), Interfaces y Casos de Uso/Servicios de Negocio.
- **SubastaYa.Infrastructure:** Contexto EF Core (`SubastaYaContext`), Repositorios con transacciones ACID y migraciones Code-First.
- **SubastaYa.API:** Controladores RESTful bajo convención de sustantivos plurales, SignalR Hubs (`/hubs/subastas`), Background Worker (`SubastaClosingWorker`) y documentación OpenAPI con Swagger.

### Frontend
- **SPA Moderna:** TypeScript + HTML5 / CSS3 modular, empaquetado con **Vite**.
- **Cliente SignalR:** Sincronización en vivo de salas de subasta.
- **Paginación Server-Side:** Consumo eficiente de `Skip` y `Take` desde SQL Server.

### Base de Datos
- **Motor:** SQL Server / LocalDB.
- **Estrategia:** Code-First con Entity Framework Core.

---

##  Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18 o superior)](https://nodejs.org/) y npm
- [SQL Server](https://www.microsoft.com/sql-server/) o **SQL Server Express / LocalDB** (incluido habitualmente con Visual Studio)

---

##  Instalación y Puesta en Marcha

### 1. Clonar el Repositorio
```bash
git clone https://github.com/AlejoIzzo/SubastaYa.git
cd SubastaYa
```

### 2. Configurar y Levantar el Backend

1. Ingresar a la carpeta de configuración del backend:
   ```bash
   cd backend/SubastaYa.API
   ```
2. Verificar la cadena de conexión en `appsettings.json` (por defecto apunta a LocalDB):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SubastaYaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
3. Aplicar las migraciones Code-First y cargar los datos semilla (Seed Data):
   ```bash
   dotnet ef database update --project ../SubastaYa.Infrastructure --startup-project .
   ```
   *(Si no tenés instalada la herramienta CLI de EF Core globalmente, podés instalarla con: `dotnet tool install --global dotnet-ef`)*
4. Iniciar la API Backend:
   ```bash
   dotnet run --launch-profile https
   ```
   - **API escuchando en:** `https://localhost:7282`
   - **Documentación Swagger UI:** `https://localhost:7282/swagger`
   - **SignalR Hub:** `https://localhost:7282/hubs/subastas`

### 3. Levantar el Frontend

En una nueva terminal:
```bash
cd frontend
npm install
npm run dev
```
- **Aplicación Web accesible en:** `http://localhost:5173`

---

##  Datos Semilla para Pruebas (Seed Data)

El sistema precarga automáticamente las cuentas requeridas por la cátedra para evaluar todos los flujos:

| Usuario | Email | Saldo Total | Saldo Retenido | Saldo Disponible | Rol / Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Vendedor Test** | `vendedor@test.com` | $0 | $0 | $0 | Publicador de subastas |
| **Comprador 1** | `comprador1@test.com` | $150.000 | $45.000 | $105.000 | Postor líder en subasta activa |
| **Comprador 2** | `comprador2@test.com` | $200.000 | $0 | $200.000 | Postor habilitado para superar ofertas |
| **Sin Fondos** | `sinfondos@test.com` | $500 | $0 | $500 | Para probar rechazo por saldo insuficiente |

### Subastas de Prueba Precargadas:
1. **Activa estándar (Id: 1):** Cierra en ~25 minutos con líder en $45.000 (para probar pujas en vivo).
2. **Activa crítica (Id: 2):** Cierra en menos de 2 minutos (para probar cambio de color del timer visual y extensión Anti-Sniping).
3. **Próxima (Id: 3):** Inicio programado a +24 horas (pujas bloqueadas).
4. **Vencida con Ganador (Id: 4):** Para verificar liquidación final del Worker.
5. **Vencida Desierta (Id: 5):** Vencida sin ofertas para verificar pase a estado DESIERTA.

---

##  Prueba de Concurrencia Optimista (Stress Test - HTTP 409 Conflict)

### Fundamento Teórico
Para evitar condiciones de carrera (Race Conditions) como la pérdida de ofertas simultáneas o inconsistencias en los saldos en garantía, las entidades `Subasta` y `Billetera` cuentan con una propiedad de control de versión:
```csharp
[Timestamp]
public byte[] Version { get; set; }
```
Cuando dos usuarios intentan pujar sobre la misma subasta en el mismo milisegundo:
1. Ambas peticiones leen el mismo token de versión inicial.
2. La primera petición en ser procesada actualiza la subasta, incrementa la versión y hace `COMMIT` atómico (retornando `201 Created`).
3. La segunda petición intenta actualizar con una versión obsoleta. Entity Framework Core detecta la colisión (`DbUpdateConcurrencyException`), aborta la transacción y el sistema responde explícitamente con **`HTTP 409 Conflict`**, registrando el intento en la tabla de auditoría inmutable.

### Script de Prueba (Ejecución en Paralelo)

Podés ejecutar este script en **PowerShell** con la API en ejecución para disparar dos ofertas en el mismo instante y comprobar el rechazo por concurrencia:

```powershell
# Disparo simultáneo de 2 pujas sobre la Subasta 1
$body1 = '{"compradorId": 2, "monto": 55000}'
$body2 = '{"compradorId": 3, "monto": 55000}'

$task1 = [System.Threading.Tasks.Task]::Run({
    curl.exe -k -s -w "\nStatus: %{http_code}\n" -X POST "https://localhost:7282/api/subastas/1/pujas" -H "Content-Type: application/json" -d '{"compradorId": 2, "monto": 55000}'
})
$task2 = [System.Threading.Tasks.Task]::Run({
    curl.exe -k -s -w "\nStatus: %{http_code}\n" -X POST "https://localhost:7282/api/subastas/1/pujas" -H "Content-Type: application/json" -d '{"compradorId": 3, "monto": 55000}'
})

[System.Threading.Tasks.Task]::WaitAll($task1, $task2)
```

**Resultado esperado:**
- Una de las peticiones retorna: `HTTP 201 Created` (Puja registrada exitosamente).
- La otra petición colisiona y retorna: `HTTP 409 Conflict` con el mensaje:
  ```json
  {
    "status": 409,
    "error": "Conflict",
    "message": "La subasta fue modificada por otra oferta en este mismo instante. Por favor, actualice la vista e intente nuevamente."
  }
  ```

---

##  Endpoints Principales de la API REST

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/subastas` | Catálogo con paginación server-side (`pagina`, `tamanioPagina`) y filtros (`categoriaId`, `estado`, `precioMin`, etc.). |
| `GET` | `/api/subastas/{id}` | Detalle completo de una subasta y sus últimas ofertas. |
| `POST` | `/api/subastas` | Publicación de nueva subasta (vendedor). |
| `POST` | `/api/subastas/{id}/pujas` | Envío de oferta con validación de fondos, anti-sniping y concurrencia. |
| `GET` | `/api/billeteras/{id}` | Consulta de saldo (Total, Retenido, Disponible). |
| `GET` | `/api/billeteras/{id}/transacciones` | Historial de movimientos paginado (Depósitos, Retenciones, Pagos, Cobros). |
| `POST` | `/api/billeteras/{id}/depositos` | Carga de saldo simulada. |
| `GET` | `/api/usuarios/{id}/dashboard` | Panel de usuario: estadísticas, subastas y participaciones paginadas. |
| `GET` | `/api/auditorias` | Consulta de la pista de auditoría inmutable. |
