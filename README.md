# DeviceManagePro

DeviceManagePro is a .NET 8 web API application designed to manage devices, clients, and device types. It features role-based authentication and authorization using JWT, robust logging through middleware, and a well-organized project structure.

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Database Seeding](#database-seeding)
  - [Running the Application](#running-the-application)
- [Endpoints](#endpoints)
- [Contributing](#contributing)
- [License](#license)

## Features
- Device Management: Add, update, delete, and retrieve devices.
- Client Management: Manage client data.
- Device Types: Handle different types of devices.
- Role-Based Authentication: Uses JWT for secure authentication and authorization.
- Action Logging: Middleware logs user actions for audit purposes.

## Technologies Used
- .NET 8
- Entity Framework Core
- Microsoft Identity
- AutoMapper
- JWT (JSON Web Tokens)
- SQL Server
- Serilog

## Project Structure
```
/ Controllers
    - DeviceController.cs
    - ClientController.cs
    - AccountController.cs
/ Core
    / IConfiguration
        - IUnitOfWork.cs
    / IRepositories
        - IClientRepository.cs
        - IDeviceRepository.cs
        - IGenericRepository.cs
    / Repositories
        - ClientRepository.cs
        - DeviceRepository.cs
        - GenericRepository.cs
/ Data
    - AppDbContext.cs
    - SeedData.cs
    - UnitOfWork.cs
/ Middlewares
    - ActionLoggingMiddleware.cs
/ Models
    - Device.cs
    - Client.cs
    - DeviceType.cs
    - Login.cs
    - Register.cs
    - UserRole.cs
/ Dtos
    / Requests
        - CreateClientRequest.cs
        - CreateDeviceRequest.cs
        - UpdateClientRequest.cs
        - UpdateDeviceRequest.cs
    / Responses
        - GetClientResponse.cs
        - GetDeviceResponse.cs
/ Profiles
    - ClientProfile.cs
    - DeviceProfile.cs
appsettings.json
Program.cs
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server

### Installation
1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/DeviceManagePro.git
    cd DeviceManagePro
    ```

2. Set up the database connection string in `appsettings.json`:
    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=your_server;Database=DevicesController;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
    }
    ```

3. Restore the NuGet packages:
    ```sh
    dotnet restore
    ```

### Database Seeding
To seed the database with initial data, run the following command:
```sh
dotnet ef database update
dotnet run --project .\Data\SeedData.cs
```

### Running the Application
To run the application, use the following command:
```sh
dotnet run
```
The application will be available at `https://localhost7001`.

## Endpoints

### Authentication
- `POST /api/Account/Login`: Authenticate user and return JWT.
- `POST /api/Account/Register`: Register a new user.

### Devices
- `GET /api/Device/GetAll`: Get all devices.
- `GET /api/Device/GetAllByClientId/{ClientId}`: Get devices by client ID.
- `GET /api/Device/GetById/{id}`: Get a device by ID.
- `GET /api/Device/GetStatus/{id}`: Get the status of a device.
- `POST /api/Device/Add`: Add a new device.
- `POST /api/Device/AddDeviceType`: Add a new device type.
- `PUT /api/Device/Upsert`: Update or insert a device.
- `DELETE /api/Device/Delete/{id}`: Delete a device.
- `POST /api/Device/Assign/{DeviceId}`: Assign a device to a client.
- `POST /api/Device/Unassign/{DeviceId}`: Unassign a device from a client.
- `GET /api/Device/ToggleStatus/{id}`: Toggle the status of a device.

### Clients
- `GET /api/Client/GetAll`: Get all clients.
- `GET /api/Client/GetById/{id}`: Get a client by ID.
- `POST /api/Client/Add`: Add a new client.
- `PUT /api/Client/Update`: Update a client.
- `DELETE /api/Client/Delete/{id}`: Delete a client.

## Contributing
Contributions are welcome! Please open an issue or submit a pull request for any changes.

## License
This project is licensed under the MIT License.
