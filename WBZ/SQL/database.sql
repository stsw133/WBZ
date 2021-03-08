-- Database: wbz
CREATE DATABASE wbz
    WITH 
    OWNER = wbz
    ENCODING = 'UTF8'
    LC_COLLATE = 'Polish_Poland.1250'
    LC_CTYPE = 'Polish_Poland.1250'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Role: wbz_user
CREATE ROLE wbz_user WITH
  LOGIN
  SUPERUSER
  INHERIT
  CREATEDB
  CREATEROLE
  REPLICATION;

-- SCHEMA: wbz
CREATE SCHEMA wbz AUTHORIZATION postgres;
COMMENT ON SCHEMA wbz IS 'Wielkopolski Bank ¯ywnoœci';
GRANT ALL ON SCHEMA wbz TO postgres;
GRANT ALL ON SCHEMA wbz TO wbz_user;

-- Table: wbz.config
CREATE TABLE wbz.config
(
    property character varying(50) COLLATE pg_catalog."default" NOT NULL,
    value character varying(100) COLLATE pg_catalog."default",
    CONSTRAINT config_pkey PRIMARY KEY (property)
)
TABLESPACE pg_default;

INSERT INTO wbz.config (property, value) VALUES
    ('VERSION', '1.2.0'),
    ('LOGS_ENABLED', '0'),
    ('EMAIL_HOST', 'smtp.gmail.com'),
	('EMAIL_PORT', '587'),
	('EMAIL_ADDRESS', 'wbz.email.testowy@gmail.com'),
	('EMAIL_PASSWORD', '');

-- Table: wbz.users
CREATE TABLE wbz.users
(
    id serial NOT NULL,
    username character varying(20) COLLATE pg_catalog."default" NOT NULL,
    password text COLLATE pg_catalog."default" NOT NULL,
    email character varying(60) COLLATE pg_catalog."default" NOT NULL,
    phone character varying(16) COLLATE pg_catalog."default",
    forename character varying(30) COLLATE pg_catalog."default",
    lastname character varying(30) COLLATE pg_catalog."default",
    blocked boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT users_pkey PRIMARY KEY (id),
    CONSTRAINT users_username_key UNIQUE (username),
    CONSTRAINT users_email_key UNIQUE (email)
)
TABLESPACE pg_default;

-- Table: wbz.users_permissions
CREATE TABLE wbz.users_permissions
(
    "user" integer NOT NULL,
    perm character varying(50) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT users_permissions_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)
TABLESPACE pg_default;
/*
INSERT INTO wbz.users_permissions ("user", perm) VALUES
    (1, 'admin'),
    (1, 'users_preview'), (1, 'users_save'), (1, 'users_delete'),
    (1, 'employees_preview'), (1, 'employees_save'), (1, 'employees_delete'),
    (1, 'documents_preview'), (1, 'documents_save'), (1, 'documents_delete'),
    (1, 'articles_preview'), (1, 'articles_save'), (1, 'articles_delete'),
    (1, 'stores_preview'), (1, 'stores_save'), (1, 'stores_delete'),
    (1, 'contractors_preview'), (1, 'contractors_save'), (1, 'contractors_delete'),
    (1, 'families_preview'), (1, 'families_save'), (1, 'families_delete'),
    (1, 'distributions_preview'), (1, 'distributions_save'), (1, 'distributions_delete'),
    (1, 'attributes_classes_preview'), (1, 'attributes_classes_save'), (1, 'attributes_classes_delete'),
    (1, 'attachments_preview'), (1, 'attachments_save'), (1, 'attachments_delete'),
    (1, 'logs_preview'), (1, 'logs_save'), (1, 'logs_delete'),
    (1, 'stats_preview'), (1, 'stats_save'), (1, 'stats_delete'),
    (1, 'community_preview'), (1, 'community_save'), (1, 'community_delete');
*/
-- Table: wbz.logs
CREATE TABLE wbz.logs
(
    id bigserial NOT NULL,
    "user" integer NOT NULL,
    module character varying(50) COLLATE pg_catalog."default" NOT NULL,
    instance integer NOT NULL,
    type smallint NOT NULL DEFAULT 1,
    content text COLLATE pg_catalog."default" NOT NULL,
    datetime timestamp(6) with time zone NOT NULL DEFAULT now(),
    CONSTRAINT logs_pkey PRIMARY KEY (id),
    CONSTRAINT logs_perms_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)
TABLESPACE pg_default;

-- Table: wbz.attachments
CREATE TABLE wbz.attachments
(
    id bigserial NOT NULL,
    "user" integer NOT NULL,
    module character varying(50) COLLATE pg_catalog."default" NOT NULL,
    instance integer NOT NULL,
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    file bytea NOT NULL,
    comment text COLLATE pg_catalog."default",
    CONSTRAINT attachments_pkey PRIMARY KEY (id),
    CONSTRAINT attachments_perms_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)
TABLESPACE pg_default;

