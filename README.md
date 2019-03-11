# OCER_APP
Online Construction Equipment Rental

OCER-APP
Application consists of 4 layers

Application.API -> Restfull Service(ASP.NET Core Web API)

Application.Core -> Core functionalities(Class Library)

Application.Infrastructure -> Infrastructure layer for Data Access,Repositories(Class Library)

Application.Web -> Frontend Project(ASP.NET Core MVC)

The API project should then be run on the Web project

Application use In-Memory Database,Caching Mechanishm used for Equipment List

Used for Generic Repository for data access layer

The user can add new equipment in the system,view the report and download(txt format)
