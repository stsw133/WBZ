﻿CREATE TABLE wbz.stores
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
)