﻿#region

using System.Linq;
using EvilDICOM.Core.Enums;
using L = EvilDICOM.Core.Logging.EvilLogger;

#endregion

namespace EvilDICOM.Core.IO.Reading
{
    /// <summary>
    ///     This class can read the DICOM preamble consisting of 128 null bits followed by the ASCII characters DICM.
    /// </summary>
    public static class DICOMPreambleReader
    {
        /// <summary>
        ///     Reads the first 132 bits of a file to check if it contains the DICOM preamble.
        /// </summary>
        /// <param name="dr">a stream containing the bits of the file</param>
        /// <returns>a boolean indicating whether or not the DICOM preamble was in the file</returns>
        public static bool Read(DICOMBinaryReader dr)
        {
            if (dr.StreamLength > 132)
            {
                var nullPreamble = dr.Take(128);
                if (nullPreamble.Any(b => b != 0x00))
                    L.Instance.Log("Missing 128 byte null byte preamble.", LogPriority.WARNING);
                //READ D I C M
                var dcm = dr.Take(4);
                if (dcm[0] != 'D' || dcm[1] != 'I' || dcm[2] != 'C' || dcm[3] != 'M')
                {
                    L.Instance.Log("Missing characters D I C M in bits 128-131.", LogPriority.WARNING);
                    dr.StreamPosition -= 132; //Rewind
                    return false;
                }
            }
            return true;
        }
    }
}