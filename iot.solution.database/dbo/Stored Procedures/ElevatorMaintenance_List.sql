﻿/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[ElevatorMaintenance_List]
	 @companyGuid	= '2D442AEA-E58B-4E8E-B09B-5602E1AA545A'
	,@entityGuid	= '98611812-0DB2-4183-B352-C3FEC9A3D1A4'
	,@currentDate	= '2020-05-21 06:47:56.890'
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SSES-2 27-02-2020 [Nishit Khakhi]	Added Initial Version to List Elevator Maintenance
002 SSES-2 13-03-2020 [Nishit Khakhi]	Updated to add createdDate & isDeleted flag
*******************************************************************/
CREATE PROCEDURE [dbo].[ElevatorMaintenance_List]
(	@companyGuid		UNIQUEIDENTIFIER
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
	,@currentDate		DATETIME			= NULL
	,@search			VARCHAR(100)		= NULL
	,@pageSize			INT
	,@pageNumber		INT
	,@orderby			VARCHAR(100)		= NULL
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			VARCHAR(10)
	,@culture			VARCHAR(10)			= 'en-Us'
	,@output			SMALLINT			OUTPUT
	,@fieldName			VARCHAR(255)		OUTPUT
	,@count				INT OUTPUT
	,@enableDebugInfo		CHAR(1)			= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'ElevatorMaintenance_List' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
				, CONVERT(VARCHAR(MAX),@entityGuid) AS '@entityGuid'
            	, CONVERT(VARCHAR(50),@currentDate) as '@currentDate'
				, CONVERT(VARCHAR(MAX),@search) AS '@search'
				, CONVERT(VARCHAR(MAX),@pageSize) AS '@pageSize'
				, CONVERT(VARCHAR(MAX),@pageNumber) AS '@pageNumber'
				, CONVERT(VARCHAR(MAX),@orderby) AS '@orderby'
				, CONVERT(VARCHAR(MAX),@version) AS '@version'
            	, CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETDATE())
    END
    
    BEGIN TRY

		SELECT
		 @output = 1
		,@count = -1

		IF OBJECT_ID('tempdb..#temp_ElevatorMaintenance') IS NOT NULL DROP TABLE #temp_ElevatorMaintenance

		CREATE TABLE #temp_ElevatorMaintenance
		(	[guid]			UNIQUEIDENTIFIER
			,[companyGuid]	UNIQUEIDENTIFIER
			,[entityGuid]	UNIQUEIDENTIFIER
			,[elevatorGuid]	UNIQUEIDENTIFIER
			,[name]			NVARCHAR(500)
			,[building]		NVARCHAR(500)
			,[wing]			NVARCHAR(500)
			,[description]	NVARCHAR(1000)
			,[status]		NVARCHAR(100)
			,[startDateTime]	DATETIME
			,[endDateTime]		DATETIME
			,[createdDate]	DATETIME
			,[rowNum]		INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'status asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		( SELECT
			EM.[guid]
			, EM.[companyGuid]
			, EM.[entityGuid]
			, EM.[elevatorGuid]
			, E.[name] AS [elevatorName]
			, EP.[name] AS [building]
			, G.[name] AS [wing]
			, EM.[description]
			, CASE WHEN @currentDate >= [startDateTime] AND @currentDate <= [endDateTime]
				THEN ''Under Maintenance''
				ELSE CASE WHEN [startDateTime] < @currentDate AND [endDateTime] < @currentDate
				THEN ''Completed''
				ELSE ''Scheduled''
				END
			END AS [status]
			, EM.[startDateTime] AS [startDateTime]
			, EM.[endDateTime] AS [endDateTime]
			, EM.[createdDate] 
			FROM [dbo].[ElevatorMaintenance] EM WITH (NOLOCK) 
			INNER JOIN [dbo].[Elevator] E ON EM.[elevatorGuid] = E.[guid] AND E.[isDeleted] = 0
			INNER JOIN [dbo].[Entity] G WITH (NOLOCK) ON EM.[entityGuid] = G.[guid] AND G.[isDeleted] = 0
			LEFT JOIN [dbo].[Entity] EP WITH (NOLOCK) ON G.[parentEntityGuid] = EP.[guid] AND EP.[isDeleted] = 0 
			WHERE EM.[companyGuid]=@companyGuid AND EM.[isDeleted] = 0 '
			+ CASE WHEN @entityGuid IS NULL THEN '' ELSE
				' AND EM.[entityGuid] = @entityGuid '
			END +
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (G.[name] LIKE ''%' + @search + '%''
			  OR E.[name] LIKE ''%' + @search + '%''
			  OR EP.[name] LIKE ''%' + @search + '%''
			  OR EM.[description] LIKE ''%' + @search + '%'' 
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_ElevatorMaintenance
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER, @entityGuid UNIQUEIDENTIFIER, @currentDate DATETIME '
			, @orderby		= @orderby			
			, @companyGuid	= @companyGuid			
			, @entityGuid	= @entityGuid			
			, @currentDate  = @currentDate
		SET @count = @@ROWCOUNT

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					EM.[guid]
					, EM.[companyGuid]
					, EM.[entityGuid]
					, EM.[elevatorGuid]
					, EM.[name]
					, EM.[building]
					, EM.[wing]
					, EM.[description]
					, EM.[status]		
					, EM.[startDateTime]
					, EM.[endDateTime]
					, EM.[createdDate]
				FROM #temp_ElevatorMaintenance EM
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					EM.[guid]
					, EM.[companyGuid]
					, EM.[entityGuid]
					, EM.[elevatorGuid]
					, EM.[name]
					, EM.[building]
					, EM.[wing]
					, EM.[description]
					, EM.[status]	
					, EM.[startDateTime]
					, EM.[endDateTime]	
					, EM.[createdDate]	
				FROM #temp_ElevatorMaintenance EM
			END
	   
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