CREATE TABLE wbz.stores_articles
(
    id serial PRIMARY KEY,
    store integer NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    reserved numeric(15,3) NOT NULL DEFAULT 0,
    CONSTRAINT stores_articles_store_fkey FOREIGN KEY (store)
        REFERENCES wbz.stores (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT stores_articles_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
)