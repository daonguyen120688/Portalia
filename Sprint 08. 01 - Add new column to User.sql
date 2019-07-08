--INTE: isqlp01e1\portalia,9153
--QA: QSSQL11E1\PORTALIA,9155
--PROD: PSQLP01E1\PORTALIA,9155

USE Portalia;
GO

ALTER TABLE [dbo].[AspNetUsers]
ADD [HasChangedPassword] BIT NOT NULL DEFAULT 0

ALTER TABLE [dbo].[AspNetUsers]
ADD [CanSeeWelcomeCards] BIT NOT NULL DEFAULT 1

GO

UPDATE [dbo].[AspNetUsers]
SET [CanSeeWelcomeCards] = 0

GO