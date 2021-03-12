CREATE TABLE wbz.distributions
(
    id serial PRIMARY KEY,
    name character varying(50),
    datereal date,
    status smallint NOT NULL DEFAULT 0,
    archival boolean NOT NULL,
    comment text,
    icon bytea
)