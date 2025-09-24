DECLARE @currDate DATETIME = GETDATE();
DECLARE @firstDayOfMonth DATETIME = DATEADD(month, DATEDIFF(month, 0, @currDate), 0),
	    @lastDayOf13Month DATETIME = DATEADD(day, -1, DATEADD(month, 13, DATEADD(month, DATEDIFF(month, 0, @currDate), 0)));

/*DECLARE @products NVARCHAR(MAX) = 'Styrene, Melamine',
        @regions NVARCHAR(MAX) = 'Europe, Asia';*/

WITH CountryRegion AS (
SELECT ctry.*
  FROM Country ctry
	   INNER JOIN AreaCountryMapping acm ON ctry.ID = acm.CountryID
	   INNER JOIN Area r ON acm.AreaID = r.ID
 WHERE r.[Description] IN (SELECT CONCAT('Order_', TRIM(value)) FROM STRING_SPLIT(@regions, ',') WHERE LEN(value) > 0)
),
PlantCapacity AS (
	SELECT pv.*,
		   LAG(pv.[Value]) OVER (PARTITION BY pv.PlantID
								 ORDER BY pv.YearID) PreviousValue,
		   LAG(pv.EOYCapacity) OVER (PARTITION BY pv.PlantID
									 ORDER BY pv.YearID) PreviousEOYCapacity,
		   LAG(pv.PlantStatusID) OVER (PARTITION BY pv.PlantID
										   ORDER BY pv.YearID) PreviousPlantStatus
	  FROM PlantValue pv
		   INNER JOIN Plant po ON pv.PlantID = po.ID
		   INNER JOIN Product pr ON po.ProductID = pr.ID
		   INNER JOIN CountryRegion cr ON po.CountryID = cr.ID
	 WHERE pv.[Type] = 1 AND
		   pv.YearID >= (YEAR(@firstDayOfMonth)-1) AND 
		   pv.YearID <= YEAR(@lastDayOf13Month) AND
		   pr.[Description] IN (SELECT TRIM(value) FROM STRING_SPLIT(@products, ',') WHERE LEN(value) > 0)
)

SELECT dbo.fn_PascalCase(ctry.[Description]) Country,
	   dbo.fn_PascalCase(co.[Description]) Company,
	   dbo.fn_PascalCase(s.[Description]) 'Site',
	   pp.PlantNumber PlantNo,
	   CASE
		WHEN pvt.PreviousEOYCapacity = 0 AND pvt.PreviousValue = 0
			THEN 'New Plant'
		WHEN (pvt.EOYCapacity - ISNULL(pvT.PreviousEOYCapacity, 0)) > 0
			THEN 'Expansion'
		WHEN pvt.PlantStatusID IN (6,7) OR pvt.EOYCapacity = 0
			THEN 'Permanent Closure'
		ELSE
			'Reduction'
	   END 'Type',
	   FORMAT(pvt.CapacityChangeDate, 'MMM yyyy') EstimatedStart,
	   CAST(pvt.EOYCapacity AS DECIMAL(7,2)) NewAnnualCapacity,
	   CAST((pvt.EOYCapacity - ISNULL(pvT.PreviousEOYCapacity, 0)) AS DECIMAL(7,2)) CapacityChange,
	   ISNULL((CAST((CAST((CASE
							WHEN pvT.PreviousEOYCapacity = 0
								THEN NULL
							ELSE
								((pvT.EOYCapacity - ISNULL(pvT.PreviousEOYCapacity, 0)) / pvT.PreviousEOYCapacity) * 100
						   END) AS DECIMAL(7,2))) AS NVARCHAR(7))), '100.00') PercentChange,
	   CONVERT(VARCHAR, COALESCE(pvt.EditedDate, pvt.CreatedDate), 106) LastUpdated
  FROM Plant po
	   INNER JOIN PhysicalPlant pp ON po.PhysicalPlantID = pp.ID
								  AND pp.StatusID = 1
	   INNER JOIN Product pr ON po.ProductID = pr.ID
	   INNER JOIN CountryRegion ctry ON po.CountryID = ctry.ID
	   INNER JOIN Company co ON pp.CompanyID = co.ID
	   INNER JOIN [Site] s ON po.SiteID = s.ID
	   INNER JOIN PlantCapacity pvt ON po.ID = pvT.PlantID AND
									  (pvt.PreviousEOYCapacity <> pvT.EOYCapacity OR
									   pvt.PreviousEOYCapacity IS NULL)
 WHERE po.StatusID = 1 AND
	   pr.[Description] IN (SELECT TRIM(value) FROM STRING_SPLIT(@products, ',') WHERE LEN(value) > 0) AND
	   pvt.CapacityChangeDate >= @firstDayOfMonth AND
	   pvt.CapacityChangeDate <= @lastDayOf13Month AND
	   pvt.YearID >= YEAR(@firstDayOfMonth) AND
	   pvt.YearID <= YEAR(@lastDayOf13Month) AND
	   pvt.PlantStatusID NOT IN (2,4) AND
       NOT (pvt.PlantStatusID IN (6, 7) AND pvt.PreviousPlantStatus IN (2, 4))
 ORDER BY pvt.CapacityChangeDate