CREATE TABLE wbz.notifications
(
    id serial PRIMARY KEY,
    "user" integer NOT NULL,
    content character varying(100) NOT NULL,
    read boolean NOT NULL DEFAULT false,
    datetime timestamp(6) with time zone NOT NULL DEFAULT now(),
    action character varying(20),
    CONSTRAINT notifications_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)