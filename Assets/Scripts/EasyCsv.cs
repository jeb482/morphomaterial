using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EasyCsv {
    public class Csv {
        List<Row> Rows;
        int NumRows;
        int NumCols;

        

        public Csv()
        {
            Rows = new List<Row>();
        }

        public bool IsEmpty
        {
            get {
                foreach (var r in Rows)
                    if (!r.IsEmpty)
                        return false;
                return true;
            }
        }

        private void FillUntil(int i)
        {
            if (Rows.Count <= i)
            {
                for (int j = Rows.Count; j <= i; j++)
                {
                    Rows.Add(new Row());
                }
            }
        }

        public Row this[int i]
        {
            get
            {
                FillUntil(i);
                return Rows[i];
            }
            set
            {
                FillUntil(i);
                Rows[i] = value;
            }
        }

        public void WriteToFile(string filename)
        {
            using (var file = new StreamWriter(filename))
            {
                foreach (Row r in Rows)
                    file.Write(r.ToString()+"\n");
            }
        }

        public class Row
        {
            List<string> RowData;
            public int Length
            {
                get
                {
                    return RowData.Count;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    foreach (var s in RowData)
                        if (s != "")
                        {
                            return false;
                        }
                    return true;
                }
            }

            public Row()
            {
                RowData = new List<string>();
            }
            
            private void FillUntil(int i)
            {
                if (RowData.Count <= i)
                {
                    for (int j = RowData.Count; j <= i; j++)
                    {
                        RowData.Add("");
                    }
                }
            }

            public string this[int i]
            {
                get
                {
                    if (RowData.Count < i)
                        return "";
                    return RowData[i];
                }
                set
                {
                    FillUntil(i);
                    RowData[i] = value;
                }
            }

            public override string ToString()
            {
                string str = "";
                for (int i = 0; i < RowData.Count-1; i++)
                {
                    str += "\"" + RowData[i] + "\",";
                }
                if (RowData.Count > 0)
                    str += "\"" + RowData[RowData.Count - 1] + "\"";
                return str;
            }



        }
    
    }
	
}
