using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KinectServer
{


    class CsvFileWriter : CsvFileCommon, IDisposable
    {

        // Private members
        private StreamWriter Writer;
        private string path = "";
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Initializes a new instance of the CsvFileWriter class for the
        /// specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public CsvFileWriter(Stream stream)
        {
            Writer = new StreamWriter(stream);
        }

        /// <summary>
        /// Initializes a new instance of the CsvFileWriter class for the
        /// specified file path.
        /// </summary>
        /// <param name="path">The name of the CSV file to write to</param>
        public CsvFileWriter(string path)
        {
            Writer = File.AppendText(path);
            //Path = path;
        }

        /// <summary>
        /// Writes a row of columns to the current CSV file.
        /// </summary>
        /// <param name="columns">The list of columns to write</param>
        public void WriteRow(List<string> columns)
        {
            // Verify required argument
            if (columns == null)
                throw new ArgumentNullException("columns");

            string toWriteToFile = "";
            
            for (int i = 0; i < columns.Count; i++)
            {
                toWriteToFile += columns[i];

                if (i < columns.Count - 1)
                {
                    toWriteToFile += ",";
                }
            }

            Writer.WriteLine(toWriteToFile);
            
        }

        // Propagate Dispose to StreamWriter
        public void Dispose()
        {
            Writer.Close();
            //Writer.Dispose();
        }
    }
}
