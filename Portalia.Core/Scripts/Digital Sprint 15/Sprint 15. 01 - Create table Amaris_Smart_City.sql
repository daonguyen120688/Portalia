USE Portalia
GO

CREATE TABLE [dbo].[Amaris_Smart_City]
(
 [ID]             int IDENTITY (1, 1) NOT NULL ,
 [Name]           nvarchar(max) NOT NULL ,
 [CountryId]      varchar(3) NULL ,
 [LocationId]     int NULL ,
 [RegionId]       int NULL ,
 [IsAmarisCity]   bit NULL ,
 [Latitude]       float NULL ,
 [Longitude]      float NULL ,
 [GooglePlaceId]  varchar(400) NULL ,
 [TimezoneId]     int NULL ,
 [HelpPlaceId]    varchar(400) NULL ,
 [GeocodePlaceId] varchar(400) NULL ,
 [Valid]          bit NOT NULL ,

 CONSTRAINT [PK_Amaris_Smart_City] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO








