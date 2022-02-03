CREATE TABLE [dbo].[ElevatorMaintenance] (
    [guid]         UNIQUEIDENTIFIER NOT NULL,
    [companyGuid]  UNIQUEIDENTIFIER NOT NULL,
    [entityGuid]   UNIQUEIDENTIFIER NOT NULL,
    [elevatorGuid] UNIQUEIDENTIFIER NOT NULL,
    [description]  NVARCHAR (1000)  NULL,
	[startDateTime]  DATETIME       NULL,
	[endDateTime]  DATETIME         NULL,
    [createdDate]  DATETIME         NULL,
    [isDeleted] BIT Default (0) NOT NULL, 
    CONSTRAINT [PK__Elevator__497F6CB4D51981B3] PRIMARY KEY CLUSTERED ([guid] ASC)
);

