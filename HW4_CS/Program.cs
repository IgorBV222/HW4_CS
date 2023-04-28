using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace HW4_CS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Задачи 1 и 2.\nЗадача 1. Написать программу, принимающую на вход файл \"task.txt\"\nсодержащий арифметические операции (сложение, вычитание, умножение, деление) в одно действие,\nвида:" +
                "23 * 45 = . И создающий(если нет)/добавляющий(если есть) файл \"solution.txt\" вида: 23 * 45 = 1035." +
                "\nПрограмма должна кореектно обрабатывать корректные данные и логировать ошибку в случае некорректных в выходной файл. " +
                "\nЗадача 2. Изменить код предыдущей программы таким образом," +
                "\nчтобы в она могла принимать произвольное имена нескольких файлов с заданиями и создавать несколько файлов с решениями." +
                "\nПример: команда \"program.exe task001.txt task002.txt task00n.txt\" \nдолжна создавать файлы решений task001_solution.txt task002_solution.txt task00n_solution.txt." +
                "\r\nВ случае отсутствия параметров программа должна действовать, как в задании 001,\nесли необходимые файлы отсутствуют, программа должна уведомить об этом и корректно завершить работу.\n" +
                "__________________________________________________________________________________________");
            List<string> wResult = new List<string>();
            var regexpLeft = new Regex(@"(-){0,1}(\d){1,}([\.\,]\d{1,}){0,}(\s)(\+|\-|\*|\/)");
            var regexpRigth = new Regex(@"(\+|\-|\*|\/)(\s)(-){0,1}(\d){1,}([\.\,]\d{1,}){0,}");
            var regNumber = new Regex(@"(-){0,1}(\d){1,}(\s*)([\.\,]\d{1,}){0,}");
            var regexpOperation = new Regex(@"(\+|\-|\*|\/)(\s)");
            var regOperation = new Regex(@"(\+|\-|\*|\/)");
            double numLeft = 0;
            double numRigth = 0;
            double result = 0;
            string arithmeticOperation = "";
            string path;
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    path = arg;
                    string line;
                    try
                    {
                        StreamReader sr = new StreamReader(path);
                        Console.WriteLine("\nДанные прочитаны из файла " + path);
                        line = sr.ReadLine();
                        while (line != null)
                        {
                            Console.WriteLine(line);
                            string tmpLine = Regex.Replace(line, @"\.", ",");
                            MatchCollection matchesLeft = regexpLeft.Matches(tmpLine);
                            MatchCollection matchesRigth = regexpRigth.Matches(tmpLine);
                            MatchCollection matchesOperation = regexpOperation.Matches(tmpLine);
                            foreach (Match match in matchesLeft)
                            {
                                MatchCollection numberLeft = regNumber.Matches(match.ToString());
                                numLeft = double.Parse(numberLeft[0].ToString());
                            }
                            foreach (Match match in matchesOperation)
                            {
                                MatchCollection operation = regOperation.Matches(match.ToString());
                                arithmeticOperation = operation[0].ToString();
                            }
                            foreach (Match match in matchesRigth)
                            {
                                MatchCollection numberRigth = regNumber.Matches(match.ToString());
                                numRigth = double.Parse(numberRigth[0].ToString());
                            }
                            switch (arithmeticOperation)
                            {
                                case "+": result = numLeft + numRigth; break;
                                case "-": result = numLeft - numRigth; break;
                                case "*": result = numLeft * numRigth; break;
                                case "/":
                                    if (numRigth != 0)
                                    {
                                        result = numLeft / numRigth;
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nОшибка:на ноль нельзя делить");
                                        result = -1;
                                    }; break;
                                default: result = -1; break;
                            }
                            string str = (numLeft.ToString() + " " + arithmeticOperation + " " + numRigth.ToString() + " = " + result.ToString());
                            wResult.Add(str);
                            line = sr.ReadLine();
                        }
                        sr.Close();                        
                        Console.ReadLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e.Message);
                    }
                    try
                    {
                        StreamWriter sw = new StreamWriter((path.Remove(path.Length - 4) + "_solution.txt"), true);
                        Console.WriteLine("\nДанные записаны в файл " + (path.Remove(path.Length - 4) + "_solution.txt"));
                        for (int i = 0; i < wResult.Count; i++)
                        {
                            Console.WriteLine(wResult[i]);
                            sw.WriteLine(wResult[i]);
                        }
                        wResult.Clear();
                        sw.Close();                       
                        Console.ReadKey();                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nException: " + e.Message);
                    }                    
                }
            }
            else
            {
                Console.WriteLine("\nФайл(ы) не указан(ы)!!!\n");
            }

            Console.WriteLine("Задача 3. Программы должна принимать на вход, как аргумент командной строки имя каталога в файловой системе" +
                "\nи выводить список файлов и папок этого каталога." +
                "\nВ случае отсутствия параметров программа должна вывести информацию о текущем каталоге.\r\n" +
                "______________________________________________________________________________________");

            string path1;
            try
            {
                path1 = args[0];
            }
            catch
            {
                Console.WriteLine("Параметры отсутствуют. Информация о текущем каталоге: \r\n");
                path1 = Environment.CurrentDirectory;
            }
            DirectoryInfo myDirInfo = new DirectoryInfo(path1);
            foreach (var item in myDirInfo.EnumerateDirectories())
            {
                Console.WriteLine($"Каталог {item}");
            }
            foreach (var item in myDirInfo.EnumerateFiles())
            {
                Console.WriteLine($"Файл {item} имеет размер {item.Length} байт, был изменён {item.LastWriteTime}");
            }         
            Console.ReadKey();

            Console.WriteLine("Задача 4. Есть каталог A где хранится файл с общим доступом\n" +
                "и каталог B, где хранятся копии этого файла (с суфиксом времени копирования) для возможности восстановления.\n" +
                "Программа должна проверять время изменения файла в каталоге А и создавать его копию в каталоге B,\n" +
                "если время его изменения позже, чем последнее время изменения в каталоге копий.\n" +
                "_______________________________________________________________________________");          

            string sourceDir = @"D:\System Files\Documents\IGOR\C#\ДЗ\HW4_CS\HW4_CS\bin\Debug\A";
            string backupDir = @"D:\System Files\Documents\IGOR\C#\ДЗ\HW4_CS\HW4_CS\bin\Debug\B";
            try
            {
                string[] faleList = Directory.GetFiles(sourceDir, "A.txt");               
                foreach (string fale in faleList)
                {
                    string fileName = fale.Substring(sourceDir.Length + 1); 
                    string lastModifiedFale = "";

                    DirectoryInfo myDirInfo1 = new DirectoryInfo(sourceDir);
                    foreach (var item in myDirInfo1.EnumerateFiles())
                    {
                        Console.WriteLine($"Файл {item} был изменён {item.LastWriteTime}");
                        lastModifiedFale = item.LastWriteTime.ToString("dd_MM_yyyy_HH_mm_ss");                        
                    }                    
                    string fileNameCopy = fileName.Remove(fileName.Length - 4) + "_" + lastModifiedFale + ".txt";                   
                    try
                    {                        
                        File.Copy(Path.Combine(sourceDir, fileName), Path.Combine(backupDir, fileNameCopy));
                        Console.WriteLine($"Создана архивная копия: {fileNameCopy}");
                    }                    
                    catch (IOException copyError)
                    {
                        Console.WriteLine(copyError.Message);
                    }
                }
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                Console.WriteLine(dirNotFound.Message);
            }
        }
    }
}