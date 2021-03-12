CREATE TABLE wbz.documents
(
    id serial PRIMARY KEY,
    type character varying(50) NOT NULL,
    name character varying(50) NOT NULL,
    store integer NOT NULL,
    contractor integer NOT NULL,
    dateissue date NOT NULL DEFAULT now(),
    status smallint NOT NULL DEFAULT 0,
    archival boolean NOT NULL DEFAULT false,
    comment text,
    icon bytea,
    CONSTRAINT documents_headers_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT documents_headers_contractor_fkey FOREIGN KEY (contractor)
        REFERENCES wbz.contractors (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)