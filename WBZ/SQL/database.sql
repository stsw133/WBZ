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
GRANT ALL ON SCHEMA wbz TO postgres;
GRANT ALL ON SCHEMA wbz TO wbz_user;

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
    username character varying(20) NOT NULL UNIQUE,
    password text NOT NULL,
    email character varying(60) NOT NULL UNIQUE,
    phone character varying(16),
    forename character varying(30),
    lastname character varying(30),
    blocked boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false
);

-- Table: wbz.users_permissions
CREATE TABLE wbz.users_permissions
(
    "user" integer NOT NULL,
    perm character varying(50) NOT NULL,
    CONSTRAINT users_permissions_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
);

-- Table: wbz.logs
CREATE TABLE wbz.logs
(
    id bigserial PRIMARY KEY,
    "user" integer NOT NULL,
    module character varying(50) NOT NULL,
    instance integer NOT NULL,
    type smallint NOT NULL DEFAULT 1,
    content text NOT NULL,
    datetime timestamp(6) with time zone NOT NULL DEFAULT now(),
    CONSTRAINT logs_perms_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.attachments
CREATE TABLE wbz.attachments
(
    id bigserial PRIMARY KEY,
    "user" integer NOT NULL,
    module character varying(50) NOT NULL,
    instance integer NOT NULL,
    name character varying(50) NOT NULL,
    file bytea NOT NULL,
    comment text,
    CONSTRAINT attachments_perms_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.icons
CREATE TABLE wbz.icons
(
    id bigserial PRIMARY KEY,
    module character varying(50) NOT NULL,
    name character varying(50) NOT NULL,
    "format" character varying(10) NOT NULL,
    "path" character varying(250) NOT NULL,
    height integer NOT NULL,
    width integer NOT NULL,
    size integer NOT NULL,
    file bytea NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    comment text
);

-- Table: wbz.groups
CREATE TABLE wbz.groups
(
    id serial PRIMARY KEY,
    module character varying(50) NOT NULL,
    name character varying(50) NOT NULL,
    instance integer,
    owner integer,
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
);

-- Table: wbz.attributes_classes
CREATE TABLE wbz.attributes_classes
(
    id serial PRIMARY KEY,
    module character varying(50) NOT NULL,
    name character varying(50) NOT NULL,
    type character varying(20) NOT NULL,
    "values" character varying(100),
    defvalue character varying(50),
    required boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
);

-- Table: wbz.attributes
CREATE TABLE wbz.attributes
(
    id bigserial PRIMARY KEY,
    instance integer NOT NULL,
    class integer NOT NULL,
    value character varying(50) NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_class_fkey FOREIGN KEY (class)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Table: wbz.attributes
CREATE TABLE wbz.attributes_values
(
    id bigserial PRIMARY KEY,
    class integer NOT NULL,
    value character varying(50) NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_values_class_fkey FOREIGN KEY (class)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Table: wbz.contacts
CREATE TABLE wbz.contacts
(
    id serial PRIMARY KEY,
    module character varying(50) NOT NULL,
    instance integer NOT NULL,
    email character varying(60),
    phone character varying(12),
    forename character varying(30),
    lastname character varying(30),
    "default" boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false
);

-- Table: wbz.notifications
CREATE TABLE wbz.notifications
(
    id serial PRIMARY KEY,
    "user" integer NOT NULL,
    content character varying(100) NOT NULL,
    read boolean NOT NULL DEFAULT false,
    datetime timestamp(6) with time zone NOT NULL DEFAULT now(),
    action character varying(20),
    CONSTRAINT notifications_user_fkey FOREIGN KEY ("user")
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
    price numeric(16,2),
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
);

-- Table: wbz.articles_measures
CREATE TABLE wbz.articles_measures
(
    id serial PRIMARY KEY,
    article integer NOT NULL,
    name character varying(10) NOT NULL,
    converter double precision NOT NULL DEFAULT 1,
    "default" boolean NOT NULL DEFAULT false,
    CONSTRAINT articles_measures_article_fkey FOREIGN KEY (article)
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
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
);

-- Table: wbz.stores_articles
CREATE TABLE wbz.stores_articles
(
    id serial PRIMARY KEY,
    store integer NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    reserved numeric(15,3) NOT NULL DEFAULT 0,
    CONSTRAINT stores_articles_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT stores_articles_article_fkey FOREIGN KEY (article)
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
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
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
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
);

-- Table: wbz.documents
CREATE TABLE wbz.documents
(
    id serial PRIMARY KEY,
    type character varying(50) NOT NULL,
    name character varying(50) NOT NULL,
    store integer NOT NULL,
    contractor integer NOT NULL,
    dateissue date NOT NULL DEFAULT now(),
    status smallint NOT NULL DEFAULT 0,
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea,
    CONSTRAINT documents_headers_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT documents_headers_contractor_fkey FOREIGN KEY (contractor)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

-- Table: wbz.documents_positions
CREATE TABLE wbz.documents_positions
(
    id bigserial PRIMARY KEY,
    document integer NOT NULL,
    "position" smallint NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    cost numeric(16,2) NOT NULL DEFAULT 0,
    tax numeric(16,2) NOT NULL DEFAULT 0,
    CONSTRAINT documents_positions_document_fkey FOREIGN KEY (document)
        REFERENCES wbz.documents (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT documents_positions_article_fkey FOREIGN KEY (article)
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
    archival boolean NOT NULL,
    comment text,
    icon bytea
);

-- Table: wbz.distributions_positions
CREATE TABLE wbz.distributions_positions
(
    id bigserial PRIMARY KEY,
    distribution integer NOT NULL,
    "position" smallint,
    family integer NOT NULL,
    members smallint NOT NULL,
    store integer NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    status smallint NOT NULL,
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
);

-- Table: wbz.employees
CREATE TABLE wbz.employees
(
    id serial PRIMARY KEY,
    email character varying(60),
    phone character varying(16),
    forename character varying(50) NOT NULL,
    lastname character varying(30) NOT NULL,
    city character varying(40),
    address character varying(60),
    postcode character varying(6),
    department character varying(40),
    position character varying(40),
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
);

-- Table: wbz.vehicles
CREATE TABLE wbz.vehicles
(
    id serial PRIMARY KEY,
    register character varying(20) NOT NULL UNIQUE,
    brand character varying(40),
    model character varying(60),
    capacity decimal(18,3),
    forwarder integer,
    driver integer,
    prodyear integer,
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea,
    CONSTRAINT vehicles_headers_forwarder_fkey FOREIGN KEY (forwarder)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT vehicles_headers_driver_fkey FOREIGN KEY (driver)
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
