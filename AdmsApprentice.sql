if not  exists (select schema_id('Infrastructure') )
begin

exec ('CREATE SCHEMA [Infrastructure] AUTHORIZATION [dbo]')

end
go

if exists (select object_id('dbo.tfndetail'))
begin
	drop table [dbo].[TfnDetail]
end
go
if exists (select object_id('dbo.TfnStatusHistory'))
begin
drop table [dbo].[TfnStatusHistory]
end
go
if exists (select object_id('dbo.Apprentice'))
begin
drop table [dbo].Apprentice
end
go


PRINT N'Creating [dbo].[TfnDetail]...';


GO
CREATE TABLE [dbo].[TfnDetail] (
    [TfnDetailId]  INT          IDENTITY (1, 1) NOT NULL,
    [ApprenticeId] INT          NOT NULL,
    [TFN]          VARCHAR (20) NOT NULL,
    [Status]       VARCHAR (10) NOT NULL,
    [CreatedBy]                        NVARCHAR (1024) NOT NULL,
    [CreatedOn]                        DATETIME2 (7)   NOT NULL,
    [UpdatedBy]                        NVARCHAR (1024) NOT NULL,
    [UpdatedOn]                        DATETIME2 (7)   NOT NULL,
    [Version]                          ROWVERSION      NOT NULL,
    CONSTRAINT [PK_TfnDetail] PRIMARY KEY CLUSTERED ([TfnDetailId] ASC)
);


GO
PRINT N'Creating [dbo].[TfnStatusHistory]...';


GO
CREATE TABLE [dbo].[TfnStatusHistory] (
    [TfnStatusHistoryId] INT          IDENTITY (1, 1) NOT NULL,
    [TfnDetailId]        INT          NOT NULL,
    [Status]             VARCHAR (10) NOT NULL,
    [CreatedBy]                        NVARCHAR (1024) NOT NULL,
    [CreatedOn]                        DATETIME2 (7)   NOT NULL,
    [UpdatedBy]                        NVARCHAR (1024) NOT NULL,
    [UpdatedOn]                        DATETIME2 (7)   NOT NULL,
    [Version]                          ROWVERSION      NOT NULL,
    CONSTRAINT [PK_TfnStatusHistory] PRIMARY KEY CLUSTERED ([TfnStatusHistoryId] ASC)
);


GO
PRINT N'Creating [dbo].[Apprentice]...';
go

create table dbo.Apprentice (
    ApprenticeId INT          IDENTITY (1, 1) NOT NULL,

Surname varchar(50) Not Null,
FirstName varchar(50) Not Null,
OtherNames varchar(50) Null,
PreferredName varchar(50)    Null,        
BirthDate date Not Null,
GenderCode varchar(20) Null,
EmailAddress varchar(320) Null,
SelfAssessedDisabilityCode varchar(20) Null,
IndigenousStatusCode     varchar(20)    Null,
CitizenshipCode  varchar(20) Null,
EducationLevelCode varchar(20) Null,
LeftSchoolMonthCode varchar(20) Null,
LeftSchoolYearCode varchar(20) Null,
ProfileTypeCode          varchar(20) Null,  
DeceasedFlag            bit Null,
ActiveFlag          bit Null,
    [CreatedBy]                        NVARCHAR (1024) NOT NULL,
    [CreatedOn]                        DATETIME2 (7)   NOT NULL,
    [UpdatedBy]                        NVARCHAR (1024) NOT NULL,
    [UpdatedOn]                        DATETIME2 (7)   NOT NULL,
    [Version]                          ROWVERSION      NOT NULL,
    [_AuditEventId]                    BIGINT          NOT NULL,
)

go


if exists (select object_id('Infrastructure.AuditEvent'))
begin
drop table [Infrastructure].[AuditEvent]
end
go

CREATE TABLE [Infrastructure].[AuditEvent] (
    [AuditEventId]        BIGINT           IDENTITY (1, 1) NOT NULL,
    [CorrelationId]       UNIQUEIDENTIFIER NOT NULL,
    [EventName]           VARCHAR (200)    NULL,
    [ApplicationUsername] VARCHAR (1024)   NOT NULL,
    [CreatedOn]           DATETIME2 (7)    NOT NULL,
    [ProcessStatus]       TINYINT          CONSTRAINT [DF_AuditEvent_ProcessStatus] DEFAULT ((0)) NOT NULL,
    [SessionId]           INT              NULL,
    [LoginName]           VARCHAR (128)    CONSTRAINT [DF_AuditEvent_LoginName] DEFAULT (suser_sname()) NULL,
    [LoginTime]           DATETIME2 (7)    NULL,
    [ProgramName]         VARCHAR (256)    NULL,
    [HostName]            VARCHAR (50)     NULL,
    [ReferenceNumber]     VARCHAR (50)     NULL,
    [MessageId]           UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Infrastructure.AuditEvent] PRIMARY KEY CLUSTERED ([AuditEventId] ASC)
);


GO
CREATE TRIGGER [Infrastructure].[trig_AuditEvent_Update] ON [Infrastructure].[AuditEvent]
FOR UPDATE
AS
/*--------------------------------------------------------------------------------
  Author			: Mick Vullo
  Version			: 1.00
  CreatedDate		: 27 Mar 2015

  Object            : [Infrastructure].[trig_AuditEvent_Update]

  Description		: The table is part of the infrastructure which logs audit events. The future directions framework writes a record
                      with application specific values such as the correlationId and the ApplictaionUsername. THe framework updates this record when a transaction is created for the 
					  actual work setting the processStaus to 1.
					  this trigger then proceeds to update the login information like SessionID etc
					  The AuditEventId identity column is placed into the CONTEXT_INFO to be picked up and used in the audit tables so that we can correlate application 
					  events with audit records
  Ammendment History:

----------------------------------------------------------------------------------*/
DECLARE @AuditEventId varbinary(128);

SELECT TOP 1 @AuditEventId = CAST (AuditEventId AS Varbinary) FROM Inserted

SET CONTEXT_INFO @AuditEventId

UPDATE [Infrastructure].[AuditEvent]
SET 
	 [ProcessStatus] = i.ProcessStatus
    ,[SessionId] = s.SessionID
    ,[LoginName] = s.LoginName
    ,[LoginTime] = s.LoginTime
    ,[ProgramName] = s.[ProgramName]
    ,[HostName] = s.[HostName]
FROM 
	INSERTED i
	CROSS JOIN 
	(
		SELECT 
			SessionID = SPID
			,LoginName =  Loginame
			,LoginTime = login_Time
			,[ProgramName] = program_name
			,hostname
		FROM 
			[master].sys.sysprocesses
		WHERE 
			SPID = @@SPID
	) s
WHERE
	[Infrastructure].[AuditEvent].AuditEventId = i.AuditEventId
	AND i.ProcessStatus = 1


