CREATE TABLE wbz.vehicles
(
    id serial PRIMARY KEY,
    register character varying(20) NOT NULL UNIQUE,
    brand character varying(40),
    model character varying(60),
    capacity decimal(18,3),
    forwarder integer,
    driver integer,
    prodyear integer,
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea,
    CONSTRAINT vehicles_headers_forwarder_fkey FOREIGN KEY (forwarder)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT vehicles_headers_driver_fkey FOREIGN KEY (driver)
        REFERENCES wbz.employees (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)