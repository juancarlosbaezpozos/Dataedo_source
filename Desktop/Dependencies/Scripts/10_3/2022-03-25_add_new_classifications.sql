DECLARE @id AS UNIQUEIDENTIFIER = '0C746DC8-8745-4C09-AF54-0DD6075F115F'
DECLARE @version as int = 10 
DECLARE @update as int = 3
DECLARE @release as int = 0
DECLARE @change_no as varchar(25) = 'DEV-172'

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

DECLARE @new_classificator_id int;
DECLARE @new_classificator_built_in_id int;

--fix old names

UPDATE [dbo].[classificators]
SET [title] = 'PII - Personally Identifiable Information'
WHERE [built_in_id] = 1 AND [title] = 'Personally Identifiable Information (PII)'

UPDATE [dbo].[classificators]
SET [title] = 'GDPR - General Data Protection Regulation'
WHERE [built_in_id] = 2 AND [title] = 'GDPR'

--HIPPA

--[built_in_id] equal 3 is reserved for HIPPA
SET @new_classificator_built_in_id = 3

--clear [built_in_id] in any potential user-inserted rows
UPDATE [dbo].[classificators]
SET [built_in_id] = NULL
WHERE [built_in_id] = @new_classificator_built_in_id

--insert data
INSERT INTO [dbo].[classificators] (
    [built_in_id]
    ,[title]
    ,[custom_field_1_name]
    ,[custom_field_2_name]
    ,[custom_field_1_class_id]
    ,[custom_field_2_class_id]
    ,[custom_field_1_definition]
    ,[custom_field_2_definition]
    ,[description]
)
VALUES (
    @new_classificator_built_in_id
    ,'HIPAA - Health Insurance Portability and Accountability Act'
    ,'HIPAA Sensitivity Level' --name of the 1st custom field created by the classificator; shouldn't be already in use
    ,'HIPAA Data Domain' --name of the 2nd custom field created by the classificator; shouldn't be already in use
    ,5 --1st custom field type from [dbo].[custom_field_classes]; 5 is the dedicated dropdown list for classification
    ,6 --2nd custom field type from [dbo].[custom_field_classes]; 6 is the dedicated text field for classification
    ,'Confidential, Internal, Public' --options for the 1st custom field's dropdown
    ,NULL --options for the 2nd custom field - due to its class none are required
    ,'Health Insurance Portability and Accountability Act (HIPAA) Classification'
);

--set @new_classificator_id from newly inserted row's [classificator_id]
SET @new_classificator_id = (
    SELECT TOP (1) [classificator_id]
    FROM [dbo].[classificators]
    WHERE [built_in_id] = @new_classificator_built_in_id
)

--clear [built_in_id] in any potential user-inserted rows
UPDATE [dbo].[classificator_rules]
SET [built_in_id] = NULL
WHERE ([built_in_id] >= 72 AND [built_in_id] <= 88) OR [built_in_id] = 120

--insert data
INSERT INTO [dbo].[classificator_rules] (
    [built_in_id]
    ,[classificator_id]
    ,[mask_name]
    ,[custom_field_1_value]
    ,[custom_field_2_value]
)
VALUES
    (72, @new_classificator_id, 'Social Security No', 'Confidential', 'Health Insurance Data'),
    (73, @new_classificator_id, 'Name', 'Confidential', 'Personal Data'),
    (74, @new_classificator_id, 'Address', 'Confidential', 'Personal Data'),
    (75, @new_classificator_id, 'Date of Birth', 'Confidential', 'Personal Data'),
    (76, @new_classificator_id, 'Phone', 'Confidential', 'Personal Data'),
    (77, @new_classificator_id, 'Email', 'Confidential', 'Personal Data'),
    (78, @new_classificator_id, 'Face Photo', 'Confidential', 'Personal Data'),
    (79, @new_classificator_id, 'Medical procedure', 'Internal', 'Medical Data'),
    (80, @new_classificator_id, 'Medical record number', 'Internal', 'Medical Data'),
    (81, @new_classificator_id, 'Health plan', 'Internal', 'Health Insurance Data'),
    (82, @new_classificator_id, 'Health insurance beneficiary number', 'Internal', 'Health Insurance Data'),
    (83, @new_classificator_id, 'Fingerprints', 'Confidential', 'Personal Data'),
    (84, @new_classificator_id, 'Voice-print', 'Confidential', 'Personal Data'),
    (85, @new_classificator_id, 'Patient number', 'Internal', 'Personal Data'),
    (86, @new_classificator_id, 'Healthcare payments', 'Internal', 'Health Insurance Data'),
    (87, @new_classificator_id, 'Mental health history', 'Confidential', 'Medical Data'),
    (88, @new_classificator_id, 'Healthcare services', 'Internal', 'Health Insurance Data'),
    (120, @new_classificator_id, 'Address Location', 'Confidential', 'Personal Data');

