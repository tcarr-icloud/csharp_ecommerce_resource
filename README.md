E-commerce Resource Service (C#/.NET)

Overview
- This repository contains a minimal ASP.NET Core Web API that exposes basic e‑commerce resources: Accounts, Carts, and Orders.
- It also demonstrates producing and consuming Kafka messages and persisting simple event-style records to DynamoDB (via the AWS SDK).
- OpenAPI is enabled to generate the API contract in development.

Tech stack
- Language: C# 14.0
- Runtime: .NET 10.0 (TargetFramework: net10.0)
- Web framework: ASP.NET Core (Minimal hosting model, MVC controllers)
- Package manager: NuGet (via dotnet CLI)
- Key dependencies:
  - Microsoft.AspNetCore.OpenApi (OpenAPI document generation)
  - Confluent.Kafka (Kafka producer and consumer)
  - AWSSDK.DynamoDBv2 (DynamoDB client)

Project structure
- csharp_ecommerce_resource.sln – Solution file
- csharp_ecommerce_resource/ – Web API project
  - Accounts/ – Account DTOs, messages, controller, and service interface/impl
  - Carts/ – Cart DTOs, messages, controller, and service interface/impl
  - Orders/ – Order DTOs, messages, controller, and service interface/impl
  - Models/ – Shared model types
  - Services/ – Kafka producer, Kafka background consumer, DynamoDB service, processing interfaces
  - Program.cs – Application entry point and DI configuration
  - appsettings.json, appsettings.Development.json – Configuration
  - Properties/launchSettings.json – Development profiles

Entry point
- Program.cs uses the ASP.NET Core minimal hosting model and configures Controllers + OpenAPI.
- In Development, the OpenAPI document is mapped at /openapi/v1.json (no UI by default).

HTTP API endpoints (high level)
- Accounts: /api/Account
  - POST /api/Account – Create an account
  - GET /api/Account/{id} – Get account by id
  - GET /api/Account – List accounts
  - PUT /api/Account/{id} – Update account
  - DELETE /api/Account/{id} – Delete account
- Carts: /api/Cart
  - POST /api/Cart – Create a cart
  - GET /api/Cart/{id} – Get cart by id
  - GET /api/Cart – List carts
  - PUT /api/Cart/{id} – Update cart
  - DELETE /api/Cart/{id} – Delete cart
- Orders: /api/Order
  - POST /api/Order – Create an order
  - GET /api/Order/{id} – Get order by id
  - GET /api/Order – List orders
  - PUT /api/Order/{id} – Update order
  - DELETE /api/Order/{id} – Delete order

Background processing
- A hosted background service (ConsumerService<T>) subscribes to a Kafka topic and dispatches messages to an injected IProcessor<T> implementation.
- Program.cs currently wires a ConsumerService for AccountMessage with:
  - Topic: accounts
  - BootstrapServers: localhost:29092
  - GroupId: accounts-consumer-group

Requirements
- .NET SDK 10.0 (or a compatible SDK capable of targeting net10.0)
  - Verify your installed SDK: dotnet --info
- Kafka broker reachable at localhost:29092 (default in code)
- DynamoDB Local (or a real DynamoDB endpoint)
  - Default configured endpoint in code: http://localhost:8000

Setup
1) Install the .NET SDK
- https://dotnet.microsoft.com/download

2) Prepare external services
- Kafka (example using Docker; adjust to your environment):
  - Example (Bitnami image):
    - docker run -d --name kafka -p 29092:29092 -e KAFKA_ENABLE_KRAFT=yes -e KAFKA_CFG_LISTENERS=PLAINTEXT://:9092,CONTROLLER://:9093,EXTERNAL://:29092 -e KAFKA_CFG_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092,EXTERNAL://localhost:29092 -e KAFKA_CFG_LISTENER_SECURITY_PROTOCOL_MAP=PLAINTEXT:PLAINTEXT,CONTROLLER:PLAINTEXT,EXTERNAL:PLAINTEXT -e KAFKA_BROKER_ID=1 -e KAFKA_CFG_CONTROLLER_QUORUM_VOTERS=1@localhost:9093 -e KAFKA_CFG_CONTROLLER_LISTENER_NAMES=CONTROLLER bitnami/kafka:latest
  - TODO: Provide a docker-compose.yml tailored to this project.
