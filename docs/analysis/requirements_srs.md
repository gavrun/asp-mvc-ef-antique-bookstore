# Software Requirements Specification (SRS) - Antique Bookstore

**Version:** 0.1
**Date:** [Date]

**Comment:** This document is a mockup. 

**Reference:** This document's structure is based on **IEEE Std 830-1998**. Content aligns with **ISO/IEC/IEEE 29148:2018**.

---

## 1. Introduction

### 1.1 Purpose
Defines functional and non-functional requirements for the "Antique Bookstore" web application, guiding design, implementation, and testing.

### 1.2 Scope
**In Scope:** A web application enabling administrators to manage catalog (books, authors, categories) and users to browse, search, register/login, manage cart, and place orders. Data storage via relational database.

**Out of Scope:** Payment integration, delivery management, analytics.

### 1.3 Definitions & Acronyms
*   **SRS:** Software Requirements Specification
*   **MVC:** Model-View-Controller
*   **EF Core:** Entity Framework Core
*   **System:** The "Antique Bookstore" web application
*   **User:** Registered user.
*   **Admin:** User with catalog management privileges.
*   **Book:** Catalog item.
*   **Order:** User-selected books for purchase.

### 1.4 Document Overview
Section 2 provides a system overview. Section 3 details specific requirements (Functional, Non-Functional, Interface, Data).

---

## 2. Overall Description

### 2.1 Product Perspective
A standalone ASP.NET Core 8 web application using EF Core and ASP.NET Core Identity.

### 2.2 Product Features (Summary)
*   Catalog Management (Books, Authors, Categories)
*   Book Browsing
*   User Account Management (Registration, Login)
*   Order Placement & History

### 2.3 User Characteristics
*   **Customers:** General users familiar with e-commerce.
*   **Administrators:** Staff responsible for catalog upkeep.

### 2.4 Constraints
*   Technology: .NET 8, SQL Server (or compatible RDBMS).
*   Functionality: Excludes payment processing in this version.

### 2.5 Assumptions & Dependencies
*   Users have internet access and a modern browser.
*   Deployment server and database infrastructure are available.

---

## 3. Specific Requirements

### 3.1 Functional Requirements

#### Catalog Management (Admin)
*   **FR-CAT-01:** Admins must be able to perform CRUD operations on Books (incl. details, relationships, image).
*   **FR-CAT-02:** Admins must be able to perform CRUD operations on Authors.

#### Browsing (User)
*   **FR-BRW-01:** The system must display a paginated list of Books with key info.
*   **FR-BRW-02:** The system must display detailed information for a selected Book.

#### User & Order Management (User)
*   **FR-USR-01:** Users must be able to register an account (via Identity).
*   **FR-USR-02:** Users must be able to log in (via Identity).
*   **FR-ORD-01:** Users must be able to add Books to catalog.
*   **FR-ORD-02:** Users must be able to place an Order (simplified address info).

---

### 3.2 Non-Functional Requirements

*   **NFR-PERF-01 (Performance):** Key page loads < 1s (avg. under typical load).
*   **NFR-SEC-01 (Security):** HTTPS required; Passwords hashed (Identity); Admin functions role-restricted.
*   **NFR-USAB-01 (Usability):** Intuitive interface; Consistent navigation; Basic mobile responsiveness.
*   **NFR-REL-01 (Reliability):** Target uptime.

---

### 3.3 Interface Requirements

*   **UI:** Web interface via modern browsers (Chrome, Firefox, Edge).
*   **Programmatic:** EF Core for database interaction.
*   **Hardware:** Standard client/server hardware assumed.

---

### 3.4 Data Requirements

*   Data structure as per conceptual model.
*   Persistent storage in SQL Server.
*   Data integrity constraints enforced (e.g., valid foreign keys).

---
