CREATE TABLE wbz.users_permissions
(
	"user" integer NOT NULL,
    perm character varying(50) NOT NULL,
    CONSTRAINT users_permissions_user_fkey FOREIGN KEY ("user")
        REFERENCES wbz.users (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)
