﻿/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[ElevatorMaintenance_UpComingList]
	 @guid	= 'FA973382-0321-4701-A03E-CDDEAEC9F68B'
	,@currentDate	= '2020-05-21 06:47:56.890'
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SAQM-1	17-03-2020 [Nishit Khakhi]	Added Initial Version to List Upcoming Elevator Maintenance
002	SSES-87	10-04-2020 [Nishit Khakhi]	Updated to list only upcoming maintenance
*******************************************************************/

CREATE PROCEDURE [dbo].[ElevatorMaintenance_UpComingList]
(	@companyGuid		UNIQUEIDENTIFIER	= NULL
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
	,@guid				UNIQUEIDENTIFIER	= NULL
	,@currentDate		DATETIME			= NULL
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			VARCHAR(10)
	,@culture			VARCHAR(10)			= 'en-Us'
	,@output			SMALLINT			OUTPUT
	,@fieldName			VARCHAR(255)		OUTPUT
	,@count				INT OUTPUT
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
            SELECT 'ElevatorMaintenance_UpComingList' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
				, CONVERT(VARCHAR(MAX),@entityGuid) AS '@entityGuid'
            	, CONVERT(VARCHAR(MAX),@guid) AS '@guid'
				, CONVERT(VARCHAR(50),@currentDate) as '@currentDate'
				, CONVERT(VARCHAR(MAX),@version) AS '@version'
            	, CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETDATE())
    END
    
    BEGIN TRY

		SELECT TOP 10
			EM.[guid]
			, EM.[companyGuid]
			, EM.[entityGuid]
			, EM.[elevatorGuid]
			, E.[name] AS [elevatorName]
			, EP.[name] AS [building]
			, G.[name] AS [wing]
			, EM.[description]
			, EM.[startDateTime]
			, EM.[endDateTime]
			FROM [dbo].[ElevatorMaintenance] EM WITH (NOLOCK) 
			INNER JOIN [dbo].[Elevator] E (NOLOCK) ON EM.[elevatorGuid] = E.[guid] AND E.[isDeleted] = 0
			INNER JOIN [dbo].[Entity] G WITH (NOLOCK) ON EM.[entityGuid] = G.[guid] AND G.[isDeleted] = 0
			LEFT JOIN [dbo].[Entity] EP WITH (NOLOCK) ON G.[parentEntityGuid] = EP.[guid] AND EP.[isDeleted] = 0 
			WHERE EM.[isDeleted] = 0 
			AND EM.[companyGuid]= ISNULL(@companyGuid,EM.[companyGuid])
			AND EM.[entityGuid]= ISNULL(@entityGuid,EM.[entityGuid])
			AND EM.[elevatorGuid]= ISNULL(@guid,EM.[elevatorGuid])
			AND CONVERT(DATETIME,[startDateTime]) >= CONVERT(DATETIME,@currentDate)
			ORDER BY [startDateTime] ASC

			SET @count = @@ROWCOUNT

        SET @output = 1
		SET @fieldName = 'Success'
	END TRY	
	BEGIN CATCH	
		DECLARE @errorReturnMessage VARCHAR(MAX)

		SET @output = 0

		SELECT @errorReturnMessage = 
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   + 
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as VARCHAR), '')  + 
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as VARCHAR), '') + 
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as VARCHAR), '') + 
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as VARCHAR), '') + 
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as VARCHAR), '') + 
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as VARCHAR(max)), '')
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