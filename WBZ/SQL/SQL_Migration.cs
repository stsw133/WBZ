using Npgsql;
using StswExpress.Globals;
using System;
using System.Windows;

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
				using (var sqlConn = SQL.connOpenedWBZ)
				{
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
alter table wbz.groups add column if not exists instance integer;
alter table wbz.groups add column if not exists icon bytea;
alter table wbz.attributes_classes add column if not exists icon bytea;
alter table wbz.articles add column if not exists icon bytea;
alter table wbz.stores add column if not exists icon bytea;
alter table wbz.companies add column if not exists icon bytea;
alter table wbz.families add column if not exists icon bytea;
alter table wbz.documents add column if not exists icon bytea;
alter table wbz.distributions add column if not exists icon bytea;
alter table wbz.logs add column if not exists type smallint not null default 1;

alter table wbz.users_permissions drop column if exists id;
drop sequence if exists wbz.users_permissions_id_seq;

CREATE TABLE wbz.employees
(
    id serial NOT NULL,
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
	icon bytea,
	CONSTRAINT employees_pkey PRIMARY KEY(id)
) TABLESPACE pg_default;

CREATE TABLE wbz.attributes_values
(
    id bigserial NOT NULL,
    class integer NOT NULL,
    value character varying(50) COLLATE pg_catalog.""default"" NOT NULL,
	archival boolean NOT NULL DEFAULT false,
	CONSTRAINT attributes_values_pkey PRIMARY KEY(id),
	CONSTRAINT attributes_values_class_fkey FOREIGN KEY(class)
        REFERENCES wbz.attributes_classes(id) MATCH SIMPLE
		ON UPDATE CASCADE
		ON DELETE CASCADE
) TABLESPACE pg_default;

							update wbz.config set value='1.1.0' where property='VERSION'", sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
						}
						///1.1.0 => 1.2
						if (SQL.GetPropertyValue("VERSION") == "1.1.0")
						{
							var sqlCmd = new NpgsqlCommand(@"
update wbz.users_permissions set perm='contractors_preview' where perm='companies_preview';
update wbz.users_permissions set perm='contractors_save' where perm='companies_save';
update wbz.users_permissions set perm='contractors_delete' where perm='companies_delete';

alter table wbz.companies rename to contractors;
alter table wbz.documents rename column company to contractor;

CREATE TABLE wbz.vehicles
(
    id serial NOT NULL,
    register character varying(20) COLLATE pg_catalog.""default"" NOT NULL,
	brand character varying(40) COLLATE pg_catalog.""default"",
	model character varying(60) COLLATE pg_catalog.""default"",
	capacity decimal(18, 3),
	forwarder integer,
	driver integer,
	prodyear integer,
	archival boolean NOT NULL DEFAULT false,
	comment text COLLATE pg_catalog.""default"",
	icon bytea,
	CONSTRAINT vehicles_pkey PRIMARY KEY(id),
	CONSTRAINT vehicles_register_key UNIQUE (register),
	CONSTRAINT vehicles_headers_forwarder_fkey FOREIGN KEY(forwarder)
		REFERENCES wbz.contractors(id) MATCH SIMPLE
		ON UPDATE CASCADE
		ON DELETE NO ACTION,
	CONSTRAINT vehicles_headers_driver_fkey FOREIGN KEY(driver)
		REFERENCES wbz.employees(id) MATCH SIMPLE
		ON UPDATE CASCADE
		ON DELETE NO ACTION
) TABLESPACE pg_default;

							update wbz.config set value='1.2' where property='VERSION'", sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
						}

						sqlTran.Commit();
					}
				}

				result = true;

				Global.AppDatabase.Version = SQL.GetPropertyValue("VERSION");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
	}
}
