# Recipe Book Service

### Pre-Requisite

- C#
- ASP .Net Core
- Sql server

### How to run the project

 run `docker-compose up` to start the kafka , zookeeper and sqlserver containers

### Api Documentation
- [http://localhost:9095/swagger-ui/index.html](http://localhost:5009/swagger/index.html)

### What it contains

- [X] Recipe Book Service
- [X] Recipe Book Unit Test
- [X] Recipe Book Integration Test on the Login endpoint

### Default Users

- "Email": "admin@example.com",
  "Password": "Admin@123",
  "Role": "ADMIN"

- "Email": "user@example.com",
  "Password": "User@123",
  "Role": "USER"
