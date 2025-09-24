**Entities Overview**


In this project, I model three core entities: User, Post, Comment.

I follow a relational-database style approach:


- Every entity has a primary key (Id of type int).
- Relationships are modeled using *Foreign keys* rather than associations or references.


**User**

Represents a person who interacts with the system

- **Id (int)** -> Primary Key
- **Username (string)** -> The unique name chosen by the user.
- **Password (string)** -> The user's password

A user can:

- Write **Posts**
- Write **Comments**

**Post**

Represents a message created by user.

- **Id (int)** -> Primary Key
- **Title (string)** -> The title of the post
- **Body (string)** -> The content of the post
- **UserId (int)** -> Foreign Key referencing the **User** who wrote the post

A post can have multiple **Comments**, but we model this via *PostId* foreign key inside *Comment*.

**Comment** 

Represents a user's reply to a post.

- **Id (int)** -> Primary Key
- **Body (string)** -> The text of the comment
- **UserId (int)** -> Foreign key referencing the **User** who wrote the comment
- **PostId (int)** -> Foreign key referencing the **Post** the comment belongs to

---
**Repository Layer**

**What is a Repository?**
A **Repository** is an abstraction of data access for a single entity.
- Each entity (e.g., User, Post, Comment) has its own repository interface.
- The repository hides how data is stored (List, file, database, etc.).
- Higher layers (CLI, Web API, etc.) work only with the repository interface, not the actual storage.

This creates a **layered architecture** where:

- The **presentation/network layer** (CLI, Web API) can change.
- The **persistence layer** (List, file, DB) can change.
- But the **repository interface remain the same**.

---
**CRUD Methods**

Each repository interface defines **5 standard CRUD operations:**

1. **Add**
- Stores a new entity
- Returns the created entity (with *Id* set by the system)
2. **Update**
- Updates an existing entity
- If the entity does not exist, an exception is thrown
3. **Delete**
- Deletes an entity by its *Id*
- If no entity is found, an exception is thrown
4. **GetSingle**
- Returns the entity matching the given *id*
- If not found, an exception is thrown
5. **GetMany**
- Returns an *IQueryable* with multiple entities
- Supports iteration (foreach) and filtering using LINQ

---
**Task and Async**

- *Task T* = represents an operation that will eventually return a result of type *T*.
- *Task* = represents an operation that will eventually complete (but has no return value).
- Methods are suffixed with *Async* by convertion (e.g., AddAsync).
- This enables **asynchronous programming**, where different operations (e.g., read from a DB oe file) can run concurrently.

---
**In Memory Repository Inmlementation**


![img_1.png](img/img_1.png)

![img.png](img/img.png)

![img_2.png](img/img_2.png)

![img_3.png](img/img_3.png)

---
**Dummy Data Initialization**

To make testing easier, each in-memory repository (User, Comment, Post) comes preloaded with some sample data.

This ensures the application has content right after startup, making it easier to test CRUD operations.

---
## CLI 
**Purpose**

This project is a **C# .NET console application** that simulates a mini blog system with **Users, Posts, and Comments.**
The main goals were to practice:

- **Repository Pattern**

- **Dependency Injection**

- **Asynchronous programming (async/await)**

- **Clean UI design (views split by responsibility)**

- **Initial dummy data seeding**
---
## Core Concepts in this project

**Dependency Injection**

- We only instantiate repos in **Program.cs**
- These are passed into CliApp, and further into views.

**WHY?**

- Keeps all parts of app working on the same instance.

- Easy to replace InMemoryRepository with DbRepository later.

- Avoids “new UserInMemoryRepository()” being spread all over your code (which would break data consistency).

---
**Asynchronous Programming**

- In .NET, data access is often **I/O bound** (files, DB, network). Async keeps the app responsive.
- Any repo method that touches data is async:

  *User user = await userRepo.AddAsync(new User {...});*

- To use await, the method itself must be async Task:

  *private async Task CreateUserAsync()*

  *{*

  *User created = await userRepository.AddAsync(user);*
  *Console.WriteLine(created.Id);*

  *}*
- **Turtles all the way down** → if you call async, your method must be async, and so on, all the way to Main.

- That’s why Program.cs has: *await cliApp.StartAsync();*

---
**UI Design (Views)**

- Instead of one giant CLI class, we split into small, single-responsibility views:

- - CreateUserView → only user creation

- - ListUsersView → only listing users

- - CreatePostView, ListPostsView, SinglePostView, AddCommentView

