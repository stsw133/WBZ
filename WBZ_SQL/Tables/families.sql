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
)