using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Linq;

namespace Factory
{

    #region INTERFACES
    public interface IConfig
    {
        string get_Type { get; set; }
        string get_Prefix { get; set; }
        string get_FilePath { get; set; }
    }

    public interface IDataLoader : IConfig, IDataProcessor
    {
        void LoadData();
    }

    public interface IDataProcessor
    {
        void ProcessData();

        void AddPrefix();
    }
    #endregion

    //Enter class
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process process = new Process();
                process.LoadData();
                process.ProcessData();
                process.AddPrefix();
                process.ProcessData();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: {0}.", e.Message);
                Console.ResetColor();
            }
            Console.ReadLine();
        }
    }

    //Class for process data
    class Process : IDataProcessor
    {
        #region FIELDS
        //<summary>
        //Fileds
        //</summary>
        private LOG log = new LOG();
        public string get_Type { get; set; }
        public string get_Prefix { get; set; } = "prcss01_";
        public string get_FilePath { get; set; }
        List<Field> list = new List<Field>();
        public delegate void delegat(Field f);
        #endregion

        #region PUBLIC METHOD
        public void AddPrefix()
        {
            //<summary>
            //UI for change prefix
            //</summary>

            Console.Write("Zadej novou hodnotu Prefixu: ");
            string old = get_Prefix;
            get_Prefix = Console.ReadLine();
            log.write(String.Format("Old value: {0}, new value {1}", old, get_Prefix));
        }

        public void LoadData()
        {
            //<summary>
            //Load data from file
            //</summary>

            //UI
            Console.Write("Zadej cestu k souboru: ");
            get_FilePath = Console.ReadLine();

            log.write("--- NACITANI DAT ---");
            log.write(String.Format("Cesta k souboru: {0}", get_FilePath));

            //Test existing a file
            if (!System.IO.File.Exists(get_FilePath))
            {
                log.write("File does not exist.");
                throw new Exception("File does not exist");
            }

            //File will be save in string field
            string[] file = System.IO.File.ReadAllLines(get_FilePath);

            foreach (string row in file)
            {
                try
                {
                    //Int test
                    int? inte = Convert.ToInt32(row);
                    //test for Data Type MaxValue
                    if (inte < Int32.MaxValue)
                    {
                        list.Add(new Field { number = (int)inte, write = 1 });
                        log.write(String.Format("Pridan int: \n {0}", row));
                    }
                    else
                    {
                        log.write(String.Format("Int nepridan: \n {0}", row));
                    }
                }
                catch
                {
                    try
                    {
                        //Float test
                        float? fl = Convert.ToSingle(Double.Parse(row, CultureInfo.InvariantCulture));
                        //test for Data Type MaxValue
                        if (fl < Single.MaxValue)
                        {
                            list.Add(new Field { single = (float)fl, write = 2 });
                            log.write(String.Format("Pridan float: \n {0}", row));
                        }
                        else
                        {
                            log.write(String.Format("Float nepridan: \n {0}", row));
                        }
                    }
                    catch
                    {
                        //All other data type will be set as string
                        list.Add(new Field { value = row, write = 3 });
                        log.write(String.Format("Pridan string: \n {0}", row));
                    }
                }
            }
        }


        public void ProcessData()
        {
            //<summary>
            //Write and save Log information
            //</summary>
            log.write("--- START PROGRAMU ---");

            //If for empty list -> first has to be call method LoadData()
            if (list.Count == 0)
            {
                log.write("No data");
                throw new Exception("No data");
            }

            //Step by step processing data in list feld with switch
            foreach (Field f in list)
            {
                switch (f.write)
                {
                    case 1:
                        process_int(f);
                        log.write(String.Format("Zpracovani int: \n {0}", f.number));
                        break;
                    case 2:
                        process_float(f);
                        log.write(String.Format("Zpracovani float: \n {0}", f.single));
                        break;
                    case 3:
                        process_string(f);
                        log.write(String.Format("Zpracovani string: \n {0}", f.value));
                        break;
                }
            }
            log.write("--- KONEC PROGRAMU ---");
            write();
            LogWrite();
        }

        #endregion

        #region OUT MESSAGES
        private void write()
        {
            //<summary>
            //Write and save OUT informations
            //</summary>
            StreamWriter stream = new StreamWriter("C:\\OUT.txt");
            Console.WriteLine("----------- OUT -----------");
            foreach (Field f in list)
            {
                Console.WriteLine(f.ToString());
                stream.WriteLine(f.ToString());
            }
            stream.Close();
        }

        public void LogWrite()
        {
            //<summary>
            //Write and save Log informations
            //</summary>
            StreamWriter stream = new StreamWriter("C:\\LOG.txt");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("----------- LOG -----------");
            foreach (string f in log)
            {
                Console.WriteLine(f.ToString());
                stream.WriteLine(f.ToString());
            }
            stream.Close();
            Console.ResetColor();
        }
        #endregion

        #region PROCESS DATA INT,FLOAT,STRING
        private void process_int(Field f)
        {
            //<summary>
            //Process int data type.
            //</summary>
            f.number += 1000;
            string format = String.Format("{0}{1}", this.get_Prefix, f.number);
            f.value = format;
        }

        private void process_float(Field f)
        {
            //<summary>
            //Process float data type.
            //</summary>
            f.single += 0.5f;
            f.number = (int)f.single;
            process_int(f);
            string format = String.Format("{0}{1}", "FLOAT_", f.value);
            f.value = format;
        }

        private void process_string(Field f)
        {
            //<summary>
            //Process string data type
            //</summary>
            List<Sort> sort = new List<Sort>();
            string[] split = f.value.Split(' ');
            for (int i = 0; i < split.Length; i++)
            {
                sort.Add(new Sort(split[i]));
            }
            sort.Sort((x, y) => x.value.CompareTo(y.value));
            string join = get_Prefix;
            foreach (Sort s in sort)
            {
                join += s.ToString();
                join += " ";
            }
            f.value = join;
        }
        #endregion
    }

    #region STRUCTURE
    //Structure for log
    class LOG : IEnumerable
    {
        List<string> log = new List<string>();
        public void write(string value)
        {
            log.Add(value);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)log).GetEnumerator();
        }
    }

    //Structure for data field
    class Field
    {
        public string value { get; set; }
        public int number { get; set; }
        public float single { get; set; }

        public int write { get; set; }

        public override string ToString()
        {
            return value;
        }
    }

    class Sort : IEnumerable, IComparable
    {
        public string value { get; set; }

        public Sort(string value)
        {
            this.value = value;
        }


        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public override string ToString()
        {
            return value;
        }

        public int CompareTo(object obj)
        {
            return value.CompareTo(obj);
        }
    }
    #endregion
}
