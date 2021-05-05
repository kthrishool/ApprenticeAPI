--[PostDeployment.sql|2.0]
DECLARE	@InformationalHeaderString_postdeploy_start		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('blue')


RAISERROR('%s----------------------------PostDeployment script starting...----------------------------',10,1,@InformationalHeaderString_postdeploy_start) WITH NOWAIT;

DECLARE @Environment	varchar(50) = [DeploymentAdmin].ChangeControl.GetEnvironment();

SET @Environment = 
					CASE 
						WHEN @Environment LIKE '%DEV%' THEN 'DEV'
						WHEN @Environment LIKE '%TEST%' THEN 'TEST'
						ELSE @Environment 
					END

RAISERROR('Environment %s detected',10,1,@Environment) WITH NOWAIT;

:r .\Misc\PostDeploymentTasks_Start.sql

:r .\Misc\DatabaseConfigurations.sql

:r .\Security\AuditSecurity.sql

:r .\Security\DeleteSecurity.sql

:r .\Security\CreateSecurity.sql

:r .\Misc\CLRConfiguration.sql

:r .\CDC\cdc_postdeployment.sql

:r .\Data\DataPopulation.sql

:r .\Misc\DropUnusedObjects.sql

:r .\Misc\ApplyVersionNumber.sql 

:r .\Misc\PostDeployment_Release_Validation.sql

:r .\Misc\PostDeploymentTasks_End.sql

:r .\Misc\DropSnapshot.sql

:r .\Misc\SetDatabaseMultiUser.sql

DECLARE	@InformationalHeaderString_postdeploy_end		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('blue')
		,@PostDeploy_Dtm								VARCHAR(50)		= CONVERT(VARCHAR(50),GETDATE(),121)
		,@PostDeploy_ServerName							VARCHAR(250)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(250))
		,@PostDeploy_DatabaseName						SYSNAME			= DB_NAME()
		,@PostDeploy_AuditMessage						VARCHAR(MAX)	= 'deployment completed'
		,@PostDeploy_ScriptName							VARCHAR(50)		= 'PostDeployment.sql';

EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
	@DatabaseName		= @PostDeploy_DatabaseName
	,@ServerName		= @PostDeploy_ServerName
	,@AuditMessage		= @PostDeploy_AuditMessage 
	,@ScriptName		= @PostDeploy_ScriptName
	,@AuditType			= 'Completed';

RAISERROR('End Time %s',10,1,@PostDeploy_Dtm) WITH NOWAIT;
RAISERROR('%s----------------------------PostDeployment script complete----------------------------',10,1,@InformationalHeaderString_postdeploy_end) WITH NOWAIT;
--[PostDeployment.sql|2.0]