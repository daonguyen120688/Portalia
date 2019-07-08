USE [Portalia]
GO


CREATE TABLE [dbo].[Skill]
(
 [SkillId]        int IDENTITY (1, 1) NOT NULL ,
 [Label]          nvarchar(1000) NULL ,
 [BusinessLineId] int NULL ,
 [Validated]      bit NULL ,
 [Created_Date]   datetime2(7) NULL ,
 [CreatedById]    int NULL ,

 CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED ([SkillId] ASC)
);
GO










