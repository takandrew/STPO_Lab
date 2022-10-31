using LiveCharts.Wpf.Charts.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace STPO_Lab1.Model
{
    public class DataProccessing
    {
        private decimal[] dCoeffs;

        int FillCoeffsArray(String str)
        {
            string[] tmpCoeffs = str.Trim().Split(' ');
            int numOfCoeffs = tmpCoeffs.Length;
            dCoeffs = new decimal[tmpCoeffs.Length];
            for (int i = 0; i < tmpCoeffs.Length; i++)
            {
                int c1 = 0, c2 = 0; ;
                for (int j = 0; j < tmpCoeffs[i].Length; j++)
                {
                    if (tmpCoeffs[i].Length == 1 && (tmpCoeffs[i][j] == '-' || tmpCoeffs[i][j] == ','))
                    {
                        MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    if (!Char.IsDigit(tmpCoeffs[i][j]) && (tmpCoeffs[i][j] != '-' && tmpCoeffs[i][j] != ','))
                    {
                        MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    if (tmpCoeffs[i][j] == '-')
                    {
                        c1++;
                    }
                    if (tmpCoeffs[i][j] == ',')
                    {
                        c2++;
                    }
                }
                if (c1 < 2 && c2 < 2)
                {
                    dCoeffs[i] = Convert.ToDecimal(tmpCoeffs[i]);
                }
                else
                {
                    MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return -1;
                }
            }
            return 0;
        }

        decimal RandomMeInterval(int testType)
        {
            decimal interval = 0;
            if (testType == 1)
            {
                Random rnd = new Random();
                interval = rnd.Next(1, 1000) * 0.00001M;
            }
            return interval;
        }

        String[] GetTestParams(int method, int testType)
        {
            String[] pars = new String[2];
            if (testType == 1)
                pars[0] = "Позитивный";
            else
                pars[0] = "Негативный";

            if (method == 1)
                pars[1] = "Парабола";
            else if (method == 2)
                pars[1] = "Трапеция";
            else if (method == 3)
                pars[1] = "Монте Карло";
            return pars;
        }

        bool IsInBounds(decimal res1, decimal res2, decimal eps)
        {
            if (Math.Abs(res1 - res2) <= eps)
                return true;
            else
                return false;
        }

        decimal IntegrateThis(decimal leftBorder, decimal rightBorder)
        {
            decimal result = 0;
            for (int j = 0; j < dCoeffs.Length; j++)
            {
                result += dCoeffs[j] * (decimal)((Math.Pow((double)rightBorder, j + 1) / (j + 1)) - (Math.Pow((double)leftBorder, j + 1) / (j + 1)));
            }
            return result;
        }

        //private void SaveBtn_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog sf = new SaveFileDialog();
        //    sf.Filter = "txt files (*.txt)|*.txt";
        //    sf.FilterIndex = 1;
        //    sf.RestoreDirectory = true;
        //    sf.CreatePrompt = true;
        //    sf.CheckPathExists = true;

        //    if (sf.ShowDialog() == DialogResult.OK)
        //    {
        //        FileStream fs;
        //        StreamWriter sw;
        //        try
        //        {
        //            String text = Convert.ToString(ResultsTB.Text);
        //            fs = new FileStream(sf.FileName, FileMode.Create);
        //            sw = new StreamWriter(fs);
        //            sw.Write(text);
        //            sw.Close();
        //            fs.Close();
        //            MessageBox.Show("Файл сохранен.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }
        //        catch (Exception exc)
        //        {
        //            MessageBox.Show(exc.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }
        //    }
        //}

        private string TalkWithProcess(Process process)
        {
            List<string> outputText = new List<string>();
            try
            {
                process.Start();
                StreamWriter streamWriter = process.StandardInput;
                StreamReader outputReader = process.StandardOutput;
                StreamReader errorReader = process.StandardError;
                string text = String.Empty;
                while (!outputReader.EndOfStream)
                {
                    outputText.Add(outputReader.ReadLine());
                    streamWriter.WriteLine(text);
                }

                while (!errorReader.EndOfStream)
                {
                    string errText = errorReader.ReadLine();
                    streamWriter.WriteLine(text);
                }

                streamWriter.Close();
                process.WaitForExit();

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return outputText[0];
        }
        
        public void ProccessData(ParameterValue parameterValue, int selectedTypeNum, int selectedErrorNum, 
            out List<decimal> parabolaList, out List<decimal> trapezeList, out List<decimal> monteCarloList, out string ResultsTB)
        {
            decimal leftBorder = 0, rightBorder = 0,interval = 0, eps = 0, resultCode2;
            int method = 0, errorType = 0;
            ResultsTB = String.Empty;
            string coeffs = String.Empty;
            parabolaList = new List<decimal>();
            trapezeList = new List<decimal>();
            monteCarloList = new List<decimal>();

            if (selectedTypeNum == 1)
            {
                FillCoeffsArray(parameterValue.CoeffString);
                for (int k = 0; k < 3; k++)
                {
                    interval = parameterValue.StarterStep;
                    for (int i = 0; i < parameterValue.TestCaseQuantity; i++)
                    {
                        interval += parameterValue.Increment;
                        String argv = " " + parameterValue.LeftBorder.ToString().Replace('.', ',') + " " + parameterValue.RightBorder.ToString().Replace('.', ',') + " " + interval.ToString().Replace('.',',') + " " + Convert.ToString(k + 1) + " " + parameterValue.CoeffString;

                        String output = "";

                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.CreateNoWindow = true;
                        startInfo.RedirectStandardOutput = true;
                        startInfo.RedirectStandardError = true;
                        startInfo.RedirectStandardInput = true;
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "Integral3x.exe";
                        startInfo.UseShellExecute = false;
                        startInfo.Arguments = argv;
                        process.StartInfo = startInfo;
                        decimal resultCode1;
                        try
                        {
                            output = TalkWithProcess(process);
                            resultCode1 = 0;
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        String[] pars = new String[2];
                        pars = GetTestParams(k + 1, selectedTypeNum);
                        String[] getVal = output.Split(' ');
                        resultCode1 = (decimal)Convert.ToDouble(getVal[2].Remove(getVal[2].Length - 2).Replace('.', ','));

                        resultCode2 = IntegrateThis(parameterValue.LeftBorder, parameterValue.RightBorder);

                        double s = Convert.ToDouble(output.Replace("\n", "").Replace("S", "").Replace("=", "").Replace("\r", ""));
                        double resEps = Math.Abs(s - (double)resultCode2);

                        String result = "";
                        ResultsTB += "Тест " + (i + 1) + " " + pars[0] + "\r\nМетод: " + pars[1] + "\r\n" +
                         "Левая граница: " + parameterValue.LeftBorder + "\r\nПравая граница: " + parameterValue.RightBorder + "\r\nШаг интегрирования: " + interval +
                         "\r\nEPS: " + resEps + "\r\nIntegral3x: " + output.Replace('\n', ' ') + " | Oracle: S = " + resultCode2 + "\r\n" +
                         result + "\r\n\r\n";
                        
                        if (k == 0)
                            parabolaList.Add((decimal)Math.Round(resEps, 3));
                        else if (k == 1)
                            trapezeList.Add((decimal)Math.Round(resEps, 3));
                        else
                            monteCarloList.Add((decimal)Math.Round(resEps, 3));

                    }
                }
            }
            else if (selectedTypeNum == 2)
            {
                try
                {
                    errorType = Convert.ToInt32(selectedErrorNum + 1);
                    if (errorType == 0)
                    {
                        MessageBox.Show("Выберите тип ошибки для тестирования!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Данные в " + exc.StackTrace + " не валидны!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int resultCode1 = 0;
                string _leftBorder = String.Empty;
                string _rightBorder = String.Empty;
                string supposedError = String.Empty;
                string gotError = String.Empty;

                if (errorType == 1)
                {
                    method = 1;
                    _leftBorder = "эта_левая_граница_не_число";
                    rightBorder = 1;
                    eps = 1;
                    coeffs = "1 1 1 1 1";
                    interval = RandomMeInterval(selectedTypeNum);
                    supposedError = "Левая граница диапазона не является числом!";

                }
                else if (errorType == 2)
                {
                    method = 1;
                    _rightBorder = "эта_правая_граница_не_число";
                    leftBorder = 1;
                    eps = 1;
                    coeffs = "1 1 1 1 1";
                    interval = RandomMeInterval(selectedTypeNum);
                    supposedError = "Правая граница диапазона не является числом!";
                }
                else if (errorType == 3)
                {
                    method = 1;
                    leftBorder = 9;
                    rightBorder = 1;
                    eps = 1;
                    coeffs = "1 1 1 1 1";
                    interval = RandomMeInterval(selectedTypeNum);
                    supposedError = "Левая граница диапазона должна быть < правой границы диапазона!";
                }
                else if (errorType == 4)
                {
                    method = 1;
                    leftBorder = 1;
                    rightBorder = 5;
                    eps = 1;
                    coeffs = "1 1 1 1 1";
                    interval = 25;
                    supposedError = "Шаг интегрирования должен быть в пределах [0.000001; 0.5]";
                }
                else if (errorType == 5)
                {
                    method = 6;
                    leftBorder = 1;
                    rightBorder = 5;
                    eps = 1;
                    coeffs = "1 1 1 1 1";
                    interval = (decimal)0.05;
                    supposedError = "Четвертый параметр определяет метод интегрирования и должен быть в пределах [1; 3]";
                }
                else if (errorType == 6)
                {
                    method = 1;
                    leftBorder = 1;
                    rightBorder = 5;
                    eps = 1;
                    coeffs = "";
                    interval = RandomMeInterval(selectedTypeNum);
                    supposedError = "Число параметров не соответствует ожидаемому и должно быть, как минимум 5!";
                }

                string argv = String.Empty;
                if (errorType == 1 ) 
                    argv = " " + _leftBorder.ToString() + " " + rightBorder.ToString().Replace('.', ',') + " " + interval.ToString().Replace('.', ',') + " " + method.ToString() + " " + coeffs;
                else if (errorType == 2)
                    argv = " " + leftBorder.ToString().Replace('.', ',') + " " + _rightBorder.ToString() + " " + interval.ToString().Replace('.', ',') + " " + method.ToString() + " " + coeffs;
                else
                    argv = " " + leftBorder.ToString().Replace('.', ',') + " " + rightBorder.ToString().Replace('.', ',') + " " + interval.ToString().Replace('.', ',') + " " + method.ToString() + " " + coeffs;

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "Integral3x.exe";
                startInfo.UseShellExecute = false;
                startInfo.Arguments = argv;
                process.StartInfo = startInfo;
                gotError = TalkWithProcess(process);
                ResultsTB += "Левая граница: " + leftBorder.ToString() + "\r\nПравая граница: " + rightBorder.ToString() + "\r\nШаг интегрирования: " + interval.ToString() + "\r\nМетод: " + method.ToString() +
                             "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: " + supposedError + "\r\n" +
                             "Ответ от Integral3x.exe: " + gotError + "\r\n";
            }
        }
    }
}
