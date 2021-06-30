using Npgsql;
using StswExpress;
using System;
using System.IO;
using System.Windows;

namespace WBZ
{
	internal static class SQL_Migration
	{
		/// <summary>
		/// Updates schema to make it compatible with the newest app version
		/// </summary>
		/// <returns>True - if schema has been updated</returns>
		internal static bool DoWork()
		{
			bool result = false;

			try
			{
				using (var sqlConn = SQL.ConnOpenedWBZ)
				{
					using (var sqlTran = sqlConn.BeginTransaction())
					{
						/// first time
						if (SQL.GetPropertyValue("VERSION") == null)
						{
							using var sqlCmd = new NpgsqlCommand(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL/database.sql")), sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
							using var sqlCmd2 = new NpgsqlCommand($"insert into wbz.config (property, value) values ('VERSION', '{Fn.AppVersion()}')", sqlConn, sqlTran);
							sqlCmd2.ExecuteNonQuery();
						}
						/// 1.0.0 => 1.0.1
						if (SQL.GetPropertyValue("VERSION") == "1.0.0")
						{
							using var sqlCmd = new NpgsqlCommand(@"
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
						/// 1.0.1 => 1.0.2
						if (SQL.GetPropertyValue("VERSION") == "1.0.1")
						{
							using var sqlCmd = new NpgsqlCommand(@"
								update wbz.config set value='1.0.2' where property='VERSION'", sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
						}
						/// 1.0.2 => 1.1.0
						if (SQL.GetPropertyValue("VERSION") == "1.0.2")
						{
							using var sqlCmd = new NpgsqlCommand(@"
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
						/// 1.1.0 => 1.2
						if (SQL.GetPropertyValue("VERSION") == "1.1.0")
						{
							using var sqlCmd = new NpgsqlCommand(@"
alter table wbz.companies rename to contractors;
alter table wbz.documents rename column company to contractor;
alter table wbz.stores_articles rename to stores_resources;

CREATE TABLE wbz.vehicles
(
    id serial PRIMARY KEY,
    register character varying(20) NOT NULL UNIQUE,
    brand character varying(40),
    model character varying(60),
    capacity decimal(18,3),
    forwarder_id integer,
    driver_id integer,
    prodyear integer,
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT vehicles_headers_forwarder_fkey FOREIGN KEY (forwarder_id)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT vehicles_headers_driver_fkey FOREIGN KEY (driver_id)
        REFERENCES wbz.employees (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

CREATE TABLE wbz.icons
(
    id bigserial PRIMARY KEY,
    module_alias character varying(3) NOT NULL,
    name character varying(255) NOT NULL,
    format character varying(10) NOT NULL,
    path character varying(2000) NOT NULL,
    height integer NOT NULL,
    width integer NOT NULL,
    size integer NOT NULL,
    content bytea NOT NULL,
    is_archival boolean NOT NULL DEFAULT false,
    comment text
);

alter table wbz.articles drop column if exists icon;
alter table wbz.attributes_classes drop column if exists icon;
alter table wbz.contractors drop column if exists icon;
alter table wbz.distributions drop column if exists icon;
alter table wbz.documents drop column if exists icon;
alter table wbz.employees drop column if exists icon;
alter table wbz.families drop column if exists icon;
alter table wbz.groups drop column if exists icon;
alter table wbz.stores drop column if exists icon;
alter table wbz.vehicles drop column if exists icon;

alter table wbz.articles add column if not exists icon_id integer;
alter table wbz.attributes_classes add column if not exists icon_id integer;
alter table wbz.contractors add column if not exists icon_id integer;
alter table wbz.distributions add column if not exists icon_id integer;
alter table wbz.documents add column if not exists icon_id integer;
alter table wbz.employees add column if not exists icon_id integer;
alter table wbz.families add column if not exists icon_id integer;
alter table wbz.groups add column if not exists icon_id integer;
alter table wbz.stores add column if not exists icon_id integer;
alter table wbz.vehicles add column if not exists icon_id integer;

alter table wbz.articles alter column price type numeric(12,2);
alter table wbz.articles rename column archival to is_archival;
alter table wbz.articles_measures rename column article to article_id;
alter table wbz.articles_measures rename column ""default"" to is_default;
alter table wbz.attachments rename column ""user"" to user_id;
alter table wbz.attachments rename column module to module_alias;
alter table wbz.attachments alter column module_alias type varchar(3);
alter table wbz.attachments rename column instance to instance_id;
alter table wbz.attachments rename column file to content;
alter table wbz.attachments add column if not exists path character varying(2000);
alter table wbz.attachments add column if not exists format character varying(10);
alter table wbz.attachments add column if not exists size integer;
alter table wbz.attachments alter column name type varchar(255);
alter table wbz.attributes rename column instance to instance_id;
alter table wbz.attributes rename column class to class_id;
alter table wbz.attributes alter column value type varchar(255);
alter table wbz.attributes rename column archival to is_archival;
alter table wbz.attributes_classes rename column module to module_alias;
alter table wbz.attributes_classes alter column module_alias type varchar(3);
alter table wbz.attributes_classes rename column archival to is_archival;
alter table wbz.attributes_classes rename column required to is_required;
alter table wbz.attributes_values rename column class to class_id;
alter table wbz.attributes_values alter column value type varchar(255);
alter table wbz.attributes_values rename column archival to is_archival;
alter table wbz.contacts rename column module to module_alias;
alter table wbz.contacts alter column module_alias type varchar(3);
alter table wbz.contacts rename column instance to instance_id;
alter table wbz.contacts alter column email type varchar(100);
alter table wbz.contacts alter column phone type varchar(30);
alter table wbz.contacts rename column archival to is_archival;
alter table wbz.contacts rename column ""default"" to is_default;
alter table wbz.contractors rename column archival to is_archival;
alter table wbz.distributions rename column archival to is_archival;
alter table wbz.distributions_positions rename column ""position"" to pos;
alter table wbz.distributions_positions rename column distribution to distribution_id;
alter table wbz.distributions_positions rename column article to article_id;
alter table wbz.distributions_positions rename column store to store_id;
alter table wbz.distributions_positions rename column family to family_id;
alter table wbz.distributions_positions rename column amount to quantity;
alter table wbz.documents alter column type type varchar(3);
alter table wbz.documents rename column contractor to contractor_id;
alter table wbz.documents rename column store to store_id;
alter table wbz.documents rename column archival to is_archival;
alter table wbz.documents_positions rename column document to document_id;
alter table wbz.documents_positions rename column ""position"" to pos;
alter table wbz.documents_positions rename column article to article_id;
alter table wbz.documents_positions rename column cost to net;
alter table wbz.documents_positions rename column amount to quantity;
alter table wbz.documents_positions alter column net type numeric(15,2);
alter table wbz.documents_positions alter column tax type numeric(15,2);
alter table wbz.employees alter column forename type varchar(30);
alter table wbz.employees alter column email type varchar(100);
alter table wbz.employees alter column phone type varchar(30);
alter table wbz.employees rename column archival to archival_id;
alter table wbz.families rename column archival to is_archival;
alter table wbz.groups rename column module to module_alias;
alter table wbz.groups alter column module_alias type varchar(3);
alter table wbz.groups rename column instance to instance_id;
alter table wbz.groups rename column owner to owner_id;
alter table wbz.groups rename column archival to is_archival;
alter table wbz.logs rename column ""user"" to user_id;
alter table wbz.logs rename column module to module_alias;
alter table wbz.logs alter column module_alias type varchar(3);
alter table wbz.logs rename column instance to instance_id;
alter table wbz.logs rename column datetime to datecreated;
alter table wbz.notifications rename column ""user"" to user_id;
alter table wbz.notifications rename column ""read"" to is_read;
alter table wbz.notifications rename column datetime to datecreated;
alter table wbz.stores rename column archival to is_archival;
alter table wbz.stores_resources rename column store to store_id;
alter table wbz.stores_resources rename column article to article_id;
alter table wbz.stores_resources rename column amount to quantity;
alter table wbz.users rename column username to login;
alter table wbz.users alter column email type varchar(100);
alter table wbz.users alter column phone type varchar(30);
alter table wbz.users rename column archival to is_archival;
alter table wbz.users rename column blocked to is_blocked;
alter table wbz.users_permissions rename column ""user"" to user_id;

								update wbz.config set value='1.2' where property='VERSION'", sqlConn, sqlTran);
							sqlCmd.ExecuteNonQuery();
						}

						sqlTran.Commit();
					}
				}

				Fn.AppDatabase.Version = SQL.GetPropertyValue("VERSION");
				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return result;
		}
	}
}
