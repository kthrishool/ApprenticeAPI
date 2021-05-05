--[PreDeployment.sql|2.0]
DECLARE	@InformationalHeaderString_predeploy_start		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('blue')
		,@PreDeploy_Environment							VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@PreDeploy_Server								VARCHAR(250)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(250))
		,@PreDeploy_Dtm									VARCHAR(50)		= CONVERT(VARCHAR(50),GETDATE(),121)
		,@PreDeploy_DatabaseName						SYSNAME			= DB_NAME()
		,@PreDeploy_AuditMessage						VARCHAR(MAX)	= 'deployment started'
		,@PreDeploy_ScriptName							VARCHAR(50)		= 'PreDeployment.sql';


RAISERROR('%s----------------------------PreDeployment tasks starting...----------------------------',10,1,@InformationalHeaderString_predeploy_start) WITH NOWAIT;
RAISERROR('Start Time %s',10,1,@PreDeploy_Dtm) WITH NOWAIT;
RAISERROR('Environment %s detected...',10,1,@PreDeploy_Environment) WITH NOWAIT;
RAISERROR('SQL Instance %s detected...',10,1,@PreDeploy_Server) WITH NOWAIT;

EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
	@DatabaseName		= @PreDeploy_DatabaseName
	,@ServerName		= @PreDeploy_Server
	,@AuditMessage		= @PreDeploy_AuditMessage 
	,@ScriptName		= @PreDeploy_ScriptName
	,@AuditType			= 'Started';


:r .\DeploymentConfigurations\Seed_DeploymentConfigurations.sql

:r .\Misc\SetDatabaseRestrictedUser.sql

:r .\Misc\PreDeploymentTasks_Start.sql

:r .\Misc\PreDeployment_Release_Validation.sql

:r .\Misc\CreateSnapshot.sql  

:r .\CDC\cdc_predeployment.sql   

:r .\Misc\PreDeploymentTasks_End.sql

DECLARE	@InformationalHeaderString_predeploy_end		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('blue')

RAISERROR('%s----------------------------PreDeployment tasks complete----------------------------',10,1,@InformationalHeaderString_predeploy_end) WITH NOWAIT;
--[PreDeployment.sql|2.0]