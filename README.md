# 🏠 Sakenny (سكنّي) – Backend  

<img width="1920" height="767" alt="screencapture-localhost-4200-rent-2025-08-19-20_37_29" src="https://github.com/user-attachments/assets/c3b8283e-584e-40b9-9c03-5b3f5d4db873" />

<h2 align="center">
  <b>A Full-Stack Property Rental Platform built with .NET & Angular</b><br>
  Graduation Project – ITI Alexandria | Full Stack Development Track
</h2>  

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet" width="115"/>
  <img src="https://img.shields.io/badge/Entity%20Framework-Core-blue?logo=ef" width="200"/>
  <img src="https://img.shields.io/badge/SQL%20Server-DB-red?logo=microsoftsqlserver" width="143"/>
  <img src="https://img.shields.io/badge/Stripe-Payments-blueviolet?logo=stripe" width="180"/>
  <img src="https://img.shields.io/badge/Google%20Maps-Integration-green?logo=googlemaps" width="240"/>
  <img src="https://img.shields.io/badge/Azure%20Blob-Storage-0078D4?logo=microsoftazure&logoColor=white" width="200"/>
</p>



---

## 📖 Overview  

**Sakenny (سكنّي)**, meaning *"House me"* in Arabic, is a property rental platform inspired by **Airbnb**.  
It provides a **dynamic, secure, and responsive experience** for renting properties, managing listings, and handling payments.  

The platform supports **three types of accounts**:  
- 👤 **Customer** – Browse and rent properties, manage booking history, make secure payments via Stripe, and view locations on Google Maps.  
- 🏡 **Host** – Post properties for rent (pending admin approval), manage listings, track income, and access a personalized dashboard.  
- 🛡️ **Admin** – Review and approve property requests, manage users & data, and access an advanced analytics dashboard.  

### 🔑 Key Highlights  
- 🔐 Authentication & Authorization with JWT + Google Login  
- 💳 Secure payment processing via **Stripe**  
- 📍 Location services powered by **Google Maps API**  
- 📊 Role-based dashboards (Customer, Host, Admin)  
- 📅 Real-time booking validation (dates locked both frontend & backend)
- 🖼️ Image Storage using **Azure Blob Storage** (AzStorage via Azurite) – upload, manage, and retrieve property images with secure URLs.

This repository contains the **Backend** of Sakenny, built with **.NET (N-Tier Architecture)** following **Clean Code principles** and the **Dependency Inversion Principle**.

---  

## 🎥 Demo  

Check out the demo of **Sakenny** here:  
👉 [Watch on Google Drive](YOUR_GOOGLE_DRIVE_LINK_HERE)  

---

## ✨ Features  

The **Sakenny Backend** provides all the core functionalities required to run a property rental platform securely and efficiently.  

### 🔑 Authentication & Authorization  
- JWT-based authentication for secure API access.  
- Role-based authorization (**Customer**, **Host**, **Admin**).  
- Login & Signup with **Google OAuth**.  

### 💳 Payment Integration  
- Stripe integration for safe and reliable payments.  
- Transaction tracking in user booking history.  

### 🏡 Property Management  
- Hosts can submit properties for rental.  
- Properties require **Admin approval** before being visible to customers.  
- Property availability is automatically managed to prevent double-booking.  

### 📍 Location Services  
- **Google Maps API** integration:  
  - Hosts can select property location when submitting a listing.  
  - Customers can view property locations on a map.  
  - Search results support **nearby property suggestions**.
 
### 🖼️ Image Management
- Images uploaded by hosts are stored in Azure Blob Storage (Azurite for local development).
- Each image generates a secure URL stored in the database for efficient retrieval.

### 📅 Booking System  
- Customers can rent properties for specific dates.  
- Booked dates become **unavailable** both in the frontend (date picker disabled) and backend (server validation).  
- Rental history is stored per customer.  

### 📊 Dashboards  
- **Customer Dashboard** – Manage bookings & history.  
- **Host Dashboard** – Manage properties, monitor income.  
- **Admin Dashboard** – Manage properties, users, approvals, and overall system data.  

### 🛠️ Clean Architecture & Best Practices  
- **N-Tier Architecture**: Data Access Layer, Application Layer, API Layer.  
- **Dependency Inversion Principle** & **Clean Code** standards.  
- Generic repositories with **Unit of Work pattern** for reusable, maintainable code.  

---

## 🛠️ Tech Stack  

The **Sakenny Backend** is built with modern technologies to ensure scalability, security, and maintainability.  

