CREATE OR ALTER PROCEDURE pGetThreeClosestBranches (@lat float, @lng float)
AS
BEGIN
    SELECT TOP 3 
        Id,
        Street,
        City,
        Region,
        Latitude,
        Longitude,
        SQRT(POWER(Latitude - @lat, 2) + POWER(Longitude - @lng, 2)) * 100 as Distance -- Convert to kilometers
    FROM Branches
    ORDER BY SQRT(POWER(Latitude - @lat, 2) + POWER(Longitude - @lng, 2))
END
GO