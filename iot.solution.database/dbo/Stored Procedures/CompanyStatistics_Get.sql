/*******************************************************************
DECLARE @output INT = 0
		,@fieldName				nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[CompanyStatistics_Get]
	 @guid				= '2D442AEA-E58B-4E8E-B09B-5602E1AA545A'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
    ,@syncDate		= @syncDate		OUTPUT          
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 
 001	SSES-2 06-03-2020 [Nishit Khakhi]	Added Initial Version to Get Company Statistics
*******************************************************************/

CREATE PROCEDURE [dbo].[CompanyStatistics_Get]
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
            SELECT 'CompanyStatistics_Get' AS '@procName'
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
		;WITH CTE_Building
		AS (	SELECT [companyGuid], COUNT(1) [totalCount] 
				FROM [dbo].[Entity] (NOLOCK) 
				WHERE [companyGuid] = @guid AND [isDeleted] = 0 AND [parentEntityGuid] IS NULL 
				 and [Guid] not in (select entityGuid from [dbo].[Company] where [Guid]=@guid) 
				GROUP BY [companyGuid]
		)
		--,CTE_DeviceCount
		--AS (	SELECT [companyGuid]
		--				, SUM(CASE WHEN [isProvisioned] = 1 THEN 1 ELSE 0 END) [connectedDeviceCount] 
		--				, SUM(CASE WHEN [isProvisioned] = 0 THEN 1 ELSE 0 END) [disconnectedDeviceCount] 
		--		FROM [dbo].[Elevator] (NOLOCK) 
		--		WHERE [companyGuid] = @guid AND [isDeleted] = 0
		--		GROUP BY [companyGuid]
		--)
		,CTE_MaintenanceCount
		AS (	SELECT [companyGuid]
						, SUM(CASE WHEN [status] = 'Scheduled' THEN 1 ELSE 0 END) [scheduledCount] 
						, SUM(CASE WHEN [status] = 'Under Maintenance' THEN 1 ELSE 0 END) [underMaintenanceCount] 
				FROM [dbo].[ElevatorMaintenance] (NOLOCK) 
				WHERE [companyGuid] = @guid AND [isDeleted] = 0
				GROUP BY [companyGuid]
		)
		,CTE_AlertCount
		AS (	SELECT [companyGuid]
						, SUM(CASE WHEN [severity] = 'Critical' THEN 1 ELSE 0 END) [criticalAlertCount] 
						, SUM(CASE WHEN [severity] = 'Information' THEN 1 ELSE 0 END) [informationAlertCount] 
						, SUM(CASE WHEN [severity] = 'Major' THEN 1 ELSE 0 END) [majorAlertCount] 
						, SUM(CASE WHEN [severity] = 'Minor' THEN 1 ELSE 0 END) [minorAlertCount] 
						, SUM(CASE WHEN [severity] = 'Warning' THEN 1 ELSE 0 END) [warningAlertCount] 
				FROM [dbo].[IOTConnectAlert] (NOLOCK) 
				WHERE [companyGuid] = @guid
				GROUP BY [companyGuid]
		)
		,CTE_EnergyCount
		AS (	SELECT E.[companyGuid]
						, SUM([sum]) [energyCount]
				FROM [dbo].[TelemetrySummary_HourWise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE E.[companyGuid] = @guid AND [attribute] = 'consumption'
				GROUP BY E.[companyGuid]
		)
		,CTE_DeviceEnergyCount
		AS (	SELECT T.[deviceGuid]
						, SUM([sum]) [energyCount]
				FROM [dbo].[TelemetrySummary_HourWise] T (NOLOCK) 
				INNER JOIN [dbo].[Elevator] E (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE E.[companyGuid] = @guid AND [attribute] = 'consumption'
				GROUP BY T.[deviceGuid]
		)
		SELECT [guid]
				, ISNULL(L.[totalCount],0) AS [totalBuilding]
				--, ISNULL(D.[connectedDeviceCount],0) AS [totalConnectedDevices]
				--, ISNULL(D.[disconnectedDeviceCount],0) AS [totalDisconnectedDevices]
				, ISNULL(M.[scheduledCount],0) AS [totalScheduledCount]
				, ISNULL(M.[underMaintenanceCount],0) AS [totalUnderMaintenanceCount]
				, ISNULL(A.[criticalAlertCount],0) AS [criticalAlertCount]
				, ISNULL(A.[informationAlertCount],0) AS [informationAlertCount]
				, ISNULL(A.[criticalAlertCount],0) + ISNULL(A.[informationAlertCount],0) + ISNULL(A.[majorAlertCount],0) + ISNULL(A.[minorAlertCount],0) + ISNULL(A.[warningAlertCount],0) AS [totalAlert]
				, ISNULL(A.[majorAlertCount],0) AS [majorAlertCount]
				, ISNULL(A.[minorAlertCount],0) AS [minorAlertCount]
				, ISNULL(A.[warningAlertCount],0) AS [warningAlertCount]
				, ISNULL(E.[energyCount],0) AS [totalEnergyCount]
				, ISNULL(MinCount.[name],'') AS [minElevatorName]
				, ISNULL(MinCount.[minCount],0) AS [minElevatorEnergyCount]
				, ISNULL(MaxCount.[name],'') AS [maxElevatorName]
				, ISNULL(MaxCount.[maxCount],0) AS [maxElevatorEnergyCount]
		FROM [dbo].[Company] C (NOLOCK) 
		LEFT JOIN CTE_Building L ON C.[guid] = L.[companyGuid]
		--LEFT JOIN CTE_DeviceCount D ON C.[guid] = D.[companyGuid]
		LEFT JOIN CTE_MaintenanceCount M ON C.[guid] = M.[companyGuid]
		LEFT JOIN CTE_AlertCount A ON C.[guid] = A.[companyGuid]
		LEFT JOIN CTE_EnergyCount E ON C.[guid] = E.[companyGuid]
		LEFT JOIN 
			(SELECT	TOP	1 E.[companyGuid], 
				E.[name],
				MIN([energyCount]) as [minCount]
			 FROM CTE_DeviceEnergyCount CDE 
			 INNER JOIN [dbo].[Elevator] E (NOLOCK) ON CDE.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0
			 GROUP BY E.[companyGuid],E.[name]
			 ORDER BY [minCount] DESC 
			) MinCount ON MinCount.[companyGuid] = C.[guid]
		LEFT JOIN 
			(SELECT	TOP	1 E.[companyGuid], 
				E.[name],
				MAX([energyCount]) as [maxCount]
			 FROM CTE_DeviceEnergyCount CDE 
			 INNER JOIN [dbo].[Elevator] E (NOLOCK) ON CDE.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0
			 GROUP BY E.[companyGuid],E.[name]
			 ORDER BY [maxCount] DESC 
			) MaxCount ON MaxCount.[companyGuid] = C.[guid]
		WHERE C.[guid]=@guid AND C.[isDeleted]=0
		
		--INNER JOIN [dbo].[Entity] EN ON E.[entityGuid] = EN.[guid] ANd EN.[isDeleted] = 0
		--LEFT JOIN [dbo].[Entity] PE ON EN.[parentEntityGuid] = PE.[guid] AND PE.[isDeleted] = 0
			 

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