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
)