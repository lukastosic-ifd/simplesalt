# Simple Salt

This is REST API that can help you to encode and decode string values using a simple single "salt" value that you can store somewhere safe (Google Cloud Secrets Manager?).

It is based on the HashIds project https://hashids.org/net/ and https://github.com/ullmark/hashids.net

**This is not supposed to be use to store important passwords because it is reversible and only bound to a single parameter to encode/decode data**

# How does this work

You can have different salt config for different input `type`.

Salts are stored as environment variables.

You have to configure salt mapping property either in `appsettings.json` or again as an environment variable `SALTMAPPING`.

Salt mapping is configured in the format:

`{salt_type1}:{env_variable1};{salt_type2}:{env_variable2};{salt_type3}:{env_vriable3}...`

For example:

`type1:SALT_TYPE1;type2:SALT_TYPE2`

This means that for `type1` you are supposed to have an environment variable `SALT_TYPE1` that contains the salt for the encoding/decoding.

## What can be the salt?

Salt can be any string (any length)

# Request-Response

## Encode

`GET /encode?value={value}&type={type}`

for example:

`GET /encode?value=abcdef&type=type1`

Response is simple encoded string

`"k22VQoDem8cObmLaZkGd"`

## Decode

`GET /decode?value={value}&type={type}`

for example:

`GET /decode?value=k22VQoDem8cObmLaZkGd&type=type1`

Response is simple decoded string:

`"abcdef"`

