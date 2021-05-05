--[ChangeControl-DeploymentTask_SeedData.sql|2.0]
RAISERROR('STARTING ChangeControl-DeploymentTask_SeedData.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @DeploymentTask_SeedData_Environment		VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@DeploymentTask_SeedData_DatabaseName		VARCHAR(255)	= DB_NAME() --TFS executes this script directly, so cant use SQLCMD variable $(DatabaseName)
		,@DeploymentTask_SeedData_NewVersionNumber	VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@DeploymentTask_SeedData_ServerName		NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@DeploymentTask_SeedData_AuditMessage		VARCHAR(MAX)
		,@DeploymentTask_SeedData_ScriptName		VARCHAR(50)		= 'ChangeControl-DeploymentTask_SeedData.sql';


SELECT	@DeploymentTask_SeedData_Environment = 
								CASE 
									WHEN @DeploymentTask_SeedData_Environment LIKE '%DEV%' THEN 'DEV'
									WHEN @DeploymentTask_SeedData_Environment LIKE '%TEST%' THEN 'TEST'
									ELSE @DeploymentTask_SeedData_Environment 
								END

IF OBJECT_ID('tempdb..#DeploymentTask') IS NOT NULL
	DROP TABLE #DeploymentTask;

CREATE TABLE #DeploymentTask
(
	[DatabaseName]				NVARCHAR(128)	NOT NULL
	,[Reference]				VARCHAR(50)		NOT NULL
	,[TaskName]					VARCHAR(128)	NOT NULL
	,[TaskSQL]					VARCHAR(MAX)	NULL
	,[TargetDatabaseName]		SYSNAME			NOT NULL
	,[DeploymentSequence]		VARCHAR(50)		NOT NULL
	,[DeploymentTaskSequence]	INT				NOT NULL
	,[Disabled]					BIT				NULL
	,[AbortOnFailure]			BIT				NULL
);


;WITH new_values_all_environments 
(	
		NewVersionNumber			--1.0,1.1,2.0...
		,Environment				--'ALL','PROD','TEST'...
		,DeploymentSequence			--'PRE_PREDEPLOYMENT_START','PREDEPLOYMENT_START','PREDEPLOYMENT_END','POSTDEPLOYMENT_START','POSTDEPLOYMENT_END'
		,DeploymentTaskSequence		--1,2,3...	
		,TaskName	
		,TargetDatabaseName	
		,[Disabled]					--0,1
		,[AbortOnFailure]			--0,1
		,TaskSQL	
)
AS
(
SELECT	NULL	--NewVersionNumber
		,NULL	--Environment		
		,NULL	--DeploymentSequence		
		,NULL	--DeploymentTaskSequence	
		,NULL	--TaskName	
		,NULL	--TargetDatabaseName	
		,NULL	--[Disabled]
		,NULL	--[AbortOnFailure]
		,NULL	--TaskSQL	
)
,new_values AS
(
SELECT	@DeploymentTask_SeedData_DatabaseName			AS DatabaseName
		,@DeploymentTask_SeedData_NewVersionNumber		AS Reference
		,DeploymentSequence			
		,DeploymentTaskSequence		
		,TaskName	
		,TargetDatabaseName	
		,[Disabled]	
		,[AbortOnFailure]
		,TaskSQL	
FROM	new_values_all_environments
WHERE	Environment IN ( @DeploymentTask_SeedData_Environment, 'ALL' )
AND		NewVersionNumber = @DeploymentTask_SeedData_NewVersionNumber
)
INSERT INTO #DeploymentTask
(
		DatabaseName				
		,Reference				
		,TaskName					
		,TaskSQL					
		,TargetDatabaseName		
		,DeploymentSequence		
		,DeploymentTaskSequence	
		,[Disabled]
		,[AbortOnFailure]
)
SELECT	DatabaseName				
		,Reference				
		,TaskName					
		,TaskSQL					
		,TargetDatabaseName		
		,DeploymentSequence		
		,DeploymentTaskSequence	
		,[Disabled]
		,[AbortOnFailure]
FROM	new_values


BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[UpsertDeploymentTask]
		@DatabaseName	= @DeploymentTask_SeedData_DatabaseName
		,@Reference		= @DeploymentTask_SeedData_NewVersionNumber;		
END TRY
BEGIN CATCH
	SELECT @DeploymentTask_SeedData_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @DeploymentTask_SeedData_DatabaseName
		,@ServerName		= @DeploymentTask_SeedData_ServerName
		,@AuditMessage		= @DeploymentTask_SeedData_AuditMessage 
		,@ScriptName		= @DeploymentTask_SeedData_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @DeploymentTask_SeedData_DatabaseName
		,@ScriptName	= @DeploymentTask_SeedData_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
--[ChangeControl-DeploymentTask_SeedData.sql|2.0]