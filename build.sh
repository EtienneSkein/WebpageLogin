#!/usr/bin/env bash
set -e

echo "Running tests..."
cd backend/auth-service && dotnet test --verbosity minimal
cd ../user-service && dotnet test --verbosity minimal
cd ../../frontend && npm test -- --watchAll=false
cd ../..

echo "Building and starting containers..."
docker compose up --build

echo ""
echo "Frontend: http://localhost:5173"
echo "Auth API: http://localhost:8081"
echo "User API: http://localhost:8082"
