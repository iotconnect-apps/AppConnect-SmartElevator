/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)

EXEC [dbo].[ElevatorMaintenance_Get]
	 @guid				= 'E9F77DD4-78BC-4461-9D00-64D927998ABE'
	,@currentDate	= '2020-05-21 06:47:56.890'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
               
 SELECT @output status,  @fieldName AS fieldName    
 
 001 SSES-2 02-06-2020 [Nishit Khakhi]	Added Initial Version to Get Elevator Maintenance Information
*******************************************************************/
CREATE PROCEDURE [dbo].[ElevatorMaintenance_Get]
(	 
	 @guid				UNIQUEIDENTIFIER
	,@currentDate		DATETIME			= NULL
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT	
	,@culture			NVARCHAR(10)	  = 'en-Us'
	,@enableDebugInfo	CHAR(1)			  = '0'
)
AS
BEGIN
    SET NOCOUNT ON
	DECLARE @orderBy VARCHAR(10)
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'ElevatorMaintenance_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'		
			, CONVERT(VARCHAR(50),@currentDate) as '@currentDate'
			, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    Set @output = 1
    SET @fieldName = 'Success'

    BEGIN TRY
		SELECT EM.[guid]
			, EM.[companyGuid]
			, EM.[entityGuid]
			, EM.[elevatorGuid]
			, E.[name] 
			, EP.[guid] AS [buildingGuid]
			, EP.[name] AS [building]
			, G.[guid] AS [wingGuid]
			, G.[name] AS [wing]
			, EM.[description]
			, CASE WHEN @currentDate >= [startDateTime] AND @currentDate <= [endDateTime]
				THEN 'Under Maintenance'
				ELSE CASE WHEN [startDateTime] < @currentDate AND [endDateTime] < @currentDate
				THEN 'Completed'
				ELSE 'Scheduled'
				END
			END AS [status]
			, EM.[startDateTime] AS [startDateTime]
			, EM.[endDateTime] AS [endDateTime]
			, EM.[createdDate]
		FROM [dbo].[ElevatorMaintenance] EM (NOLOCK)
		INNER JOIN [dbo].[Elevator] E WITH (NOLOCK) ON EM.[ElevatorGuid] = E.[guid] AND E.[isDeleted] = 0
		INNER JOIN [dbo].[Entity] G WITH (NOLOCK) ON EM.[entityGuid] = G.[guid] AND G.[isDeleted] = 0
		LEFT JOIN [dbo].[Entity] EP WITH (NOLOCK) ON G.[parentEntityGuid] = EP.[guid] AND EP.[isDeleted] = 0 
		WHERE EM.[guid]=@guid AND EM.[isDeleted]=0

	END TRY
	BEGIN CATCH
		DECLARE @errorReturnMessage nvarchar(MAX)

		SET @output = 0

		SELECT @errorReturnMessage =
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   +
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as nvarchar), '')  +
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as nvarchar), '') +
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as nvarchar), '') +
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as nvarchar), '') +
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as nvarchar), '') +
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as nvarchar(max)), '')
		RAISERROR (@errorReturnMessage, 11, 1)

		IF (XACT_STATE()) = -1
		BEGIN
			ROLLBACK TRANSACTION
		END
		IF (XACT_STATE()) = 1
		BEGIN
			ROLLBACK TRANSACTION
		END
		RAISERROR (@errorReturnMessage, 11, 1)
	END CATCH
END