using System;
using System.IO;
using ExcelDataReader.Core.BinaryFormat;

namespace ExcelDataReader
{
    /// <summary>
    /// The ExcelReader Factory
    /// </summary>
    public static class ExcelReaderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ExcelBinaryReader"/> or <see cref="ExcelOpenXmlReader"/>
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="convertOADates">If <see langword="true"/> convert OA dates to <see cref="DateTime"/>. Only applicable to binary (xls) files.</param>
        /// <returns>The excel data reader.</returns>
        [Obsolete("Use CreateReader() with a configuration object instead")]
        public static IExcelDataReader CreateReader(Stream fileStream, bool convertOADates)
        {
            return CreateReader(fileStream, new ExcelReaderConfiguration()
            {
                ConvertOaDate = convertOADates,
            });
        }

        /// <summary>
        /// Creates an instance of <see cref="ExcelBinaryReader"/> or <see cref="ExcelOpenXmlReader"/>
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <returns>The excel data reader.</returns>
        public static IExcelDataReader CreateReader(Stream fileStream, ExcelReaderConfiguration configuration = null)
        { 
            var probe = new byte[8];
            fileStream.Read(probe, 0, probe.Length);
            fileStream.Seek(0, SeekOrigin.Begin);

            if (XlsWorkbook.IsCompoundDocument(probe) || XlsWorkbook.IsRawBiffStream(probe))
            {
                return CreateBinaryReader(fileStream, configuration);
            }

            // zip files start with 'PK'
            if (probe[0] == 0x50 && probe[1] == 0x4B)
            {
                return CreateOpenXmlReader(fileStream, configuration);
            }

            throw new NotSupportedException("Unknown file format");
        }

        /// <summary>
        /// Creates an instance of <see cref="ExcelBinaryReader"/>
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="convertOADate">If true oa dates will be converer to <see cref="DateTime"/>.</param>
        /// <returns>The excel data reader.</returns>
        [Obsolete("Use CreateBinaryReader() with a configuration object instead")]
        public static IExcelDataReader CreateBinaryReader(Stream fileStream, bool convertOADate)
        {
            return CreateBinaryReader(fileStream, new ExcelReaderConfiguration()
            {
                ConvertOaDate = convertOADate,
            });
        }

        /// <summary>
        /// Creates an instance of <see cref="ExcelBinaryReader"/>
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <returns>The excel data reader.</returns>
        public static IExcelDataReader CreateBinaryReader(Stream fileStream, ExcelReaderConfiguration configuration = null)
        {
            return new ExcelBinaryReader(fileStream, configuration);
        }

        /// <summary>
        /// Creates an instance of <see cref="ExcelOpenXmlReader"/>
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="configuration">The reader configuration -or- <see langword="null"/> to use the default configuration.</param>
        /// <returns>The excel data reader.</returns>
        public static IExcelDataReader CreateOpenXmlReader(Stream fileStream, ExcelReaderConfiguration configuration = null)
        {
            return new ExcelOpenXmlReader(fileStream, configuration);
        }
    }
}
