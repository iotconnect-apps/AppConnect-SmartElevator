/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName					nvarchar(255)
	,@syncDate	DATETIME
EXEC [dbo].[Chart_TripsByElevator]	
	@companyguid = 'B811B983-9448-4021-9978-6C699404EB81'
	,@guid	= '937bb2f8-e615-493a-99ec-56523fd075b3'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName, @syncDate syncDate

001	SSES-2 19-03-2020 [Nishit Khakhi]	Added Initial Version to represent Peak Hour By Elevators

*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_TripsByElevator]
(	@companyguid		UNIQUEIDENTIFIER	
	,@guid				UNIQUEIDENTIFIER	
	,@invokinguser		UNIQUEIDENTIFIER	= NULL
	,@version			nvarchar(10)              
	,@output			SMALLINT			OUTPUT
	,@fieldname			nvarchar(255)		OUTPUT
	,@syncDate			DATETIME			OUTPUT
	,@culture			nvarchar(10)		= 'en-Us'	
	,@enabledebuginfo	CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Chart_TripsByElevator' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, CONVERT(nvarchar(MAX),@guid) AS '@guid' 
			, CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
    BEGIN TRY  
		IF OBJECT_ID('tempdb..#weekdays') IS NOT NULL BEGIN DROP TABLE #weekdays END
		IF OBJECT_ID('tempdb..#OperationHours') IS NOT NULL BEGIN DROP TABLE #OperationHours END
		
		CREATE TABLE #weekdays ([weekDay] NVARCHAR(20))
		CREATE TABLE #OperationHours ([Time] NVARCHAR(20), [attribute] NVARCHAR(1000), [uniqueId] NVARCHAR(500), [value] BIGINT) 
		
		INSERT INTO #weekdays values ('00:00'),('03:00'),('06:00'),('09:00'),('12:00'),('15:00'),('18:00'),('21:00')

		INSERT INTO #OperationHours([Time],[attribute],[uniqueId],[value])
		SELECT CASE WHEN LEN(CONVERT(NVARCHAR(2),([Hour]*3))) < 2 THEN 
							'0' + CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						ELSE CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						END ,[attribute],[uniqueId],[value]
			FROM ( 
				SELECT DATEPART(HOUR,[date])/3 AS [Hour],[attribute],E.[uniqueId],SUM([sum]) AS [value] 
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON E.[guid] = @guid
				WHERE [attribute] = 'trip' AND CONVERT(Date,[date]) = CONVERT(DATE,GETUTCDATE())
				GROUP BY DATEPART(HOUR,[date])/3,[attribute],E.[uniqueId]
				) [data]

		SELECT * FROM (
			SELECT O.[Time] AS [Time], 'Trip' AS [attribute], [uniqueId] AS [uniqueId], ISNULL([value],0) [value]
			FROM #OperationHours O 
			) A
			ORDER BY 
				CASE [Time] 
						WHEN '00:00' THEN 1
						WHEN '03:00' THEN 2
						WHEN '06:00' THEN 3
						WHEN '09:00' THEN 4
						WHEN '12:00' THEN 5
						WHEN '15:00' THEN 6
						WHEN '18:00' THEN 7
						WHEN '21:00' THEN 8
						ELSE 9
				END 
				
        SET @output = 1
		SET @fieldname = 'Success'   
        SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')     
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


