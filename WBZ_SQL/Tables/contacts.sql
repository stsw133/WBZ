CREATE TABLE wbz.contacts
(
    id serial PRIMARY KEY,
    module character varying(50) NOT NULL,
    instance integer NOT NULL,
    email character varying(60),
    phone character varying(12),
    forename character varying(30),
    lastname character varying(30),
    "default" boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false
)