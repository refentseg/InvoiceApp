# InvoiceApp

Invoice App which allows authenticated user to Create, Read, Update or Delete Invoices and Customers

## Table of Contents

- [Description](#description)
- [Tech Stack](#tech-stack)
- [Installation](#installation)
  - [Standard Installation](#standard-installation)
  - [Docker Installation](#docker-installation)
- [Usage](#usage)
- [Credits](#credits)

## Description

InvoiceApp is a full-stack application that provides comprehensive invoice and customer management capabilities. The application features user authentication and allows authorized users to perform complete CRUD operations on both invoices and customers. Built with modern technologies, it offers both web and cross-platform mobile access through Angular and MAUI clients.

## Tech Stack

**Frontend:** Angular  
**Backend:** ASP.NET Web API  
**ORM:** Entity Framework  
**Database:** SQLite  
**Cross-Platform:** MAUI

## Planning (Wireframe)

<img width="5908" alt="InvoiceApp(2)" src="https://github.com/user-attachments/assets/87ede31d-034e-4f27-a9e4-317032747433">

## Project Structure

The application consists of two main client applications:
- **Client**: Angular web application
- **MAUIClient**: MAUI cross-platform mobile application

Both clients connect to the same ASP.NET Web API backend for consistent data management across platforms.

## Installation

### Standard Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/refentseg/InvoiceApp.git
   cd InvoiceApp
   ```

2. **Backend Setup (ASP.NET Web API)**
   ```bash
   cd API
   dotnet restore
   dotnet build
   dotnet run
   ```
   The API will be available at `http://localhost:5000`

3. **Frontend Setup (Angular Client)**
   ```bash
   cd client
   npm install
   ng serve
   ```
   **Note:** Make sure the backend API is running before starting the Angular client.

4. **MAUI Client Setup**
   ```bash
   cd MAUIClient
   dotnet restore
   dotnet build
   dotnet run
   ```
   **Note:** Ensure the backend API is running before 
   launching the MAUI client.

5. **Database Setup**
   - The SQLite database will be automatically created when you first run the API
   - Entity Framework migrations will handle the initial schema setup

### Docker Installation

```bash
# Clone the repository
git clone https://github.com/refentseg/InvoiceApp.git
cd InvoiceApp

# Build and run with Docker Compose
docker-compose up --build
```

The application will be available at:
- Web API: `http://localhost:5000`
- Angular Client: `http://localhost:4200`
- **Alternative**: You can also access the web application directly at `http://localhost:5000` when the API is running

## Usage

### Web Version

The Angular web client provides a full-featured interface for managing invoices and customers. Here's a demo of the application in action:

![Usage](/assets/usage.gif)

**Key Features:**
- **Authentication**: Secure user login and registration
- **Dashboard**: Overview of recent invoices and customer statistics
- **Invoice Management**: Create, view, edit, and delete invoices
- **Customer Management**: Maintain customer database with full CRUD operations
- **Responsive Design**: Works seamlessly across desktop and mobile browsers

**Getting Started:**
1. Navigate to `http://localhost:4200` (Angular dev server) or `http://localhost:5000` (direct API hosting) after installation
2. Register a new account or login with existing credentials
3. Start creating customers and invoices through the intuitive interface

### Mobile Version (MAUI)

The MAUI client provides cross-platform mobile access with native performance on iOS, Android, and Windows platforms.

## Credits

**Developer:** [Refentse Gaonnwe](https://github.com/refentseg)

Special thanks to the open-source community and the technologies that made this project possible:
- Angular Team
- Microsoft .NET Team
- Entity Framework Team