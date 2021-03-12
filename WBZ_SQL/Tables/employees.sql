CREATE TABLE wbz.employees
(
    id serial PRIMARY KEY,
    email character varying(60),
    phone character varying(16),
    forename character varying(50) NOT NULL,
    lastname character varying(30) NOT NULL,
    city character varying(40),
    address character varying(60),
    postcode character varying(6),
    department character varying(40),
    position character varying(40),
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea
)