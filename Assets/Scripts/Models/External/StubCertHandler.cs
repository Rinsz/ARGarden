using UnityEngine.Networking;

namespace Models.External
{
    // TODO Issue cert for api and remove stub
    internal class StubCertHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}