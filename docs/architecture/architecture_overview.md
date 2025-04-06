# Architecture

## Overview

The project is implemented using the **MVC (Model-View-Controller)** architectural pattern on the **ASP.NET Core 8** platform. **Entity Framework Core** is used for data access, and **ASP.NET Core Identity** is used for authentication and authorization.

## Technological Stack

*   **Platform:** ASP.NET Core 8
*   **Language:** C#
*   **Data Access:** Entity Framework Core (EF Core)
*   **Database:** SQL Server (based on the initial Identity migration)
*   **Authentication:** ASP.NET Core Identity
*   **Frontend:** HTML, CSS, JavaScript, Bootstrap (from the standard template)

## Project Structure

The project is organized with a separation of concerns into layers (`Domain`, `Data`, `Services`, `Controllers`, `Views`) to improve maintainability. 

See project tree document for the detailed file structure.