 RaceDay7 – Event Management System (Part 3)

## Project Description

RaceDay7 is a full-stack web-based event management system developed for the South African road running, walking and cycling community.

This repository contains **Part 3** of the Portfolio of Evidence (POE). This phase focuses on developing the MVC web application, integrating Azure Blob Storage for event image management and implementing Docker containerisation supported by a GitHub Actions CI/CD workflow.

## User Roles

The system supports three user roles:

1. **Participant:** Can browse available events, submit event entries (subject to Organiser approval), view personal performance history and manage their profile.
2. **Organiser:** Can create, edit and delete events, manage event categories, approve participant entries and capture event results.
3. **Admin:** Has administrative access to the system, including user management and the ability to override event permissions.

## Technology Stack

- **Framework:** ASP.NET Core 8.0 (MVC)
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity with custom one-to-one role mapping
- **Cloud Storage:** Microsoft Azure Blob Storage
- **Containerisation:** Docker
- **CI/CD:** GitHub Actions
- **Container Registry:** Azure Container Registry (ACR)

## Setup and Run Instructions (Local Development)

Follow these steps to run the application locally:

1. Clone this repository to your local machine.
2. Open the solution in Visual Studio.
3. Configure the required SQL Server and Azure Storage connection strings in `appsettings.json`.
4. Open the Package Manager Console in Visual Studio.
5. Execute the following command to apply the Entity Framework Core database migrations:

   `Update-Database`

6. Build and run the application by pressing `F5` in Visual Studio.

**Note:** Valid database and Azure Storage configuration settings are required for the application to operate correctly.

## Docker Deployment

The application is containerised using a multi-stage `Dockerfile`, located in the root directory of the repository.

The published Docker image can be pulled and executed using:

```bash
docker run -d -p 8080:8080 --name raceday-container raceday7st10485655.azurecr.io/raceday-app:v1.0.0
Once the container is running, the application can be accessed at:

http://localhost:8080

Docker and CI/CD: CloudLabs Environment Workaround
Challenge
The CloudLabs virtual machine provided for this assignment operates under restricted security policies and does not provide administrator privileges. Consequently, installing and configuring Docker Desktop locally was not possible because the installation requires administrator access to enable components such as WSL 2 and the Virtual Machine Platform.

Additionally, the Azure for Students subscription presented restrictions when attempting to use direct Azure Container Registry build tasks through az acr build.

Implementation and Solution
To fulfil the containerisation requirement, a cloud-based CI/CD workflow was implemented using GitHub Actions and Azure Container Registry.

The workflow operates as follows:

A GitHub Actions workflow was configured in .github/workflows/docker-build.yml.

When changes are pushed to the main branch, GitHub Actions starts an Ubuntu-based runner.

The runner builds the application using the project's Dockerfile.

Once the build succeeds, the resulting Docker image is pushed to Azure Container Registry.

This approach demonstrates that the application can be successfully containerised and published to a cloud container registry without requiring local administrator privileges.

CI/CD Evidence
Successful Docker build and push workflow:

https://docs/ci-cd-green-check.png

GitHub Actions workflow checks:

https://docs/allchecks-part3.png

Azure Blob Storage Integration
Azure Blob Storage is integrated into the application to support event banner image uploads.

Organisers can upload event images, which are stored in an Azure Storage container. The corresponding image URLs are saved in the SQL Server database and used to display the uploaded banners on the Event Details page.

This integration provides cloud-based image storage rather than requiring uploaded images to be stored directly within the application.

Azure Blob Storage Evidence
https://docs/blob1.png

https://docs/blob2.png

https://docs/blob3.png

Video Demonstration
The following YouTube video demonstrates the RaceDay7 application, including its MVC structure, Azure Blob Storage integration and Docker configuration.

Watch the RaceDay7 Video Demonstration

Supporting Documentation
Supporting project documentation is available in the docs/ directory.

The documentation includes:

Entity Relationship Diagram (ERD): race_day_erd (1).pdf

SQL Database Script: script.sql-raceday7.sql

CI/CD Evidence: Screenshots of successful GitHub Actions workflows.

Azure Blob Storage Evidence: Screenshots demonstrating cloud image storage.

Repository Structure
text
RaceDay7/
│
├── docs/
│   ├── allchecks-part3.png
│   ├── blob1.png
│   ├── blob2.png
│   ├── blob3.png
│   ├── ci-cd-green-check.png
│   ├── race_day_erd (1).pdf
│   └── script.sql-raceday7.sql
│
├── .github/
│   └── workflows/
│       ├── docker-build.yml
│       └── dotnet-build.yml
│
├── Controllers/
├── Models/
├── Views/
├── Services/
├── Dockerfile
├── appsettings.json
└── ...
Repository and Version Control Note
During the final stages of submission preparation, some project files were uploaded directly through the GitHub web interface to ensure that the latest source code and supporting resources were available in the remote repository.

The repository contains the submitted application source code, configuration files, supporting documentation and CI/CD workflow files. The video demonstration provides additional evidence of the application's functionality and implementation.

Project: RaceDay7
Portfolio of Evidence: Part 3
Student Number: ST10485655
