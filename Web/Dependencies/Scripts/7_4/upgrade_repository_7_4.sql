-- Version਍䤀䘀ഀഀ
  (਍   匀䔀䰀䔀䌀吀 䌀伀唀一吀⠀⨀⤀ഀഀ
     FROM [version]਍     圀䠀䔀刀䔀 嬀瘀攀爀猀椀漀渀崀 㴀 㜀ഀഀ
           AND [update] = 4਍           䄀一䐀 嬀爀攀氀攀愀猀攀崀 㴀 　ഀഀ
  ) = 0਍    䈀䔀䜀䤀一ഀഀ
        INSERT INTO [version]਍        ⠀嬀瘀攀爀猀椀漀渀崀Ⰰഀഀ
         [update],਍         嬀爀攀氀攀愀猀攀崀Ⰰഀഀ
         [stable]਍        ⤀ഀഀ
        VALUES਍        ⠀㜀Ⰰഀഀ
         4,਍         　Ⰰഀഀ
         0਍        ⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍ⴀⴀ 䌀栀愀渀最椀渀最 搀攀昀愀甀氀琀猀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀开爀攀氀愀琀椀漀渀猀崀 䐀刀伀倀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀愀戀氀攀猀开爀攀氀愀琀椀漀渀猀开挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀开爀攀氀愀琀椀漀渀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀愀戀氀攀猀开爀攀氀愀琀椀漀渀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables_relations] DROP  CONSTRAINT [DF_tables_relations_modified_by]਍䜀伀ഀഀ
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䐀刀伀倀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀爀椀最最攀爀猀开挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀爀椀最最攀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开琀爀椀最最攀爀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers] DROP  CONSTRAINT [DF_triggers_modified_by]਍䜀伀ഀഀ
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀崀 䐀刀伀倀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀开挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [unique_constraints] DROP  CONSTRAINT [DF_unique_constraints_modified_by]਍䜀伀ഀഀ
ALTER TABLE [unique_constraints] ADD  CONSTRAINT [DF_unique_constraints_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀开挀漀氀甀洀渀猀崀 䐀刀伀倀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀开挀漀氀甀洀渀猀开挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀开挀漀氀甀洀渀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀开挀漀氀甀洀渀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [unique_constraints_columns] DROP  CONSTRAINT [DF_unique_constraints_columns_modified_by]਍䜀伀ഀഀ
ALTER TABLE [unique_constraints_columns] ADD  CONSTRAINT [DF_unique_constraints_columns_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀猀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 挀甀猀琀漀洀开昀椀攀氀搀猀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀 ⠀ഀഀ
    SELECT *਍    䘀刀伀䴀 猀礀猀⸀挀漀氀甀洀渀猀ഀഀ
    WHERE object_id = OBJECT_ID(N'[custom_fields]')਍        䄀一䐀 渀愀洀攀 㴀 ✀琀攀爀洀开瘀椀猀椀戀椀氀椀琀礀✀ഀഀ
)਍䈀䔀䜀䤀一ഀഀ
    ALTER TABLE [custom_fields]਍    䄀䐀䐀 嬀琀攀爀洀开瘀椀猀椀戀椀氀椀琀礀崀 嬀戀椀琀崀 一伀吀 一唀䰀䰀 䐀䔀䘀䄀唀䰀吀 ⠀⠀㄀⤀⤀ഀഀ
END਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀ഀഀ
਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀ഀഀ
([term_type_id]           [int] IDENTITY(1,1) NOT NULL,਍ 嬀琀椀琀氀攀崀                  渀瘀愀爀挀栀愀爀⠀㈀㔀　⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
 [icon_id]                [int] NULL,਍ 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀          搀愀琀攀琀椀洀攀 一伀吀 一唀䰀䰀Ⰰഀഀ
 [created_by]             nvarchar(1024) NULL,਍ 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 搀愀琀攀琀椀洀攀 一伀吀 一唀䰀䰀Ⰰഀഀ
 [modified_by]            nvarchar(1024) NULL,਍ 嬀猀漀甀爀挀攀开椀搀崀              嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
 CONSTRAINT [PK_glossary_term_types] PRIMARY KEY CLUSTERED([term_type_id] ASC)਍ 圀䤀吀䠀⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ䤀䜀一伀刀䔀开䐀唀倀开䬀䔀夀 㴀 伀䘀䘀Ⰰ䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
)਍伀一 嬀倀刀䤀䴀䄀刀夀崀㬀ഀഀ
GO਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀ഀഀ
ADD CONSTRAINT [DF_glossary_term_types_creation_date] DEFAULT GETDATE() FOR [creation_date];਍䜀伀ഀഀ
ALTER TABLE [glossary_term_types]਍䄀䐀䐀 䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀开挀爀攀愀琀攀搀开戀礀崀 䐀䔀䘀䄀唀䰀吀 匀唀匀䔀刀开匀一䄀䴀䔀⠀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀㬀ഀഀ
GO਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀ഀഀ
ADD CONSTRAINT [DF_glossary_term_types_last_modification_date] DEFAULT GETDATE() FOR [last_modification_date];਍䜀伀ഀഀ
ALTER TABLE [glossary_term_types]਍䄀䐀䐀 䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀开洀漀搀椀昀椀攀搀开戀礀崀 䐀䔀䘀䄀唀䰀吀 匀唀匀䔀刀开匀一䄀䴀䔀⠀⤀ 䘀伀刀 嬀洀漀搀椀昀椀攀搀开戀礀崀㬀ഀഀ
GO਍ഀഀ
GRANT DELETE ON [glossary_term_types] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT INSERT ON [glossary_term_types] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT SELECT ON [glossary_term_types] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT UPDATE ON [glossary_term_types] TO [users] AS [dbo];਍䜀伀ഀഀ
਍匀䔀吀 䤀䐀䔀一吀䤀吀夀开䤀一匀䔀刀吀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀 伀一 ഀഀ
GO਍䤀一匀䔀刀吀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀 ⠀嬀琀攀爀洀开琀礀瀀攀开椀搀崀Ⰰ 嬀琀椀琀氀攀崀Ⰰ 嬀椀挀漀渀开椀搀崀Ⰰ 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀挀爀攀愀琀攀搀开戀礀崀Ⰰ 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀洀漀搀椀昀椀攀搀开戀礀崀Ⰰ 嬀猀漀甀爀挀攀开椀搀崀⤀ 嘀䄀䰀唀䔀匀 ⠀㄀Ⰰ 一✀吀攀爀洀✀Ⰰ ㄀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 一唀䰀䰀⤀ഀഀ
GO਍䤀一匀䔀刀吀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀 ⠀嬀琀攀爀洀开琀礀瀀攀开椀搀崀Ⰰ 嬀琀椀琀氀攀崀Ⰰ 嬀椀挀漀渀开椀搀崀Ⰰ 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀挀爀攀愀琀攀搀开戀礀崀Ⰰ 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀洀漀搀椀昀椀攀搀开戀礀崀Ⰰ 嬀猀漀甀爀挀攀开椀搀崀⤀ 嘀䄀䰀唀䔀匀 ⠀㈀Ⰰ 一✀䌀愀琀攀最漀爀礀✀Ⰰ ㈀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 一唀䰀䰀⤀ഀഀ
GO਍䤀一匀䔀刀吀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀 ⠀嬀琀攀爀洀开琀礀瀀攀开椀搀崀Ⰰ 嬀琀椀琀氀攀崀Ⰰ 嬀椀挀漀渀开椀搀崀Ⰰ 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀挀爀攀愀琀攀搀开戀礀崀Ⰰ 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀洀漀搀椀昀椀攀搀开戀礀崀Ⰰ 嬀猀漀甀爀挀攀开椀搀崀⤀ 嘀䄀䰀唀䔀匀 ⠀㌀Ⰰ 一✀刀甀氀攀✀Ⰰ ㌀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 一唀䰀䰀⤀ഀഀ
GO਍䤀一匀䔀刀吀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀 ⠀嬀琀攀爀洀开琀礀瀀攀开椀搀崀Ⰰ 嬀琀椀琀氀攀崀Ⰰ 嬀椀挀漀渀开椀搀崀Ⰰ 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀挀爀攀愀琀攀搀开戀礀崀Ⰰ 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀洀漀搀椀昀椀攀搀开戀礀崀Ⰰ 嬀猀漀甀爀挀攀开椀搀崀⤀ 嘀䄀䰀唀䔀匀 ⠀㐀Ⰰ 一✀倀漀氀椀挀礀✀Ⰰ 㐀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 一唀䰀䰀⤀ഀഀ
GO਍匀䔀吀 䤀䐀䔀一吀䤀吀夀开䤀一匀䔀刀吀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀 伀䘀䘀ഀഀ
GO਍ഀഀ
SET ANSI_NULLS ON;਍䜀伀ഀഀ
਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一㬀ഀഀ
GO਍ഀഀ
-- =============================================਍ⴀⴀ 䄀甀琀栀漀爀㨀ऀ 倀愀眀攀䈀 䬀眀愀爀挀椀䐀猁欀椀ഀഀ
-- Create date: 2018-12-12਍ⴀⴀ 䐀攀猀挀爀椀瀀琀椀漀渀㨀 唀瀀搀愀琀攀猀 氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀Ⰰ 洀漀搀椀昀椀攀搀开戀礀 挀漀氀甀洀渀猀 漀渀 椀渀猀攀爀琀 漀爀 甀瀀搀愀琀攀⸀ഀഀ
-- =============================================਍䌀刀䔀䄀吀䔀 吀刀䤀䜀䜀䔀刀 嬀琀爀最开最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀开䴀漀搀椀昀礀崀 伀一 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀ഀഀ
AFTER INSERT,UPDATE਍䄀匀ഀഀ
    BEGIN਍        唀倀䐀䄀吀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀ഀഀ
          SET਍              嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 㴀 䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
              [modified_by] = SUSER_SNAME()਍          圀䠀䔀刀䔀 嬀琀攀爀洀开琀礀瀀攀开椀搀崀 䤀一ഀഀ
                                 (਍                                  匀䔀䰀䔀䌀吀 䐀䤀匀吀䤀一䌀吀ഀഀ
                                         [term_type_id]਍                                    䘀刀伀䴀 嬀䤀渀猀攀爀琀攀搀崀ഀഀ
                                 );਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_types] ENABLE TRIGGER [trg_glossary_term_types_Modify];਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 最氀漀猀猀愀爀礀开琀攀爀洀猀ഀഀ
਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀⠀ഀഀ
     [term_id] [int] IDENTITY(1,1) NOT NULL,਍     嬀搀愀琀愀戀愀猀攀开椀搀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [type_id] [int] NOT NULL,਍     嬀瀀愀爀攀渀琀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
     [title] [nvarchar](250) NOT NULL,਍     嬀搀攀猀挀爀椀瀀琀椀漀渀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [description_search] [nvarchar](max) NULL,਍     嬀搀攀猀挀爀椀瀀琀椀漀渀开瀀氀愀椀渀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field1] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㈀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field3] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㐀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field5] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㘀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field7] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㠀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field9] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㄀　崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field11] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㄀㈀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field13] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㄀㐀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field15] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㄀㘀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field17] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㄀㠀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field19] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㈀　崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field21] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㈀㈀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field23] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㈀㐀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field25] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㈀㘀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field27] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㈀㠀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field29] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㌀　崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field31] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㌀㈀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field33] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㌀㐀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field35] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㌀㘀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field37] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㌀㠀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [field39] [nvarchar](max) NULL,਍     嬀昀椀攀氀搀㐀　崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [creation_date] [datetime] NOT NULL,਍     嬀挀爀攀愀琀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [last_modification_date] [datetime] NOT NULL,਍     嬀洀漀搀椀昀椀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [source_id] [int] NULL,਍ 䌀伀一匀吀刀䄀䤀一吀 嬀倀䬀开最氀漀猀猀愀爀礀开琀攀爀洀猀崀 倀刀䤀䴀䄀刀夀 䬀䔀夀 䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍     嬀琀攀爀洀开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],਍ 䌀伀一匀吀刀䄀䤀一吀 嬀唀䬀开最氀漀猀猀愀爀礀开琀攀爀洀猀开琀椀琀氀攀崀 唀一䤀儀唀䔀 一伀一䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍     嬀琀椀琀氀攀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀 吀䔀堀吀䤀䴀䄀䜀䔀开伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_creation_date]  DEFAULT (getdate()) FOR [creation_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀猀开洀漀搀椀昀椀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀洀漀搀椀昀椀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms]  WITH CHECK ADD  CONSTRAINT [FK_glossary_terms_database] FOREIGN KEY([database_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀搀愀琀愀戀愀猀攀猀崀 ⠀嬀搀愀琀愀戀愀猀攀开椀搀崀⤀ഀഀ
ON DELETE CASCADE਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开最氀漀猀猀愀爀礀开琀攀爀洀猀开搀愀琀愀戀愀猀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms]  WITH CHECK ADD  CONSTRAINT [FK_glossary_terms_glossary_term_types] FOREIGN KEY([type_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开琀礀瀀攀猀崀 ⠀嬀琀攀爀洀开琀礀瀀攀开椀搀崀⤀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms] CHECK CONSTRAINT [FK_glossary_terms_glossary_term_types]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开最氀漀猀猀愀爀礀开琀攀爀洀猀开最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀瀀愀爀攀渀琀开椀搀崀⤀ഀഀ
REFERENCES [glossary_terms] ([term_id])਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开最氀漀猀猀愀爀礀开琀攀爀洀猀开最氀漀猀猀愀爀礀开琀攀爀洀猀崀ഀഀ
GO਍ഀഀ
GRANT DELETE ON [glossary_terms] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT INSERT ON [glossary_terms] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT SELECT ON [glossary_terms] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT UPDATE ON [glossary_terms] TO [users] AS [dbo];਍䜀伀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一㬀ഀഀ
GO਍ഀഀ
SET QUOTED_IDENTIFIER ON;਍䜀伀ഀഀ
਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Author:	 Paweł Kwarciński਍ⴀⴀ 䌀爀攀愀琀攀 搀愀琀攀㨀 ㈀　㄀㠀ⴀ㄀㈀ⴀ㄀㈀ഀഀ
-- Description: Updates last_modification_date, modified_by columns on insert or update.਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
CREATE TRIGGER [trg_glossary_terms_Modify] ON [glossary_terms]਍䄀䘀吀䔀刀 䤀一匀䔀刀吀Ⰰ唀倀䐀䄀吀䔀ഀഀ
AS਍    䈀䔀䜀䤀一ഀഀ
        UPDATE [glossary_terms]਍          匀䔀吀ഀഀ
              [last_modification_date] = GETDATE(),਍              嬀洀漀搀椀昀椀攀搀开戀礀崀 㴀 匀唀匀䔀刀开匀一䄀䴀䔀⠀⤀ഀഀ
          WHERE [term_id] IN਍                            ⠀ഀഀ
                             SELECT DISTINCT਍                                    嬀琀攀爀洀开椀搀崀ഀഀ
                               FROM [Inserted]਍                            ⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䔀一䄀䈀䰀䔀 吀刀䤀䜀䜀䔀刀 嬀琀爀最开最氀漀猀猀愀爀礀开琀攀爀洀猀开䴀漀搀椀昀礀崀㬀ഀഀ
GO਍ഀഀ
-- Table - glossary_mappings਍ഀഀ
SET ANSI_NULLS ON਍䜀伀ഀഀ
਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一ഀഀ
GO਍ഀഀ
CREATE TABLE [glossary_mappings](਍     嬀洀愀瀀瀀椀渀最开椀搀崀 嬀椀渀琀崀 䤀䐀䔀一吀䤀吀夀⠀㄀Ⰰ㄀⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
     [term_id] [int] NOT NULL,਍     嬀漀戀樀攀挀琀开琀礀瀀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　　⤀ 一唀䰀䰀Ⰰഀഀ
     [object_id] [int] NOT NULL,਍     嬀攀氀攀洀攀渀琀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
     [creation_date] [datetime] NOT NULL,਍     嬀挀爀攀愀琀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [last_modification_date] [datetime] NOT NULL,਍     嬀洀漀搀椀昀椀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [source_id] [int] NULL,਍ 䌀伀一匀吀刀䄀䤀一吀 嬀倀䬀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 倀刀䤀䴀䄀刀夀 䬀䔀夀 䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍     嬀洀愀瀀瀀椀渀最开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],਍䌀伀一匀吀刀䄀䤀一吀 嬀唀䬀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 唀一䤀儀唀䔀 一伀一䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍     嬀琀攀爀洀开椀搀崀 䄀匀䌀Ⰰഀഀ
     [object_type] ASC,਍     嬀漀戀樀攀挀琀开椀搀崀 䄀匀䌀Ⰰഀഀ
     [element_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 䤀䜀一伀刀䔀开䐀唀倀开䬀䔀夀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
) ON [PRIMARY]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开挀爀攀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_mappings] ADD  CONSTRAINT [DF_glossary_mappings_created_by]  DEFAULT (suser_sname()) FOR [created_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_mappings] ADD  CONSTRAINT [DF_glossary_mappings_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开最氀漀猀猀愀爀礀开琀攀爀洀猀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀琀攀爀洀开椀搀崀⤀ഀഀ
REFERENCES [glossary_terms] ([term_id])਍伀一 䐀䔀䰀䔀吀䔀 䌀䄀匀䌀䄀䐀䔀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_mappings] CHECK CONSTRAINT [FK_glossary_mappings_glossary_terms]਍䜀伀ഀഀ
਍䜀刀䄀一吀 䐀䔀䰀䔀吀䔀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 䤀一匀䔀刀吀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 匀䔀䰀䔀䌀吀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 唀倀䐀䄀吀䔀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍ഀഀ
SET ANSI_NULLS ON;਍䜀伀ഀഀ
਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一㬀ഀഀ
GO਍ഀഀ
-- =============================================਍ⴀⴀ 䄀甀琀栀漀爀㨀ऀ 倀愀眀攀䈀 䬀眀愀爀挀椀䐀猁欀椀ഀഀ
-- Create date: 2018-12-12਍ⴀⴀ 䐀攀猀挀爀椀瀀琀椀漀渀㨀 唀瀀搀愀琀攀猀 氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀Ⰰ 洀漀搀椀昀椀攀搀开戀礀 挀漀氀甀洀渀猀 漀渀 椀渀猀攀爀琀 漀爀 甀瀀搀愀琀攀⸀ഀഀ
-- =============================================਍䌀刀䔀䄀吀䔀 吀刀䤀䜀䜀䔀刀 嬀琀爀最开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开䴀漀搀椀昀礀崀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀ഀഀ
AFTER INSERT,UPDATE਍䄀匀ഀഀ
    BEGIN਍        唀倀䐀䄀吀䔀 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀ഀഀ
          SET਍              嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 㴀 䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
              [modified_by] = SUSER_SNAME()਍          圀䠀䔀刀䔀 嬀洀愀瀀瀀椀渀最开椀搀崀 䤀一ഀഀ
                               (਍                                匀䔀䰀䔀䌀吀 䐀䤀匀吀䤀一䌀吀ഀഀ
                                       [mapping_id]਍                                  䘀刀伀䴀 嬀䤀渀猀攀爀琀攀搀崀ഀഀ
                               );਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_mappings] ENABLE TRIGGER [trg_glossary_mappings_Modify];਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一ഀഀ
