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
)