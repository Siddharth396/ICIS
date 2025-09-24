DECLARE @currDate DATETIME = getdate();
DECLARE @firstDayOfMonth DATETIME = DATEADD(month, DATEDIFF(month, 0, @currDate), 0),
	    @lastDayOf13Month DATETIME = DATEADD(day, -1, DATEADD(month, 13, DATEADD(month, DATEDIFF(month, 0, @currDate), 0)));
/*DECLARE @products NVARCHAR(MAX) = 'Styrene',
        @regions NVARCHAR(MAX) = 'Europe, China, Asia';*/

SELECT CONCAT(CONVERT(VARCHAR, START_DATE_BASE, 106), ' (', fSrc.[Description], ')') 'OutageStart',
	   CONCAT(CONVERT(VARCHAR, END_DATE_BASE, 106), ' (', tSrc.[Description], ')') 'OutageEnd',
	   dbo.fn_PascalCase(ctry.[Description]) 'Country',
	   dbo.fn_PascalCase(co.[Description]) 'Company',
	   dbo.fn_PascalCase(s.[Description]) 'Site',
	   p.PlantNumber 'PlantNo',
	   CASE
			WHEN ort.[Description] = 'Planned reason'
				THEN 'Scheduled'
			WHEN lav.IsForceMajeureDeclared  = 0
				THEN 'Unscheduled'
			WHEN lav.IsForceMajeureDeclared = 1
				THEN 'Unscheduled (Force Majeure)'
	   END Cause,    
	   CONCAT(OutagePercentage,
			  '% (est. ',
			  CAST(CAPACITY_LOSS_BASE AS DECIMAL(8,1)),
			  'kt)'
			  ) 'CapacityLoss',
	   CAST(pv.EOYCapacity AS DECIMAL(7,2)) TotalAnnualCapacity,
	   CONVERT(VARCHAR, lav.EditedDate, 106) LastUpdated,
	   ISNULL(ovc.Comment, '--') Comments
  FROM IDDN.OutageBestView obv
	   INNER JOIN ENUM.OutageInformationSourceType fSrc ON obv.SourceStartDate = fSrc.ID
	   INNER JOIN ENUM.OutageInformationSourceType tSrc ON obv.SourceEndDate = tSrc.ID
	   INNER JOIN Plant p ON obv.PlantID = p.ID
	   INNER JOIN Product pr ON p.ProductID = pr.ID
	   INNER JOIN Country ctry ON p.CountryID = ctry.ID
	   INNER JOIN AreaCountryMapping acm ON ctry.ID = acm.CountryID
	   INNER JOIN Area r ON acm.AreaID = r.ID
						AND r.[Description] LIKE 'Order_%'
	   INNER JOIN PhysicalPlantOwnership ppo ON p.PhysicalPlantID = ppo.PhysicalPlantID
											AND YEAR(obv.START_DATE_BASE) = ppo.YearID
	   INNER JOIN Company co ON ppo.CompanyID = co.ID
	   INNER JOIN [Site] s ON p.SiteID = s.ID
	   INNER JOIN STDC.LatestActiveOutageVersion lav ON obv.OutageID = lav.OutageID
	   INNER JOIN (SELECT *,
						  ROW_NUMBER() OVER (PARTITION BY OutageVersionID 
					ORDER BY OutageInformationSourceTypeID DESC) rn
					 FROM STDC.OutageVersionDetail
					WHERE OutageReasonID Is NOT NULL) ovd ON lav.ID = ovd.OutageVersionID 
														 AND ovd.rn = 1
	   INNER JOIN STDC.OutageReason orsn ON ovd.OutageReasonID = orsn.ID
	   INNER JOIN ENUM.OutageReasonType ort ON orsn.OutageReasonTypeId = ort.ID
	    LEFT JOIN STDC.OutageVersionComment ovc ON ovd.OutageVersionID = ovc.OutageVersionID
											   AND ovc.CommentType = 3
	   INNER JOIN PlantValue pv ON obv.PlantID = pv.PlantID
							   AND YEAR(obv.START_DATE_BASE) = pv.YearID
							   AND pv.[Type] = 1
 WHERE obv.IsDeleted = 0 AND
	   pr.[Description] IN (SELECT TRIM(value) FROM STRING_SPLIT(@products, ',') WHERE LEN(value) > 0) AND
	   r.[Description] IN (SELECT CONCAT('Order_', TRIM(value)) FROM STRING_SPLIT(@regions, ',') WHERE LEN(value) > 0) AND
	   ((START_DATE_BASE <= @firstDayOfMonth AND END_DATE_BASE >= @lastDayOf13Month)
		OR (START_DATE_BASE BETWEEN @firstDayOfMonth AND @lastDayOf13Month)
	    OR (END_DATE_BASE BETWEEN @firstDayOfMonth AND @lastDayOf13Month))
 ORDER BY START_DATE_BASE