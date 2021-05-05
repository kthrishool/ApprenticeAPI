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


