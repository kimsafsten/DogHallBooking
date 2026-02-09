# HallBooking
By **Kim Säfsten**  
2026-02-05  
School project for **MI Systemutvecklare.NET**  
Course: **Entity Framwork**

A console application written in C# using **Entity Framework Core (Code First)** together with  
**PostgreSQL**.  
The application is a booking system for a dog training hall and was created as a school
assignment to demonstrate skills in EF Core.

---

## Technology
- .NET
- Entity Framework Core
- PostgreSQL
- EF Core Migrations (Code First)

---

## Database

The connection string is configured in `AppDbContext`:


```csharp
optionsBuilder.UseNpgsql(
    "Host=localhost;Database=hall_booking_db;Username=postgres;Password=postgres");
```
Change username, password, or database name if needed.

## Create and seed database

 1. Start PostgreSQL

 2. Create the database (run in Query Console):
```SQL
CREATE DATABASE hall_booking_db;
```
3. Run migrations (inncludes seed data):
```bash
dotnet ef database update
```
Seed data is defined in migrations and is created automatically.

 4. Start the application
 
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