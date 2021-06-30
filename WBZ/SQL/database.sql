-- Database: wbz
--CREATE DATABASE WBZ
--    WITH 
--    OWNER = WBZ
--    ENCODING = 'UTF8'
--    LC_COLLATE = 'Polish_Poland.1250'
--    LC_CTYPE = 'Polish_Poland.1250'
--    TABLESPACE = pg_default
--    CONNECTION LIMIT = -1;

-- Role: wbz_user
CREATE ROLE userWBZ WITH
  LOGIN
  SUPERUSER
  INHERIT
  CREATEDB
  CREATEROLE
  REPLICATION;
 
-- SCHEMA: wbz
CREATE SCHEMA wbz AUTHORIZATION userWBZ;
--GRANT ALL ON SCHEMA wbz TO postgres;
GRANT ALL ON SCHEMA wbz TO userWBZ;

-- Table: wbz.config
CREATE TABLE wbz.config
(
    property character varying(50) PRIMARY KEY,
    value character varying(100)
);

-- Table: wbz.users
CREATE TABLE wbz.users
(
    id serial PRIMARY KEY,
    login character varying(20) NOT NULL UNIQUE,
    password text NOT NULL,
    email character varying(100) NOT NULL UNIQUE,
    phone character varying(30),
    forename character varying(30),
    lastname character varying(30),
    is_archival boolean NOT NULL DEFAULT false,
    is_blocked boolean NOT NULL DEFAULT false
);

-- Table: wbz.users_permissions
CREATE TABLE wbz.users_permissions
(
    user_id integer NOT NULL,
    perm character varying(50) NOT NULL,
    CONSTRAINT users_permissions_user_fkey FOREIGN KEY (user_id)
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
);

