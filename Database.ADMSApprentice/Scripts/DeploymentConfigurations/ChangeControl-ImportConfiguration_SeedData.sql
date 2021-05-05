--[ChangeControl-ImportConfiguration_SeedData.sql|2.0]
RAISERROR('STARTING ChangeControl-ImportConfiguration_SeedData.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @ImportConfiguration_DataImportRootPath		VARCHAR(MAX)	= '$(CSVPath)'						
		,@ImportConfiguration_Environment			VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@TablesToImport							VARCHAR(MAX)
		,@ImportConfiguration_DatabaseName			VARCHAR(255)	= DB_NAME()
		,@ImportConfiguration_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@ImportConfiguration_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@ImportConfiguration_AuditMessage			VARCHAR(MAX)
		,@ImportConfiguration_ScriptName			VARCHAR(50)		= 'ChangeControl-ImportConfiguration_SeedData.sql';



IF OBJECT_ID('tempdb..#ImportConfigurations') IS NOT NULL
	DROP TABLE #ImportConfigurations;

CREATE TABLE #ImportConfigurations (ConfigurationName VARCHAR (256) NOT NULL, ConfigurationValue VARCHAR (MAX) NULL)

SELECT	@ImportConfiguration_Environment = 
													CASE 
														WHEN @ImportConfiguration_Environment LIKE '%DEV%' THEN 'DEV'
														WHEN @ImportConfiguration_Environment LIKE '%TEST%' THEN 'TEST'
														ELSE @ImportConfiguration_Environment 
													END
;WITH TablesToImport 
(			Environment				,TableSchema		,TableName				,LoadTableIndexName	,EmptyTableOnly)
AS
(
		SELECT	NULL				,NULL				,NULL					,NULL				,NULL

)
,CheckEmplyTableOnly AS
(
SELECT		t.TableSchema		
			,t.TableName			
			,t.LoadTableIndexName
FROM		TablesToImport															t
LEFT JOIN	sys.schemas																s
ON			s.name				= t.TableSchema
LEFT JOIN	sys.tables																a
ON			a.name				= t.TableName
AND			a.schema_id			= s.schema_id
LEFT JOIN	sys.partitions															p
ON			p.object_id			= a.object_id
AND			p.index_id			IN (0,1)
WHERE		t.Environment		IN ( @ImportConfiguration_Environment, 'ALL' )
AND			CASE
				WHEN COALESCE(t.EmptyTableOnly,0) = 0	THEN 1
				WHEN COALESCE(t.EmptyTableOnly,0) = 1 
				AND COALESCE(p.rows,0) = 0				THEN 1
				ELSE 0
			END					= 1
)
,StuffIntoXML (TableNames) AS
(
SELECT		LTRIM(RTRIM(COALESCE(TableSchema,'dbo')))
			+ '.' + LTRIM(RTRIM(TableName)) 
			+ CASE WHEN COALESCE(LTRIM(RTRIM(LoadTableIndexName)),'') <> '' THEN '.' + LTRIM(RTRIM(LoadTableIndexName)) ELSE '' END
			+ ','
FROM		CheckEmplyTableOnly
WHERE		COALESCE(TableName,'') <> ''
FOR XML PATH('')
)
SELECT @TablesToImport = TableNames FROM StuffIntoXML;


;WITH ImportConfigurations
(				EnvironmentName							,ConfigurationName										,ConfigurationValue
)
AS
(
SELECT			'ALL'									,'ImportConfiguration_StagingSchema'					,'DeploymentStaging'
UNION	SELECT	'ALL'									,'ImportConfiguration_RowDelimiter'						,'|~#|\n'
UNION	SELECT	'ALL'									,'ImportConfiguration_ColumnDelimiter'					,'|~!|'
UNION	SELECT	'ALL'									,'ImportConfiguration_DataImportFilePath'				,@ImportConfiguration_DataImportRootPath --+ 'Import\'
UNION	SELECT	'ALL'									,'ImportConfiguration_TBDFilePath'						,@ImportConfiguration_DataImportRootPath + 'TBD\'
UNION	SELECT	'ALL'									,'ImportConfiguration_TablesToImport'					,@TablesToImport
UNION	SELECT	'ALL'									,'ImportConfiguration_BCPUsingUnicodeCharacters'		,'0'
UNION	SELECT	'ALL'									,'ImportConfiguration_DataExportFilePath'				,@ImportConfiguration_DataImportRootPath
UNION	SELECT	'ALL'									,'ImportConfiguration_EnableMoveInputFilesTBD'			,'1'

UNION	SELECT	'DEV'									,'ImportConfiguration_DataImportFile_MaxAgeHours'		,'100000'
UNION	SELECT	'TEST'									,'ImportConfiguration_DataImportFile_MaxAgeHours'		,'72'
UNION	SELECT	'PREPROD,PROD,TRAINING,PRETRAINING'		,'ImportConfiguration_DataImportFile_MaxAgeHours'		,'6'

UNION	SELECT	'DEV,TEST,TRAINING,PRETRAINING'			,'ImportConfiguration_TBDFileRetentionDays'				,'0'
UNION	SELECT	'PREPROD'								,'ImportConfiguration_TBDFileRetentionDays'				,'1'
UNION	SELECT	'PROD'									,'ImportConfiguration_TBDFileRetentionDays'				,'7'
)
,filter_environment AS
(
SELECT DISTINCT e.Element																						AS EnvironmentName
				,v.ConfigurationName
				,v.ConfigurationValue
FROM			ImportConfigurations																			v
CROSS APPLY		[DeploymentAdmin].ChangeControl.UDF_SplitStringToTable_varcharmax(v.EnvironmentName,',')		e
WHERE			e.Element	IN ( @ImportConfiguration_Environment, 'ALL'	)
)
,ranked_configurations AS
(
SELECT			ROW_NUMBER() OVER (PARTITION BY ConfigurationName ORDER BY CASE WHEN EnvironmentName = 'ALL' THEN 0 ELSE 1 END DESC) AS configuration_rank
				,ConfigurationName
				,ConfigurationValue
FROM			filter_environment
)
INSERT INTO		#ImportConfigurations (ConfigurationName, ConfigurationValue)
SELECT			ConfigurationName
				,ConfigurationValue
FROM			ranked_configurations 
WHERE			configuration_rank							= 1 --de-dupe any configurations



BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[UpsertImportConfiguration]
		@DatabaseName	= @ImportConfiguration_DatabaseName
		,@Reference		= @ImportConfiguration_NewVersionNumber;

END TRY
BEGIN CATCH
	SELECT @ImportConfiguration_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @ImportConfiguration_DatabaseName
		,@ServerName		= @ImportConfiguration_ServerName
		,@AuditMessage		= @ImportConfiguration_AuditMessage 
		,@ScriptName		= @ImportConfiguration_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @ImportConfiguration_DatabaseName
		,@ScriptName	= @ImportConfiguration_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
--[ChangeControl-ImportConfiguration_SeedData.sql|2.0]