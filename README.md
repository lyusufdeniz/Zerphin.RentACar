# 🚗 Zerphin Rent A Car

A modern car rental management system. A full-stack application built with Clean Architecture principles, featuring .NET 9.0 backend and Angular 20 frontend.

## 📸 Screenshots

![Screenshot 1](https://i.hizliresim.com/7r346f1.png)

![Screenshot 2](https://i.hizliresim.com/b6dsa5o.png)

![Screenshot 3](https://i.hizliresim.com/4i2rm6e.png)

![Screenshot 4](https://i.hizliresim.com/92gdiwi.png)

![Screenshot 5](https://i.hizliresim.com/f0a2zfa.png)

## 📋 Table of Contents

- [Screenshots](#-screenshots)
- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Installation](#-installation)
- [Usage](#-usage)
- [API Documentation](#-api-documentation)
- [Contributing](#-contributing)

## ✨ Features

### 🔐 Authentication and Authorization
- JWT-based authentication
- Refresh token support
- Secure password management

### 🚙 Vehicle Management
- Add, update, and delete vehicles
- Vehicle search and filtering
- Vehicle status tracking (Available, Rented, Maintenance, etc.)
- Vehicle categories
- Vehicle image management (Cloudinary integration)
- Vehicle statistics

### 📅 Rental Management
- Create and manage rentals
- Rental status tracking
- Late fee and damage fee calculation
- Mileage tracking
- Pickup/return locations

### 👥 User Management
- User registration and profile management
- User roles and permissions
- User search and filtering

### 🛡️ Insurance Management
- Add and manage insurance
- Vehicle-insurance association

### 📊 Reporting and Statistics
- Vehicle statistics
- Rental reports
- Dashboard views

## 🛠️ Technology Stack

### Backend
- **.NET 9.0** - Framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Validation
- **JWT** - Authentication
- **Cloudinary** - Image management
- **Swagger/OpenAPI** - API documentation

### Frontend
- **Angular 20** - Framework
- **Nx** - Monorepo management
- **Angular Material** - UI components
- **Chart.js** - Charts and statistics
- **RxJS** - Reactive programming
- **TypeScript** - Programming language

### Architecture
- **Clean Architecture** - Layered architecture
- **CQRS** - Command Query Responsibility Segregation
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management

## 📁 Project Structure

```
Zerphin.RentACar/
├── API/                          # Backend project
│   ├── Zerphin.RentACar.API/    # Web API layer
│   │   ├── Controllers/         # API endpoints
│   │   ├── Middleware/          # Custom middlewares
│   │   ├── Filters/             # Action filters
│   │   └── Extensions/          # Extension methods
│   │
│   ├── Zerphin.RentACar.Application/  # Application layer
│   │   └── Features/            # CQRS commands/queries
│   │       ├── Authentication/
│   │       ├── Vehicles/
│   │       ├── Rentals/
│   │       ├── Users/
│   │       └── Insurances/
│   │
│   ├── Zerphin.RentACar.Domain/       # Domain layer
│   │   ├── Entities/            # Domain entities
│   │   ├── ValueObjects/        # Value objects
│   │   ├── Contracts/           # Interfaces
│   │   └── Exceptions/          # Custom exceptions
│   │
│   └── Zerphin.RentACar.Infrastructure/  # Infrastructure layer
│       ├── Data/                # DbContext and configurations
│       ├── Repositories/        # Repository implementations
│       └── Services/            # Service implementations
│
└── Client/                      # Frontend project
    └── apps/
        └── backoffice/          # Angular application
```

## 🚀 Installation

### Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) or SQL Server LocalDB
- [Git](https://git-scm.com/)

### Backend Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/Zerphin.RentACar.git
cd Zerphin.RentACar
```

2. Navigate to the API folder:
```bash
cd API
```

3. Configure the database connection string:
   - Open `Zerphin.RentACar.API/appsettings.Development.json`
   - Update the `ConnectionStrings:DefaultConnection` value according to your SQL Server connection

4. Run database migrations:
```bash
cd Zerphin.RentACar.API
dotnet ef database update --project ../Zerphin.RentACar.Infrastructure
```

5. Run the application:
```bash
dotnet run
```

The API will run on `https://localhost:5001` by default.

### Frontend Installation

1. Navigate to the Client folder:
```bash
cd Client
```

2. Install dependencies:
```bash
npm install
```

3. Run the application:
```bash
npx nx serve backoffice
```

The frontend will run on `http://localhost:4200` by default.

### Configuration

#### Backend Configuration

Configure the following settings in the `appsettings.Development.json` file:

- **JWT**: JWT token settings
- **Cloudinary**: Cloudinary account information for image uploads
- **Cors**: Frontend URLs

#### Frontend Configuration

Update API endpoints in the frontend configuration file (usually in `environment.ts`).

## 📖 Usage

### API Endpoints

#### Authentication
- `POST /api/authentication/login` - User login
- `POST /api/authentication/register` - User registration
- `POST /api/authentication/refresh-token` - Token refresh

#### Vehicles
- `GET /api/vehicles/search` - Search vehicles
- `GET /api/vehicles/id/` - Get vehicle details
- `POST /api/vehicles` - Create vehicle (Admin/Manager)
- `PUT /api/vehicles` - Update vehicle (Admin/Manager)
- `DELETE /api/vehicles` - Delete vehicle (Admin/Manager)
- `GET /api/vehicles/statistics` - Vehicle statistics

#### Rentals
- `GET /api/rentals/search` - Search rentals
- `POST /api/rentals` - Create rental
- `PUT /api/rentals` - Update rental
- `DELETE /api/rentals` - Delete rental

#### Users
- `GET /api/users/search` - Search users
- `GET /api/users/id/` - Get user details
- `PUT /api/users` - Update user
- `DELETE /api/users` - Delete user

#### Insurances
- `GET /api/insurances/search` - Search insurances
- `POST /api/insurances` - Create insurance
- `PUT /api/insurances` - Update insurance
- `DELETE /api/insurances` - Delete insurance

