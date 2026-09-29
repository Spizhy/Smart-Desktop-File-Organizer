# Smart Desktop File Organizer

## Overview
Smart Desktop File Organizer is a background service written in C# (.NET 8) that automates the management of cluttered directories. By constantly monitoring specified folders (like the Downloads folder), it automatically categorizes incoming files, archives outdated documents, and cleans up temporary files based on user-defined rules.

## Key Features
- **Real-Time Monitoring:** Utilizes `FileSystemWatcher` to detect new files instantly.
- **Rule-Based Sorting:** Moves files into specific directories (e.g., `.pdf` to Documents, `.exe` to Installers).
- **Auto-Archiving:** Compresses files that haven't been accessed in over 30 days into ZIP archives.
- **Logging:** Maintains a detailed log of all file operations for safety and rollback purposes.
- **Background Execution:** Runs seamlessly as a Windows Service without interrupting user workflow.

## Tech Stack
- **Language:** C#
- **Framework:** .NET 8 (Worker Service)
- **Testing:** xUnit, Moq

## Installation & Setup
1. Clone the repository.
2. Open `SmartFileOrganizer.sln` in Visual Studio or JetBrains Rider.
3. Modify the monitoring rules in `src/SmartFileOrganizer.Worker/appsettings.json`.
4. Build the solution and register the worker as a Windows Service using the standard `sc create` command or run it as a standalone executable.