- DynamoDB Local (example using Docker):
  - docker run -d --name dynamodb -p 8000:8000 amazon/dynamodb-local
  - Note: The code uses BasicAWSCredentials with dummy values and points to the local endpoint.

Configuration
- appsettings.json contains basic logging and host settings.
- appsettings.Development.json includes a Kafka section with default local settings (BootstrapServers, Topic, GroupId). It is valid JSON and can be safely loaded at runtime.
- Important: Several configuration values are hard-coded in code at the moment:
  - Kafka producer bootstrap servers: Services/KafkaProducerService.cs → localhost:29092
  - Kafka consumer config (topic, bootstrap servers, group id): Program.cs
  - DynamoDB endpoint and credentials: Services/IDynamodbService.cs (DynamoDbService)
- TODO: Externalize these values to configuration (appsettings + environment variables) and bind via Options.

Environment variables
- Common ASP.NET Core variables you may use:
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
- Future improvements (not yet wired in code):
  - KAFKA__BOOTSTRAP_SERVERS
  - KAFKA__TOPIC
  - KAFKA__GROUP_ID
  - DYNAMODB__SERVICE_URL
  - AWS__ACCESS_KEY_ID / AWS__SECRET_ACCESS_KEY (only when not using local)
  - TODO: Implement configuration binding for these.

Restore, build, and run
- From the repository root:
- Restore packages:
  - dotnet restore
- Build:
  - dotnet build -c Debug
- Run the API:
  - dotnet run --project csharp_ecommerce_resource
- The API will start with HTTPS redirection enabled. In Development, the OpenAPI document should be available at:
  - https://localhost:PORT/openapi/v1.json (port depends on your profile)

Local testing
- You can issue HTTP requests using the provided http files:
  - csharp_ecommerce_resource/http-client.http
  - csharp_ecommerce_resource/csharp_ecommerce_resource.http
- Many IDEs (e.g., JetBrains Rider, Visual Studio Code Rest Client) can execute these directly.

Unit and integration tests
- No test projects were found in this solution.
- TODO: Add unit tests for controllers and services (Kafka producer, consumer processor, DynamoDB service).
- Once tests exist, run them with:
  - dotnet test

Notes on data persistence
- DynamoDbService uses a local DynamoDB endpoint (http://localhost:8000) and dummy BasicAWSCredentials for local development.
- Table names (hard-coded): accounts, carts, orders. Ensure these tables exist in your local DynamoDB before calling endpoints that persist data.
- Example to create a table in DynamoDB Local (using AWS CLI v2):
  - aws dynamodb create-table --table-name accounts --attribute-definitions AttributeName=Id,AttributeType=S --key-schema AttributeName=Id,KeyType=HASH --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 --endpoint-url http://localhost:8000
  - Repeat similarly for carts and orders.

OpenAPI
- In Development, the OpenAPI document is exposed via app.MapOpenApi(). This provides the JSON document at /openapi/v1.json. An interactive Swagger UI is not configured.
- TODO: Add Swagger UI if desired (e.g., Swashbuckle.AspNetCore) or use the new ASP.NET Core 10 Minimal OpenAPI UI when available.

Known limitations and TODOs
- Configuration is partially hard-coded in code; switch to IOptions-bound settings.
- Add Docker Compose for Kafka + DynamoDB Local to simplify local setup.
- Add test projects and CI workflows.
- Validate and document all DTO schemas in the README or OpenAPI description.
- Choose and add a LICENSE file.

License
- TODO: Select an open-source license (e.g., MIT, Apache-2.0) and add a LICENSE file at the repo root.
