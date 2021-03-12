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