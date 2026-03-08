# 🚗 MotoMarket - Cross-Platform Automotive Classifieds System

## 📖 About The Project
MotoMarket is a comprehensive **Integrated Information System (IIS)** tailored for the automotive industry. It allows users to browse, manage, and publish vehicle listings, as well as communicate in real-time. 

The system was designed with a focus on high scalability, security, and maintainability, utilizing modern architectural patterns like **Clean Architecture** and **CQRS**. It consists of a centralized API that serves two distinct clients: a server-side rendered Web Application and a cross-platform Mobile Application.



## ✨ Key Features
* **Cross-Platform Access:** Fully functional Web application (MVC) and Mobile application (MAUI).
* **Advanced Search & Filtering:** Dynamic database queries using `IQueryable` and data projections for optimized performance.
* **Real-Time Communication:** Built-in chat system using **SignalR** (WebSockets) allowing users to message each other seamlessly across web and mobile platforms.
* **Secure Authorization:** Hybrid authentication supporting both Cookies (for Web) and JWT Tokens (for Mobile).
* **RBAC Admin Panel:** Dedicated back-office for managing dictionaries (brands, models), completely decoupled from direct database access using DTOs.
* **Media Handling:** Native camera integration in the mobile app and secure file upload to the server.
* **Security First:** Implemented defenses against IDOR (Insecure Direct Object Reference) to ensure users can only modify their own resources.

## 🛠 Built With (Tech Stack)
* **Backend:** .NET 8, C#, ASP.NET Core Web API
* **Web Client:** ASP.NET Core MVC, Bootstrap 5, HTML/CSS/JS
* **Mobile Client:** .NET MAUI (focus on Android)
* **Architecture:** Clean Architecture, CQRS (MediatR), Repository Pattern
* **Database:** SQL Server, Entity Framework Core (Code-First)
* **Real-time:** ASP.NET Core SignalR
* **Security:** ASP.NET Core Identity, JWT

---

## 🚀 Getting Started / How to Run Locally

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server (e.g., SQL Server Express or LocalDB)
* Visual Studio 2022 (with ASP.NET and web development, and .NET MAUI workloads installed)
* Android Emulator (if running the mobile project)

### Installation & Setup

1. **Clone the repository**
   ```sh
   git clone [https://github.com/YOUR_USERNAME/MotoMarket.git](https://github.com/YOUR_USERNAME/MotoMarket.git)
   ```

2. **Database Configuration**
   The project uses EF Core Code-First approach. You do not need to run SQL scripts manually. The database will be created and updated automatically upon the first run via `context.Database.Migrate()`.
   * Open the solution in Visual Studio.
   * Verify the Connection String in `MotoMarket.Api/appsettings.json`. Update the Server name if necessary (e.g., to `(localdb)\mssqllocaldb` or `.\SQLEXPRESS`).

3. **Data Initialization (Seeder)**
   To make testing easier, the system includes a Data Seeder that automatically populates the database with initial dictionaries (Brands, Models) and an Admin account.
   * **Admin Login:** `admin@motomarket.pl`
   * **Admin Password:** `Admin123!`

4. **Running the Application (Crucial Step)**
   Since this is a distributed system, the API must be running for the clients to work. You need to configure Visual Studio to run multiple projects simultaneously:
   * Right-click on the **Solution** in Solution Explorer -> Select **Properties**.
   * Go to **Multiple startup projects**.
   * Set the action to **Start** for the following projects:
     1. `MotoMarket.Api` (Move it to the top so it starts first)    
     2. `MotoMarket.Web`
     3. `MotoMarket.Mobile` *(Optional - requires Android Emulator configured)*
   * Click **Apply** and run the solution (F5).

### 📱 Running the Mobile App (MAUI)
The mobile application was primarily configured and tested for the **Android Emulator**. Ensure your emulator is running and the API is accessible from the Android virtual device (this may require configuring your API to listen on a specific IP address rather than just `localhost`, or using `10.0.2.2` in the MAUI app's HTTP client configuration).

---

## 👨‍💻 Author
**Twoje Imię i Nazwisko** * GitHub: [@ernest-ostap](https://github.com/ernest-ostap)
* LinkedIn: [Ernest Ostap](https://www.linkedin.com/in/ernest-ostap)
