using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi
{
    internal sealed class HostConfig
    {
        public HostConfig(bool runSsl, string sslCertThumbprint, ushort port)
        {
            if (string.IsNullOrWhiteSpace(sslCertThumbprint)) throw new ArgumentNullException(nameof(sslCertThumbprint));

            RunSsl = runSsl;
            SslCertThumbprint = sslCertThumbprint;
            Port = port;
        }

        internal ushort Port { get; }
        internal bool RunSsl { get; }
        internal string SslCertThumbprint { get; }
    }
}