-- Table: wbz.logs
CREATE TABLE wbz.logs
(
    id bigserial PRIMARY KEY,
    user_id integer NOT NULL,
    module_alias character varying(3) NOT NULL,
    instance_id integer NOT NULL,
    type smallint NOT NULL DEFAULT 1,
    content text NOT NULL,
    datecreated timestamp(6) with time zone NOT NULL DEFAULT now(),
    CONSTRAINT logs_perms_user_fkey FOREIGN KEY (user_id)
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.attachments
CREATE TABLE wbz.attachments
(
    id bigserial PRIMARY KEY,
    user_id integer NOT NULL,
    module_alias character varying(3) NOT NULL,
    instance_id integer NOT NULL,
    name character varying(255) NOT NULL,
    format character varying(10) NOT NULL,
    path character varying(2000) NOT NULL,
    size integer NOT NULL,
    content bytea NOT NULL,
    comment text,
    CONSTRAINT attachments_perms_user_fkey FOREIGN KEY (user_id)
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.icons
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

-- Table: wbz.groups
CREATE TABLE wbz.groups
(
    id serial PRIMARY KEY,
    module_alias character varying(3) NOT NULL,
    name character varying(50) NOT NULL,
    instance_id integer,
    owner_id integer,
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.attributes_classes
CREATE TABLE wbz.attributes_classes
(
    id serial PRIMARY KEY,
    module_alias character varying(3) NOT NULL,
    name character varying(50) NOT NULL,
    type character varying(20) NOT NULL,
    values character varying(100),
    defvalue character varying(50),
    is_archival boolean NOT NULL DEFAULT false,
    is_required boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.attributes
CREATE TABLE wbz.attributes
(
    id bigserial PRIMARY KEY,
    instance_id integer NOT NULL,
    class_id integer NOT NULL,
    value character varying(255) NOT NULL,
    is_archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_class_fkey FOREIGN KEY (class_id)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Table: wbz.attributes
CREATE TABLE wbz.attributes_values
(
    id bigserial PRIMARY KEY,
    class_id integer NOT NULL,
    value character varying(255) NOT NULL,
    is_archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_values_class_fkey FOREIGN KEY (class_id)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Table: wbz.contacts
CREATE TABLE wbz.contacts
(
    id serial PRIMARY KEY,
    module_alias character varying(3) NOT NULL,
    instance_id integer NOT NULL,
    email character varying(100),
    phone character varying(30),
    forename character varying(30),
    lastname character varying(30),
    is_archival boolean NOT NULL DEFAULT false,
    is_default boolean NOT NULL DEFAULT false
);

-- Table: wbz.notifications
CREATE TABLE wbz.notifications
(
    id serial PRIMARY KEY,
    user_id integer NOT NULL,
    content character varying(100) NOT NULL,
    is_read boolean NOT NULL DEFAULT false,
    datecreated timestamp(6) with time zone NOT NULL DEFAULT now(),
    action character varying(20),
    CONSTRAINT notifications_user_fkey FOREIGN KEY (user_id)
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Table: wbz.articles
CREATE TABLE wbz.articles
(
    id serial PRIMARY KEY,
    codename character varying(20) UNIQUE,
    name character varying(100) NOT NULL,
    ean character varying(20),
    price numeric(12,2),
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.articles_measures
CREATE TABLE wbz.articles_measures
(
    id serial PRIMARY KEY,
    article_id integer NOT NULL,
    name character varying(10) NOT NULL,
    converter double precision NOT NULL DEFAULT 1,
    is_default boolean NOT NULL DEFAULT false,
    CONSTRAINT articles_measures_article_fkey FOREIGN KEY (article_id)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Table: wbz.stores
CREATE TABLE wbz.stores
(
    id serial PRIMARY KEY,
    codename character varying(20) UNIQUE,
    name character varying(100) NOT NULL,
    city character varying(40) NOT NULL,
    address character varying(60) NOT NULL,
    postcode character varying(6) NOT NULL,
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.stores_resources
CREATE TABLE wbz.stores_resources
(
    id serial PRIMARY KEY,
    store_id integer NOT NULL,
    article_id integer NOT NULL,
    quantity numeric(15,3) NOT NULL DEFAULT 0,
    reserved numeric(15,3) NOT NULL DEFAULT 0,
    CONSTRAINT stores_resources_store_fkey FOREIGN KEY (store_id)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT stores_resources_article_fkey FOREIGN KEY (article_id)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Table: wbz.contractors
CREATE TABLE wbz.contractors
(
    id serial PRIMARY KEY,
    codename character varying(20),
    name character varying(100) NOT NULL,
    branch character varying(40),
    nip character varying(10),
    regon character varying(9),
    city character varying(40) NOT NULL,
    address character varying(60) NOT NULL,
    postcode character varying(6) NOT NULL,
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.families
CREATE TABLE wbz.families
(
    id serial PRIMARY KEY,
    declarant character varying(50) NOT NULL,
    lastname character varying(30) NOT NULL,
    members smallint NOT NULL,
    city character varying(40) NOT NULL,
    address character varying(60) NOT NULL,
    postcode character varying(6) NOT NULL,
    status boolean NOT NULL DEFAULT true,
    c_sms boolean NOT NULL DEFAULT false,
    c_call boolean NOT NULL DEFAULT false,
    c_email boolean NOT NULL DEFAULT false,
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.documents
CREATE TABLE wbz.documents
(
    id serial PRIMARY KEY,
    type character varying(3) NOT NULL,
    name character varying(50) NOT NULL,
    store_id integer NOT NULL,
    contractor_id integer NOT NULL,
    dateissue date NOT NULL DEFAULT now(),
    status smallint NOT NULL DEFAULT 0,
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT documents_headers_store_fkey FOREIGN KEY (store_id)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT documents_headers_contractor_fkey FOREIGN KEY (contractor_id)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.documents_positions
CREATE TABLE wbz.documents_positions
(
    id bigserial PRIMARY KEY,
    document_id integer NOT NULL,
    pos smallint NOT NULL,
    article_id integer NOT NULL,
    quantity numeric(15,3) NOT NULL DEFAULT 0,
    net numeric(15,2) NOT NULL DEFAULT 0,
    tax numeric(15,2) NOT NULL DEFAULT 0,
    CONSTRAINT documents_positions_document_fkey FOREIGN KEY (document_id)
        REFERENCES wbz.documents (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT documents_positions_article_fkey FOREIGN KEY (article_id)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.distributions
CREATE TABLE wbz.distributions
(
    id serial PRIMARY KEY,
    name character varying(50),
    datereal date,
    status smallint NOT NULL DEFAULT 0,
    is_archival boolean NOT NULL,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.distributions_positions
CREATE TABLE wbz.distributions_positions
(
    id bigserial PRIMARY KEY,
    distribution_id integer NOT NULL,
    pos smallint,
    family_id integer NOT NULL,
    members smallint NOT NULL,
    store_id integer NOT NULL,
    article_id integer NOT NULL,
    quantity numeric(15,3) NOT NULL DEFAULT 0,
    status smallint NOT NULL,
    CONSTRAINT distributions_positions_distribution_fkey FOREIGN KEY (distribution_id)
        REFERENCES wbz.distributions (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT distributions_positions_family_fkey FOREIGN KEY (family_id)
        REFERENCES wbz.families (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT distributions_positions_store_fkey FOREIGN KEY (store_id)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT distributions_positions_article_fkey FOREIGN KEY (article_id)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.employees
CREATE TABLE wbz.employees
(
    id serial PRIMARY KEY,
    email character varying(100),
    phone character varying(30),
    forename character varying(30) NOT NULL,
    lastname character varying(30) NOT NULL,
    city character varying(40),
    address character varying(60),
    postcode character varying(6),
    department character varying(40),
    position character varying(40),
    is_archival boolean NOT NULL DEFAULT false,
    comment text,
    icon_id integer,
    CONSTRAINT groups_icon_fkey FOREIGN KEY (icon_id)
        REFERENCES wbz.icons (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.vehicles
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
