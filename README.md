# asp-mvc-ef-antique-bookstore

## Overview

**"Antique Bookstore"** is a study project that demonstrates development of a web application on the ASP.NET Core MVC platform using Entity Framework Core. The application is originally designed to replace oldfashioned index cards and lists, made to manage a catalog of antique books store, place and manage orders, keep orders history, etc.

[comment]: ![Label](images/2025-04-06-104845.png)


### Roadmap of the project

```
├── AntiqueBookstore
└── docs
    ├── analysis
    │   ├── analysis.md
    │   ├── business_scenario.md
    │   ├── functional_requirements.md
    │   └── requirements_srs.md
    ├── architecture
    │   └── architecture_overview.md
    ├── data_model
    │   ├── conceptual_model.md
    │   └── conceptual_model_diagram.drawio
    ├── database_schema
    │   ├── database_erd_dbml_diagram.dsl
    │   └── dbml_readme.md
    ├── deployment
    │   ├── release_notes.md
    │   └── user_guide.md
    ├── design
    │   └── wireframe.drawio
    ├── planning
    │   ├── development_plan_tasks.md
    │   ├── development_sdlc_agile.md
    │   ├── project_plan.md
    │   ├── project_tree.txt
    │   ├── ps-draw-project-tree.ps1
    │   └── ps-dump-project-code.ps1
    └── testing
        ├── test_case.md
        └── test_plan.md
```

### Structure of the project

```
AntiqueBookstore
├── Areas
│   └── Identity
├── Controllers
├── Data
│   ├── Configurations
│   ├── Interceptors
│   ├── Migrations
│   └── Seed
├── Domain
│   ├── Entities
│   ├── Enums
│   ├── Exceptions
│   └── Interfaces
├── Models
├── Properties
├── Services
├── TagHelpers
├── Views
│   ├── Authors
│   ├── Books
│   ├── Customers
│   ├── Home
│   ├── Orders
│   ├── Shared
│   └── UserManagement
└── wwwroot
    ├── css
    ├── images
    │   └── covers
    ├── js
    └── lib
        ├── bootstrap
        ├── bootswatch
        ├── jquery
        ├── jquery-validation
        └── jquery-validation-unobtrusive
```

### Requirements and steps to deploy

[TBD] *work in progress*