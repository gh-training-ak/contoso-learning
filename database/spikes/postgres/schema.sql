-- Spike only. Not applied anywhere. See issue #447.

CREATE SCHEMA learning;

CREATE TABLE learning.mentor
(
    mentor_id       uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    display_name    text        NOT NULL,
    headline        text,
    hourly_rate     numeric(10, 2) NOT NULL CHECK (hourly_rate >= 0),
    currency        char(3)     NOT NULL DEFAULT 'GBP',
    meeting_type    smallint    NOT NULL,
    city            text        NOT NULL,
    postcode        text        NOT NULL,
    location        point       NOT NULL,
    is_published    boolean     NOT NULL DEFAULT false,
    created_at      timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX mentor_published_rate_idx
    ON learning.mentor (is_published, hourly_rate)
    INCLUDE (display_name, currency, city);

-- The reason for the spike. Postgres gives us this for free, azure sql needs
-- the geography type and a different query shape.
CREATE INDEX mentor_location_idx ON learning.mentor USING gist (location);
