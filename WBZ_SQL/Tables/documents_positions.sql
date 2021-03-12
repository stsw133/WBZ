CREATE TABLE wbz.documents_positions
(
    id bigserial PRIMARY KEY,
    document integer NOT NULL,
    "position" smallint NOT NULL,
    article integer NOT NULL,
    amount numeric(15,3) NOT NULL DEFAULT 0,
    cost numeric(16,2) NOT NULL DEFAULT 0,
    tax numeric(16,2) NOT NULL DEFAULT 0,
    CONSTRAINT documents_positions_document_fkey FOREIGN KEY (document)
        REFERENCES wbz.documents (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT documents_positions_article_fkey FOREIGN KEY (article)
        REFERENCES wbz.articles (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
)