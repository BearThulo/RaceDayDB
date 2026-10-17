# RaceDay

## Project Overview

RaceDay is an event management project for running, walking and cycling events. It includes database planning documents and an ASP.NET Core Web API for managing event information.

## Technologies Used

- C#
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- Microsoft SQL Server
- Swagger / OpenAPI

## Project Structure

- `RaceDayDB/` contains the database script and Part 1 planning documents.
- `RaceDay.API/RACEDAY.API/` contains the Web API source code.
- `EventsController.cs` handles event-related API requests.
- `RaceDayDbContext.cs` connects the application to its database through Entity Framework Core.

## Events API

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/Events` | Retrieve all events |
| GET | `/api/Events/{id}` | Retrieve one event |
| POST | `/api/Events` | Create an event |
| PUT | `/api/Events/{id}` | Update an event |
| DELETE | `/api/Events/{id}` | Delete an event |

## Running the Project

1. Install the .NET 10 SDK and Microsoft SQL Server.
2. Open the solution in Visual Studio.
3. Update the development connection string to point to your local database.
4. Ensure the database and required tables are available.
5. Build and run the API project.
6. Test the endpoints using Swagger, when enabled, or the provided HTTP request file.

## Database

The SQL script in the `RaceDayDB/` folder provides the database setup. Check that the connection string uses the correct SQL Server instance and database.

## Testing

Use the API project’s HTTP request file or Swagger interface to test the Events endpoints and verify their responses.
