CREATE TABLE wbz.attributes
(
    id bigserial PRIMARY KEY,
    instance integer NOT NULL,
    class integer NOT NULL,
    value character varying(50) NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_class_fkey FOREIGN KEY (class)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)