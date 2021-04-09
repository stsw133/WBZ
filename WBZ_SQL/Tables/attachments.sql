CREATE TABLE wbz.attachments
(
    id bigserial PRIMARY KEY,
    "user" integer NOT NULL,
    module character varying(50)  NOT NULL,
    instance integer NOT NULL,
    name character varying(50) NOT NULL,
    file bytea NOT NULL,
    comment text,
    CONSTRAINT attachments_perms_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)