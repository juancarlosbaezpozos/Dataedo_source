IF(
    (SELECT COUNT(*)
    FROM [version]
    WHERE [version] = 6 AND [update] = 1 AND [release] = 0)
    = 0)
BEGIN
	INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (6, 1, 0, 0)
END
GO

UPDATE [version]
set [stable] = 1
where [version] = 6 and [update] = 1 AND [release] = 0
GO
