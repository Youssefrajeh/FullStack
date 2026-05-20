-- Create Branches table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Branches](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Street] [nvarchar](250) NULL,
        [City] [nvarchar](150) NULL,
        [Region] [nvarchar](5) NULL,
        [Longitude] [float] NULL,
        [Latitude] [float] NULL,
        [Distance] [float] NULL,
        CONSTRAINT [PK_Branches] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create the stored procedure for finding closest branches
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