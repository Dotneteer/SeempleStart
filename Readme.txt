Installation and configuration

Use Visual Studio 2015 with Run As Admin

1. Install SQL Server 2014 Express (with LocalDB)
2. In Solution Explorer, select the solution node, and from the 
   contex menu invoke Open Folder in File Explorer
3. In File Explorer, run DeployDatabaseForUnitTests.cmd
4. Install Firebird 2.5 database engine (http://www.firebirdsql.org/)
5. Install any Firebird admin utility, for example, FlameRobin (http://www.flamerobin.org/)
6. Under Localhost, Create new database:
   - Display name: Seemplest.Test
   - Database path: C:\Temp\SeemplestTest.KSFDB
   - User name: SYSDBA
   - Password: masterkey
   - Charset: UNICODE_FSS
   - Role: <empty>
7. Build | Clean Solution
8. Build | Rebuild Solution
9. Check with Test | Run | All Test

