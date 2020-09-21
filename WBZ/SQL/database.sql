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
    ('VERSION', '1.1.0'), --wersja bazy danych
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
    id serial NOT NULL,
    "user" integer NOT NULL,
    perm character varying(50) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT users_permissions_pkey PRIMARY KEY ("id"),
    CONSTRAINT users_permissions_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)
TABLESPACE pg_default;

INSERT INTO wbz.users_permissions ("user", perm) VALUES
    (1, 'admin'),
    (1, 'users_preview'), (1, 'users_save'), (1, 'users_delete'),
    (1, 'employees_preview'), (1, 'employees_save'), (1, 'employees_delete'),
    (1, 'documents_preview'), (1, 'documents_save'), (1, 'documents_delete'),
    (1, 'articles_preview'), (1, 'articles_save'), (1, 'articles_delete'),
    (1, 'stores_preview'), (1, 'stores_save'), (1, 'stores_delete'),
    (1, 'companies_preview'), (1, 'companies_save'), (1, 'companies_delete'),
    (1, 'families_preview'), (1, 'families_save'), (1, 'families_delete'),
    (1, 'distributions_preview'), (1, 'distributions_save'), (1, 'distributions_delete'),
    (1, 'attributes_classes_preview'), (1, 'attributes_classes_save'), (1, 'attributes_classes_delete'),
    (1, 'attachments_preview'), (1, 'attachments_save'), (1, 'attachments_delete'),
    (1, 'logs_preview'), (1, 'logs_save'), (1, 'logs_delete'),
    (1, 'stats_preview'), (1, 'stats_save'), (1, 'stats_delete'),
    (1, 'community_preview'), (1, 'community_save'), (1, 'community_delete');

-- Table: wbz.logs
CREATE TABLE wbz.logs
(
    id bigserial NOT NULL,
    "user" integer NOT NULL,
    module character varying(50) COLLATE pg_catalog."default" NOT NULL,
    instance integer NOT NULL,
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
    owner integer, --id grupy do której ta grupa nale¿y
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    CONSTRAINT groups_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.attributes_classes
CREATE TABLE wbz.attributes_classes
(
    id serial NOT NULL,
    module character varying(50) COLLATE pg_catalog."default" NOT NULL,
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    type character varying(20) COLLATE pg_catalog."default" NOT NULL, --typ zmiennej np. int / bool etc.
    "values" character varying(100) COLLATE pg_catalog."default", --wartoœci jakie mo¿e przyj¹æ atrybut (null jeœli dowolne)
    defvalue character varying(50) COLLATE pg_catalog."default",
    required boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
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
    read boolean NOT NULL DEFAULT false, --okreœla czy notyfikacja zosta³a przeczytana
    datetime timestamp(6) with time zone NOT NULL DEFAULT now(), --data wystawienia notyfikacji
    action character varying(20) COLLATE pg_catalog."default", --akcja odpalaj¹ca siê po klikniêciu
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
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
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
    amount numeric(15,3) NOT NULL DEFAULT 0, --iloœæ w kilogramach
    reserved numeric(15,3) NOT NULL DEFAULT 0, --iloœæ w kilogramach
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

-- Table: wbz.companies
CREATE TABLE wbz.companies
(
    id serial NOT NULL,
    codename character varying(20) COLLATE pg_catalog."default",
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    branch character varying(40) COLLATE pg_catalog."default",
    nip character varying(10) COLLATE pg_catalog."default" NOT NULL,
    regon character varying(9) COLLATE pg_catalog."default" NOT NULL,
    city character varying(40) COLLATE pg_catalog."default" NOT NULL,
    address character varying(60) COLLATE pg_catalog."default" NOT NULL,
    postcode character varying(6) COLLATE pg_catalog."default" NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    CONSTRAINT companies_pkey PRIMARY KEY (id)
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
    c_sms boolean NOT NULL DEFAULT false, --czy rodzina ma byæ powiadamiana wiadomoœciami sms
    c_call boolean NOT NULL DEFAULT false, --czy rodzina ma byæ powiadamiana przez rozmowê telefoniczn¹
    c_email boolean NOT NULL DEFAULT false, --czy rodzina ma byæ powiadamiana drog¹ poczty elektronicznej
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    CONSTRAINT families_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;

-- Table: wbz.documents
CREATE TABLE wbz.documents
(
    id serial NOT NULL,
    type character varying(50) COLLATE pg_catalog."default" NOT NULL, --np. 'invoice'
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    store integer NOT NULL,
    company integer NOT NULL,
    dateissue date NOT NULL DEFAULT now(), --data wystawienia dokumentu
    status smallint NOT NULL DEFAULT 0, --0 = w buforze, 1 = zatwierdzona, -1 = wycofana
    archival boolean NOT NULL DEFAULT false,
    comment text COLLATE pg_catalog."default",
    CONSTRAINT documents_headers_pkey PRIMARY KEY (id),
    CONSTRAINT documents_headers_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT documents_headers_company_fkey FOREIGN KEY (company)
        REFERENCES wbz.companies (id) MATCH SIMPLE
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
    amount numeric(15,3) NOT NULL DEFAULT 0, --iloœæ w kilogramach
    cost numeric(16,2) NOT NULL DEFAULT 0, --koszt netto w z³
    tax numeric(16,2) NOT NULL DEFAULT 0, --koszt podatku w z³
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
    datereal date, --data realizacji dystrybucji
    status smallint NOT NULL DEFAULT 0, --0 = w buforze, 1 = zatwierdzona, -1 = wycofana
    archival boolean NOT NULL,
    comment text COLLATE pg_catalog."default",
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
    amount numeric(15,3) NOT NULL DEFAULT 0, --iloœæ w kilogramach
    status smallint NOT NULL, --0 = nic / 1 = poinformowano / 2 = odebrano
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
    "user" integer,
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
    CONSTRAINT employees_pkey PRIMARY KEY (id),
    CONSTRAINT employees_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE SET NULL
        NOT VALID
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
