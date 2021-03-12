CREATE TABLE wbz.articles_measures
(
    id serial PRIMARY KEY,
    article integer NOT NULL,
    name character varying(10) NOT NULL,
    converter double precision NOT NULL DEFAULT 1,
    "default" boolean NOT NULL DEFAULT false,
    CONSTRAINT articles_measures_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)