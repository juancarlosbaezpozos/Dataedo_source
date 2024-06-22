DECLARE @id AS UNIQUEIDENTIFIER = '3e13cfc9-1dc9-4ad9-8500-ec771e117801'
DECLARE @version as int = 10
DECLARE @update as int = 2
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-3630'

IF NOT EXISTS (SELECT [id] FROM [database_update_log] WHERE [id] =  @id)
BEGIN

IF (SELECT COUNT(*) FROM [version] WHERE [version] = @version AND [update] = @update AND [release] = @release) = 0
     BEGIN
       INSERT INTO [version] ([version], [update], [release], [stable]) VALUES (@version, @update, @release, 0);
     END
  ELSE
     BEGIN
       UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 0 WHERE [version] = @version AND [update] = @update AND [release] = @release;
     END

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  -- SQL script here. WITOUT key word 'GO'!
  
    INSERT [dbo].[import_tables] 
    ([database_name], [table_schema], [table_name], [object_type], [object_subtype], [description], [field1])
    VALUES 
    ('Sample Interface Database', 'dbo', 'Person', 'TABLE', 'TABLE', 'Human beings involved.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Address', 'TABLE', 'TABLE', 'Postal address.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Company', 'TABLE', 'TABLE', 'Involved firms.', 'Planned'),
    ('Sample Interface Database', 'dbo', 'PerAdd', 'VIEW', 'TABLE', 'Humans with their address.', 'Planned'),
    ('Sample Interface Database', 'dbo', 'ZipCodes', 'STRUCTURE', 'STRUCTURE', 'All area codes in USA.', 'Active');

    INSERT [dbo].[import_columns] 
    ([database_name], [table_schema], [table_name], [table_object_type], [column_name], [column_level], [ordinal_position], [item_type], [datatype], [nullable], [default_value], [is_identity], [is_computed], [description], [field1]) 
    VALUES 
    ('Sample Interface Database', 'dbo', 'Person', 'TABLE', 'person_id', 1, 1, 'COLUMN', 'int', 0, NULL, 1, 0, 'Primary key for Person records.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Person', 'TABLE', 'name', 1, 2, 'COLUMN', 'varchar(100)', 0, NULL, 0, 0, 'Person name.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Person', 'TABLE', 'address_id', 1, 3, 'COLUMN', 'int', 0, NULL, 0, 0, 'Reference to address.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Address', 'TABLE', 'address_id', 1, 1, 'COLUMN', 'int', 0, NULL, 1, 0, 'Primary key for Address records.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Address', 'TABLE', 'address', 1, 2, 'COLUMN', 'varchar(MAX)', 0, NULL, 0, 0, 'Simple address.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Address', 'TABLE', 'zip_code', 1, 2, 'COLUMN', 'varchar(20)', 0, NULL, 0, 0, 'Area code.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Address', 'TABLE', 'country_code', 1, 3, 'COLUMN', 'varchar(10)', 0, NULL, 0, 0, 'Country Code in US, CA, DE, PL, etc. format.', 'Active'),
    ('Sample Interface Database', 'dbo', 'Company', 'TABLE', 'company_id', 1, 1, 'COLUMN', 'int', 0, NULL, 1, 0, 'Primary key for Company records.', 'Planned'),
    ('Sample Interface Database', 'dbo', 'Company', 'TABLE', 'name', 1, 2, 'COLUMN', 'varchar(100)', 0, NULL, 0, 0, 'Company name.', 'Planned'),
    ('Sample Interface Database', 'dbo', 'Company', 'TABLE', 'address_id', 1, 3, 'COLUMN', 'int', 0, NULL, 0, 0, 'Reference to address.', 'Planned'),
    ('Sample Interface Database', 'dbo', 'PerAdd', 'VIEW', 'person_id', 1, 1, 'COLUMN', 'int', 0, NULL, 1, 0, 'Primary key for Person records.', 'Active'),
    ('Sample Interface Database', 'dbo', 'PerAdd', 'VIEW', 'name', 1, 2, 'COLUMN', 'varchar(100)', 0, NULL, 0, 0, 'Person name.', 'Active'),
    ('Sample Interface Database', 'dbo', 'PerAdd', 'VIEW', 'address', 1, 3, 'COLUMN', 'varchar(MAX)', 0, NULL, 0, 0, 'Concatenated address.', 'Active'),
    ('Sample Interface Database', 'dbo', 'ZipCodes', 'STRUCTURE', 'id', 1, 1, 'COLUMN', 'int', 0, NULL, 1, 0, 'Zip code id.', 'Active'),
    ('Sample Interface Database', 'dbo', 'ZipCodes', 'STRUCTURE', 'name', 1, 2, 'COLUMN', 'varchar(100)', 0, NULL, 0, 0, 'Zip code name.', 'Active');

    INSERT [dbo].[import_tables_keys_columns] 
    ([database_name], [table_schema], [table_name], [table_object_type], [key_name], [key_type], [description], [column_name]) 
    VALUES 
    ('Sample Interface Database', 'dbo', 'Person', 'TABLE', 'PK_Person_id', 'PK', 'Primary key (clustered) constraint', 'person_id'),
    ('Sample Interface Database', 'dbo', 'Address', 'TABLE', 'PK_Address_id', 'PK', 'Primary key (clustered) constraint', 'address_id'),
    ('Sample Interface Database', 'dbo', 'Company', 'TABLE', 'PK_Company_id', 'PK', 'Primary key (clustered) constraint', 'company_id'),
    ('Sample Interface Database', 'dbo', 'PerAdd', 'VIEW', 'PK_PerAdd_id', 'PK', 'Primary key (clustered) constraint', 'person_id');

    INSERT [dbo].[import_tables_foreign_keys_columns] 
    ([database_name], [foreign_table_schema], [foreign_table_name], [foreign_table_object_type], [primary_table_schema], [primary_table_name], [primary_table_object_type], [foreign_column_name], [primary_column_name], [column_pair_order], [description]) 
    VALUES 
    ('Sample Interface Database', 'dbo', 'Person', 'TABLE', 'dbo', 'Address', 'TABLE', 'address_id', 'address_id', 1, 'Relation person to address.'),
    ('Sample Interface Database', 'dbo', 'Company', 'TABLE', 'dbo', 'Address', 'TABLE', 'address_id', 'address_id', 1, 'Relation company to address.');

    INSERT [dbo].[import_procedures] 
    ([database_name], [procedure_schema], [procedure_name], [object_type], [object_subtype], [function_type], [language], [definition], [description]) 
    VALUES 
    ('Sample Interface Database', 'dbo', 'deleteCompaniesFromCountry', 'PROCEDURE', 'PROCEDURE', NULL, 'SQL', 'CREATE PROCEDURE deleteCompaniesFromCountry; DELETE FROM Country c JOIN Address a WHERE ...', 'Deleting companies from given country'),
    ('Sample Interface Database', 'dbo', 'getAddressForCompanies', 'FUNCTION', 'FUNCTION', 'Scalar-valued', 'SQL', 'CREATE FUNCTION getAddressForCompanies(companyPrefix) RETURNS ...', 'Listing all addresses for companies with similar name.');

    INSERT [dbo].[import_procedures_parameters] 
    ([database_name], [procedure_schema], [procedure_name], [procedure_object_type], [parameter_name], [ordinal_position], [parameter_mode], [datatype]) 
    VALUES 
    ('Sample Interface Database', 'dbo', 'deleteCompaniesFromCountry', 'PROCEDURE', 'Returns', 0, 'OUT', 'boolean'),
    ('Sample Interface Database', 'dbo', 'getAddressForCompanies', 'FUNCTION', 'Attr', 0, 'IN', 'varchar'),
    ('Sample Interface Database', 'dbo', 'getAddressForCompanies', 'FUNCTION', 'Returns', 0, 'OUT', 'array');

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO
