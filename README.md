# LocalDbSpike
> Create localdb databse by mounting a localdb file, use it


## Excellent ABC
https://blogs.msdn.microsoft.com/sqlexpress/2011/07/12/introducing-localdb-an-improved-sql-express/

## Version

Regarding localdb based Unit testing, using mdf mount/unmount, we may have version compatibility issue.

So here I share my experimentations with you.

 Case that won’t work:

Generate from data definition sql statements mdf against.\SQLEXPRESS, it will generate a DB with internal database version 869.
And mount the file to (localdb)\v11.0 (internal database version 706) wil fail:

System.Data.SqlClient.SqlException : The database 'Caraibes-local' cannot be opened because it is version 869. This server supports version 706 and earlier. A downgrade path is not supported.
Could not open new database 'Caraibes-local'. CREATE DATABASE is aborted.


So if we want to use localdb for unit tests using file mount/unmount solution
We should guarantee that generated mdf/ldf have database internal version <= target sql server’s supporting database internal version

It seems that (LocalDB)\MSSQLLocalDB is version agnostics.
Amount/ Unmount on (LocalDB)\MSSQLLocalDB will work anyway. I would suggest using this server for unit testing

Hereunder sql server version, internal database version mapping (http://sqlserverbuilds.blogspot.com/2014/01/sql-server-internal-database-versions.html
)

SQL Server Version	Internal Database Version	Database Compatibility Level
SQL Server 2019 CTP
895	150
SQL Server 2017 OR last .\SQLEXPRESS	869	140
SQL Server 2016
852	130
SQL Server 2014
782	120
SQL Server 2012 OR (localdb)\v11.0	706	110

```sql
SELECT @@version
--v11.0 ===> Microsoft SQL Server 2012 - 11.0.2318.0 (X64)   Apr 19 2012 11:53:44   Copyright (c) Microsoft Corporation  Express Edition (64-bit) on Windows NT 6.2 <X64> (Build 9200: ) 
```


```sql
SELECT @@version
--(LocalDB)\MSSQLLocalDB ====>  Microsoft SQL Server 2017 (RTM) - 14.0.1000.169 (X64)   Aug 22 2017 17:04:49   Copyright (C) 2017 Microsoft Corporation  Express Edition (64-bit) on Windows 10 Enterprise 10.0 <X64> (Build 16299: ) 
```

```sql
SELECT @@version
--.\SQLEXPRESS ===> Microsoft SQL Server 2017 (RTM) - 14.0.1000.169 (X64)   Aug 22 2017 17:04:49   Copyright (C) 2017 Microsoft Corporation  Express Edition (64-bit) on Windows 10 Enterprise 10.0 <X64> (Build 16299: )
```