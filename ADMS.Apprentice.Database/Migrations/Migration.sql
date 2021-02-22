IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210219015618_InitialCreate')
BEGIN
    CREATE TABLE [dbo].[ClaimSubmission] (
        [Id] int NOT NULL IDENTITY,
        [SubmissionStatus] int NOT NULL,
        [Type] int NOT NULL,
        [Category] int NOT NULL,
        [ApprenticeId] int NOT NULL,
        [ApprenticeName] nvarchar(300) NOT NULL,
        [EmployerId] int NOT NULL,
        [EmployerName] nvarchar(300) NOT NULL,
        [NetworkProviderId] int NOT NULL,
        [NetworkProviderName] nvarchar(300) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_ClaimSubmission] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210219015618_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210219015618_InitialCreate', N'5.0.3');
END;
GO

COMMIT;
GO

