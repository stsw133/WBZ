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
)