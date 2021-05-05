--[DatabaseConfigurations.sql|2.0]
RAISERROR('STARTING DatabaseConfigurations.sql...',10,1) WITH NOWAIT;

DECLARE @DefaultPathData							nvarchar(256)	= CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(256))
		,@DatabaseConfigurations_sql				nvarchar(max)
		,@DatabaseConfigurations_DatabaseName		varchar(255)	= DB_NAME()
		,@DatabaseConfigurations_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@DatabaseConfigurations_AuditMessage		VARCHAR(MAX)
		,@DatabaseConfigurations_ScriptName			VARCHAR(50)		= 'DatabaseConfigurations.sql';



DECLARE @DatabaseConfigurations_cdc_filename		nvarchar(256) = @DefaultPathData + [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration('$(DatabaseName)','DbDataFileNameCDC')
BEGIN TRY
	IF NOT EXISTS
	(
	SELECT	1
	FROM	sys.databases									d
	JOIN	sys.server_principals							p
	ON		p.sid					= d.owner_sid
	AND		p.name					= N'sa'
	WHERE	d.name					= N'$(DatabaseName)'
	)
		ALTER AUTHORIZATION ON DATABASE::[$(DatabaseName)] TO sa;  
	
	IF NOT EXISTS(SELECT TOP 1 1 FROM sys.filegroups WHERE name = N'CDC')
		ALTER DATABASE [$(DatabaseName)] ADD FILEGROUP [CDC];
	
	SELECT @DatabaseConfigurations_sql = N'ALTER DATABASE [$(DatabaseName)] ADD FILE ( NAME = N''CDC'', FILENAME = ''' + @DatabaseConfigurations_cdc_filename + ''', SIZE = 100MB , FILEGROWTH = 10MB ) TO FILEGROUP [CDC];'
	
	IF NOT EXISTS(SELECT TOP 1 1 FROM sys.database_files d inner join sys.filegroups f ON d.data_space_id = f.data_space_id WHERE f.name = 'CDC')
		EXEC sp_executesql @DatabaseConfigurations_sql
	
	
	IF (1 = FULLTEXTSERVICEPROPERTY(N'IsFullTextInstalled'))
		EXEC [$(DatabaseName)].[dbo].[sp_fulltext_database] @action = 'enable'
	
	
	IF NOT EXISTS( SELECT TOP 1 1 FROM sys.messages WHERE message_id = 50005 )
		EXEC sp_addmessage 
			@msgnum		= 50005
			,@severity	= 16
			,@msgtext	= N'Parameter %s is invalid or undefined.'
			,@lang		= 'us_english';
	
END TRY
BEGIN CATCH
	SELECT @DatabaseConfigurations_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @DatabaseConfigurations_DatabaseName
		,@ServerName		= @DatabaseConfigurations_ServerName
		,@AuditMessage		= @DatabaseConfigurations_AuditMessage 
		,@ScriptName		= @DatabaseConfigurations_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @DatabaseConfigurations_DatabaseName
		,@ScriptName	= @DatabaseConfigurations_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
--[DatabaseConfigurations.sql|2.0]