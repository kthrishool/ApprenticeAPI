--[ChangeControl-DeploymentConfiguration_SeedData.sql|2.2]
RAISERROR('STARTING ChangeControl-DeploymentConfiguration_SeedData.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @DeploymentConfiguration_SeedData_Environment				VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@DeploymentConfiguration_SeedData_DatabaseName				VARCHAR(255)	= DB_NAME() --TFS executes this script directly, so cant use SQLCMD variable $(DatabaseName)
		,@DeploymentConfiguration_SeedData_NewVersionNumber			VARCHAR(21)
		,@DeploymentConfiguration_SeedData_ServerName				NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@DeploymentConfiguration_SeedData_AuditMessage				VARCHAR(MAX)
		,@DeploymentConfiguration_SeedData_ScriptName				VARCHAR(50)		= 'ChangeControl-DeploymentConfiguration_SeedData.sql';


SELECT	@DeploymentConfiguration_SeedData_Environment = 
								CASE 
									WHEN @DeploymentConfiguration_SeedData_Environment LIKE '%DEV%' THEN 'DEV'
									WHEN @DeploymentConfiguration_SeedData_Environment LIKE '%TEST%' THEN 'TEST'
									ELSE @DeploymentConfiguration_SeedData_Environment 
								END

IF OBJECT_ID('tempdb..#DeploymentConfigurations') IS NOT NULL
	DROP TABLE #DeploymentConfigurations;

CREATE TABLE #DeploymentConfigurations (ConfigurationName VARCHAR (256) NOT NULL, ConfigurationValue VARCHAR (MAX) NULL)


;WITH general_configurations 
			(Environment			,ConfigurationName													,ConfigurationValue) AS
(
		SELECT		'ALL'			,'ExpectedVersionNumber'											, '0.0'
UNION	SELECT		'ALL'			,'NewVersionNumber'													, '1.0'
UNION	SELECT		'ALL'			,'DbDataFileNameCDC'												, DB_NAME() + '_CDC.mdf' 
UNION	SELECT		'ALL'			,'DeloymentStartDtm'												, CONVERT(VARCHAR(100),GETDATE(),121)
--all the administrative overrides supported											  
--UNION	SELECT		'ALL'			,'AdminConfiguration_Disable_DropSnapshot'							,'1' 
--UNION	SELECT		'ALL'			,'AdminConfiguration_Disable_CreateSnapshot'						,'1' 
--UNION	SELECT		'ALL'			,'AdminConfiguration_DisableValidation_DataImportFile_MaxAgeHours'	,'1' 
--UNION	SELECT		'ALL'			,'AdminConfiguration_Enable_SecurityCreateLogins'					,'1' 
--UNION	SELECT		'ALL'			,'AdminConfiguration_Enable_SetDatabaseRestrictedUser'				,'1'
--UNION	SELECT		'ALL'			,'AdminConfiguration_Enable_ReapplyReleaseTasks'					,'1'
UNION	SELECT		'ALL'			,'AdminConfiguration_Disable_DeleteSecurity'						,'1' 
)
,filter_environment AS
(
SELECT DISTINCT e.Element																						AS EnvironmentName
				,v.ConfigurationName
				,v.ConfigurationValue
FROM			general_configurations																			v
CROSS APPLY		[DeploymentAdmin].ChangeControl.UDF_SplitStringToTable_varcharmax(v.Environment,',')			e
WHERE			e.Element	IN ( @DeploymentConfiguration_SeedData_Environment, 'ALL'	)
)
,ranked_configurations AS
(
SELECT			ROW_NUMBER() OVER (PARTITION BY ConfigurationName ORDER BY CASE WHEN EnvironmentName = 'ALL' THEN 0 ELSE 1 END DESC) AS configuration_rank
				,ConfigurationName
				,ConfigurationValue
FROM			filter_environment
)
INSERT INTO		#DeploymentConfigurations (ConfigurationName, ConfigurationValue)
SELECT			ConfigurationName
				,ConfigurationValue
FROM			ranked_configurations 
WHERE			configuration_rank							= 1 --de-dupe any configurations


SELECT @DeploymentConfiguration_SeedData_NewVersionNumber = ConfigurationValue FROM #DeploymentConfigurations WHERE ConfigurationName = 'NewVersionNumber'



------------------------------------------------CDC Configurations------------------------------------------------
--which cdc enabled tables have changes for the next release?  i.e. CDC tables to be stopped 
;WITH cdc_tables_with_changes 
(					tbl_schema				,tbl_name) 
AS
(
		SELECT		NULL					,NULL										
)
--which tables should be enabled for CDC?
,cdc_enabled_tables 
(					tbl_schema				,tbl_name) 
AS
(
		SELECT		NULL					,NULL
)
,cdc_job_configurations 
(					Environment				,ConfigurationName							,ConfigurationValue)
AS
(
		SELECT		'PROD,PREPROD'			,'CDC_Cleanup_retention'					,'10080'	--7 days
UNION	SELECT		'DEV,TEST'				,'CDC_Cleanup_retention'					,'4320'		--3 days

UNION	SELECT		'ALL'					,'CDC_Capture_maxtrans'						,'500'
UNION	SELECT		'ALL'					,'CDC_Capture_maxscans'						,'10'

UNION	SELECT		'ALL'					,'CDC_Role_Name'							,'CDCRole'
UNION	SELECT		'ALL'					,'CDC_Support_Net_Changes'					,'1'		--default behaviour 0/off
UNION	SELECT		'ALL'					,'CDC_Additional_Backup_Columns'			,'1'		--default behaviour 0/off
)
,filter_environment AS
(
SELECT DISTINCT e.Element																						AS EnvironmentName
				,v.ConfigurationName
				,v.ConfigurationValue
FROM			cdc_job_configurations																			v
CROSS APPLY		[DeploymentAdmin].ChangeControl.UDF_SplitStringToTable_varcharmax(v.Environment,',')			e
WHERE			e.Element	IN ( @DeploymentConfiguration_SeedData_Environment, 'ALL'	)
)
,ranked_configurations AS
(
SELECT			ROW_NUMBER() OVER (PARTITION BY ConfigurationName ORDER BY CASE WHEN EnvironmentName = 'ALL' THEN 0 ELSE 1 END DESC) AS configuration_rank
				,ConfigurationName
				,ConfigurationValue
FROM			filter_environment
)
INSERT INTO		#DeploymentConfigurations (ConfigurationName, ConfigurationValue)
SELECT			ConfigurationName
				,ConfigurationValue
FROM			ranked_configurations 
WHERE			configuration_rank							= 1 --de-dupe any configurations
UNION
SELECT			'CDCEnabledTables'																											AS ConfigurationName
				,SUBSTRING((SELECT ',' + tbl_schema + '.' + tbl_name FROM cdc_enabled_tables FOR XML PATH ( '' ) ), 2, 50000)				AS ConfigurationValue
UNION
SELECT			'CDCTablesWithChanges'																										AS ConfigurationName
				,SUBSTRING((SELECT ',' + tbl_schema + '.' + tbl_name FROM cdc_tables_with_changes FOR XML PATH ( '' ) ), 2, 50000)			AS ConfigurationValue



BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[UpsertDeploymentConfiguration]
		@DatabaseName	= @DeploymentConfiguration_SeedData_DatabaseName	
		,@Reference		= @DeploymentConfiguration_SeedData_NewVersionNumber;			
END TRY
BEGIN CATCH
	SELECT @DeploymentConfiguration_SeedData_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @DeploymentConfiguration_SeedData_DatabaseName
		,@ServerName		= @DeploymentConfiguration_SeedData_ServerName
		,@AuditMessage		= @DeploymentConfiguration_SeedData_AuditMessage 
		,@ScriptName		= @DeploymentConfiguration_SeedData_ScriptName
		,@AuditType			= 'Error';


	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @DeploymentConfiguration_SeedData_DatabaseName
		,@ScriptName	= @DeploymentConfiguration_SeedData_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
--[ChangeControl-DeploymentConfiguration_SeedData.sql|2.2]