CREATE TABLE wbz.icons
(
    id bigserial PRIMARY KEY,
    module character varying(50) NOT NULL,
    name character varying(50) NOT NULL,
    "format" character varying(10) NOT NULL,
    "path" character varying(250) NULL,
    height integer NOT NULL,
    width integer NOT NULL,
    file bytea NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    comment text
)