# LMS Backend API

## Setup Instructions

1. Clone the repository
2. Copy `appsettings.Example.json` to `appsettings.json`
3. Update connection strings and API keys in `appsettings.json`
4. Run `dotnet restore`
5. Run `dotnet run`

## Environment Variables for Production

Set these in Azure App Service Configuration:

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__SecretKey`
- `Cloudinary__CloudName`
- `Cloudinary__ApiKey`
- `Cloudinary__ApiSecret`

## API Documentation

Access Swagger UI at: `/swagger`#   c n n e t - b e  
 