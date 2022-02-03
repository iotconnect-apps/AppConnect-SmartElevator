/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[configuration] WHERE [configKey] = 'db-version')
BEGIN
	INSERT [dbo].[Configuration] ([guid], [configKey], [value], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) 
        VALUES (N'cf45da4c-1b49-49f5-a5c3-8bc29c1999ea', N'db-version', N'1', 0, CAST(N'2020-01-01T13:16:53.940' AS DateTime), NULL, CAST(N'2020-04-08T13:16:53.940' AS DateTime), NULL)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[configuration] WHERE [configKey] = 'telemetry-last-exectime')
BEGIN
	INSERT [dbo].[Configuration] ([guid], [configKey], [value], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) 
        VALUES (N'465970b2-8bc3-435f-af97-8ca26f2bf383', N'telemetry-last-exectime', N'2020-01-01 12:08:02.380', 0, CAST(N'2020-02-25T06:41:01.030' AS DateTime), NULL, CAST(N'2020-02-25T06:41:01.030' AS DateTime), NULL)
END

IF NOT EXISTS(SELECT 1 FROM dbo.[configuration] WHERE [configKey] = 'db-version') 
	OR ((SELECT CONVERT(FLOAT,[value]) FROM dbo.[configuration] WHERE [configKey] = 'db-version') < 1 )
BEGIN

	INSERT [dbo].[KitType] ([guid], [companyGuid], [name], [code], [tag], [isActive], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) 
		VALUES (N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'b811b983-9448-4021-9978-6c699404eb81', N'SEDefault', N'SEDefault', N'base', 1, 0, CAST(N'2020-02-25T11:58:30.127' AS DateTime), N'6ba9cad3-c112-44df-9490-74292f7ed5b5', NULL, NULL)
	
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'21dae181-228c-40de-848f-0e5a1045269e', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'vibration', N'vibration', NULL, N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'1990bf7d-9612-431e-b103-0fc10e41e807', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'rpm', N'rpm', NULL, N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'fa41936f-e933-4cff-94f2-109bbed6e082', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'air_quality', N'quality', NULL, N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'33e9e021-09b9-4309-983f-808a311e8ff6', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'currentin', N'consumption', NULL, N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'5e84cbd7-8030-4a87-8431-820e953dcf4b', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'temp', N'temp', NULL, N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'0005cf96-56fa-434f-9f60-8aeabbc0e6ad', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'humidity', N'humidity', NULL, N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'2e375a70-41ea-4d95-b28b-98aa6e41472b', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'trip', N'trip', NULL, N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description]) VALUES (N'57394742-ea69-435a-883f-c6b815a53ab4', NULL, N'92ff19d0-8863-4142-a19c-9a74d6f9353d', N'fire_detector', N'fireDetector', NULL, N'')
	
	INSERT INTO [dbo].[AdminUser] ([guid],[email],[companyGuid],[firstName],[lastName],[password],[isActive],[isDeleted],[createdDate]) VALUES (NEWID(),'admin@elevator.com','AB469212-2488-49AD-BC94-B3A3F45590D2','Elevator','admin','Softweb#123',1,0,GETUTCDATE())

	INSERT [dbo].[UserDasboardWidget] ([Guid], [DashboardName], [Widgets], [IsDefault], [IsSystemDefault], [UserId], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (N'2AFB7737-9F88-4BD1-9447-14D495E40DE0', N'Default Dashboard', N'[]', 0, 1, N'00000000-0000-0000-0000-000000000000', 1, 0, CAST(N'2020-07-06T14:52:39.567' AS DateTime), N'00000000-0000-0000-0000-000000000000', CAST(N'2020-07-06T14:53:09.490' AS DateTime), N'00000000-0000-0000-0000-000000000000')

	UPDATE [dbo].[Configuration]
	SET [value]  = '1'
	WHERE [configKey] = 'db-version'

END
GO