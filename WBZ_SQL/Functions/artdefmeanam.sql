CREATE OR REPLACE FUNCTION wbz.artdefmeanam(
	_article integer)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN coalesce((SELECT name FROM wbz.articles_measures WHERE article = _article AND "default"=true), 'kg');
END
$BODY$;