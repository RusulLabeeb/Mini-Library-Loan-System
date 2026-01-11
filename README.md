# 🧪 Task 1: Extend & Inhance the Books API

You already have a working Books API.

Your task is to extend it by adding a few new endpoints and improving existing ones.

Note that there is no need for having a Database or adding any external packages. All you need to do is create some new `.cs` files.


## Requirements

### 1. Fix the “Get Single Book” endpoint
Current behavior:
GET `/books/{id}` always returns the first book.

Your task is to update this endpoint so that:

- It returns the book with the matching id

- If the book does not exist, return 404 Not Found

📌 Hint:
Use `FirstOrDefault` from `LINQ`.

### 2. Add an Update (PUT) endpoint
Create a new endpoint: `PUT /books/{id}`

**_Behavior:_**

- Accepts a `BookRequest` from the body

- Updates the Title and Category of the existing book

- If the book does not exist → return 404 _Not Found_

- If the category is invalid → return 400 _Bad Request_

- If successful → return 204 _No Content_

📌 Rules:

- The Id must not be changed

- Reuse the same allowed categories logic


### 3. Add a Get-By-Category endpoint

Create a new endpoint: `GET /books/category/{category}`

**_Behavior:_**
Behavior:

- Returns all books that belong to the given category

- If no books exist in that category → return an empty list
  
- Category matching should be case-insensitive. (i.e sending `GET /books/category/novel` should behave the same as if we send `GET /books/category/NOvel
`)

### 4. Prevent Duplicate Titles (Creating a Book)
Improve the existing POST endpoint: `POST /books`

New Rule:
- If a book with the same title already exists → return 400 Bad Request.
- Titles should be compared case-insensitively


# Task 2: Add Authors to the API
You are going to extend the API by introducing Authors. This task focuses on:
- Creating a new model class.
- Creating a new controller.
- Practicing basic CRUD operations.

You need to create a new `Author` class as below: 
```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

Create a new controller called `AuthorsController` with following details:
- Route: `/authors`.
- Use `[ApiController]`.
- Inherit from `ControllerBase`.
- Store authors in a static in-memory list.
- Have some initial data. Start with at least 2 authors in the list, example below:
```csharp
public static List<Author> authors = new List<Author>(){
    new Author { Id = 1, Name = "George Orwell" }
    new Author { Id = 2, Name = "Yuval Noah Harari" }
};
```


## Requirements

### 1. Implement `GET /authors`
- It should return all authors in the list.

### 2. Implement `GET /authors/{id}`
- Returns a single author by `id`.
- If not found → return 404 `Not Found`.

### 3. POST /authors
- Accepts an `AuthorRequest` object from the request body (you need to create this class and it should contain only a string `Name` property)
- Automatically generates a new Id.
- Basic validation: If `Name` is empty or shorter than 3 characters → return 400 `Bad Request`
- If successful → return 201 `Created`

### 4. Implement `DELETE /authors/{id}`
- Deletes the author with the given id.
- If the author does not exist → return 404 `Not Found`.
- If deleted → return 204 `No Content`.

