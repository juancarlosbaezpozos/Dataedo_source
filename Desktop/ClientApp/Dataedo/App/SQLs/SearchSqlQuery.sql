-- #region IGNORE (part ignored in application, DISABLED NOW)
-- INPUT
DECLARE @documentations_set NVARCHAR(200) = '1'; --21 10 1 12 11 6 7 5 22 8 9 3 
DECLARE @types_set NVARCHAR(56) = 'DOCUMENTATION MODULE TABLE VIEW PROCEDURE FUNCTION';
DECLARE @words_set NVARCHAR(200) = 'Deleting a table''s and view''s columns';
DECLARE @word NVARCHAR(200) = 'doc';
--Table and view columns.
-- #endregion

DECLARE @documentations TABLE(value NVARCHAR(50))
INSERT INTO @documentations SELECT * FROM [dbo].[split_strings](@documentations_set, ' ')

DECLARE @types TABLE(value NVARCHAR(50))
INSERT INTO @types SELECT * FROM [dbo].[split_strings](@types_set, ' ')

DECLARE @words TABLE(value NVARCHAR(50))
INSERT INTO @words SELECT * FROM [dbo].[split_strings](@words_set, ' ')

SELECT
	--[type],
	[database_id],
	--[module_id],
	--[object_id],
	--[element_id],
	SUM(
		CASE WHEN name LIKE '%' + @word + '%' THEN
			40 ELSE 0 END +
		CASE WHEN title LIKE '%' + @word + '%' THEN
			30 ELSE 0 END +
		CASE WHEN description LIKE '%' + @word + '%' THEN
			10 ELSE 0 END
	) AS 'rank'

FROM databases
GROUP BY [database_id]
ORDER BY [rank] DESC





		[element_type]
	FROM
	(
		-- DOCUMENTATION
		SELECT
			'DOCUMENTATION' as 'type',
			'OBJECT' as 'element_type',
			database_id,
			NULL as 'module_id',
			NULL as 'object_id',
			NULL as 'element_id',
			words.value as 'word',
			name,
			title,
			description
		FROM @words as words CROSS JOIN
			(SELECT database_id, name, title, description
				FROM [databases]
				WHERE
					database_id IN (SELECT * FROM @documentations)
						AND 'DOCUMENTATION' IN (SELECT * FROM @types)) modules
)



SELECT
	[rank],
	results.[type],
	results.[element_type],
	results.[database_id], d.name as 'database_name', d.title as 'database_title',
	results.[module_id], m.title as 'module_title',
	results.[object_id],
	(CASE
		WHEN results.type = 'TABLE' OR results.type = 'VIEW' THEN t.name
		WHEN results.type = 'PROCEDURE' OR results.type = 'FUNCTION' THEN p.name
	END) as 'object_name',
	(CASE
		WHEN results.type = 'TABLE' OR results.type = 'VIEW' THEN t.title
		WHEN results.type = 'PROCEDURE' OR results.type = 'FUNCTION' THEN p.title
	END) as 'object_title',
	results.[element_id],
	(CASE
		WHEN results.element_type = 'COLUMN' THEN _c.name
		WHEN results.element_type = 'RELATION' THEN _tr.name
		WHEN results.element_type = 'CONSTRAINT' THEN _uc.name
		WHEN results.element_type = 'TRIGGER' THEN _t.name
		WHEN results.element_type = 'PARAMETER' THEN _p.name
	END) as 'element_name',
	(CASE
		WHEN results.element_type = 'COLUMN' THEN _c.title
		WHEN results.element_type = 'RELATION' THEN NULL
		WHEN results.element_type = 'CONSTRAINT' THEN NULL
		WHEN results.element_type = 'TRIGGER' THEN NULL
		WHEN results.element_type = 'PARAMETER' THEN NULL
	END) as 'element_title'
