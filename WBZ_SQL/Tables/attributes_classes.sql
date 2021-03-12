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
)