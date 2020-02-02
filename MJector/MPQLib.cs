using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJector
{
    using System.Runtime.InteropServices;
    using System.IO;

    public class MPQLib
    {
        [DllImport("MPQlib.dll", EntryPoint = "A3")] public static extern bool MPQOpenArchive(string szMpqName, int dwPriority, int dwFlags, ref int phMPQ, bool bNoListFile);
        [DllImport("MPQlib.dll", EntryPoint = "A4")] public static extern bool MPQCloseArchive(int phMPQ);
        [DllImport("MPQlib.dll", EntryPoint = "B2")] public static extern bool MPQExtractFile(int phMPQ, string szToExtract, string szExtracted);
        [DllImport("MPQlib.dll", EntryPoint = "B4")] public static extern bool MPQCreateArchive(string szMapName, int dwCreationDisposion, int dwHashTableSize, ref int phMPQ, int dwBlockSize = 3, bool bNoListFile = false);
        [DllImport("MPQlib.dll", EntryPoint = "B5")] public static extern bool MPQAddFile(int hMPQ, string szFileName, string szArchiveName, int dwFlags, ref bool pbReplaced);
        [DllImport("MPQlib.dll", EntryPoint = "B7")] public static extern bool MPQRemoveFile(int hMPQ, string szFileName);
        [DllImport("MPQlib.dll", EntryPoint = "C1")] public static extern bool MPQHasFile(int phMPQ, string szFileName);
        [DllImport("MPQlib.dll", EntryPoint = "C7")] public static extern bool MPQCompactArchive(int hMPQ, string szListFile = null, long CompactCB = 0, long lpCallbackData = 0);
        //[DllImport("MPQlib.dll", EntryPoint = "C7")] public static extern bool MPQCompactArchive(int hMPQ, string szListFile = null, long CompactCB = 0, long lpCallbackData = 0);

        // Values for MpqCreateArchive
        public const int MIN_HASH_TABLE = 0x2;
        public const int MAX_HASH_TABLE = 0x40000;
        public const int MIN_BLOCK_SIZE = 0x1;
        public const int MAX_BLOCK_SIZE = 0xF;

        // Values for MpqOpenArchive
        public const int OPEN_NORMAL = 0x0; // Open normal archives.

        // Values for MpqOpenFile
        public const int OPEN_INTERNAL = 0x0; // Open files inside MPQ archive.
        public const int OPEN_LOCAL = -1; //Open local files.   

        // Constants for MpqCreateArchive
        public const int CREATE_NEW = 0x1;
        public const int CREATE_ALWAYS = 0x2;
        public const int OPEN_EXISTING = 0x3;

        // Flags for MpqAddFile
        public const int FILE_COMPRESS_MULTI = 0x200;
        public const int FILE_COMPRESS_PKWARE = 0x100;
    }
}