FROM
(
	SELECT
		[type],
		[database_id],
		[module_id],
		[object_id],
		[element_id],
		SUM(
			CASE WHEN element_type = 'OBJECT' AND name LIKE '%' + @words_set + '%' THEN
				70 ELSE 0 END +
			CASE WHEN element_type = 'OBJECT' AND title LIKE '%' + @words_set + '%' THEN
				60 ELSE 0 END +
			CASE WHEN element_type = 'OBJECT' AND description LIKE '%' + @words_set + '%' THEN
				50 ELSE 0 END +
			CASE WHEN element_type = 'OBJECT' AND name LIKE '%' + word + '%' THEN
				40 ELSE 0 END +
			CASE WHEN element_type = 'OBJECT' AND title LIKE '%' + word + '%' THEN
				30 ELSE 0 END +
			CASE WHEN element_type = 'OBJECT' AND description LIKE '%' + word + '%' THEN
				10 ELSE 0 END +

			CASE WHEN element_type <> 'OBJECT' AND name LIKE '%' + @words_set + '%' THEN
				50 ELSE 0 END +
			CASE WHEN element_type <> 'OBJECT' AND title LIKE '%' + @words_set + '%' THEN
				40 ELSE 0 END +
			CASE WHEN element_type <> 'OBJECT' AND description LIKE '%' + @words_set + '%' THEN
				30 ELSE 0 END +
			CASE WHEN element_type <> 'OBJECT' AND name LIKE '%' + word + '%' THEN
				20 ELSE 0 END +
			CASE WHEN element_type <> 'OBJECT' AND title LIKE '%' + word + '%' THEN
				10 ELSE 0 END +
			CASE WHEN element_type <> 'OBJECT' AND description LIKE '%' + word + '%' THEN
				5 ELSE 0 END
		) AS 'rank',
		[element_type]
	FROM
	(
		-- DOCUMENTATION
		SELECT
			'DOCUMENTATION' as 'type',
			'OBJECT' as 'element_type',
			database_id,
			NULL as 'module_id',
			NULL as 'object_id',
			NULL as 'element_id',
			words.value as 'word',
			name,
			title,
			description
		FROM @words as words CROSS JOIN
			(SELECT database_id, name, title, description
				FROM [databases]
				WHERE
					database_id IN (SELECT * FROM @documentations)
						AND 'DOCUMENTATION' IN (SELECT * FROM @types)) modules

		UNION ALL

		-- MODULE
		SELECT
			'MODULE' as 'type',
			'OBJECT' as 'element_type',
			database_id,
			module_id,
			NULL as 'object_id',
			NULL as 'element_id',
			words.value as 'word',
			name,
			title,
			description
		FROM @words as words CROSS JOIN
			(SELECT database_id, module_id, NULL as name, title, description
				FROM [modules]
				WHERE
					database_id IN (SELECT * FROM @documentations)
						AND 'MODULE' IN (SELECT * FROM @types)) joined

		UNION ALL

		-- TABLE or VIEW
		SELECT
			object_type as 'type',
			'OBJECT' as 'element_type',
			database_id,
			module_id,
			table_id as 'object_id',
			NULL as 'element_id',
			words.value as 'word',
			name,
			title,
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, b.table_id, b.name, b.title, b.description
				FROM [tables] b
					LEFT OUTER JOIN tables_modules j ON b.table_id = j.table_id
				WHERE 
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined

		UNION ALL

		SELECT
			object_type as 'type',
			'COLUMN' as 'element_type',
			database_id,
			module_id,
			table_id as 'object_id',
			element_id,
			words.value as 'word',
			name,
			title,
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, t.table_id, b.column_id as 'element_id', b.name, b.title, b.description
				FROM [columns] b
					LEFT OUTER JOIN [tables_modules] j ON b.table_id = j.table_id
					LEFT OUTER JOIN [tables] t ON b.table_id = t.table_id
				WHERE 
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined

		UNION ALL

		SELECT
			object_type as 'type',
			'RELATION' as 'element_type',
			database_id,
			module_id,
			table_id as 'object_id',
			element_id,
			words.value as 'word',
			name,
			NULL as 'title',
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, t.table_id, b.table_relation_id as 'element_id', b.name, b.description
				FROM [tables_relations] b
					LEFT OUTER JOIN [tables_modules] j ON b.pk_table_id = j.table_id
					LEFT OUTER JOIN [tables] t ON b.pk_table_id = t.table_id
				WHERE 
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined
					
		UNION ALL

		SELECT
			object_type as 'type',
			'RELATION' as 'element_type',
			database_id,
			module_id,
			table_id as 'object_id',
			element_id,
			words.value as 'word',
			name,
			NULL as 'title',
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, t.table_id, b.table_relation_id as 'element_id', b.name, b.description
				FROM [tables_relations] b
					LEFT OUTER JOIN [tables_modules] j ON b.fk_table_id = j.table_id
					LEFT OUTER JOIN [tables] t ON b.fk_table_id = t.table_id
				WHERE 
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined

		UNION ALL

		SELECT
			object_type as 'type',
			'CONSTRAINT' as 'element_type',
			database_id,
			module_id,
			table_id as 'object_id',
			element_id,
			words.value as 'word',
			name,
			NULL as 'title',
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, t.table_id, b.unique_constraint_id as 'element_id', b.name, b.description
				FROM [unique_constraints] b
					LEFT OUTER JOIN [tables_modules] j ON b.table_id = j.table_id
					LEFT OUTER JOIN [tables] t ON b.table_id = t.table_id
				WHERE 
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined

		UNION ALL

		SELECT
			object_type as 'type',
			'TRIGGER' as 'element_type',
			database_id,
			module_id,
			table_id as 'object_id',
			element_id,
			words.value as 'word',
			name,
			NULL as 'title',
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, t.table_id, b.trigger_id as 'element_id', b.name, b.description
				FROM [triggers] b
					LEFT OUTER JOIN [tables_modules] j ON b.table_id = j.table_id
					LEFT OUTER JOIN [tables] t ON b.table_id = t.table_id
				WHERE 
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined

		UNION ALL

		-- PROCEDURE or FUNCTION
		SELECT
			object_type as 'type',
			'OBJECT' as 'element_type',
			database_id,
			module_id,
			procedure_id as 'object_id',
			NULL as 'element_id',
			words.value as 'word',
			name,
			title,
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, b.procedure_id, name, title, description
				FROM [procedures] b
					LEFT OUTER JOIN [procedures_modules] j ON b.procedure_id = j.procedure_id
				WHERE
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined

		UNION ALL

		SELECT
			object_type as 'type',
			'PARAMETER' as 'element_type',
			database_id,
			module_id,
			procedure_id as 'object_id',
			element_id,
			words.value as 'word',
			name,
			NULL as 'title',
			description
		FROM @words as words CROSS JOIN
			(SELECT object_type, database_id, module_id, b.procedure_id, b.parameter_id as 'element_id', b.name, b.description
				FROM [parameters] b
					LEFT OUTER JOIN [procedures_modules] j ON b.procedure_id = j.procedure_id
					LEFT OUTER JOIN [procedures] t ON b.procedure_id = t.procedure_id
				WHERE
					database_id IN (SELECT * FROM @documentations)
						AND object_type IN (SELECT * FROM @types)) joined
	)
	AS results
	GROUP BY [type], [element_type], [database_id], [module_id], [object_id], [element_id]
)
AS results
	LEFT OUTER JOIN [databases] d ON results.[database_id] = d.[database_id]
	LEFT OUTER JOIN [modules] m ON results.[module_id] = m.[module_id]
	LEFT OUTER JOIN [tables] t ON results.[object_id] = t.[table_id]
	LEFT OUTER JOIN [procedures] p ON results.[object_id] = p.[procedure_id]

	LEFT OUTER JOIN [columns] _c ON results.[element_id] = _c.[column_id]
	LEFT OUTER JOIN [tables_relations] _tr ON results.[element_id] = _tr.[table_relation_id]
	LEFT OUTER JOIN [unique_constraints] _uc ON results.[element_id] = _uc.[unique_constraint_id]
	LEFT OUTER JOIN [triggers] _t ON results.[element_id] = _t.[trigger_id]

	LEFT OUTER JOIN [parameters] _p ON results.[element_id] = _p.[parameter_id]
WHERE [rank] > 0
ORDER BY [rank] DESC