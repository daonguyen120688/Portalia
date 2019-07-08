
GO

USE [Portalia]
GO

/****** Object:  Schema [Auth]    Script Date: 07/01/2019 15:17:41 ******/
CREATE SCHEMA [Auth]
GO
/****** Object:  Table [Auth].[Migration]    Script Date: 07/01/2019 15:17:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Auth].[Migration](
	[OldAspNetUserId] [nvarchar](128) NOT NULL,
	[NewAuth0Id] [varchar](128) NOT NULL,
 CONSTRAINT [PK_Migration] PRIMARY KEY CLUSTERED 
(
	[OldAspNetUserId] ASC,
	[NewAuth0Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  StoredProcedure [Auth].[CreateMigration]    Script Date: 13/01/2019 15:54:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [Auth].[CreateMigration]
	@OldAspNetUserId nvarchar(128),
	@NewAuth0Id varchar(128)
AS

BEGIN TRANSACTION MigrateUser

ALTER TABLE [AspNetUsers] NOCHECK CONSTRAINT ALL
ALTER TABLE [UserProfile] NOCHECK CONSTRAINT ALL
ALTER TABLE [Proposal] NOCHECK CONSTRAINT ALL
ALTER TABLE [AspNetUserClaims] NOCHECK CONSTRAINT ALL
ALTER TABLE [AspNetUserLogins] NOCHECK CONSTRAINT ALL
ALTER TABLE [AspNetUserRoles] NOCHECK CONSTRAINT ALL
ALTER TABLE WorkContracts NOCHECK CONSTRAINT ALL

UPDATE [AspNetUsers]
SET [Id]=@NewAuth0Id, [UserName]=@NewAuth0Id
WHERE [Id]=@OldAspNetUserId

UPDATE [UserProfile]
SET [IdentityUserId]=@NewAuth0Id
WHERE [IdentityUserId]=@OldAspNetUserId

UPDATE [Proposal]
SET [UserId]=@NewAuth0Id
WHERE [UserId]=@OldAspNetUserId

UPDATE [AspNetUserClaims]
SET [UserId]=@NewAuth0Id
WHERE [UserId]=@OldAspNetUserId

UPDATE [AspNetUserLogins]
SET [UserId]=@NewAuth0Id
WHERE [UserId]=@OldAspNetUserId

UPDATE [AspNetUserRoles]
SET [UserId]=@NewAuth0Id
WHERE [UserId]=@OldAspNetUserId

UPDATE WorkContracts
SET [UserId]=@NewAuth0Id
WHERE [UserId]=@OldAspNetUserId

ALTER TABLE [AspNetUsers] CHECK CONSTRAINT ALL
ALTER TABLE [UserProfile] CHECK CONSTRAINT ALL
ALTER TABLE [Proposal] CHECK CONSTRAINT ALL
ALTER TABLE [AspNetUserClaims] CHECK CONSTRAINT ALL
ALTER TABLE [AspNetUserLogins] CHECK CONSTRAINT ALL
ALTER TABLE [AspNetUserRoles] CHECK CONSTRAINT ALL
ALTER TABLE WorkContracts CHECK CONSTRAINT ALL

INSERT INTO [Auth].[Migration] (OldAspNetUserId, NewAuth0Id)
VALUES (@OldAspNetUserId, @NewAuth0Id)

COMMIT TRANSACTION MigrateUser
GO
