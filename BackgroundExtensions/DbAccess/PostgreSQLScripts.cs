namespace BackgroundExtensions.DbAccess;

public class PostgreSQLScripts
{
    public static string CreateExtensionsTable => @"
		create table if not exists extensions (
			id serial primary key,
			filename text not null,
			name text not null,
			version text not null
		)
	";

    public static string PostExtension => @"
		insert into extensions (filename, name, version) values (@filename, @name, @version)
	";

	public static string GetActualExtensions => @"
		select max(filename) as FileName, max(name) as Name, max(version) as Version from extensions group by name
	";
}
