/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[Entity_List]
	 @companyGuid	= 'B811B983-9448-4021-9978-6C699404EB81'
	,@parentEntityGuid	= 'B811B983-9448-4021-9978-6C699404EB81'
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SSES-2	25/02/2020	Nishit Khakhi	Updated for Elevator DB
*******************************************************************/
CREATE PROCEDURE [dbo].[Entity_List]
(	@companyGuid		UNIQUEIDENTIFIER
	,@parentEntityGuid	UNIQUEIDENTIFIER	= NULL
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
	,@enableDebugInfo		CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'Entity_List' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
				, CONVERT(VARCHAR(MAX),@parentEntityGuid) AS '@parentEntityGuid'
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

		SELECT	@output = 1
				,@count = -1

		IF OBJECT_ID('tempdb..#temp_Entity') IS NOT NULL DROP TABLE #temp_Entity

		CREATE TABLE #temp_Entity
		(	[guid]				UNIQUEIDENTIFIER
			,[companyGuid]		UNIQUEIDENTIFIER
			,[parentEntityGuid]	UNIQUEIDENTIFIER
			,[name]				NVARCHAR(500)
			,[description]		NVARCHAR(500)
			,[address]			NVARCHAR(500)
			,[address2]			NVARCHAR(500)
			,[city]				NVARCHAR(50)
			,[zipCode]			NVARCHAR(10)
			,[stateGuid]		UNIQUEIDENTIFIER NULL
			,[countryGuid]		UNIQUEIDENTIFIER NULL
			,[image]			NVARCHAR(250)
			,[latitude]			NVARCHAR(50)
			,[longitude]		NVARCHAR(50)
			,[elevatorCount]	BIGINT
			,[wingCount]		BIGINT
			,[isActive]			BIT
			,[rowNum]			INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'name asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		(
			SELECT
			G.[guid]
			, G.[companyGuid]
			, G.[parentEntityGuid]
			, G.[name]
			, G.[description]
			, G.[address] 
			, G.[address2] AS address2
			, G.[city]
			, G.[zipCode]
			, G.[stateGuid]
			, G.[countryGuid]
			, G.[image]
			, G.[latitude]
			, G.[longitude]
			, 0 AS [elevatorCount]
			, 0 AS [wingCount]
			, G.[isActive]			
			FROM [dbo].[Entity] AS G WITH (NOLOCK) 
			 WHERE G.[companyGuid]=@companyGuid AND G.[isDeleted]=0 '
			  + ' and G.Guid not in (select entityGuid from [dbo].[Company] where [Guid]=@companyGuid) '
			 + CASE WHEN @parentEntityGuid IS NOT NULL THEN ' AND G.[parentEntityGuid] = @parentEntityGuid ' ELSE
				' AND G.[parentEntityGuid] IS NULL ' END 
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (G.name LIKE ''%' + @search + '%''
			  OR G.address LIKE ''%' + @search + '%''
			  OR G.address2 LIKE ''%' + @search + '%''
			  OR G.zipCode LIKE ''%' + @search + '%''
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_Entity
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER, @parentEntityGuid UNIQUEIDENTIFIER '
			, @orderby		= @orderby			
			, @companyGuid	= @companyGuid			
			, @parentEntityGuid	= @parentEntityGuid			
			
		SET @count = @@ROWCOUNT
		
		;WITH CTE_Elevator_Parent
		AS
		(	SELECT GH.[parentEntityGuid], COUNT(E.[guid]) AS [totalCount] 
			FROM [dbo].[Elevator] E (NOLOCK)
			INNER JOIN dbo.[Entity] GH (NOLOCK) ON (E.[entityGuid] = GH.[guid] OR E.[entityGuid] = GH.[parentEntityGuid])
			WHERE E.[companyGuid] = @companyGuid AND E.[isActive] = 1 AND E.[isDeleted] = 0
			GROUP BY GH.[parentEntityGuid]
		)
		, CTE_Elevator_Child
		AS
		(	SELECT GH.[guid], COUNT(E.[guid]) AS [totalCount] 
			FROM [dbo].[Elevator] E (NOLOCK)
			INNER JOIN dbo.[Entity] GH (NOLOCK) ON (E.[entityGuid] = GH.[guid] OR E.[entityGuid] = GH.[parentEntityGuid])
			WHERE E.[companyGuid] = @companyGuid AND E.[isActive] = 1 AND E.[isDeleted] = 0
			GROUP BY GH.[guid]
		)
		, CTE_Wing
		AS
		(	SELECT GH.[guid], COUNT(E.[guid]) AS [totalCount] 
			FROM [dbo].[Entity] E (NOLOCK)
			INNER JOIN #temp_Entity GH ON E.[parentEntityGuid] = GH.[guid]
			WHERE E.[companyGuid] = @companyGuid AND E.[isActive] = 1 AND E.[isDeleted] = 0
			GROUP BY GH.[guid]
		)
		UPDATE t
		SET [elevatorCount] = CASE WHEN @parentEntityGuid IS NULL THEN ISNULL(PD.[totalCount],0) ELSE ISNULL(CD.[totalCount],0) END
			,[wingCount] = ISNULL(CW.[totalCount],0)
		FROM #temp_Entity t
		LEFT JOIN CTE_Elevator_Parent PD ON t.[guid] = PD.[parentEntityGuid]
		LEFT JOIN CTE_Elevator_Child CD ON t.[guid] = CD.[guid]
		LEFT JOIN CTE_Wing CW ON t.[guid] = CW.[guid]

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					G.[guid]
					, G.[parentEntityGuid]
					, G.[name]
					, G.[description]
					, G.[address] 
					, G.[address2] AS address2
					, G.[city]
					, G.[zipCode]
					, G.[stateGuid]
					, G.[countryGuid]
					, G.[image]
					, G.[latitude]
					, G.[longitude]
					, G.[elevatorCount]
					, G.[wingCount]
					, G.[isActive]					
				FROM #temp_Entity G
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					G.[guid]
					, G.[parentEntityGuid]
					, G.[name]
					, G.[description]
					, G.[address] 
					, G.[address2] AS address2
					, G.[city]
					, G.[zipCode]
					, G.[stateGuid]
					, G.[countryGuid]
					, G.[image]
					, G.[latitude]
					, G.[longitude]
					, G.[elevatorCount]
					, G.[wingCount]
					, G.[isActive]		
				FROM #temp_Entity G
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