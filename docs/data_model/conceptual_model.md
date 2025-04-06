# Conceptual Data Model

## Overview

This document describes the high-level conceptual data model for the 'Antique Bookstore' web application. A visual representation can be found in the diagram .drawio files.

## Core Entities 

*   **Book:** Represents a book in the catalog.
*   **Author:** Represents the author of a book.
*   **Category:** Represents the category or genre of a book.
*   **User:** Represents a registered user (implemented via ASP.NET Core Identity).
*   **Order:** Represents an order placed.
*   **OrderItem:** Represents an item within an order.

## Key Relationships 

*   Book -> Author(s)
*   Order -> User
*   Order -> OrderItems
*   OrderItem -> Book

etc.