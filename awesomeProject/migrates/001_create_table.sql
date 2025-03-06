-- 001_create_table.sql
CREATE TABLE IF NOT EXISTS ioc_data (
                                        id UUID PRIMARY KEY,
                                        source String,
                                        first_seen DateTime,
                                        last_seen DateTime,
                                        type String,
                                        value String,
                                        tags String,
                                        additional_data String
);
