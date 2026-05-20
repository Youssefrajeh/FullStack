# Fullstack Web Application 🚀

Welcome to the Fullstack Web Application project! This is a complete, modern web application designed to demonstrate how a backend API and a frontend User Interface work together to create a seamless experience. 

This project is built using industry-standard tools and is perfect for students learning about full-stack development, database management, and cloud deployment.

## 🛠️ Tech Stack

### Frontend (User Interface)
* **Vue.js 3:** A progressive JavaScript framework for building user interfaces.
* **Quasar Framework:** A powerful Vue UI framework that provides beautiful, ready-to-use material design components.
* **Location:** The frontend code is located in the `CaseStudy01` folder.

### Backend (Server & API)
* **ASP.NET Core 8 (C#):** A robust, high-performance backend framework by Microsoft used to build our RESTful APIs.
* **Entity Framework Core:** An Object-Relational Mapper (ORM) that lets us interact with the database using C# instead of raw SQL.
* **Location:** The backend code is located in the `Casestudy` folder.

### Database
* **Development:** Microsoft SQL Server (LocalDB) - great for offline, local development.
* **Production:** PostgreSQL (Neon.tech) - a powerful, open-source cloud database used when the app is deployed to the internet.

---

## 🏃‍♂️ How to Run the Project Locally

To run this project on your own computer, you will need to run the backend and the frontend separately.

### 1. Start the Backend
The backend serves the data and handles security (like logging in).
1. Open a terminal.
2. Navigate to the backend folder:
   ```bash
   cd Casestudy
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
   *The backend will usually start on `https://localhost:7196` or a similar port.*

### 2. Start the Frontend
The frontend is the visual website that users interact with.
1. Open a **new** terminal window.
2. Navigate to the frontend folder:
   ```bash
   cd CaseStudy01
   ```
3. Install the required Node.js packages (you only need to do this once):
   ```bash
   npm install
   ```
4. Start the development server:
   ```bash
   npx quasar dev
   ```
   *The website will automatically open in your browser, usually at `http://localhost:9000`.*

---

## ☁️ How it is Deployed (Production)

When this app is ready for the real world, we deploy it to **Render** using a **Docker Container**. 

* **Docker (`Dockerfile`):** Think of Docker as a shipping container. The `Dockerfile` in the root of this project contains instructions to build the frontend, build the backend, and package them together into a single, runnable "image".
* **Environment Variables:** When deployed, the backend automatically detects a `DATABASE_URL` environment variable. If it finds one, it connects to our cloud PostgreSQL database instead of the local SQL Server!
* **Unified Serving:** In production, the ASP.NET Core backend is configured to serve the compiled frontend files directly, so we only need to host one web service.

## 📁 Project Structure Highlights
* `Casestudy/Controllers/` - Contains the API endpoints (e.g., `/api/product`) that the frontend talks to.
* `Casestudy/DAL/` - Data Access Layer. Contains the Domain Classes (Models) and DAOs (Database methods).
* `CaseStudy01/src/pages/` - The different screens/pages of the website (Home, Cart, Login).
* `CaseStudy01/src/utils/apiutils.js` - Helper file the frontend uses to automatically detect whether it should send API requests to `localhost` (in development) or `/api/` (in production).
