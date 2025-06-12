# KickstarterAPI

KickstarterAPI is a ASP.NET Core 9 Web API project designed to manage Kickstarter projects. It provides RESTful endpoints for performing CRUD operations. Users can interact with the application via Scalar or Postman.

## Project setup

Run the project in Visual Studio or JetBrains Rider. It should automatically apply migrations and seed the database.

## Usage

Once the application is running, you can interact with it using either Scalar or Postman.

To access Scalar:

1. Navigate to `https://localhost:<port>/scalar` in your web browser.
2. Explore available endpoints and perform operations.

## Endpoints

The API exposes the following endpoints:

- **Kickstarters**: 
  - `GET /api/Kickstarter`: Get all Kickstarter projects.
  - **Parameters**:
    - `ID optional`: Search by ID.
    - `Name optional`: Search by Name.
    - `Category optional`: Search by Category.
    - `Subcategory optional`: Search by Subcategory.
    - `Country optional`: Search by Country.
    - `LaunchedFrom
       LaunchedTo optional`: You can set the parameter to give min or max or range.
    - `DeadlineFrom
       DeadlineTo optional`: You can set the parameter to give min or max or range.
    - `GoalMin
       GoalMax optional`: You can set the parameter to give min or max or range.
    - `PledgedMin
       PledgedMax optional`: You can set the parameter to give min or max or range.
    - `BackersMin
       BackersMax optional`: You can set the parameter to give min or max or range.
    - `State`: Search by status.
    - `SortBy`: Sort by table names.
    - `SortDirection`: `desc` or `asc`.

  - `GET /api/Kickstarters/{id}`: Get a specific Kickstarter project by ID.
  - `POST /api/Kickstarter`: Create a new Kickstarter project.
  - `PUT /api/Kickstarter/{id}`: Update an existing Kickstarter project.
  - `DELETE /api/Kickstarter/{id}`: Delete a Kickstarter project.

- **Users**:
  - `POST /api/Users/login`: Login in to user.
  - `POST /api/Users/Register`: Register a new user.

## Authentication/Authorization

KickstarterAPI utilizes JWT Bearer Token for authentication and authorization. To access protected endpoints, include the JWT token in the Authorization header of your requests:
 ```
 Bearer YOUR_JWT_TOKEN
 ``` 
