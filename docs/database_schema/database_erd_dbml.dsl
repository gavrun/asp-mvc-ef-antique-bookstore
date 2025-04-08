Table Book {
    BookID char(8) [pk, increment]
    Title varchar [not null]
    Publisher varchar [null]
    PublicationDate year [null, note: 'more 1600 and less 2099']
    PurchasePrice decimal [null]
    ConditionID int [ref: > BookCondition.ConditionID, not null]
    RecommendedPrice decimal [null]
    StatusID int [ref: > BookStatus.StatusID, not null]
}

Table BookCondition {
    ConditionID int [pk, increment]
    Condition varchar [not null, unique, note: 'Excellent, VeryGood, Good, Fair, Poor, Damaged']
    Description varchar [null]
    IsUsed boolean [not null, default: false]
}

Table BookStatus {
    StatusID int [pk, increment]
    Name varchar [not null, unique, note: 'InStock, OnOrder, Sold']
    Description varchar [null]
    IsUsed boolean [not null, default: false]
}

Table Author {
    AuthorID int [pk, increment]
    FirstName varchar [not null]
    LastName varchar [not null]
    BirthYear year [not null]
    DeathYear year [null]
    Bio varchar [null]
}

Table BookAuthor {
    BookID char(8) [ref: > Book.BookID, not null]
    AuthorID int [ref: > Author.AuthorID, not null]
    Primary Key (BookID, AuthorID) [note: 'composite key to avoid duplicates']
}

Table Employee {
    EmployeeID int [pk, increment]
    FirstName varchar
    LastName varchar
    HireDate date [not null]
    IsActive boolean [not null, default: true, note: 'Active, Inactive']
    Comment varchar [null]
}

Table Position {
    PositionID int [pk, increment]
    Title varchar [not null, unique]
    RoleID int [ref: > Role.RoleID, not null]
    WorkSchedule enum('FullTime', 'PartTime') [note: 'never changes']
}

Table Role {
    RoleID int [pk, increment]
    Name varchar [not null, unique, note: 'Manager, Sales']
    Description varchar [null]
    IsExists boolean [not null, default: false]
}

Table PositionHistory {
    PromotionID int [pk, increment]
    EmployeeID int [ref: > Employee.EmployeeID, not null]
    PositionID int [ref: > Position.PositionID, not null]
    StartDate date [not null, note: 'first hire date']
    EndDate date [null, note: 'updated on fire or position change']
    IsActive boolean [not null, default: false]
}

Table Customer {
    CustomerID int [pk, increment]
    FirstName varchar [not null]
    LastName varchar [not null]
    Phone varchar [null]
    IsActive boolean [not null, default: true, note: 'True for active, false for inactive']
    Comment varchar [null]
}

Table DeliveryAddress {
    AddressID int [pk, increment]
    CustomerID int [ref: > Customer.CustomerID, not null]
    AddressAlias varchar [null]
    Country varchar [not null]
    Index int [not null]
    City varchar [not null]
    Address varchar [not null]
    Details varchar [null]
}

Table Order {
    OrderID int [pk, increment]
    CustomerID int [ref: > Customer.CustomerID]
    EmployeeID int [ref: > Employee.EmployeeID]
    DeliveryAddressID int [ref: > DeliveryAddress.AddressID, null]
    OrderStatusID int [ref: > OrderStatus.OrderStatusID, not null]
    OrderDate date
    DeliveryDate date [null]
    PaymentMethodID int [ref: > PaymentMethod.MethodID, not null]
    PaymentDate date
}

Table OrderStatus {
    OrderStatusID int [pk, increment]
    Name varchar [not null, unique, note: 'PendingShipment, StorePickup, Shipped, Received']
    Description varchar [null]
    IsUsed boolean [not null, default: false]
}

Table PaymentMethod {
    MethodID int [pk, increment]
    Name varchar [not null, unique]
    Description varchar [null]
    IsUsed boolean [not null, default: false]
}

Table Sale {
    SaleID int [pk, increment]
    OrderID int [not null, ref: > Order.OrderID]
    BookID char(8) [ref: > Book.BookID, not null]
    SalePrice decimal [not null]
    EventID varchar [ref: > SaleEvent.EventID, not null, note: 'holiday sales, Christmas sale']
}

Table SaleEvent {
    EventID int [pk, increment]
    Name varchar [not null]
    Discount int [not null, note: 'affects sale price']
    StartDate date [not null]
    EndDate date [not null]
}

Table SalesAuditLog {
    EventID int [pk, increment]
    Timestamp timestamp [not null, default: `now()`]
    TableName varchar [not null]
    Operation varchar [not null]
    ColumnName varchar [not null]
    OldValue varchar [not null]
    NewValue varchar [not null]
    Login varchar [note: 'EmployeeID uses sql login']
}