--FERPA

SET @new_classificator_built_in_id = 4

UPDATE [dbo].[classificators]
SET [built_in_id] = NULL
WHERE [built_in_id] = @new_classificator_built_in_id

INSERT INTO [dbo].[classificators] (
    [built_in_id]
    ,[title]
    ,[custom_field_1_name]
    ,[custom_field_2_name]
    ,[custom_field_1_class_id]
    ,[custom_field_2_class_id]
    ,[custom_field_1_definition]
    ,[custom_field_2_definition]
    ,[description]
)
VALUES (
    @new_classificator_built_in_id
    ,'FERPA - Family Educational Rights and Privacy Act'
    ,'FERPA Classification' --name of the 1st custom field created by the classificator; shouldn't be already in use
    ,'FERPA Data Domain' --name of the 2nd custom field created by the classificator; shouldn't be already in use
    ,5 --1st custom field type from [dbo].[custom_field_classes]; 5 is the dedicated dropdown list for classification
    ,6 --2nd custom field type from [dbo].[custom_field_classes]; 6 is the dedicated text field for classification
    ,'Confidential, Directory, Non-FERPA' --options for the 1st custom field's dropdown
    ,NULL --options for the 2nd custom field - due to its class none are required
    ,'The Family Educational Rights and Privacy Act (FERPA) Classification'
);

SET @new_classificator_id = (
    SELECT TOP (1) [classificator_id]
    FROM [dbo].[classificators]
    WHERE [built_in_id] = @new_classificator_built_in_id
)

UPDATE [dbo].[classificator_rules]
SET [built_in_id] = NULL
WHERE ([built_in_id] >= 89 AND [built_in_id] <= 102) OR [built_in_id] = 121

INSERT INTO [dbo].[classificator_rules] (
    [built_in_id]
    ,[classificator_id]
    ,[mask_name]
    ,[custom_field_1_value]
    ,[custom_field_2_value]
)
VALUES
    (89, @new_classificator_id, 'Name', 'Confidential', 'Personal Data'),
    (90, @new_classificator_id, 'Address', 'Confidential', 'Personal Data'),
    (91, @new_classificator_id, 'Date of Birth', 'Confidential', 'Personal Data'),
    (92, @new_classificator_id, 'Phone', 'Confidential', 'Personal Data'),
    (93, @new_classificator_id, 'Face Photo', 'Directory', 'Personal Data'),
    (94, @new_classificator_id, 'Student Name', 'Directory', 'Student Personal Data'),
    (95, @new_classificator_id, 'School Name', 'Directory', 'School Data'),
    (96, @new_classificator_id, 'Field of study', 'Directory', 'School Data'),
    (97, @new_classificator_id, 'Dates of Attendance', 'Directory', 'School Data'),
    (98, @new_classificator_id, 'Degrees', 'Directory', 'Student Personal Data'),
    (99, @new_classificator_id, 'Awards', 'Directory', 'Student Personal Data'),
    (100, @new_classificator_id, 'Test scores', 'Confidential', 'Student Personal Data'),
    (101, @new_classificator_id, 'Financial aids', 'Confidential', 'Student Personal Data'),
    (102, @new_classificator_id, 'Placement records', 'Confidential', 'Student Personal Data'),
    (121, @new_classificator_id, 'Address Location', 'Confidential', 'Personal Data');

--CCPA

SET @new_classificator_built_in_id = 5

UPDATE [dbo].[classificators]
SET [built_in_id] = NULL
WHERE [built_in_id] = @new_classificator_built_in_id

