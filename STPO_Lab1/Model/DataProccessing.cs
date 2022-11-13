using LiveCharts.Wpf.Charts.Base;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace STPO_Lab1.Model
{
    public class DataProccessing
    {
        private decimal[] dCoeffs;

        int FillCoeffsArray(String str)
        {
            string[] tmpCoeffs = str.Trim().Split(' ');
            dCoeffs = new decimal[tmpCoeffs.Length];
            for (int i = 0; i < tmpCoeffs.Length; i++)
            {
                int c1 = 0, c2 = 0; ;
                for (int j = 0; j < tmpCoeffs[i].Length; j++)
                {
                    if (tmpCoeffs[i].Length == 1 && (tmpCoeffs[i][j] == '-' || tmpCoeffs[i][j] == '.'))
                    {
                        MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    if (!Char.IsDigit(tmpCoeffs[i][j]) && (tmpCoeffs[i][j] != '-' && tmpCoeffs[i][j] != '.'))
                    {
                        MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return -1;
                    }
                    if (tmpCoeffs[i][j] == '-')
                    {
                        c1++;
                    }
                    if (tmpCoeffs[i][j] == '.')
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

        decimal IntegrateThis(decimal leftBorder, decimal rightBorder)
        {
            decimal result = 0;
            for (int j = 0; j < dCoeffs.Length; j++)
            {
                result += dCoeffs[j] * (decimal)((Math.Pow((double)rightBorder, j + 1) / (j + 1)) - (Math.Pow((double)leftBorder, j + 1) / (j + 1)));
            }
            return result;
        }

        public void ExportDataToFile(string text)
        {
            var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog();
            dialog.AlwaysAppendDefaultExtension = true;
            dialog.DefaultExtension = ".txt";
            dialog.Filters.Add(new CommonFileDialogFilter("txt files", "*.txt"));

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    using (var sw = new System.IO.StreamWriter(dialog.FileName, false, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

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
            string resultText = String.Empty;
            for (int i = 0; i < outputText.Count; i++)
            {
                if (i != outputText.Count - 1)
                    resultText += outputText[i] + "\n";
            }

            return resultText;
        }

        public void ProcessDataPositive(ParameterValue parameterValue, out List<decimal> parabolaList,
            out List<decimal> trapezeList, out List<decimal> monteCarloList, out string ResultsTB, out string ResultFailTB)
        {
            decimal resultCode2, interval = 0;
            ResultsTB = String.Empty;
            ResultFailTB = String.Empty;
            parabolaList = new List<decimal>();
            trapezeList = new List<decimal>();
            monteCarloList = new List<decimal>();

            if (FillCoeffsArray(parameterValue.CoeffString) == -1)
                return;

            StringBuilder resultStringBuilder = new StringBuilder();
            StringBuilder resultFailStringBuilder = new StringBuilder();

            for (int k = 0; k < 3; k++)
            {
                interval = parameterValue.StarterStep;
                for (int i = 0; i < parameterValue.TestCaseQuantity; i++)
                {
                    interval += parameterValue.Increment;
                    String argv = " " + parameterValue.LeftBorder.ToString().Replace('.', ',') + " " + parameterValue.RightBorder.ToString().Replace('.', ',') + " " + interval.ToString().Replace('.', ',') + " " + Convert.ToString(k + 1) + " " + parameterValue.CoeffString.Replace('.', ',');

                    String output = "";
                    decimal resultCode1;

                    try
                    {
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

                        output = TalkWithProcess(process);
                        resultCode1 = 0;
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    String[] pars = new String[2];
                    pars = GetTestParams(k + 1, 1);
                    String[] getVal = output.Split(' ');
                    resultCode1 = decimal.Parse(getVal[2], NumberStyles.Any, CultureInfo.InvariantCulture);

                    resultCode2 = IntegrateThis(parameterValue.LeftBorder, parameterValue.RightBorder);

                    double s = Convert.ToDouble(output.Replace("\n", "").Replace("S", "").Replace("=", "").Replace("\r", ""));
                    double resEps = Math.Abs(s - (double)resultCode2);

                    string result = "Тест " + (i + 1) + " " + pars[0] + "\r\nМетод: " + pars[1] + "\r\n" +
                                    "Левая граница: " + parameterValue.LeftBorder + "\r\nПравая граница: " +
                                    parameterValue.RightBorder + "\r\nШаг интегрирования: " + interval +
                                    "\r\nEPS: " + resEps + "\r\nIntegral3x: " + output.Replace('\n', ' ') +
                                    " | Oracle: S = " + resultCode2 + "\r\n" + "\r\n\r\n";

                    resultStringBuilder.Append(result);
                    if (!(resEps < (double)parameterValue.AllowableEPS))
                    {
                        resultFailStringBuilder.Append(result);
                    }

                    if (k == 0)
                        parabolaList.Add((decimal)Math.Round(resEps, 3));
                    else if (k == 1)
                        trapezeList.Add((decimal)Math.Round(resEps, 3));
                    else
                        monteCarloList.Add((decimal)Math.Round(resEps, 3));

                }
            }

            ResultsTB = resultStringBuilder.ToString();
            ResultFailTB = resultFailStringBuilder.ToString();
        }

        delegate void ErrorTypeRandFunc(ref StringBuilder result);

        private void ErrorType1(ref StringBuilder result)
        {
            result.Append("Ошибка типа 1 сработала. \n");
            //TODO: Написать обработку ошибки 1
        }

        private void ErrorType2(ref StringBuilder result)
        {
            result.Append("Ошибка типа 2 сработала. \n");
            //TODO: Написать обработку ошибки 2
        }

        private void ErrorType3(ref StringBuilder result)
        {
            result.Append("Ошибка типа 3 сработала. \n");
            //TODO: Написать обработку ошибки 3
        }

        private void ErrorType4(ref StringBuilder result)
        {
            result.Append("Ошибка типа 4 сработала. \n");
            //TODO: Написать обработку ошибки 4
        }

        private void ErrorType5(ref StringBuilder result)
        {
            result.Append("Ошибка типа 5 сработала. \n");
            //TODO: Написать обработку ошибки 5
        }

        private void ErrorType6(ref StringBuilder result)
        {
            result.Append("Ошибка типа 6 сработала. \n");
            //TODO: Написать обработку ошибки 6
        }

        public void ProcessDataNegative(int testCaseQuantity, int selectedErrorNum, out string ResultsTB)
        {
            ResultsTB = String.Empty;

            StringBuilder result = new StringBuilder();
            ErrorTypeRandFunc? errorTypeRandFunc = null;

            for (int i = 0; i < testCaseQuantity; i++)
            {
                switch (selectedErrorNum+1)
                {
                    case 1:
                        errorTypeRandFunc += ErrorType1;
                        break;
                    case 2:
                        errorTypeRandFunc += ErrorType2;
                        break;
                    case 3:
                        errorTypeRandFunc += ErrorType3;
                        break;
                    case 4:
                        errorTypeRandFunc += ErrorType4;
                        break;
                    case 5:
                        errorTypeRandFunc += ErrorType5;
                        break;
                    case 6:
                        errorTypeRandFunc += ErrorType6;
                        break;
                }
            }

            errorTypeRandFunc(ref result);

            ResultsTB = result.ToString();

        }

        public void ProcessDataNegativeRandom(int testCaseQuantity, out string ResultsTB)
        {
            //TODO: Нужно подумать над концепцией рандомных тест-кейсов. Скорее всего нужно генерить прям по каждому параметру тест-кейс

            ResultsTB = String.Empty;

            Random rnd = new Random();

            List<int> errorTypes = new List<int>();
            for (int i = 0; i < testCaseQuantity; i++)
            {
                int errorType = rnd.Next(1, 7);
                errorTypes.Add(errorType);
            }

            StringBuilder result = new StringBuilder();
            ErrorTypeRandFunc? errorTypeRandFunc = null;

            for (int i = 0; i < errorTypes.Count; i++)
            {
                switch (errorTypes[i])
                {
                    case 1:
                        errorTypeRandFunc += ErrorType1;
                        break;
                    case 2:
                        errorTypeRandFunc += ErrorType2;
                        break;
                    case 3:
                        errorTypeRandFunc += ErrorType3;
                        break;
                    case 4:
                        errorTypeRandFunc += ErrorType4;
                        break;
                    case 5:
                        errorTypeRandFunc += ErrorType5;
                        break;
                    case 6:
                        errorTypeRandFunc += ErrorType6;
                        break;
                }
            }

            errorTypeRandFunc(ref result);

            ResultsTB = result.ToString();
        }
    }
}
