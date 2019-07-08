USE Portalia
GO

CREATE TABLE [dbo].[City]
(
 [Id]           int IDENTITY (1, 1) NOT NULL ,
 [Name]         nvarchar(250) NULL ,
 [NameASCII]    nvarchar(250) NULL ,
 [Country]      varchar(100) NULL ,
 [CountryCode2] varchar(2) NULL ,
 [CountryCode3] varchar(3) NULL ,
 [Lat]          varchar(50) NULL ,
 [Long]         varchar(50) NULL ,

 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
















