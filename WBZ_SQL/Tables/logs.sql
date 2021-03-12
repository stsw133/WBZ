CREATE TABLE wbz.logs
(
    id bigserial PRIMARY KEY,
    "user" integer NOT NULL,
    module character varying(50) NOT NULL,
    instance integer NOT NULL,
    type smallint NOT NULL DEFAULT 1,
    content text NOT NULL,
    datetime timestamp(6) with time zone NOT NULL DEFAULT now(),
    CONSTRAINT logs_perms_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)