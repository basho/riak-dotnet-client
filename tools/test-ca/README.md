Test SSL CA
===========

### Creating Root CA cert

```
openssl req -new -x509 -days 3650 \
    -extensions v3_ca \
    -keyout private/cakey.pem \
    -out certs/cacert.pem \
    -config conf/openssl.conf
```

### Create cert request for `riakuser`

```
openssl req -new -config conf/openssl.conf \
    -nodes \
    -out req/riakuser-client-cert.pem \
    -keyout private/riakuser-client-cert-key.pem \
    -subj '/C=US/ST=WA/O=Basho Technologies/OU=Development/CN=riakuser/emailAddress=riakuser@basho.com'
```

### Sign certificate request with Root CA

```
openssl ca -verbose -batch \
    -config conf/openssl.conf \
    -out certs/riakuser-client-cert.pem \
    -infiles req/riakuser-client-cert.pem
```

### Create cert request and cert for `riak-test` server

```
openssl req -new -config conf/openssl.conf -nodes \
    -out req/riak-test.pem \
    -keyout private/riak-test-key.pem \
    -subj '/C=US/ST=WA/O=Basho Technologies/OU=Development/CN=riak-test/emailAddress=riakuser@basho.com'
```

```
openssl ca -batch -config conf/openssl.conf \
    -in req/riak-test.pem \
    -out certs/riak-test-cert.pem
```

### Testing

Ensure that `riak-test` is an alias for `127.0.0.1` (or the appropriate
IP address) in your `hosts` file.

```
curl -vvv4 --cacert ~/Projects/basho/CorrugatedIron/tools/test-ca/certs/cacert.pem https://riak-test:10418/stats
```

