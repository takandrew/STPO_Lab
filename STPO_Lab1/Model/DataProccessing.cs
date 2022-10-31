using LiveCharts.Wpf.Charts.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                pars[0] = "POSITIVE";
            else
                pars[0] = "NEGATIVE";

            if (method == 1)
                pars[1] = "PARABOLIC";
            else if (method == 2)
                pars[1] = "TRAPEZIA";
            else if (method == 3)
                pars[1] = "MONTE CARLO";
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

        public void ProccessData(ParameterValue parameterValue, int selectedTypeNum, int selectedErrorNum, 
            out List<(decimal, decimal)> parabolaList, out List<(decimal, decimal)> trapezeList, out List<(decimal, decimal)> monteCarloList, out string ResultsTB)
        {
            decimal leftBorder = 0, rightBorder = 0,interval = 0, eps = 0, resultCode2;
            int method = 0, testType, errorType = 0;
            ResultsTB = String.Empty;
            string coeffs = String.Empty;
            parabolaList = new List<(decimal, decimal)>();
            trapezeList = new List<(decimal, decimal)>();
            monteCarloList = new List<(decimal, decimal)>();

            if (selectedTypeNum == 1)
            {
                FillCoeffsArray(parameterValue.CoeffString);
                for (int k = 0; k < 3; k++)
                {
                    interval = parameterValue.StarterStep;
                    for (int i = 0; i < parameterValue.TestCaseQuantity; i++)
                    {
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.CreateNoWindow = true;
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "Integral3x.exe";
                        startInfo.RedirectStandardOutput = true;
                        startInfo.UseShellExecute = false;
                        interval += parameterValue.Increment;
                        String argv = " " + parameterValue.LeftBorder.ToString().Replace('.', ',') + " " + parameterValue.RightBorder.ToString().Replace('.', ',') + " " + interval.ToString().Replace('.',',') + " " + Convert.ToString(k + 1) + " " + parameterValue.CoeffString;

                        String output = "";

                        startInfo.Arguments = argv;
                        process.StartInfo = startInfo;
                        decimal resultCode1;
                        try
                        {
                            process.Start();
                            output += process.StandardOutput.ReadLine();

                            if (!process.WaitForExit(1000))
                            {
                                resultCode1 = 0;
                                process.Kill(true);
                            }
                            else
                                resultCode1 = -1;
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (resultCode1 < 0)
                        {
                            MessageBox.Show("В Integral3x что-то не так, позже мы разберемся, что именно", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        ResultsTB += "TEST " + (i + 1) + " " + pars[0] + "\r\nMETHOD: " + pars[1] + "\r\n" +
                         "LB: " + parameterValue.LeftBorder + "\r\nRB: " + parameterValue.RightBorder + "\r\nStep: " + interval +
                         "\r\nEPS: " + resEps + "\r\nIntegral3x: " + output.Replace('\n', ' ') + " | Oracle: S = " + resultCode2 + "\r\n" +
                         result + "\r\n\r\n";
                        
                        if (k == 0)
                            parabolaList.Add((interval, (decimal)Math.Round(resEps, 3)));
                        else if (k == 1)
                            trapezeList.Add((interval, (decimal)Math.Round(resEps, 3)));
                        else
                            monteCarloList.Add((interval, (decimal)Math.Round(resEps, 3)));

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

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "Integral3x.exe";
                int resultCode1 = 0;

                if (errorType == 1)
                {
                    method = 1;
                    String _leftBorder = "эта_левая_граница_не_число";
                    rightBorder = 1;
                    eps = 1;
                    coeffs = "1 1 1 1 1";

                    interval = RandomMeInterval(selectedTypeNum);
                    String argv = " " + _leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

                    startInfo.Arguments = argv;
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    resultCode1 = process.ExitCode;

                }
                else if (errorType == 2)
                {
                    method = 1;
                    String _rightBorder = "эта_правая_граница_не_число";
                    leftBorder = 1;
                    eps = 1;
                    coeffs = "1 1 1 1 1";

                    interval = RandomMeInterval(selectedTypeNum);
                    String argv = " " + leftBorder.ToString() + " " + _rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

                    startInfo.Arguments = argv;
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    resultCode1 = process.ExitCode;
                }
                else if (errorType == 3)
                {
                    method = 1;
                    leftBorder = 9;
                    rightBorder = 1;
                    eps = 1;
                    coeffs = "1 1 1 1 1";

                    interval = RandomMeInterval(selectedTypeNum);
                    String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

                    startInfo.Arguments = argv;
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    resultCode1 = process.ExitCode;
                }
                else if (errorType == 4)
                {
                    method = 1;
                    leftBorder = 1;
                    rightBorder = 5;
                    eps = 1;
                    coeffs = "1 1 1 1 1";

                    interval = 25;
                    String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

                    startInfo.Arguments = argv;
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    resultCode1 = process.ExitCode;
                }
                else if (errorType == 5)
                {
                    method = 6;
                    leftBorder = 1;
                    rightBorder = 5;
                    eps = 1;
                    coeffs = "1 1 1 1 1";

                    interval = 0.05M;
                    String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

                    startInfo.Arguments = argv;
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    resultCode1 = process.ExitCode;
                }
                else if (errorType == 6)
                {
                    method = 1;
                    leftBorder = 1;
                    rightBorder = 5;
                    eps = 1;
                    coeffs = "";

                    interval = RandomMeInterval(selectedTypeNum);
                    String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

                    startInfo.Arguments = argv;
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    resultCode1 = process.ExitCode;
                }

                if (resultCode1 <= 0)
                {
                    if (resultCode1 == -1)
                    {
                        ResultsTB += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
                                     "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Левая граница диапазона не является числом!\r\n" +
                                     "Ответ от Integral3x.exe: Левая граница диапазона не является числом!\r\n";
                        return;
                    }
                    else if (resultCode1 == -2)
                    {
                        ResultsTB += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
                        "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Правая граница диапазона не является числом!\r\n" +
                         "Ответ от Integral3x.exe: Правая граница диапазона не является числом!\r\n";
                        return;
                    }
                    else if (resultCode1 == -3)
                    {
                        ResultsTB += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
                        "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Левая граница диапазона должна быть <правой границы диапазона!\r\n" +
                         "Ответ от Integral3x.exe: Левая граница диапазона должна быть <правой границы диапазона!\r\n";
                        return;
                    }
                    else if (resultCode1 == -4)
                    {
                        ResultsTB += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
                        "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Шаг интегрирования должен быть в пределах[0.000001; 0.5]\r\n" +
                         "Ответ от Integral3x.exe: Шаг интегрирования должен быть в пределах[0.000001; 0.5]\r\n";
                        return;
                    }
                    else if (resultCode1 == -5)
                    {
                        ResultsTB += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
                        "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Четвертый параметр определяет метод интегрирования и должен быть в пределах[1; 3]\r\n" +
                         "Ответ от Integral3x.exe: Четвертый параметр определяет метод интегрирования и должен быть в пределах[1; 3]\r\n";
                        return;
                    }
                    else if (resultCode1 == 0)
                    {
                        ResultsTB += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
                        "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Число параметров не соответствует ожидаемому идолжно быть, как минимум 5!\r\n" +
                         "Ответ от Integral3x.exe: Число параметров не соответствует ожидаемому идолжно быть, как минимум 5!\r\n";
                        return;
                    }
                }
            }
        }
    }
}
