Test SSL CA
===========

### Creating Root CA cert

*NB:* password `basho`

All of these `openssl` commands should be run from the `test-ca`
directory and should use the `-config conf/openssl.conf` argument.

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

### Export `riakuser` client cert in `PFX` format

This will create a file with the public *and* private key. Useful for importing
into the Windows certificate store or as a standalone file for client
certificate use. When exporting do *NOT* set an export password, or you will have
to supply it when importing into the Win cert store.

```
openssl pkcs12 -export \
    -in certs/riakuser-client-cert.pem \
    -inkey private/riakuser-client-cert-key.pem \
    -out certs/riakuser-client-cert.pfx
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

### Generate revocation list

```
openssl ca -config conf/openssl.conf \
    -keyfile private/cakey.pem \
    -cert certs/cacert.pem \
    -gencrl -out crl/crl.pem
```

### Testing

Ensure that `riak-test` is an alias for `127.0.0.1` (or the appropriate
IP address) in your `hosts` file.

```
curl -vvv4 --cacert ~/Projects/basho/riak-dotnet-client/tools/test-ca/certs/cacert.pem https://riak-test:10418/stats
```

### Resources

https://gist.github.com/lukebakken/ab4db1efa3e1847c7731

https://jamielinux.com/articles/2013/08/generate-certificate-revocation-list-revoke-certificates/
