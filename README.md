# IpWatch

Desktop application for monitoring servers inside VPN or closed networks. This solution provides real-time server monitoring capabilities with email notifications for server status changes.

## Features

- **Server Monitoring**: Track server availability and response times
- **Email Notifications**: Receive alerts when servers go down or come back online
- **Multiple Storage Options**: Support for both local file storage and SQL Server database
- **Windows Service**: Run as a background service for continuous monitoring
- **WPF Desktop App**: User-friendly interface for configuration and management
- **Flexible Configuration**: Support for multiple environments with JSON configuration

## Projects Structure

### Core Libraries
- **WatcherCore** - Core business logic and entity definitions
- **LocalDiskRepo** - File-based repository implementation using JSON storage
- **LocalDbRepo** - SQL Server database repository implementation using Entity Framework
- **Messaging** - Email notification services and templates

### Applications
- **WatcherApp** - WPF desktop application for managing watch configurations
- **WatcherService** - Windows service for continuous server monitoring

### Tests
- **LocalDiskRepo.Tests** - Unit tests for file-based repository
- **LocalDbRepo.Tests** - Unit tests for database repository (requires SQL Server LocalDB)

## Technology Stack

- **.NET 8.0** - Latest LTS version of .NET
- **Entity Framework Core 8.0** - Object-relational mapping for database operations
- **WPF** - Windows Presentation Foundation for desktop UI
- **MSTest** - Unit testing framework
- **Newtonsoft.Json** - JSON serialization for file-based storage
- **Telerik UI for WPF** - Advanced UI controls (external dependency)

## Requirements

- **Windows OS** - Required for WPF application and Windows Service
- **.NET 8.0 Runtime** - Download from [Microsoft .NET downloads](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server LocalDB** (optional) - For database storage option
- **Telerik UI for WPF** (optional) - For enhanced UI components in WatcherApp

## Getting Started

### Building the Solution

```bash
# Clone the repository
git clone https://github.com/kirakosyan/IpWatch.git
cd IpWatch

# Restore packages and build
dotnet restore
dotnet build
```

### Running Tests

```bash
# Run file-based repository tests
dotnet test LocalDiskRepo.Tests

# Run database tests (requires SQL Server LocalDB on Windows)
dotnet test LocalDbRepo.Tests
```

### Configuration

The application supports multiple configuration files:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development environment settings
- `appsettings.Release.json` - Production environment settings

### Running the Service

```bash
# Run the monitoring service
cd WatcherService
dotnet run
```

### Running the Desktop App

```bash
# Run the WPF application (Windows only)
cd WatcherApp
dotnet run
```

## Storage Options

### File-Based Storage (LocalDiskRepo)
- Stores configuration in JSON files
- No external database dependencies
- Suitable for small deployments

### Database Storage (LocalDbRepo)
- Uses SQL Server with Entity Framework Core
- Supports migrations and complex queries
- Suitable for enterprise deployments

## Email Templates

The solution includes HTML email templates for notifications:
- Server status change notifications
- Customizable templates with CSS styling
- Support for rich content and branding

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is open source. Please check the license file for details.

## Support

For issues and questions, please use the GitHub issue tracker.
