-- Version਍䤀䘀ഀഀ
  (਍   匀䔀䰀䔀䌀吀 䌀伀唀一吀⠀⨀⤀ഀഀ
     FROM [version]਍     圀䠀䔀刀䔀 嬀瘀攀爀猀椀漀渀崀 㴀 㜀ഀഀ
           AND [update] = 5਍           䄀一䐀 嬀爀攀氀攀愀猀攀崀 㴀 　ഀഀ
  ) = 0਍    䈀䔀䜀䤀一ഀഀ
        INSERT INTO [version]਍        ⠀嬀瘀攀爀猀椀漀渀崀Ⰰഀഀ
         [update],਍         嬀爀攀氀攀愀猀攀崀Ⰰഀഀ
         [stable]਍        ⤀ഀഀ
        VALUES਍        ⠀㜀Ⰰഀഀ
         5,਍         　Ⰰഀഀ
         0਍        ⤀㬀ഀഀ
END;਍䜀伀ഀഀ
਍ⴀⴀ 䘀椀砀椀渀最 䈀甀猀椀渀攀猀猀 䜀氀漀猀猀愀爀礀 最爀愀渀琀猀ഀഀ
GRANT DELETE ON [glossary_term_relationships] TO [users] AS [dbo]਍䜀伀ഀഀ
GRANT INSERT ON [glossary_term_relationships] TO [users] AS [dbo]਍䜀伀ഀഀ
GRANT SELECT ON [glossary_term_relationships] TO [users] AS [dbo]਍䜀伀ഀഀ
GRANT UPDATE ON [glossary_term_relationships] TO [users] AS [dbo]਍䜀伀ഀഀ
GRANT ALTER ON [glossary_term_relationships] TO [users] AS [dbo]਍䜀伀ഀഀ
਍ⴀⴀ 䘀椀砀椀渀最 䘀䬀猀ഀഀ
ALTER TABLE [custom_fields_values] DROP CONSTRAINT [FK_custom_fields_values_custom_fields]਍䜀伀ഀഀ
ALTER TABLE [custom_fields_values]  WITH CHECK ADD  CONSTRAINT [FK_custom_fields_values_custom_fields] FOREIGN KEY([custom_field_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀挀甀猀琀漀洀开昀椀攀氀搀猀崀 ⠀嬀挀甀猀琀漀洀开昀椀攀氀搀开椀搀崀⤀ഀഀ
ON DELETE CASCADE਍䜀伀ഀഀ
ALTER TABLE [custom_fields_values] CHECK CONSTRAINT [FK_custom_fields_values_custom_fields]਍䜀伀ഀഀ
਍䤀䘀 䔀堀䤀匀吀匀ഀഀ
         (਍          匀䔀䰀䔀䌀吀 ⨀ഀഀ
            FROM [INFORMATION_SCHEMA].[REFERENTIAL_CONSTRAINTS]਍            圀䠀䔀刀䔀 嬀䌀伀一匀吀刀䄀䤀一吀开一䄀䴀䔀崀 㴀 ✀䘀䬀开琀愀戀氀攀猀开搀愀琀愀戀愀猀攀✀ഀഀ
         )਍    䈀䔀䜀䤀一ഀഀ
        ALTER TABLE [tables] DROP CONSTRAINT [FK_tables_database];਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
਍ⴀⴀ 䐀愀琀愀戀愀猀攀猀 琀愀戀氀攀 挀漀氀甀洀渀猀ഀഀ
ALTER TABLE [databases]਍䄀䐀䐀 嬀猀猀氀开欀攀礀开瀀愀琀栀崀  渀瘀愀爀挀栀愀爀⠀㔀　　⤀ 一唀䰀䰀Ⰰഀഀ
    [ssl_cert_path] nvarchar(500) NULL,਍    嬀猀猀氀开挀愀开瀀愀琀栀崀   渀瘀愀爀挀栀愀爀⠀㔀　　⤀ 一唀䰀䰀Ⰰഀഀ
    [ssl_cipher]    nvarchar(max) NULL;਍䜀伀ഀഀ
਍ⴀⴀ 唀猀攀爀 挀漀渀渀攀挀琀椀漀渀猀 琀愀戀氀攀ഀഀ
ALTER TABLE [user_connections] ALTER COLUMN [created_by] NVARCHAR (1024) NULL;਍䜀伀ഀഀ
ALTER TABLE [user_connections] ALTER COLUMN [modified_by] NVARCHAR (1024) NULL;਍䜀伀ഀഀ
਍ⴀⴀ 䌀甀猀琀漀洀 昀椀攀氀搀猀ഀഀ
ALTER TABLE [columns]਍䄀䐀䐀 嬀昀椀攀氀搀㐀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field42]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field44]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field46]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field48]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field50]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field52]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field54]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field56]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field58]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field60]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field62]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field64]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field66]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field68]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field70]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field72]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field74]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field76]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field78]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field80]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field82]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field84]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field86]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field88]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field90]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field92]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field94]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field96]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field98]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field100] nvarchar(max) NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀搀愀琀愀戀愀猀攀猀崀ഀഀ
ADD [field41]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field43]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field45]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field47]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field49]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field51]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field53]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field55]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field57]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field59]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field61]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field63]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field65]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field67]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field69]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field71]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field73]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field75]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field77]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field79]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field81]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field83]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field85]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field87]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field89]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field91]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field93]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field95]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field97]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field99]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㄀　　崀 渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [glossary_terms]਍䄀䐀䐀 嬀昀椀攀氀搀㐀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field42]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field44]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field46]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field48]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field50]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field52]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field54]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field56]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field58]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field60]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field62]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field64]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field66]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field68]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field70]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field72]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field74]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field76]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field78]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field80]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field82]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field84]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field86]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field88]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field90]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field92]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field94]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field96]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field98]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field100] nvarchar(max) NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀洀漀搀甀氀攀猀崀ഀഀ
ADD [field41]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field43]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field45]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field47]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field49]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field51]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field53]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field55]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field57]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field59]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field61]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field63]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field65]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field67]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field69]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field71]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field73]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field75]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field77]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field79]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field81]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field83]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field85]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field87]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field89]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field91]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field93]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field95]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field97]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field99]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㄀　　崀 渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [parameters]਍䄀䐀䐀 嬀昀椀攀氀搀㐀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field42]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field44]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field46]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field48]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field50]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field52]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field54]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field56]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field58]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field60]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field62]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field64]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field66]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field68]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field70]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field72]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field74]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field76]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field78]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field80]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field82]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field84]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field86]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field88]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field90]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field92]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field94]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field96]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field98]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field100] nvarchar(max) NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀瀀爀漀挀攀搀甀爀攀猀崀ഀഀ
ADD [field41]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field43]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field45]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field47]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field49]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field51]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field53]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field55]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field57]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field59]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field61]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field63]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field65]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field67]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field69]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field71]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field73]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field75]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field77]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field79]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field81]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field83]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field85]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field87]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field89]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field91]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field93]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field95]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field97]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field99]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㄀　　崀 渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [tables]਍䄀䐀䐀 嬀昀椀攀氀搀㐀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field42]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field44]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field46]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field48]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field50]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field52]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field54]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field56]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field58]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field60]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field62]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field64]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field66]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field68]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field70]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field72]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field74]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field76]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field78]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field80]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field82]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field84]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field86]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field88]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field90]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field92]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field94]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field96]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field98]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field100] nvarchar(max) NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀琀愀戀氀攀猀开爀攀氀愀琀椀漀渀猀崀ഀഀ
ADD [field41]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field43]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field45]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field47]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field49]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field51]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field53]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field55]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field57]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field59]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field61]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field63]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field65]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field67]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field69]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field71]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field73]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field75]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field77]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field79]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field81]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field83]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field85]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field87]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field89]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field91]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field93]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field95]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field97]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field99]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㄀　　崀 渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
ALTER TABLE [triggers]਍䄀䐀䐀 嬀昀椀攀氀搀㐀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field42]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field44]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field46]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field48]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field50]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field52]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field54]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field56]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field58]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field60]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field62]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field64]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field66]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field68]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field70]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field72]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field74]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field76]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field78]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field80]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field82]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field84]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field86]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field88]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field90]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㄀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field92]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㌀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field94]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㔀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field96]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㜀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field98]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㤀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field100] nvarchar(max) NULL;਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀甀渀椀焀甀攀开挀漀渀猀琀爀愀椀渀琀猀崀ഀഀ
ADD [field41]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field43]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field45]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field47]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㐀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field49]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field51]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field53]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field55]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field57]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㔀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field59]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field61]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field63]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field65]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field67]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㘀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field69]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field71]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field73]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field75]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field77]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㜀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field79]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field81]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field83]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field85]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field87]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㠀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field89]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀　崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field91]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㈀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field93]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㐀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field95]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㘀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field97]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㤀㠀崀  渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
    [field99]  nvarchar(max) NULL,਍    嬀昀椀攀氀搀㄀　　崀 渀瘀愀爀挀栀愀爀⠀洀愀砀⤀ 一唀䰀䰀㬀ഀഀ
GO਍ഀഀ
-- Indexes਍䤀䘀 䔀堀䤀匀吀匀ഀഀ
         (਍          匀䔀䰀䔀䌀吀 ⨀ഀഀ
            FROM [sys].[indexes]਍            圀䠀䔀刀䔀 嬀渀愀洀攀崀 㴀 ✀椀砀开挀漀氀甀洀渀猀开搀攀猀挀✀ഀഀ
                  AND object_id = OBJECT_ID(N'[columns]')਍         ⤀ഀഀ
    BEGIN਍        䐀刀伀倀 䤀一䐀䔀堀 嬀椀砀开挀漀氀甀洀渀猀开搀攀猀挀崀 伀一 嬀挀漀氀甀洀渀猀崀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍ഀഀ
IF NOT EXISTS਍             ⠀ഀഀ
              SELECT *਍                䘀刀伀䴀 嬀猀礀猀崀⸀嬀椀渀搀攀砀攀猀崀ഀഀ
                WHERE [name] = 'ix_columns_desc'਍                      䄀一䐀 漀戀樀攀挀琀开椀搀 㴀 伀䈀䨀䔀䌀吀开䤀䐀⠀一✀嬀挀漀氀甀洀渀猀崀✀⤀ഀഀ
             )਍    䈀䔀䜀䤀一ഀഀ
        CREATE INDEX [ix_columns_desc] ON [columns]([table_id],[name],[title]) INCLUDE([description],[field1],[field2],[field3],[field4],[field5],[field6],[field7],[field8],[field9],[field10],[field11],[field12],[field13],[field14],[field15],[field16],[field17],[field18],[field19],[field20],[field21],[field22],[field23],[field24],[field25],[field26],[field27],[field28],[field29],[field30],[field31],[field32],[field33],[field34],[field35],[field36],[field37],[field38],[field39],[field40],[field41],[field42],[field43],[field44],[field45],[field46],[field47],[field48],[field49],[field50],[field51],[field52],[field53],[field54],[field55],[field56],[field57],[field58],[field59],[field60],[field61],[field62],[field63],[field64],[field65],[field66],[field67],[field68],[field69],[field70],[field71],[field72],[field73],[field74],[field75],[field76],[field77],[field78],[field79],[field80],[field81],[field82],[field83],[field84],[field85],[field86],[field87],[field88],[field89],[field90],[field91],[field92],[field93],[field94],[field95],[field96],[field97],[field98],[field99],[field100]);਍䔀一䐀㬀ഀഀ
