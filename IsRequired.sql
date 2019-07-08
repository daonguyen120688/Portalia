Go
USE Portalia;
ALTER TABLE [dbo].[AttributeDetail]
ADD IsRequired bit
GO
USE Portalia;

update [dbo].[AttributeDetail] set IsRequired = 0

update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 1
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 2
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 3
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 5
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 8
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 14
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 16
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 17
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 18
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 20
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 26
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 27
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 28
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 30
update [dbo].[AttributeDetail] set IsRequired = 1 where AttributeDetailId = 31

