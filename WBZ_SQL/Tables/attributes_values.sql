CREATE TABLE wbz.attributes_values
(
    id bigserial PRIMARY KEY,
    class integer NOT NULL,
    value character varying(50) NOT NULL,
    archival boolean NOT NULL DEFAULT false,
    CONSTRAINT attributes_values_class_fkey FOREIGN KEY (class)
        REFERENCES wbz.attributes_classes (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)