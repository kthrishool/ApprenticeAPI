--[CLRConfiguration.sql|1.0]
RAISERROR(N'Starting CLRConfiguration.sql',10,1) WITH NOWAIT;

IF EXISTS(select 1 from sys.assemblies where name = '$(DatabaseName)')
	ALTER ASSEMBLY [$(DatabaseName)] WITH PERMISSION_SET = EXTERNAL_ACCESS;


IF EXISTS(select 1 from sys.configurations where name = 'clr enabled' AND value = 0)
BEGIN
	RECONFIGURE WITH OVERRIDE;
	EXEC sp_configure 'clr enabled', 1;
	RECONFIGURE WITH OVERRIDE;
END
--[CLRConfiguration.sql|1.0]