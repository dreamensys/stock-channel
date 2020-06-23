# Stock Channel Chat

This is a web-based chat built with SignalR and RabbitMQ.

### Instructions to run the app
1. Open the Package Manager Console from Visual Studio 2019 or higher and then run `EntityFrameworkCore\Update-Database`

2. In order to let the app be able to comunicate with the RabbitMQ instance, you can chose one of these 2 options:
  - Install and run RabbitMQ client for Windows (https://www.rabbitmq.com/install-windows.html)
  - Download and install Docker Desktop (https://hub.docker.com/editions/community/docker-ce-desktop-windows) and then run:
    `docker-compse up` from terminal at the same level as .sln file.
    
3. Start debuging the application, Stock Bot API as well as Chat will start.

**Notes**
- The application uses a Local MSSQL Database.


### Libraries & Patterns
- DDD
- Dependecy Injection
- Clean Architecture
- Repository Pattern
- Newtonsoft
- SignalR
- RabbitMQ
- CsvHelper
