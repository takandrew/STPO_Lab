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
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
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

        private string TalkWithProcess(string argv)
        {
            List<string> outputText = new List<string>();
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
                        output = TalkWithProcess(argv);
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

        private void GetBorders(ref Random rnd, bool isCorrect,out decimal leftBorder, out decimal rightBorder)
        {
            if (isCorrect)
            {
                leftBorder = rnd.Next(-10, 10);
                rightBorder = rnd.Next((int)leftBorder + 1, (int)leftBorder + 21);
            }
            else
            {
                 rightBorder = rnd.Next(-10, 10);
                 leftBorder = rnd.Next((int)rightBorder + 1, (int)rightBorder + 21);
            }
        }
        private void GetIncrement(ref Random rnd, bool isCorrect, out decimal increment)
        {
            if (isCorrect)
            {
                increment = (decimal)rnd.Next(2, 400000) / 1000000;
            }
            else
            {
                int isIncrementBiggerThanMax = rnd.Next(0, 2);
                if (isIncrementBiggerThanMax == 0)
                    increment = (decimal)rnd.Next(1, 100) / 100000000;
                else
                    increment = (decimal)rnd.Next(1, 100) / 2;
            }
        }

        private void GetMethod(ref Random rnd, bool isCorrect, out int method)
        {
            if (isCorrect)
            {
                method = rnd.Next(1, 4);
            }
            else
            {
                int isMethodBiggerThanMax = rnd.Next(0, 2);
                if (isMethodBiggerThanMax == 0)
                    method = rnd.Next(4, 100);
                else
                    method = rnd.Next(-100, 1);
            }
        }

        private void GetCoeffs(ref Random rnd, out string coeffs)
        {
            coeffs = String.Empty;
            int coeffQuantity = rnd.Next(1, 20);
            for (int j = 0; j < coeffQuantity; j++)
            {
                coeffs += rnd.Next(-100, 100) + " ";
            }
        }

        delegate void ErrorTypeRandFunc(ref int testNumber, ref Random rnd, ref StringBuilder result, ref StringBuilder resultFail);

        private void ErrorType1(ref int testNumber, ref Random rnd, ref StringBuilder result, ref StringBuilder resultFail)
        {
            GetBorders(ref rnd, true, out decimal leftBorder, out decimal rightBorder);
            GetIncrement(ref rnd, true, out decimal increment);
            GetMethod(ref rnd, true, out int method);
            GetCoeffs(ref rnd, out string coeffs);

            string supposedAnswer = "Левая граница диапазона не является числом!\n";

            List<string> argvParts = new List<string>();
            argvParts.Add("leftborder"); argvParts.Add(rightBorder.ToString()); 
            argvParts.Add(increment.ToString()); argvParts.Add(method.ToString()); 
            argvParts.Add(coeffs);

            string argv = string.Join(" ", argvParts).Replace('.', ',');

            string gotError = TalkWithProcess(argv);

            string gotResult = "Тест " + testNumber + ": Негативный" + "\r\nЛевая граница: " + argvParts[0] + "\r\nПравая граница: " + argvParts[1] +
                               "\r\nШаг интегрирования: " + argvParts[2] + "\r\nМетод: " + argvParts[3] +
                               "\r\nCoeffs: " + coeffs + "\r\n---" + "\r\nОжидаемый ответ: \n" +
                               supposedAnswer.ToString() + "---\r\n" +
                               "Ответ от Integral3x.exe: \n" + gotError + "\r\n";

            result.Append(gotResult);
            if (supposedAnswer.ToString() != gotError)
                resultFail.Append(gotResult);
            testNumber++;
        }

        private void ErrorType2(ref int testNumber, ref Random rnd, ref StringBuilder result, ref StringBuilder resultFail)
        {
            GetBorders(ref rnd, true, out decimal leftBorder, out decimal rightBorder);
            GetIncrement(ref rnd, true, out decimal increment);
            GetMethod(ref rnd, true, out int method);
            GetCoeffs(ref rnd, out string coeffs);

            string supposedAnswer = "Правая граница диапазона не является числом!\n";

            List<string> argvParts = new List<string>();
            argvParts.Add(leftBorder.ToString()); argvParts.Add("rightborder");
            argvParts.Add(increment.ToString()); argvParts.Add(method.ToString());
            argvParts.Add(coeffs);

            string argv = string.Join(" ", argvParts).Replace('.', ',');

            string gotError = TalkWithProcess(argv);

            string gotResult = "Тест " + testNumber + ": Негативный" + "\r\nЛевая граница: " + argvParts[0] + "\r\nПравая граница: " + argvParts[1] +
                               "\r\nШаг интегрирования: " + argvParts[2] + "\r\nМетод: " + argvParts[3] +
                               "\r\nCoeffs: " + coeffs + "\r\n---" + "\r\nОжидаемый ответ: \n" +
                               supposedAnswer.ToString() + "---\r\n" +
                               "Ответ от Integral3x.exe: \n" + gotError + "\r\n";

            result.Append(gotResult);
            if (supposedAnswer.ToString() != gotError)
                resultFail.Append(gotResult);
            testNumber++;
        }

        private void ErrorType3(ref int testNumber, ref Random rnd, ref StringBuilder result, ref StringBuilder resultFail)
        {
            GetBorders(ref rnd, false, out decimal leftBorder, out decimal rightBorder);
            GetIncrement(ref rnd, true, out decimal increment);
            GetMethod(ref rnd, true, out int method);
            GetCoeffs(ref rnd, out string coeffs);

            string supposedAnswer = "Левая граница диапазона должна быть < правой границы диапазона!\n";

            List<string> argvParts = new List<string>();
            argvParts.Add(leftBorder.ToString()); argvParts.Add(rightBorder.ToString());
            argvParts.Add(increment.ToString()); argvParts.Add(method.ToString());
            argvParts.Add(coeffs);

            string argv = string.Join(" ", argvParts).Replace('.', ',');

            string gotError = TalkWithProcess(argv);

            string gotResult = "Тест " + testNumber + ": Негативный" + "\r\nЛевая граница: " + argvParts[0] + "\r\nПравая граница: " + argvParts[1] +
                               "\r\nШаг интегрирования: " + argvParts[2] + "\r\nМетод: " + argvParts[3] +
                               "\r\nCoeffs: " + coeffs + "\r\n---" + "\r\nОжидаемый ответ: \n" +
                               supposedAnswer.ToString() + "---\r\n" +
                               "Ответ от Integral3x.exe: \n" + gotError + "\r\n";

            result.Append(gotResult);
            if (supposedAnswer.ToString() != gotError)
                resultFail.Append(gotResult);
            testNumber++;
        }

        private void ErrorType4(ref int testNumber, ref Random rnd, ref StringBuilder result, ref StringBuilder resultFail)
        {
            GetBorders(ref rnd, true, out decimal leftBorder, out decimal rightBorder);
            GetIncrement(ref rnd, false, out decimal increment);
            GetMethod(ref rnd, true, out int method);
            GetCoeffs(ref rnd, out string coeffs);

            string supposedAnswer = "Шаг интегрирования должен быть в пределах [0.000001;0.5]\n";

            List<string> argvParts = new List<string>();
            argvParts.Add(leftBorder.ToString()); argvParts.Add(rightBorder.ToString());
            argvParts.Add(increment.ToString()); argvParts.Add(method.ToString());
            argvParts.Add(coeffs);

            string argv = string.Join(" ", argvParts).Replace('.', ',');

            string gotError = TalkWithProcess(argv);

            string gotResult = "Тест " + testNumber + ": Негативный" + "\r\nЛевая граница: " + argvParts[0] + "\r\nПравая граница: " + argvParts[1] +
                               "\r\nШаг интегрирования: " + argvParts[2] + "\r\nМетод: " + argvParts[3] +
                               "\r\nCoeffs: " + coeffs + "\r\n---" + "\r\nОжидаемый ответ: \n" +
                               supposedAnswer.ToString() + "---\r\n" +
                               "Ответ от Integral3x.exe: \n" + gotError + "\r\n";

            result.Append(gotResult);
            if (supposedAnswer.ToString() != gotError)
                resultFail.Append(gotResult);
            testNumber++;
        }

        private void ErrorType5(ref int testNumber, ref Random rnd, ref StringBuilder result, ref StringBuilder resultFail)
        {
            GetBorders(ref rnd, true, out decimal leftBorder, out decimal rightBorder);
            GetIncrement(ref rnd, true, out decimal increment);
            GetMethod(ref rnd, false, out int method);
            GetCoeffs(ref rnd, out string coeffs);

            string supposedAnswer = "Четвертый параметр определяет метод интегрирования и должен быть в пределах [1;3]\n";

            List<string> argvParts = new List<string>();
            argvParts.Add(leftBorder.ToString()); argvParts.Add(rightBorder.ToString());
            argvParts.Add(increment.ToString()); argvParts.Add(method.ToString());
            argvParts.Add(coeffs);

            string argv = string.Join(" ", argvParts).Replace('.', ',');

            string gotError = TalkWithProcess(argv);

            string gotResult = "Тест " + testNumber + ": Негативный" + "\r\nЛевая граница: " + argvParts[0] + "\r\nПравая граница: " + argvParts[1] +
                               "\r\nШаг интегрирования: " + argvParts[2] + "\r\nМетод: " + argvParts[3] +
                               "\r\nCoeffs: " + coeffs + "\r\n---" + "\r\nОжидаемый ответ: \n" +
                               supposedAnswer.ToString() + "---\r\n" +
                               "Ответ от Integral3x.exe: \n" + gotError + "\r\n";

            result.Append(gotResult);
            if (supposedAnswer.ToString() != gotError)
                resultFail.Append(gotResult);
            testNumber++;
        }

        private void ErrorType6(ref int testNumber, ref Random rnd, ref StringBuilder result, ref StringBuilder resultFail)
        {
            GetBorders(ref rnd, true, out decimal leftBorder, out decimal rightBorder);
            GetIncrement(ref rnd, true, out decimal increment);
            GetMethod(ref rnd, true, out int method);

            string supposedAnswer = "Число параметров не соответствует ожидаемому и должно быть, как минимум 5!\n";

            List<string> argvParts = new List<string>();
            argvParts.Add(leftBorder.ToString()); argvParts.Add(rightBorder.ToString());
            argvParts.Add(increment.ToString()); argvParts.Add(method.ToString());

            string argv = string.Join(" ", argvParts).Replace('.', ',');

            string gotError = TalkWithProcess(argv);

            string gotResult = "Тест " + testNumber + ": Негативный" + "\r\nЛевая граница: " + argvParts[0] + "\r\nПравая граница: " + argvParts[1] +
                               "\r\nШаг интегрирования: " + argvParts[2] + "\r\nМетод: " + argvParts[3] +
                               "\r\nCoeffs: " + "\r\n---" + "\r\nОжидаемый ответ: \n" +
                               supposedAnswer.ToString() + "---\r\n" +
                               "Ответ от Integral3x.exe: \n" + gotError + "\r\n";

            result.Append(gotResult);
            if (supposedAnswer.ToString() != gotError)
                resultFail.Append(gotResult);
        }

        public void ProcessDataNegative(int testCaseQuantity, int selectedErrorNum, out string ResultsTB, out string ResultsFail)
        {
            ResultsTB = String.Empty;
            ResultsFail = String.Empty;
            Random rnd = new Random();

            StringBuilder result = new StringBuilder();
            StringBuilder resultFail = new StringBuilder();
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

            int testNumber = 1;
            errorTypeRandFunc(ref testNumber,ref rnd, ref result, ref resultFail);

            ResultsTB = result.ToString();
            ResultsFail = resultFail.ToString();

        }

        public void ProcessDataNegativeRandom(int testCaseQuantity, out string ResultsTB, out string ResultsFail)
        {
            ResultsTB = String.Empty;
            ResultsFail = String.Empty;

            StringBuilder result = new StringBuilder();
            StringBuilder resultFail = new StringBuilder();
            ParameterValue parameterValue = new ParameterValue();
            Random rnd = new Random();
            

            for (int k = 0; k < testCaseQuantity; k++)
            {
                int errorQuantity = rnd.Next(1, 7);
                List<int> errorTypes = new List<int>();
                for (int i = 0; i < errorQuantity; i++)
                {
                    int errorType = rnd.Next(1, 7);
                    if (errorType == 3 || errorType == 1 || errorType == 2)
                    {
                        if (errorType == 3 && !errorTypes.Contains(errorType) && (!errorTypes.Contains(1) && !errorTypes.Contains(2)))
                            errorTypes.Add(errorType);
                        else
                        {
                            if ((errorType == 1 || errorType == 2) && !errorTypes.Contains(errorType) && !errorTypes.Contains(3))
                                errorTypes.Add(errorType);
                            else
                                i--;
                        }
                        if (errorType == 3)
                        {
                            if (errorTypes.Contains(4) && errorTypes.Contains(5) && errorTypes.Contains(6))
                            {
                                errorQuantity -= 2;
                                break;
                            }
                        }
                        else
                        {
                            if (errorTypes.Contains(1) && errorTypes.Contains(2) && errorTypes.Contains(4) &&
                                errorTypes.Contains(5) && errorTypes.Contains(6))
                            {
                                errorQuantity -= 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!errorTypes.Contains(errorType))
                            errorTypes.Add(errorType);
                        else
                            i--;
                    }
                }

                StringBuilder supposedAnswer = new StringBuilder();

                parameterValue.TestCaseQuantity = 1;
                parameterValue.AllowableEPS = 0;
                int method;

                string argv = String.Empty;
                List<string> argvParts = new List<string>();

                if (errorTypes.Contains(3))
                {
                    supposedAnswer.Append("Левая граница диапазона должна быть < правой границы диапазона!\n");
                    GetBorders(ref rnd, false, out decimal leftBorder, out decimal rightBorder);
                    parameterValue.LeftBorder = leftBorder;
                    parameterValue.RightBorder = rightBorder;
                }
                else
                {
                    GetBorders(ref rnd, true, out decimal leftBorder, out decimal rightBorder);
                    parameterValue.LeftBorder = leftBorder;
                    parameterValue.RightBorder = rightBorder;
                }
                if (errorTypes.Contains(1))
                {
                    supposedAnswer.Append("Левая граница диапазона не является числом!\n");
                    argvParts.Add("leftborder");
                }
                else
                {
                    argvParts.Add(parameterValue.LeftBorder.ToString());
                }
                if (errorTypes.Contains(2))
                {
                    supposedAnswer.Append("Правая граница диапазона не является числом!\n");
                    argvParts.Add("rightborder");
                }
                else
                {
                    argvParts.Add(parameterValue.RightBorder.ToString());
                }
                if (errorTypes.Contains(4))
                {
                    supposedAnswer.Append("Шаг интегрирования должен быть в пределах [0.000001;0.5]\n");
                    GetIncrement(ref rnd, false, out decimal increment);
                    parameterValue.Increment = increment;
                    argvParts.Add(parameterValue.Increment.ToString());
                }
                else
                {
                    GetIncrement(ref rnd, true, out decimal increment);
                    parameterValue.Increment = increment;
                    argvParts.Add(parameterValue.Increment.ToString());
                }
                if (errorTypes.Contains(5))
                {
                    supposedAnswer.Append("Четвертый параметр определяет метод интегрирования и должен быть в пределах [1;3]\n");
                    GetMethod(ref rnd, false, out int getMethod);
                    method = getMethod;
                    argvParts.Add(method.ToString());
                }
                else
                {
                    GetMethod(ref rnd, true, out int getMethod);
                    method = getMethod;
                    argvParts.Add(method.ToString());
                }
                if (errorTypes.Contains(6))
                {
                    supposedAnswer.Append("Число параметров не соответствует ожидаемому и должно быть, как минимум 5!\n");
                }
                else
                {
                    GetCoeffs(ref rnd, out string coeffsStr);
                    parameterValue.CoeffString = coeffsStr;
                    argvParts.Add(parameterValue.CoeffString);
                }

                argv = string.Join(" ", argvParts).Replace('.', ',');

                string gotError = TalkWithProcess(argv);

                string gotResult = "Тест " + (k+1) + ": Негативный" + "\r\nЛевая граница: " + argvParts[0] + "\r\nПравая граница: " + argvParts[1] +
                                   "\r\nШаг интегрирования: " + argvParts[2] + "\r\nМетод: " + argvParts[3] +
                                   "\r\nCoeffs: " + parameterValue.CoeffString + "\r\n---" + "\r\nОжидаемый ответ: \n" +
                                   supposedAnswer.ToString() + "---\r\n" +
                                   "Ответ от Integral3x.exe: \n" + gotError + "\r\n";

                result.Append(gotResult);
                if (supposedAnswer.ToString() != gotError)
                    resultFail.Append(gotResult);
            }

            ResultsTB = result.ToString();
            ResultsFail = resultFail.ToString();
        }
    }
}
