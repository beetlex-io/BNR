using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace BNR
{
    /// <summary>
    /// {N:ORDER/00000}
    /// </summary>
    [ParameterType("N")]
    public class SequenceParameter : IParameterHandler
    {
        private static MemoryMappedFile mSequenceFile;

        private static MemoryMappedViewAccessor mAccessor;

        private static byte[] mBuffer = new byte[64];

        private static Dictionary<string, SequenceItem> mSequenceItems;

        private static System.IO.FileStream mFileStream;

        private static int mRecordSize = 64;

        private static int mRecordCount = 1024 * 100;

        static SequenceParameter()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "sequence.data";
            if (!System.IO.File.Exists(filename))
            {
                mFileStream = System.IO.File.Open(filename, System.IO.FileMode.OpenOrCreate);
                byte[] data = new byte[mRecordSize * mRecordCount];
                mFileStream.Write(data, 0, mRecordSize * mRecordCount);
                mFileStream.Flush();
                mFileStream.Close();
            }
            mSequenceFile = MemoryMappedFile.CreateFromFile(filename);
            mAccessor = mSequenceFile.CreateViewAccessor(mRecordSize, 0, MemoryMappedFileAccess.ReadWrite);
            mSequenceItems = new Dictionary<string, SequenceItem>();
            Load();
        }

        public SequenceParameter()
        {

        }


        private SequenceItem GetSequenceItem(string key)
        {
            lock (mSequenceItems)
            {
                SequenceItem result;
                if (!mSequenceItems.TryGetValue(key, out result))
                {
                    result = new SequenceItem();
                    result.Index = mSequenceItems.Count;
                    result.Name = key;
                    mSequenceItems[key] = result;

                }
                return result;
            }

        }

        private static void Save(SequenceItem item)
        {
            lock (mAccessor)
            {
                string value = item.Name + "=" + item.Value.ToString();
                Int16 length = (Int16)Encoding.UTF8.GetBytes(value, 0, value.Length, mBuffer, 2);
                BitConverter.GetBytes(length).CopyTo(mBuffer, 0);
                mAccessor.WriteArray<byte>(item.Index * mRecordSize, mBuffer, 0, mBuffer.Length);
            }

        }

        private static void Load()
        {
            for (int i = 0; i < 1024; i++)
            {
                mAccessor.ReadArray<byte>(i * mRecordSize, mBuffer, 0, mBuffer.Length);
                int length = BitConverter.ToInt16(mBuffer, 0);
                if (length > 0)
                {
                    string value = Encoding.UTF8.GetString(mBuffer, 2, length);
                    string[] properties = value.Split('=');
                    SequenceItem item = new SequenceItem();
                    item.Index = i;
                    item.Name = properties[0];
                    item.Value = long.Parse(properties[1]);
                    mSequenceItems[item.Name] = item;
                }
                else
                    break;

            }
        }

        public void Execute(StringBuilder sb, string value)
        {
            string[] properties = value.Split('/');
            StringBuilder key = new StringBuilder();
            string[] items = RuleAnalysis.Execute(properties[0]);
            foreach (string p in items)
            {

                string[] sps = RuleAnalysis.GetProperties(p);
                IParameterHandler handler = null;
                if (Factory.Handlers.TryGetValue(sps[0], out handler))
                {
                    handler.Execute(key, sps[1]);
                }
            }

            SequenceItem item = GetSequenceItem(key.ToString());
            lock (item)
            {
                item.Value++;
                sb.Append(item.Value.ToString(properties[1]));
            }
           Save(item);

        }

        public class SequenceItem
        {




            public int Index { get; set; }

            public string Name { get; set; }

            public long Value { get; set; }


        }




        public BNRFactory Factory
        {
            get;
            set;
        }
    }
}
