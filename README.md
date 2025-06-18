# LoggerAnalyzer2

LoggerAnalyzer2 is an ASP.NET Core application for analyzing and displaying logs with Source Generator support.

## Project Structure

- **LoggerAnalyzer2** — The main ASP.NET web application implementing the user interface for working with logs.
- **SourceGeneration** — A separate Source Generator project that scans for logging method calls in code and generates log usage information in JSON format.

## Key Features

- **Source Generator** automatically detects logger method calls and generates a `generated_log_info.json` file with logger usage information.
- The main application reads this JSON file directly, using local classes for deserialization (`LogUsingInfo`, `LogUsingInfoList`).
- Case-insensitive JSON deserialization is supported for compatibility.

### User Interface
- Fixed sidebar for search (230px wide), with enlarged and centered controls and styled checkboxes.
- Bottom status bar displaying log counters in a table format.
- Multiple filters in the sidebar can be expanded simultaneously.

## Quick Start

1. **Build SourceGeneration**
   - Navigate to the SourceGeneration folder and build the project:
     ```sh
     dotnet build
     ```
   - After the first run or build, a `generated_log_info.json` file will appear in the LoggerAnalyzer2 root directory.

2. **Run LoggerAnalyzer2**
   - Navigate to the LoggerAnalyzer2 folder and start the application:
     ```sh
     dotnet run
     ```
   - Open your browser and go to the address shown in the console (usually http://localhost:5000 or http://localhost:5001).

## Important Changes

- The view file `LogTable.temp.cshtml` was removed; all logic is now in `LogTable.cshtml`.
- Filters are implemented via server handlers, and multiple filters can be opened at once.
- The status bar uses a table to display log counters.
- The application now works directly with the JSON file instead of the generated class (to avoid Source Generator dependency issues).

## Requirements
- .NET 8.0 SDK
- Modern browser (Chrome, Edge, Firefox, etc.)

_Last updated: June 18, 2025_
