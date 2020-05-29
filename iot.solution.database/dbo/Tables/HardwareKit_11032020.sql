CREATE TABLE [dbo].[HardwareKit_11032020] (
    [guid]          UNIQUEIDENTIFIER NOT NULL,
    [kitTypeGuid]   UNIQUEIDENTIFIER NOT NULL,
    [kitCode]       NVARCHAR (50)    NOT NULL,
    [companyGuid]   UNIQUEIDENTIFIER NULL,
    [entityGuid]    UNIQUEIDENTIFIER NULL,
    [uniqueId]      NVARCHAR (500)   NOT NULL,
    [name]          NVARCHAR (500)   NOT NULL,
    [note]          NVARCHAR (1000)  NOT NULL,
    [tag]           NVARCHAR (50)    NULL,
    [isProvisioned] BIT              NOT NULL,
    [isActive]      BIT              NOT NULL,
    [isDeleted]     BIT              NOT NULL,
    [createdDate]   DATETIME         NULL,
    [createdBy]     UNIQUEIDENTIFIER NULL,
    [updatedDate]   DATETIME         NULL,
    [updatedBy]     UNIQUEIDENTIFIER NULL
);

