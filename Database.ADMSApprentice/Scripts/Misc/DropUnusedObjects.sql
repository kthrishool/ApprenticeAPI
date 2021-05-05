--[DropUnusedObjects.sql|1.1]
SET NOCOUNT ON;
RAISERROR(N'STARTING DropUnusedObjects.sql...',10,1) WITH NOWAIT;


--IF EXISTS(SELECT 1 FROM sys.procedures WHERE name = 'RefreshLastLonProcessDate' AND schema_id = SCHEMA_ID('Admin'))
--BEGIN
--	RAISERROR(N'Dropping [Admin].[RefreshLastLonProcessDate]',10,1) WITH NOWAIT;
--	DROP PROCEDURE [Admin].[RefreshLastLonProcessDate];
--END
--[DropUnusedObjects.sql|1.1]