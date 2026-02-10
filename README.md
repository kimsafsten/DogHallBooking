# HallBooking
By **Kim Säfsten**  
2026-02-05  
School project for **MI Systemutvecklare.NET**  
Course: **Entity Framwork**

A console application written in C# using **Entity Framework Core (Code First)** and  
**PostgreSQL**.  
The application is a bookingsystem for a dog training hall and was created as a school
assignment to demonstrate skills in EF Core.

---

## Technology
- .NET
- Entity Framework Core
- PostgreSQL
- EF Core Migrations (Code First)

---

## Database

The application uses **PostgreSQL** with **Entity Framework Core (Code First)**.

Database configuration is handled via `appsettings.json`.
An example file is provided as `appsettings.Example.json`.


```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=hall_booking_db;Username=postgres;Password=CHANGEME"
  }
}
```
Create `appsettings.json` from the example and update the connection string to match your local setup.
The file is excluded from version control.

## Create and seed database

 1. Create the database:
```SQL
CREATE DATABASE hall_booking_db;
```
2. Apply migrations (inncludes seed data):
```bash
dotnet ef database update
```
Seed data is defined in migrations and is created automatically.

 3. Run the application
 
 ## Log in
 
 **Member**
  - Log in using an email address
   - If email does not exist,a new member can be created

 Seeded emails:
 - cissi@example.com
 - brotherbear@mail.com
 - anna@mail.com

 **Admin**
 - Password **admin26**

## Functionality

**Member**

 - Update own details
 - View courses and enroll
 - View available times and book the hall

**Admin**
 - Manage members, courses, and course sessions (CRUD)
 - Manage course enrollments (many-to-many relation)
 - View and modify hall bookings
 - View and update payments

## Summary
The project demonstrates EF Core using relational modeling, migrations, seed data,
projections, aggregation, and transactions in a fully functional console application.