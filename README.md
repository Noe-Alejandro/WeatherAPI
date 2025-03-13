# Weather API Backend

Este es un proyecto backend en .NET para manejar solicitudes relacionadas con la consulta de clima.

---

## 🔧 Requisitos previos
- .NET 6.0 o superior instalado.
- Visual Studio o Visual Studio Code configurado para desarrollo con .NET.

---

## 📦 Instalación de paquetes NuGet

Desde la consola del administrador de paquetes o utilizando la CLI de .NET, instala los siguientes paquetes en el proyecto `WeatherAPI`:

```bash
    dotnet add WeatherAPI package Microsoft.AspNetCore.Mvc.Versioning
    dotnet add WeatherAPI package Swashbuckle.AspNetCore
```

---

## 📄 Configuración de `appsettings.Development.json`

Configura los orígenes permitidos para CORS en tu archivo `appsettings.Development.json` de esta forma:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173"
    ]
  }
}
```

---

## 🔧 Configuración de `launchSettings.json`

Para asegurarte de que la aplicación se abra en Swagger al iniciar, edita tu archivo `launchSettings.json` en la ruta:  
`WeatherAPI/Properties/launchSettings.json` y asegúrate de que incluya lo siguiente:

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:44349",
      "launchUrl": "swagger",
      "launchBrowser": true
    },
    "https": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:44349",
      "launchUrl": "swagger",
      "launchBrowser": true
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  },
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "https://localhost:44336/",
      "launchUrl": "swagger",
      "sslPort": 44336
    }
  }
}
```

---

## 💡 Iniciar el proyecto

1. Restaurar paquetes NuGet:
```bash
    dotnet restore
```

2. Compilar el proyecto:
```bash
    dotnet build
```

3. Ejecutar el proyecto utilizando el perfil de `IIS Express` (Swagger):
```bash
    dotnet run --launch-profile "IIS Express" --project WeatherAPI
```

---

