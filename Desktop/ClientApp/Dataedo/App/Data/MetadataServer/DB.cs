using Dataedo.App.Data.MetadataServer.DataLineage;
using Dataedo.App.Tools.ERD;

namespace Dataedo.App.Data.MetadataServer;

internal static class DB
{
	public static DatabaseDB Database = new DatabaseDB();

	public static BusinessGlossaryDB BusinessGlossary = new BusinessGlossaryDB();

	public static ColumnDB Column = new ColumnDB();

	public static TableDB Table = new TableDB();

	public static RelationsDB Relation = new RelationsDB();

	public static RelationColumnDB RelationColumns = new RelationColumnDB();

	public static DependenciesDB Dependency = new DependenciesDB();

	public static DataProcessesDB DataProcess = new DataProcessesDB();

	public static ProcedureDB Procedure = new ProcedureDB();

	public static ParameterDB Parameter = new ParameterDB();

	public static ModulesDB Module = new ModulesDB();

	public static SchemaImportsAndChangesDB SchemaImportsAndChanges = new SchemaImportsAndChangesDB();

	public static DataProfilingDB DataProfiling = new DataProfilingDB();

	public static IgnoredObjects IgnoredObjects = new IgnoredObjects();

	public static TriggerDB Trigger = new TriggerDB();

	public static ConstraintDB Constraint = new ConstraintDB();

	public static ConstraintColumnDB ConstraintColumn = new ConstraintColumnDB();

	public static NodeDB ErdNode = new NodeDB();

	public static LinkDB ErdLink = new LinkDB();

	public static PostItDB ErdPostIt = new PostItDB();

	public static NodeColumnDB NodeColumn = new NodeColumnDB();

	public static DocumentationProgressDB DocumentationProgress = new DocumentationProgressDB();

	public static CustomFieldDB CustomField = new CustomFieldDB(Dataedo.App.StaticData.Commands);

	public static UserPersonalSettingsDB UserPersonalSettings = new UserPersonalSettingsDB();

	public static ClassificatorMetaDB Classificator = new ClassificatorMetaDB(Dataedo.App.StaticData.Commands);

	public static SessionDB Session = new SessionDB();

	public static DataFlowsDB DataFlows = new DataFlowsDB();

	public static ObjectBrowserDB ObjectBrowser = new ObjectBrowserDB();

	public static CommunityDB Community = new CommunityDB();

	public static HistoryDB History = new HistoryDB();

	public static InterfaceTablesDB InterfaceTables = new InterfaceTablesDB();

	public static void ReloadClasses()
	{
		Database = new DatabaseDB();
		BusinessGlossary = new BusinessGlossaryDB();
		Column = new ColumnDB();
		Table = new TableDB();
		Relation = new RelationsDB();
		RelationColumns = new RelationColumnDB();
		Dependency = new DependenciesDB();
		DataProcess = new DataProcessesDB();
		Procedure = new ProcedureDB();
		Parameter = new ParameterDB();
		Module = new ModulesDB();
		SchemaImportsAndChanges = new SchemaImportsAndChangesDB();
		IgnoredObjects = new IgnoredObjects();
		Trigger = new TriggerDB();
		Constraint = new ConstraintDB();
		ConstraintColumn = new ConstraintColumnDB();
		ErdNode = new NodeDB();
		ErdLink = new LinkDB();
		ErdPostIt = new PostItDB();
		NodeColumn = new NodeColumnDB();
		DocumentationProgress = new DocumentationProgressDB();
		CustomField = new CustomFieldDB(Dataedo.App.StaticData.Commands);
		UserPersonalSettings = new UserPersonalSettingsDB();
		Classificator = new ClassificatorMetaDB(Dataedo.App.StaticData.Commands);
		Session = new SessionDB();
		DataFlows = new DataFlowsDB();
		ObjectBrowser = new ObjectBrowserDB();
		DataProfiling = new DataProfilingDB();
		Community = new CommunityDB();
		History = new HistoryDB();
		InterfaceTables = new InterfaceTablesDB();
	}
}