-- Table: wbz.groups
CREATE TABLE wbz.groups
(
    id serial NOT NULL,
    module character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    instance integer,
    owner integer,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT groups_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

INSERT INTO wbz.groups (id, module, name, instance, owner, archival, comment, icon) VALUES
    (1, 'logs', 'Logi', null, 0, false, '', null),
    (2, 'logs', 'B³êdy', null, 0, false, '', null);

-- Table: wbz.attributes_classes
CREATE TABLE wbz.attributes_classes
(
    id serial NOT NULL,
    module character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    type character varying(20) COLLATE pg_catalog."default" NOT NULL,
    "values" character varying(100) COLLATE pg_catalog."default",
    defvalue character varying(50) COLLATE pg_catalog."default",
    required boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT attributes_classes_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.attributes
CREATE TABLE wbz.attributes
(
    id bigserial NOT NULL,
    instance integer NOT NULL,
    class integer NOT NULL,
    value character varying(50) COLLATE pg_catalog."default" NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_pkey PRIMARY KEY (id),
    CONSTRAINT attributes_class_fkey FOREIGN KEY (class)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
TABLESPACE pg_default;

-- Table: wbz.attributes
CREATE TABLE wbz.attributes_values
(
    id bigserial NOT NULL,
    class integer NOT NULL,
    value character varying(50) COLLATE pg_catalog."default" NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_values_pkey PRIMARY KEY (id),
    CONSTRAINT attributes_values_class_fkey FOREIGN KEY (class)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
TABLESPACE pg_default;

-- Table: wbz.contacts
CREATE TABLE wbz.contacts
(
    id serial NOT NULL,
    module character varying(50) COLLATE pg_catalog."default" NOT NULL,
    instance integer NOT NULL,
    email character varying(60) COLLATE pg_catalog."default",
    phone character varying(12) COLLATE pg_catalog."default",
    forename character varying(30) COLLATE pg_catalog."default",
    lastname character varying(30) COLLATE pg_catalog."default",
    "default" boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT contacts_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.notifications
CREATE TABLE wbz.notifications
(
    id serial NOT NULL,
    "user" integer NOT NULL,
    content character varying(100) COLLATE pg_catalog."default" NOT NULL,
    read boolean NOT NULL DEFAULT false,
    datetime timestamp(6) with time zone NOT NULL DEFAULT now(),
    action character varying(20) COLLATE pg_catalog."default",
    CONSTRAINT notifications_pkey PRIMARY KEY (id),
    CONSTRAINT notifications_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
TABLESPACE pg_default;

-- Table: wbz.articles
CREATE TABLE wbz.articles
(
    id serial NOT NULL,
    codename character varying(20) COLLATE pg_catalog."default",
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    ean character varying(20) COLLATE pg_catalog."default",
    price numeric(16,2),
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT articles_pkey PRIMARY KEY (id),
    CONSTRAINT articles_codename_key UNIQUE (codename)
)
TABLESPACE pg_default;

-- Table: wbz.articles_measures
CREATE TABLE wbz.articles_measures
(
    id serial NOT NULL,
    article integer NOT NULL,
    name character varying(10) COLLATE pg_catalog."default" NOT NULL,
    converter double precision NOT NULL DEFAULT 1,
    "default" boolean NOT NULL DEFAULT false,
    CONSTRAINT articles_measures_pkey PRIMARY KEY (id),
    CONSTRAINT articles_measures_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
TABLESPACE pg_default;

-- Table: wbz.stores
CREATE TABLE wbz.stores
(
    id serial NOT NULL,
    codename character varying(20) COLLATE pg_catalog."default",
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    city character varying(40) COLLATE pg_catalog."default" NOT NULL,
    address character varying(60) COLLATE pg_catalog."default" NOT NULL,
    postcode character varying(6) COLLATE pg_catalog."default" NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT stores_pkey PRIMARY KEY (id),
    CONSTRAINT stores_codename_key UNIQUE (codename)
)
TABLESPACE pg_default;

-- Table: wbz.stores_articles
CREATE TABLE wbz.stores_articles
(
    id serial NOT NULL,
    store integer NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    reserved numeric(15,3) NOT NULL DEFAULT 0,
    CONSTRAINT stores_articles_pkey PRIMARY KEY (id),
    CONSTRAINT stores_articles_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT stores_articles_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)
TABLESPACE pg_default;

-- Table: wbz.contractors
CREATE TABLE wbz.contractors
(
    id serial NOT NULL,
    codename character varying(20) COLLATE pg_catalog."default",
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    branch character varying(40) COLLATE pg_catalog."default",
    nip character varying(10) COLLATE pg_catalog."default",
    regon character varying(9) COLLATE pg_catalog."default",
    city character varying(40) COLLATE pg_catalog."default" NOT NULL,
    address character varying(60) COLLATE pg_catalog."default" NOT NULL,
    postcode character varying(6) COLLATE pg_catalog."default" NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT contractors_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.families
CREATE TABLE wbz.families
(
    id serial NOT NULL,
    declarant character varying(50) COLLATE pg_catalog."default" NOT NULL,
    lastname character varying(30) COLLATE pg_catalog."default" NOT NULL,
    members smallint NOT NULL,
    city character varying(40) COLLATE pg_catalog."default" NOT NULL,
    address character varying(60) COLLATE pg_catalog."default" NOT NULL,
    postcode character varying(6) COLLATE pg_catalog."default" NOT NULL,
    status boolean NOT NULL DEFAULT true,
    c_sms boolean NOT NULL DEFAULT false,
    c_call boolean NOT NULL DEFAULT false,
    c_email boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT families_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.documents
CREATE TABLE wbz.documents
(
    id serial NOT NULL,
    type character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    store integer NOT NULL,
    contractor integer NOT NULL,
    dateissue date NOT NULL DEFAULT now(),
    status smallint NOT NULL DEFAULT 0,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT documents_headers_pkey PRIMARY KEY (id),
    CONSTRAINT documents_headers_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT documents_headers_contractor_fkey FOREIGN KEY (contractor)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)
TABLESPACE pg_default;

-- Table: wbz.documents_positions
CREATE TABLE wbz.documents_positions
(
    id bigserial NOT NULL,
    document integer NOT NULL,
    "position" smallint NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    cost numeric(16,2) NOT NULL DEFAULT 0,
    tax numeric(16,2) NOT NULL DEFAULT 0,
    CONSTRAINT documents_positions_pkey PRIMARY KEY (id),
    CONSTRAINT documents_positions_document_fkey FOREIGN KEY (document)
        REFERENCES wbz.documents (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT documents_positions_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)
TABLESPACE pg_default;

-- Table: wbz.distributions
CREATE TABLE wbz.distributions
(
    id serial NOT NULL,
    name character varying(50) COLLATE pg_catalog."default",
    datereal date,
    status smallint NOT NULL DEFAULT 0,
    archival boolean NOT NULL,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT distribution_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.distributions_positions
CREATE TABLE wbz.distributions_positions
(
    id bigserial NOT NULL,
    distribution integer NOT NULL,
    "position" smallint,
    family integer NOT NULL,
    members smallint NOT NULL,
    store integer NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    status smallint NOT NULL,
    CONSTRAINT distributions_positions_pkey PRIMARY KEY (id),
    CONSTRAINT distributions_positions_distribution_fkey FOREIGN KEY (distribution)
        REFERENCES wbz.distributions (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT distributions_positions_family_fkey FOREIGN KEY (family)
        REFERENCES wbz.families (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT distributions_positions_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT distributions_positions_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)
TABLESPACE pg_default;

-- Table: wbz.employees
CREATE TABLE wbz.employees
(
    id serial NOT NULL,
    email character varying(60) COLLATE pg_catalog."default",
    phone character varying(16) COLLATE pg_catalog."default",
    forename character varying(50) COLLATE pg_catalog."default" NOT NULL,
    lastname character varying(30) COLLATE pg_catalog."default" NOT NULL,
    city character varying(40) COLLATE pg_catalog."default",
    address character varying(60) COLLATE pg_catalog."default",
    postcode character varying(6) COLLATE pg_catalog."default",
    department character varying(40) COLLATE pg_catalog."default",
    position character varying(40) COLLATE pg_catalog."default",
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT employees_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.vehicles
CREATE TABLE wbz.vehicles
(
    id serial NOT NULL,
    register character varying(20) COLLATE pg_catalog."default" NOT NULL,
    brand character varying(40) COLLATE pg_catalog."default",
    model character varying(60) COLLATE pg_catalog."default",
    capacity decimal(18,3),
    forwarder integer,
    driver integer,
    prodyear integer,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    icon bytea,
    CONSTRAINT vehicles_pkey PRIMARY KEY (id),
    CONSTRAINT vehicles_register_key UNIQUE (register),
    CONSTRAINT vehicles_headers_forwarder_fkey FOREIGN KEY (forwarder)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT vehicles_headers_driver_fkey FOREIGN KEY (driver)
        REFERENCES wbz.employees (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)
TABLESPACE pg_default;

-- FUNCTION: wbz.artdefmeaval(integer, double precision)
CREATE OR REPLACE FUNCTION wbz.artdefmeaval(
	_article integer,
	_amount double precision)
    RETURNS double precision
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN _amount * coalesce(nullif((select converter FROM wbz.articles_measures WHERE article = _article AND "default"=true),0),1);
END
$BODY$;

-- FUNCTION: wbz.artdefmeacon(integer)
CREATE OR REPLACE FUNCTION wbz.artdefmeacon(
	_article integer)
    RETURNS double precision
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN coalesce(nullif((select converter FROM wbz.articles_measures WHERE article = _article AND "default"=true),0),1);
END
$BODY$;

-- FUNCTION: wbz.artdefmeanam(integer)
CREATE OR REPLACE FUNCTION wbz.artdefmeanam(
	_article integer)
    RETURNS character varying
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN coalesce((SELECT name FROM wbz.articles_measures WHERE article = _article AND "default"=true), 'kg');
END
$BODY$;
