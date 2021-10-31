using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaSchnaff.Shared.DBModels
{
    public class ScanError
    {
        public ScanError(string error, string sourcePath)
        {
            Error = error;
            SourcePath = sourcePath;
        }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public ScanError()
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        {

        }

        public int Id { get; set; }
        public string Error {  get; set; }
        public string SourcePath { get; set; }
    }
}
