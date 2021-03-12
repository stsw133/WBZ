CREATE TABLE wbz.users
(
	id serial PRIMARY KEY,
    username character varying(20) NOT NULL UNIQUE,
    password text NOT NULL,
    email character varying(60) NOT NULL UNIQUE,
    phone character varying(16),
    forename character varying(30),
    lastname character varying(30),
    blocked boolean NOT NULL DEFAULT false,
    archival boolean NOT NULL DEFAULT false
)
