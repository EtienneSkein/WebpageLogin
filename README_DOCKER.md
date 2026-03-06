# WebApp Login - Full Stack Demo

A full-stack web application demonstrating user authentication and management, built with modern technologies and containerized for easy deployment.

## Tech Stack

- **Frontend**: React 18 + TypeScript + Vite, Mantine UI
- **Backend**: ASP.NET Core 8 (.NET 8) - Split into Auth Service and User Service
- **Database**: PostgreSQL 15
- **Containerization**: Docker + Docker Compose
- **Authentication**: JWT tokens

## Features

- User registration with password validation
- JWT-based login and authentication
- Protected user profile page
- Microservices architecture (auth-service, user-service)
- Responsive UI with Mantine components

## Prerequisites

- Docker Desktop installed and running
- Optional: .NET 8 SDK and Node.js 18+ for local development

## Quick Start

1. Clone the repository and navigate to the root directory.

2. Copy the example environment file:
   ```bash
   cp .env.example .env
   ```

3. Run the full stack:
   ```bash
   ./build.sh
   ```
   Or manually:
   ```bash
   docker compose up --build
   ```

4. Access the application:
   - **Frontend**: http://localhost:5173
   - **Auth API**: http://localhost:8081
   - **User API**: http://localhost:8082

## Architecture

The application is structured as microservices:

- **db**: PostgreSQL database
- **auth-service**: Handles user registration and login (port 8081)
- **user-service**: Manages user profiles and data (port 8082)
- **frontend**: React SPA (port 5173)

Services communicate via HTTP and share the same database.

## Development

### Running Tests
```bash
./build.sh  # Runs all tests before building
```

### Local Development
- Backend: `cd backend/auth-service && dotnet run`
- Frontend: `npm run dev:frontend` (from root) or `cd frontend && npm run dev`

### API Endpoints
- Auth Service (8081):
  - POST /api/auth/register
  - POST /api/auth/login
- User Service (8082):
  - GET /api/users/me (requires JWT)

## Common Tasks

- Stop containers: `docker compose down`
- Rebuild: `docker compose build`
- View logs: `docker compose logs <service-name>`

## Service Architecture & Networking

- `db` runs PostgreSQL. Data is persisted into a Docker volume called `db-data`.
- `api` is built from `./backend`. It listens on port 8080 and depends on
the database.
- `frontend` builds the React app and serves it on port 5173. It depends on
  `api` and hits it using the environment variable `VITE_API_URL`.

All services share the default network that Docker Compose creates. Within that
network, containers can resolve each other by service name. For example, from
`api` you can connect to the database with the host `db` and port `5432`.

Enjoy your containerized full-stack development environment!