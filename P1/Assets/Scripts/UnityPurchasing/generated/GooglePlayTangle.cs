// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("T2hC3+4BIeXHXUFAWNvD0qLu9p5cKUxGK0ekVAGwJmDbyjXXlWPeg6cVlrWnmpGevRHfEWCalpaWkpeUrvW0KJ2P6BQclY3OPvANA0tYFn0T2rjSYEJM+iWqH6u+F0ofXZFu7zzsVzLVEM9+uiqlj/EW9O/61ZidFZaYl6cVlp2VFZaWl1t2GXi4T63zG89/cXb39ueKs7o9uJEFapcZ16qBFQARWgyUwv1YeYYtumcqNuftNRIpSsaYMAT/VhoWsBEmVUptEONaGq6Hg0HDCIbjm03D2NVVFGUBL2l7eju+1ZeNcSO5Sj0/xsejBe1TxYDHBjqWhR1phSjDAJlMxfTkuNY5bTx9rfXQtrOoGyqln4ggIfvEAqf6Fhb3VzqGRJWUlpeW");
        private static int[] order = new int[] { 7,2,7,6,6,5,7,8,13,12,13,11,12,13,14 };
        private static int key = 151;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
