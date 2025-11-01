# Usage Examples

This document provides practical examples of using the PostgreSQL Remote Database Backup Tool.

## Example 1: Interactive Mode (Recommended for First-Time Users)

Simply run the application without any arguments:

```bash
dotnet run --project ConsoleApp1/ConsoleApp1.csproj
```

Or if using the compiled executable:

```bash
./ConsoleApp1/bin/Release/net8.0/ConsoleApp1
```

The application will prompt you for:
1. Database host (e.g., `db.example.com` or `192.168.1.100`)
2. Database port (press Enter for default 5432)
3. Database name (e.g., `myapp_production`)
4. Username (e.g., `postgres` or `dbadmin`)
5. Password (input will be hidden with asterisks)
6. Output path (e.g., `C:\backups\` or `/home/user/backups/`)

### Example Interactive Session:

```
=== PostgreSQL Remote Database Backup Tool ===

This tool helps you backup a remote PostgreSQL database to your local computer.

Enter database host (e.g., localhost or remote.server.com): db.example.com
Enter database port (default: 5432): 
Enter database name: myapp_db
Enter username: postgres
Enter password: ********
Enter output path for backup file (e.g., C:\backups\mydb.sql): C:\backups\

Starting backup of database 'myapp_db' from db.example.com:5432
Backup will be saved to: C:\backups\myapp_db_backup_20241101_140530.sql
Executing pg_dump...
pg_dump: last built-in OID is 16383
pg_dump: reading extensions
pg_dump: identifying extension members
...
Backup completed successfully!
Backup file saved to: C:\backups\myapp_db_backup_20241101_140530.sql
Backup file size: 1234.56 KB

✓ Backup completed successfully!

Press any key to exit...
```

## Example 2: Command-Line Mode (For Automation)

Provide connection parameters as command-line arguments:

```bash
dotnet run --project ConsoleApp1/ConsoleApp1.csproj -- db.example.com myapp_db postgres C:\backups\ 5432
```

The tool will only prompt for the password (for security reasons).

### Linux/macOS Example:

```bash
./ConsoleApp1/bin/Release/net8.0/ConsoleApp1 db.example.com myapp_db postgres /home/user/backups/ 5432
```

### Windows Example:

```cmd
ConsoleApp1\bin\Release\net8.0\ConsoleApp1.exe db.example.com myapp_db postgres C:\backups\ 5432
```

## Example 3: Backing Up to a Specific File

To save the backup with a specific filename:

```bash
dotnet run --project ConsoleApp1/ConsoleApp1.csproj -- localhost mydb postgres C:\backups\mydb_before_migration.sql
```

## Example 4: Localhost Backup

To backup a database running on your local machine:

```bash
dotnet run --project ConsoleApp1/ConsoleApp1.csproj -- localhost testdb postgres C:\backups\
```

## Example 5: Custom Port

If your PostgreSQL server runs on a non-standard port:

```bash
dotnet run --project ConsoleApp1/ConsoleApp1.csproj -- db.example.com mydb postgres C:\backups\ 5433
```

## Restoring a Backup

Once you have a backup file, you can restore it using `psql`:

### Restore to New Database:

```bash
# Create the database first
psql -h localhost -U postgres -c "CREATE DATABASE mydb_restored;"

# Restore the backup
psql -h localhost -U postgres -d mydb_restored -f C:\backups\myapp_db_backup_20241101_140530.sql
```

### Restore to Existing Database (WARNING: This will overwrite existing data):

```bash
psql -h localhost -U postgres -d mydb -f C:\backups\myapp_db_backup_20241101_140530.sql
```

## Common Use Cases

### 1. Daily Automated Backups (Windows Task Scheduler)

Create a batch file `backup_database.bat`:

```batch
@echo off
cd C:\path\to\project
echo mypassword | ConsoleApp1\bin\Release\net8.0\ConsoleApp1.exe db.example.com mydb postgres C:\backups\ 5432
```

Schedule it in Windows Task Scheduler to run daily.

### 2. Daily Automated Backups (Linux Cron)

Create a shell script `backup_database.sh`:

```bash
#!/bin/bash
cd /path/to/project
echo "mypassword" | ./ConsoleApp1/bin/Release/net8.0/ConsoleApp1 db.example.com mydb postgres /home/user/backups/ 5432
```

Make it executable and add to crontab:
```bash
chmod +x backup_database.sh
crontab -e
# Add line: 0 2 * * * /path/to/backup_database.sh
```

### 3. Pre-Deployment Backup

Before deploying changes, create a backup:

```bash
dotnet run --project ConsoleApp1/ConsoleApp1.csproj -- production.db.example.com myapp_prod dbadmin C:\backups\pre_deploy_backup.sql
```

### 4. Development Database Refresh

Backup production and restore to development:

```bash
# Backup production
dotnet run --project ConsoleApp1/ConsoleApp1.csproj -- prod.example.com myapp postgres /tmp/prod_backup.sql

# Restore to development
psql -h localhost -U postgres -d myapp_dev -f /tmp/prod_backup.sql
```

## Tips

1. **Test First**: Always test the backup and restore process with a test database before using it in production.

2. **Storage Space**: Ensure you have enough disk space. Database backups can be large.

3. **Backup Rotation**: Implement a backup rotation policy to manage old backups and save disk space.

4. **Compression**: For large databases, compress the backup files:
   ```bash
   gzip C:\backups\mydb_backup.sql
   ```

5. **Security**: Never store passwords in scripts. Use environment variables or password files instead.

6. **Network**: Ensure your local machine can reach the remote database server (check firewalls, security groups, etc.).