<p align="center">
  
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](#)
[![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework-Core-00599C?style=for-the-badge&logo=nuget&logoColor=white)](#)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](#)
[![Stripe](https://img.shields.io/badge/Stripe-Payments-626CD9?style=for-the-badge&logo=stripe&logoColor=white)](#)
[![Google Maps API](https://img.shields.io/badge/Google%20Maps-API-4285F4?style=for-the-badge&logo=googlemaps&logoColor=white)](#)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-Mapping-FF6F00?style=for-the-badge&logo=automapper&logoColor=white)](#)
[![Azure Blob Storage](https://img.shields.io/badge/Azure-Blob%20Storage-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)](#)


</p>  

### 📦 Backend Technologies  
- **.NET 8.0** – API development with ASP.NET Core.  
- **Entity Framework Core** – ORM for database operations.  
- **SQL Server** – Relational database management system.  
- **AutoMapper** – Object-to-object mapping for DTOs and models.  
- **Stripe API** – Secure online payments.  
- **Google Maps API** – Location & geospatial services.  

### 🏗️ Architecture & Patterns  
- **N-Tier Architecture** (Data Access, Application, API Layers).  
- **Repository & Unit of Work Pattern**.  
- **Dependency Injection** for better scalability and testing.  
- **DTOs & Services** for clear separation of concerns.  
---

## 🏗️ Architecture  

The **Sakenny Backend** follows an **N-Tier Architecture** to separate concerns and improve maintainability.  
This structure ensures that each layer has a **single responsibility**, making the project scalable and easier to test.  

### 🔹 Layers Overview  
- **Data Access Layer (DAL)**  
  - Contains **Models**, **Configurations**, **Repositories**, and the **Unit of Work**.  
  - Uses **Entity Framework Core** to interact with the database.  
  - Implements **generic repositories** to avoid repetitive code.  

- **Application Layer**  
  - Contains **DTOs**, **Services**, and their **Interfaces**.  
  - Responsible for **business logic** and orchestrating data between DAL and API.  

- **API Layer**  
  - Exposes RESTful **Controllers** for communication with the frontend.  
  - Contains the **Mapping Profile** (AutoMapper) to map between Models and DTOs.  
  - Handles **Authentication & Authorization** using JWT and Google OAuth.  

---

### 📂 Folder Structure  

```plaintext
Sakenny.Backend/
├── Sakenny.DataAccess/
│   ├── Models/
│   ├── Configurations/
│   ├── Repositories/
│   ├── Interfaces/
│   ├── UnitOfWork.cs
│   └── AppDbContext.cs
│
├── Sakenny.Application/
│   ├── DTOs/
│   ├── Services/
│   └── Interfaces/
│
├── Sakenny.API/
│   ├── Controllers/
│   └── MappingProfiles/
│
└── Sakenny.sln
```
---

## 📐 Design Principles  

- ✅ **Dependency Inversion Principle (DIP)** – High-level modules depend on abstractions, not concrete implementations.  
- ✅ **Separation of Concerns** – Each layer is responsible for a specific aspect of the system.  
- ✅ **Clean Code Practices** – Generic repositories, DTO mappings, and proper layering.  

---
## 🎨 Frontend Repository  

The **Sakenny Frontend** is built with **Angular**, styled mainly with **Tailwind CSS**, and enhanced with **PrimeNG** components.  
It provides a responsive, user-friendly interface that connects seamlessly with this backend API.  

👉 Check it out here: [Sakenny Frontend Repository](https://github.com/CozyQuest/front-end-.git)

---

## 🖼️ Screenshots  

### Database Schema (SQL Server)  

![Screenshot 2025-08-19 214024](https://github.com/user-attachments/assets/3d7441c2-d79a-4dba-8206-64ac17ac7b65)

### Swagger UI  

<img alt="screencapture-jdg827ff-7279-uks1-devtunnels-ms-swagger-index-html-2025-08-19-21_41_46" src="https://github.com/user-attachments/assets/42914956-d173-4c16-a2b2-1a1673d97b0b" />

---

## 📜 License  

This project is licensed under the **MIT License**.  
You are free to use, modify, and distribute this software, provided that proper attribution is given.  

[![License: MIT](https://img.shields.io/badge/License-MIT-red.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

---

## 👨‍💻 Team  

Meet the people behind this project:  

| Avatar | Name | GitHub |
|--------|------|--------|
| <img src="https://avatars.githubusercontent.com/u/39447236?v=4" width="50" height="50" style="border-radius:50%"/> | **Mohab Wafaie** | [@MohabWafaie](https://github.com/MohabWafaie) |
| <img src="https://avatars.githubusercontent.com/u/75574826?v=4" width="50" height="50" style="border-radius:50%"/> | **Ahmed Waleed** | [@Ahmedabdeen15](https://github.com/Ahmedabdeen15) |
| <img src="https://avatars.githubusercontent.com/u/103130605?v=4" width="50" height="50" style="border-radius:50%"/> | **Nancy EL Sherbiny** | [@NancELSherbiny](https://github.com/NancELSherbiny) |
| <img src="https://avatars.githubusercontent.com/u/105066899?v=4" width="50" height="50" style="border-radius:50%"/> | **Marwan Fawzy Shahat Mahmoud** | [@ArabianHindi](https://github.com/ArabianHindi) |
| <img src="https://avatars.githubusercontent.com/u/152093660?v=4" width="50" height="50" style="border-radius:50%"/> | **Rodina Elfeky** | [@RodinaElfeky](https://github.com/RodinaElfeky) |
| <img src="https://avatars.githubusercontent.com/u/95757948?v=4" width="50" height="50" style="border-radius:50%"/> | **Ahmed Aseel** | [@Ahmed-Aseel](https://github.com/Ahmed-Aseel) |

---

## ❤️ Made With Love  

This project was made with ❤️, ☕, and a passion for clean architecture.  

