# GUtils.Api

A RESTful API project that exposes the ClassCopier functionality through HTTP endpoints using **FastEndpoints**.

## Features

- **FastEndpoints**: High-performance, feature-rich alternative to traditional ASP.NET Core Controllers
- **Scalar UI**: Modern, beautiful interactive API documentation interface
- **Swagger/OpenAPI**: Auto-generated OpenAPI specification
- **Deep Copy Endpoints**: REST endpoints for creating deep copies of objects
- **.NET 9 & C# 13**: Built with the latest .NET framework

## Running the API

### Development Mode

```bash
dotnet run --project GUtils.Api
```

The API will start on:
- HTTPS: `https://localhost:7000`
- HTTP: `http://localhost:5000`

The Scalar UI will automatically open at: `https://localhost:7000/scalar/v1`

### Accessing API Documentation

- **Scalar UI**: `https://localhost:7000/scalar/v1` (recommended)
- **Swagger UI**: `https://localhost:7000/swagger`
- **OpenAPI JSON**: `https://localhost:7000/swagger/v1/swagger.json`

## API Endpoints

### ClassCopier Endpoints

#### POST /api/ClassCopier/deep-copy
Creates a deep copy of a simple object.

**Request Body:**
```json
{
  "id": 1,
  "name": "Example",
  "metadata": {
    "key1": "value1",
    "key2": 42
  }
}
```

**Response:**
```json
{
  "id": 1,
  "name": "Example",
  "metadata": {
    "key1": "value1",
    "key2": 42
  }
}
```

#### POST /api/ClassCopier/deep-copy-complex
Creates a deep copy of a complex object with nested structures.

**Request Body:**
```json
{
  "id": 1,
  "name": "Complex Example",
  "nestedData": {
    "list1": ["item1", "item2"],
    "list2": ["item3", "item4"]
  },
  "items": ["a", "b", "c"],
  "createdAt": "2024-01-01T00:00:00Z"
}
```

#### GET /api/ClassCopier/health
Health check endpoint.

**Response:**
```json
{
  "status": "healthy",
  "service": "ClassCopier API",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## Project Structure

```
GUtils.Api/
??? Endpoints/
?   ??? DeepCopyEndpoint.cs           # Simple deep copy endpoint
?   ??? DeepCopyComplexEndpoint.cs    # Complex deep copy endpoint
?   ??? HealthEndpoint.cs             # Health check endpoint
??? Models/
?   ??? ErrorResponse.cs              # Shared error response model
??? Properties/
?   ??? launchSettings.json           # Launch configuration
??? appsettings.json                  # Application settings
??? appsettings.Development.json      # Development settings
??? GUtils.Api.csproj                 # Project file
??? Program.cs                        # Application entry point
??? requests.http                     # Sample HTTP requests
??? README.md                         # This file
```

## Dependencies

- **FastEndpoints** (v5.32.0): High-performance endpoint framework
- **FastEndpoints.Swagger** (v5.32.0): Swagger/OpenAPI generation for FastEndpoints
- **Scalar.AspNetCore** (v1.2.42): Modern API documentation UI
- **GUtils**: Reference to the main GUtils project containing ClassCopier

## FastEndpoints Benefits

- **Performance**: Significantly faster than traditional controllers
- **Simplified Structure**: One class per endpoint for better organization
- **Built-in Validation**: Integrated FluentValidation support
- **Easy Testing**: Endpoints are easier to unit test
- **Better Separation of Concerns**: Each endpoint is self-contained

## Scalar UI Benefits

- **Beautiful Design**: Modern, clean interface
- **Better UX**: Improved developer experience over traditional Swagger UI
- **Dark Mode**: Built-in dark mode support
- **Enhanced Navigation**: Better organized API exploration
- **Try It Out**: Interactive request testing with syntax highlighting

## Configuration

The API uses standard ASP.NET Core configuration:
- `appsettings.json`: Production settings
- `appsettings.Development.json`: Development-specific settings
- `launchSettings.json`: Launch profiles for development

## Development

### Adding New Endpoints

1. Create a new class in the `Endpoints/` directory
2. Inherit from `Endpoint<TRequest, TResponse>` or `EndpointWithoutRequest<TResponse>`
3. Override `Configure()` to set up routing and metadata
4. Override `HandleAsync()` to implement endpoint logic
5. Add XML documentation comments for OpenAPI documentation

Example:
```csharp
public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    public override void Configure()
    {
        this.Post("/api/my-endpoint");
        this.AllowAnonymous();
        this.Summary(s => s.Summary = "My endpoint description");
    }

    public override async Task HandleAsync(MyRequest req, CancellationToken ct)
    {
        var response = new MyResponse(req.Data);
        await this.SendOkAsync(response, ct);
    }
}
```

### Testing with Scalar UI

1. Run the project
2. Navigate to `https://localhost:7000/scalar/v1` (opens automatically)
3. Use the interactive UI to test endpoints
4. View beautiful, well-organized request/response schemas
5. Try out requests directly in the browser with syntax highlighting

## Notes

- FastEndpoints automatically discovers and registers all endpoints in the assembly
- Scalar UI and Swagger UI are both available in Development mode
- HTTPS redirection is enabled by default
- All endpoints are under the `/api` route prefix
- Error handling uses FastEndpoints' built-in `ThrowError()` method
- Scalar provides a superior developer experience compared to traditional Swagger UI