- This improves readability, testability, and teamwork.

This follows **Single Responsibility Principle (SRP)** from SOLID

---
## File Repository – Method Explanations

Each repo also has some private helpers (constructor setup, LoadAllAsync, SaveAllAsync, and SeedIfEmptyAsync) and a SemaphoreSlim to avoid concurrent write collisions.

- System.Text.Json for serialization

- File.ReadAllTextAsync / WriteAllTextAsync for async I/O

- SemaphoreSlim to serialize writes

- AppContext.BaseDirectory + Data/ as storage location

---
**Constructor** 


![img.png](img/img_5.png)

**What it does**

1. **Ensures storage exists:** Creates the Data/ folder if missing.

2. **Ensures file exists:** Creates xxx.json with an empty JSON array [ ] if missing.

3. **Seeds data (optional requirement):** If the list is empty, writes initial dummy entities.

**Why**

- So the repository is safe to use immediately, without manual setup.

- Seeding gives you realistic data for testing and demos.

**Pitfalls covered**

- Repeated runs are idempotent: CreateDirectory and the existence check won’t break anything.

---
**AddAsync(T entity)**


![img_4.png](img/img_4.png)

**What it does**

1. **Locks** to prevent concurrent writers from stepping on each other.

2. **Loads** the entire list from the file.

3. **Generates a new Id** as maxId + 1 (or 1 if empty).

4. **Adds** the entity to the list.

5. **Saves** the entire list back to disk.

**Why**

- File storage isn’t transactional; we serialize the whole collection atomically.

- Assigning IDs in the repository keeps UI simple.

**Edge cases**

- If items were deleted, IDs aren’t reused (common and OK).

- If two threads add simultaneously, SemaphoreSlim prevents ID collisions.

---
**UpdateAsync(T entity)**


![img_6.png](img/img_6.png)

**What it does**

1. **Locks** writes.

2. **Loads** the full list.

3. **Finds** the entity by Id.

4. **Overwrites** the entity in place.

5. **Saves** back to the file.

**Why**

- Simple, clear semantics: full read → modify in memory → full write.

**Edge cases**

- Missing entity: we silently return (or you can throw). Decide and document behavior.

---
**DeleteAsync(int id)**


![img.png](img/img_7.png)

**What it does**

1. **Locks** writes.

2. **Loads** list.

3. **Removes** all items with matching Id.

4. **Saves** list.

**Why**

- Full overwrite keeps the on-disk file consistent.

- RemoveAll handles duplicates defensively (there shouldn’t be any).

**Edge cases**

- Deleting a non-existing id is a no-op (or throw; your choice).

---
**GetSingleAsync(int id)**


![img.png](img/img_8.png)

**What it does**

1. **Loads** the list (async).

2. **Returns** the first item with that Id, or null.

**Why**

- Read operations can avoid locking since we’re not mutating, and file reads are atomic per call.

**Edge cases**

- null return means “not found” (UI can decide how to report).

---
**GetMany() (non-async)**


![img.png](img/img_9.png)

**What it does**

1. **Synchronously** unwraps ReadAllTextAsync using .Result.

2. **Deserializes** the JSON to a list.

3. **Returns** an IQueryable<T> over the in-memory list.

**Why the “cheat”**

- The interface requires a **non-async** method returning IQueryable<T>.

- We can’t await here, so we use .Result exactly as the teacher advised.

**Caveats (good to mention in exam)**

- .Result can deadlock in ASP.NET contexts; okay in a simple console app.

- If we later switch to EF Core, IQueryable will be backed by the DB provider and translated to SQL; our UI code remains unchanged.

---
**LoadAllAsync() (private helper)**


![img.png](img/img_10.png)

**What it does**

- Reads the entire file asynchronously and deserializes it to a List<T>.

**Why**

- Centralizes deserialization and default handling (?? new()).

**Edge cases**

- If the file is empty/corrupt, you might add try/catch → return empty list or rethrow.

---
**SaveAllAsync() (private helper)**


![img.png](img/img_11.png)

**What it does**

- Serializes the whole list and writes it back to the file asynchronously.

**Why**

- Keeps file state in sync with memory changes.

**Edge cases**

- Consider handling IO exceptions if you want robust error messages.

---
**SeedIfEmptyAsync() (private helper)**


![img.png](img/img_12.png)

**What it does**

- If the file contains [] or nothing, writes a few initial entities.

**Why**

- Makes the app usable immediately (test listing/reading without manual creation).

---