CREATE OR REPLACE FUNCTION wbz.artdefmeacon(
	_article integer)
    RETURNS double precision
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN coalesce(nullif((select converter FROM wbz.articles_measures WHERE article = _article AND "default"=true),0),1);
END
$BODY$;