Riak .NET Client and Riak Security
==================================

# SSL Certs Setup

* A SSL Certificate Authority must be set up to sign your Riak node
certificates and client certificates (if using them), *OR* the use of a
well-known CA like Verisign.

* An SSL certificate must be created for each of your Riak cluster
nodes. The certificate's `CN` (Common Name) must *exactly* match the
fully-qualified domain name for the server as used by your client.

* (optional) SSL client certificates must be created if you are using
that method of authentication (as opposed to password authentication).
They should be in `PFX` format containing the certificate's public and
private key for use on Windows. The `CN` part of the certificate's
subject must *exactly* match the user name configured for authentication
in Riak (see the [Basho documentation](http://docs.basho.com/riak/2.0.2/ops/running/authz/)).

# Riak Cluster Setup

Please see the [Basho
documentation](http://docs.basho.com/riak/2.0.2/ops/running/authz/) for
instructions on how to configure and enable Riak Security. Most
importantly, if you use your own Certificate Authority please ensure
that the CA's root certificate (public key part) is installed on each
Riak node with Riak configured to use that via `ssl.cacertfile` in
`riak.conf`.

*Note:* at this time, the following `riak.conf` settings are required to
enable TLSv1:

```
tls_protocols.sslv3 = off
tls_protocols.tlsv1 = on
tls_protocols.tlsv1.1 = on
tls_protocols.tlsv1.2 = on
```

Additionally, this setting can be used to disable certificate revocation
list checking:

```
check_crl = off
```

# Windows Setup

## Certificate Installation

You can either distribute your Root CA certificate (public key part) as
a file or install it to every client machines' "Trusted Root
Certification Authorities" store. If you don't wish this to be global,
it can be installed to the "Current User" store of the user as which
your Riak client code runs.

If you are using client certificates, you can distribute them as files
or install them to the "Personal" location in the "Local Computer" or
"Current User" store. Since a client certificate by necessity includes
the private key, permission to read the private key must be given to the
appropriate users. The Certificates MMC snap-in can be used to do this
([docs](http://technet.microsoft.com/en-us/library/ee662329.aspx)) as
well as the `winhttpcertcfg` utility
([docs](http://blogs.technet.com/b/operationsguy/archive/2010/11/29/provide-access-to-private-keys-commandline-vs-powershell.aspx)).

## Riak Client Configuration

Here is a complete configuration for a multiple node cluster using
Riak Security:

```xml
<configuration>
  <configSections>
    <section name="riakConfiguration" type="RiakClient.Config.RiakClusterConfiguration, RiakClient" />
  </configSections>
  <riakConfiguration nodePollTime="5000" defaultRetryWaitTime="200" defaultRetryCount="3">
    <authentication username="riakuser" password="Test1234"
       certificateAuthorityFile="full_or_relative_path_to\cacert.pem"
       clientCertificateFile="full_or_relative_path_to\riakuser-client-cert.pfx"
       clientCertificateSubject="E=riakuser@myorg.com, CN=riakuser, OU=Development, O=Basho Technologies, S=WA, C=US"
       checkCertificateRevocation="false" />
    <nodes>
      <node name="node1"
            hostAddress="riak-node1.mydomain.internal"
            pbcPort="8097" poolSize="16" />
      <node name="node2"
            hostAddress="riak-node2.mydomain.internal"
            pbcPort="8097" poolSize="16" />
      <node name="node3"
            hostAddress="riak-node3.mydomain.internal"
            pbcPort="8097" poolSize="16" />
    </nodes>
  </riakConfiguration>
</configuration>
```

### `authentication` element

* `username` - this is a required attribute that will be used to
authenticate the client. It must exactly match the user name configured
via the `riak-admin security` command on your Riak cluster.

* `password` - this is an optional attribute that is used when client
certificates are not configured.

* `clientCertificateAuthorityFile` - this attribute provides a full or
relative path to the certificate containing the public key portion of
your certificate authority's root cert. If you are using a commercial CA
this attribute can be removed.

* `clientCertificateFile` - this attribute provides a full or relative
path to the client certificate you are using. If this attribute is used
the `password` attribute can be removed.

* `clientCertificateSubject` - this attribute is used to load client
certificates from the Windows certificate store. It must *exactly* match
the certificates' subject. If you are distributing client certificates
as files this attribute can be removed.

* `clientCertificateRevocation` - this attribute should be set to `true`
if you wish for certificate revocation lists to be checked.

### Other

* The SSL certificates for the three Riak nodes must have `CN` values in
their subject that *exactly* match the host names
(`riak-node1.mydomain.internal`, for instance). Without this the Riak
client code can't correctly verify the server for secure communications.

