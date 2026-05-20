# 🛒 INFO3181 – Fullstack E-Commerce Case Study

**Student:** Youssef Rajeh (1196323)  
**Course:** INFO3181 – Full Stack Web Development  
**Live Demo:** [https://fullstack-lofy.onrender.com](https://fullstack-lofy.onrender.com)

---

## 📖 About This Project

This is a fullstack web application that simulates an e-commerce store. Users can browse products by brand, add items to a shopping cart, place orders, view order history, and locate the nearest store branches on a map.

The project demonstrates how a **frontend** (what the user sees) communicates with a **backend** (the server) through **RESTful API** calls, and how data is stored and retrieved from a **relational database**.

---

## 🛠️ Technologies Used

| Layer       | Technology                        | Purpose                                      |
|-------------|-----------------------------------|----------------------------------------------|
| Frontend    | Vue.js 3 + Quasar Framework       | User interface and page routing               |
| Backend     | ASP.NET Core 8 (C#)              | RESTful API endpoints and business logic      |
| Database    | PostgreSQL (Neon)                 | Storing products, customers, orders, branches |
| ORM         | Entity Framework Core             | Mapping C# objects to database tables         |
| Auth        | JWT (JSON Web Tokens)            | Secure login and protected routes             |
| Maps        | TomTom Maps SDK                  | Branch locator with geocoding                 |
| Deployment  | Docker + Render                  | Containerized cloud hosting                   |

---

## 📁 Project Structure

```
Fullstack/
├── CaseStudy01/                  ← Frontend (Vue.js / Quasar)
│   ├── src/
│   │   ├── pages/                ← All the app pages (Home, Login, Cart, etc.)
│   │   ├── layouts/              ← MainLayout.vue (navbar + footer)
│   │   ├── router/               ← Page routing configuration
│   │   └── utils/                ← API helper functions (fetcher, poster)
│   └── package.json
│
├── Casestudy/                    ← Backend (ASP.NET Core)
│   ├── Controllers/              ← API endpoints
│   │   ├── BrandController.cs    ← GET /api/Brand
│   │   ├── ProductController.cs  ← GET /api/Product
│   │   ├── OrderController.cs    ← POST /api/Order
│   │   ├── LoginController.cs    ← POST /api/Login
│   │   ├── RegisterController.cs ← POST /api/Register
│   │   ├── BranchController.cs   ← GET /api/Branch/{lat}/{lon}
│   │   └── DataController.cs     ← GET /api/Data (load seed data)
│   ├── DAL/
│   │   ├── AppDbContext.cs       ← Database context (EF Core)
│   │   ├── DomainClasses/        ← Models (Brand, Product, Customer, Order, etc.)
│   │   ├── DAO/                  ← Data Access Objects (database queries)
│   │   └── DataUtility.cs        ← Seed data loader
│   └── Program.cs                ← App startup and configuration
│
├── Dockerfile                    ← Multi-stage Docker build
└── README.md
```

---

## 🔑 Key Features

- **User Registration & Login** – Passwords are hashed with salt. JWT tokens are used for authentication.
- **Browse Products** – View products filtered by brand with images and pricing.
- **Shopping Cart** – Add/remove items and place orders with stock validation.
- **Order History** – View past orders with itemized details.
- **Branch Locator** – Enter an address to find the 3 closest store branches on a TomTom map using the Haversine distance formula.
- **Data Utility** – Admin page to load product and branch seed data into the database.

---

## 🏃‍♂️ How to Run Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- SQL Server LocalDB (comes with Visual Studio)

### 1. Start the Backend
```bash
cd Casestudy
dotnet run
```
The API will start on `https://localhost:7017`

### 2. Start the Frontend
```bash
cd CaseStudy01
npm install
npx quasar dev
```
The app will open at `http://localhost:9000`

### 3. Load Seed Data
1. Navigate to **Data Utils** in the app menu
2. Click **Load Branches** to populate branch locations
3. Go back and click the main load button to populate brands and products

---

## ☁️ Deployment

This project is deployed to **Render** using Docker.

The `Dockerfile` uses a **multi-stage build**:
1. **Stage 1** – Builds the Quasar frontend into static files
2. **Stage 2** – Builds the .NET backend into a published release
3. **Stage 3** – Combines both into a lightweight runtime image

In production, the backend serves the frontend files directly from the `wwwroot` folder, so only **one web service** is needed.

### Environment Variables on Render
| Variable       | Description                          |
|----------------|--------------------------------------|
| `DATABASE_URL` | PostgreSQL connection string (Neon)  |

---

## 📡 API Endpoints

| Method | Endpoint                     | Auth Required | Description                    |
|--------|------------------------------|---------------|--------------------------------|
| GET    | `/api/Brand`                 | Yes           | Get all brands                 |
| GET    | `/api/Product/{brandId}`     | Yes           | Get products by brand          |
| POST   | `/api/Register`              | No            | Register a new customer        |
| POST   | `/api/Login`                 | No            | Login and receive JWT token    |
| POST   | `/api/Order/{customerId}`    | Yes           | Place a new order              |
| GET    | `/api/Order/{email}`         | Yes           | Get order history              |
| GET    | `/api/Branch/{lat}/{lon}`    | No            | Get 3 closest branches         |
| GET    | `/api/Data`                  | No            | Load product seed data         |
| GET    | `/api/Data/loadbranches`     | No            | Load branch seed data          |

---

## 📚 What I Learned

- How to build and connect a **frontend** and **backend** using REST APIs
- How to use **Entity Framework Core** as an ORM to interact with a database
- How to implement **JWT authentication** for secure API access
- How to use **Docker** to containerize and deploy a fullstack application
- How to integrate **third-party APIs** (TomTom Maps) for geolocation features
- How to manage **environment-specific configuration** (dev vs production)
