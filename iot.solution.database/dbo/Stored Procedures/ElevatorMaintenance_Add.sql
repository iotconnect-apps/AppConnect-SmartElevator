
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)	
	,@newid		UNIQUEIDENTIFIER
EXEC [dbo].[ElevatorMaintenance_Add]	
	@companyGuid	= '2D442AEA-E58B-4E8E-B09B-5602E1AA545A'
	,@entityGuid			= '98611812-0DB2-4183-B352-C3FEC9A3D1A4'
	,@elevatorGuid	= '98611812-0DB2-4183-B352-C3FEC9A3D1A4'
	,@description	= 'New Factory Description'
	,@startDateTime		= '2020-05-19 15:10:04.620'
	,@endDateTime		= '2020-05-19 23:10:04.620'
	,@invokingUser	= 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'              
	,@version		= 'v1'              
	,@newid			= @newid		OUTPUT
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName, @newid newid

001	SSES-2 28-02-2020 [Nishit Khakhi]	Added Initial Version to Add Elevator Maintenance
002 SSES-2 13-03-2020 [Nishit Khakhi]	Updated to add createdDate & isDeleted flag
*******************************************************************/
CREATE PROCEDURE [dbo].[ElevatorMaintenance_Add]
(	@companyGuid	UNIQUEIDENTIFIER
	,@entityGuid	UNIQUEIDENTIFIER
	,@elevatorGuid  UNIQUEIDENTIFIER	
	,@description	NVARCHAR(1000)		= NULL
	,@startDateTime	DATETIME			= NULL
	,@endDateTime	DATETIME			= NULL
	,@invokingUser	UNIQUEIDENTIFIER	= NULL
	,@version		nvarchar(10)    
	,@newid			UNIQUEIDENTIFIER	OUTPUT
	,@output		SMALLINT			OUTPUT    
	,@fieldName		nvarchar(100)		OUTPUT   
	,@culture		nvarchar(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)			= '0'
)	
AS
BEGIN
	SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'ElevatorMaintenance_Add' AS '@procName'             
            	, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
            	, CONVERT(nvarchar(MAX),@entityGuid) AS '@entityGuid' 
				, CONVERT(nvarchar(MAX),@elevatorGuid) AS '@elevatorGuid' 				
				, @description AS '@description' 
				, CONVERT(nvarchar(50),@startDateTime) AS '@startDateTime'
				, CONVERT(nvarchar(50),@endDateTime) AS '@endDateTime'
				, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            	, CONVERT(nvarchar(MAX),@version) AS '@version' 
            	, CONVERT(nvarchar(MAX),@output) AS '@output' 
            	, @fieldName AS '@fieldName'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END       
	
	SET @newid = NEWID()

	SET @output = 1
	SET @fieldName = 'Success'

	BEGIN TRY

		IF EXISTS(SELECT TOP 1 1 FROM [dbo].[ElevatorMaintenance] 
					where [elevatorGuid] = @elevatorGuid and [companyGuid] = @companyGuid and [entityGuid] = @entityGuid 
						AND ((@startDateTime BETWEEN [startDateTime] AND [endDateTime]) OR (@endDateTime BETWEEN [startDateTime] AND [endDateTime]))
						AND [isDeleted] = 0)
		BEGIN
			SET @output = -3
			SET @fieldname = 'Elevator Maintenance already exists for selected wing!'	
			RETURN;
		END		
		
	BEGIN TRAN	
		
			INSERT INTO [dbo].[ElevatorMaintenance](
				[guid]		
				,[companyGuid]
				,[entityGuid]
				,[elevatorGuid]
				,[description]
				,[startDateTime]
				,[endDateTime]
				,[createddate]
				,[isDeleted]
				)
			VALUES(@newid
				,@companyGuid
				,@entityGuid
				,@elevatorGuid
				,@description
				,@startDateTime
				,@endDateTime
				,GETUTCDATE()
				,0
				)	
	COMMIT TRAN	
	END TRY	
	BEGIN CATCH
	SET @output = 0
	DECLARE @errorReturnMessage nvarchar(MAX)

	SELECT
		@errorReturnMessage = ISNULL(@errorReturnMessage, ' ') + SPACE(1) +
		'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), ' ') +
		'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), ' ') +
		'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), ' ') +
		'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), ' ') +
		'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), ' ') +
		'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(MAX)), ' ')

	RAISERROR (@errorReturnMessage
	, 11
	, 1
	)

	IF (XACT_STATE()) = -1 BEGIN
		ROLLBACK TRANSACTION
	END
	IF (XACT_STATE()) = 1 BEGIN
		ROLLBACK TRANSACTION
	END
	END CATCH
END