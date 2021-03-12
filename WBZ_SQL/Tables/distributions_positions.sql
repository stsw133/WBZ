CREATE TABLE wbz.distributions_positions
(
    id bigserial PRIMARY KEY,
    distribution integer NOT NULL,
    "position" smallint,
    family integer NOT NULL,
    members smallint NOT NULL,
    store integer NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    status smallint NOT NULL,
    CONSTRAINT distributions_positions_distribution_fkey FOREIGN KEY (distribution)
        REFERENCES wbz.distributions (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT distributions_positions_family_fkey FOREIGN KEY (family)
        REFERENCES wbz.families (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT distributions_positions_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT distributions_positions_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)