GO਍ഀഀ
-- Classificator਍ഀഀ
-- Table - classificators਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀⠀ഀഀ
     [classificator_id] [int] IDENTITY(1,1) NOT NULL,਍     嬀戀甀椀氀琀开椀渀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
     [title] [nvarchar](250) NOT NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开渀愀洀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一唀䰀䰀Ⰰഀഀ
     [custom_field_2_name] [nvarchar](250) NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开渀愀洀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一唀䰀䰀Ⰰഀഀ
     [custom_field_4_name] [nvarchar](250) NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开渀愀洀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一唀䰀䰀Ⰰഀഀ
     [custom_field_1_id] [int] NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
     [custom_field_3_id] [int] NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
     [custom_field_5_id] [int] NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开搀攀昀椀渀椀琀椀漀渀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [custom_field_2_definition] [nvarchar](max) NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开搀攀昀椀渀椀琀椀漀渀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [custom_field_4_definition] [nvarchar](max) NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开搀攀昀椀渀椀琀椀漀渀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [description] [nvarchar](max) NULL,਍     嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀 嬀搀愀琀攀琀椀洀攀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [created_by] [nvarchar](1024) NULL,਍     嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 嬀搀愀琀攀琀椀洀攀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [modified_by] [nvarchar](1024) NULL,਍     嬀猀漀甀爀挀攀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
 CONSTRAINT [PK_classificators] PRIMARY KEY CLUSTERED ਍⠀ഀഀ
     [classificator_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 䤀䜀一伀刀䔀开䐀唀倀开䬀䔀夀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开挀爀攀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificators] ADD  CONSTRAINT [DF_classificators_created_by]  DEFAULT (suser_sname()) FOR [created_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificators] ADD  CONSTRAINT [DF_classificators_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开挀甀猀琀漀洀开昀椀攀氀搀猀开㄀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开椀搀崀⤀ഀഀ
REFERENCES [custom_fields] ([custom_field_id])਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开挀甀猀琀漀洀开昀椀攀氀搀猀开㄀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_2] FOREIGN KEY([custom_field_2_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀挀甀猀琀漀洀开昀椀攀氀搀猀崀 ⠀嬀挀甀猀琀漀洀开昀椀攀氀搀开椀搀崀⤀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_2]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开挀甀猀琀漀洀开昀椀攀氀搀猀开㌀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开椀搀崀⤀ഀഀ
REFERENCES [custom_fields] ([custom_field_id])਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开挀甀猀琀漀洀开昀椀攀氀搀猀开㌀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_4] FOREIGN KEY([custom_field_4_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀挀甀猀琀漀洀开昀椀攀氀搀猀崀 ⠀嬀挀甀猀琀漀洀开昀椀攀氀搀开椀搀崀⤀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_4]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀  圀䤀吀䠀 䌀䠀䔀䌀䬀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开挀甀猀琀漀洀开昀椀攀氀搀猀开㔀崀 䘀伀刀䔀䤀䜀一 䬀䔀夀⠀嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开椀搀崀⤀ഀഀ
REFERENCES [custom_fields] ([custom_field_id])਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 䌀䠀䔀䌀䬀 䌀伀一匀吀刀䄀䤀一吀 嬀䘀䬀开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开挀甀猀琀漀洀开昀椀攀氀搀猀开㔀崀ഀഀ
GO਍ഀഀ
GRANT DELETE ON [classificators] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT INSERT ON [classificators] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT SELECT ON [classificators] TO [users] AS [dbo];਍䜀伀ഀഀ
GRANT UPDATE ON [classificators] TO [users] AS [dbo];਍䜀伀ഀഀ
਍匀䔀吀 䤀䐀䔀一吀䤀吀夀开䤀一匀䔀刀吀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 伀一 ഀഀ
GO਍䤀一匀䔀刀吀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 ⠀嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开椀搀崀Ⰰ 嬀戀甀椀氀琀开椀渀开椀搀崀Ⰰ 嬀琀椀琀氀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀搀攀猀挀爀椀瀀琀椀漀渀崀Ⰰ 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀挀爀攀愀琀攀搀开戀礀崀Ⰰ 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀洀漀搀椀昀椀攀搀开戀礀崀Ⰰ 嬀猀漀甀爀挀攀开椀搀崀⤀ 嘀䄀䰀唀䔀匀 ⠀㄀Ⰰ ㄀Ⰰ 一✀倀攀爀猀漀渀愀氀氀礀 䤀搀攀渀琀椀昀椀愀戀氀攀 䤀渀昀漀爀洀愀琀椀漀渀 ⠀倀䤀䤀⤀✀Ⰰ 一✀倀䤀䤀✀Ⰰ 一✀倀䤀䤀 䐀愀琀愀 吀礀瀀攀✀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一✀匀攀渀猀椀琀椀瘀攀 倀䤀䤀Ⰰ 一漀渀ⴀ匀攀渀猀椀琀椀瘀攀 倀䤀䤀Ⰰ 一漀渀ⴀ倀䤀䤀✀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 一唀䰀䰀⤀ഀഀ
GO਍䤀一匀䔀刀吀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 ⠀嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开椀搀崀Ⰰ 嬀戀甀椀氀琀开椀渀开椀搀崀Ⰰ 嬀琀椀琀氀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开渀愀洀攀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开椀搀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㄀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㌀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀挀甀猀琀漀洀开昀椀攀氀搀开㔀开搀攀昀椀渀椀琀椀漀渀崀Ⰰ 嬀搀攀猀挀爀椀瀀琀椀漀渀崀Ⰰ 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀挀爀攀愀琀攀搀开戀礀崀Ⰰ 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ 嬀洀漀搀椀昀椀攀搀开戀礀崀Ⰰ 嬀猀漀甀爀挀攀开椀搀崀⤀ 嘀䄀䰀唀䔀匀 ⠀㈀Ⰰ ㈀Ⰰ 一✀䜀䐀倀刀✀Ⰰ 一✀䜀䐀倀刀 䌀氀愀猀猀椀昀椀挀愀琀椀漀渀✀Ⰰ 一✀䜀䐀倀刀 䐀愀琀愀 吀礀瀀攀✀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一✀倀攀爀猀漀渀愀氀 䐀愀琀愀Ⰰ 匀瀀攀挀椀愀氀 䌀愀琀攀最漀爀礀Ⰰ 一漀渀ⴀ倀攀爀猀漀渀愀氀 䐀愀琀愀✀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 一唀䰀䰀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 最攀琀搀愀琀攀⠀⤀Ⰰ 猀甀猀攀爀开猀渀愀洀攀⠀⤀Ⰰ 一唀䰀䰀⤀ഀഀ
GO਍匀䔀吀 䤀䐀䔀一吀䤀吀夀开䤀一匀䔀刀吀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 伀䘀䘀ഀഀ
GO਍ഀഀ
-- =============================================਍ⴀⴀ 䄀甀琀栀漀爀㨀ऀऀ倀愀眀攀䈀 䬀眀愀爀挀椀䐀猁欀椀ഀഀ
-- Create date:     2019-05-23਍ⴀⴀ 䐀攀猀挀爀椀瀀琀椀漀渀㨀ऀ唀瀀搀愀琀攀猀 氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀Ⰰ 洀漀搀椀昀椀攀搀开戀礀 挀漀氀甀洀渀猀 漀渀 椀渀猀攀爀琀 漀爀 甀瀀搀愀琀攀ഀഀ
-- =============================================਍䌀刀䔀䄀吀䔀 吀刀䤀䜀䜀䔀刀 嬀琀爀最开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开䴀漀搀椀昀礀崀ഀഀ
   ON  [classificators]਍   䄀䘀吀䔀刀 䤀一匀䔀刀吀Ⰰ唀倀䐀䄀吀䔀ഀഀ
AS ਍䈀䔀䜀䤀一ഀഀ
     UPDATE [classificators]਍     匀䔀吀  氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀 㴀 䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
           modified_by = suser_sname()਍     圀䠀䔀刀䔀 挀氀愀猀猀椀昀椀挀愀琀漀爀开椀搀 䤀一 ⠀匀䔀䰀䔀䌀吀 䐀䤀匀吀䤀一䌀吀 挀氀愀猀猀椀昀椀挀愀琀漀爀开椀搀 䘀刀伀䴀 䤀渀猀攀爀琀攀搀⤀ഀഀ
END਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 䔀一䄀䈀䰀䔀 吀刀䤀䜀䜀䔀刀 嬀琀爀最开挀氀愀猀猀椀昀椀挀愀琀漀爀猀开䴀漀搀椀昀礀崀ഀഀ
GO਍ഀഀ
-- Table - classificator_rules਍䌀刀䔀䄀吀䔀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀⠀ഀഀ
     [classificator_rule_id] [int] IDENTITY(1,1) NOT NULL,਍     嬀戀甀椀氀琀开椀渀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
     [classificator_id] [int] NOT NULL,਍     嬀洀愀猀欀开渀愀洀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
     [custom_field_1_value] [nvarchar](250) NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㈀开瘀愀氀甀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一唀䰀䰀Ⰰഀഀ
     [custom_field_3_value] [nvarchar](250) NULL,਍     嬀挀甀猀琀漀洀开昀椀攀氀搀开㐀开瘀愀氀甀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一唀䰀䰀Ⰰഀഀ
     [custom_field_5_value] [nvarchar](250) NULL,਍     嬀挀漀洀洀攀渀琀猀崀 嬀渀瘀愀爀挀栀愀爀崀⠀洀愀砀⤀ 一唀䰀䰀Ⰰഀഀ
     [creation_date] [datetime] NOT NULL,਍     嬀挀爀攀愀琀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [last_modification_date] [datetime] NOT NULL,਍     嬀洀漀搀椀昀椀攀搀开戀礀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㄀　㈀㐀⤀ 一唀䰀䰀Ⰰഀഀ
     [source_id] [int] NULL,਍ 䌀伀一匀吀刀䄀䤀一吀 嬀倀䬀开挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀 倀刀䤀䴀䄀刀夀 䬀䔀夀 䌀䰀唀匀吀䔀刀䔀䐀 ഀഀ
(਍     嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀开椀搀崀 䄀匀䌀ഀഀ
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]਍⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀 吀䔀堀吀䤀䴀䄀䜀䔀开伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_rules] ADD  CONSTRAINT [DF_classificator_rules_creation_date]  DEFAULT (getdate()) FOR [creation_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀开挀爀攀愀琀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_rules] ADD  CONSTRAINT [DF_classificator_rules_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀开洀漀搀椀昀椀攀搀开戀礀崀  䐀䔀䘀䄀唀䰀吀 ⠀猀甀猀攀爀开猀渀愀洀攀⠀⤀⤀ 䘀伀刀 嬀洀漀搀椀昀椀攀搀开戀礀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_rules]  WITH CHECK ADD  CONSTRAINT [FK_classificator_rules_classificators] FOREIGN KEY([classificator_id])਍刀䔀䘀䔀刀䔀一䌀䔀匀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀猀崀 ⠀嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开椀搀崀⤀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_rules] CHECK CONSTRAINT [FK_classificator_rules_classificators]਍䜀伀ഀഀ
਍䜀刀䄀一吀 䐀䔀䰀䔀吀䔀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 䤀一匀䔀刀吀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 匀䔀䰀䔀䌀吀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 唀倀䐀䄀吀䔀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍ഀഀ
SET IDENTITY_INSERT [classificator_rules] ON ਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (1, 1, 1, N'Address', N'Sensitive PII', N'Address', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (2, 15, 1, N'Bank Account', N'Sensitive PII', N'Bank Account', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (3, 17, 1, N'Credit Card No', N'Sensitive PII', N'Credit Card No', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (4, 18, 1, N'Date of Birth', N'Non-Sensitive PII', N'Date of Birth', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (5, 19, 1, N'Driver''s License', N'Sensitive PII', N'Driver''s License', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (6, 20, 1, N'Email', N'Sensitive PII', N'Email', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (7, 22, 1, N'Face Photo', N'Sensitive PII', N'Face Photo', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (8, 23, 1, N'Gender', N'Non-Sensitive PII', N'Gender', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (9, 26, 1, N'Login', N'Sensitive PII', N'Login', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (10, 29, 1, N'Name', N'Sensitive PII', N'Name', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (11, 30, 1, N'National ID', N'Sensitive PII', N'National ID', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (12, 31, 1, N'Passport', N'Sensitive PII', N'Passport', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (13, 33, 1, N'Phone', N'Sensitive PII', N'Phone', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (14, 36, 1, N'Social Media', N'Sensitive PII', N'Social Media', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (15, 37, 1, N'Social Security No', N'Sensitive PII', N'Social Security No', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (16, 38, 1, N'Tax ID', N'Sensitive PII', N'Tax ID', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (17, 39, 2, N'Address', N'Personal Data', N'Address', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (18, 40, 2, N'Bank Account', N'Personal Data', N'Bank Account', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (19, 41, 2, N'Biometric', N'Special Category', N'Biometric', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (20, 42, 2, N'Credit Card No', N'Personal Data', N'Credit Card No', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (21, 43, 2, N'Date of Birth', N'Personal Data', N'Date of Birth', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (22, 44, 2, N'Driver''s License', N'Personal Data', N'Driver''s License', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (23, 45, 2, N'Email', N'Personal Data', N'Email', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (24, 46, 2, N'Ethnicity', N'Special Category', N'Ethnicity', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (25, 47, 2, N'Face Photo', N'Personal Data', N'Face Photo', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (26, 48, 2, N'Gender', N'Special Category', N'Gender', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (27, 49, 2, N'Genetic', N'Special Category', N'Genetic', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (28, 50, 2, N'IP Address', N'Personal Data', N'IP Address', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (29, 51, 2, N'Login', N'Personal Data', N'Login', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (30, 52, 2, N'Marital Status', N'Personal Data', N'Marital Status', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (31, 53, 2, N'Medical Records', N'Special Category', N'Medical Records', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (32, 54, 2, N'Name', N'Personal Data', N'Name', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (33, 55, 2, N'National ID', N'Personal Data', N'National ID', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (34, 56, 2, N'Passport', N'Personal Data', N'Passport', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (35, 57, 2, N'Password', N'Personal Data', N'Password', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (36, 58, 2, N'Phone', N'Personal Data', N'Phone', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (37, 59, 2, N'Religion', N'Special Category', N'Religion', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (38, 60, 2, N'Sexual Orientation', N'Special Category', N'Sexual Orientation', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (39, 61, 2, N'Social Media', N'Personal Data', N'Social Media', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (40, 62, 2, N'Social Security No', N'Personal Data', N'Social Security No', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (41, 63, 2, N'Tax ID', N'Personal Data', N'Tax ID', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (42, 64, 2, N'Politics', N'Special Category', N'Politics', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (43, 65, 2, N'Trade Unions', N'Special Category', N'Trade Unions', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (44, 67, 1, N'Ethnicity', N'Non-Sensitive PII', N'Ethnicity', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (45, 70, 1, N'Address Location', N'Non-Sensitive PII', N'Address Location', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_rules] ([classificator_rule_id], [built_in_id], [classificator_id], [mask_name], [custom_field_1_value], [custom_field_2_value], [custom_field_3_value], [custom_field_4_value], [custom_field_5_value], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (46, 71, 2, N'Address Location', N'Personal Data', N'Address Location', NULL, NULL, NULL, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
SET IDENTITY_INSERT [classificator_rules] OFF਍䜀伀ഀഀ
਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Author:		Paweł Kwarciński਍ⴀⴀ 䌀爀攀愀琀攀 搀愀琀攀㨀     ㈀　㄀㤀ⴀ　㔀ⴀ㈀㌀ഀഀ
-- Description:	Updates last_modification_date, modified_by columns on insert or update਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
CREATE TRIGGER [trg_classificator_rules_Modify]਍   伀一  嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀ഀഀ
   AFTER INSERT,UPDATE਍䄀匀 ഀഀ
BEGIN਍     唀倀䐀䄀吀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开爀甀氀攀猀崀ഀഀ
     SET  last_modification_date = GETDATE(),਍           洀漀搀椀昀椀攀搀开戀礀 㴀 猀甀猀攀爀开猀渀愀洀攀⠀⤀ഀഀ
     WHERE classificator_rule_id IN (SELECT DISTINCT classificator_rule_id FROM Inserted)਍䔀一䐀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_rules] ENABLE TRIGGER [trg_classificator_rules_Modify]਍䜀伀ഀഀ
਍ⴀⴀ 吀愀戀氀攀 ⴀ 挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀ഀഀ
CREATE TABLE [classificator_masks](਍     嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀开椀搀崀 嬀椀渀琀崀 䤀䐀䔀一吀䤀吀夀⠀㄀Ⰰ㄀⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
     [built_in_id] [int] NULL,਍     嬀洀愀猀欀开渀愀洀攀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一伀吀 一唀䰀䰀Ⰰഀഀ
     [mask] [nvarchar](250) NOT NULL,਍     嬀搀愀琀愀开琀礀瀀攀猀崀 嬀渀瘀愀爀挀栀愀爀崀⠀㈀㔀　⤀ 一唀䰀䰀Ⰰഀഀ
     [sort] [int] NOT NULL,਍     嬀椀猀开挀漀氀甀洀渀崀 嬀戀椀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [is_title] [bit] NOT NULL,਍     嬀椀猀开搀攀猀挀爀椀瀀琀椀漀渀崀 嬀戀椀琀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [comments] [nvarchar](max) NULL,਍     嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀 嬀搀愀琀攀琀椀洀攀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [created_by] [nvarchar](1024) NULL,਍     嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀 嬀搀愀琀攀琀椀洀攀崀 一伀吀 一唀䰀䰀Ⰰഀഀ
     [modified_by] [nvarchar](1024) NULL,਍     嬀猀漀甀爀挀攀开椀搀崀 嬀椀渀琀崀 一唀䰀䰀Ⰰഀഀ
 CONSTRAINT [PK_classificator_masks] PRIMARY KEY CLUSTERED ਍⠀ഀഀ
     [classificator_mask_id] ASC਍⤀圀䤀吀䠀 ⠀倀䄀䐀开䤀一䐀䔀堀 㴀 伀䘀䘀Ⰰ 匀吀䄀吀䤀匀吀䤀䌀匀开一伀刀䔀䌀伀䴀倀唀吀䔀 㴀 伀䘀䘀Ⰰ 䤀䜀一伀刀䔀开䐀唀倀开䬀䔀夀 㴀 伀䘀䘀Ⰰ 䄀䰀䰀伀圀开刀伀圀开䰀伀䌀䬀匀 㴀 伀一Ⰰ 䄀䰀䰀伀圀开倀䄀䜀䔀开䰀伀䌀䬀匀 㴀 伀一⤀ 伀一 嬀倀刀䤀䴀䄀刀夀崀ഀഀ
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀开猀漀爀琀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀㤀㤀⤀⤀ 䘀伀刀 嬀猀漀爀琀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_is_column]  DEFAULT ((1)) FOR [is_column]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀开椀猀开琀椀琀氀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀⠀　⤀⤀ 䘀伀刀 嬀椀猀开琀椀琀氀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_is_description]  DEFAULT ((0)) FOR [is_description]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀开挀爀攀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀挀爀攀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_created_by]  DEFAULT (suser_sname()) FOR [created_by]਍䜀伀ഀഀ
਍䄀䰀吀䔀刀 吀䄀䈀䰀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 䄀䐀䐀  䌀伀一匀吀刀䄀䤀一吀 嬀䐀䘀开挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀  䐀䔀䘀䄀唀䰀吀 ⠀最攀琀搀愀琀攀⠀⤀⤀ 䘀伀刀 嬀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]਍䜀伀ഀഀ
਍䜀刀䄀一吀 䐀䔀䰀䔀吀䔀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 䤀一匀䔀刀吀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 匀䔀䰀䔀䌀吀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍䜀刀䄀一吀 唀倀䐀䄀吀䔀 伀一 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀 吀伀 嬀甀猀攀爀猀崀 䄀匀 嬀搀戀漀崀㬀ഀഀ
GO਍ഀഀ
SET IDENTITY_INSERT [classificator_masks] ON ਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (1, 1, N'Login', N'%username%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (2, 2, N'Login', N'User%name', N'text', 99, 0, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (3, 3, N'Login', N'user', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (4, 4, N'Login', N'%user_name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (5, 5, N'Login', N'%login%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (6, 6, N'Login', N'Login%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (7, 7, N'Login', N'%login', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (8, 8, N'Login', N'%updated%by', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (9, 9, N'Login', N'%created%by', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (10, 10, N'Login', N'%modified%by', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (11, 11, N'Login', N'%deleted%by', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (12, 12, N'Login', N'created', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (13, 13, N'Login', N'modified', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (14, 14, N'Login', N'last_modified', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (15, 15, N'Login', N'updated', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (16, 16, N'Login', N'last_updated', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (17, 17, N'Password', N'%pwd%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (18, 18, N'Password', N'%password%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (19, 19, N'Password', N'%passwd%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (20, 20, N'Password', N'%psswd%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (21, 21, N'Password', N'pass', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (22, 22, N'Password', N'password', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (23, 23, N'Email', N'%email%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (24, 24, N'Email', N'%e_mail%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (25, 25, N'Email', N'%e-mail%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (26, 26, N'Name', N'%first%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (27, 27, N'Name', N'%name%first%', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (28, 28, N'Name', N'f_name', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (29, 29, N'Name', N'fname', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (30, 30, N'Name', N'%middle%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (31, 31, N'Name', N'%name%middle%', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (32, 32, N'Name', N'%last%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (33, 33, N'Name', N'%name%last%', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (34, 34, N'Name', N'%surname%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (35, 35, N'Name', N'%maiden%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (36, 36, N'Name', N'%name%maiden%', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (37, 37, N'Phone', N'%phone%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (38, 38, N'Phone', N'%mobile%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (39, 39, N'Phone', N'tel%no%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (40, 40, N'Address', N'%addr%', N'text', 150, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (41, 41, N'Address', N'%address%', N'text', 120, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (42, 42, N'Address Location', N'%post_code%', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (43, 43, N'Address Location', N'%postcode%', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (44, 44, N'Address Location', N'%zipcode%', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (45, 45, N'Address Location', N'%zip_code%', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (46, 46, N'Address Location', N'zip', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (47, 47, N'Address Location', N'%postal%', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (48, 48, N'Address Location', N'%area%code%', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (49, 49, N'Address Location', N'%reset%code%', N'text, number', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (50, 50, N'Address', N'%street%', N'text', 150, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (51, 51, N'Address Location', N'%city%', N'text', 200, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (52, 52, N'Address Location', N'%province%', N'text', 200, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (53, 53, N'Address Location', N'%state%', N'text', 200, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (54, 54, N'Address Location', N'%country%', N'text', 200, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (55, 55, N'Date of Birth', N'%date_of_birth%', N'date, text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (56, 56, N'Date of Birth', N'%dateofbirth%', N'date, text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (57, 57, N'Date of Birth', N'%birthday%', N'date, text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (58, 58, N'Date of Birth', N'%birthdate%', N'date, text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (59, 59, N'Date of Birth', N'dob', N'date, text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (60, 60, N'Social Security No', N'%ssn', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (61, 61, N'Social Security No', N'%ss_num%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (62, 62, N'Social Security No', N'%ssnum%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (63, 63, N'Social Security No', N'sin', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (64, 64, N'Social Security No', N'%employee%ssn%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (65, 66, N'Social Security No', N'%social%sec%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (66, 68, N'Social Security No', N'social security number', N'text, number', 99, 1, 1, 0, N'US', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (67, 69, N'Gender', N'gender', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (68, 70, N'Gender', N'sex', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (69, 71, N'Marital Status', N'marital%stat%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (70, 72, N'National ID', N'national%id%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (71, 73, N'Passport', N'%passport%no%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (72, 74, N'Passport', N'passport', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (73, 75, N'Passport', N'%passport%num%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (74, 76, N'National ID', N'%identification%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (75, 77, N'National ID', N'%id%number%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (76, 78, N'National ID', N'%national%id%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (77, 80, N'National ID', N'itin', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (78, 82, N'IP Address', N'ip', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (79, 83, N'IP Address', N'%\_ip', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (80, 84, N'IP Address', N'ip\_%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (81, 85, N'IP Address', N'mac', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (82, 86, N'IP Address', N'%ip%addr%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (83, 87, N'IP Address', N'%mac%addr%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (84, 88, N'IP Address', N'%mac%no%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (85, 89, N'IP Address', N'%mac%num%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (86, 90, N'IP Address', N'IP address', N'text, number', 99, 0, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (87, 91, N'Name', N'%person%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (88, 92, N'Name', N'%customer%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (89, 93, N'Name', N'%client%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (90, 94, N'Name', N'%patient%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (91, 95, N'Name', N'%student%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (92, 96, N'Name', N'lname', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (93, 97, N'Name', N'l_name', N'text', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (94, 98, N'Social Media', N'%facebook%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (95, 99, N'Social Media', N'%linkedin%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (96, 100, N'Social Media', N'%twitter%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (97, 101, N'Social Media', N'%instagram%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (98, 102, N'Social Media', N'%skype%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (99, 103, N'Social Media', N'%tumblr%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (100, 104, N'Social Media', N'%pinterest%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (101, 105, N'Social Media', N'%youtube%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (102, 106, N'Social Media', N'%tik%tok%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (103, 107, N'Social Media', N'%social%media%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (104, 109, N'Social Media', N'%profile%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (105, 110, N'Social Media', N'%reddit%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (106, 111, N'Social Media', N'%viber%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (107, 112, N'Social Media', N'%snapchat%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (108, 113, N'Social Media', N'%social%profile%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (109, 114, N'Tax ID', N'%tax%id%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (110, 115, N'Tax ID', N'%tax%no%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (111, 116, N'Tax ID', N'%tax%num%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (112, 117, N'Name', N'%employee%name%', N'text', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (113, 119, N'Credit Card No', N'%credit%card%no%', N'text, number', 20, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (114, 120, N'Credit Card No', N'%credit%card%num%', N'text, number', 20, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (115, 121, N'Credit Card No', N'%cc_num%', N'text, number', 50, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (116, 122, N'Credit Card No', N'%ccnum%', N'text, number', 50, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (117, 123, N'Credit Card No', N'%credit%card%', N'text, number', 70, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (118, 124, N'Driver''s License', N'%driver%lic%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (119, 126, N'Tax ID', N'%vat%code%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (120, 127, N'Tax ID', N'%vat%no%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (121, 129, N'Tax ID', N'%vat%id%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (122, 130, N'Tax ID', N'%tax%code%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (123, 132, N'Tax ID', N'%ird%num%', N'text, number', 99, 1, 1, 0, N'New Zealand', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (124, 133, N'Tax ID', N'%ird%no%', N'text, number', 99, 1, 1, 0, N'New Zealand', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (125, 135, N'Tax ID', N'ird', N'text, number', 99, 1, 1, 0, N'New Zealand', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (126, 138, N'Tax ID', N'tfn', N'text, number', 99, 1, 1, 0, N'Australia', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (127, 141, N'Tax ID', N'tin', N'text, number', 99, 1, 0, 0, N'Tax Identification number', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (128, 142, N'Social Security No', N'ssn', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (129, 144, N'Social Security No', N'social isurance number', N'text, number', 99, 1, 1, 0, N'Canada', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (130, 146, N'Social Security No', N'sin', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (131, 148, N'Tax ID', N'%tax%ref%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (132, 149, N'Tax ID', N'utr', N'text, number', 99, 1, 0, 0, N'United Kingdom', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (133, 151, N'Tax ID', N'nip', N'text, number', 99, 1, 1, 0, N'Poland', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (134, 152, N'Social Security No', N'social security n%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (135, 153, N'Social Security No', N'social Insurance n%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (136, 155, N'Tax ID', N'%tax%file%', N'text, number', 99, 1, 1, 0, N'Australia', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (137, 156, N'Bank Account', N'%bank%acc%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (138, 157, N'Bank Account', N'iban', N'text, number', 20, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (139, 161, N'Bank Account', N'%iban%n%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (140, 162, N'Bank Account', N'%account%no%', N'text, number', 200, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (141, 163, N'Bank Account', N'%accnt%n%', N'text, number', 300, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (142, 164, N'Bank Account', N'%account%num%', N'text, number', 200, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (143, 165, N'Bank Account', N'%swift%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (144, 166, N'Credit Card No', N'ccn', N'text, number', 20, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (145, 167, N'Credit Card No', N'%debit%card%', N'text, number', 70, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (146, 168, N'Credit Card No', N'name%card%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (147, 169, N'Credit Card No', N'card%name%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (148, 170, N'Credit Card No', N'card%owner%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (149, 171, N'Credit Card No', N'%mastercard%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (150, 172, N'Credit Card No', N'%am%ex%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (151, 173, N'Credit Card No', N'%visa%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (152, 174, N'Credit Card No', N'%american%ex%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (153, 175, N'Credit Card No', N'%maestro%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (154, 176, N'Credit Card No', N'card%holder%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (155, 177, N'Face Photo', N'face%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (156, 178, N'Face Photo', N'%facial%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (157, 179, N'Face Photo', N'photo', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (158, 180, N'Biometric', N'%iris', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (159, 181, N'Biometric', N'iris%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (160, 183, N'Biometric', N'%fingerprint%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (161, 184, N'Ethnicity', N'race', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (162, 185, N'Ethnicity', N'%ethnic%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (163, 186, N'Ethnicity', N'%nationality%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (164, 187, N'Ethnicity', N'%citizenship%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (165, 189, N'Ethnicity', N'%place%birth%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (166, 191, N'Ethnicity', N'%birth%place%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (167, 192, N'Ethnicity', N'%birth%count%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (168, 194, N'Ethnicity', N'%count%birth%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (169, 195, N'Ethnicity', N'%city%birth%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (170, 197, N'Ethnicity', N'%birth%city%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (171, 198, N'Ethnicity', N'racial%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (172, 199, N'Sexual Orientation', N'%sex%orient%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (173, 200, N'Sexual Orientation', N'%sex%pref%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (174, 203, N'Medical Records', N'%medical%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (175, 204, N'Medical Records', N'%health%r', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (176, 205, N'Medical Records', N'%patient%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (177, 206, N'Medical Records', N'%clinic%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (178, 208, N'Medical Records', N'%prescription%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (179, 209, N'Medical Records', N'%drugs%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (180, 210, N'Religion', N'%religio%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (181, 212, N'Religion', N'%church%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (182, 213, N'Religion', N'%beliefs%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (183, 214, N'Genetic', N'%genetic%', NULL, 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (184, 215, N'Medical Records', N'%sickness%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (185, 216, N'Medical Records', N'%illness%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (186, 217, N'Medical Records', N'%treatment%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (187, 218, N'Medical Records', N'%disease%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (188, 219, N'Phone', N'tel%nu%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (189, 221, N'Phone', N'fax%nu%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (190, 222, N'Phone', N'fax%no%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (191, 224, N'Phone', N'tel', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (192, 225, N'Phone', N'fax', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (193, 226, N'Politics', N'%politic%', N'%', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (194, 227, N'Trade Unions', N'%trade%', N'text, number', 99, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (195, 229, N'Trade Unions', N'%union%', N'text, number', 150, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (196, 230, N'Trade Unions', N'%trad%union%', N'text, number', 20, 1, 0, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (197, 232, N'National ID', N'%insee%', N'text, number', 99, 1, 1, 0, N'France', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (198, 233, N'Tax ID', N'%steuer%id%', N'text, number', 99, 1, 1, 0, N'Germany', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (199, 234, N'National ID', N'%personenanzahl%', N'text, number', 99, 1, 1, 0, N'Germany', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (200, 235, N'Social Security No', N'%versicherung%num%', N'text, number', 99, 1, 1, 0, N'Germany', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (201, 236, N'Social Security No', N'%versicherung%no%', N'text, number', 99, 1, 1, 0, N'Germany', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (202, 238, N'Social Security No', N'vsnr', N'text, number', 99, 1, 1, 0, N'Germany', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (203, 240, N'National ID', N'%tautotita%', N'text, number', 99, 1, 1, 0, N'Greece', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (204, 243, N'Social Security No', N'%ppsn%', N'text, number', 99, 1, 1, 0, N'Ireland', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (205, 244, N'Social Security No', N'nino', N'text, number', 99, 1, 1, 0, N'UK', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (206, 245, N'National ID', N'%pesel%', NULL, 99, 1, 1, 0, N'Poland', getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
INSERT [classificator_masks] ([classificator_mask_id], [built_in_id], [mask_name], [mask], [data_types], [sort], [is_column], [is_title], [is_description], [comments], [creation_date], [created_by], [last_modification_date], [modified_by], [source_id]) VALUES (207, 247, N'Address', N'%house%', N'text, number', 99, 1, 1, 0, NULL, getdate(), N'Dataedo', getdate(), suser_sname(), NULL)਍䜀伀ഀഀ
SET IDENTITY_INSERT [classificator_masks] OFF਍䜀伀ഀഀ
਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Author:		Paweł Kwarciński਍ⴀⴀ 䌀爀攀愀琀攀 搀愀琀攀㨀     ㈀　㄀㤀ⴀ　㔀ⴀ㈀㌀ഀഀ
-- Description:	Updates last_modification_date, modified_by columns on insert or update਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
CREATE TRIGGER [trg_classificator_masks_Modify]਍   伀一  嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀ഀഀ
   AFTER INSERT,UPDATE਍䄀匀 ഀഀ
BEGIN਍     唀倀䐀䄀吀䔀 嬀挀氀愀猀猀椀昀椀挀愀琀漀爀开洀愀猀欀猀崀ഀഀ
     SET  last_modification_date = GETDATE(),਍           洀漀搀椀昀椀攀搀开戀礀 㴀 猀甀猀攀爀开猀渀愀洀攀⠀⤀ഀഀ
     WHERE classificator_mask_id IN (SELECT DISTINCT classificator_mask_id FROM Inserted)਍䔀一䐀ഀഀ
GO਍ഀഀ
ALTER TABLE [classificator_masks] ENABLE TRIGGER [trg_classificator_masks_Modify]਍䜀伀ഀഀ
਍ⴀⴀ 匀挀栀攀洀愀 䌀栀愀渀最攀 吀爀愀挀欀椀渀最 琀爀椀最最攀爀猀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一ഀഀ
GO਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一ഀഀ
GO਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Description:	Insert column's changes to schema change tracking tables਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
ALTER TRIGGER [trg_columns_change_track_update] ON [columns]਍䘀伀刀 唀倀䐀䄀吀䔀ഀഀ
AS਍     ⴀⴀ 挀栀攀挀欀 椀昀 挀漀氀甀洀渀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
     IF NOT (UPDATE([datatype]) OR UPDATE([data_length])  OR UPDATE([nullable]) OR UPDATE([default_value]) OR UPDATE([is_identity]) OR UPDATE([is_computed]) OR UPDATE([computed_formula])  OR UPDATE([name]) OR UPDATE([status]))  ਍      䈀䔀䜀䤀一ഀഀ
          RETURN;਍     䔀一䐀㬀ഀഀ
     -- skip manual objects created through Dataedo application਍     䤀䘀 䔀堀䤀匀吀匀ഀഀ
              (਍               匀䔀䰀䔀䌀吀 吀伀倀 ⠀㄀⤀ ㄀ഀഀ
                 FROM [INSERTED]਍                 圀䠀䔀刀䔀 嬀猀漀甀爀挀攀崀 㴀 ✀唀匀䔀刀✀ഀഀ
              )਍         䈀䔀䜀䤀一ഀഀ
             RETURN;਍         䔀一䐀㬀ഀഀ
     -- check if object property change ਍     䤀䘀 䔀堀䤀匀吀匀ഀഀ
              (਍               匀䔀䰀䔀䌀吀 吀伀倀 ⠀㄀⤀ ㄀ഀഀ
                 FROM [inserted] [i]਍                      䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀挀漀氀甀洀渀开椀搀崀 㴀 嬀搀崀⸀嬀挀漀氀甀洀渀开椀搀崀ഀഀ
                 WHERE਍                 ⴀⴀ椀猀渀甀氀氀⠀搀⸀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀Ⰰ　⤀ 㰀㸀 椀⸀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀 伀刀ഀഀ
                 ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')਍                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰ✀✀⤀ഀഀ
                 OR ISNULL([d].[nullable],'') <> ISNULL([i].[nullable],'')਍                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀攀昀愀甀氀琀开瘀愀氀甀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀攀昀愀甀氀琀开瘀愀氀甀攀崀Ⰰ✀✀⤀ഀഀ
                 OR ISNULL([d].[is_identity],'') <> ISNULL([i].[is_identity],'')਍                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀椀猀开挀漀洀瀀甀琀攀搀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀椀猀开挀漀洀瀀甀琀攀搀崀Ⰰ✀✀⤀ഀഀ
                 OR ISNULL([d].[computed_formula],'') <> ISNULL([i].[computed_formula],'')਍                 伀刀 嬀搀崀⸀嬀渀愀洀攀崀 㰀㸀 嬀椀崀⸀嬀渀愀洀攀崀ഀഀ
              )਍         䈀䔀䜀䤀一ഀഀ
             UPDATE [columns_changes]਍               匀䔀吀ഀഀ
                   [valid_to] = GETDATE()਍               䘀刀伀䴀 嬀挀漀氀甀洀渀猀开挀栀愀渀最攀猀崀 嬀挀崀ഀഀ
                    INNER JOIN [inserted] [u] ON [c].[column_id] = [u].[column_id]਍               圀䠀䔀刀䔀 嬀瘀愀氀椀搀开琀漀崀 䤀匀 一唀䰀䰀㬀ഀഀ
             -- insert changes (before and after values) for columns - when column was updated਍             䤀一匀䔀刀吀 䤀一吀伀 嬀挀漀氀甀洀渀猀开挀栀愀渀最攀猀崀ഀഀ
             ([column_id],਍              嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
              [database_id],਍              嬀琀愀戀氀攀开猀挀栀攀洀愀崀Ⰰഀഀ
              [table_name],਍              嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
              [column_name],਍              嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
              [data_length],਍              嬀渀甀氀氀愀戀氀攀崀Ⰰഀഀ
              [default_value],਍              嬀椀猀开椀搀攀渀琀椀琀礀崀Ⰰഀഀ
              [is_computed],਍              嬀挀漀洀瀀甀琀攀搀开昀漀爀洀甀氀愀崀Ⰰഀഀ
              [before_ordinal_position],਍              嬀戀攀昀漀爀攀开挀漀氀甀洀渀开渀愀洀攀崀Ⰰഀഀ
              [before_datatype],਍              嬀戀攀昀漀爀攀开搀愀琀愀开氀攀渀最琀栀崀Ⰰഀഀ
              [before_nullable],਍              嬀戀攀昀漀爀攀开搀攀昀愀甀氀琀开瘀愀氀甀攀崀Ⰰഀഀ
              [before_is_identity],਍              嬀戀攀昀漀爀攀开椀猀开挀漀洀瀀甀琀攀搀崀Ⰰഀഀ
              [before_computed_formula],਍              嬀漀瀀攀爀愀琀椀漀渀崀Ⰰഀഀ
              [valid_from],਍              嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
             )਍                    匀䔀䰀䔀䌀吀 嬀椀崀⸀嬀挀漀氀甀洀渀开椀搀崀Ⰰഀഀ
                           [i].[table_id],਍                           嬀琀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
                           [t].[schema] AS [table_schema],਍                           嬀琀崀⸀嬀渀愀洀攀崀 䄀匀 嬀琀愀戀氀攀开渀愀洀攀崀Ⰰഀഀ
                           [i].[ordinal_position],਍                           嬀椀崀⸀嬀渀愀洀攀崀 䄀匀 嬀挀漀氀甀洀渀开渀愀洀攀崀Ⰰഀഀ
                           [i].[datatype],਍                           嬀椀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰഀഀ
                           [i].[nullable],਍                           嬀椀崀⸀嬀搀攀昀愀甀氀琀开瘀愀氀甀攀崀Ⰰഀഀ
                           [i].[is_identity],਍                           嬀椀崀⸀嬀椀猀开挀漀洀瀀甀琀攀搀崀Ⰰഀഀ
                           [i].[computed_formula],਍                           嬀搀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
                           [d].[name] AS [column_name],਍                           嬀搀崀⸀嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
                           [d].[data_length],਍                           嬀搀崀⸀嬀渀甀氀氀愀戀氀攀崀Ⰰഀഀ
                           [d].[default_value],਍                           嬀搀崀⸀嬀椀猀开椀搀攀渀琀椀琀礀崀Ⰰഀഀ
                           [d].[is_computed],਍                           嬀搀崀⸀嬀挀漀洀瀀甀琀攀搀开昀漀爀洀甀氀愀崀Ⰰഀഀ
                           'UPDATED',਍                           嬀挀挀崀⸀嬀瘀愀氀椀搀开琀漀崀Ⰰഀഀ
                           [i].[update_id]਍                      䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                           INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]਍                           伀唀吀䔀刀 䄀倀倀䰀夀ഀഀ
                                      (਍                                       匀䔀䰀䔀䌀吀 䴀䄀堀⠀嬀瘀愀氀椀搀开琀漀崀⤀ 䄀匀 嬀瘀愀氀椀搀开琀漀崀ഀഀ
                                         FROM [columns_changes] [c]਍                                         圀䠀䔀刀䔀 嬀挀崀⸀嬀挀漀氀甀洀渀开椀搀崀 㴀 嬀搀崀⸀嬀挀漀氀甀洀渀开椀搀崀ഀഀ
                                               AND [c].[valid_to] IS NOT NULL਍                                      ⤀ 嬀挀挀崀ഀഀ
                           LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]਍                      圀䠀䔀刀䔀 嬀椀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀ഀഀ
                            AND [d].[status] = 'A'਍                            䄀一䐀 ⠀䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰ　⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰ　⤀ഀഀ
                                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[nullable],'') <> ISNULL([i].[nullable],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀攀昀愀甀氀琀开瘀愀氀甀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀攀昀愀甀氀琀开瘀愀氀甀攀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[is_identity],'') <> ISNULL([i].[is_identity],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀椀猀开挀漀洀瀀甀琀攀搀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀椀猀开挀漀洀瀀甀琀攀搀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[computed_formula],'') <> ISNULL([i].[computed_formula],'')਍                                 伀刀 嬀搀崀⸀嬀渀愀洀攀崀 㰀㸀 嬀椀崀⸀嬀渀愀洀攀崀⤀㬀ഀഀ
         END;਍    䈀䔀䜀䤀一ഀഀ
        -- insert changes (deleted values) for columns - when column was deleted਍        䤀一匀䔀刀吀 䤀一吀伀 嬀挀漀氀甀洀渀猀开挀栀愀渀最攀猀崀ഀഀ
        ([column_id],਍         嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
         [database_id],਍         嬀琀愀戀氀攀开猀挀栀攀洀愀崀Ⰰഀഀ
         [table_name],਍         嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
         [column_name],਍         嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
         [data_length],਍         嬀渀甀氀氀愀戀氀攀崀Ⰰഀഀ
         [default_value],਍         嬀椀猀开椀搀攀渀琀椀琀礀崀Ⰰഀഀ
         [is_computed],਍         嬀挀漀洀瀀甀琀攀搀开昀漀爀洀甀氀愀崀Ⰰഀഀ
         [operation],਍         嬀瘀愀氀椀搀开琀漀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [d].[column_id],਍                      嬀搀崀⸀嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
                      [t].[database_id],਍                      嬀琀崀⸀嬀猀挀栀攀洀愀崀 䄀匀 嬀琀愀戀氀攀开猀挀栀攀洀愀崀Ⰰഀഀ
                      [t].[name] AS [table_name],਍                      嬀搀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
                      [d].[name] AS [column_name],਍                      嬀搀崀⸀嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
                      [d].[data_length],਍                      嬀搀崀⸀嬀渀甀氀氀愀戀氀攀崀Ⰰഀഀ
                      [d].[default_value],਍                      嬀搀崀⸀嬀椀猀开椀搀攀渀琀椀琀礀崀Ⰰഀഀ
                      [d].[is_computed],਍                      嬀搀崀⸀嬀挀漀洀瀀甀琀攀搀开昀漀爀洀甀氀愀崀Ⰰഀഀ
                      'DELETED',਍                      䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
                      [i].[update_id]਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]਍                      䰀䔀䘀吀 伀唀吀䔀刀 䨀伀䤀一 嬀琀愀戀氀攀猀崀 嬀琀崀 伀一 嬀椀崀⸀嬀琀愀戀氀攀开椀搀崀 㴀 嬀琀崀⸀嬀琀愀戀氀攀开椀搀崀ഀഀ
                 WHERE [i].[status] = 'D'਍                       䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀㬀ഀഀ
        -- insert changes (updated values) for columns - when column was restored਍        䤀一匀䔀刀吀 䤀一吀伀 嬀挀漀氀甀洀渀猀开挀栀愀渀最攀猀崀ഀഀ
        ([column_id],਍         嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
         [database_id],਍         嬀琀愀戀氀攀开猀挀栀攀洀愀崀Ⰰഀഀ
         [table_name],਍         嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
         [column_name],਍         嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
         [data_length],਍         嬀渀甀氀氀愀戀氀攀崀Ⰰഀഀ
         [default_value],਍         嬀椀猀开椀搀攀渀琀椀琀礀崀Ⰰഀഀ
         [is_computed],਍         嬀挀漀洀瀀甀琀攀搀开昀漀爀洀甀氀愀崀Ⰰഀഀ
         [operation],਍         嬀瘀愀氀椀搀开昀爀漀洀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [i].[column_id],਍                      嬀椀崀⸀嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
                      [t].[database_id],਍                      嬀琀崀⸀嬀猀挀栀攀洀愀崀 䄀匀 嬀琀愀戀氀攀开猀挀栀攀洀愀崀Ⰰഀഀ
                      [t].[name] AS [table_name],਍                      嬀椀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
                      [i].[name] AS [column_name],਍                      嬀椀崀⸀嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
                      [i].[data_length],਍                      嬀椀崀⸀嬀渀甀氀氀愀戀氀攀崀Ⰰഀഀ
                      [i].[default_value],਍                      嬀椀崀⸀嬀椀猀开椀搀攀渀琀椀琀礀崀Ⰰഀഀ
                      [i].[is_computed],਍                      嬀椀崀⸀嬀挀漀洀瀀甀琀攀搀开昀漀爀洀甀氀愀崀Ⰰഀഀ
                      'ADDED',਍                      䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
                      [i].[update_id]਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]਍                      䰀䔀䘀吀 伀唀吀䔀刀 䨀伀䤀一 嬀琀愀戀氀攀猀崀 嬀琀崀 伀一 嬀椀崀⸀嬀琀愀戀氀攀开椀搀崀 㴀 嬀琀崀⸀嬀琀愀戀氀攀开椀搀崀ഀഀ
                 WHERE [i].[status] = 'A'਍                       䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䐀✀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一ഀഀ
GO਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一ഀഀ
GO਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Description:	Insert parameter's changes to schema change tracking tables਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
ALTER TRIGGER [trg_parameters_change_track_update] ON [parameters]਍䘀伀刀 唀倀䐀䄀吀䔀ഀഀ
AS਍     ⴀⴀ 挀栀攀挀欀 椀昀 瀀愀爀愀洀攀琀攀爀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
     IF NOT (UPDATE([ordinal_position]) OR UPDATE([parameter_mode])  OR UPDATE([datatype]) OR UPDATE([data_length]) OR UPDATE([status]))  ਍      䈀䔀䜀䤀一ഀഀ
          RETURN;਍     䔀一䐀㬀ഀഀ
਍     ⴀⴀ 猀欀椀瀀 洀愀渀甀愀氀 漀戀樀攀挀琀猀 挀爀攀愀琀攀搀 琀栀爀漀甀最栀 䐀愀琀愀攀搀漀 愀瀀瀀氀椀挀愀琀椀漀渀ഀഀ
     IF EXISTS਍              ⠀ഀഀ
               SELECT TOP (1) 1਍                 䘀刀伀䴀 嬀䤀一匀䔀刀吀䔀䐀崀ഀഀ
                 WHERE [source] = 'USER'਍              ⤀ഀഀ
         BEGIN਍             刀䔀吀唀刀一㬀ഀഀ
         END;਍     ⴀⴀ 挀栀攀挀欀 椀昀 漀戀樀攀挀琀 瀀爀漀瀀攀爀琀礀 挀栀愀渀最攀ഀഀ
     IF EXISTS਍              ⠀ഀഀ
               SELECT TOP (1) 1਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]਍                 圀䠀䔀刀䔀ഀഀ
                 --isnull(d.[name],'') <> i.[name] or਍                 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰ✀✀⤀ഀഀ
                 OR ISNULL([d].[parameter_mode],'') <> ISNULL([i].[parameter_mode],'')਍                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀愀琀愀琀礀瀀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀愀琀愀琀礀瀀攀崀Ⰰ✀✀⤀ഀഀ
                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')਍              ⤀ഀഀ
         BEGIN਍             唀倀䐀䄀吀䔀 嬀瀀愀爀愀洀攀琀攀爀猀开挀栀愀渀最攀猀崀ഀഀ
               SET਍                   嬀瘀愀氀椀搀开琀漀崀 㴀 䜀䔀吀䐀䄀吀䔀⠀⤀ഀഀ
               FROM [parameters_changes] [c]਍                    䤀一一䔀刀 䨀伀䤀一 嬀椀渀猀攀爀琀攀搀崀 嬀甀崀 伀一 嬀挀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀 㴀 嬀甀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀ഀഀ
               WHERE [valid_to] IS NULL;਍             ⴀⴀ 椀渀猀攀爀琀 挀栀愀渀最攀猀 ⠀戀攀昀漀爀攀 愀渀搀 愀昀琀攀爀 瘀愀氀甀攀猀⤀ 昀漀爀 瀀愀爀愀洀攀琀攀爀 ⴀ 眀栀攀渀 瀀愀爀愀洀攀琀攀爀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
             INSERT INTO [parameters_changes]਍             ⠀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
              [parameter_id],਍              嬀瀀爀漀挀攀搀甀爀攀开椀搀崀Ⰰഀഀ
              [ordinal_position],਍              嬀瀀愀爀愀洀攀琀攀爀开洀漀搀攀崀Ⰰഀഀ
              [name],਍              嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
              [data_length],਍              嬀戀攀昀漀爀攀开漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
              [before_parameter_mode],਍              嬀戀攀昀漀爀攀开搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
              [before_data_length],਍              嬀漀瀀攀爀愀琀椀漀渀崀Ⰰഀഀ
              [valid_from],਍              嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
             )਍                    匀䔀䰀䔀䌀吀 嬀瀀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
                           [i].[parameter_id],਍                           嬀椀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀Ⰰഀഀ
                           [i].[ordinal_position],਍                           嬀椀崀⸀嬀瀀愀爀愀洀攀琀攀爀开洀漀搀攀崀Ⰰഀഀ
                           [i].[name],਍                           嬀椀崀⸀嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
                           [i].[data_length],਍                           嬀搀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
                           [d].[parameter_mode],਍                           嬀搀崀⸀嬀搀愀琀愀琀礀瀀攀崀Ⰰഀഀ
                           [d].[data_length],਍                           ✀唀倀䐀䄀吀䔀䐀✀Ⰰഀഀ
                           [cc].[valid_to],਍                           嬀椀崀⸀嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
                      FROM [inserted] [i]਍                           䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀瀀愀爀愀洀攀琀攀爀开椀搀崀 㴀 嬀搀崀⸀嬀瀀愀爀愀洀攀琀攀爀开椀搀崀ഀഀ
                           JOIN [procedures] [p] ON [d].[procedure_id] = [p].[procedure_id]਍                           伀唀吀䔀刀 䄀倀倀䰀夀ഀഀ
                                      (਍                                       匀䔀䰀䔀䌀吀 䴀䄀堀⠀嬀瘀愀氀椀搀开琀漀崀⤀ 䄀匀 嬀瘀愀氀椀搀开琀漀崀ഀഀ
                                         FROM [procedures_changes] [c]਍                                         圀䠀䔀刀䔀 嬀挀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀 㴀 嬀搀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀ഀഀ
                                               AND [c].[valid_to] IS NOT NULL਍                                      ⤀ 嬀挀挀崀ഀഀ
                      WHERE [i].[status] = 'A'਍                            䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀ഀഀ
                            AND (ISNULL([d].[ordinal_position],'') <> ISNULL([i].[ordinal_position],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀瀀愀爀愀洀攀琀攀爀开洀漀搀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀瀀愀爀愀洀攀琀攀爀开洀漀搀攀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰ✀✀⤀⤀㬀ഀഀ
         END;਍    䈀䔀䜀䤀一ഀഀ
        -- insert changes (deleted values) for parameter - when parameter was deleted਍        䤀一匀䔀刀吀 䤀一吀伀 嬀瀀愀爀愀洀攀琀攀爀猀开挀栀愀渀最攀猀崀ഀഀ
        ([database_id],਍         嬀瀀愀爀愀洀攀琀攀爀开椀搀崀Ⰰഀഀ
         [procedure_id],਍         嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
         [parameter_mode],਍         嬀渀愀洀攀崀Ⰰഀഀ
         [datatype],਍         嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰഀഀ
         [operation],਍         嬀瘀愀氀椀搀开琀漀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [p].[database_id],਍                      嬀搀崀⸀嬀瀀愀爀愀洀攀琀攀爀开椀搀崀Ⰰഀഀ
                      [d].[procedure_id],਍                      嬀搀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
                      [d].[parameter_mode],਍                      嬀搀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                      [d].[datatype],਍                      嬀搀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰഀഀ
                      'DELETED',਍                      䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
                      [i].[update_id]਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]਍                      䨀伀䤀一 嬀瀀爀漀挀攀搀甀爀攀猀崀 嬀瀀崀 伀一 嬀搀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀 㴀 嬀瀀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀ഀഀ
                 WHERE [i].[status] = 'D'਍                       䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀㬀ഀഀ
        -- insert changes (updated values) for parameter - when parameter was restored਍        䤀一匀䔀刀吀 䤀一吀伀 嬀瀀愀爀愀洀攀琀攀爀猀开挀栀愀渀最攀猀崀ഀഀ
        ([database_id],਍         嬀瀀愀爀愀洀攀琀攀爀开椀搀崀Ⰰഀഀ
         [procedure_id],਍         嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
         [parameter_mode],਍         嬀渀愀洀攀崀Ⰰഀഀ
         [datatype],਍         嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰഀഀ
         [operation],਍         嬀瘀愀氀椀搀开昀爀漀洀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [p].[database_id],਍                      嬀椀崀⸀嬀瀀愀爀愀洀攀琀攀爀开椀搀崀Ⰰഀഀ
                      [i].[procedure_id],਍                      嬀椀崀⸀嬀漀爀搀椀渀愀氀开瀀漀猀椀琀椀漀渀崀Ⰰഀഀ
                      [i].[parameter_mode],਍                      嬀椀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                      [i].[datatype],਍                      嬀椀崀⸀嬀搀愀琀愀开氀攀渀最琀栀崀Ⰰഀഀ
                      'ADDED',਍                      䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
                      [i].[update_id]਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]਍                      䨀伀䤀一 嬀瀀爀漀挀攀搀甀爀攀猀崀 嬀瀀崀 伀一 嬀搀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀 㴀 嬀瀀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀ഀഀ
                 WHERE [i].[status] = 'A'਍                       䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䐀✀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一ഀഀ
GO਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一ഀഀ
GO਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Description:	Insert procedure's changes to schema change tracking tables਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
ALTER TRIGGER [trg_procedures_change_track_update] ON [procedures]਍䘀伀刀 唀倀䐀䄀吀䔀ഀഀ
AS਍     ⴀⴀ 挀栀攀挀欀 椀昀 瀀爀漀挀攀搀甀爀攀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
     IF NOT (UPDATE([schema]) OR UPDATE([name])  OR UPDATE([object_type]) OR UPDATE([definition]) OR UPDATE([function_type]) OR UPDATE([subtype]) OR UPDATE([dbms_last_modification_date]) OR UPDATE([status]))  ਍      䈀䔀䜀䤀一ഀഀ
          RETURN;਍     䔀一䐀㬀ഀഀ
਍     ⴀⴀ 猀欀椀瀀 洀愀渀甀愀氀 漀戀樀攀挀琀猀 挀爀攀愀琀攀搀 琀栀爀漀甀最栀 䐀愀琀愀攀搀漀 愀瀀瀀氀椀挀愀琀椀漀渀ഀഀ
     IF EXISTS਍              ⠀ഀഀ
               SELECT TOP (1) 1਍                 䘀刀伀䴀 嬀䤀一匀䔀刀吀䔀䐀崀ഀഀ
                 WHERE [source] = 'USER'਍              ⤀ഀഀ
         BEGIN਍             刀䔀吀唀刀一㬀ഀഀ
         END;਍     ⴀⴀ 挀栀攀挀欀 椀昀 漀戀樀攀挀琀 瀀爀漀瀀攀爀琀礀 挀栀愀渀最攀 ഀഀ
     IF EXISTS਍              ⠀ഀഀ
               SELECT TOP (1) 1਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]਍                 圀䠀䔀刀䔀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀猀挀栀攀洀愀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀猀挀栀攀洀愀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀昀甀渀挀琀椀漀渀开琀礀瀀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀昀甀渀挀琀椀漀渀开琀礀瀀攀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ✀✀⤀ഀഀ
              )਍         䈀䔀䜀䤀一ഀഀ
             UPDATE [procedures_changes]਍               匀䔀吀ഀഀ
                   [valid_to] = GETDATE()਍               䘀刀伀䴀 嬀瀀爀漀挀攀搀甀爀攀猀开挀栀愀渀最攀猀崀 嬀挀崀ഀഀ
                    INNER JOIN [inserted] [u] ON [c].[procedure_id] = [u].[procedure_id]਍               圀䠀䔀刀䔀 嬀瘀愀氀椀搀开琀漀崀 䤀匀 一唀䰀䰀㬀ഀഀ
             -- insert changes (before and after values) for procedure - when procedure was updated਍             䤀一匀䔀刀吀 䤀一吀伀 嬀瀀爀漀挀攀搀甀爀攀猀开挀栀愀渀最攀猀崀ഀഀ
             ([database_id],਍              嬀瀀爀漀挀攀搀甀爀攀开椀搀崀Ⰰഀഀ
              [schema],਍              嬀渀愀洀攀崀Ⰰഀഀ
              [object_type],਍              嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
              [function_type],਍              嬀猀甀戀琀礀瀀攀崀Ⰰഀഀ
              [before_object_type],਍              嬀戀攀昀漀爀攀开搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
              [before_function_type],਍              嬀戀攀昀漀爀攀开猀甀戀琀礀瀀攀崀Ⰰഀഀ
              [operation],਍              嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰഀഀ
              [valid_from],਍              嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
             )਍                    匀䔀䰀䔀䌀吀 嬀椀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
                           [i].[procedure_id],਍                           嬀椀崀⸀嬀猀挀栀攀洀愀崀Ⰰഀഀ
                           [i].[name],਍                           嬀椀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
                           [i].[definition],਍                           嬀椀崀⸀嬀昀甀渀挀琀椀漀渀开琀礀瀀攀崀Ⰰഀഀ
                           [i].[subtype],਍                           嬀搀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
                           [d].[definition],਍                           嬀搀崀⸀嬀昀甀渀挀琀椀漀渀开琀礀瀀攀崀Ⰰഀഀ
                           [d].[subtype],਍                           ✀唀倀䐀䄀吀䔀䐀✀Ⰰഀഀ
                           [i].[dbms_last_modification_date],਍                           嬀挀挀崀⸀嬀瘀愀氀椀搀开琀漀崀Ⰰഀഀ
                           [i].[update_id]਍                      䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                           INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]਍                           伀唀吀䔀刀 䄀倀倀䰀夀ഀഀ
                                      (਍                                       匀䔀䰀䔀䌀吀 䴀䄀堀⠀嬀瘀愀氀椀搀开琀漀崀⤀ 䄀匀 嬀瘀愀氀椀搀开琀漀崀ഀഀ
                                         FROM [procedures_changes] [c]਍                                         圀䠀䔀刀䔀 嬀挀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀 㴀 嬀搀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀ഀഀ
                                               AND [c].[valid_to] IS NOT NULL਍                                      ⤀ 嬀挀挀崀ഀഀ
                      WHERE [i].[status] = 'A'਍                            䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀ഀഀ
                            AND (ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[function_type],'') <> ISNULL([i].[function_type],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀猀甀戀琀礀瀀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀猀甀戀琀礀瀀攀崀Ⰰ✀✀⤀⤀㬀ഀഀ
         END;਍    䈀䔀䜀䤀一ഀഀ
        -- insert changes (deleted values) for procedure - when procedure was deleted਍        䤀一匀䔀刀吀 䤀一吀伀 嬀瀀爀漀挀攀搀甀爀攀猀开挀栀愀渀最攀猀崀ഀഀ
        ([database_id],਍         嬀瀀爀漀挀攀搀甀爀攀开椀搀崀Ⰰഀഀ
         [schema],਍         嬀渀愀洀攀崀Ⰰഀഀ
         [object_type],਍         嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
         [function_type],਍         嬀猀甀戀琀礀瀀攀崀Ⰰഀഀ
         [operation],਍         嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰഀഀ
         [valid_to],਍         嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
        )਍               匀䔀䰀䔀䌀吀 嬀搀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
                      [d].[procedure_id],਍                      嬀搀崀⸀嬀猀挀栀攀洀愀崀Ⰰഀഀ
                      [d].[name],਍                      嬀搀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
                      [d].[definition],਍                      嬀搀崀⸀嬀昀甀渀挀琀椀漀渀开琀礀瀀攀崀Ⰰഀഀ
                      [d].[subtype],਍                      ✀䐀䔀䰀䔀吀䔀䐀✀Ⰰഀഀ
                      [i].[dbms_last_modification_date],਍                      䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
                      [i].[update_id]਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]਍                 圀䠀䔀刀䔀 嬀椀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䐀✀ഀഀ
                       AND [d].[status] = 'A';਍        ⴀⴀ椀渀猀攀爀琀 挀栀愀渀最攀猀 ⠀甀瀀搀愀琀攀搀 瘀愀氀甀攀猀⤀ 昀漀爀 瀀爀漀挀攀搀甀爀攀 ⴀ 眀栀攀渀 瀀爀漀挀攀搀甀爀攀 眀愀猀 爀攀猀琀漀爀攀搀ഀഀ
        INSERT INTO [procedures_changes]਍        ⠀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
         [procedure_id],਍         嬀猀挀栀攀洀愀崀Ⰰഀഀ
         [name],਍         嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
         [definition],਍         嬀昀甀渀挀琀椀漀渀开琀礀瀀攀崀Ⰰഀഀ
         [subtype],਍         嬀漀瀀攀爀愀琀椀漀渀崀Ⰰഀഀ
         [dbms_last_modification_date],਍         嬀瘀愀氀椀搀开昀爀漀洀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [i].[database_id],਍                      嬀椀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀Ⰰഀഀ
                      [i].[schema],਍                      嬀椀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                      [i].[object_type],਍                      嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
                      [i].[function_type],਍                      嬀椀崀⸀嬀猀甀戀琀礀瀀攀崀Ⰰഀഀ
                      'ADDED',਍                      嬀椀崀⸀嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰഀഀ
                      GETDATE(),਍                      嬀椀崀⸀嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
                 FROM [inserted] [i]਍                      䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀 㴀 嬀搀崀⸀嬀瀀爀漀挀攀搀甀爀攀开椀搀崀ഀഀ
                 WHERE [i].[status] = 'A'਍                       䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䐀✀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一ഀഀ
GO਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一ഀഀ
GO਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Description:	Insert table's changes to schema change tracking tables਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
ALTER TRIGGER [trg_tables_change_track_update] ON [tables]਍䘀伀刀 唀倀䐀䄀吀䔀ഀഀ
AS਍     ⴀⴀ 挀栀攀挀欀 椀昀 琀愀戀氀攀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
     IF NOT (UPDATE([schema]) OR UPDATE([name])  OR UPDATE([object_type]) OR UPDATE([subtype]) OR UPDATE([definition]) OR UPDATE([dbms_last_modification_date]) OR UPDATE([status]))  ਍      䈀䔀䜀䤀一ഀഀ
          RETURN;਍     䔀一䐀㬀ഀഀ
਍     ⴀⴀ 猀欀椀瀀 洀愀渀甀愀氀 漀戀樀攀挀琀猀 挀爀攀愀琀攀搀 琀栀爀漀甀最栀 䐀愀琀愀攀搀漀 愀瀀瀀氀椀挀愀琀椀漀渀ഀഀ
     IF EXISTS਍              ⠀ഀഀ
               SELECT TOP (1) 1਍                 䘀刀伀䴀 嬀䤀一匀䔀刀吀䔀䐀崀ഀഀ
                 WHERE [source] = 'USER'਍              ⤀ഀഀ
         BEGIN਍             刀䔀吀唀刀一㬀ഀഀ
         END;਍     ⴀⴀ 挀栀攀挀欀 椀昀 漀戀樀攀挀琀 瀀爀漀瀀攀爀琀礀 挀栀愀渀最攀 ഀഀ
     IF EXISTS਍              ⠀ഀഀ
               SELECT TOP (1) 1਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]਍                 圀䠀䔀刀䔀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀猀挀栀攀洀愀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀猀挀栀攀洀愀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[dbms_last_modification_date],'') <> ISNULL([i].[dbms_last_modification_date],'')਍              ⤀ഀഀ
         BEGIN਍             唀倀䐀䄀吀䔀 嬀琀愀戀氀攀猀开挀栀愀渀最攀猀崀ഀഀ
               SET਍                   嬀瘀愀氀椀搀开琀漀崀 㴀 䜀䔀吀䐀䄀吀䔀⠀⤀ഀഀ
               FROM [tables_changes] [c]਍                    䤀一一䔀刀 䨀伀䤀一 嬀椀渀猀攀爀琀攀搀崀 嬀甀崀 伀一 嬀挀崀⸀嬀琀愀戀氀攀开椀搀崀 㴀 嬀甀崀⸀嬀琀愀戀氀攀开椀搀崀ഀഀ
               WHERE [valid_to] IS NULL;਍             ⴀⴀ 椀渀猀攀爀琀 挀栀愀渀最攀猀 ⠀戀攀昀漀爀攀 愀渀搀 愀昀琀攀爀 瘀愀氀甀攀猀⤀ 昀漀爀 琀愀戀氀攀 ⴀ 眀栀攀渀 琀愀戀氀攀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
             INSERT INTO [tables_changes]਍             ⠀嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
              [database_id],਍              嬀猀挀栀攀洀愀崀Ⰰഀഀ
              [name],਍              嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
              [subtype],਍              嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
              [before_schema],਍              嬀戀攀昀漀爀攀开渀愀洀攀崀Ⰰഀഀ
              [before_object_type],਍              嬀戀攀昀漀爀攀开猀甀戀琀礀瀀攀崀Ⰰഀഀ
              [before_definition],਍              嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰഀഀ
              [operation],਍              嬀瘀愀氀椀搀开昀爀漀洀崀Ⰰഀഀ
              [update_id]਍             ⤀ഀഀ
                    SELECT [i].[table_id],਍                           嬀椀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
                           [i].[schema],਍                           嬀椀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                           [i].[object_type],਍                           嬀椀崀⸀嬀猀甀戀琀礀瀀攀崀Ⰰഀഀ
                           [i].[definition],਍                           嬀搀崀⸀嬀猀挀栀攀洀愀崀Ⰰഀഀ
                           [d].[name],਍                           嬀搀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
                           [d].[subtype],਍                           嬀搀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
                           [i].[dbms_last_modification_date],਍                           ✀唀倀䐀䄀吀䔀䐀✀Ⰰഀഀ
                           [cc].[valid_to],਍                           嬀椀崀⸀嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
                      FROM [inserted] [i]਍                           䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀琀愀戀氀攀开椀搀崀 㴀 嬀搀崀⸀嬀琀愀戀氀攀开椀搀崀ഀഀ
                           OUTER APPLY਍                                      ⠀ഀഀ
                                       SELECT MAX([valid_to]) AS [valid_to]਍                                         䘀刀伀䴀 嬀琀愀戀氀攀猀开挀栀愀渀最攀猀崀 嬀挀崀ഀഀ
                                         WHERE [c].[table_id] = [d].[table_id]਍                                               䄀一䐀 嬀挀崀⸀嬀瘀愀氀椀搀开琀漀崀 䤀匀 一伀吀 一唀䰀䰀ഀഀ
                                      ) [cc]਍                      圀䠀䔀刀䔀 嬀椀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀ഀഀ
                            AND [d].[status] = 'A'਍                            ⴀⴀ 愀渀搀 椀猀渀甀氀氀⠀搀⸀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀Ⰰ✀✀⤀ 㰀㸀 椀⸀氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀ഀഀ
                            AND (ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀猀甀戀琀礀瀀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀猀甀戀琀礀瀀攀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰ✀✀⤀⤀㬀ഀഀ
         END;਍    䈀䔀䜀䤀一ഀഀ
        -- insert changes (deleted values) for table - when table was deleted਍        䤀一匀䔀刀吀 䤀一吀伀 嬀琀愀戀氀攀猀开挀栀愀渀最攀猀崀ഀഀ
        ([table_id],਍         嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
         [schema],਍         嬀渀愀洀攀崀Ⰰഀഀ
         [object_type],਍         嬀猀甀戀琀礀瀀攀崀Ⰰഀഀ
         [definition],਍         嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰഀഀ
         [operation],਍         嬀瘀愀氀椀搀开琀漀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [d].[table_id],਍                      嬀搀崀⸀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
                      [d].[schema],਍                      嬀搀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                      [d].[object_type],਍                      嬀搀崀⸀嬀猀甀戀琀礀瀀攀崀Ⰰഀഀ
                      [d].[definition],਍                      嬀椀崀⸀嬀搀戀洀猀开氀愀猀琀开洀漀搀椀昀椀挀愀琀椀漀渀开搀愀琀攀崀Ⰰഀഀ
                      'DELETED',਍                      䜀䔀吀䐀䄀吀䔀⠀⤀Ⰰഀഀ
                      [i].[update_id]਍                 䘀刀伀䴀 嬀椀渀猀攀爀琀攀搀崀 嬀椀崀ഀഀ
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]਍                 圀䠀䔀刀䔀 嬀椀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䐀✀ഀഀ
                       AND [d].[status] = 'A';਍        ⴀⴀ 椀渀猀攀爀琀 挀栀愀渀最攀猀 ⠀甀瀀搀愀琀攀搀 瘀愀氀甀攀猀⤀ 昀漀爀 琀愀戀氀攀 ⴀ 眀栀攀渀 琀愀戀氀攀 眀愀猀 爀攀猀琀漀爀攀搀ഀഀ
        INSERT INTO [tables_changes]਍        ⠀嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
         [database_id],਍         嬀猀挀栀攀洀愀崀Ⰰഀഀ
         [name],਍         嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
         [subtype],਍         嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
         [dbms_last_modification_date],਍         嬀漀瀀攀爀愀琀椀漀渀崀Ⰰഀഀ
         [valid_from],਍         嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
        )਍               匀䔀䰀䔀䌀吀 嬀椀崀⸀嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
                      [i].[database_id],਍                      嬀椀崀⸀嬀猀挀栀攀洀愀崀Ⰰഀഀ
                      [i].[name],਍                      嬀椀崀⸀嬀漀戀樀攀挀琀开琀礀瀀攀崀Ⰰഀഀ
                      [i].[subtype],਍                      嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
                      [i].[dbms_last_modification_date],਍                      ✀䄀䐀䐀䔀䐀✀Ⰰഀഀ
                      GETDATE(),਍                      嬀椀崀⸀嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
                 FROM [inserted] [i]਍                      䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀琀愀戀氀攀开椀搀崀 㴀 嬀搀崀⸀嬀琀愀戀氀攀开椀搀崀ഀഀ
                 WHERE [i].[status] = 'A'਍                       䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䐀✀㬀ഀഀ
    END;਍䜀伀ഀഀ
਍匀䔀吀 䄀一匀䤀开一唀䰀䰀匀 伀一ഀഀ
GO਍匀䔀吀 儀唀伀吀䔀䐀开䤀䐀䔀一吀䤀䘀䤀䔀刀 伀一ഀഀ
GO਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
-- Description:	Insert trigger's changes to schema change tracking tables਍ⴀⴀ 㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀㴀ഀഀ
ALTER TRIGGER [trg_triggers_change_track_update] ON [triggers]਍䘀伀刀 唀倀䐀䄀吀䔀ഀഀ
AS਍     ⴀⴀ 挀栀攀挀欀 椀昀 琀爀椀最最攀爀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
     IF NOT (UPDATE([table_id]) OR UPDATE([name]) OR UPDATE([before]) OR UPDATE([after]) OR UPDATE([instead_of]) OR UPDATE([on_insert]) OR UPDATE([on_update])  OR UPDATE([on_delete]) ਍               伀刀 唀倀䐀䄀吀䔀⠀嬀搀椀猀愀戀氀攀搀崀⤀ 伀刀 唀倀䐀䄀吀䔀⠀嬀搀攀昀椀渀椀琀椀漀渀崀⤀ 伀刀 唀倀䐀䄀吀䔀⠀嬀琀礀瀀攀崀⤀ 伀刀 唀倀䐀䄀吀䔀⠀嬀猀琀愀琀甀猀崀⤀⤀  ഀഀ
      BEGIN਍          刀䔀吀唀刀一㬀ഀഀ
     END;਍ഀഀ
     -- skip manual objects created through Dataedo application਍     䤀䘀 䔀堀䤀匀吀匀ഀഀ
              (਍               匀䔀䰀䔀䌀吀 吀伀倀 ⠀㄀⤀ ㄀ഀഀ
                 FROM [INSERTED]਍                 圀䠀䔀刀䔀 嬀猀漀甀爀挀攀崀 㴀 ✀唀匀䔀刀✀ഀഀ
              )਍         䈀䔀䜀䤀一ഀഀ
             RETURN;਍         䔀一䐀㬀ഀഀ
     -- check if object property change ਍     䤀䘀 䔀堀䤀匀吀匀ഀഀ
              (਍               匀䔀䰀䔀䌀吀 吀伀倀 ⠀㄀⤀ ㄀ഀഀ
                 FROM [inserted] [i]਍                      䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀琀爀椀最最攀爀开椀搀崀 㴀 嬀搀崀⸀嬀琀爀椀最最攀爀开椀搀崀ഀഀ
                 WHERE ISNULL([d].[table_id],'') <> ISNULL([i].[table_id],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[before],'') <> ISNULL([i].[before],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀愀昀琀攀爀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀愀昀琀攀爀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[instead_of],'') <> ISNULL([i].[instead_of],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[on_update],'') <> ISNULL([i].[on_update],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[disabled],'') <> ISNULL([i].[disabled],'')਍                       伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ഀഀ
                       OR ISNULL([d].[type],'') <> ISNULL([i].[type],'')਍              ⤀ഀഀ
         BEGIN਍             唀倀䐀䄀吀䔀 嬀琀爀椀最最攀爀猀开挀栀愀渀最攀猀崀ഀഀ
               SET਍                   嬀瘀愀氀椀搀开琀漀崀 㴀 䜀䔀吀䐀䄀吀䔀⠀⤀ഀഀ
               FROM [triggers_changes] [c]਍                    䤀一一䔀刀 䨀伀䤀一 嬀椀渀猀攀爀琀攀搀崀 嬀甀崀 伀一 嬀挀崀⸀嬀琀愀戀氀攀开椀搀崀 㴀 嬀甀崀⸀嬀琀愀戀氀攀开椀搀崀ഀഀ
               WHERE [valid_to] IS NULL;਍             ⴀⴀ 椀渀猀攀爀琀 挀栀愀渀最攀猀 ⠀戀攀昀漀爀攀 愀渀搀 愀昀琀攀爀 瘀愀氀甀攀猀⤀ 昀漀爀 琀爀椀最最攀爀 ⴀ 眀栀攀渀 琀爀椀最最攀爀 眀愀猀 甀瀀搀愀琀攀搀ഀഀ
             INSERT INTO [triggers_changes]਍             ⠀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
              [trigger_id],਍              嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
              [name],਍              嬀戀攀昀漀爀攀崀Ⰰഀഀ
              [after],਍              嬀椀渀猀琀攀愀搀开漀昀崀Ⰰഀഀ
              [on_insert],਍              嬀漀渀开甀瀀搀愀琀攀崀Ⰰഀഀ
              [on_delete],਍              嬀搀椀猀愀戀氀攀搀崀Ⰰഀഀ
              [definition],਍              嬀戀攀昀漀爀攀开戀攀昀漀爀攀崀Ⰰഀഀ
              [before_after],਍              嬀戀攀昀漀爀攀开椀渀猀琀攀愀搀开漀昀崀Ⰰഀഀ
              [before_on_insert],਍              嬀戀攀昀漀爀攀开漀渀开甀瀀搀愀琀攀崀Ⰰഀഀ
              [before_on_delete],਍              嬀戀攀昀漀爀攀开搀椀猀愀戀氀攀搀崀Ⰰഀഀ
              [before_definition],਍              嬀琀礀瀀攀崀Ⰰഀഀ
              [operation],਍              嬀瘀愀氀椀搀开昀爀漀洀崀Ⰰഀഀ
              [update_id]਍             ⤀ഀഀ
                    SELECT [t].[database_id],਍                           嬀椀崀⸀嬀琀爀椀最最攀爀开椀搀崀Ⰰഀഀ
                           [i].[table_id],਍                           嬀椀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                           [i].[before],਍                           嬀椀崀⸀嬀愀昀琀攀爀崀Ⰰഀഀ
                           [i].[instead_of],਍                           嬀椀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰഀഀ
                           [i].[on_update],਍                           嬀椀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰഀഀ
                           [i].[disabled],਍                           嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
                           [d].[before],਍                           嬀搀崀⸀嬀愀昀琀攀爀崀Ⰰഀഀ
                           [d].[instead_of],਍                           嬀搀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰഀഀ
                           [d].[on_update],਍                           嬀搀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰഀഀ
                           [d].[disabled],਍                           嬀搀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
                           [d].[type],਍                           ✀唀倀䐀䄀吀䔀䐀✀Ⰰഀഀ
                           [cc].[valid_to],਍                           嬀椀崀⸀嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
                      FROM [inserted] [i]਍                           䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀琀爀椀最最攀爀开椀搀崀 㴀 嬀搀崀⸀嬀琀爀椀最最攀爀开椀搀崀ഀഀ
                           LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]਍                           伀唀吀䔀刀 䄀倀倀䰀夀ഀഀ
                                      (਍                                       匀䔀䰀䔀䌀吀 䴀䄀堀⠀嬀瘀愀氀椀搀开琀漀崀⤀ 䄀匀 嬀瘀愀氀椀搀开琀漀崀ഀഀ
                                         FROM [triggers_changes] [c]਍                                         圀䠀䔀刀䔀 嬀挀崀⸀嬀琀爀椀最最攀爀开椀搀崀 㴀 嬀搀崀⸀嬀琀爀椀最最攀爀开椀搀崀ഀഀ
                                               AND [c].[valid_to] IS NOT NULL਍                                      ⤀ 嬀挀挀崀ഀഀ
                      WHERE [i].[status] = 'A'਍                            䄀一䐀 嬀搀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀ഀഀ
                            AND (ISNULL([d].[table_id],'') <> ISNULL([i].[table_id],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀渀愀洀攀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[before],'') <> ISNULL([i].[before],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀愀昀琀攀爀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀愀昀琀攀爀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[instead_of],'') <> ISNULL([i].[instead_of],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[on_update],'') <> ISNULL([i].[on_update],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[disabled],'') <> ISNULL([i].[disabled],'')਍                                 伀刀 䤀匀一唀䰀䰀⠀嬀搀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ 㰀㸀 䤀匀一唀䰀䰀⠀嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰ✀✀⤀ഀഀ
                                 OR ISNULL([d].[type],'') <> ISNULL([i].[type],''));਍         䔀一䐀㬀ഀഀ
    BEGIN਍        ⴀⴀ 椀渀猀攀爀琀 挀栀愀渀最攀猀 ⠀搀攀氀攀琀攀搀 瘀愀氀甀攀猀⤀ 昀漀爀 琀爀椀最最攀爀 ⴀ 眀栀攀渀 琀爀椀最最攀爀 眀愀猀 搀攀氀攀琀攀搀ഀഀ
        INSERT INTO [triggers_changes]਍        ⠀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
         [trigger_id],਍         嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
         [name],਍         嬀戀攀昀漀爀攀崀Ⰰഀഀ
         [after],਍         嬀椀渀猀琀攀愀搀开漀昀崀Ⰰഀഀ
         [on_insert],਍         嬀漀渀开甀瀀搀愀琀攀崀Ⰰഀഀ
         [on_delete],਍         嬀搀椀猀愀戀氀攀搀崀Ⰰഀഀ
         [definition],਍         嬀琀礀瀀攀崀Ⰰഀഀ
         [operation],਍         嬀瘀愀氀椀搀开琀漀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [t].[database_id],਍                      嬀搀崀⸀嬀琀爀椀最最攀爀开椀搀崀Ⰰഀഀ
                      [d].[table_id],਍                      嬀搀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                      [d].[before],਍                      嬀搀崀⸀嬀愀昀琀攀爀崀Ⰰഀഀ
                      [d].[instead_of],਍                      嬀搀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰഀഀ
                      [d].[on_update],਍                      嬀搀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰഀഀ
                      [d].[disabled],਍                      嬀搀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
                      [d].[type],਍                      ✀䐀䔀䰀䔀吀䔀䐀✀Ⰰഀഀ
                      GETDATE(),਍                      嬀椀崀⸀嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
                 FROM [inserted] [i]਍                      䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀琀爀椀最最攀爀开椀搀崀 㴀 嬀搀崀⸀嬀琀爀椀最最攀爀开椀搀崀ഀഀ
                      LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]਍                 圀䠀䔀刀䔀 嬀椀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䐀✀ഀഀ
                       AND [d].[status] = 'A';਍        ⴀⴀ 椀渀猀攀爀琀 挀栀愀渀最攀猀 ⠀甀瀀搀愀琀攀搀 瘀愀氀甀攀猀⤀ 昀漀爀 琀爀椀最最攀爀 ⴀ 眀栀攀渀 琀爀椀最最攀爀 眀愀猀 爀攀猀琀漀爀攀搀ഀഀ
        INSERT INTO [triggers_changes]਍        ⠀嬀搀愀琀愀戀愀猀攀开椀搀崀Ⰰഀഀ
         [trigger_id],਍         嬀琀愀戀氀攀开椀搀崀Ⰰഀഀ
         [name],਍         嬀戀攀昀漀爀攀崀Ⰰഀഀ
         [after],਍         嬀椀渀猀琀攀愀搀开漀昀崀Ⰰഀഀ
         [on_insert],਍         嬀漀渀开甀瀀搀愀琀攀崀Ⰰഀഀ
         [on_delete],਍         嬀搀椀猀愀戀氀攀搀崀Ⰰഀഀ
         [definition],਍         嬀琀礀瀀攀崀Ⰰഀഀ
         [operation],਍         嬀瘀愀氀椀搀开昀爀漀洀崀Ⰰഀഀ
         [update_id]਍        ⤀ഀഀ
               SELECT [t].[database_id],਍                      嬀椀崀⸀嬀琀爀椀最最攀爀开椀搀崀Ⰰഀഀ
                      [i].[table_id],਍                      嬀椀崀⸀嬀渀愀洀攀崀Ⰰഀഀ
                      [i].[before],਍                      嬀椀崀⸀嬀愀昀琀攀爀崀Ⰰഀഀ
                      [i].[instead_of],਍                      嬀椀崀⸀嬀漀渀开椀渀猀攀爀琀崀Ⰰഀഀ
                      [i].[on_update],਍                      嬀椀崀⸀嬀漀渀开搀攀氀攀琀攀崀Ⰰഀഀ
                      [i].[disabled],਍                      嬀椀崀⸀嬀搀攀昀椀渀椀琀椀漀渀崀Ⰰഀഀ
                      [i].[type],਍                      ✀䄀䐀䐀䔀䐀✀Ⰰഀഀ
                      GETDATE(),਍                      嬀椀崀⸀嬀甀瀀搀愀琀攀开椀搀崀ഀഀ
                 FROM [inserted] [i]਍                      䤀一一䔀刀 䨀伀䤀一 嬀搀攀氀攀琀攀搀崀 嬀搀崀 伀一 嬀椀崀⸀嬀琀爀椀最最攀爀开椀搀崀 㴀 嬀搀崀⸀嬀琀爀椀最最攀爀开椀搀崀ഀഀ
                      LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]਍                 圀䠀䔀刀䔀 嬀椀崀⸀嬀猀琀愀琀甀猀崀 㴀 ✀䄀✀ഀഀ
                       AND [d].[status] = 'D';਍    䔀一䐀㬀ഀഀ
GO਍ഀഀ
-- Version਍唀倀䐀䄀吀䔀 嬀瘀攀爀猀椀漀渀崀ഀഀ
  SET਍      嬀猀琀愀戀氀攀崀 㴀 ㄀ഀഀ
  WHERE [version] = 7਍        䄀一䐀 嬀甀瀀搀愀琀攀崀 㴀 㔀ഀഀ
        AND [release] = 0;਍䜀伀