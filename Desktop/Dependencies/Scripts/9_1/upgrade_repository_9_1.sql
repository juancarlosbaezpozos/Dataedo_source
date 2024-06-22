-- Version਍䤀䘀ഀഀ
  (਍   匀䔀䰀䔀䌀吀 䌀伀唀一吀⠀⨀⤀ഀഀ
     FROM [version]਍     圀䠀䔀刀䔀 嬀瘀攀爀猀椀漀渀崀 㴀 㤀ഀഀ
           AND [update] = 1਍           䄀一䐀 嬀爀攀氀攀愀猀攀崀 㴀 　ഀഀ
  ) = 0਍    䈀䔀䜀䤀一ഀഀ
        INSERT INTO [version]਍        ⠀嬀瘀攀爀猀椀漀渀崀Ⰰഀഀ
         [update],਍         嬀爀攀氀攀愀猀攀崀Ⰰഀഀ
         [stable]਍        ⤀ഀഀ
        VALUES਍        ⠀㤀Ⰰഀഀ
         1,਍         　Ⰰഀഀ
         0਍        ⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 昀攀攀搀戀愀挀欀ഀഀ
਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀崀⠀ഀഀ
	[feedback_id] [int] IDENTITY(1,1) NOT NULL,਍ऀ嬀甀猀攀爀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
	[type] [nvarchar](100) NOT NULL,਍ऀ嬀挀漀洀洀攀渀琀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
	[rating] [tinyint] NULL,਍ऀ嬀爀攀猀漀氀瘀攀搀崀 嬀戀椀琀崀 一唀䰀䰀Ⰰഀഀ
	[creation_date] [datetime] NOT NULL,਍ऀ嬀挀爀攀愀琀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
	[last_modification_date] [datetime] NOT NULL,਍ऀ嬀洀漀搀椀昀椀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
	[source_id] [int] NULL,਍ 䌀伀一匀吀刀䄀䤀一吀 嬀倀䬀开昀攀攀搀戀愀挀欀崀 倀刀䤀䴀䄀刀夀 䬀䔀夀 䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍ऀ嬀昀攀攀搀戀愀挀欀开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀 吀䔀堀吀䤀䴀䄀䜀䔀开伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback] ADD  CONSTRAINT [DF_feedback_creation_date]  DEFAULT (getdate()) FOR [creation_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开昀攀攀搀戀愀挀欀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback] ADD  CONSTRAINT [DF_feedback_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开昀攀攀搀戀愀挀欀开洀漀搀椀昀椀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀洀漀搀椀昀椀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback] ADD  CONSTRAINT [DF_feedback_type]  DEFAULT ('COMMENT') FOR [type]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开昀攀攀搀戀愀挀欀开氀椀挀攀渀猀攀猀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀甀猀攀爀开椀搀崀⤀ഀഀ
REFERENCES [licenses] ([license_id])਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开昀攀攀搀戀愀挀欀开氀椀挀攀渀猀攀猀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_user_id] ON [feedback]਍⠀ഀഀ
	[user_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_creation_date] ON [feedback]਍⠀ഀഀ
	[creation_date] DESC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_last_modification_date] ON [feedback]਍⠀ഀഀ
	[last_modification_date] DESC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_resolved] ON [feedback]਍⠀ഀഀ
	[resolved] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
GRANT DELETE ON [feedback] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT INSERT ON [feedback] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT SELECT ON [feedback] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT UPDATE ON [feedback] TO [users] AS [dbo];਍䜀伀ഀഀ
਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Author:		Paweł Kwarciński਍ⴀⴀ 䌀爀攀愀琀攀 搀愀琀攀㨀 ㈀　㈀㄀ⴀ　㈀ⴀ㄀㄀ഀഀ
-- Description:	Updates last_modification_date, modified_by columns on insert or update਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
CREATE TRIGGER [trg_feedback_Modify]਍   伀一  嬀昀攀攀搀戀愀挀欀崀ഀഀ
   AFTER INSERT,UPDATE਍䄀匀 ഀഀ
BEGIN਍     唀倀䐀䄀吀䔀 嬀昀攀攀搀戀愀挀欀崀ഀഀ
     SET [last_modification_date] = GETDATE(),਍           嬀洀漀搀椀昀椀攀搀开戀礀崀 㴀 猀甀猀攀爀开猀渀愀洀攀⠀⤀ഀഀ
     WHERE [feedback_id] IN (SELECT DISTINCT [feedback_id] FROM Inserted)਍䔀一䐀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback] ENABLE TRIGGER [trg_feedback_Modify]਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀ഀഀ
਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀⠀ഀഀ
	[comment_id] [int] IDENTITY(1,1) NOT NULL,਍    嬀昀攀攀搀戀愀挀欀开椀搀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
	[user_id] [int] NULL,਍ऀ嬀愀挀琀椀漀渀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　　⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
	[comment] [nvarchar](max) NULL,਍    嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀 嬀搀愀琀攀琀椀洀攀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
	[created_by] [nvarchar](1024) NULL,਍ऀ嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 嬀搀愀琀攀琀椀洀攀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
	[modified_by] [nvarchar](1024) NULL,਍ऀ嬀猀漀甀爀挀攀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
 CONSTRAINT [PK_feedback_comments] PRIMARY KEY CLUSTERED ਍⠀ഀഀ
	[comment_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 䤀䜀一伀刀䔀开䐀唀倀开䬀䔀夀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀开挀爀攀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_comments] ADD  CONSTRAINT [DF_feedback_comments_created_by]  DEFAULT (suser_sname()) FOR [created_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_comments] ADD  CONSTRAINT [DF_feedback_comments_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀开愀挀琀椀漀渀崀  䐀䔀䘀䄀唀䰀吀 ⠀✀䌀伀䴀䴀䔀一吀✀⤀ 䘀伀刀 嬀愀挀琀椀漀渀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_comments]  WITH CHECK ADD  CONSTRAINT [FK_feedback_comments_feedback] FOREIGN KEY([feedback_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀昀攀攀搀戀愀挀欀崀 ⠀嬀昀攀攀搀戀愀挀欀开椀搀崀⤀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_comments] CHECK CONSTRAINT [FK_feedback_comments_feedback]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀开氀椀挀攀渀猀攀猀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀甀猀攀爀开椀搀崀⤀ഀഀ
REFERENCES [licenses] ([license_id])਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀开氀椀挀攀渀猀攀猀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_feedback_id] ON [feedback_comments]਍⠀ഀഀ
	[feedback_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_user_id] ON [feedback_comments]਍⠀ഀഀ
	[user_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_creation_date] ON [feedback_comments]਍⠀ഀഀ
	[creation_date] DESC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_last_modification_date] ON [feedback_comments]਍⠀ഀഀ
	[last_modification_date] DESC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
GRANT DELETE ON [feedback_comments] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT INSERT ON [feedback_comments] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT SELECT ON [feedback_comments] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT UPDATE ON [feedback_comments] TO [users] AS [dbo];਍䜀伀ഀഀ
਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Author:		Paweł Kwarciński਍ⴀⴀ 䌀爀攀愀琀攀 搀愀琀攀㨀 ㈀　㈀㄀ⴀ　㈀ⴀ㄀㄀ഀഀ
-- Description:	Updates last_modification_date, modified_by columns on insert or update਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
CREATE TRIGGER [trg_feedback_comments_Modify]਍   伀一  嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀ഀഀ
   AFTER INSERT,UPDATE਍䄀匀 ഀഀ
BEGIN਍     唀倀䐀䄀吀䔀 嬀昀攀攀搀戀愀挀欀开挀漀洀洀攀渀琀猀崀ഀഀ
     SET [last_modification_date] = GETDATE(),਍           嬀洀漀搀椀昀椀攀搀开戀礀崀 㴀 猀甀猀攀爀开猀渀愀洀攀⠀⤀ഀഀ
     WHERE [comment_id] IN (SELECT DISTINCT [comment_id] FROM Inserted)਍䔀一䐀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_comments] ENABLE TRIGGER [trg_feedback_comments_Modify]਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 昀攀攀搀戀愀挀欀开氀椀渀欀猀ഀഀ
਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀⠀ഀഀ
	[link_id] [int] IDENTITY(1,1) NOT NULL,਍    嬀昀攀攀搀戀愀挀欀开椀搀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
	[object_type] [nvarchar](100) NOT NULL,਍    嬀漀戀樀攀挀琀开椀搀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
	[creation_date] [datetime] NOT NULL,਍ऀ嬀挀爀攀愀琀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
	[last_modification_date] [datetime] NOT NULL,਍ऀ嬀洀漀搀椀昀椀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
	[source_id] [int] NULL,਍ 䌀伀一匀吀刀䄀䤀一吀 嬀倀䬀开昀攀攀搀戀愀挀欀开氀椀渀欀猀崀 倀刀䤀䴀䄀刀夀 䬀䔀夀 䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍ऀ嬀氀椀渀欀开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_links] ADD  CONSTRAINT [DF_feedback_links_creation_date]  DEFAULT (getdate()) FOR [creation_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开昀攀攀搀戀愀挀欀开氀椀渀欀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_links] ADD  CONSTRAINT [DF_feedback_links_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开昀攀攀搀戀愀挀欀开氀椀渀欀猀开洀漀搀椀昀椀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀洀漀搀椀昀椀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_links]  WITH CHECK ADD  CONSTRAINT [FK_feedback_links_feedback] FOREIGN KEY([feedback_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀昀攀攀搀戀愀挀欀崀 ⠀嬀昀攀攀搀戀愀挀欀开椀搀崀⤀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_links] CHECK CONSTRAINT [FK_feedback_links_feedback]਍䜀伀ഀഀ
਍䌀刀䔀䄀吀䔀 一伀一䌀䰀唀匀吀䔀刀䔀䐀 䤀一䐀䔀堀 嬀䤀堀开昀攀攀搀戀愀挀欀开椀搀崀 伀一 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀ഀഀ
(਍ऀ嬀昀攀攀搀戀愀挀欀开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍䜀伀ഀഀ
਍䌀刀䔀䄀吀䔀 一伀一䌀䰀唀匀吀䔀刀䔀䐀 䤀一䐀䔀堀 嬀䤀堀开挀爀攀愀琀椀漀渀开搀愀琀攀崀 伀一 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀ഀഀ
(਍ऀ嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀 䐀䔀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍䜀伀ഀഀ
਍䌀刀䔀䄀吀䔀 一伀一䌀䰀唀匀吀䔀刀䔀䐀 䤀一䐀䔀堀 嬀䤀堀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 伀一 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀ഀഀ
(਍ऀ嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 䐀䔀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍䜀伀ഀഀ
਍䌀刀䔀䄀吀䔀 一伀一䌀䰀唀匀吀䔀刀䔀䐀 䤀一䐀䔀堀 嬀䤀堀开漀戀樀攀挀琀开琀礀瀀攀开漀戀樀攀挀琀开椀搀崀 伀一 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀ഀഀ
(਍ऀ嬀漀戀樀攀挀琀开琀礀瀀攀崀 䄀匀䌀Ⰰഀഀ
    [object_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
GRANT DELETE ON [feedback_links] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT INSERT ON [feedback_links] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT SELECT ON [feedback_links] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT UPDATE ON [feedback_links] TO [users] AS [dbo];਍䜀伀ഀഀ
਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Author:		Paweł Kwarciński਍ⴀⴀ 䌀爀攀愀琀攀 搀愀琀攀㨀 ㈀　㈀㄀ⴀ　㈀ⴀ㄀㄀ഀഀ
-- Description:	Updates last_modification_date, modified_by columns on insert or update਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
CREATE TRIGGER [trg_feedback_links_Modify]਍   伀一  嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀ഀഀ
   AFTER INSERT,UPDATE਍䄀匀 ഀഀ
BEGIN਍     唀倀䐀䄀吀䔀 嬀昀攀攀搀戀愀挀欀开氀椀渀欀猀崀ഀഀ
     SET [last_modification_date] = GETDATE(),਍           嬀洀漀搀椀昀椀攀搀开戀礀崀 㴀 猀甀猀攀爀开猀渀愀洀攀⠀⤀ഀഀ
     WHERE [link_id] IN (SELECT DISTINCT [link_id] FROM Inserted)਍䔀一䐀ഀഀ
GO਍ഀഀ
ALTER TABLE [feedback_links] ENABLE TRIGGER [trg_feedback_links_Modify]਍䜀伀ഀഀ
਍ⴀⴀ 䘀攀攀搀戀愀挀欀 ⴀ 挀漀氀甀洀渀猀ഀഀ
ALTER TABLE [columns]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀漀氀甀洀渀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀漀氀甀洀渀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀漀氀甀洀渀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀漀氀甀洀渀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀挀漀氀甀洀渀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [columns] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀漀氀甀洀渀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [columns]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [columns]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [columns]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Feedback - tables਍ഀഀ
ALTER TABLE [tables]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀愀戀氀攀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀愀戀氀攀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀愀戀氀攀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀愀戀氀攀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀琀愀戀氀攀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [tables]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [tables]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [tables]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Feedback - parameters਍ഀഀ
ALTER TABLE [parameters]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀愀爀愀洀攀琀攀爀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀愀爀愀洀攀琀攀爀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀愀爀愀洀攀琀攀爀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀愀爀愀洀攀琀攀爀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀愀爀愀洀攀琀攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [parameters]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [parameters]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [parameters]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Feedback - procedures਍ഀഀ
ALTER TABLE [procedures]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀爀漀挀攀搀甀爀攀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀爀漀挀攀搀甀爀攀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀爀漀挀攀搀甀爀攀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开瀀爀漀挀攀搀甀爀攀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [procedures] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [procedures]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [procedures]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [procedures]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Feedback - triggers਍ഀഀ
ALTER TABLE [triggers]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀爀椀最最攀爀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀爀椀最最攀爀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀爀椀最最攀爀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀爀椀最最攀爀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀琀爀椀最最攀爀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [triggers]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [triggers]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [triggers]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Feedback - gloassary terms਍ഀഀ
ALTER TABLE [glossary_terms]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [glossary_terms]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [glossary_terms]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [glossary_terms]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Feedback - databases਍ഀഀ
ALTER TABLE [databases]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开搀愀琀愀戀愀猀攀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开搀愀琀愀戀愀猀攀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开搀愀琀愀戀愀猀攀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开搀愀琀愀戀愀猀攀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀搀愀琀愀戀愀猀攀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [databases] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [databases]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [databases]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [databases]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Feedback - modules਍ഀഀ
ALTER TABLE [modules]਍䄀䐀䐀 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [rating] [decimal](3, 2) NULL,਍    嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_warnings_count] [int] NULL,਍    嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_questions_count] [int] NULL,਍    嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
    [open_todos_count] [int] NULL,਍    嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_comments_count]  DEFAULT ((0)) FOR [comments_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开洀漀搀甀氀攀猀开爀愀琀椀渀最崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀爀愀琀椀渀最崀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_rating_count]  DEFAULT ((0)) FOR [rating_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开洀漀搀甀氀攀猀开漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_warnings_count]  DEFAULT ((0)) FOR [warnings_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开洀漀搀甀氀攀猀开漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_questions_count]  DEFAULT ((0)) FOR [questions_count]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开洀漀搀甀氀攀猀开漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_todos_count]  DEFAULT ((0)) FOR [todos_count]਍䜀伀ഀഀ
਍唀倀䐀䄀吀䔀 嬀洀漀搀甀氀攀猀崀ഀഀ
SET [comments_count] = 0,਍ऀ嬀爀愀琀椀渀最崀 㴀 　Ⰰഀഀ
	[rating_count] = 0,਍ऀ嬀漀瀀攀渀开眀愀爀渀椀渀最猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[warnings_count] = 0,਍ऀ嬀漀瀀攀渀开焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[questions_count] = 0,਍ऀ嬀漀瀀攀渀开琀漀搀漀猀开挀漀甀渀琀崀 㴀 　Ⰰഀഀ
	[todos_count] = 0਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀挀漀洀洀攀渀琀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ALTER COLUMN [rating] [decimal](3, 2) NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀爀愀琀椀渀最开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ALTER COLUMN [open_warnings_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀眀愀爀渀椀渀最猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ALTER COLUMN [open_questions_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀焀甀攀猀琀椀漀渀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [modules] ALTER COLUMN [open_todos_count] [int] NOT NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀 䄀䰀吀䔀刀 䌀伀䰀唀䴀一 嬀琀漀搀漀猀开挀漀甀渀琀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [modules]਍⠀ഀഀ
	[open_warnings_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_questions_count] ON [modules]਍⠀ഀഀ
	[open_questions_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [modules]਍⠀ഀഀ
	[open_todos_count] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
-- Databases - columns਍ഀഀ
IF NOT EXISTS਍             ⠀ഀഀ
              SELECT *਍                䘀刀伀䴀 嬀猀礀猀崀⸀嬀挀漀氀甀洀渀猀崀ഀഀ
                WHERE [object_id] = OBJECT_ID(N'[databases]')਍                      䄀一䐀 嬀渀愀洀攀崀 㴀 ✀挀漀渀渀攀挀琀椀漀渀开爀漀氀攀✀ഀഀ
             )਍    䈀䔀䜀䤀一ഀഀ
        ALTER TABLE [databases]਍        䄀䐀䐀 嬀挀漀渀渀攀挀琀椀漀渀开爀漀氀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　　⤀ 䐀䔀䘀䄀唀䰀吀⠀一唀䰀䰀⤀ 一唀䰀䰀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍ⴀⴀ 唀猀攀爀 䌀漀渀渀攀挀琀椀漀渀猀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀ഀഀ
             (਍              匀䔀䰀䔀䌀吀 ⨀ഀഀ
                FROM [sys].[columns]਍                圀䠀䔀刀䔀 嬀漀戀樀攀挀琀开椀搀崀 㴀 伀䈀䨀䔀䌀吀开䤀䐀⠀一✀嬀甀猀攀爀开挀漀渀渀攀挀琀椀漀渀猀崀✀⤀ഀഀ
                      AND [name] = 'connection_role'਍             ⤀ഀഀ
    BEGIN਍        䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀甀猀攀爀开挀漀渀渀攀挀琀椀漀渀猀崀ഀഀ
        ADD [connection_role] [nvarchar](100) DEFAULT(NULL) NULL;਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
-- Version਍唀倀䐀䄀吀䔀 嬀瘀攀爀猀椀漀渀崀ഀഀ
  SET਍      嬀猀琀愀戀氀攀崀 㴀 ㄀ഀഀ
  WHERE [version] = 9਍        䄀一䐀 嬀甀瀀搀愀琀攀崀 㴀 ㄀ഀഀ
        AND [release] = 0;਍䜀伀