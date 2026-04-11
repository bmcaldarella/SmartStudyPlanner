# 📚 Smart Study Planner

## 🌐 Live Application

You can access the deployed application here:
👉 **[Smart Study Planner Live Site](https://smartstudyplanner-glk5.onrender.com/)**

---

## 🎥 Demo Video

Watch a walkthrough of the application:
👉 **[Project Demo Video]()**

---

## 📌 Overview

Smart Study Planner is a web application built with **.NET Blazor** that helps students organize their academic work. Users can create subjects, manage study tasks, set deadlines, and track their progress in a simple and structured way.

This project was developed as part of a group assignment to demonstrate full-stack web development using Blazor, authentication, and database integration.

---

## 👥 Team

- Brandon Michel Caldarella
- Camila Sol Moya Casanova
- Collins Onyebuchi Attah
- Efehi Nehikhare
- Kinyera Alvine
- Letlotlo Stanford Hlaoli

---

## 🚀 Getting Started

### 🔧 Requirements

* .NET 8 or later
* SQL Server (or LocalDB)
* Visual Studio / VS Code

---

### ▶️ How to Run the Project

1. Clone or download the repository
2. Open the project in Visual Studio
3. Configure the database connection in **appsettings.json**
4. Apply database migrations:

   ```
   Update-Database
   ```
5. Run the application:

   ```
   dotnet run
   ```
6. Open your browser and go to:

   ```
   https://localhost:xxxx
   ```

---

## 👤 User Guide

### 🔐 1. Register & Login

* Create a new account using the **Register** page
* Log in using your email and password

---

### 🏠 2. Home Page

* If logged in → Go directly to your **Dashboard**
* If not logged in → Register or Login

---

### 📊 3. Dashboard

The dashboard provides an overview of:

* Total tasks
* Pending tasks
* Completed tasks
* Subjects

You can:

* View upcoming tasks
* Filter tasks by priority or subject
* Mark tasks as completed or undo completion

---

### 📝 4. Manage Tasks

#### ➕ Add Task

* Go to **Add New Task**
* Enter:

  * Title
  * Description (optional)
  * Subject
  * Priority (Low, Medium, High)
  * Deadline (optional)

#### ✏️ Edit Task

* Click **Edit** on any task
* Update details or mark as completed

#### ✅ Complete Task

* Click **Mark Complete** on a task

#### 🔄 Undo Completion

* Use **Undo** to mark a task as pending again

---

### 📚 5. Manage Subjects

#### ➕ Create Subject

* Enter subject name and description

#### ✏️ Edit Subject

* Update subject details

#### ❌ Delete Subject

* Remove a subject (and its tasks)

---

## ✨ Features

* User authentication (login & registration)
* Task management (create, edit, delete)
* Subject organization
* Task prioritization
* Deadline tracking
* Progress overview dashboard
* Secure user-specific data

---

## 🛠️ Technologies Used

* **.NET Blazor (Server)**
* **C#**
* **Entity Framework Core**
* **ASP.NET Identity**
* **SQL Server**
* **Bootstrap (UI styling)**

---

## 📁 Project Structure (Simplified)

* **Components/** → UI pages (Dashboard, Tasks, Subjects)
* **Models/** → Data models (Task, Subject, User)
* **Services/** → Business logic (TaskService, SubjectService)
* **Data/** → Database context and configuration

---

## 📌 Notes

* Each user can only access their own tasks and subjects
* The application requires authentication to use core features
* Data is stored securely in the database

---
