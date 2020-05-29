/*******************************************************************
DECLARE @output INT = 0
		,@fieldName				nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[BuildingStatistics_Get]
	 @guid				= '7548D722-D3B5-4D79-AE44-61B307DB979C'	
	,@frequency			= 'W'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	 ,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 
 001	SSES-2 30-03-2020 [Nishit Khakhi]	Added Initial Version to Get Building Statistics
*******************************************************************/

CREATE PROCEDURE [dbo].[BuildingStatistics_Get]
(	 @guid				UNIQUEIDENTIFIER	
	,@frequency			CHAR(1)				= NULL			
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT
	,@syncDate			DATETIME			OUTPUT
	,@culture			NVARCHAR(10)	  = 'en-Us'
	,@enableDebugInfo	CHAR(1)			  = '0'
)
AS
BEGIN
    SET NOCOUNT ON
	IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'BuildingStatistics_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'			
			, @frequency AS '@frequency'
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
		DECLARE @dt DATETIME = GETUTCDATE(), @endDate DATETIME
		SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
		IF ISNULL(@frequency,'') = 'D'
		BEGIN
			SET @endDate = @dt
		END
		ELSE IF ISNULL(@frequency,'') = 'W'
		BEGIN	
			SET @endDate = DATEADD(DAY,-7,@dt)
		END
		ELSE IF ISNULL(@frequency,'') = 'M'
		BEGIN
			SET @endDate = DATEADD(MONTH,-1,@dt)
		END	

		IF OBJECT_ID('tempdb..#Entity') IS NOT NULL BEGIN DROP TABLE #Entity END

		CREATE TABLE #Entity([guid] UNIQUEIDENTIFIER, [building]  UNIQUEIDENTIFIER, [deviceGuid] UNIQUEIDENTIFIER)
		
		INSERT INTO #Entity		
		SELECT EN.[guid], @guid AS [building], E.[guid] 
		FROM dbo.[Elevator] E (NOLOCK) 
		INNER JOIN dbo.[Entity] EN (NOLOCK) ON E.[entityGuid] = EN.[guid] AND EN.[isDeleted] = 0
		WHERE (EN.[guid] = @guid OR EN.[parentEntityGuid] = @guid)
			AND E.[isDeleted] = 0 

	--	SELECT * from #Entity
		IF @frequency IS NOT NULL
		BEGIN

		;WITH CTE_ElevatorCount
		AS (	SELECT EN.[building], COUNT(E.[guid]) AS [count], SUM(CASE WHEN E.[isprovisioned] = 1 THEN 1 ELSE 0 END) AS [TotalConnected]
				FROM [dbo].[Elevator] E (NOLOCK) 
				INNER JOIN #Entity EN ON E.[guid] = EN.[deviceGuid]
				GROUP BY EN.[building]
		)
		, CTE_EnergyCount
		AS (	SELECT EN.[building], SUM([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'consumption'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				WHERE CONVERT(Date,T.[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY EN.[building]
		), CTE_OperationgHoursCount
		AS (	SELECT EN.[building], SUM([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'trip'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				WHERE CONVERT(Date,T.[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY EN.[building]
		), CTE_TripCount
		AS (	SELECT EN.[building], AVG([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'trip'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				WHERE CONVERT(Date,T.[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY EN.[building]
		), CTE_Maintenance
		AS (	SELECT EN.[building], COUNT(1) AS [count]
				FROM [dbo].[ElevatorMaintenance] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[elevatorGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[status] = 'completed'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				WHERE CONVERT(Date,T.[createdDate]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY EN.[building]
		)
		, CTE_Alerts
		AS (	SELECT EN.[building], COUNT(1) AS [count]
				FROM [dbo].[IOTConnectAlert] T (NOLOCK) 
				INNER JOIN #Entity EN ON EN.[guid] = T.[entityGuid]
				WHERE CONVERT(Date,T.[eventDate]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY EN.[building]
		)
		SELECT E.[guid]
				, ISNULL(B.[count],0) AS [totalElevator]
				, ISNULL(B.[TotalConnected],0) AS [totalConnectedElevator]
				, ISNULL(EN.[count],0) AS [totalEnergy]
				, ISNULL(OH.[count],0) AS [totalOperatingHours]
				, ISNULL(TP.[count],0) AS [totalTrips]
				, ISNULL(CM.[count],0) AS [totalMaintenance]
				, ISNULL(CA.[count],0) AS [totalAlerts]
		FROM [dbo].[Entity] E (NOLOCK) 
		LEFT JOIN CTE_ElevatorCount B ON E.[guid] = B.[building]
		LEFT JOIN CTE_EnergyCount EN ON E.[guid] = EN.[building]
		LEFT JOIN CTE_OperationgHoursCount OH ON E.[guid] = OH.[building]
		LEFT JOIN CTE_TripCount TP ON E.[guid] = TP.[building]
		LEFT JOIN CTE_Maintenance CM ON E.[guid] = CM.[building]
		LEFT JOIN CTE_Alerts CA ON E.[guid] = CA.[building]
		WHERE E.[guid] = @guid AND E.[isDeleted]=0

		END
		ELSE
		BEGIN

		;WITH CTE_ElevatorCount
		AS (	SELECT EN.[building], COUNT(E.[guid]) AS [count], SUM(CASE WHEN E.[isprovisioned] = 1 THEN 1 ELSE 0 END) AS [TotalConnected]
				FROM [dbo].[Elevator] E (NOLOCK) 
				INNER JOIN #Entity EN ON E.[guid] = EN.[deviceGuid]
				GROUP BY EN.[building]
		)
		, CTE_EnergyCount
		AS (	SELECT EN.[building], SUM([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'consumption'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				GROUP BY EN.[building]
		), CTE_OperationgHoursCount
		AS (	SELECT EN.[building], SUM([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'trip'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				GROUP BY EN.[building]
		), CTE_TripCount
		AS (	SELECT EN.[building], SUM([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'trip'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				GROUP BY EN.[building]
		), CTE_Maintenance
		AS (	SELECT EN.[building], COUNT(1) AS [count]
				FROM [dbo].[ElevatorMaintenance] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[elevatorGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[status] = 'completed'
				INNER JOIN #Entity EN ON EN.[guid] = E.[entityGuid]
				GROUP BY EN.[building]
		)
		, CTE_Alerts
		AS (	SELECT EN.[building], COUNT(1) AS [count]
				FROM [dbo].[IOTConnectAlert] T (NOLOCK) 
				INNER JOIN #Entity EN ON EN.[guid] = T.[entityGuid]
				GROUP BY EN.[building]
		)
		SELECT E.[guid]
				, ISNULL(B.[count],0) AS [totalElevator]
				, ISNULL(B.[TotalConnected],0) AS [totalConnectedElevator]
				, ISNULL(EN.[count],0) AS [totalEnergy]
				, ISNULL(OH.[count],0) AS [totalOperatingHours]
				, ISNULL(TP.[count],0) AS [totalTrips]
				, ISNULL(CM.[count],0) AS [totalMaintenance]
				, ISNULL(CA.[count],0) AS [totalAlerts]
		FROM [dbo].[Entity] E (NOLOCK) 
		LEFT JOIN CTE_ElevatorCount B ON E.[guid] = B.[building]
		LEFT JOIN CTE_EnergyCount EN ON E.[guid] = EN.[building]
		LEFT JOIN CTE_OperationgHoursCount OH ON E.[guid] = OH.[building]
		LEFT JOIN CTE_TripCount TP ON E.[guid] = TP.[building]
		LEFT JOIN CTE_Maintenance CM ON E.[guid] = CM.[building]
		LEFT JOIN CTE_Alerts CA ON E.[guid] = CA.[building]
		WHERE E.[guid] = @guid AND E.[isDeleted]=0

		END

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