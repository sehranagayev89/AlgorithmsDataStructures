# PostgreSQL Remote Database Backup Tool

A simple C# console application to backup remote PostgreSQL databases to your local computer.

## Features

- Backup remote PostgreSQL databases from your local PC
- Interactive mode with prompts for connection details
- Command-line mode for automation and scripting
- Secure password input (hidden from display)
- Automatic timestamped backup files
- Progress feedback during backup process

## Prerequisites

1. **.NET 8.0 or higher** - Download from [.NET official website](https://dotnet.microsoft.com/download)
2. **PostgreSQL Client Tools** - The `pg_dump` utility must be installed and accessible in your system PATH.
   - **Windows**: Install from [PostgreSQL official website](https://www.postgresql.org/download/windows/)
   - **Linux**: `sudo apt-get install postgresql-client` (Ubuntu/Debian) or equivalent
   - **macOS**: `brew install postgresql` or download from PostgreSQL website

## Installation

1. Clone or download this repository
2. Build the solution using the .NET CLI:
   ```
   dotnet build ConsoleApp1.sln --configuration Release
   ```
3. The executable will be in `ConsoleApp1\bin\Release\net8.0\ConsoleApp1.exe` (Windows) or `ConsoleApp1/bin/Release/net8.0/ConsoleApp1` (Linux/macOS)

## Usage

### Interactive Mode

Simply run the executable without arguments:

```bash
ConsoleApp1.exe
```

The application will prompt you for:
- Database host (e.g., `myserver.com` or `192.168.1.100`)
- Database port (default: 5432)
- Database name
- Username
- Password (hidden input)
- Output path for backup file

### Command-Line Mode

For automation or scripting, you can provide parameters directly:

```bash
ConsoleApp1.exe <host> <database> <username> <output_path> [port]
```

**Example:**
```bash
ConsoleApp1.exe myserver.com mydatabase postgres C:\backups\mydb.sql 5432
```

The application will prompt only for the password (for security).

### Output

- If you specify a directory as the output path, the backup file will be named automatically with a timestamp: `<database>_backup_YYYYMMDD_HHMMSS.sql`
- If you specify a full file path, that filename will be used
- Backup files are created in plain SQL format that can be restored using `psql` or other PostgreSQL tools

## Examples

### Backup to specific file:
```bash
ConsoleApp1.exe localhost mydb postgres C:\backups\mydb_backup.sql
```

### Backup to directory (auto-generated filename):
```bash
ConsoleApp1.exe 192.168.1.50 production_db admin C:\backups\
```

### Remote server with custom port:
```bash
ConsoleApp1.exe remote.example.com mydb dbuser C:\backups\ 5433
```

## Restoring a Backup

To restore a backup, use the `psql` command:

```bash
psql -h <host> -p <port> -U <username> -d <database> -f <backup_file.sql>
```

**Example:**
```bash
psql -h localhost -U postgres -d mydatabase -f C:\backups\mydb_backup.sql
```

## Troubleshooting

### "pg_dump utility not found"

This error means PostgreSQL client tools are not installed or not in your PATH. 

**Solution:**
1. Install PostgreSQL client tools
2. Add the PostgreSQL bin directory to your system PATH
   - Windows: Add `C:\Program Files\PostgreSQL\<version>\bin` to PATH
   - Linux/macOS: Usually added automatically during installation

### Connection errors

- Verify the database server is accessible from your computer
- Check firewall settings
- Verify PostgreSQL is configured to accept remote connections (`postgresql.conf` and `pg_hba.conf`)
- Confirm username and password are correct

### Permission denied

Ensure your database user has sufficient privileges to perform backups (typically requires SELECT privilege on all tables).

## Security Notes

- Passwords are never displayed on screen
- Passwords are passed to `pg_dump` via environment variable (not command line)
- Always keep backup files secure as they contain your database data
- Consider encrypting backup files for sensitive data

## License

This project is open source and available for personal and commercial use.