INSERT INTO [dbo].[classificators] (
    [built_in_id]
    ,[title]
    ,[custom_field_1_name]
    ,[custom_field_2_name]
    ,[custom_field_1_class_id]
    ,[custom_field_2_class_id]
    ,[custom_field_1_definition]
    ,[custom_field_2_definition]
    ,[description]
)
VALUES (
    @new_classificator_built_in_id
    ,'CCPA - California Consumer Privacy Act'
    ,'CCPA Classification' --name of the 1st custom field created by the classificator; shouldn't be already in use
    ,'CCPA Data Domain' --name of the 2nd custom field created by the classificator; shouldn't be already in use
    ,5 --1st custom field type from [dbo].[custom_field_classes]; 5 is the dedicated dropdown list for classification
    ,6 --2nd custom field type from [dbo].[custom_field_classes]; 6 is the dedicated text field for classification
    ,'Personal Information, Non-CCPA' --options for the 1st custom field's dropdown
    ,NULL --options for the 2nd custom field - due to its class none are required
    ,'California Consumer Privacy Act (CCPA) Classification'
);

SET @new_classificator_id = (
    SELECT TOP (1) [classificator_id]
    FROM [dbo].[classificators]
    WHERE [built_in_id] = @new_classificator_built_in_id
)

UPDATE [dbo].[classificator_rules]
SET [built_in_id] = NULL
WHERE [built_in_id] >= 103 AND [built_in_id] <= 115

INSERT INTO [dbo].[classificator_rules] (
    [built_in_id]
    ,[classificator_id]
    ,[mask_name]
    ,[custom_field_1_value]
    ,[custom_field_2_value]
)
VALUES
    (103, @new_classificator_id, 'Name', 'Personal Information', 'Personal Data'),
    (104, @new_classificator_id, 'Address', 'Personal Information', 'Personal Data'),
    (105, @new_classificator_id, 'Phone', 'Personal Information', 'Personal Data'),
    (106, @new_classificator_id, 'Address Location', 'Personal Information', 'Personal Data'),
    (107, @new_classificator_id, 'IP Address', 'Personal Information', 'Customer Data'),
    (108, @new_classificator_id, 'Social Media', 'Personal Information', 'Personal Data'),
    (109, @new_classificator_id, 'Credit Card No', 'Personal Information', 'Payments Data'),
    (110, @new_classificator_id, 'Face Photo', 'Personal Information', 'Personal Data'),
    (111, @new_classificator_id, 'Voice-print', 'Personal Information', 'Personal Data'),
    (112, @new_classificator_id, 'Orders', 'Personal Information', 'Customer Data'),
    (113, @new_classificator_id, 'Browsing history', 'Personal Information', 'Customer Data'),
    (114, @new_classificator_id, 'Geolocation', 'Personal Information', 'Customer Data'),
    (115, @new_classificator_id, 'Date of Birth', 'Personal Information', 'Personal Data');

--PCI

SET @new_classificator_built_in_id = 6

UPDATE [dbo].[classificators]
SET [built_in_id] = NULL
WHERE [built_in_id] = @new_classificator_built_in_id

INSERT INTO [dbo].[classificators] (
    [built_in_id]
    ,[title]
    ,[custom_field_1_name]
    ,[custom_field_2_name]
    ,[custom_field_1_class_id]
    ,[custom_field_2_class_id]
    ,[custom_field_1_definition]
    ,[custom_field_2_definition]
    ,[description]
)
VALUES (
    @new_classificator_built_in_id
    ,'PCI - Payment Card Industry'
    ,'PCI Classification' --name of the 1st custom field created by the classificator; shouldn't be already in use
    ,'PCI Data Domain' --name of the 2nd custom field created by the classificator; shouldn't be already in use
    ,5 --1st custom field type from [dbo].[custom_field_classes]; 5 is the dedicated dropdown list for classification
    ,6 --2nd custom field type from [dbo].[custom_field_classes]; 6 is the dedicated text field for classification
    ,'Classified, Non-PCI' --options for the 1st custom field's dropdown
    ,NULL --options for the 2nd custom field - due to its class none are required
    ,'Payment Card Industry (PCI) Classification'
);

SET @new_classificator_id = (
    SELECT TOP (1) [classificator_id]
    FROM [dbo].[classificators]
    WHERE [built_in_id] = @new_classificator_built_in_id
)

UPDATE [dbo].[classificator_rules]
SET [built_in_id] = NULL
WHERE [built_in_id] >= 116 AND [built_in_id] <= 119

