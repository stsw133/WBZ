using Npgsql;
using System;
using System.Windows;
using WBZ.Helpers;

namespace WBZ
{
	internal static class SQL_Migration
	{
		/// <summary>
		/// Funkcja migrująca bazę do aktualnej wersji z wersji wcześniejszych
		/// </summary>
		/// <returns>True - jeśli udało się przeprowadzić migrację</returns>
		internal static bool DoWork()
		{
			bool result = false;

			try
			{
				using (var sqlConn = new NpgsqlConnection(SQL.connWBZ))
				{
					sqlConn.Open();

					using (var sqlTran = sqlConn.BeginTransaction())
					{
						///1.0.0 => 1.0.1
						if (SQL.GetPropertyValue("VERSION") == "1.0.0")
						{
							var sqlCmd = new NpgsqlCommand(@"
CREATE OR REPLACE FUNCTION wbz.artdefmeaval(
	_article integer,
	_amount double precision)
    RETURNS double precision
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN _amount * coalesce(nullif((select converter FROM wbz.articles_measures WHERE article = _article AND ""default""=true),0),1);
END
$BODY$;

CREATE OR REPLACE FUNCTION wbz.artdefmeacon(
	_article integer)
    RETURNS double precision
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN coalesce(nullif((select converter FROM wbz.articles_measures WHERE article = _article AND ""default""=true),0),1);
END
$BODY$;

CREATE OR REPLACE FUNCTION wbz.artdefmeanam(
	_article integer)
    RETURNS character varying
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN coalesce((SELECT name FROM wbz.articles_measures WHERE article = _article AND ""default""=true), 'kg');
END
$BODY$;

								update wbz.config set value='1.0.1' where property='VERSION'", sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
						}
						///1.0.1 => 1.0.2
						if (SQL.GetPropertyValue("VERSION") == "1.0.1")
						{
							var sqlCmd = new NpgsqlCommand(@"
								update wbz.config set value='1.0.2' where property='VERSION'", sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
						}
						///1.0.2 => 1.1.0
						if (SQL.GetPropertyValue("VERSION") == "1.0.2")
						{
							var sqlCmd = new NpgsqlCommand(@"
delete from wbz.users_permissions where perm='admin_config_preview';
delete from wbz.users_permissions where perm='admin_config_save';
delete from wbz.users_permissions where perm='admin_config_delete';
delete from wbz.users_permissions where perm='admin_versions_preview';
delete from wbz.users_permissions where perm='admin_versions_save';
delete from wbz.users_permissions where perm='admin_versions_delete';
update wbz.users_permissions set perm='users_preview' where perm='admin_users_preview';
update wbz.users_permissions set perm='users_save' where perm='admin_users_save';
update wbz.users_permissions set perm='users_delete' where perm='admin_users_delete';
update wbz.users_permissions set perm='attributes_classes_preview' where perm='admin_attributes_preview';
update wbz.users_permissions set perm='attributes_classes_save' where perm='admin_attributes_save';
update wbz.users_permissions set perm='attributes_classes_delete' where perm='admin_attributes_delete';
update wbz.users_permissions set perm='logs_preview' where perm='admin_logs_preview';
update wbz.users_permissions set perm='logs_save' where perm='admin_logs_save';
update wbz.users_permissions set perm='logs_delete' where perm='admin_logs_delete';

alter table wbz.users rename column active to blocked;
alter table wbz.logs rename column obj to instance;
alter table wbz.attachments rename column obj to instance;
alter table wbz.attributes rename column obj to instance;
alter table wbz.contacts rename column obj to instance;

alter table wbz.users alter column blocked set default false;
update wbz.users set blocked = not blocked;

alter table wbz.users add column if not exists archival boolean not null default false;
alter table wbz.attributes_classes add column if not exists defvalue character varying(50);
alter table wbz.attributes_classes add column if not exists required boolean not null default false;
alter table wbz.attributes_classes add column if not exists archival boolean not null default false;
alter table wbz.attributes add column if not exists archival boolean not null default false;
alter table wbz.attachments add column if not exists comment text;

CREATE TABLE wbz.employees
(
    id serial NOT NULL,
    ""user"" integer,
	email character varying(60) COLLATE pg_catalog.""default"",
	phone character varying(16) COLLATE pg_catalog.""default"",
	forename character varying(50) COLLATE pg_catalog.""default"" NOT NULL,
	lastname character varying(30) COLLATE pg_catalog.""default"" NOT NULL,
	postcode character varying(6) COLLATE pg_catalog.""default"",
	city character varying(40) COLLATE pg_catalog.""default"",
	address character varying(60) COLLATE pg_catalog.""default"",
	department character varying(40) COLLATE pg_catalog.""default"",
	position character varying(40) COLLATE pg_catalog.""default"",
	archival boolean NOT NULL DEFAULT false,
	comment text COLLATE pg_catalog.""default"",
	CONSTRAINT employees_pkey PRIMARY KEY(id),
	CONSTRAINT employees_user_fkey FOREIGN KEY(""user"")
		REFERENCES wbz.users(id) MATCH SIMPLE
		ON UPDATE CASCADE
		ON DELETE SET NULL
		NOT VALID
)
TABLESPACE pg_default;

							update wbz.config set value='1.1.0' where property='VERSION'", sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
						}

						sqlTran.Commit();
					}

					sqlConn.Close();
				}

				result = true;

				Global.Database.Version = SQL.GetPropertyValue("VERSION");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
	}
}
