# 🚗 Car Rental System API

A clean, scalable **Car Rental Management System** built with **ASP.NET Core** following **Onion Architecture** principles. The project is designed with separation of concerns, maintainability, and enterprise‑level best practices in mind.

---

## 📌 Overview

The **Car Rental System** provides a backend foundation for managing car rentals, users, roles, and authentication. The system is structured to support future expansion such as payments, booking workflows, availability tracking, and reporting.

It uses **Entity Framework Core**, **ASP.NET Identity**, and modern .NET practices with strong emphasis on:

* Clean Architecture
* Dependency Inversion
* Secure configuration handling
* Database migrations & seeding

---

## 🧱 Architecture – Onion Structure

The project follows the **Onion Architecture**, ensuring that:

* Core business logic is independent of frameworks
* Infrastructure concerns are isolated
* Dependencies always point inward

```
Car Rental System.sln
│
├── Domain/                     # Core domain entities & business rules
├── Domains/                    # Shared domain abstractions (if any)
├── Services.Abstractions/      # Interfaces / contracts
├── Service/                    # Business logic implementations
├── Persistance/                # EF Core DbContexts, Migrations, Configurations
├── Presentation/               # API Layer (Controllers, Middleware)
├── Shared/                     # Common utilities, helpers, constants
├── services/                   # Supporting services (if applicable)
└── Car Rental System/           # Application startup & configuration
```

### 🔹 Domain Layer

* Contains **Entities** (e.g. User, Car, Rental)
* Pure business logic
* No dependency on EF, ASP.NET, or any external library

### 🔹 Services / Application Layer

* Business use cases
* Application logic
* Depends only on **Domain** and **Abstractions**

### 🔹 Persistence (Infrastructure)

* Entity Framework Core
* SQL Server integration
* Migrations & database configurations
* Identity DbContext

### 🔹 Presentation Layer

* ASP.NET Core Web API
* Controllers
* Middleware (Custom Middleware supported)
* Request/Response handling

---

## 🛠️ Technologies Used

* **.NET 8 / ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server (Express)**
* **ASP.NET Identity**
* **BCrypt** (Password hashing)
* **Onion Architecture**
* **Dependency Injection**
* **GitHub Push Protection (Secrets Safe)**

---

## 🔐 Security Best Practices

* No secrets committed to GitHub
* `appsettings.Development.json` ignored
* Support for **Environment Variables / User Secrets**
* Passwords hashed using **BCrypt**

---

## ⚙️ Configuration

### Connection Strings

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CarRentalDb;Trusted_Connection=True;TrustServerCertificate=True",
  "IdentityConnection": "Server=YOUR_SERVER;Database=CarRentalDbIdentity;Trusted_Connection=True;TrustServerCertificate=True"
}
```

>

---

## 🧬 Database & Migrations

Apply migrations using:

```bash
Update-Database --context CarDbContext
Update-Database --context IdentityDbContext
```

For specific contexts:

```bash
Add-Migration "Migration_Name" --context CarDbContext
Add-Migration "Migration_Name" --context IdentityDbContext
```

---

## 🌱 Data Seeding

The system includes initial data seeding for default admin users:

* Admin User
* Secure password hashing
* Automatic creation if database is empty

---

## ▶️ Running the Project

```bash
dotnet build
dotnet run
```

API will be available at:

```
https://localhost:xxxx
```

---

## 🚀 Future Enhancements

* Car availability & booking workflow
* Payment gateway integration (Stripe ready)
* Role-based authorization
* Logging & monitoring
* Docker support
* CI/CD pipeline

---

## 📄 License

This project is for self educational and professional demonstration purposes.

---

## 👨‍💻 Author

**Nour Sherif Ali**
Software Engineer – Backend / .NET

---

⭐ If you find this project useful, feel free to star the repository and contribute.


<img width="1038" height="463" alt="carRental-1" src="https://github.com/user-attachments/assets/3a461843-44e9-41d5-a06c-5fcd636497ad" />

<img width="1052" height="620" alt="car Rental -2" src="https://github.com/user-attachments/assets/041b8d48-2738-4921-9090-af69b17b3f64" />

