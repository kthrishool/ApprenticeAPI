CREATE TABLE [dbo].[CDCLockedTables] (
    [ID]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [object_id]  INT            NOT NULL,
    [schemaname] NVARCHAR (100) NOT NULL,
    [name]       NVARCHAR (100) NOT NULL,
    [updated_on] DATETIME       NOT NULL,
    [active]     BIT            CONSTRAINT [DF_CDCLockedTables_active] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CDCLockedTables] PRIMARY KEY CLUSTERED ([ID] ASC)
);

