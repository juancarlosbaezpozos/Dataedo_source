/****** Object:  Index [IX_column_values_column_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_column_values_column_id] ON [column_values]
(
	[column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_column_values_row_count_desc]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_column_values_row_count_desc] ON [column_values]
(
	[row_count] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_columns_desc]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_columns_desc] ON [columns]
(
	[table_id] ASC,
	[name] ASC,
	[title] ASC
)
INCLUDE([description],[field1],[field2],[field3],[field4],[field5],[field6],[field7],[field8],[field9],[field10],[field11],[field12],[field13],[field14],[field15],[field16],[field17],[field18],[field19],[field20],[field21],[field22],[field23],[field24],[field25],[field26],[field27],[field28],[field29],[field30],[field31],[field32],[field33],[field34],[field35],[field36],[field37],[field38],[field39],[field40],[field41],[field42],[field43],[field44],[field45],[field46],[field47],[field48],[field49],[field50],[field51],[field52],[field53],[field54],[field55],[field56],[field57],[field58],[field59],[field60],[field61],[field62],[field63],[field64],[field65],[field66],[field67],[field68],[field69],[field70],[field71],[field72],[field73],[field74],[field75],[field76],[field77],[field78],[field79],[field80],[field81],[field82],[field83],[field84],[field85],[field86],[field87],[field88],[field89],[field90],[field91],[field92],[field93],[field94],[field95],[field96],[field97],[field98],[field99],[field100]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [columns]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [columns]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_columns_changes_update_id_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_columns_changes_update_id_table_id] ON [columns_changes]
(
	[update_id] ASC,
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_custom_fields_values]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_custom_fields_values] ON [custom_fields_values]
(
	[custom_field_id] ASC,
	[object_type] ASC,
	[object_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_custom_fields_values_distinct]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_custom_fields_values_distinct] ON [custom_fields_values]
(
	[custom_field_id] ASC,
	[value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_data_columns_flows_inflow_column_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_data_columns_flows_inflow_column_id] ON [data_columns_flows]
(
	[inflow_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_data_columns_flows_inflow_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_data_columns_flows_inflow_id] ON [data_columns_flows]
(
	[inflow_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_data_columns_flows_outflow_column_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_data_columns_flows_outflow_column_id] ON [data_columns_flows]
(
	[outflow_column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_data_columns_flows_outflow_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_data_columns_flows_outflow_id] ON [data_columns_flows]
(
	[outflow_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_data_flows_process_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_data_flows_process_id] ON [data_flows]
(
	[process_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [databases]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [databases]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referenced_database]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referenced_database] ON [dependencies]
(
	[referenced_database] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referenced_schema_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referenced_schema_name] ON [dependencies]
(
	[referenced_schema] ASC,
	[referenced_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referenced_server]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referenced_server] ON [dependencies]
(
	[referenced_server] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referenced_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referenced_type] ON [dependencies]
(
	[referenced_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referencing_database]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referencing_database] ON [dependencies]
(
	[referencing_database] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referencing_schema_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referencing_schema_name] ON [dependencies]
(
	[referencing_schema] ASC,
	[referencing_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referencing_server]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referencing_server] ON [dependencies]
(
	[referencing_server] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_dependencies_referencing_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_dependencies_referencing_type] ON [dependencies]
(
	[referencing_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_erd_post_its_module_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_erd_post_its_module_id] ON [erd_post_its]
(
	[module_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_creation_date]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_creation_date] ON [feedback]
(
	[creation_date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_last_modification_date]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_last_modification_date] ON [feedback]
(
	[last_modification_date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_resolved]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_resolved] ON [feedback]
(
	[resolved] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_user_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_user_id] ON [feedback]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_creation_date]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_creation_date] ON [feedback_comments]
(
	[creation_date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_feedback_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_feedback_id] ON [feedback_comments]
(
	[feedback_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_last_modification_date]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_last_modification_date] ON [feedback_comments]
(
	[last_modification_date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_user_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_user_id] ON [feedback_comments]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_creation_date]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_creation_date] ON [feedback_links]
(
	[creation_date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_feedback_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_feedback_id] ON [feedback_links]
(
	[feedback_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_last_modification_date]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_last_modification_date] ON [feedback_links]
(
	[last_modification_date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_object_type_object_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_object_type_object_id] ON [feedback_links]
(
	[object_type] ASC,
	[object_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_following_object_type_object_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_following_object_type_object_id] ON [following]
(
	[object_type] ASC,
	[object_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_following_user_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_following_user_id] ON [following]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_glossary_mappings_element_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_glossary_mappings_element_id] ON [glossary_mappings]
(
	[element_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_glossary_mappings_object_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_glossary_mappings_object_id] ON [glossary_mappings]
(
	[object_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_glossary_mappings_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_glossary_mappings_object_type] ON [glossary_mappings]
(
	[object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_glossary_mappings_term_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_glossary_mappings_term_id] ON [glossary_mappings]
(
	[term_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_glossary_term_relationships_destination_term_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_glossary_term_relationships_destination_term_id] ON [glossary_term_relationships]
(
	[destination_term_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UK_glossary_term_relationships]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_glossary_term_relationships] ON [glossary_term_relationships]
(
	[source_term_id] ASC,
	[destination_term_id] ASC,
	[type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_glossary_terms_parent_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_glossary_terms_parent_id] ON [glossary_terms]
(
	[parent_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [glossary_terms]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [glossary_terms]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_columns_column_level]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_column_level] ON [import_columns]
(
	[column_level] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_columns_column_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_column_name] ON [import_columns]
(
	[column_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_columns_column_path]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_column_path] ON [import_columns]
(
	[column_path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_columns_database_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_database_name] ON [import_columns]
(
	[database_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_columns_error_failed]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_error_failed] ON [import_columns]
(
	[error_failed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_columns_item_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_item_type] ON [import_columns]
(
	[item_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_columns_table_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_table_name] ON [import_columns]
(
	[table_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_columns_table_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_table_object_type] ON [import_columns]
(
	[table_object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_columns_table_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_columns_table_schema] ON [import_columns]
(
	[table_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_database_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_database_name] ON [import_procedures]
(
	[database_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_procedures_error_failed]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_error_failed] ON [import_procedures]
(
	[error_failed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_object_subtype]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_object_subtype] ON [import_procedures]
(
	[object_subtype] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_object_type] ON [import_procedures]
(
	[object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_procedure_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_procedure_name] ON [import_procedures]
(
	[procedure_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_procedure_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_procedure_schema] ON [import_procedures]
(
	[procedure_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_parameters_database_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_database_name] ON [import_procedures_parameters]
(
	[database_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_procedures_parameters_error_failed]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_error_failed] ON [import_procedures_parameters]
(
	[error_failed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_parameters_parameter_mode]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_parameter_mode] ON [import_procedures_parameters]
(
	[parameter_mode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_parameters_procedure_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_procedure_name] ON [import_procedures_parameters]
(
	[procedure_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_parameters_procedure_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_procedure_object_type] ON [import_procedures_parameters]
(
	[procedure_object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_procedures_parameters_procedure_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_procedures_parameters_procedure_schema] ON [import_procedures_parameters]
(
	[procedure_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_database_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_database_name] ON [import_tables]
(
	[database_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_tables_error_failed]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_error_failed] ON [import_tables]
(
	[error_failed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_object_subtype]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_object_subtype] ON [import_tables]
(
	[object_subtype] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_object_type] ON [import_tables]
(
	[object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_table_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_table_name] ON [import_tables]
(
	[table_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_table_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_table_schema] ON [import_tables]
(
	[table_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_database_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_database_name] ON [import_tables_foreign_keys_columns]
(
	[database_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_error_failed]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_error_failed] ON [import_tables_foreign_keys_columns]
(
	[error_failed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_foreign_column_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_column_name] ON [import_tables_foreign_keys_columns]
(
	[foreign_column_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_foreign_column_path]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_column_path] ON [import_tables_foreign_keys_columns]
(
	[foreign_column_path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_foreign_table_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_table_name] ON [import_tables_foreign_keys_columns]
(
	[foreign_table_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_foreign_table_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_table_object_type] ON [import_tables_foreign_keys_columns]
(
	[foreign_table_object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_foreign_table_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_foreign_table_schema] ON [import_tables_foreign_keys_columns]
(
	[foreign_table_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_primary_column_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_column_name] ON [import_tables_foreign_keys_columns]
(
	[primary_column_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_primary_column_path]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_column_path] ON [import_tables_foreign_keys_columns]
(
	[primary_column_path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_primary_table_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_table_name] ON [import_tables_foreign_keys_columns]
(
	[primary_table_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_primary_table_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_table_object_type] ON [import_tables_foreign_keys_columns]
(
	[primary_table_object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_foreign_keys_columns_primary_table_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_foreign_keys_columns_primary_table_schema] ON [import_tables_foreign_keys_columns]
(
	[primary_table_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_keys_columns_column_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_column_name] ON [import_tables_keys_columns]
(
	[column_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_keys_columns_column_path]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_column_path] ON [import_tables_keys_columns]
(
	[column_path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_keys_columns_database_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_database_name] ON [import_tables_keys_columns]
(
	[database_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_tables_keys_columns_error_failed]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_error_failed] ON [import_tables_keys_columns]
(
	[error_failed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_keys_columns_key_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_key_type] ON [import_tables_keys_columns]
(
	[key_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_keys_columns_table_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_table_name] ON [import_tables_keys_columns]
(
	[table_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_keys_columns_table_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_table_object_type] ON [import_tables_keys_columns]
(
	[table_object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_tables_keys_columns_table_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_tables_keys_columns_table_schema] ON [import_tables_keys_columns]
(
	[table_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_triggers_database_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_triggers_database_name] ON [import_triggers]
(
	[database_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_import_triggers_error_failed]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_triggers_error_failed] ON [import_triggers]
(
	[error_failed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_triggers_table_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_triggers_table_name] ON [import_triggers]
(
	[table_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_triggers_table_object_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_triggers_table_object_type] ON [import_triggers]
(
	[table_object_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_triggers_table_schema]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_triggers_table_schema] ON [import_triggers]
(
	[table_schema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_import_triggers_trigger_type]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_import_triggers_trigger_type] ON [import_triggers]
(
	[trigger_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_licenses_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_licenses_name] ON [licenses]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_modules_database_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_modules_database_id] ON [modules]
(
	[database_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [modules]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [modules]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [parameters]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [parameters]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_parameters_procedure_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_parameters_procedure_id] ON [parameters]
(
	[procedure_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_parameters_changes_update_id_procedure_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_parameters_changes_update_id_procedure_id] ON [parameters_changes]
(
	[update_id] ASC,
	[procedure_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_permissions_database_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_permissions_database_id] ON [permissions]
(
	[database_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_permissions_object_type_database_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_permissions_object_type_database_id] ON [permissions]
(
	[object_type] ASC,
	[database_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_permissions_role_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_permissions_role_id] ON [permissions]
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_permissions_user_group_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_permissions_user_group_id] ON [permissions]
(
	[user_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_permissions_user_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_permissions_user_id] ON [permissions]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_permissions_user_type_user_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_permissions_user_type_user_id] ON [permissions]
(
	[user_type] ASC,
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [procedures]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [procedures]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_procedures_database_type_status]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_procedures_database_type_status] ON [procedures]
(
	[database_id] ASC,
	[object_type] ASC,
	[status] ASC
)
INCLUDE([schema],[name],[title],[dbms_creation_date],[dbms_last_modification_date],[synchronization_date]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_procedure_changes_procedure_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_procedure_changes_procedure_id] ON [procedures_changes]
(
	[procedure_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UK_procedures_modules]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_procedures_modules] ON [procedures_modules]
(
	[procedure_id] ASC,
	[module_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_role_actions_action_code]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_role_actions_action_code] ON [role_actions]
(
	[action_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_role_actions_role_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_role_actions_role_id] ON [role_actions]
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_roles_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_roles_name] ON [roles]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_sessions_login]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_sessions_login] ON [sessions]
(
	[login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_sessions_user_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_sessions_user_id] ON [sessions]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [tables]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [tables]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_tables_database_type_status]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_tables_database_type_status] ON [tables]
(
	[database_id] ASC,
	[object_type] ASC,
	[status] ASC
)
INCLUDE([schema],[name],[title],[dbms_creation_date],[dbms_last_modification_date],[synchronization_date]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tables_changes_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_tables_changes_table_id] ON [tables_changes]
(
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UK_tables_modules]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_tables_modules] ON [tables_modules]
(
	[table_id] ASC,
	[module_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_tables_relations_fk_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_tables_relations_fk_table_id] ON [tables_relations]
(
	[fk_table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_tables_relations_pk_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_tables_relations_pk_table_id] ON [tables_relations]
(
	[pk_table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tables_relations_changes_update_id_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_tables_relations_changes_update_id_table_id] ON [tables_relations_changes]
(
	[update_id] ASC,
	[fk_table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_tables_relations_columns_column_fk_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_tables_relations_columns_column_fk_id] ON [tables_relations_columns]
(
	[column_fk_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_tables_relations_columns_column_pk_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_tables_relations_columns_column_pk_id] ON [tables_relations_columns]
(
	[column_pk_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_tables_relations_columns_table_relation_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_tables_relations_columns_table_relation_id] ON [tables_relations_columns]
(
	[table_relation_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tables_stats_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_tables_stats_table_id] ON [tables_stats]
(
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_todos_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_todos_count] ON [triggers]
(
	[open_todos_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_open_warnings_count]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_open_warnings_count] ON [triggers]
(
	[open_warnings_count] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_triggers_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_triggers_table_id] ON [triggers]
(
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_triggers_changes_update_id_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_triggers_changes_update_id_table_id] ON [triggers_changes]
(
	[update_id] ASC,
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ix_unique_constraints_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_unique_constraints_table_id] ON [unique_constraints]
(
	[table_id] ASC,
	[status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_unique_constraints_changes_update_id_table_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_unique_constraints_changes_update_id_table_id] ON [unique_constraints_changes]
(
	[update_id] ASC,
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_unique_constraints_columns_column_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_unique_constraints_columns_column_id] ON [unique_constraints_columns]
(
	[column_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ix_unique_constraints_columns_uc_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [ix_unique_constraints_columns_uc_id] ON [unique_constraints_columns]
(
	[unique_constraint_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UK_user_connections_license_id_database_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_user_connections_license_id_database_id] ON [user_connections]
(
	[license_id] ASC,
	[database_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_user_groups_name]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_user_groups_name] ON [user_groups]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_users_user_groups_user_group_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_users_user_groups_user_group_id] ON [users_user_groups]
(
	[user_group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_users_user_groups_user_id]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_users_user_groups_user_id] ON [users_user_groups]
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UK_version]    Script Date: 7/5/2022 9:26:44 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UK_version] ON [version]
(
	[version] ASC,
	[update] ASC,
	[release] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [changes_history] ADD  CONSTRAINT [DF_changes_history_datetime]  DEFAULT (getdate()) FOR [datetime]
GO
ALTER TABLE [changes_history] ADD  CONSTRAINT [DF_changes_history_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_sort]  DEFAULT ((99)) FOR [sort]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_is_column]  DEFAULT ((1)) FOR [is_column]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_is_title]  DEFAULT ((0)) FOR [is_title]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_is_description]  DEFAULT ((0)) FOR [is_description]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [classificator_masks] ADD  CONSTRAINT [DF_classificator_masks_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [classificator_rules] ADD  CONSTRAINT [DF_classificator_rules_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [classificator_rules] ADD  CONSTRAINT [DF_classificator_rules_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [classificator_rules] ADD  CONSTRAINT [DF_classificator_rules_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [classificator_rules] ADD  CONSTRAINT [DF_classificator_rules_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [classificators] ADD  CONSTRAINT [DF_classificators_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [classificators] ADD  CONSTRAINT [DF_classificators_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [classificators] ADD  CONSTRAINT [DF_classificators_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [classificators] ADD  CONSTRAINT [DF_classificators_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [column_values] ADD  CONSTRAINT [DF_column_values_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_primary_key]  DEFAULT ((0)) FOR [primary_key]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_nullable]  DEFAULT ((1)) FOR [nullable]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_Column_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_Column_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_Column_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_Column_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF__columns__is_iden__4AB81AF0]  DEFAULT ((0)) FOR [is_identity]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF__columns__is_comp__4BAC3F29]  DEFAULT ((0)) FOR [is_computed]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_source]  DEFAULT (N'DBMS') FOR [source]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_sort]  DEFAULT ((99999)) FOR [sort]
GO
ALTER TABLE [columns] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [columns] ADD  DEFAULT ((1)) FOR [level]
GO
ALTER TABLE [columns] ADD  DEFAULT ('COLUMN') FOR [item_type]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [columns] ADD  CONSTRAINT [DF_columns_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [columns] ADD  DEFAULT ((10)) FOR [values_list_rows_count]
GO
ALTER TABLE [columns] ADD  DEFAULT ((0)) FOR [refresh_at_import]
GO
ALTER TABLE [columns_changes] ADD  CONSTRAINT [DF_columns_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [columns_changes] ADD  CONSTRAINT [DF_columns_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [configuration] ADD  CONSTRAINT [DF_configuration_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [configuration] ADD  CONSTRAINT [DF_configuration_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [custom_field_classes] ADD  CONSTRAINT [DF_custom_field_classes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [custom_field_classes] ADD  CONSTRAINT [DF_custom_field_classes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [custom_field_classes] ADD  CONSTRAINT [DF_custom_field_classes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [custom_field_classes] ADD  CONSTRAINT [DF_custom_field_classes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_table_visibility]  DEFAULT ((1)) FOR [table_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_procedure_visibility]  DEFAULT ((1)) FOR [procedure_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_column_visibility]  DEFAULT ((1)) FOR [column_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_relation_visibility]  DEFAULT ((1)) FOR [relation_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_key_visibility]  DEFAULT ((1)) FOR [key_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_trigger_visibility]  DEFAULT ((1)) FOR [trigger_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_parameter_visibility]  DEFAULT ((1)) FOR [parameter_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_module_visibility]  DEFAULT ((1)) FOR [module_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_documentation_visibility]  DEFAULT ((1)) FOR [documentation_visibility]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [custom_fields] ADD  CONSTRAINT [DF_custom_fields_type]  DEFAULT (N'TEXT') FOR [type]
GO
ALTER TABLE [custom_fields] ADD  DEFAULT ((1)) FOR [term_visibility]
GO
ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [data_columns_flows] ADD  CONSTRAINT [DF_data_columns_flows_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [data_flows] ADD  CONSTRAINT [DF_data_flows_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [data_processes] ADD  CONSTRAINT [DF_data_processes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [database_update_log] ADD  CONSTRAINT [DF_database_update_log_executed]  DEFAULT (getdate()) FOR [executed]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_Database_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_Database_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_Database_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_Database_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_Databases_guid]  DEFAULT (newsequentialid()) FOR [guid]
GO
ALTER TABLE [databases] ADD  DEFAULT ((1)) FOR [show_schema]
GO
ALTER TABLE [databases] ADD  DEFAULT (NULL) FOR [show_schema_override]
GO
ALTER TABLE [databases] ADD  DEFAULT ('DATABASE') FOR [class]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [databases] ADD  CONSTRAINT [DF_databases_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [databases] ADD  DEFAULT (NULL) FOR [connection_role]
GO
ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [datatypes] ADD  CONSTRAINT [DF_datatypes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [dependencies] ADD  CONSTRAINT [DF_dependencies_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [dependencies] ADD  CONSTRAINT [DF_dependencies_source]  DEFAULT ('DBMS') FOR [source]
GO
ALTER TABLE [dependencies] ADD  CONSTRAINT [DF_dependencies_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [dependencies] ADD  CONSTRAINT [DF_dependencies_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [dependencies] ADD  CONSTRAINT [DF_dependencies_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [dependencies] ADD  CONSTRAINT [DF_dependencies_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [dependencies] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [dependencies_descriptions] ADD  CONSTRAINT [DF_dependencies_additions_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [documentation_custom_fields] ADD  CONSTRAINT [DF_documentation_custom_fields_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [documentation_custom_fields] ADD  CONSTRAINT [DF_documentation_custom_fields_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [documentation_custom_fields] ADD  CONSTRAINT [DF_documentation_custom_fields_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [documentation_custom_fields] ADD  CONSTRAINT [DF_documentation_custom_fields_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [erd_links] ADD  CONSTRAINT [DF_erd_links_show_label]  DEFAULT ((0)) FOR [show_label]
GO
ALTER TABLE [erd_links] ADD  CONSTRAINT [DF_erd_links_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [erd_links] ADD  CONSTRAINT [DF_erd_links_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [erd_links] ADD  CONSTRAINT [DF_erd_links_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [erd_links] ADD  CONSTRAINT [DF_erd_links_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [erd_links] ADD  CONSTRAINT [DF_erd_links_hidden]  DEFAULT ((0)) FOR [hidden]
GO
ALTER TABLE [erd_links] ADD  CONSTRAINT [DF_erd_links_link_style]  DEFAULT ('STRAIGHT') FOR [link_style]
GO
ALTER TABLE [erd_links] ADD  DEFAULT ((0)) FOR [show_join_condition]
GO
ALTER TABLE [erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [erd_nodes] ADD  CONSTRAINT [DF_erd_nodes_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [erd_post_its] ADD  CONSTRAINT [DF_erd_post_its_pos_x]  DEFAULT ((0)) FOR [pos_x]
GO
ALTER TABLE [erd_post_its] ADD  CONSTRAINT [DF_erd_post_its_pos_y]  DEFAULT ((0)) FOR [pos_y]
GO
ALTER TABLE [erd_post_its] ADD  CONSTRAINT [DF_erd_post_its_pos_z]  DEFAULT ((100)) FOR [pos_z]
GO
ALTER TABLE [erd_post_its] ADD  CONSTRAINT [DF_erd_post_its_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [erd_post_its] ADD  CONSTRAINT [DF_erd_post_its_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [erd_post_its] ADD  CONSTRAINT [DF_erd_post_its_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [erd_post_its] ADD  CONSTRAINT [DF_erd_post_its_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [feedback] ADD  CONSTRAINT [DF_feedback_type]  DEFAULT ('COMMENT') FOR [type]
GO
ALTER TABLE [feedback] ADD  CONSTRAINT [DF_feedback_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [feedback] ADD  CONSTRAINT [DF_feedback_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [feedback_comments] ADD  CONSTRAINT [DF_feedback_comments_action]  DEFAULT ('COMMENT') FOR [action]
GO
ALTER TABLE [feedback_comments] ADD  CONSTRAINT [DF_feedback_comments_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [feedback_comments] ADD  CONSTRAINT [DF_feedback_comments_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [feedback_links] ADD  CONSTRAINT [DF_feedback_links_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [feedback_links] ADD  CONSTRAINT [DF_feedback_links_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [feedback_links] ADD  CONSTRAINT [DF_feedback_links_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [feedback_links] ADD  CONSTRAINT [DF_feedback_links_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [following] ADD  CONSTRAINT [DF_following_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [following] ADD  CONSTRAINT [DF_following_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [following] ADD  CONSTRAINT [DF_following_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [following] ADD  CONSTRAINT [DF_following_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [glossary_mappings] ADD  CONSTRAINT [DF_glossary_mappings_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [glossary_mappings] ADD  CONSTRAINT [DF_glossary_mappings_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [glossary_mappings] ADD  CONSTRAINT [DF_glossary_mappings_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [glossary_mappings] ADD  CONSTRAINT [DF_glossary_mappings_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [glossary_term_relationship_types] ADD  CONSTRAINT [DF_glossary_term_relationship_types_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [glossary_term_relationship_types] ADD  CONSTRAINT [DF_glossary_term_relationship_types_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [glossary_term_relationship_types] ADD  CONSTRAINT [DF_glossary_term_relationship_types_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [glossary_term_relationship_types] ADD  CONSTRAINT [DF_glossary_term_relationship_types_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [glossary_term_relationship_types] ADD  DEFAULT ((99)) FOR [sort]
GO
ALTER TABLE [glossary_term_relationship_types] ADD  DEFAULT ((99)) FOR [sort_reverse]
GO
ALTER TABLE [glossary_term_relationships] ADD  CONSTRAINT [DF_glossary_term_relationships_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [glossary_term_relationships] ADD  CONSTRAINT [DF_glossary_term_relationships_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [glossary_term_relationships] ADD  CONSTRAINT [DF_glossary_term_relationships_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [glossary_term_relationships] ADD  CONSTRAINT [DF_glossary_term_relationships_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [glossary_term_types] ADD  CONSTRAINT [DF_glossary_term_types_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [glossary_term_types] ADD  CONSTRAINT [DF_glossary_term_types_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [glossary_term_types] ADD  CONSTRAINT [DF_glossary_term_types_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [glossary_term_types] ADD  CONSTRAINT [DF_glossary_term_types_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [glossary_term_types] ADD  CONSTRAINT [DF_glossary_term_types_list_name]  DEFAULT ('') FOR [list_name]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [glossary_terms] ADD  CONSTRAINT [DF_glossary_terms_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [guid] ADD  CONSTRAINT [DF_guid_guid]  DEFAULT (newsequentialid()) FOR [guid]
GO
ALTER TABLE [guid] ADD  DEFAULT ((0)) FOR [is_web_portal_connected]
GO
ALTER TABLE [ignored_objects] ADD  CONSTRAINT [DF_ignored_objects_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [ignored_objects] ADD  CONSTRAINT [DF_ignored_objects_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [ignored_objects] ADD  CONSTRAINT [DF_ignored_objects_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [ignored_objects] ADD  CONSTRAINT [DF_ignored_objects_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [import_columns] ADD  CONSTRAINT [DF_import_columns_table_object_type]  DEFAULT (N'TABLE') FOR [table_object_type]
GO
ALTER TABLE [import_columns] ADD  CONSTRAINT [DF_import_columns_table_column_level]  DEFAULT ((1)) FOR [column_level]
GO
ALTER TABLE [import_columns] ADD  CONSTRAINT [DF_import_columns_table_item_type]  DEFAULT (N'COLUMN') FOR [item_type]
GO
ALTER TABLE [import_columns] ADD  CONSTRAINT [DF_import_columns_table_is_identity]  DEFAULT ((0)) FOR [is_identity]
GO
ALTER TABLE [import_columns] ADD  CONSTRAINT [DF_import_columns_table_is_computed]  DEFAULT ((0)) FOR [is_computed]
GO
ALTER TABLE [import_columns] ADD  CONSTRAINT [DF_import_columns_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [import_errors] ADD  CONSTRAINT [DF_import_errors_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [import_procedures] ADD  CONSTRAINT [DF_import_procedures_object_type]  DEFAULT (N'PROCEDURE') FOR [object_type]
GO
ALTER TABLE [import_procedures] ADD  CONSTRAINT [DF_import_procedures_object_subtype]  DEFAULT (N'PROCEDURE') FOR [object_subtype]
GO
ALTER TABLE [import_procedures] ADD  CONSTRAINT [DF_import_procedures_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [import_procedures_parameters] ADD  CONSTRAINT [DF_import_procedures_procedure_object_type]  DEFAULT (N'PROCEDURE') FOR [procedure_object_type]
GO
ALTER TABLE [import_procedures_parameters] ADD  CONSTRAINT [DF_import_procedures_parameter_mode]  DEFAULT (N'IN') FOR [parameter_mode]
GO
ALTER TABLE [import_procedures_parameters] ADD  CONSTRAINT [DF_import_procedures_parameters_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [import_tables] ADD  CONSTRAINT [DF_import_tables_object_type]  DEFAULT (N'TABLE') FOR [object_type]
GO
ALTER TABLE [import_tables] ADD  CONSTRAINT [DF_import_tables_object_subtype]  DEFAULT (N'TABLE') FOR [object_subtype]
GO
ALTER TABLE [import_tables] ADD  CONSTRAINT [DF_import_tables_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [import_tables_foreign_keys_columns] ADD  CONSTRAINT [DF_import_tables_foreign_keys_columns_foreign_table_object_type]  DEFAULT (N'TABLE') FOR [foreign_table_object_type]
GO
ALTER TABLE [import_tables_foreign_keys_columns] ADD  CONSTRAINT [DF_import_tables_foreign_keys_columns_primary_table_object_type]  DEFAULT (N'TABLE') FOR [primary_table_object_type]
GO
ALTER TABLE [import_tables_foreign_keys_columns] ADD  CONSTRAINT [DF_import_tables_foreign_keys_columns_column_pair_order]  DEFAULT ((1)) FOR [column_pair_order]
GO
ALTER TABLE [import_tables_foreign_keys_columns] ADD  CONSTRAINT [DF_import_tables_foreign_keys_columns_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [import_tables_keys_columns] ADD  CONSTRAINT [DF_import_tables_keys_columns_table_object_type]  DEFAULT (N'TABLE') FOR [table_object_type]
GO
ALTER TABLE [import_tables_keys_columns] ADD  CONSTRAINT [DF_import_tables_keys_columns_key_type]  DEFAULT (N'PK') FOR [key_type]
GO
ALTER TABLE [import_tables_keys_columns] ADD  CONSTRAINT [DF_import_tables_keys_disabled]  DEFAULT ((0)) FOR [disabled]
GO
ALTER TABLE [import_tables_keys_columns] ADD  CONSTRAINT [DF_import_tables_keys_column_order]  DEFAULT ((1)) FOR [column_order]
GO
ALTER TABLE [import_tables_keys_columns] ADD  CONSTRAINT [DF_import_tables_keys_columns_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_table_object_type]  DEFAULT (N'TABLE') FOR [table_object_type]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_trigger_type]  DEFAULT (N'TRIGGER') FOR [trigger_type]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_before]  DEFAULT ((0)) FOR [before]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_after]  DEFAULT ((0)) FOR [after]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_instead_of]  DEFAULT ((0)) FOR [instead_of]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_on_insert]  DEFAULT ((0)) FOR [on_insert]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_on_update]  DEFAULT ((0)) FOR [on_update]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_on_delete]  DEFAULT ((0)) FOR [on_delete]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_disabled]  DEFAULT ((0)) FOR [disabled]
GO
ALTER TABLE [import_triggers] ADD  CONSTRAINT [DF_import_triggers_parameters_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [licenses] ADD  CONSTRAINT [DF_licenses_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [licenses] ADD  CONSTRAINT [DF_licenses_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [licenses] ADD  CONSTRAINT [DF_licenses_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [licenses] ADD  CONSTRAINT [DF_licenses_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [licenses] ADD  DEFAULT ((0)) FOR [deleted]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_Modules_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_Modules_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_Modules_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_Modules_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_Modules_erd_link_style]  DEFAULT (N'STRAIGHT') FOR [erd_link_style]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_Modules_erd_show_types]  DEFAULT ((0)) FOR [erd_show_types]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_Modules_display_documentation_name_mode]  DEFAULT (N'EXTERNAL_ENTITIES_ONLY') FOR [display_documentation_name_mode]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [modules] ADD  CONSTRAINT [DF_modules_erd_show_nullable]  DEFAULT ((0)) FOR [erd_show_nullable]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_source]  DEFAULT (N'DBMS') FOR [source]
GO
ALTER TABLE [parameters] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [parameters] ADD  CONSTRAINT [DF_parameters_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [parameters_changes] ADD  CONSTRAINT [DF_parameters_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [parameters_changes] ADD  CONSTRAINT [DF_parameters_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_Procedure_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_Procedure_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_Procedure_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_Procedure_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_source]  DEFAULT (N'DBMS') FOR [source]
GO
ALTER TABLE [procedures] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [procedures] ADD  CONSTRAINT [DF_procedures_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [procedures_changes] ADD  CONSTRAINT [DF_procedures_changes_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [procedures_changes] ADD  CONSTRAINT [DF_procedures_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [procedures_modules] ADD  CONSTRAINT [DF_procedures_modules_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [procedures_modules] ADD  CONSTRAINT [DF_procedures_modules_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [procedures_modules] ADD  CONSTRAINT [DF_procedures_modules_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [procedures_modules] ADD  CONSTRAINT [DF_procedures_modules_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [schema_updates] ADD  CONSTRAINT [DF_schema_updates_datetime]  DEFAULT (getdate()) FOR [datetime]
GO
ALTER TABLE [schema_updates] ADD  CONSTRAINT [DF_schema_updates_repository_login]  DEFAULT (suser_sname()) FOR [repository_login]
GO
ALTER TABLE [schema_updates] ADD  CONSTRAINT [DF_schema_updates_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [schema_updates] ADD  CONSTRAINT [DF_schema_updates_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [schema_updates] ADD  CONSTRAINT [DF_schema_updates_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [schema_updates] ADD  CONSTRAINT [DF_schema_updates_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [sessions] ADD  CONSTRAINT [DF_sessions_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_source]  DEFAULT (N'DBMS') FOR [source]
GO
ALTER TABLE [tables] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [tables] ADD  DEFAULT ('SQL') FOR [language]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [tables] ADD  CONSTRAINT [DF_tables_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [tables_changes] ADD  CONSTRAINT [DF_tables_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables_changes] ADD  CONSTRAINT [DF_tables_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables_modules] ADD  CONSTRAINT [DF_tables_modules_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables_modules] ADD  CONSTRAINT [DF_tables_modules_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables_modules] ADD  CONSTRAINT [DF_tables_modules_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [tables_modules] ADD  CONSTRAINT [DF_tables_modules_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_source]  DEFAULT ('USER') FOR [source]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_fk_type]  DEFAULT (N'MANY') FOR [fk_type]
GO
ALTER TABLE [tables_relations] ADD  CONSTRAINT [DF_tables_relations_pk_type]  DEFAULT (N'ONE') FOR [pk_type]
GO
ALTER TABLE [tables_relations] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [tables_relations_changes] ADD  CONSTRAINT [DF_tables_relations_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables_relations_changes] ADD  CONSTRAINT [DF_tables_relations_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables_relations_columns] ADD  CONSTRAINT [DF_tables_relations_columns_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [tables_relations_columns] ADD  CONSTRAINT [DF_tables_relations_columns_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables_relations_columns] ADD  CONSTRAINT [DF_tables_relations_columns_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables_relations_columns] ADD  CONSTRAINT [DF_tables_relations_columns_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [tables_relations_columns] ADD  CONSTRAINT [DF_tables_relations_columns_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [tables_relations_columns] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [tables_relations_columns_changes] ADD  CONSTRAINT [DF_tables_relations_columns_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables_relations_columns_changes] ADD  CONSTRAINT [DF_tables_relations_columns_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [tables_stats] ADD  CONSTRAINT [DF_tables_stats_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_before]  DEFAULT ((0)) FOR [before]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_after]  DEFAULT ((0)) FOR [after]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_instead_of]  DEFAULT ((0)) FOR [instead_of]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_on_insert]  DEFAULT ((0)) FOR [on_insert]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_on_update]  DEFAULT ((0)) FOR [on_update]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_on_delete]  DEFAULT ((0)) FOR [on_delete]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_disabled]  DEFAULT ((0)) FOR [disabled]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_type]  DEFAULT (N'TRIGGER') FOR [type]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_source]  DEFAULT (N'DBMS') FOR [source]
GO
ALTER TABLE [triggers] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_comments_count]  DEFAULT ((0)) FOR [comments_count]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_rating]  DEFAULT ((0)) FOR [rating]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_rating_count]  DEFAULT ((0)) FOR [rating_count]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_open_warnings_count]  DEFAULT ((0)) FOR [open_warnings_count]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_warnings_count]  DEFAULT ((0)) FOR [warnings_count]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_open_todos_count]  DEFAULT ((0)) FOR [open_todos_count]
GO
ALTER TABLE [triggers] ADD  CONSTRAINT [DF_triggers_todos_count]  DEFAULT ((0)) FOR [todos_count]
GO
ALTER TABLE [triggers_changes] ADD  CONSTRAINT [DF_triggers_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [triggers_changes] ADD  CONSTRAINT [DF_triggers_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [unique_constraints] ADD  CONSTRAINT [DF_unique_constraints_source]  DEFAULT ('USER') FOR [source]
GO
ALTER TABLE [unique_constraints] ADD  CONSTRAINT [DF_unique_constraints_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [unique_constraints] ADD  CONSTRAINT [DF_unique_constraints_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [unique_constraints] ADD  CONSTRAINT [DF_unique_constraints_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [unique_constraints] ADD  CONSTRAINT [DF_unique_constraints_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [unique_constraints] ADD  CONSTRAINT [DF_unique_constraints_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [unique_constraints] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [unique_constraints_changes] ADD  CONSTRAINT [DF_unique_constraints_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [unique_constraints_changes] ADD  CONSTRAINT [DF_unique_constraints_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [unique_constraints_columns] ADD  CONSTRAINT [DF_unique_constraints_columns_status]  DEFAULT ('A') FOR [status]
GO
ALTER TABLE [unique_constraints_columns] ADD  CONSTRAINT [DF_unique_constraints_columns_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [unique_constraints_columns] ADD  CONSTRAINT [DF_unique_constraints_columns_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [unique_constraints_columns] ADD  CONSTRAINT [DF_unique_constraints_columns_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [unique_constraints_columns] ADD  CONSTRAINT [DF_unique_constraints_columns_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [unique_constraints_columns] ADD  DEFAULT ((0)) FOR [temp_sync_status]
GO
ALTER TABLE [unique_constraints_columns_changes] ADD  CONSTRAINT [DF_unique_constraints_columns_changes_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [unique_constraints_columns_changes] ADD  CONSTRAINT [DF_unique_constraints_columns_changes_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [user_connections] ADD  CONSTRAINT [DF_user_connections_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [user_connections] ADD  CONSTRAINT [DF_user_connections_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [user_connections] ADD  CONSTRAINT [DF_user_connections_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [user_connections] ADD  CONSTRAINT [DF_user_connections_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [user_connections] ADD  DEFAULT (NULL) FOR [connection_role]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF__version__update__339FAB6E]  DEFAULT ((0)) FOR [update]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF_version_installation_date]  DEFAULT (getdate()) FOR [installation_date]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF_version_installed_by]  DEFAULT (suser_sname()) FOR [installed_by]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF_version_creation_date]  DEFAULT (getdate()) FOR [creation_date]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF_version_created_by]  DEFAULT (suser_sname()) FOR [created_by]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF_version_last_modification_date]  DEFAULT (getdate()) FOR [last_modification_date]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF_version_modified_by]  DEFAULT (suser_sname()) FOR [modified_by]
GO
ALTER TABLE [version] ADD  CONSTRAINT [DF__version__release]  DEFAULT ((0)) FOR [release]
GO
ALTER TABLE [classificator_rules]  WITH CHECK ADD  CONSTRAINT [FK_classificator_rules_classificators] FOREIGN KEY([classificator_id])
REFERENCES [classificators] ([classificator_id])
GO
ALTER TABLE [classificator_rules] CHECK CONSTRAINT [FK_classificator_rules_classificators]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_1] FOREIGN KEY([custom_field_1_id])
REFERENCES [custom_fields] ([custom_field_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_1]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_2] FOREIGN KEY([custom_field_2_id])
REFERENCES [custom_fields] ([custom_field_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_2]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_3] FOREIGN KEY([custom_field_3_id])
REFERENCES [custom_fields] ([custom_field_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_3]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_4] FOREIGN KEY([custom_field_4_id])
REFERENCES [custom_fields] ([custom_field_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_4]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_5] FOREIGN KEY([custom_field_5_id])
REFERENCES [custom_fields] ([custom_field_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_5]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_1] FOREIGN KEY([custom_field_1_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_1]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_2] FOREIGN KEY([custom_field_2_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_2]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_3] FOREIGN KEY([custom_field_3_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_3]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_4] FOREIGN KEY([custom_field_4_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_4]
GO
ALTER TABLE [classificators]  WITH CHECK ADD  CONSTRAINT [FK_classificators_custom_fields_classes_5] FOREIGN KEY([custom_field_5_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO
ALTER TABLE [classificators] CHECK CONSTRAINT [FK_classificators_custom_fields_classes_5]
GO
ALTER TABLE [column_values]  WITH CHECK ADD  CONSTRAINT [FK_column_values_columns] FOREIGN KEY([column_id])
REFERENCES [columns] ([column_id])
ON DELETE CASCADE
GO
ALTER TABLE [column_values] CHECK CONSTRAINT [FK_column_values_columns]
GO
ALTER TABLE [columns]  WITH CHECK ADD  CONSTRAINT [FK_column_table] FOREIGN KEY([table_id])
REFERENCES [tables] ([table_id])
ON DELETE CASCADE
GO
ALTER TABLE [columns] CHECK CONSTRAINT [FK_column_table]
GO
ALTER TABLE [columns]  WITH CHECK ADD  CONSTRAINT [FK_columns_column_parent_columns] FOREIGN KEY([parent_id])
REFERENCES [columns] ([column_id])
GO
ALTER TABLE [columns] CHECK CONSTRAINT [FK_columns_column_parent_columns]
GO
ALTER TABLE [custom_fields]  WITH CHECK ADD  CONSTRAINT [FK_custom_fields_custom_field_classes] FOREIGN KEY([custom_field_class_id])
REFERENCES [custom_field_classes] ([custom_field_class_id])
GO
ALTER TABLE [custom_fields] CHECK CONSTRAINT [FK_custom_fields_custom_field_classes]
GO
ALTER TABLE [custom_fields_values]  WITH CHECK ADD  CONSTRAINT [FK_custom_fields_values_custom_fields] FOREIGN KEY([custom_field_id])
REFERENCES [custom_fields] ([custom_field_id])
ON DELETE CASCADE
GO
ALTER TABLE [custom_fields_values] CHECK CONSTRAINT [FK_custom_fields_values_custom_fields]
GO
ALTER TABLE [data_columns_flows]  WITH NOCHECK ADD  CONSTRAINT [FK_data_columns_flows_data_flows_inflow] FOREIGN KEY([inflow_id])
REFERENCES [data_flows] ([flow_id])
GO
ALTER TABLE [data_columns_flows] NOCHECK CONSTRAINT [FK_data_columns_flows_data_flows_inflow]
GO
ALTER TABLE [data_columns_flows]  WITH NOCHECK ADD  CONSTRAINT [FK_data_columns_flows_data_flows_outflow] FOREIGN KEY([outflow_id])
REFERENCES [data_flows] ([flow_id])
GO
ALTER TABLE [data_columns_flows] NOCHECK CONSTRAINT [FK_data_columns_flows_data_flows_outflow]
GO
ALTER TABLE [data_flows]  WITH CHECK ADD  CONSTRAINT [FK_data_flows_data_processes] FOREIGN KEY([process_id])
REFERENCES [data_processes] ([process_id])
ON DELETE CASCADE
GO
ALTER TABLE [data_flows] CHECK CONSTRAINT [FK_data_flows_data_processes]
GO
ALTER TABLE [dependencies_descriptions]  WITH CHECK ADD  CONSTRAINT [FK_dependencies_desrciptions_databases] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
GO
ALTER TABLE [dependencies_descriptions] CHECK CONSTRAINT [FK_dependencies_desrciptions_databases]
GO
ALTER TABLE [documentation_custom_fields]  WITH CHECK ADD  CONSTRAINT [FK_documentation_custom_fields_custom_fields] FOREIGN KEY([custom_field_id])
REFERENCES [custom_fields] ([custom_field_id])
GO
ALTER TABLE [documentation_custom_fields] CHECK CONSTRAINT [FK_documentation_custom_fields_custom_fields]
GO
ALTER TABLE [documentation_custom_fields]  WITH CHECK ADD  CONSTRAINT [FK_documentation_custom_fields_databases] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
GO
ALTER TABLE [documentation_custom_fields] CHECK CONSTRAINT [FK_documentation_custom_fields_databases]
GO
ALTER TABLE [erd_links]  WITH CHECK ADD  CONSTRAINT [FK_erd_links_modules] FOREIGN KEY([module_id])
REFERENCES [modules] ([module_id])
GO
ALTER TABLE [erd_links] CHECK CONSTRAINT [FK_erd_links_modules]
GO
ALTER TABLE [erd_links]  WITH CHECK ADD  CONSTRAINT [FK_erd_links_tables_relations] FOREIGN KEY([relation_id])
REFERENCES [tables_relations] ([table_relation_id])
GO
ALTER TABLE [erd_links] CHECK CONSTRAINT [FK_erd_links_tables_relations]
GO
ALTER TABLE [erd_nodes]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_modules] FOREIGN KEY([module_id])
REFERENCES [modules] ([module_id])
GO
ALTER TABLE [erd_nodes] CHECK CONSTRAINT [FK_erd_nodes_modules]
GO
ALTER TABLE [erd_nodes]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_tables] FOREIGN KEY([table_id])
REFERENCES [tables] ([table_id])
GO
ALTER TABLE [erd_nodes] CHECK CONSTRAINT [FK_erd_nodes_tables]
GO
ALTER TABLE [erd_nodes_columns]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_columns_columns] FOREIGN KEY([column_id])
REFERENCES [columns] ([column_id])
GO
ALTER TABLE [erd_nodes_columns] CHECK CONSTRAINT [FK_erd_nodes_columns_columns]
GO
ALTER TABLE [erd_nodes_columns]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_columns_erd_nodes] FOREIGN KEY([node_id])
REFERENCES [erd_nodes] ([node_id])
GO
ALTER TABLE [erd_nodes_columns] CHECK CONSTRAINT [FK_erd_nodes_columns_erd_nodes]
GO
ALTER TABLE [erd_nodes_columns]  WITH CHECK ADD  CONSTRAINT [FK_erd_nodes_columns_modules] FOREIGN KEY([module_id])
REFERENCES [modules] ([module_id])
GO
ALTER TABLE [erd_nodes_columns] CHECK CONSTRAINT [FK_erd_nodes_columns_modules]
GO
ALTER TABLE [erd_post_its]  WITH CHECK ADD  CONSTRAINT [FK_erd_post_its_modules] FOREIGN KEY([module_id])
REFERENCES [modules] ([module_id])
ON DELETE CASCADE
GO
ALTER TABLE [erd_post_its] CHECK CONSTRAINT [FK_erd_post_its_modules]
GO
ALTER TABLE [feedback]  WITH CHECK ADD  CONSTRAINT [FK_feedback_licenses] FOREIGN KEY([user_id])
REFERENCES [licenses] ([license_id])
GO
ALTER TABLE [feedback] CHECK CONSTRAINT [FK_feedback_licenses]
GO
ALTER TABLE [feedback_comments]  WITH CHECK ADD  CONSTRAINT [FK_feedback_comments_feedback] FOREIGN KEY([feedback_id])
REFERENCES [feedback] ([feedback_id])
GO
ALTER TABLE [feedback_comments] CHECK CONSTRAINT [FK_feedback_comments_feedback]
GO
ALTER TABLE [feedback_comments]  WITH CHECK ADD  CONSTRAINT [FK_feedback_comments_licenses] FOREIGN KEY([user_id])
REFERENCES [licenses] ([license_id])
GO
ALTER TABLE [feedback_comments] CHECK CONSTRAINT [FK_feedback_comments_licenses]
GO
ALTER TABLE [feedback_links]  WITH CHECK ADD  CONSTRAINT [FK_feedback_links_feedback] FOREIGN KEY([feedback_id])
REFERENCES [feedback] ([feedback_id])
GO
ALTER TABLE [feedback_links] CHECK CONSTRAINT [FK_feedback_links_feedback]
GO
ALTER TABLE [following]  WITH CHECK ADD  CONSTRAINT [FK_following_licenses] FOREIGN KEY([user_id])
REFERENCES [licenses] ([license_id])
ON DELETE CASCADE
GO
ALTER TABLE [following] CHECK CONSTRAINT [FK_following_licenses]
GO
ALTER TABLE [glossary_mappings]  WITH CHECK ADD  CONSTRAINT [FK_glossary_mappings_glossary_terms] FOREIGN KEY([term_id])
REFERENCES [glossary_terms] ([term_id])
ON DELETE CASCADE
GO
ALTER TABLE [glossary_mappings] CHECK CONSTRAINT [FK_glossary_mappings_glossary_terms]
GO
ALTER TABLE [glossary_term_relationships]  WITH CHECK ADD  CONSTRAINT [FK_glossary_term_relationships_glossary_term_relationship_types] FOREIGN KEY([type_id])
REFERENCES [glossary_term_relationship_types] ([type_id])
ON DELETE CASCADE
GO
ALTER TABLE [glossary_term_relationships] CHECK CONSTRAINT [FK_glossary_term_relationships_glossary_term_relationship_types]
GO
ALTER TABLE [glossary_term_relationships]  WITH CHECK ADD  CONSTRAINT [FK_glossary_term_relationships_glossary_terms_destination] FOREIGN KEY([destination_term_id])
REFERENCES [glossary_terms] ([term_id])
GO
ALTER TABLE [glossary_term_relationships] CHECK CONSTRAINT [FK_glossary_term_relationships_glossary_terms_destination]
GO
ALTER TABLE [glossary_term_relationships]  WITH CHECK ADD  CONSTRAINT [FK_glossary_term_relationships_glossary_terms_source] FOREIGN KEY([source_term_id])
REFERENCES [glossary_terms] ([term_id])
GO
ALTER TABLE [glossary_term_relationships] CHECK CONSTRAINT [FK_glossary_term_relationships_glossary_terms_source]
GO
ALTER TABLE [glossary_terms]  WITH CHECK ADD  CONSTRAINT [FK_glossary_terms_database] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
ON DELETE CASCADE
GO
ALTER TABLE [glossary_terms] CHECK CONSTRAINT [FK_glossary_terms_database]
GO
ALTER TABLE [glossary_terms]  WITH CHECK ADD  CONSTRAINT [FK_glossary_terms_glossary_term_types] FOREIGN KEY([type_id])
REFERENCES [glossary_term_types] ([term_type_id])
GO
ALTER TABLE [glossary_terms] CHECK CONSTRAINT [FK_glossary_terms_glossary_term_types]
GO
ALTER TABLE [glossary_terms]  WITH CHECK ADD  CONSTRAINT [FK_glossary_terms_glossary_terms] FOREIGN KEY([parent_id])
REFERENCES [glossary_terms] ([term_id])
GO
ALTER TABLE [glossary_terms] CHECK CONSTRAINT [FK_glossary_terms_glossary_terms]
GO
ALTER TABLE [ignored_objects]  WITH CHECK ADD  CONSTRAINT [FK_ignored_objects_databases] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
ON DELETE CASCADE
GO
ALTER TABLE [ignored_objects] CHECK CONSTRAINT [FK_ignored_objects_databases]
GO
ALTER TABLE [modules]  WITH CHECK ADD  CONSTRAINT [FK_modules_database] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
ON DELETE CASCADE
GO
ALTER TABLE [modules] CHECK CONSTRAINT [FK_modules_database]
GO
ALTER TABLE [parameters]  WITH CHECK ADD  CONSTRAINT [FK_parameters_procedures] FOREIGN KEY([procedure_id])
REFERENCES [procedures] ([procedure_id])
ON DELETE CASCADE
GO
ALTER TABLE [parameters] CHECK CONSTRAINT [FK_parameters_procedures]
GO
ALTER TABLE [permissions]  WITH CHECK ADD  CONSTRAINT [FK_permissions_databases] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
ON DELETE CASCADE
GO
ALTER TABLE [permissions] CHECK CONSTRAINT [FK_permissions_databases]
GO
ALTER TABLE [permissions]  WITH CHECK ADD  CONSTRAINT [FK_permissions_licenses] FOREIGN KEY([user_id])
REFERENCES [licenses] ([license_id])
ON DELETE CASCADE
GO
ALTER TABLE [permissions] CHECK CONSTRAINT [FK_permissions_licenses]
GO
ALTER TABLE [permissions]  WITH CHECK ADD  CONSTRAINT [FK_permissions_roles] FOREIGN KEY([role_id])
REFERENCES [roles] ([role_id])
ON DELETE CASCADE
GO
ALTER TABLE [permissions] CHECK CONSTRAINT [FK_permissions_roles]
GO
ALTER TABLE [permissions]  WITH CHECK ADD  CONSTRAINT [FK_permissions_user_groups] FOREIGN KEY([user_group_id])
REFERENCES [user_groups] ([user_group_id])
ON DELETE CASCADE
GO
ALTER TABLE [permissions] CHECK CONSTRAINT [FK_permissions_user_groups]
GO
ALTER TABLE [procedures]  WITH CHECK ADD  CONSTRAINT [FK_procedures_database] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
ON DELETE CASCADE
GO
ALTER TABLE [procedures] CHECK CONSTRAINT [FK_procedures_database]
GO
ALTER TABLE [procedures_modules]  WITH CHECK ADD  CONSTRAINT [FK_procedures_modules_module] FOREIGN KEY([module_id])
REFERENCES [modules] ([module_id])
ON DELETE CASCADE
GO
ALTER TABLE [procedures_modules] CHECK CONSTRAINT [FK_procedures_modules_module]
GO
ALTER TABLE [procedures_modules]  WITH CHECK ADD  CONSTRAINT [FK_procedures_modules_procedure] FOREIGN KEY([procedure_id])
REFERENCES [procedures] ([procedure_id])
GO
ALTER TABLE [procedures_modules] CHECK CONSTRAINT [FK_procedures_modules_procedure]
GO
ALTER TABLE [role_actions]  WITH CHECK ADD  CONSTRAINT [FK_role_actions_roles] FOREIGN KEY([role_id])
REFERENCES [roles] ([role_id])
ON DELETE CASCADE
GO
ALTER TABLE [role_actions] CHECK CONSTRAINT [FK_role_actions_roles]
GO
ALTER TABLE [tables]  WITH CHECK ADD  CONSTRAINT [FK_tables_databases] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
GO
ALTER TABLE [tables] CHECK CONSTRAINT [FK_tables_databases]
GO
ALTER TABLE [tables_modules]  WITH CHECK ADD  CONSTRAINT [FK_tables_modules_module] FOREIGN KEY([module_id])
REFERENCES [modules] ([module_id])
ON DELETE CASCADE
GO
ALTER TABLE [tables_modules] CHECK CONSTRAINT [FK_tables_modules_module]
GO
ALTER TABLE [tables_modules]  WITH CHECK ADD  CONSTRAINT [FK_tables_modules_tables] FOREIGN KEY([table_id])
REFERENCES [tables] ([table_id])
GO
ALTER TABLE [tables_modules] CHECK CONSTRAINT [FK_tables_modules_tables]
GO
ALTER TABLE [tables_relations]  WITH CHECK ADD  CONSTRAINT [FK_tables_relations_table_fk] FOREIGN KEY([fk_table_id])
REFERENCES [tables] ([table_id])
GO
ALTER TABLE [tables_relations] CHECK CONSTRAINT [FK_tables_relations_table_fk]
GO
ALTER TABLE [tables_relations]  WITH CHECK ADD  CONSTRAINT [FK_tables_relations_table_pk] FOREIGN KEY([pk_table_id])
REFERENCES [tables] ([table_id])
GO
ALTER TABLE [tables_relations] CHECK CONSTRAINT [FK_tables_relations_table_pk]
GO
ALTER TABLE [tables_relations_columns]  WITH CHECK ADD  CONSTRAINT [FK_tables_relations_columns_column_fk] FOREIGN KEY([column_fk_id])
REFERENCES [columns] ([column_id])
GO
ALTER TABLE [tables_relations_columns] CHECK CONSTRAINT [FK_tables_relations_columns_column_fk]
GO
ALTER TABLE [tables_relations_columns]  WITH CHECK ADD  CONSTRAINT [FK_tables_relations_columns_column_pk] FOREIGN KEY([column_pk_id])
REFERENCES [columns] ([column_id])
GO
ALTER TABLE [tables_relations_columns] CHECK CONSTRAINT [FK_tables_relations_columns_column_pk]
GO
ALTER TABLE [tables_relations_columns]  WITH CHECK ADD  CONSTRAINT [FK_tables_relations_columns_relation] FOREIGN KEY([table_relation_id])
REFERENCES [tables_relations] ([table_relation_id])
ON DELETE CASCADE
GO
ALTER TABLE [tables_relations_columns] CHECK CONSTRAINT [FK_tables_relations_columns_relation]
GO
ALTER TABLE [tables_stats]  WITH CHECK ADD  CONSTRAINT [FK_tables_stats_tables] FOREIGN KEY([table_id])
REFERENCES [tables] ([table_id])
ON DELETE CASCADE
GO
ALTER TABLE [tables_stats] CHECK CONSTRAINT [FK_tables_stats_tables]
GO
ALTER TABLE [triggers]  WITH CHECK ADD  CONSTRAINT [FK_triggers_table] FOREIGN KEY([table_id])
REFERENCES [tables] ([table_id])
ON DELETE CASCADE
GO
ALTER TABLE [triggers] CHECK CONSTRAINT [FK_triggers_table]
GO
ALTER TABLE [unique_constraints]  WITH CHECK ADD  CONSTRAINT [FK_unique_constraints_tables] FOREIGN KEY([table_id])
REFERENCES [tables] ([table_id])
ON DELETE CASCADE
GO
ALTER TABLE [unique_constraints] CHECK CONSTRAINT [FK_unique_constraints_tables]
GO
ALTER TABLE [unique_constraints_columns]  WITH CHECK ADD  CONSTRAINT [FK_unique_constraints_columns_columns] FOREIGN KEY([column_id])
REFERENCES [columns] ([column_id])
GO
ALTER TABLE [unique_constraints_columns] CHECK CONSTRAINT [FK_unique_constraints_columns_columns]
GO
ALTER TABLE [unique_constraints_columns]  WITH CHECK ADD  CONSTRAINT [FK_unique_constraints_columns_unique_constraints] FOREIGN KEY([unique_constraint_id])
REFERENCES [unique_constraints] ([unique_constraint_id])
ON DELETE CASCADE
GO
ALTER TABLE [unique_constraints_columns] CHECK CONSTRAINT [FK_unique_constraints_columns_unique_constraints]
GO
ALTER TABLE [user_connections]  WITH CHECK ADD  CONSTRAINT [FK_user_connections_databases] FOREIGN KEY([database_id])
REFERENCES [databases] ([database_id])
GO
ALTER TABLE [user_connections] CHECK CONSTRAINT [FK_user_connections_databases]
GO
ALTER TABLE [user_connections]  WITH CHECK ADD  CONSTRAINT [FK_user_connections_licenses] FOREIGN KEY([license_id])
REFERENCES [licenses] ([license_id])
GO
ALTER TABLE [user_connections] CHECK CONSTRAINT [FK_user_connections_licenses]
GO
ALTER TABLE [users_user_groups]  WITH CHECK ADD  CONSTRAINT [FK_users_user_groups_licenses] FOREIGN KEY([user_id])
REFERENCES [licenses] ([license_id])
ON DELETE CASCADE
GO
ALTER TABLE [users_user_groups] CHECK CONSTRAINT [FK_users_user_groups_licenses]
GO
ALTER TABLE [users_user_groups]  WITH CHECK ADD  CONSTRAINT [FK_users_user_groups_user_groups] FOREIGN KEY([user_group_id])
REFERENCES [user_groups] ([user_group_id])
ON DELETE CASCADE
GO
ALTER TABLE [users_user_groups] CHECK CONSTRAINT [FK_users_user_groups_user_groups]
GO
/****** Object:  Trigger [trg_classificator_masks_Modify]    Script Date: 7/5/2022 9:26:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Szymon Karpęcki
-- Create date: 28/11/2021
-- Description: Delete modified_by from triggers
-- =============================================
CREATE TRIGGER [trg_classificator_masks_Modify]
   ON  [classificator_masks]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [classificator_masks]
     SET  
        last_modification_date = GETDATE()
     WHERE classificator_mask_id IN (SELECT DISTINCT classificator_mask_id FROM Inserted)
END
GO
ALTER TABLE [classificator_masks] ENABLE TRIGGER [trg_classificator_masks_Modify]
GO
/****** Object:  Trigger [trg_classificator_rules_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_classificator_rules_Modify]
   ON  [classificator_rules]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [classificator_rules]
     SET  
        last_modification_date = GETDATE()
     WHERE classificator_rule_id IN (SELECT DISTINCT classificator_rule_id FROM Inserted)
END
GO
ALTER TABLE [classificator_rules] ENABLE TRIGGER [trg_classificator_rules_Modify]
GO
/****** Object:  Trigger [trg_classificators_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_classificators_Modify]
   ON  [classificators]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [classificators]
     SET  
        last_modification_date = GETDATE()
     WHERE classificator_id IN (SELECT DISTINCT classificator_id FROM Inserted)
END
GO
ALTER TABLE [classificators] ENABLE TRIGGER [trg_classificators_Modify]
GO
/****** Object:  Trigger [trg_columns_change_track_insert]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert column's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_columns_change_track_insert] ON [columns]
FOR INSERT
AS
     -- skip manual objects created through Dataedo application
     -- or first documentation import to Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted]
                 WHERE [source] = 'USER'
              )
        OR EXISTS
                 (
                  SELECT TOP (1) 1
                    FROM [inserted] [i]
                         JOIN [schema_updates] [u] ON [u].[update_id] = [i].[update_id]
                                                            AND [u].[type] = 'IMPORT'
                 )
         BEGIN
             RETURN;
         END;
     INSERT INTO [columns_changes]
     ([column_id],
      [table_id],
      [database_id],
      [table_schema],
      [table_name],
      [ordinal_position],
      [column_name],
      [path],
      [parent_id], 
      [level],
      [item_type],
      [datatype],
      [data_length],
      [nullable],
      [default_value],
      [is_identity],
      [is_computed],
      [computed_formula],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [i].[column_id],
                   [i].[table_id],
                   [t].[database_id],
                   [t].[schema] AS [table_schema],
                   [t].[name] AS [table_name],
                   [i].[ordinal_position],
                   [i].[name] AS [column_name],
                   [i].[path],
                   [i].[parent_id], 
                   [i].[level],
                   [i].[item_type],
                   [i].[datatype],
                   [i].[data_length],
                   [i].[nullable],
                   [i].[default_value],
                   [i].[is_identity],
                   [i].[is_computed],
                   [i].[computed_formula],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i]
                   LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id];
GO
ALTER TABLE [columns] ENABLE TRIGGER [trg_columns_change_track_insert]
GO
/****** Object:  Trigger [trg_columns_change_track_update]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Columns - triggers
-- =============================================
-- Description:	Insert column's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_columns_change_track_update] ON [columns]
FOR UPDATE
AS
     -- check if column was updated
     IF NOT (UPDATE([datatype]) OR UPDATE([data_length])  OR UPDATE([nullable]) OR UPDATE([default_value]) OR UPDATE([is_identity]) OR UPDATE([is_computed]) OR UPDATE([computed_formula])  OR UPDATE([name]) OR UPDATE([status]) 
     OR UPDATE([parent_id]) OR UPDATE([path]) OR UPDATE([level]) OR UPDATE([item_type]))
      BEGIN
          RETURN;
     END;
     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;
     -- check if object property change 
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                 WHERE
                 --isnull(d.ordinal_position,0) <> i.ordinal_position OR
                 ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')
                 OR ISNULL([d].[nullable],'') <> ISNULL([i].[nullable],'')
                 OR ISNULL([d].[default_value],'') <> ISNULL([i].[default_value],'')
                 OR ISNULL([d].[is_identity],'') <> ISNULL([i].[is_identity],'')
                 OR ISNULL([d].[is_computed],'') <> ISNULL([i].[is_computed],'')
                 OR ISNULL([d].[computed_formula],'') <> ISNULL([i].[computed_formula],'')
                 OR ISNULL([d].[parent_id],0) <> ISNULL([i].[parent_id],0)
                 OR ISNULL([d].[path],'') <> ISNULL([i].[path],'')
                 OR ISNULL([d].[level],0) <> ISNULL([i].[level],0)
                 OR ISNULL([d].[item_type],'') <> ISNULL([i].[item_type],'')
                 OR [d].[name] <> [i].[name]
              )
         BEGIN
             UPDATE [columns_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [columns_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[column_id] = [u].[column_id]
               WHERE [valid_to] IS NULL;
             -- insert changes (before and after values) for columns - when column was updated
             INSERT INTO [columns_changes]
             ([column_id],
              [table_id],
              [database_id],
              [table_schema],
              [table_name],
              [ordinal_position],
              [column_name],
              [path],
              [parent_id], 
              [level],
              [item_type],
              [datatype],
              [data_length],
              [nullable],
              [default_value],
              [is_identity],
              [is_computed],
              [computed_formula],
              [before_parent_id],
              [before_path],
              [before_level],
              [before_item_type],
              [before_ordinal_position],
              [before_column_name],
              [before_datatype],
              [before_data_length],
              [before_nullable],
              [before_default_value],
              [before_is_identity],
              [before_is_computed],
              [before_computed_formula],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [i].[column_id],
                           [i].[table_id],
                           [t].[database_id],
                           [t].[schema] AS [table_schema],
                           [t].[name] AS [table_name],
                           [i].[ordinal_position],
                           [i].[name] AS [column_name],
                           [i].[path] AS [path],
                           [i].[parent_id] AS [parent_id], 
                           [i].[level] AS [level],
                           [i].[item_type] AS [item_type],
                           [i].[datatype],
                           [i].[data_length],
                           [i].[nullable],
                           [i].[default_value],
                           [i].[is_identity],
                           [i].[is_computed],
                           [i].[computed_formula],
                           [d].[parent_id],
                           [d].[path],
                           [d].[level],
                           [d].[item_type],
                           [d].[ordinal_position],
                           [d].[name] AS [column_name],
                           [d].[datatype],
                           [d].[data_length],
                           [d].[nullable],
                           [d].[default_value],
                           [d].[is_identity],
                           [d].[is_computed],
                           [d].[computed_formula],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [columns_changes] [c]
                                         WHERE [c].[column_id] = [d].[column_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                           LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[ordinal_position],0) <> ISNULL([i].[ordinal_position],0)
                                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')
                                 OR ISNULL([d].[nullable],'') <> ISNULL([i].[nullable],'')
                                 OR ISNULL([d].[default_value],'') <> ISNULL([i].[default_value],'')
                                 OR ISNULL([d].[is_identity],'') <> ISNULL([i].[is_identity],'')
                                 OR ISNULL([d].[is_computed],'') <> ISNULL([i].[is_computed],'')
                                 OR ISNULL([d].[computed_formula],'') <> ISNULL([i].[computed_formula],'')
                                 OR ISNULL([d].[parent_id],0) <> ISNULL([i].[parent_id],0)
                                 OR ISNULL([d].[path],'') <> ISNULL([i].[path],'')
                                 OR ISNULL([d].[level],0) <> ISNULL([i].[level],0)
                                 OR ISNULL([d].[item_type],'') <> ISNULL([i].[item_type],'')
                                 OR [d].[name] <> [i].[name]);
         END;
    BEGIN
        -- insert changes (deleted values) for columns - when column was deleted
        INSERT INTO [columns_changes]
        ([column_id],
         [table_id],
         [database_id],
         [table_schema],
         [table_name],
         [ordinal_position],
         [column_name],
         [path],
         [parent_id], 
         [level],
         [item_type],
         [datatype],
         [data_length],
         [nullable],
         [default_value],
         [is_identity],
         [is_computed],
         [computed_formula],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [d].[column_id],
                      [d].[table_id],
                      [t].[database_id],
                      [t].[schema] AS [table_schema],
                      [t].[name] AS [table_name],
                      [d].[ordinal_position],
                      [d].[name] AS [column_name],
                      [d].[path] AS [path],
                      [d].[parent_id] AS [parent_id], 
                      [d].[level] AS [level],
                      [d].[item_type] AS [item_type],
                      [d].[datatype],
                      [d].[data_length],
                      [d].[nullable],
                      [d].[default_value],
                      [d].[is_identity],
                      [d].[is_computed],
                      [d].[computed_formula],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                      LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';
        -- insert changes (updated values) for columns - when column was restored
        INSERT INTO [columns_changes]
        ([column_id],
         [table_id],
         [database_id],
         [table_schema],
         [table_name],
         [ordinal_position],
         [column_name],
         [path],
         [parent_id], 
         [level],
         [item_type],
         [datatype],
         [data_length],
         [nullable],
         [default_value],
         [is_identity],
         [is_computed],
         [computed_formula],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [i].[column_id],
                      [i].[table_id],
                      [t].[database_id],
                      [t].[schema] AS [table_schema],
                      [t].[name] AS [table_name],
                      [i].[ordinal_position],
                      [i].[name] AS [column_name],
                      [i].[path] AS [path],
                      [i].[parent_id] AS [parent_id], 
                      [i].[level] AS [level],
                      [i].[item_type] AS [item_type],
                      [i].[datatype],
                      [i].[data_length],
                      [i].[nullable],
                      [i].[default_value],
                      [i].[is_identity],
                      [i].[is_computed],
                      [i].[computed_formula],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[column_id] = [d].[column_id]
                      LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';
    END;
GO
ALTER TABLE [columns] ENABLE TRIGGER [trg_columns_change_track_update]
GO
/****** Object:  Trigger [trg_columns_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_columns_Modify]
   ON  [columns]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [columns]
     SET  
        last_modification_date = GETDATE()
     WHERE column_id IN (SELECT DISTINCT column_id FROM Inserted)
END
GO
ALTER TABLE [columns] ENABLE TRIGGER [trg_columns_Modify]
GO
/****** Object:  Trigger [trg_columns_changes_description_modified]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_columns_changes_description_modified]
	ON [columns_changes]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [columns_changes] 
		SET [description_date] = GETDATE()
		WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
	END
END
GO
ALTER TABLE [columns_changes] ENABLE TRIGGER [trg_columns_changes_description_modified]
GO
/****** Object:  Trigger [trg_custom_fields_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_custom_fields_Modify]
   ON  [custom_fields]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [custom_fields]
     SET  
        last_modification_date = GETDATE()
     WHERE custom_field_id IN (SELECT DISTINCT custom_field_id FROM Inserted)
END
GO
ALTER TABLE [custom_fields] ENABLE TRIGGER [trg_custom_fields_Modify]
GO
/****** Object:  Trigger [trg_data_columns_flows_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
        CREATE TRIGGER [trg_data_columns_flows_Modify]
           ON  [data_columns_flows]
           AFTER UPDATE
        AS 
        BEGIN
             UPDATE [data_columns_flows]
             SET  
                 [last_modification_date] = GETDATE()
             WHERE [data_columns_flow_id] IN (SELECT DISTINCT [data_columns_flow_id] FROM Inserted)
        END
GO
ALTER TABLE [data_columns_flows] ENABLE TRIGGER [trg_data_columns_flows_Modify]
GO
/****** Object:  Trigger [trg_data_flow_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
        CREATE TRIGGER [trg_data_flow_Modify]
           ON  [data_flows]
           AFTER INSERT,UPDATE
        AS 
        BEGIN
             UPDATE [data_flows]
             SET  
                 [last_modification_date] = GETDATE()
             WHERE [flow_id] IN (SELECT DISTINCT [flow_id] FROM Inserted)
        END
GO
ALTER TABLE [data_flows] ENABLE TRIGGER [trg_data_flow_Modify]
GO
/****** Object:  Trigger [trg_data_flows_delete_flow]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_data_flows_delete_flow]
		ON [data_flows]
		FOR DELETE
	AS
	BEGIN
		SET NOCOUNT ON
		DELETE FROM [data_columns_flows] WHERE [inflow_id] IN (SELECT flow_id FROM DELETED)
		DELETE FROM [data_columns_flows] WHERE [outflow_id] IN (SELECT flow_id FROM DELETED)
	END
GO
ALTER TABLE [data_flows] ENABLE TRIGGER [trg_data_flows_delete_flow]
GO
/****** Object:  Trigger [trg_data_processes_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
        CREATE TRIGGER [trg_data_processes_Modify]
           ON  [data_processes]
           AFTER INSERT,UPDATE
        AS 
        BEGIN
             UPDATE [data_processes]
             SET  
                 [last_modification_date] = GETDATE()
             WHERE [process_id] IN (SELECT DISTINCT [process_id] FROM Inserted)
        END
GO
ALTER TABLE [data_processes] ENABLE TRIGGER [trg_data_processes_Modify]
GO
/****** Object:  Trigger [trg_databases_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_databases_Modify]
   ON  [databases] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [databases]
     SET  
        last_modification_date = GETDATE()
     WHERE database_id IN (SELECT DISTINCT database_id FROM Inserted)
END
GO
ALTER TABLE [databases] ENABLE TRIGGER [trg_databases_Modify]
GO
/****** Object:  Trigger [trg_dependencies_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_dependencies_Modify]
   ON  [dependencies]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [dependencies]
     SET  
        last_modification_date = GETDATE()
     WHERE dependency_id IN (SELECT DISTINCT dependency_id FROM Inserted)
END
GO
ALTER TABLE [dependencies] ENABLE TRIGGER [trg_dependencies_Modify]
GO
/****** Object:  Trigger [trg_dependencies_descriptions_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_dependencies_descriptions_Modify]
   ON  [dependencies_descriptions]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [dependencies_descriptions]
     SET  
        last_modification_date = GETDATE()
     WHERE dependency_descriptions_id IN (SELECT DISTINCT dependency_descriptions_id FROM Inserted)
END
GO
ALTER TABLE [dependencies_descriptions] ENABLE TRIGGER [trg_dependencies_descriptions_Modify]
GO
/****** Object:  Trigger [trg_documentation_custom_fields_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_documentation_custom_fields_Modify]
   ON  [documentation_custom_fields]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [documentation_custom_fields]
     SET  
        last_modification_date = GETDATE()
     WHERE documentation_custom_field_id IN (SELECT DISTINCT documentation_custom_field_id FROM Inserted)
END
GO
ALTER TABLE [documentation_custom_fields] ENABLE TRIGGER [trg_documentation_custom_fields_Modify]
GO
/****** Object:  Trigger [trg_erd_links_descriptions_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_erd_links_descriptions_Modify]
   ON  [erd_links]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [erd_links]
     SET  
        last_modification_date = GETDATE()
     WHERE link_id IN (SELECT DISTINCT link_id FROM Inserted)
END
GO
ALTER TABLE [erd_links] ENABLE TRIGGER [trg_erd_links_descriptions_Modify]
GO
/****** Object:  Trigger [trg_erd_nodes_descriptions_Modify]    Script Date: 7/5/2022 9:26:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_erd_nodes_descriptions_Modify]
   ON  [erd_nodes]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [erd_nodes]
     SET  
        last_modification_date = GETDATE()
     WHERE node_id IN (SELECT DISTINCT node_id FROM Inserted)
END
GO
ALTER TABLE [erd_nodes] ENABLE TRIGGER [trg_erd_nodes_descriptions_Modify]
GO
/****** Object:  Trigger [trg_erd_post_its_descriptions_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_erd_post_its_descriptions_Modify]
   ON  [erd_post_its]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [erd_post_its]
     SET  
        last_modification_date = GETDATE()
     WHERE post_it_id IN (SELECT DISTINCT post_it_id FROM Inserted)
END
GO
ALTER TABLE [erd_post_its] ENABLE TRIGGER [trg_erd_post_its_descriptions_Modify]
GO
/****** Object:  Trigger [trg_feedback_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_feedback_Modify]
   ON  [feedback]
   AFTER UPDATE
AS 
BEGIN
     UPDATE [feedback]
     SET 
        [last_modification_date] = GETDATE()
     WHERE [feedback_id] IN (SELECT DISTINCT [feedback_id] FROM Inserted)
END
GO
ALTER TABLE [feedback] ENABLE TRIGGER [trg_feedback_Modify]
GO
/****** Object:  Trigger [trg_feedback_comments_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_feedback_comments_Modify]
   ON  [feedback_comments]
   AFTER UPDATE
AS 
BEGIN
     UPDATE [feedback_comments]
     SET 
         [last_modification_date] = GETDATE()
     WHERE [comment_id] IN (SELECT DISTINCT [comment_id] FROM Inserted)
END
GO
ALTER TABLE [feedback_comments] ENABLE TRIGGER [trg_feedback_comments_Modify]
GO
/****** Object:  Trigger [trg_feedback_links_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_feedback_links_Modify]
   ON  [feedback_links]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [feedback_links]
     SET 
         [last_modification_date] = GETDATE()
     WHERE [link_id] IN (SELECT DISTINCT [link_id] FROM Inserted)
END
GO
ALTER TABLE [feedback_links] ENABLE TRIGGER [trg_feedback_links_Modify]
GO
/****** Object:  Trigger [trg_following_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
        CREATE TRIGGER [trg_following_Modify]
           ON  [following] 
           AFTER INSERT,UPDATE
        AS 
        BEGIN
             UPDATE [following]
             SET  
                 [last_modification_date] = GETDATE()
             WHERE [following_id] IN (SELECT DISTINCT [following_id] FROM Inserted)
        END
GO
ALTER TABLE [following] ENABLE TRIGGER [trg_following_Modify]
GO
/****** Object:  Trigger [trg_glossary_mappings_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_glossary_mappings_Modify] 
    ON [glossary_mappings]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_mappings]
          SET
              [last_modification_date] = GETDATE()
          WHERE [mapping_id] IN (SELECT DISTINCT [mapping_id] FROM [Inserted])
    END;
GO
ALTER TABLE [glossary_mappings] ENABLE TRIGGER [trg_glossary_mappings_Modify]
GO
/****** Object:  Trigger [trg_glossary_term_relationship_types_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_glossary_term_relationship_types_Modify] 
    ON [glossary_term_relationship_types]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_term_relationship_types]
          SET
              [last_modification_date] = GETDATE()
          WHERE [type_id] IN(SELECT DISTINCT [type_id] FROM [Inserted]);
    END;
GO
ALTER TABLE [glossary_term_relationship_types] ENABLE TRIGGER [trg_glossary_term_relationship_types_Modify]
GO
/****** Object:  Trigger [trg_glossary_term_relationships_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_glossary_term_relationships_Modify] 
    ON [glossary_term_relationships]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_term_relationships]
          SET
              [last_modification_date] = GETDATE()
          WHERE [relationship_id] IN ( SELECT DISTINCT [relationship_id] FROM [Inserted]);
    END;
GO
ALTER TABLE [glossary_term_relationships] ENABLE TRIGGER [trg_glossary_term_relationships_Modify]
GO
/****** Object:  Trigger [trg_glossary_term_types_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_glossary_term_types_Modify] 
    ON [glossary_term_types]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_term_types]
          SET
              [last_modification_date] = GETDATE()
          WHERE [term_type_id] IN (SELECT DISTINCT [term_type_id] FROM [Inserted]);
    END;
GO
ALTER TABLE [glossary_term_types] ENABLE TRIGGER [trg_glossary_term_types_Modify]
GO
/****** Object:  Trigger [trg_glossary_terms_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_glossary_terms_Modify] 
    ON [glossary_terms]
    AFTER INSERT,UPDATE
AS
    BEGIN
        UPDATE [glossary_terms]
          SET
              [last_modification_date] = GETDATE()
          WHERE [term_id] IN (SELECT DISTINCT [term_id] FROM [Inserted]);
    END;
GO
ALTER TABLE [glossary_terms] ENABLE TRIGGER [trg_glossary_terms_Modify]
GO
/****** Object:  Trigger [trg_ignored_objects_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_ignored_objects_Modify]
   ON [ignored_objects]
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [ignored_objects]
     SET  
         last_modification_date = GETDATE()
     WHERE ignored_object_id IN (SELECT DISTINCT ignored_object_id FROM Inserted)
END
GO
ALTER TABLE [ignored_objects] ENABLE TRIGGER [trg_ignored_objects_Modify]
GO
/****** Object:  Trigger [trg_licenses_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_licenses_Modify]
   ON [licenses] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [licenses]
     SET  
         last_modification_date = GETDATE()
     WHERE license_id IN (SELECT DISTINCT license_id FROM Inserted)
END
GO
ALTER TABLE [licenses] ENABLE TRIGGER [trg_licenses_Modify]
GO
/****** Object:  Trigger [trg_modules_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_modules_Modify]
   ON [modules]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [modules]
     SET  
         last_modification_date = GETDATE()
     WHERE module_id IN (SELECT DISTINCT module_id FROM Inserted)
END
GO
ALTER TABLE [modules] ENABLE TRIGGER [trg_modules_Modify]
GO
/****** Object:  Trigger [trg_parameters_change_track_insert]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert parameter's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_parameters_change_track_insert] ON [parameters]
FOR INSERT
AS
     INSERT INTO [parameters_changes]
     ([database_id],
      [parameter_id],
      [procedure_id],
      [ordinal_position],
      [parameter_mode],
      [name],
      [datatype],
      [data_length],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [p].[database_id],
                   [i].[parameter_id],
                   [i].[procedure_id],
                   [i].[ordinal_position],
                   [i].[parameter_mode],
                   [i].[name],
                   [i].[datatype],
                   [i].[data_length],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i]
                   JOIN [procedures] [p] ON [i].[procedure_id] = [p].[procedure_id]
              WHERE NOT EXISTS
                              (
                               SELECT 1
                                 FROM [schema_updates] [u]
                                 WHERE [u].[update_id] = [i].[update_id]
                                       AND [u].[type] = 'IMPORT'
                              );
GO
ALTER TABLE [parameters] ENABLE TRIGGER [trg_parameters_change_track_insert]
GO
/****** Object:  Trigger [trg_parameters_change_track_update]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert parameter's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_parameters_change_track_update] ON [parameters]
FOR UPDATE
AS
     -- check if parameter was updated
     IF NOT (UPDATE([ordinal_position]) OR UPDATE([parameter_mode])  OR UPDATE([datatype]) OR UPDATE([data_length]) OR UPDATE([status]))  
      BEGIN
          RETURN;
     END;
     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;
     -- check if object property change
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                 WHERE
                 --isnull(d.[name],'') <> i.[name] or
                 ISNULL([d].[ordinal_position],'') <> ISNULL([i].[ordinal_position],'')
                 OR ISNULL([d].[parameter_mode],'') <> ISNULL([i].[parameter_mode],'')
                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],'')
              )
         BEGIN
             UPDATE [parameters_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [parameters_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[procedure_id] = [u].[procedure_id]
               WHERE [valid_to] IS NULL;
             -- insert changes (before and after values) for parameter - when parameter was updated
             INSERT INTO [parameters_changes]
             ([database_id],
              [parameter_id],
              [procedure_id],
              [ordinal_position],
              [parameter_mode],
              [name],
              [datatype],
              [data_length],
              [before_ordinal_position],
              [before_parameter_mode],
              [before_datatype],
              [before_data_length],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [p].[database_id],
                           [i].[parameter_id],
                           [i].[procedure_id],
                           [i].[ordinal_position],
                           [i].[parameter_mode],
                           [i].[name],
                           [i].[datatype],
                           [i].[data_length],
                           [d].[ordinal_position],
                           [d].[parameter_mode],
                           [d].[datatype],
                           [d].[data_length],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                           JOIN [procedures] [p] ON [d].[procedure_id] = [p].[procedure_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [procedures_changes] [c]
                                         WHERE [c].[procedure_id] = [d].[procedure_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[ordinal_position],'') <> ISNULL([i].[ordinal_position],'')
                                 OR ISNULL([d].[parameter_mode],'') <> ISNULL([i].[parameter_mode],'')
                                 OR ISNULL([d].[datatype],'') <> ISNULL([i].[datatype],'')
                                 OR ISNULL([d].[data_length],'') <> ISNULL([i].[data_length],''));
         END;
    BEGIN
        -- insert changes (deleted values) for parameter - when parameter was deleted
        INSERT INTO [parameters_changes]
        ([database_id],
         [parameter_id],
         [procedure_id],
         [ordinal_position],
         [parameter_mode],
         [name],
         [datatype],
         [data_length],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [p].[database_id],
                      [d].[parameter_id],
                      [d].[procedure_id],
                      [d].[ordinal_position],
                      [d].[parameter_mode],
                      [d].[name],
                      [d].[datatype],
                      [d].[data_length],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                      JOIN [procedures] [p] ON [d].[procedure_id] = [p].[procedure_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';
        -- insert changes (updated values) for parameter - when parameter was restored
        INSERT INTO [parameters_changes]
        ([database_id],
         [parameter_id],
         [procedure_id],
         [ordinal_position],
         [parameter_mode],
         [name],
         [datatype],
         [data_length],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [p].[database_id],
                      [i].[parameter_id],
                      [i].[procedure_id],
                      [i].[ordinal_position],
                      [i].[parameter_mode],
                      [i].[name],
                      [i].[datatype],
                      [i].[data_length],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[parameter_id] = [d].[parameter_id]
                      JOIN [procedures] [p] ON [d].[procedure_id] = [p].[procedure_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';
    END;
GO
ALTER TABLE [parameters] ENABLE TRIGGER [trg_parameters_change_track_update]
GO
/****** Object:  Trigger [trg_parameters_Modify]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_parameters_Modify]
   ON  [parameters]
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [parameters]
     SET  
         last_modification_date = GETDATE()
     WHERE parameter_id IN (SELECT DISTINCT parameter_id FROM Inserted)
END
GO
ALTER TABLE [parameters] ENABLE TRIGGER [trg_parameters_Modify]
GO
/****** Object:  Trigger [trg_parameters_changes_description_modified]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_parameters_changes_description_modified]
	ON [parameters_changes]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [parameters_changes] 
		SET [description_date] = GETDATE()
		WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
	END
END
ALTER TABLE [parameters_changes] ENABLE TRIGGER [trg_parameters_changes_description_modified]
GO
ALTER TABLE [parameters_changes] ENABLE TRIGGER [trg_parameters_changes_description_modified]
GO
/****** Object:  Trigger [trg_procedures_change_track_insert]    Script Date: 7/5/2022 9:26:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert procedure's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_procedures_change_track_insert] ON [procedures]
FOR INSERT
AS
     -- skip manual objects created through Dataedo application
     -- or first documentation import to Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted]
                 WHERE [source] = 'USER'
              )
        OR EXISTS
                 (
                  SELECT TOP (1) 1
                    FROM [inserted] [i]
                         JOIN [schema_updates] [u] ON [u].[update_id] = [i].[update_id]
                                                            AND [u].[type] = 'IMPORT'
                 )
         BEGIN
             RETURN;
         END;
     INSERT INTO [procedures_changes]
     ([database_id],
      [procedure_id],
      [schema],
      [name],
      [object_type],
      [definition],
      [function_type],
      [subtype],
      [operation],
      [dbms_last_modification_date],
      [valid_from],
      [update_id]
     )
            SELECT [i].[database_id],
                   [i].[procedure_id],
                   [i].[schema],
                   [i].[name],
                   [i].[object_type],
                   [i].[definition],
                   [i].[function_type],
                   [i].[subtype],
                   'ADDED',
                   GETDATE(),
                   [i].[dbms_last_modification_date],
                   [i].[update_id]
              FROM [inserted] [i];
GO
ALTER TABLE [procedures] ENABLE TRIGGER [trg_procedures_change_track_insert]
GO
/****** Object:  Trigger [trg_procedures_change_track_update]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert procedure's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_procedures_change_track_update] ON [procedures]
FOR UPDATE
AS
     -- check if procedure was updated
     IF NOT (UPDATE([schema]) OR UPDATE([name])  OR UPDATE([object_type]) OR UPDATE([definition]) OR UPDATE([function_type]) OR UPDATE([subtype]) OR UPDATE([dbms_last_modification_date]) OR UPDATE([status]))  
      BEGIN
          RETURN;
     END;
     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;
     -- check if object property change 
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                 WHERE ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                       OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                       OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                       OR ISNULL([d].[function_type],'') <> ISNULL([i].[function_type],'')
                       OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')
                       OR ISNULL([d].[dbms_last_modification_date],'') <> ISNULL([i].[dbms_last_modification_date],'')
              )
         BEGIN
             UPDATE [procedures_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [procedures_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[procedure_id] = [u].[procedure_id]
               WHERE [valid_to] IS NULL;
             -- insert changes (before and after values) for procedure - when procedure was updated
             INSERT INTO [procedures_changes]
             ([database_id],
              [procedure_id],
              [schema],
              [name],
              [object_type],
              [definition],
              [function_type],
              [subtype],
              [before_object_type],
              [before_definition],
              [before_function_type],
              [before_subtype],
              [operation],
              [dbms_last_modification_date],
              [valid_from],
              [update_id]
             )
                    SELECT [i].[database_id],
                           [i].[procedure_id],
                           [i].[schema],
                           [i].[name],
                           [i].[object_type],
                           [i].[definition],
                           [i].[function_type],
                           [i].[subtype],
                           [d].[object_type],
                           [d].[definition],
                           [d].[function_type],
                           [d].[subtype],
                           'UPDATED',
                           [i].[dbms_last_modification_date],
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [procedures_changes] [c]
                                         WHERE [c].[procedure_id] = [d].[procedure_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                                 OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                                 OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                                 OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                                 OR ISNULL([d].[function_type],'') <> ISNULL([i].[function_type],'')
                                 OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],''));
         END;
    BEGIN
        -- insert changes (deleted values) for procedure - when procedure was deleted
        INSERT INTO [procedures_changes]
        ([database_id],
         [procedure_id],
         [schema],
         [name],
         [object_type],
         [definition],
         [function_type],
         [subtype],
         [operation],
         [dbms_last_modification_date],
         [valid_to],
         [update_id]
        )
               SELECT [d].[database_id],
                      [d].[procedure_id],
                      [d].[schema],
                      [d].[name],
                      [d].[object_type],
                      [d].[definition],
                      [d].[function_type],
                      [d].[subtype],
                      'DELETED',
                      [i].[dbms_last_modification_date],
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';
        --insert changes (updated values) for procedure - when procedure was restored
        INSERT INTO [procedures_changes]
        ([database_id],
         [procedure_id],
         [schema],
         [name],
         [object_type],
         [definition],
         [function_type],
         [subtype],
         [operation],
         [dbms_last_modification_date],
         [valid_from],
         [update_id]
        )
               SELECT [i].[database_id],
                      [i].[procedure_id],
                      [i].[schema],
                      [i].[name],
                      [i].[object_type],
                      [i].[definition],
                      [i].[function_type],
                      [i].[subtype],
                      'ADDED',
                      [i].[dbms_last_modification_date],
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[procedure_id] = [d].[procedure_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';
    END;
GO
ALTER TABLE [procedures] ENABLE TRIGGER [trg_procedures_change_track_update]
GO
/****** Object:  Trigger [trg_procedures_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_procedures_Modify]
   ON  [procedures] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [procedures]
     SET 
        last_modification_date = GETDATE()
     WHERE procedure_id IN (SELECT DISTINCT procedure_id FROM Inserted)
END
GO
ALTER TABLE [procedures] ENABLE TRIGGER [trg_procedures_Modify]
GO
/****** Object:  Trigger [trg_procedures_changes_description_modified]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_procedures_changes_description_modified]
	ON [procedures_changes]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [procedures_changes] 
		SET [description_date] = GETDATE()
		WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
	END
END
GO
ALTER TABLE [procedures_changes] ENABLE TRIGGER [trg_procedures_changes_description_modified]
GO
/****** Object:  Trigger [trg_procedures_modules_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_procedures_modules_Modify]
   ON  [procedures_modules]
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [procedures_modules]
     SET  
        last_modification_date = GETDATE()
     WHERE procedure_module_id IN (SELECT DISTINCT procedure_module_id FROM Inserted)
END
GO
ALTER TABLE [procedures_modules] ENABLE TRIGGER [trg_procedures_modules_Modify]
GO
/****** Object:  Trigger [trg_schema_updates_description_modified]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_schema_updates_description_modified]
	ON [schema_updates]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [schema_updates] 
		SET [description_date] = GETDATE()
		WHERE [update_id] in (SELECT DISTINCT [update_id] FROM Inserted)
	END
END
GO
ALTER TABLE [schema_updates] ENABLE TRIGGER [trg_schema_updates_description_modified]
GO
/****** Object:  Trigger [trg_tables_change_track_insert]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert table's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_tables_change_track_insert] ON [tables]
FOR INSERT
AS
     -- skip manual objects created through Dataedo application
     -- or first documentation import to Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
                 WHERE [source] = 'USER'
              )
        OR EXISTS
                 (
                  SELECT TOP (1) 1
                    FROM [inserted] [i]
                         JOIN [schema_updates] [u] ON [u].[update_id] = [i].[update_id]
                                                            AND [u].[type] = 'IMPORT'
                 )
         BEGIN
             RETURN;
         END;
     INSERT INTO [tables_changes]
     ([table_id],
      [database_id],
      [schema],
      [name],
      [object_type],
      [subtype],
      [definition],
      [dbms_last_modification_date],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [i].[table_id],
                   [i].[database_id],
                   [i].[schema],
                   [i].[name],
                   [i].[object_type],
                   [i].[subtype],
                   [i].[definition],
                   [i].[dbms_last_modification_date],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i];
GO
ALTER TABLE [tables] ENABLE TRIGGER [trg_tables_change_track_insert]
GO
/****** Object:  Trigger [trg_tables_change_track_update]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert table's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_tables_change_track_update] ON [tables]
FOR UPDATE
AS
     -- check if table was updated
     IF NOT (UPDATE([schema]) OR UPDATE([name])  OR UPDATE([object_type]) OR UPDATE([subtype]) OR UPDATE([definition]) OR UPDATE([dbms_last_modification_date]) OR UPDATE([status]))  
      BEGIN
          RETURN;
     END;
     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;
     -- check if object property change 
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                 WHERE ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                       OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                       OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')
                       OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                       OR ISNULL([d].[dbms_last_modification_date],'') <> ISNULL([i].[dbms_last_modification_date],'')
              )
         BEGIN
             UPDATE [tables_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [tables_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[table_id] = [u].[table_id]
               WHERE [valid_to] IS NULL;
             -- insert changes (before and after values) for table - when table was updated
             INSERT INTO [tables_changes]
             ([table_id],
              [database_id],
              [schema],
              [name],
              [object_type],
              [subtype],
              [definition],
              [before_schema],
              [before_name],
              [before_object_type],
              [before_subtype],
              [before_definition],
              [dbms_last_modification_date],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [i].[table_id],
                           [i].[database_id],
                           [i].[schema],
                           [i].[name],
                           [i].[object_type],
                           [i].[subtype],
                           [i].[definition],
                           [d].[schema],
                           [d].[name],
                           [d].[object_type],
                           [d].[subtype],
                           [d].[definition],
                           [i].[dbms_last_modification_date],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [tables_changes] [c]
                                         WHERE [c].[table_id] = [d].[table_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            -- and isnull(d.last_modification_date,'') <> i.last_modification_date
                            AND (ISNULL([d].[schema],'') <> ISNULL([i].[schema],'')
                                 OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                                 OR ISNULL([d].[object_type],'') <> ISNULL([i].[object_type],'')
                                 OR ISNULL([d].[subtype],'') <> ISNULL([i].[subtype],'')
                                 OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                                 OR ISNULL([d].[dbms_last_modification_date],'') <> ISNULL([i].[dbms_last_modification_date],''));
         END;
    BEGIN
        -- insert changes (deleted values) for table - when table was deleted
        INSERT INTO [tables_changes]
        ([table_id],
         [database_id],
         [schema],
         [name],
         [object_type],
         [subtype],
         [definition],
         [dbms_last_modification_date],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [d].[table_id],
                      [d].[database_id],
                      [d].[schema],
                      [d].[name],
                      [d].[object_type],
                      [d].[subtype],
                      [d].[definition],
                      [i].[dbms_last_modification_date],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';
        -- insert changes (updated values) for table - when table was restored
        INSERT INTO [tables_changes]
        ([table_id],
         [database_id],
         [schema],
         [name],
         [object_type],
         [subtype],
         [definition],
         [dbms_last_modification_date],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [i].[table_id],
                      [i].[database_id],
                      [i].[schema],
                      [i].[name],
                      [i].[object_type],
                      [i].[subtype],
                      [i].[definition],
                      [i].[dbms_last_modification_date],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[table_id] = [d].[table_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';
    END;
GO
ALTER TABLE [tables] ENABLE TRIGGER [trg_tables_change_track_update]
GO
/****** Object:  Trigger [trg_tables_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_tables_Modify]
   ON  [tables] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [tables]
     SET 
        last_modification_date = GETDATE()
     WHERE table_id IN (SELECT DISTINCT table_id FROM Inserted)
END
GO
ALTER TABLE [tables] ENABLE TRIGGER [trg_tables_Modify]
GO
/****** Object:  Trigger [trg_tables_changes_description_modified]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_tables_changes_description_modified]
	ON [tables_changes]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [tables_changes] 
		SET [description_date] = GETDATE()
		WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
	END
END
GO
ALTER TABLE [tables_changes] ENABLE TRIGGER [trg_tables_changes_description_modified]
GO
/****** Object:  Trigger [trg_tables_modules_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_tables_modules_Modify]
   ON  [tables_modules] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [tables_modules]
     SET 
        last_modification_date = GETDATE()
     WHERE table_module_id IN (SELECT DISTINCT table_module_id FROM Inserted)
END
GO
ALTER TABLE [tables_modules] ENABLE TRIGGER [trg_tables_modules_Modify]
GO
/****** Object:  Trigger [trg_tables_relations_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_tables_relations_Modify]
   ON  [tables_relations] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [tables_relations]
     SET 
        last_modification_date = GETDATE()
     WHERE table_relation_id IN (SELECT DISTINCT table_relation_id FROM Inserted)
END
GO
ALTER TABLE [tables_relations] ENABLE TRIGGER [trg_tables_relations_Modify]
GO
/****** Object:  Trigger [trg_tables_relations_changes_description_modified]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_tables_relations_changes_description_modified]
	ON [tables_relations_changes]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [tables_relations_changes] 
		SET [description_date] = GETDATE()
		WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
	END
END
GO
ALTER TABLE [tables_relations_changes] ENABLE TRIGGER [trg_tables_relations_changes_description_modified]
GO
/****** Object:  Trigger [trg_tables_relations_cols_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_tables_relations_cols_Modify]
   ON  [tables_relations_columns] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [tables_relations_columns]
     SET 
        last_modification_date = GETDATE()
     WHERE table_relation_column_id IN (SELECT DISTINCT table_relation_column_id FROM Inserted)
END
GO
ALTER TABLE [tables_relations_columns] ENABLE TRIGGER [trg_tables_relations_cols_Modify]
GO
/****** Object:  Trigger [trg_triggers_change_track_insert]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_triggers_change_track_insert] ON [triggers]
FOR INSERT
AS
     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;
     INSERT INTO [triggers_changes]
     ([database_id],
      [trigger_id],
      [table_id],
      [name],
      [before],
      [after],
      [instead_of],
      [on_insert],
      [on_update],
      [on_delete],
      [disabled],
      [definition],
      [type],
      [operation],
      [valid_from],
      [update_id]
     )
            SELECT [t].[database_id],
                   [i].[trigger_id],
                   [i].[table_id],
                   [i].[name],
                   [i].[before],
                   [i].[after],
                   [i].[instead_of],
                   [i].[on_insert],
                   [i].[on_update],
                   [i].[on_delete],
                   [i].[disabled],
                   [i].[definition],
                   [i].[type],
                   'ADDED',
                   GETDATE(),
                   [i].[update_id]
              FROM [inserted] [i]
                   LEFT OUTER JOIN [tables] [t] ON [i].[table_id] = [t].[table_id]
              WHERE NOT EXISTS
                              (
                               SELECT 1
                                 FROM [schema_updates] [u]
                                 WHERE [u].[update_id] = [i].[update_id]
                                       AND [u].[type] = 'IMPORT'
                              );
GO
ALTER TABLE [triggers] ENABLE TRIGGER [trg_triggers_change_track_insert]
GO
/****** Object:  Trigger [trg_triggers_change_track_update]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Insert trigger's changes to schema change tracking tables
-- =============================================
CREATE TRIGGER [trg_triggers_change_track_update] ON [triggers]
FOR UPDATE
AS
     -- check if trigger was updated
     IF NOT (UPDATE([table_id]) OR UPDATE([name]) OR UPDATE([before]) OR UPDATE([after]) OR UPDATE([instead_of]) OR UPDATE([on_insert]) OR UPDATE([on_update])  OR UPDATE([on_delete]) 
               OR UPDATE([disabled]) OR UPDATE([definition]) OR UPDATE([type]) OR UPDATE([status]))  
      BEGIN
          RETURN;
     END;
     -- skip manual objects created through Dataedo application
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [INSERTED]
                 WHERE [source] = 'USER'
              )
         BEGIN
             RETURN;
         END;
     -- check if object property change 
     IF EXISTS
              (
               SELECT TOP (1) 1
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                 WHERE ISNULL([d].[table_id],'') <> ISNULL([i].[table_id],'')
                       OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                       OR ISNULL([d].[before],'') <> ISNULL([i].[before],'')
                       OR ISNULL([d].[after],'') <> ISNULL([i].[after],'')
                       OR ISNULL([d].[instead_of],'') <> ISNULL([i].[instead_of],'')
                       OR ISNULL([d].[on_insert],'') <> ISNULL([i].[on_insert],'')
                       OR ISNULL([d].[on_update],'') <> ISNULL([i].[on_update],'')
                       OR ISNULL([d].[on_delete],'') <> ISNULL([i].[on_delete],'')
                       OR ISNULL([d].[disabled],'') <> ISNULL([i].[disabled],'')
                       OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                       OR ISNULL([d].[type],'') <> ISNULL([i].[type],'')
              )
         BEGIN
             UPDATE [triggers_changes]
               SET
                   [valid_to] = GETDATE()
               FROM [triggers_changes] [c]
                    INNER JOIN [inserted] [u] ON [c].[table_id] = [u].[table_id]
               WHERE [valid_to] IS NULL;
             -- insert changes (before and after values) for trigger - when trigger was updated
             INSERT INTO [triggers_changes]
             ([database_id],
              [trigger_id],
              [table_id],
              [name],
              [before],
              [after],
              [instead_of],
              [on_insert],
              [on_update],
              [on_delete],
              [disabled],
              [definition],
              [before_before],
              [before_after],
              [before_instead_of],
              [before_on_insert],
              [before_on_update],
              [before_on_delete],
              [before_disabled],
              [before_definition],
              [type],
              [operation],
              [valid_from],
              [update_id]
             )
                    SELECT [t].[database_id],
                           [i].[trigger_id],
                           [i].[table_id],
                           [i].[name],
                           [i].[before],
                           [i].[after],
                           [i].[instead_of],
                           [i].[on_insert],
                           [i].[on_update],
                           [i].[on_delete],
                           [i].[disabled],
                           [i].[definition],
                           [d].[before],
                           [d].[after],
                           [d].[instead_of],
                           [d].[on_insert],
                           [d].[on_update],
                           [d].[on_delete],
                           [d].[disabled],
                           [d].[definition],
                           [d].[type],
                           'UPDATED',
                           [cc].[valid_to],
                           [i].[update_id]
                      FROM [inserted] [i]
                           INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                           LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]
                           OUTER APPLY
                                      (
                                       SELECT MAX([valid_to]) AS [valid_to]
                                         FROM [triggers_changes] [c]
                                         WHERE [c].[trigger_id] = [d].[trigger_id]
                                               AND [c].[valid_to] IS NOT NULL
                                      ) [cc]
                      WHERE [i].[status] = 'A'
                            AND [d].[status] = 'A'
                            AND (ISNULL([d].[table_id],'') <> ISNULL([i].[table_id],'')
                                 OR ISNULL([d].[name],'') <> ISNULL([i].[name],'')
                                 OR ISNULL([d].[before],'') <> ISNULL([i].[before],'')
                                 OR ISNULL([d].[after],'') <> ISNULL([i].[after],'')
                                 OR ISNULL([d].[instead_of],'') <> ISNULL([i].[instead_of],'')
                                 OR ISNULL([d].[on_insert],'') <> ISNULL([i].[on_insert],'')
                                 OR ISNULL([d].[on_update],'') <> ISNULL([i].[on_update],'')
                                 OR ISNULL([d].[on_delete],'') <> ISNULL([i].[on_delete],'')
                                 OR ISNULL([d].[disabled],'') <> ISNULL([i].[disabled],'')
                                 OR ISNULL([d].[definition],'') <> ISNULL([i].[definition],'')
                                 OR ISNULL([d].[type],'') <> ISNULL([i].[type],''));
         END;
    BEGIN
        -- insert changes (deleted values) for trigger - when trigger was deleted
        INSERT INTO [triggers_changes]
        ([database_id],
         [trigger_id],
         [table_id],
         [name],
         [before],
         [after],
         [instead_of],
         [on_insert],
         [on_update],
         [on_delete],
         [disabled],
         [definition],
         [type],
         [operation],
         [valid_to],
         [update_id]
        )
               SELECT [t].[database_id],
                      [d].[trigger_id],
                      [d].[table_id],
                      [d].[name],
                      [d].[before],
                      [d].[after],
                      [d].[instead_of],
                      [d].[on_insert],
                      [d].[on_update],
                      [d].[on_delete],
                      [d].[disabled],
                      [d].[definition],
                      [d].[type],
                      'DELETED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                      LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'D'
                       AND [d].[status] = 'A';
        -- insert changes (updated values) for trigger - when trigger was restored
        INSERT INTO [triggers_changes]
        ([database_id],
         [trigger_id],
         [table_id],
         [name],
         [before],
         [after],
         [instead_of],
         [on_insert],
         [on_update],
         [on_delete],
         [disabled],
         [definition],
         [type],
         [operation],
         [valid_from],
         [update_id]
        )
               SELECT [t].[database_id],
                      [i].[trigger_id],
                      [i].[table_id],
                      [i].[name],
                      [i].[before],
                      [i].[after],
                      [i].[instead_of],
                      [i].[on_insert],
                      [i].[on_update],
                      [i].[on_delete],
                      [i].[disabled],
                      [i].[definition],
                      [i].[type],
                      'ADDED',
                      GETDATE(),
                      [i].[update_id]
                 FROM [inserted] [i]
                      INNER JOIN [deleted] [d] ON [i].[trigger_id] = [d].[trigger_id]
                      LEFT OUTER JOIN [tables] [t] ON [d].[table_id] = [t].[table_id]
                 WHERE [i].[status] = 'A'
                       AND [d].[status] = 'D';
    END;
GO
ALTER TABLE [triggers] ENABLE TRIGGER [trg_triggers_change_track_update]
GO
/****** Object:  Trigger [trg_triggers_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_triggers_Modify]
   ON  [triggers] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [triggers]
     SET 
          last_modification_date = GETDATE()
     WHERE trigger_id IN (SELECT DISTINCT trigger_id FROM Inserted)
END
GO
ALTER TABLE [triggers] ENABLE TRIGGER [trg_triggers_Modify]
GO
/****** Object:  Trigger [trg_triggers_changes_description_modified]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_triggers_changes_description_modified]
	ON [triggers_changes]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [triggers_changes] 
		SET [description_date] = GETDATE()
		WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
	END
END
GO
ALTER TABLE [triggers_changes] ENABLE TRIGGER [trg_triggers_changes_description_modified]
GO
/****** Object:  Trigger [trg_unique_constraints_Modify]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_unique_constraints_Modify]
   ON  [unique_constraints] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [unique_constraints]
     SET 
        last_modification_date = GETDATE()
     WHERE unique_constraint_id IN (SELECT DISTINCT unique_constraint_id FROM Inserted)
END
GO
ALTER TABLE [unique_constraints] ENABLE TRIGGER [trg_unique_constraints_Modify]
GO
/****** Object:  Trigger [trg_unique_constraints_changes_description_modified]    Script Date: 7/5/2022 9:26:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_unique_constraints_changes_description_modified]
	ON [unique_constraints_changes]
	AFTER UPDATE
AS 
BEGIN
	IF UPDATE ([description]) 
	BEGIN
		UPDATE [unique_constraints_changes] 
		SET [description_date] = GETDATE()
		WHERE [id] in (SELECT DISTINCT [id] FROM Inserted)
	END
END
GO
ALTER TABLE [unique_constraints_changes] ENABLE TRIGGER [trg_unique_constraints_changes_description_modified]
GO
/****** Object:  Trigger [trg_unique_constraints_cols_Modify]    Script Date: 7/5/2022 9:26:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_unique_constraints_cols_Modify]
   ON  [unique_constraints_columns] 
   AFTER INSERT, UPDATE
AS 
BEGIN
     UPDATE [unique_constraints_columns]
     SET 
        last_modification_date = GETDATE()
     WHERE unique_constraint_column_id IN (SELECT DISTINCT unique_constraint_column_id FROM Inserted)
END
GO
ALTER TABLE [unique_constraints_columns] ENABLE TRIGGER [trg_unique_constraints_cols_Modify]
GO
/****** Object:  Trigger [trg_user_connections_Modify]    Script Date: 7/5/2022 9:26:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_user_connections_Modify]
   ON  [user_connections]
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [user_connections]
     SET  
        last_modification_date = GETDATE()
     WHERE connection_id IN (SELECT DISTINCT connection_id FROM Inserted)
END
GO
ALTER TABLE [user_connections] ENABLE TRIGGER [trg_user_connections_Modify]
GO
/****** Object:  Trigger [trg_version_Modify]    Script Date: 7/5/2022 9:26:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [trg_version_Modify]
   ON  [version] 
   AFTER INSERT,UPDATE
AS 
BEGIN
     UPDATE [version]
          SET  
             last_modification_date = GETDATE()
     WHERE version_entry_id IN (SELECT DISTINCT version_entry_id FROM Inserted)
END
GO
ALTER TABLE [version] ENABLE TRIGGER [trg_version_Modify] 
GO

INSERT INTO guid ([guid], [is_web_portal_connected]) VALUES (DEFAULT, DEFAULT);
GO