GO਍ഀഀ
SET QUOTED_IDENTIFIER ON਍䜀伀ഀഀ
਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀崀⠀ഀഀ
     [type_id] [int] IDENTITY(1,1) NOT NULL,਍     嬀琀椀琀氀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
     [title_reverse] [nvarchar](250) NOT NULL,਍     嬀椀猀开猀礀洀洀攀琀爀椀挀愀氀崀 嬀戀椀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [creation_date] [datetime] NOT NULL,਍     嬀挀爀攀愀琀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [last_modification_date] [datetime] NOT NULL,਍     嬀洀漀搀椀昀椀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [source_id] [int] NULL,਍ 䌀伀一匀吀刀䄀䤀一吀 嬀倀䬀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀崀 倀刀䤀䴀䄀刀夀 䬀䔀夀 䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍     嬀琀礀瀀攀开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationship_types] ADD  CONSTRAINT [DF_glossary_term_relationship_types_creation_date]  DEFAULT (getdate()) FOR [creation_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationship_types] ADD  CONSTRAINT [DF_glossary_term_relationship_types_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀开洀漀搀椀昀椀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀洀漀搀椀昀椀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
GRANT DELETE ON [glossary_term_relationship_types] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT INSERT ON [glossary_term_relationship_types] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT SELECT ON [glossary_term_relationship_types] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT UPDATE ON [glossary_term_relationship_types] TO [users] AS [dbo];਍䜀伀ഀഀ
਍䜀伀ഀഀ
SET IDENTITY_INSERT [glossary_term_relationship_types] ON ਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (1, N'Related term', N'Related term', 1, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (2, N'Replaces', N'Is replaced by', 0, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (3, N'Is calculated from', N'Is used to calculate', 0, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (4, N'Is a synonym of', N'Is a synonym of', 1, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (5, N'Has a preferred synonym', N'Is a preferred synonym of', 0, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (6, N'Is a type of', N'Has a type', 0, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (7, N'Contains', N'Is contained in', 0, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
INSERT [glossary_term_relationship_types] ([type_id], [title], [title_reverse], [is_symmetrical], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (8, N'Is used by', N'Uses', 0, getdate(), N'LSREP17', getdate(), N'LSREP17', NULL)਍䜀伀ഀഀ
SET IDENTITY_INSERT [glossary_term_relationship_types] OFF਍䜀伀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一㬀ഀഀ
GO਍ഀഀ
SET QUOTED_IDENTIFIER ON;਍䜀伀ഀഀ
਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Author:	 Paweł Kwarciński਍ⴀⴀ 䌀爀攀愀琀攀 搀愀琀攀㨀 ㈀　㄀㠀ⴀ㄀㈀ⴀ㄀㈀ഀഀ
-- Description: Updates last_modification_date, modified_by columns on insert or update.਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
CREATE TRIGGER [trg_glossary_term_relationship_types_Modify] ON [glossary_term_relationship_types]਍䄀䘀吀䔀刀 䤀一匀䔀刀吀Ⰰ唀倀䐀䄀吀䔀ഀഀ
AS਍    䈀䔀䜀䤀一ഀഀ
        UPDATE [glossary_term_relationship_types]਍          匀䔀吀ഀഀ
              [last_modification_date] = GETDATE(),਍              嬀洀漀搀椀昀椀攀搀开戀礀崀 㴀 匀唀匀䔀刀开匀一䄀䴀䔀⠀⤀ഀഀ
          WHERE [type_id] IN਍                          ⠀ഀഀ
                           SELECT DISTINCT਍                                  嬀琀礀瀀攀开椀搀崀ഀഀ
                             FROM [Inserted]਍                          ⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀崀 䔀一䄀䈀䰀䔀 吀刀䤀䜀䜀䔀刀 嬀琀爀最开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀开䴀漀搀椀昀礀崀㬀ഀഀ
GO਍ഀഀ
-- Table - glossary_term_relationships਍ഀഀ
SET ANSI_NULLS ON਍䜀伀ഀഀ
਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一ഀഀ
GO਍ഀഀ
CREATE TABLE [glossary_term_relationships](਍     嬀爀攀氀愀琀椀漀渀猀栀椀瀀开椀搀崀 嬀椀渀琀崀 䤀䐀䔀一吀䤀吀夀⠀㄀Ⰰ㄀⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
     [source_term_id] [int] NOT NULL,਍     嬀搀攀猀琀椀渀愀琀椀漀渀开琀攀爀洀开椀搀崀 嬀椀渀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [type_id] [int] NOT NULL,਍     嬀搀攀猀挀爀椀瀀琀椀漀渀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [creation_date] [datetime] NOT NULL,਍     嬀挀爀攀愀琀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [last_modification_date] [datetime] NOT NULL,਍     嬀洀漀搀椀昀椀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [source_id] [int] NULL,਍ 䌀伀一匀吀刀䄀䤀一吀 嬀倀䬀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀 倀刀䤀䴀䄀刀夀 䬀䔀夀 䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍     嬀爀攀氀愀琀椀漀渀猀栀椀瀀开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀 吀䔀堀吀䤀䴀䄀䜀䔀开伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
CREATE UNIQUE NONCLUSTERED INDEX [UK_glossary_term_relationships] ON [glossary_term_relationships]਍⠀ഀഀ
     [source_term_id] ASC,਍     嬀搀攀猀琀椀渀愀琀椀漀渀开琀攀爀洀开椀搀崀 䄀匀䌀Ⰰഀഀ
     [type_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 匀伀刀吀开䤀一开吀䔀䴀倀䐀䈀 㴀 伀䘀䘀Ⰰ 䤀䜀一伀刀䔀开䐀唀倀开䬀䔀夀 㴀 伀䘀䘀Ⰰ 䐀刀伀倀开䔀堀䤀匀吀䤀一䜀 㴀 伀䘀䘀Ⰰ 伀一䰀䤀一䔀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationships] ADD  CONSTRAINT [DF_glossary_term_relationships_creation_date]  DEFAULT (getdate()) FOR [creation_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationships] ADD  CONSTRAINT [DF_glossary_term_relationships_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开洀漀搀椀昀椀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀洀漀搀椀昀椀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationships]  WITH CHECK ADD  CONSTRAINT [FK_glossary_term_relationships_glossary_term_relationship_types] FOREIGN KEY([type_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀崀 ⠀嬀琀礀瀀攀开椀搀崀⤀ഀഀ
ON DELETE CASCADE਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀开琀礀瀀攀猀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationships]  WITH CHECK ADD  CONSTRAINT [FK_glossary_term_relationships_glossary_terms_destination] FOREIGN KEY([destination_term_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀 ⠀嬀琀攀爀洀开椀搀崀⤀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationships] CHECK CONSTRAINT [FK_glossary_term_relationships_glossary_terms_destination]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开最氀漀猀猀愀爀礀开琀攀爀洀猀开猀漀甀爀挀攀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀猀漀甀爀挀攀开琀攀爀洀开椀搀崀⤀ഀഀ
REFERENCES [glossary_terms] ([term_id])਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开最氀漀猀猀愀爀礀开琀攀爀洀猀开猀漀甀爀挀攀崀ഀഀ
GO਍ഀഀ
SET ANSI_NULLS ON;਍䜀伀ഀഀ
਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一㬀ഀഀ
GO਍ഀഀ
-- =============================================਍ⴀⴀ 䄀甀琀栀漀爀㨀ऀ 倀愀眀攀䈀 䬀眀愀爀挀椀䐀猁欀椀ഀഀ
-- Create date: 2018-12-12਍ⴀⴀ 䐀攀猀挀爀椀瀀琀椀漀渀㨀 唀瀀搀愀琀攀猀 氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀Ⰰ 洀漀搀椀昀椀攀搀开戀礀 挀漀氀甀洀渀猀 漀渀 椀渀猀攀爀琀 漀爀 甀瀀搀愀琀攀⸀ഀഀ
-- =============================================਍䌀刀䔀䄀吀䔀 吀刀䤀䜀䜀䔀刀 嬀琀爀最开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开䴀漀搀椀昀礀崀 伀一 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀ഀഀ
AFTER INSERT,UPDATE਍䄀匀ഀഀ
    BEGIN਍        唀倀䐀䄀吀䔀 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀ഀഀ
          SET਍              嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 㴀 䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
              [modified_by] = SUSER_SNAME()਍          圀䠀䔀刀䔀 嬀爀攀氀愀琀椀漀渀猀栀椀瀀开椀搀崀 䤀一ഀഀ
                                    (਍                                     匀䔀䰀䔀䌀吀 䐀䤀匀吀䤀一䌀吀ഀഀ
                                            [relationship_id]਍                                       䘀刀伀䴀 嬀䤀渀猀攀爀琀攀搀崀ഀഀ
                                    );਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_term_relationships] ENABLE TRIGGER [trg_glossary_term_relationships_Modify];਍䜀伀ഀഀ
਍ⴀⴀ 唀瀀搀愀琀椀渀最 猀挀栀攀洀愀 搀愀琀愀ഀഀ
਍唀倀䐀䄀吀䔀 嬀琀崀ഀഀ
  SET਍      嬀琀崀⸀嬀猀挀栀攀洀愀崀 㴀 ✀✀ഀഀ
  FROM [tables] [t]਍       䨀伀䤀一 嬀搀愀琀愀戀愀猀攀猀崀 嬀搀崀 伀一 嬀搀崀⸀嬀琀礀瀀攀崀 䤀一⠀✀䴀夀匀儀䰀✀Ⰰ ✀䴀夀匀儀䰀㠀✀Ⰰ✀䴀䄀刀䤀䄀䐀䈀✀Ⰰ✀䄀唀刀伀刀䄀✀Ⰰ✀倀䔀刀䌀伀一䄀开䴀夀匀儀䰀✀⤀ഀഀ
  AND [t].[database_id] = [d].[database_id]਍  圀䠀䔀刀䔀 嬀琀崀⸀嬀猀挀栀攀洀愀崀 㴀 嬀搀崀⸀嬀渀愀洀攀崀㬀ഀഀ
਍䜀伀ഀഀ
UPDATE [p]਍  匀䔀吀ഀഀ
      [p].[schema] = ''਍  䘀刀伀䴀 嬀瀀爀漀挀攀搀甀爀攀猀崀 嬀瀀崀ഀഀ
       JOIN [databases] [d] ON [d].[type] IN('MYSQL', 'MYSQL8','MARIADB','AURORA','PERCONA_MYSQL')਍  䄀一䐀 嬀瀀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀 㴀 嬀搀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀ഀഀ
  WHERE [p].[schema] = [d].[name];਍䜀伀ഀഀ
਍ⴀⴀ 䘀椀砀椀渀最 䘀䬀猀ഀഀ
਍䤀䘀 䔀堀䤀匀吀匀ഀഀ
         (਍          匀䔀䰀䔀䌀吀 ⨀ഀഀ
            FROM [INFORMATION_SCHEMA].[REFERENTIAL_CONSTRAINTS]਍            圀䠀䔀刀䔀 嬀䌀伀一匀吀刀䄀䤀一吀开一䄀䴀䔀崀 㴀 ✀䘀䬀开琀愀戀氀攀猀开琀愀戀氀攀猀✀ഀഀ
         )਍    䈀䔀䜀䤀一ഀഀ
        ALTER TABLE [tables] DROP CONSTRAINT [FK_tables_tables];਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
IF EXISTS਍         ⠀ഀഀ
          SELECT *਍            䘀刀伀䴀 嬀琀愀戀氀攀猀崀 嬀琀崀ഀഀ
            WHERE [t].[database_id] NOT IN਍                                          ⠀ഀഀ
                                           SELECT [database_id]਍                                             䘀刀伀䴀 嬀搀愀琀愀戀愀猀攀猀崀ഀഀ
                                          )਍         ⤀ഀഀ
    BEGIN਍        䤀一匀䔀刀吀 䤀一吀伀 嬀搀愀琀愀戀愀猀攀猀崀ഀഀ
        ([title],਍         嬀琀礀瀀攀崀Ⰰഀഀ
         [name],਍         嬀栀漀猀琀崀Ⰰഀഀ
         [description],਍         嬀搀攀猀挀爀椀瀀琀椀漀渀开猀攀愀爀挀栀崀Ⰰഀഀ
         [description_plain]਍        ⤀ഀഀ
        VALUES਍        ⠀✀刀攀挀漀瘀攀爀攀搀✀Ⰰഀഀ
         'MANUAL',਍         ✀䴀䄀一唀䄀䰀✀Ⰰഀഀ
         'MANUAL',਍         ✀吀愀戀氀攀猀 眀椀琀栀 洀椀猀猀椀渀最 瀀愀爀攀渀琀 搀愀琀愀戀愀猀攀 䤀䐀⸀✀Ⰰഀഀ
         'Tables with missing parent database ID.',਍         ✀吀愀戀氀攀猀 眀椀琀栀 洀椀猀猀椀渀最 瀀愀爀攀渀琀 搀愀琀愀戀愀猀攀 䤀䐀⸀✀ഀഀ
        );਍ഀഀ
        UPDATE [databases]਍          匀䔀吀ഀഀ
              [name] = 'MANUAL'+CONVERT(nvarchar(max),SCOPE_IDENTITY())਍          圀䠀䔀刀䔀 嬀搀愀琀愀戀愀猀攀开椀搀崀 㴀 匀䌀伀倀䔀开䤀䐀䔀一吀䤀吀夀⠀⤀㬀ഀഀ
਍        唀倀䐀䄀吀䔀 嬀琀愀戀氀攀猀崀ഀഀ
          SET਍              嬀搀愀琀愀戀愀猀攀开椀搀崀 㴀 匀䌀伀倀䔀开䤀䐀䔀一吀䤀吀夀⠀⤀ഀഀ
          WHERE [database_id] NOT IN਍                                    ⠀ഀഀ
                                     SELECT [database_id]਍                                       䘀刀伀䴀 嬀搀愀琀愀戀愀猀攀猀崀ഀഀ
                                    );਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables]਍圀䤀吀䠀 䌀䠀䔀䌀䬀ഀഀ
ADD CONSTRAINT [FK_tables_databases] FOREIGN KEY([database_id]) REFERENCES [databases]([database_id]);਍䜀伀ഀഀ
ALTER TABLE [tables] CHECK CONSTRAINT [FK_tables_databases];਍䜀伀ഀഀ
਍ⴀⴀ 䤀渀搀攀砀攀猀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀ഀഀ
             (਍              匀䔀䰀䔀䌀吀 ⨀ഀഀ
                FROM [sys].[indexes]਍                圀䠀䔀刀䔀 嬀渀愀洀攀崀 㴀 ✀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开琀攀爀洀开椀搀✀ഀഀ
                      AND [object_id] = OBJECT_ID(N'[glossary_mappings]')਍             ⤀ഀഀ
    BEGIN਍        䌀刀䔀䄀吀䔀 䤀一䐀䔀堀 嬀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开琀攀爀洀开椀搀崀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀⠀嬀琀攀爀洀开椀搀崀⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀ഀഀ
             (਍              匀䔀䰀䔀䌀吀 ⨀ഀഀ
                FROM [sys].[indexes]਍                圀䠀䔀刀䔀 嬀渀愀洀攀崀 㴀 ✀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开漀戀樀攀挀琀开椀搀✀ഀഀ
                      AND [object_id] = OBJECT_ID(N'[glossary_mappings]')਍             ⤀ഀഀ
    BEGIN਍        䌀刀䔀䄀吀䔀 䤀一䐀䔀堀 嬀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开漀戀樀攀挀琀开椀搀崀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀⠀嬀漀戀樀攀挀琀开椀搀崀⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀ഀഀ
             (਍              匀䔀䰀䔀䌀吀 ⨀ഀഀ
                FROM [sys].[indexes]਍                圀䠀䔀刀䔀 嬀渀愀洀攀崀 㴀 ✀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开攀氀攀洀攀渀琀开椀搀✀ഀഀ
                      AND [object_id] = OBJECT_ID(N'[glossary_mappings]')਍             ⤀ഀഀ
    BEGIN਍        䌀刀䔀䄀吀䔀 䤀一䐀䔀堀 嬀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开攀氀攀洀攀渀琀开椀搀崀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀⠀嬀攀氀攀洀攀渀琀开椀搀崀⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀ഀഀ
             (਍              匀䔀䰀䔀䌀吀 ⨀ഀഀ
                FROM [sys].[indexes]਍                圀䠀䔀刀䔀 嬀渀愀洀攀崀 㴀 ✀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开漀戀樀攀挀琀开琀礀瀀攀✀ഀഀ
                      AND [object_id] = OBJECT_ID(N'[glossary_mappings]')਍             ⤀ഀഀ
    BEGIN਍        䌀刀䔀䄀吀䔀 䤀一䐀䔀堀 嬀椀砀开最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀开漀戀樀攀挀琀开琀礀瀀攀崀 伀一 嬀最氀漀猀猀愀爀礀开洀愀瀀瀀椀渀最猀崀⠀嬀漀戀樀攀挀琀开琀礀瀀攀崀⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀ഀഀ
             (਍              匀䔀䰀䔀䌀吀 ⨀ഀഀ
                FROM [sys].[indexes]਍                圀䠀䔀刀䔀 嬀渀愀洀攀崀 㴀 ✀椀砀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开搀攀猀琀椀渀愀琀椀漀渀开琀攀爀洀开椀搀✀ഀഀ
                      AND [object_id] = OBJECT_ID(N'[glossary_term_relationships]')਍             ⤀ഀഀ
    BEGIN਍        䌀刀䔀䄀吀䔀 䤀一䐀䔀堀 嬀椀砀开最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀开搀攀猀琀椀渀愀琀椀漀渀开琀攀爀洀开椀搀崀 伀一 嬀最氀漀猀猀愀爀礀开琀攀爀洀开爀攀氀愀琀椀漀渀猀栀椀瀀猀崀⠀嬀搀攀猀琀椀渀愀琀椀漀渀开琀攀爀洀开椀搀崀⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍䤀䘀 一伀吀 䔀堀䤀匀吀匀ഀഀ
             (਍              匀䔀䰀䔀䌀吀 ⨀ഀഀ
                FROM [sys].[indexes]਍                圀䠀䔀刀䔀 嬀渀愀洀攀崀 㴀 ✀椀砀开最氀漀猀猀愀爀礀开琀攀爀洀猀开瀀愀爀攀渀琀开椀搀✀ഀഀ
                      AND [object_id] = OBJECT_ID(N'[glossary_terms]')਍             ⤀ഀഀ
    BEGIN਍        䌀刀䔀䄀吀䔀 䤀一䐀䔀堀 嬀椀砀开最氀漀猀猀愀爀礀开琀攀爀洀猀开瀀愀爀攀渀琀开椀搀崀 伀一 嬀最氀漀猀猀愀爀礀开琀攀爀洀猀崀⠀嬀瀀愀爀攀渀琀开椀搀崀⤀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍ⴀⴀ 嘀攀爀猀椀漀渀ഀഀ
UPDATE [version]਍  匀䔀吀ഀഀ
      [stable] = 1਍  圀䠀䔀刀䔀 嬀瘀攀爀猀椀漀渀崀 㴀 㜀ഀഀ
        AND [update] = 4਍        䄀一䐀 嬀爀攀氀攀愀猀攀崀 㴀 　㬀ഀഀ
GO