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
