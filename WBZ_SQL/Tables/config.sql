CREATE TABLE wbz.config
(
	property character varying(50) PRIMARY KEY,
    value character varying(100)
);

INSERT INTO wbz.config (property, value) VALUES
    ('VERSION', '1.2.0'),
    ('LOGS_ENABLED', '0'),
    ('EMAIL_HOST', 'smtp.gmail.com'),
	('EMAIL_PORT', '587'),
	('EMAIL_ADDRESS', 'wbz.email.testowy@gmail.com'),
	('EMAIL_PASSWORD', '');