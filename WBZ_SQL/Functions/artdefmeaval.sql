CREATE OR REPLACE FUNCTION wbz.artdefmeaval(
	_article integer,
	_amount double precision)
    RETURNS double precision
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
AS $BODY$
BEGIN
   RETURN _amount * coalesce(nullif((select converter FROM wbz.articles_measures WHERE article = _article AND "default"=true),0),1);
END
$BODY$;