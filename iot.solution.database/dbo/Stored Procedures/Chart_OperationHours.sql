/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName					nvarchar(255)
	,@syncDate	DATETIME
EXEC [dbo].[Chart_OperationHours]	
	@companyguid = '493256B8-F657-4CF1-8F7E-6B1FD473FEC8'
	--,@guid	= 'C72E9BBB-FED3-4C14-B396-95177B09AFF6'
	,@frequency = 'M'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName, @syncDate syncDate

001	SSES-2 19-03-2020 [Nishit Khakhi]	Added Initial Version to represent Operating Hours by Building / Company
002	SSES-2 27-03-2020 [Nishit Khakhi]	Updated to return default month and week day name in return while for value
003 SSES-3 14-04-2020 [Nishit Khakhi]	Updated to return Day frequency and order by
*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_OperationHours]
(	@companyguid		UNIQUEIDENTIFIER		
	,@guid				UNIQUEIDENTIFIER	= NULL	
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
            SELECT 'Chart_OperationHoursByBuilding' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, CONVERT(nvarchar(MAX),@guid) AS '@guid' 
			, CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
    BEGIN TRY  
		DECLARE @dt DATETIME = GETUTCDATE(), @endDate DATETIME
		IF OBJECT_ID ('tempdb..#ids') IS NOT NULL DROP TABLE #ids
		IF OBJECT_ID('tempdb..#weekdays') IS NOT NULL BEGIN DROP TABLE #weekdays END
		IF OBJECT_ID('tempdb..#OperationHours') IS NOT NULL BEGIN DROP TABLE #OperationHours END
		IF OBJECT_ID('tempdb..#EnergyConsumption') IS NOT NULL BEGIN DROP TABLE #EnergyConsumption END
		IF OBJECT_ID('tempdb..#finalTable') IS NOT NULL BEGIN DROP TABLE #finalTable END
		IF OBJECT_ID ('tempdb..#months') IS NOT NULL BEGIN DROP TABLE #months END
		CREATE TABLE [#months] ([date] DATE)
		CREATE TABLE #weekdays ([weekDay] NVARCHAR(20))
		CREATE TABLE #OperationHours ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [OperatingHours] BIGINT) 
		CREATE TABLE #EnergyConsumption ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [EnergyConsumption] BIGINT) 
		CREATE TABLE #finalTable ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [OperatingHours] BIGINT, [EnergyConsumption] BIGINT) 

		SELECT E.[uniqueId] as [uniqueId]
		INTO #ids
		FROM [dbo].[Entity] PN (NOLOCK) 
		INNER JOIN [dbo].[Entity] CE (NOLOCK) ON CE.[parentEntityGuid] = PN.[guid] 
		INNER JOIN [dbo].[Elevator] E (NOLOCK) ON E.[entityGuid] = CE.[guid] 
		WHERE PN.[companyGuid] = @companyguid	AND PN.[guid] = ISNULL(@guid,PN.[guid])
		AND CE.isActive = 1 AND CE.isDeleted = 0 AND E.isDeleted = 0 AND E.isActive = 1

		IF @frequency = 'D'
		BEGIN
			SET @endDate = @dt
			INSERT INTO #weekdays values ('00:00'),('03:00'),('06:00'),('09:00'),('12:00'),('15:00'),('18:00'),('21:00')

			INSERT INTO #OperationHours ([name],[OperatingHours])
			SELECT CASE WHEN LEN(CONVERT(NVARCHAR(2),([Hour]*3))) < 2 THEN 
							'0' + CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						ELSE CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						END 
					AS [HH],[OperatingHours]
			FROM ( 
				SELECT DATEPART(HOUR,[date])/3 AS [Hour],SUM([sum]) AS [OperatingHours] 
				FROM #ids I 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId]
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE E.[companyGuid] = @companyguid AND [attribute] = 'trip' AND CONVERT(Date,[date]) = CONVERT(DATE,@dt)
				GROUP BY DATEPART(HOUR,[date])/3
				) [data]
			
			INSERT INTO #EnergyConsumption([name], [EnergyConsumption])
			SELECT CASE WHEN LEN(CONVERT(NVARCHAR(2),([Hour]*3))) < 2 THEN 
							'0' + CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						ELSE CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						END 
					AS [HH],[value] AS [EnergyConsumption]
			FROM ( 
				SELECT DATEPART(HOUR,[date])/3 AS [Hour],SUM([sum]) AS [value] 
				FROM #ids I 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId]
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE E.[companyGuid] = @companyguid AND [attribute] = 'consumption' AND CONVERT(Date,[date]) = CONVERT(DATE,@dt) 
				GROUP BY DATEPART(HOUR,[date])/3
				) [data]
			
		INSERT INTO #finalTable([name])
		SELECT [weekDay]
		FROM #weekDays
		CROSS JOIN #ids

		UPDATE F
		SET [EnergyConsumption] = E.[EnergyConsumption]
			, [OperatingHours] = O.[OperatingHours]
		FROM #finalTable F
		LEFT JOIN #EnergyConsumption E ON E.[name] = F.[name]
		LEFT JOIN #OperationHours O ON O.[name] = F.[name]

		END
		ELSE IF @frequency = 'W'
		BEGIN
			SET @endDate = DATEADD(DAY,-7,@dt)
			
			INSERT INTO [#months]
			SELECT CONVERT(DATE, DATEADD(DAY, (T.i - 6), @dt)) AS [Date]
			FROM (VALUES (6), (5), (4), (3), (2), (1), (0)) AS T(i)

			INSERT INTO #OperationHours ([date],[OperatingHours])
			SELECT CONVERT(DATE,[date]) AS [Day],SUM([sum]) AS [OperatingHours] 
				FROM #ids I 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId]
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE E.[companyGuid] = @companyguid AND [attribute] = 'trip' AND CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY CONVERT(DATE,[date])
			
			INSERT INTO #EnergyConsumption([date], [EnergyConsumption])
			SELECT CONVERT(DATE,[date]) AS [Day],SUM([sum]) AS [EnergyConsumption] 
				FROM #ids I 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId]
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE E.[companyGuid] = @companyguid AND [attribute] = 'consumption' AND CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY CONVERT(DATE,[date])
		END
		ELSE
		BEGIN
			SET @endDate = DATEADD(YEAR,-1,@dt)
			
			INSERT INTO [#months]
			SELECT CONVERT(DATE, DATEADD(Month, (T.i - 11), @dt)) AS [Date]
			FROM (VALUES (11), (10), (9), (8), (7), (6), (5), (4), (3), (2), (1), (0)) AS T(i)

			INSERT INTO #OperationHours ([Year],[Month],[OperatingHours])
			SELECT DATEPART(YY,[date]) AS [Year], DATEPART(MM,[date]) AS [Month],SUM([sum]) AS [OperatingHours] 
			FROM #ids I 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId]
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid]
			WHERE E.[companyGuid] = @companyguid AND [attribute] = 'trip' AND CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
			GROUP BY DATEPART(YY,[date]), DATEPART(MM,[date])
			
			INSERT INTO #EnergyConsumption([Year],[Month],[EnergyConsumption])
			SELECT DATEPART(YY,[date]) AS [Year], DATEPART(MM,[date]) AS [Month],SUM([sum]) AS [EnergyConsumption] 
			FROM #ids I 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON I.[uniqueId] = E.[uniqueId]
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = E.[guid]
			WHERE E.[companyGuid] = @companyguid AND [attribute] = 'consumption' AND CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
			GROUP BY DATEPART(YY,[date]), DATEPART(MM,[date]) 
		END
		
		IF @frequency = 'M'
		BEGIN
			SELECT SUBSTRING(DATENAME(MONTH, M.[date]), 1, 3) + '-' + FORMAT(M.[date],'yy') AS [name]
				, ISNULL(R.[EnergyConsumption],0) [EnergyConsumption]
				, ISNULL(O.[OperatingHours],0) [OperatingHours]
			FROM [#months] M
			LEFT OUTER JOIN #EnergyConsumption R ON R.[Month] = DATEPART(MM, M.[date]) AND R.[Year] = DATEPART(YY, M.[date]) 
			LEFT OUTER JOIN #OperationHours O ON O.[Month] = DATEPART(MM, M.[date]) AND O.[Year] = DATEPART(YY, M.[date]) 
			ORDER BY  M.[date]
		END
		ELSE IF @frequency = 'W'
		BEGIN
			SELECT CONCAT(DATENAME(day, M.[date]), ' - ', FORMAT( M.[date], 'ddd')) AS [name]
				, ISNULL(R.[EnergyConsumption],0) [EnergyConsumption]
				, ISNULL(O.[OperatingHours],0) [OperatingHours]
			FROM [#months] M
			LEFT OUTER JOIN #EnergyConsumption R ON R.[date] = M.[date]
			LEFT OUTER JOIN #OperationHours O ON O.[date] = M.[date]
			ORDER BY  M.[date]
		END
		ELSE
		BEGIN
			SELECT [name], SUM(OperatingHours) AS OperatingHours, SUM(EnergyConsumption) AS EnergyConsumption
			FROM (
			SELECT [name], ISNULL([OperatingHours],0) AS [OperatingHours], ISNULL([EnergyConsumption],0) AS [EnergyConsumption] FROM #finalTable
			UNION ALL
			SELECT *, 0, 0
			FROM #weekdays
			) A -- WHERE [weekDay] NOT IN (SELECT DISTINCT [name] FROM #OperationHours)
			GROUP BY [name]
			ORDER BY 
				CASE WHEN @frequency = 'D' THEN
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