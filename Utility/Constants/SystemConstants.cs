namespace Utility
{
    /// <summary>
    ///     System constant to be used throughout a project
    /// </summary>
    public class SystemConstants
    {
        public const int Null = -1;
        public const char Delimiter = '|';
        public const string LineBreak = "\r\n";

        public const int Kilobyte = 1024;
        public const int Megabyte = 1048576;
        public const int Gigabyte = 1073741824;
        public const long Terabyte = 1099511627776;
        public const long Petabyte = 1125899906842624;

        public const long NoStartDate = 0;
        public const long NoExpiryDate = 999999;
    }

    public enum Status
    {
        Ok = 0,
        Error = 1,
        NoMatch = 2
    }
}