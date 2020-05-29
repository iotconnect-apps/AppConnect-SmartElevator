/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName					nvarchar(255)	
	,@syncDate	DATETIME
EXEC [dbo].[Chart_PeakHoursByElevator]	
	@companyguid = 'B811B983-9448-4021-9978-6C699404EB81'
	,@elevators	= 'Kit12thMarch01'
	,@frequency = 'M'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName , @syncDate syncDate

001	SSES-2 18-03-2020 [Nishit Khakhi]	Added Initial Version to represent Peak Hour By Elevators
002	SSES-2 27-03-2020 [Nishit Khakhi]	Updated to return default month and week day name in return while for value
003 SSES-3 14-04-2020 [Nishit Khakhi]	Updated to return Day frequency and order by
*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_PeakHoursByElevator]
(	@companyguid		UNIQUEIDENTIFIER	
	,@elevators			NVARCHAR(1000)		
	,@frequency			CHAR(1)				
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
            SELECT 'Chart_PeakHoursByElevator' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, @elevators AS '@elevators' 
			, @frequency AS '@frequency' 
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
    BEGIN TRY 
	 SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
		DECLARE @dt DATETIME = GETUTCDATE(), @endDate DATETIME
		IF OBJECT_ID ('tempdb..#ids') IS NOT NULL DROP TABLE #ids
		IF OBJECT_ID('tempdb..#weekdays') IS NOT NULL BEGIN DROP TABLE #weekdays END
		IF OBJECT_ID('tempdb..#OperationHours') IS NOT NULL BEGIN DROP TABLE #OperationHours END
		IF OBJECT_ID('tempdb..#finalTable') IS NOT NULL BEGIN DROP TABLE #finalTable END
		IF OBJECT_ID ('tempdb..#months') IS NOT NULL BEGIN DROP TABLE #months END
		CREATE TABLE [#months] ([date] DATE)
		CREATE TABLE #weekdays ([weekDay] NVARCHAR(20))
		CREATE TABLE #OperationHours ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [attribute] NVARCHAR(1000), [uniqueId] NVARCHAR(500), [value] BIGINT) 
		CREATE TABLE #finalTable ([date] DATE, [Year] INT, [Month] INT,[name] NVARCHAR(20), [attribute] NVARCHAR(1000), [uniqueId] NVARCHAR(500), [value] BIGINT) 
		
		SELECT [value] as [uniqueId]
		INTO #ids
		FROM string_split(@elevators,',')
		          
		IF @frequency = 'D'
		BEGIN
			SET @endDate = @dt
			INSERT INTO #weekdays values ('00:00'),('03:00'),('06:00'),('09:00'),('12:00'),('15:00'),('18:00'),('21:00')

			INSERT INTO #OperationHours([name],[attribute],[uniqueId],[value])
			SELECT CASE WHEN LEN(CONVERT(NVARCHAR(2),([Hour]*3))) < 2 THEN 
							'0' + CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						ELSE CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						END AS [Time],[attribute],[uniqueId],[value]
			FROM ( 
				SELECT DATEPART(HOUR,[date])/3 AS [Hour],[attribute],I.[uniqueId],SUM([sum]) AS [value] 
				FROM #ids I 
				LEFT JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId] AND E.[isDeleted] = 0
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid] 
				WHERE [attribute] = 'trip' AND CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY DATEPART(HOUR,[date])/3,[attribute],I.[uniqueId]
				) [data]

			INSERT INTO #finalTable([name],[uniqueId])
			SELECT [weekDay],[uniqueId]
			FROM #weekDays
			CROSS JOIN #ids

			UPDATE #finalTable
			SET [value] = o.[value]
			FROM #finalTable f
			INNER JOIN #OperationHours o ON f.[uniqueId] = o.[uniqueId] AND o.[name] = F.[name]

			SELECT * FROM (
			SELECT O.[name] , 'Trip' AS [attribute] , E.[name] AS [elevatorName] , ISNULL([value],0) [value]
			FROM #finalTable O 
			inner join [Elevator] E (nolock) ON O.[uniqueId] = E.[uniqueId] AND E.[isDeleted] = 0
			) A
			ORDER BY 
				CASE [name] 
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
		END
		ELSE IF @frequency = 'W'
		BEGIN
			SET @endDate = DATEADD(DAY,-7,@dt)
			
			INSERT INTO [#months]
			SELECT CONVERT(DATE, DATEADD(DAY, (T.i - 6), @dt)) AS [Date]
			FROM (VALUES (6), (5), (4), (3), (2), (1), (0)) AS T(i)

			INSERT INTO #OperationHours([date],[attribute],[uniqueId],[value])
			SELECT CONVERT(DATE,[date]) AS [Day],[attribute],I.[uniqueId],SUM([sum]) AS [value] 
				FROM #ids I 
				LEFT JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId] AND E.[isDeleted] = 0
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid] 
				WHERE [attribute] = 'trip' AND CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY CONVERT(DATE,[date]),[attribute],I.[uniqueId]

			INSERT INTO #finalTable([Date],[uniqueId])
			SELECT [Date] , I.[uniqueId]
			FROM #months M
			CROSS JOIN #ids I

			UPDATE F
			SET [value] = ISNULL(o.[value],0)
			FROM #finalTable F
			INNER JOIN #OperationHours o ON F.[uniqueId] = o.[uniqueId] AND o.[date] = F.[date]

			SELECT CONCAT(DATENAME(day, M.[date]), ' - ', FORMAT( M.[date], 'ddd')) AS [name]
				, 'Trip' AS [attribute] , E.[name] AS [elevatorName] , ISNULL(M.[value],0) [value]
			FROM #finalTable M 
			left join [Elevator] E (nolock) ON M.[uniqueId] = E.[uniqueId] AND E.[isDeleted] = 0
			ORDER BY  M.[date]

		END
		ELSE
		BEGIN
			SET @endDate = DATEADD(YEAR,-1,@dt)
			
			INSERT INTO [#months]
			SELECT CONVERT(DATE, DATEADD(Month, (T.i - 11), @dt)) AS [Date]
			FROM (VALUES (11), (10), (9), (8), (7), (6), (5), (4), (3), (2), (1), (0)) AS T(i)
			
			INSERT INTO #OperationHours([Year],[Month],[attribute],[uniqueId],[value])
			SELECT DATEPART(YY,[date]) AS [Year], DATEPART(MM,[date]) AS [Month],[attribute],I.[uniqueId],SUM([sum]) AS [value] 
				FROM #ids I 
				LEFT JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId] AND E.[isDeleted] = 0
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid] 
				WHERE [attribute] = 'trip' AND CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY DATEPART(YY,[date]), DATEPART(MM,[date]), [attribute],I.[uniqueId]

			INSERT INTO #finalTable([Date],[uniqueId])
			SELECT [Date] , I.[uniqueId]
			FROM #months M
			CROSS JOIN #ids I

			UPDATE F
			SET [value] = ISNULL(o.[value],0)
			FROM #finalTable F
			INNER JOIN #OperationHours o ON F.[uniqueId] = o.[uniqueId] AND O.[Month] = DATEPART(MM, F.[date]) AND O.[Year] = DATEPART(YY, F.[date]) 

			SELECT SUBSTRING(DATENAME(MONTH, M.[date]), 1, 3) + '-' + FORMAT(M.[date],'yy') AS [name]
				, 'Trip' AS [attribute] , E.[name] AS [elevatorName] , ISNULL(M.[value],0) [value]
			FROM #finalTable M 
			left join [Elevator] E (nolock) ON M.[uniqueId] = E.[uniqueId] AND E.[isDeleted] = 0
			ORDER BY  M.[date]
		END
		
        SET @output = 1
		SET @fieldname = 'Success'   
              
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