INSERT INTO [dbo].[classificator_rules] (
    [built_in_id]
    ,[classificator_id]
    ,[mask_name]
    ,[custom_field_1_value]
    ,[custom_field_2_value]
)
VALUES
    (116, @new_classificator_id, 'Credit Card No', 'Classified', 'Credit Card Data'),
    (117, @new_classificator_id, 'Name', 'Classified', 'Personal Data'),
    (118, @new_classificator_id, 'CVV2', 'Classified', 'Credit Card Data'),
    (119, @new_classificator_id, 'Date of Birth', 'Classified', 'Personal Data');

--PII

IF EXISTS
(
    SELECT 1
    FROM [dbo].[classificators]
    WHERE [title] = 'PII - Personally Identifiable Information' AND [built_in_id] = 1
)
BEGIN 
        SET @new_classificator_id = (
            SELECT TOP (1) [classificator_id]
            FROM [dbo].[classificators]
            WHERE [built_in_id] = 1
        )
        INSERT INTO [dbo].[classificator_rules] (
             [built_in_id]
             ,[classificator_id]
             ,[mask_name]
             ,[custom_field_1_value]
             ,[custom_field_2_value]
        )
        VALUES
            (122, @new_classificator_id, 'Password', 'Sensitive PII', 'Password'),
            (123, @new_classificator_id, 'Marital Status', 'Sensitive PII', 'Marital Status');
END

--MASKS

UPDATE [dbo].[classificator_masks]
SET [built_in_id] = NULL
WHERE [built_in_id] >= 248 AND [built_in_id] <= 319

