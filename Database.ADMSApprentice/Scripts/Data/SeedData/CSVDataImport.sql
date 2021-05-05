--[CSVDataImport.sql|2.0]
RAISERROR('Starting [DeploymentAdmin].Staging.DataImportProcess...',10,1) WITH NOWAIT;
DECLARE @DataImportProcess_DatabaseName		VARCHAR(255)	= DB_NAME()
		,@DataImportProcess_ServerName		NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@DataImportProcess_AuditMessage	VARCHAR(MAX)
		,@DataImportProcess_ScriptName		VARCHAR(50)		= 'CSVDataImport.sql';

BEGIN TRY
	EXEC [DeploymentAdmin].Staging.DataImportProcess 
		@DatabaseName = @DataImportProcess_DatabaseName;
END TRY
BEGIN CATCH
	SELECT @DataImportProcess_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @DataImportProcess_DatabaseName
		,@ServerName		= @DataImportProcess_ServerName
		,@AuditMessage		= @DataImportProcess_AuditMessage 
		,@ScriptName		= @DataImportProcess_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH;
--[CSVDataImport.sql|2.0]