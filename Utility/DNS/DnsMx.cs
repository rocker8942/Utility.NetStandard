using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Utility
{
    public class DnsMx
    {
        [DllImport("dnsapi", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern int DnsQuery([MarshalAs(UnmanagedType.VBByRefStr)] ref string pszName, QueryTypes wType, QueryOptions options, int aipServers, ref IntPtr ppQueryResults, int pReserved);

        [DllImport("dnsapi", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void DnsRecordListFree(IntPtr pRecordList, int FreeType);

        public static string[] GetMXRecords(string domain)
        {
            var ptr1 = IntPtr.Zero;
            IntPtr ptr2;
            MXRecord recMx;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new NotSupportedException();
            var list1 = new ArrayList();
            var num1 = DnsQuery(ref domain, QueryTypes.DNS_TYPE_MX, QueryOptions.DnsQueryBypassCache, 0, ref ptr1, 0);
            //if (num1 != 0)
            //{
            //    throw new Win32Exception(num1);
            //}
            for (ptr2 = ptr1; !ptr2.Equals(IntPtr.Zero); ptr2 = recMx.pNext)
            {
                recMx = (MXRecord) Marshal.PtrToStructure(ptr2, typeof(MXRecord));
                if (recMx.wType == 15)
                {
                    var text1 = Marshal.PtrToStringAuto(recMx.pNameExchange);
                    if (text1 != null) list1.Add(text1);
                }
            }
            DnsRecordListFree(ptr2, 0);
            return (string[]) list1.ToArray(typeof(string));
        }

        private enum QueryOptions
        {
            DnsQueryAcceptTruncatedResponse = 1,
            DnsQueryBypassCache = 8,
            DnsQueryDontResetTtlValues = 0x100000,
            DnsQueryNoHostsFile = 0x40,
            DnsQueryNoLocalName = 0x20,
            DnsQueryNoNetbt = 0x80,
            DnsQueryNoRecursion = 4,
            DnsQueryNoWireQuery = 0x10,
            DnsQueryReserved = -16777216,
            DnsQueryReturnMessage = 0x200,
            DnsQueryStandard = 0,
            DnsQueryTreatAsFqdn = 0x1000,
            DnsQueryUseTcpOnly = 2,
            DnsQueryWireOnly = 0x100
        }

        private enum QueryTypes
        {
            DNS_TYPE_MX = 15
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MXRecord
        {
            public readonly IntPtr pNext;
            public readonly string pName;
            public readonly short wType;
            public readonly short wDataLength;
            public readonly int flags;
            public readonly int dwTtl;
            public readonly int dwReserved;
            public readonly IntPtr pNameExchange;
            public readonly short wPreference;
            public readonly short Pad;
        }
    }
}