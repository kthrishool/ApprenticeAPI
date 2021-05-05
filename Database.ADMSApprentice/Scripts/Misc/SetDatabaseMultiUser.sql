--[SetDatabaseMultiUser.sql|1.1]
RAISERROR('STARTING SetDatabaseMultiUser.sql...',10,1) WITH NOWAIT;

IF EXISTS(
SELECT	TOP 1 1 
FROM	sys.databases 
WHERE	[name]				= '$(DatabaseName)'
AND		user_access_desc	<> 'MULTI_USER'
)
BEGIN
	RAISERROR('Setting database [$(DatabaseName)] to MULTI_USER',10,1) WITH NOWAIT;
	ALTER DATABASE [$(DatabaseName)] SET MULTI_USER WITH ROLLBACK IMMEDIATE;
END
--[SetDatabaseMultiUser.sql|1.1]