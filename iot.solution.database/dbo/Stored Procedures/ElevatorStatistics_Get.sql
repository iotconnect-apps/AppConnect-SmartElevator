
/*******************************************************************
DECLARE @output INT = 0
		,@fieldName				nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[ElevatorStatistics_Get]
	 @guid				= 'FA973382-0321-4701-A03E-CDDEAEC9F68B'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
    ,@syncDate		= @syncDate		OUTPUT          
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 
 001	SSES-2 03-04-2020 [Nishit Khakhi]	Added Initial Version to Get Building Statistics
*******************************************************************/

CREATE PROCEDURE [dbo].[ElevatorStatistics_Get]
(	 @guid				UNIQUEIDENTIFIER	
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT
	,@syncDate			DATETIME		  OUTPUT
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
            SELECT 'ElevatorStatistics_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'			
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
		 SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
		DECLARE @uniqueId NVARCHAR(500)

		SELECT TOP 1 @uniqueId = [uniqueId] FROM dbo.[Elevator] (NOLOCK) WHERE [guid] = @guid AND [isDeleted] = 0

		;WITH CTE_EnergyCount
		AS (	SELECT E.[uniqueId], SUM([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'consumption'
				WHERE E.[uniqueId] = @uniqueId
				GROUP BY E.[uniqueId]
		), CTE_OperationgHoursCount
		AS (	SELECT E.[uniqueId], SUM([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'trip'
				WHERE E.[uniqueId] = @uniqueId
				GROUP BY E.[uniqueId]
		), CTE_Trips
		AS (	SELECT E.[uniqueId], SUM(T.[sum]) AS [totalTrip], MIN(CAST([date] as DATE)) AS [minDate], MAX(CAST([date] as DATE)) AS [maxDate]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'trip'
				WHERE E.[uniqueId] = @uniqueId
				group by E.[uniqueId]
		), CTE_Temperature
		AS (	SELECT E.[uniqueId], AVG([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'temp'
				WHERE E.[uniqueId] = @uniqueId
				GROUP BY E.[uniqueId]
		)
		, CTE_Speed
		AS (	SELECT E.[uniqueId], AVG([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'rpm'
				WHERE E.[uniqueId] = @uniqueId
				GROUP BY E.[uniqueId]
		)
		, CTE_Vibration
		AS (	SELECT E.[uniqueId], AVG([sum]) AS [count]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0 AND T.[attribute] = 'vibration'
				WHERE E.[uniqueId] = @uniqueId
				GROUP BY E.[uniqueId]
		)
		SELECT E.[uniqueId]
			, ISNULL(EN.[count],0) AS [energyCount]
			, ISNULL(OHC.[count],0) AS [operatingHourCount]
			, ISNULL(CT.[count],0) AS [averageTemperature]
			, ISNULL(S.[count],0) AS [averageSpeed]
			, ISNULL(V.[count],0) AS [averageVibration]
			, ISNULL(ISNULL(T.[totalTrip],0) / CASE WHEN DATEDIFF(DD,T.[minDate],T.[maxDate]) = 0 THEN 1 ELSE DATEDIFF(DD,T.[minDate],T.[maxDate]) END ,0) AS [averageTrip]
			, CASE WHEN EM.[scheduledDate] IS NOT NULL AND EM.[scheduledDate] > GETUTCDATE() THEN 
				ISNULL(DATEDIFF(DD,GETUTCDATE(),EM.[scheduledDate]),0)
			  ELSE 0
			  END AS [day]
			, CASE WHEN EM.[scheduledDate] IS NOT NULL AND EM.[scheduledDate] > GETUTCDATE() THEN 
				ISNULL(DATEDIFF(HH,GETUTCDATE(),EM.[scheduledDate])%24,0) 
			  ELSE 0
			  END AS [hour]
			, CASE WHEN EM.[scheduledDate] IS NOT NULL AND EM.[scheduledDate] > GETUTCDATE() THEN 
				ISNULL(DATEDIFF(MINUTE,GETUTCDATE(),EM.[scheduledDate])%60,0) 
			  ELSE 0
			  END AS [minute]
		FROM [dbo].[Elevator] E (NOLOCK) 
		LEFT JOIN CTE_EnergyCount EN ON E.[uniqueId] = EN.[uniqueId]
		LEFT JOIN CTE_OperationgHoursCount OHC ON E.[uniqueId] = OHC.[uniqueId]
		LEFT JOIN CTE_Temperature CT ON E.[uniqueId] = CT.[uniqueId]
		LEFT JOIN CTE_Speed S ON E.[uniqueId] = S.[uniqueId]
		LEFT JOIN CTE_Vibration V ON E.[uniqueId] = V.[uniqueId]
		LEFT JOIN CTE_Trips T ON E.[uniqueId] = T.[uniqueId]
		LEFT JOIN dbo.[ElevatorMaintenance] EM (NOLOCK) ON E.[guid] = EM.[elevatorGuid] AND EM.[isDeleted] = 0 AND EM.[status] = 'Scheduled'
		--LEFT JOIN CTE_Maintenance CM ON E.[guid] = CM.[building]
		WHERE E.[guid] = @guid AND E.[isDeleted]=0


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