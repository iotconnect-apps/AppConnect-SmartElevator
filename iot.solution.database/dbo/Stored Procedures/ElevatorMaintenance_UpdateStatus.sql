
/*******************************************************************
BEGIN TRAN

DECLARE @output INT = 0	
	,@fieldname nvarchar(100)
   
EXEC [dbo].[ElevatorMaintenance_UpdateStatus]	 
	@guid			= '651B311A-40D7-4304-B3C3-7C2DC4E8F505'	
	,@companyGuid	= '651B311A-40D7-4304-B3C3-7C2DC4E8F505'	
	,@status		= ''
	,@invokinguser 	= 'FF221908-486A-4E5A-843A-C68EB413F6EA'
	,@output		= @output OUTPUT		
	,@version		= 'v1'
	,@fieldname		= @fieldname OUTPUT

SELECT @output status, @fieldname fieldName

ROLLBACK TRAN

001	SSES-2 27-02-2020 [Nishit Khakhi]	Added Initial Version to Update Elevator Maintenance Status
002	SSES-2 13-03-2020 [Nishit Khakhi]	Updated to add description, scheduledDate
*******************************************************************/

CREATE PROCEDURE [dbo].[ElevatorMaintenance_UpdateStatus]
		@guid					UNIQUEIDENTIFIER
		,@companyGuid			UNIQUEIDENTIFIER	
		,@status				NVARCHAR(100)		= NULL
		,@description			NVARCHAR(1000)		= NULL
		,@scheduledDate			DATETIME			= NULL
		,@invokinguser			UNIQUEIDENTIFIER	= NULL
		,@version				nvarchar(10)    
		,@output				SMALLINT			OUTPUT    
		,@fieldname				nvarchar(100)		OUTPUT   
		,@culture				nvarchar(10)		= 'en-Us'
		,@enabledebuginfo		CHAR(1)				= '0'
AS
BEGIN
	SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'ElevatorMaintenance_UpdateStatus' AS '@procName' 
            , CONVERT(nvarchar(MAX),@guid) AS '@guid' 
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 		
            , @status AS '@status' 
			, @description AS '@description' 
			, CONVERT(nvarchar(MAX),@scheduledDate) AS '@scheduledDate' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@output) AS '@output' 
            , CONVERT(nvarchar(MAX),@fieldname) AS '@fieldname'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END       
	
	DECLARE @dt DATETIME = GETUTCDATE()

	SET @output = 1
	SET @fieldname = 'Success'

	BEGIN TRY		
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[ElevatorMaintenance] (NOLOCK) WHERE [guid] = @guid AND [companyGuid] = @companyGuid AND [isDeleted] = 0)
		BEGIN
			SET @output = -2
			SET @fieldname = 'ElevatorMaintenanceNotFound'
			RETURN;
		END    
		
		BEGIN TRAN

			UPDATE [dbo].[ElevatorMaintenance] 
			SET [status] = ISNULL(@status,[status])
				,[description] = ISNULL(@description,[description])
				,[scheduledDate] = ISNULL(@scheduledDate,[scheduledDate])
			WHERE [guid] = @guid AND [companyGuid] = @companyGuid AND [isDeleted] = 0

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