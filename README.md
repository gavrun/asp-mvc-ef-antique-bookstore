# asp-mvc-ef-antique-bookstore


## Overview

**"Antique Bookstore"** is a study project that demonstrates a full cycle for development of a web application.

Made with C# on the ASP.NET Core platform using Entity Framework Core, follows MVC architecture pattern, and uses Razor pages for the front end. 

The project is originally designed for a fictitious company that sells antique books based on a note which a store owner made on a piece of paper.

A simple web application that allows a store staff to maintain a catalog of antique books, as well as placing and managing orders, to replace oldfashioned index cards and lists for orders history, etc.

The goal here is to learn how software industry works.


[comment]: ![Label](images/2025-04-06-104845.png)


### Roadmap of the project (product documentation)

```
├── AntiqueBookstore
└── docs
    ├── analysis
    │   ├── analysis.md
    │   ├── business_scenario.md
    │   ├── functional_requirements.md
    │   └── requirements_srs.md
    ├── api
    │   ├── api_reference.md
    │   ├── api_requirements.md
    │   └── swagger.json
    ├── architecture
    │   └── architecture_overview.md
    ├── data_model
    │   ├── conceptual_model.md
    │   └── conceptual_model_diagram.drawio
    ├── database_schema
    │   ├── database_erd_dbml.dsl
    │   ├── database_erd_diagram.png
    │   ├── database_erd_diagram.svg
    │   ├── database_erd_diagram_concept.png
    │   ├── dbml_readme.md
    │   └── localdb_export_script.sql
    ├── deployment
    │   ├── backup_and_recovery.md
    │   ├── ci_cd_pipeline.md
    │   └── infrastructure.md
    ├── legal
    │   ├── license.md
    │   ├── privacy_policy.md
    │   ├── service_level_agreement.md
    │   └── terms_of_service.md
    ├── planning
    │   ├── development_plan_tasks.md
    │   ├── development_sdlc_agile.md
    │   ├── project_plan.md
    │   ├── project_tree.txt
    │   ├── ps-draw-project-tree.ps1
    │   └── ps-dump-project-code.ps1
    ├── release_
    │   ├── changelog.md
    │   ├── readme.md
    │   ├── release_notes.md
    │   └── user_guide.md
    ├── security
    │   ├── data_protection_policy.md
    │   ├── security_requirements.md
    │   └── threat_model.md
    ├── support
    │   └── support_contacts.md
    ├── testing
    │   ├── test_case.md
    │   └── test_plan.md
    ├── ui_design
    │   ├── ui_style_guide.md
    │   └── wireframe.drawio
    └── ux
        ├── personas.md
        └── usability_test_plan.md
```

### Structure of the project (solution)

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

*work in progress*