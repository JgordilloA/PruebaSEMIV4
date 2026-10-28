# Laboratorio App

API ASP.NET Core + Angular + SQL Server Express para registrar y listar órdenes de laboratorio.

laboratorio-app/
├── backend/    API (.NET 9) 
├── frontend/   Angular 19
├── database/   Scripts SQL


## Requisitos
.NET SDK 9, Node.js + Angular CLI, SQL Server Express (`localhost\SQLEXPRESS`, Windows Authentication).

## Base de datos
Crea `labSamples2` y ejecuta `database/schema.sql` 
La cadena de conexión está en `backend/samplesManagment.Api/appsettings.json`.

## Backend (http://localhost:5228)

cd backend/samplesManagment.Api
dotnet run


Estructura: `Controllers` (HTTP) → `Services` (lógica)  → `Repositories` (SQL con Dapper) · `Middleware` (registro de solicitudes en consola y manejo de errores) · `Models`/`Exceptions`.

La creación de una muestra se ejecuta en una transacción.

## Frontend (http://localhost:4200)
```bash
cd frontend
npm install
npm start
```
La URL de la API está en `frontend/src/environments/environment.ts`. 