INSERT INTO [dbo].[classificator_masks] (
    [built_in_id]
    ,[mask_name]
    ,[mask]
    ,[data_types]
    ,[is_column]
    ,[is_title]
    ,[is_description]
)
VALUES
(248, 'Medical procedure', '%operation%date%', 'date, text', 1, 1, 0),
(250, 'Medical procedure', '%med%proc%', 'date, text', 1, 1, 0),
(251, 'Medical procedure', '%med%proc%date%', 'date, text', 1, 1, 0),
(252, 'Medical procedure', '%med%diag%', 'date, text', 1, 1, 0),
(253, 'Medical procedure', '%diagnosis%', 'date, text', 1, 1, 0),
(254, 'Medical record number', '%med%num%', 'text, number', 1, 1, 0),
(255, 'Medical record number', 'med%order', 'text, number', 1, 1, 0),
(256, 'Medical record number', '%med%no%', 'text, number', 1, 1, 0),
(257, 'Medical record number', '%med%record%', 'text, number', 1, 1, 0),
(258, 'Health plan', '%health%plan%', 'text', 1, 1, 0),
(259, 'Health plan', '%health%package%', 'text', 1, 1, 0),
(260, 'Health plan', '%med%package%', 'text', 1, 1, 0),
(261, 'Health insurance beneficiary number', '%health%claim%', 'text, number', 1, 1, 0),
(262, 'Health insurance beneficiary number', '%health%number%', 'text, number', 1, 1, 0),
(263, 'Fingerprints', 'finger', 'text', 1, 1, 0),
(264, 'Fingerprints', 'fingerprint', 'text', 1, 1, 0),
(265, 'Fingerprints', 'thumbprint', 'text', 1, 1, 0),
(266, 'Fingerprints', 'biometric%finger', 'text', 1, 1, 0),
(267, 'Voice-print', 'voice%print', 'text', 1, 1, 0),
(268, 'Voice-print', 'voice%sample', 'text', 1, 1, 0),
(269, 'Patient number', 'patient%num', 'text, number', 1, 1, 0),
(270, 'Patient number', 'patient%no', 'text, number', 1, 1, 0),
(271, 'Healthcare payments', 'health%payment', 'text, number', 1, 1, 0),
(272, 'Healthcare payments', 'health%order', 'text, number', 1, 1, 0),
(273, 'Healthcare payments', 'medical%pay', 'text, number', 1, 1, 0),
(274, 'Healthcare payments', 'med%pay', 'text, number', 1, 1, 0),
(275, 'Mental health history', 'mental%health', 'text', 1, 1, 0),
(276, 'Mental health history', 'mental%results', 'text', 1, 1, 0),
(277, 'Mental health history', 'mental%diagnosis', 'text', 1, 1, 0),
(278, 'Healthcare services', 'health%service', 'text', 1, 1, 0),
(279, 'Healthcare services', 'med%service', 'text', 1, 1, 0),
(280, 'Student Name', '%student%', 'text', 1, 1, 0),
(281, 'Student Name', '%pupil%name%', 'text', 1, 1, 0),
(282, 'Student Name', '%child%name%', 'text', 1, 1, 0),
(283, 'School Name', '%school%', 'text', 1, 1, 0),
(284, 'School Name', '%school%name%', 'text', 1, 1, 0),
(285, 'Field of study', '%study%program%', 'text', 1, 1, 0),
(286, 'Field of study', '%study%field%', 'text', 1, 1, 0),
(287, 'Field of study', '%field%', 'text', 1, 1, 0),
(288, 'Field of study', '%study%subject%', 'text', 1, 1, 0),
(289, 'Dates of Attendance', '%attendance%', 'date, text', 1, 1, 0),
(290, 'Dates of Attendance', '%presence%', 'date, text', 1, 1, 0),
(291, 'Degrees', '%graduate%', 'text, number', 1, 1, 0),
(292, 'Degrees', '%sci%deg%', 'text, number', 1, 1, 0),
(293, 'Degrees', 'science%degree', 'text, number', 1, 1, 0),
(294, 'Awards', '%student%award%', 'text, number', 1, 1, 0),
(295, 'Awards', '%awards%', 'text, number', 1, 1, 0),
(296, 'Test scores', '%test%res%', 'text, number', 1, 1, 0),
(297, 'Test scores', '%test%mark%', 'text, number', 1, 1, 0),
(298, 'Test scores', '%test%score%', 'text, number', 1, 1, 0),
(299, 'Financial aids', '%student%loan%', 'text, number', 1, 1, 0),
(300, 'Financial aids', '%stu%load%', 'text, number', 1, 1, 0),
(301, 'Financial aids', '%scholarship%', 'text, number', 1, 1, 0),
(302, 'Placement records', '%placement%test%', 'text, number', 1, 1, 0),
(303, 'Placement records', '%placement%score%', 'text, number', 1, 1, 0),
(304, 'Placement records', '%pla%results%', 'text, number', 1, 1, 0),
(305, 'Placement records', '%in%test%results%', 'text, number', 1, 1, 0),
(306, 'Orders', '%order%no%', 'text, number', 1, 1, 0),
(307, 'Orders', '%orders%history%', 'text, number', 1, 1, 0),
(308, 'Orders', '%shop%history%', 'text, number', 1, 1, 0),
(309, 'Orders', '%shop%cart%', 'text, number', 1, 1, 0),
(310, 'Orders', '%cart%', 'text, number', 1, 1, 0),
(311, 'Browsing history', '%browser%history%', 'text', 1, 1, 0),
(312, 'Browsing history', '%browser%', 'text', 1, 1, 0),
(313, 'Browsing history', '%user_agent%', 'text', 1, 1, 0),
(314, 'Browsing history', '%user%agent%', 'text', 1, 1, 0),
(315, 'Geolocation', '%geo%', 'text, number', 1, 1, 0),
(316, 'Geolocation', '%gps%', 'text, number', 1, 1, 0),
(317, 'Geolocation', '%coordinates%', 'text, number', 1, 1, 0),
(318, 'CVV2', '%cvv%', 'text, number', 1, 1, 0),
(319, 'CVV2', '%cvc%', 'text, number', 1, 1, 0),
(320, 'Credit Card No', 'card%number%', 'text, number', 1, 1, 0),
(321, 'Credit Card No', 'card%type%', 'text, number', 1, 1, 0),
(322, 'Address', '%mailing%ad%', 'text', 1, 1, 0),
(323, 'Address Location', '%district%', 'text, number', 1, 1, 0),
(324, 'Marital Status', '%marital%', 'text', 1, 1, 0),
(325, 'Marital Status', '%mar%status%', 'text', 1, 1, 0);

  /*-----------------------------------------------------------------------------------------------------------------------------*/
  INSERT INTO [database_update_log] ([id], [version_no], [change_no]) 
  VALUES (@id, (SELECT CAST(@version AS VARCHAR(2)) +'.'+ CAST(@update AS VARCHAR(1))+'.'+ CAST(@release AS VARCHAR(1))), @change_no);

UPDATE [version] SET [version] = @version, [update] = @update, [release] = @release, [stable] = 1 WHERE [version] = @version AND [update] = @update AND [release] = @release;

END
GO