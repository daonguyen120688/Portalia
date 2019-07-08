use portalia
ALTER TABLE [dbo].[AspNetUsers]
ADD IsEmployee bit NOT NULL DEFAULT(0)
GO

ALTER TABLE [dbo].[AspNetUsers]
ADD IsNew bit NOT NULL DEFAULT(0)
GO