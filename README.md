## Lesson: From Data Access Class to Repository Pattern

In the previous lesson we worked directly with ADO.NET:

- Open a connection

- Write SQL

- Execute commands

- Map rows into objects

All of that logic sat in `Program`.

That gave us clarity about how data access actually works.

Now we focus on structure.

## Step 1 – The Problem: Mixed Responsibilities

When everything lives in `Program`, one method is doing multiple jobs:

* Deciding *what* the application wants (get employees, add employee)
* Managing connections
* Writing SQL
* Mapping rows into objects

Those are different concerns.

Application logic should not know:

* Connection strings
* SQL syntax
* SqlCommand
* SqlDataReader

That is infrastructure.

This violates separation of concerns.

## Step 2 – First Improvement: Data Access Class (DAC)

We extracted all SQL-related logic into:

```csharp
EmployeeDataAccess
```

Now:

* Program expresses intent.
* EmployeeDataAccess handles storage mechanics.

This is our first real architectural boundary.

We encapsulated what varies.

What varies?

* The database engine
* The schema
* The SQL
* The mapping rules

All of that now lives in one place.

## Step 3 – But There’s Still Coupling

Even after extraction, Program still depends on:

```csharp
EmployeeDataAccess
```

That means:

* Program is coupled to a concrete implementation.
* If we changed storage (e.g. to an API, file, different DB), Program would change.

This is where the next principle enters:

> Program to abstractions, not implementations.

## Step 4 – Introducing the Repository Pattern

The Repository pattern formalizes the boundary.

Instead of Program depending on:

```csharp
EmployeeRepository
```

It depends on:

```csharp
IEmployeeRepository
```

The interface defines capability.
The class defines implementation.

Program now depends on *what can be done*, not *how it is done*.

That reduces coupling.

## Why a Generic `ICrudRepository<T, ID>`?

We noticed repetition:

Most data access layers have the same operations:

* GetAll
* GetById
* Add
* Update
* Delete

That pattern repeats across entities.

So we extracted that pattern into a reusable abstraction:

```csharp
ICrudRepository<T, ID>
```

This captures common behavior.

It does **not** know about Employee.

It does **not** know about SQL Client.

It defines capability.

This is an example of identifying invariants (stable patterns across contexts).

CRUD is invariant.
Employee-specific queries vary.

## Why a Second Interface? (IEmployeeRepository)

Employees have domain-specific queries:

* GetAllActive()
* GetBySalaryAbove(...)
* GetAllShort(...)

Those do not belong in a generic CRUD contract.

So we extend:

```csharp
IEmployeeRepository : ICrudRepository<Employee, int>
```

This gives us a family of interfaces.

This ties directly into SOLID:

* **I – Interface Segregation Principle**
  Interfaces should represent meaningful, cohesive capabilities.
  We do not want one giant interface for everything.

* **D – Dependency Inversion Principle**
  High-level modules (Program) depend on abstractions, not concrete classes.

## Layers in This Example

Application Layer
→ `Program.cs`
→ Expresses intent
→ Depends on `IEmployeeRepository`

Repository Layer
→ Implements `IEmployeeRepository`
→ Contains SQL + mapping

Database Layer
→ SQL Server

Each layer has a clear responsibility.

## Repository Pattern in One Sentence

A repository is a boundary that hides data storage mechanics behind a domain-focused interface.

## What Changed Architecturally?

Before:

Program → SQL → Database

After:

Program → IEmployeeRepository → EmployeeRepository → SQL → Database

We inserted a boundary.

That boundary allows:

* Lower coupling
* Easier testing
* Easier replacement
* Clearer responsibilities

## Final Conceptual Connections

Separation of Concerns
We separated application intent from storage mechanics.

Encapsulation
We encapsulated SQL and mapping inside the repository.

Program to Abstractions
We depend on interfaces, not concrete classes.

Interface Segregation
We split generic CRUD from domain-specific operations.

Dependency Inversion
High-level code depends on abstractions.