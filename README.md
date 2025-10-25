# ProjectManagerAPI

A .NET 8 REST API for managing projects and tasks with JWT authentication.

---

## Features

- **Authentication**: User registration and login using JWT (JSON Web Tokens)
- **Projects Management**: Create, read, update, and delete projects
- **Tasks Management**: Create, read, update, and delete tasks within projects
- **Bulk Task Creation**: Create multiple tasks in a single API call
- **User Isolation**: Users can only access their own data
- **SQLite Database**: Lightweight database for development and testing
- **Data Validation**: Input validation using DataAnnotations
- **Swagger Documentation**: Interactive API documentation

## Technology Stack

- .NET 8
- Entity Framework Core
- SQLite
- JWT Authentication
- BCrypt for password hashing
- Swagger/OpenAPI

---

## Project Structure

```
ProjectManagerAPI/
├── Controllers/
│   ├── AuthController.cs      # Authentication endpoints
│   ├── ProjectsController.cs  # Project management endpoints
│   └── TasksController.cs     # Task management endpoints
├── Data/
│   └── TaskManagerContext.cs # Entity Framework DbContext
├── DTOs/
│   ├── AuthDto.cs            # Authentication DTOs
│   ├── ProjectDto.cs         # Project DTOs
│   └── TaskDto.cs            # Task DTOs
├── Models/
│   ├── User.cs               # User entity
│   ├── Project.cs            # Project entity
│   └── Task.cs               # Task entity
├── Services/
│   ├── AuthService.cs        # Authentication business logic
│   ├── JwtService.cs         # JWT token generation
│   ├── ProjectService.cs     # Project business logic
│   └── TaskService.cs        # Task business logic
├── Program.cs                # Application configuration
└── appsettings.json          # Configuration settings
```
---

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login user and get JWT token

### Projects
- `GET /api/projects` - Get all projects for authenticated user
- `POST /api/projects` - Create a new project
- `GET /api/projects/{id}` - Get a specific project
- `PUT /api/projects/{id}` - Update a project
- `DELETE /api/projects/{id}` - Delete a project
- `POST /api/projects/{id}/tasks` - Create one or multiple tasks within a project

### Tasks
- `GET /api/tasks/project/{projectId}` - Get all tasks for a project
- `GET /api/tasks/{id}` - Get a specific task
- `POST /api/tasks/project/{projectId}` - Create a task (alternative endpoint)
- `PUT /api/tasks/{id}` - Update a task
- `DELETE /api/tasks/{id}` - Delete a task

---

## Prerequisites

- .NET 8 SDK
- Visual Studio Code or Visual Studio (optional)

---

## How to Run

1. **Clone or navigate to the project directory:**
   ```bash
   cd /path/to/ProjectManagerAPI
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Access the API:**
   - API Base URL: `http://localhost:5096`
   - Swagger UI: `http://localhost:5096/swagger/index.html`

---

## Testing the API

### 1. Register a User
```bash
curl -X POST "http://localhost:5096/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "username": "testuser",
    "password": "password123"
  }'
```

### 2. Login and Get Token
```bash
curl -X POST "http://localhost:5096/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123"
  }'
```

### 3. Create a Project
```bash
curl -X POST "http://localhost:5096/api/projects" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "title": "My Project",
    "description": "Project description"
  }'
```

### 4. Create Multiple Tasks (Bulk Creation)
```bash
curl -X POST "http://localhost:5096/api/projects/1/tasks" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '[
    {
      "title": "Task 1 - Design Phase",
      "dueDate": "2025-11-15T23:59:59Z"
    },
    {
      "title": "Task 2 - Development Phase",
      "dueDate": "2025-11-30T23:59:59Z"
    },
    {
      "title": "Task 3 - Testing Phase",
      "dueDate": "2025-12-15T23:59:59Z"
    }
  ]'
```

### 5. Create a Single Task (Backward Compatible)
```bash
curl -X POST "http://localhost:5096/api/projects/1/tasks" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '[
    {
      "title": "Single Task",
      "dueDate": "2025-12-31T23:59:59Z"
    }
  ]'
```

### 6. Update a Task
```bash
curl -X PUT "http://localhost:5096/api/tasks/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "title": "Updated Task",
    "dueDate": "2025-12-25T23:59:59Z",
    "isCompleted": true
  }'
```
---

## Database

The application uses SQLite database (`TaskManager.db`) which is automatically created when the application starts. The database file will be created in the project root directory.

## Configuration

Key configuration settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TaskManager.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ProjectManagerAPI",
    "Audience": "ProjectManagerAPI",
    "ExpirationHours": "24"
  }
}
```
---

## Development Notes

- The API runs on HTTP only (no HTTPS redirection)
- Database is automatically created and migrated on startup
- All timestamps are stored in UTC
- JWT tokens expire after 24 hours by default
- Swagger documentation is available in development mode