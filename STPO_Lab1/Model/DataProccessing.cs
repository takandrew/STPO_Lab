using LiveCharts.Wpf.Charts.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace STPO_Lab1.Model
{
    public class DataProccessing
    {
        //int FillCoeffsArray(String str)
        //{
        //    coeffs = "";
        //    String[] tmpCoeffs = str.Trim().Split(' ');
        //    numOfCoeffs = tmpCoeffs.Length;
        //    dCoeffs = new double[tmpCoeffs.Length];
        //    for (int i = 0; i < tmpCoeffs.Length; i++)
        //    {
        //        int c1 = 0, c2 = 0; ;
        //        for (int j = 0; j < tmpCoeffs[i].Length; j++)
        //        {
        //            if (tmpCoeffs[i].Length == 1 && (tmpCoeffs[i][j] == '-' || tmpCoeffs[i][j] == ','))
        //            {
        //                MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return -1;
        //            }
        //            if (!Char.IsDigit(tmpCoeffs[i][j]) && (tmpCoeffs[i][j] != '-' && tmpCoeffs[i][j] != ','))
        //            {
        //                MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return -1;
        //            }
        //            if (tmpCoeffs[i][j] == '-')
        //            {
        //                c1++;
        //            }
        //            if (tmpCoeffs[i][j] == ',')
        //            {
        //                c2++;
        //            }
        //        }
        //        if (c1 < 2 && c2 < 2)
        //        {
        //            coeffs += " " + tmpCoeffs[i];
        //            dCoeffs[i] = Convert.ToDouble(tmpCoeffs[i]);
        //        }
        //        else
        //        {
        //            MessageBox.Show("Один из коэффициентов не являются валидным!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return -1;
        //        }
        //    }
        //    return 0;
        //}

        //double RandomMeInterval(int testType)
        //{
        //    double interval = 0;
        //    if (testType == 1)
        //    {
        //        Random rnd = new Random();
        //        interval = rnd.Next(1, 1000);
        //        interval = interval * 0.00001;
        //    }
        //    return interval;
        //}

        //String[] GetTestParams(int method, int testType)
        //{
        //    String[] pars = new String[2];
        //    if (testType == 1)
        //        pars[0] = "POSITIVE";
        //    else
        //        pars[0] = "NEGATIVE";

        //    if (method == 1)
        //        pars[1] = "PARABOLIC";
        //    else if (method == 2)
        //        pars[1] = "TRAPEZIA";
        //    else if (method == 3)
        //        pars[1] = "MONTE CARLO";
        //    return pars;
        //}

        //bool IsInBounds(double res1, double res2, double eps)
        //{
        //    bool state;
        //    if (Math.Abs(res1 - res2) <= eps)
        //        state = true;
        //    else
        //        state = false;
        //    return state;
        //}

        //double IntegrateThis(double leftBorder, double rightBorder)
        //{
        //    double result = 0;
        //    for (int j = 0; j < dCoeffs.Length; j++)
        //    {
        //        result += dCoeffs[j] * ((Math.Pow(rightBorder, j + 1) / (j + 1)) - (Math.Pow(leftBorder, j + 1) / (j + 1)));
        //    }
        //    return result;
        //}

        //private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (listBox2.SelectedIndex + 1 == 1)
        //    {
        //        groupBox5.Visible = false;
        //        NumOfTestsTB.Visible = true;
        //        label6.Visible = true;
        //        label3.Visible = true;
        //        label7.Visible = true;
        //        StartStep.Visible = true;
        //        inkBox.Visible = true;
        //    }
        //    else if (listBox2.SelectedIndex + 1 == 2)
        //    {
        //        groupBox5.Visible = true;
        //        NumOfTestsTB.Visible = false;
        //        label6.Visible = false;
        //        label3.Visible = false;
        //        label7.Visible = false;
        //        StartStep.Visible = false;
        //        inkBox.Visible = false;
        //    }
        //}

        //private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}

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
        //            MessageBox.Show("Файл сохранен.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        catch (Exception exc)
        //        {
        //            MessageBox.Show(exc.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }
        //    }
        //}

        //private void StartBtn_Click(object sender, EventArgs e)
        //{

        //    chart1.Series[0].Points.Clear();
        //    chart1.Series[1].Points.Clear();
        //    chart1.Series[2].Points.Clear();
        //    ResultsTB.Text = "";
        //    double leftBorder = 0, rightBorder = 0, interval = 0, eps = 0, startStep = 0, ink = 0;
        //    int N = 0, method = 0, testType, errorType = 0;

        //    try
        //    {
        //        testType = Convert.ToInt32(listBox2.SelectedIndex + 1);
        //        if (testType == 1)
        //        {
        //            leftBorder = Convert.ToDouble(LeftBorderTB.Text);
        //            rightBorder = Convert.ToDouble(RightBorderTB.Text);
        //            ink = Convert.ToDouble(inkBox.Text);
        //            eps = 1;
        //            startStep = Convert.ToDouble(StartStep.Text);
        //            N = Convert.ToInt32(NumOfTestsTB.Text);

        //            if (testType == 0)
        //            {
        //                MessageBox.Show("Выберите тип тестирования!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            if (N > 100)
        //            {
        //                MessageBox.Show("Количество кейсой не может быть больше 100!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            if (startStep < 0.000001 || startStep > 0.5)
        //            {
        //                MessageBox.Show("Введите корректрый шаг!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            if (ink < 0.000001 || ink > 0.5)
        //            {
        //                MessageBox.Show("Введите корректрый инкримент!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            if (leftBorder > rightBorder)
        //            {
        //                MessageBox.Show("Левая граница > правой! Измените значения!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            if (FillCoeffsArray(CoeffsTB.Text) == -1)
        //                return;
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        MessageBox.Show(exc.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    if (testType == 1)
        //    {

        //        for (int k = 0; k < 3; k++)
        //        {
        //            interval = startStep;
        //            for (int i = 0; i < N; i++)
        //            {
        //                Process process = new Process();
        //                ProcessStartInfo startInfo = new ProcessStartInfo();
        //                startInfo.CreateNoWindow = true;
        //                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //                startInfo.FileName = "STPO.exe";
        //                startInfo.RedirectStandardOutput = true;
        //                startInfo.UseShellExecute = false;
        //                interval += ink;
        //                String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + Convert.ToString(k + 1) + coeffs;

        //                String output = "";

        //                startInfo.Arguments = argv;
        //                process.StartInfo = startInfo;
        //                try
        //                {
        //                    process.Start();
        //                    output = process.StandardOutput.ReadToEnd();
        //                    process.WaitForExit();
        //                }
        //                catch (Exception exc)
        //                {
        //                    MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    return;
        //                }
        //                resultCode1 = process.ExitCode;
        //                if (resultCode1 < 0)
        //                {
        //                    MessageBox.Show("В Integral3x что-то не так, позже мы разберемся, что именно", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    return;
        //                }

        //                String[] pars = new String[2];
        //                pars = GetTestParams(k + 1, testType);
        //                String[] getVal = output.Split(' ');
        //                resultCode1 = Convert.ToDouble(getVal[2].Remove(getVal[2].Length - 2).Replace('.', ','));

        //                resultCode2 = IntegrateThis(leftBorder, rightBorder);

        //                double s = Convert.ToDouble(output.Replace("\n", "").Replace("S", "").Replace("=", "").Replace("\r", "").Replace(".", ","));
        //                double resEps = Math.Abs(s - resultCode2);

        //                String result = "";
        //                ResultsTB.Text += "TEST " + (i + 1) + " " + pars[0] + "\r\n" +
        //                 "LB: " + leftBorder + "\r\nRB: " + rightBorder + "\r\nStep: " + interval + "\r\nMETHOD: " + pars[1] +
        //                 "\r\nEPS: " + resEps + "\r\nIntegral3x: " + output.Replace('\n', ' ') + " | Oracle: S = " + resultCode2 + "\r\n" +
        //                 result + "\r\n\r\n";

        //                chart1.Series[k].Points.AddXY(interval, Math.Round(resEps, 3));

        //            }
        //        }
        //    }
        //    else if (testType == 2)
        //    {
        //        try
        //        {
        //            errorType = Convert.ToInt32(listBox3.SelectedIndex + 1);
        //            if (errorType == 0)
        //            {
        //                MessageBox.Show("Выберите тип ошибки для тестирования!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            MessageBox.Show("Данные в " + exc.StackTrace + " не валидны!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        Process process = new Process();
        //        ProcessStartInfo startInfo = new ProcessStartInfo();
        //        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //        startInfo.FileName = "STPO.exe";

        //        if (errorType == 1)
        //        {
        //            method = 1;
        //            String _leftBorder = "эта_левая_граница_не_число";
        //            rightBorder = 1;
        //            eps = 1;
        //            coeffs = "1 1 1 1 1";

        //            interval = RandomMeInterval(testType);
        //            String argv = " " + _leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

        //            startInfo.Arguments = argv;
        //            process.StartInfo = startInfo;
        //            try
        //            {
        //                process.Start();
        //                process.WaitForExit();
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            resultCode1 = process.ExitCode;

        //        }
        //        else if (errorType == 2)
        //        {
        //            method = 1;
        //            String _rightBorder = "эта_правая_граница_не_число";
        //            leftBorder = 1;
        //            eps = 1;
        //            coeffs = "1 1 1 1 1";

        //            interval = RandomMeInterval(testType);
        //            String argv = " " + leftBorder.ToString() + " " + _rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

        //            startInfo.Arguments = argv;
        //            process.StartInfo = startInfo;
        //            try
        //            {
        //                process.Start();
        //                process.WaitForExit();
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            resultCode1 = process.ExitCode;
        //        }
        //        else if (errorType == 3)
        //        {
        //            method = 1;
        //            leftBorder = 9;
        //            rightBorder = 1;
        //            eps = 1;
        //            coeffs = "1 1 1 1 1";

        //            interval = RandomMeInterval(testType);
        //            String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

        //            startInfo.Arguments = argv;
        //            process.StartInfo = startInfo;
        //            try
        //            {
        //                process.Start();
        //                process.WaitForExit();
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            resultCode1 = process.ExitCode;
        //        }
        //        else if (errorType == 4)
        //        {
        //            method = 1;
        //            leftBorder = 1;
        //            rightBorder = 5;
        //            eps = 1;
        //            coeffs = "1 1 1 1 1";

        //            interval = 25;
        //            String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

        //            startInfo.Arguments = argv;
        //            process.StartInfo = startInfo;
        //            try
        //            {
        //                process.Start();
        //                process.WaitForExit();
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            resultCode1 = process.ExitCode;
        //        }
        //        else if (errorType == 5)
        //        {
        //            method = 6;
        //            leftBorder = 1;
        //            rightBorder = 5;
        //            eps = 1;
        //            coeffs = "1 1 1 1 1";

        //            interval = 0.05;
        //            String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

        //            startInfo.Arguments = argv;
        //            process.StartInfo = startInfo;
        //            try
        //            {
        //                process.Start();
        //                process.WaitForExit();
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            resultCode1 = process.ExitCode;
        //        }
        //        else if (errorType == 6)
        //        {
        //            method = 1;
        //            leftBorder = 1;
        //            rightBorder = 5;
        //            eps = 1;
        //            coeffs = "";

        //            interval = RandomMeInterval(testType);
        //            String argv = " " + leftBorder.ToString() + " " + rightBorder.ToString() + " " + interval.ToString() + " " + method.ToString() + coeffs;

        //            startInfo.Arguments = argv;
        //            process.StartInfo = startInfo;
        //            try
        //            {
        //                process.Start();
        //                process.WaitForExit();
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }
        //            resultCode1 = process.ExitCode;
        //        }

        //        if (resultCode1 <= 0)
        //        {
        //            if (resultCode1 == -1)
        //            {
        //                ResultsTB.Text += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
        //                "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Левая граница диапазона не является числом!\r\n" +
        //                 "Ответ от Integral3x.exe: Левая граница диапазона не является числом!\r\n";
        //                return;
        //            }
        //            else if (resultCode1 == -2)
        //            {
        //                ResultsTB.Text += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
        //                "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Правая граница диапазона не является числом!\r\n" +
        //                 "Ответ от Integral3x.exe: Правая граница диапазона не является числом!\r\n";
        //                return;
        //            }
        //            else if (resultCode1 == -3)
        //            {
        //                ResultsTB.Text += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
        //                "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Левая граница диапазона должна быть <правой границы диапазона!\r\n" +
        //                 "Ответ от Integral3x.exe: Левая граница диапазона должна быть <правой границы диапазона!\r\n";
        //                return;
        //            }
        //            else if (resultCode1 == -4)
        //            {
        //                ResultsTB.Text += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
        //                "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Шаг интегрирования должен быть в пределах[0.000001; 0.5]\r\n" +
        //                 "Ответ от Integral3x.exe: Шаг интегрирования должен быть в пределах[0.000001; 0.5]\r\n";
        //                return;
        //            }
        //            else if (resultCode1 == -5)
        //            {
        //                ResultsTB.Text += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
        //                "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Четвертый параметр определяет метод интегрирования и должен быть в пределах[1; 3]\r\n" +
        //                 "Ответ от Integral3x.exe: Четвертый параметр определяет метод интегрирования и должен быть в пределах[1; 3]\r\n";
        //                return;
        //            }
        //            else if (resultCode1 == 0)
        //            {
        //                ResultsTB.Text += "LB: " + leftBorder.ToString() + "\r\nRB: " + rightBorder.ToString() + "\r\nStep: " + interval.ToString() + "\r\nMETHOD: " + method.ToString() +
        //                "\r\nEPS: " + eps.ToString() + "\r\nCoeffs: " + coeffs + "\r\nОжидаемый ответ: Число параметров не соответствует ожидаемому идолжно быть, как минимум 5!\r\n" +
        //                 "Ответ от Integral3x.exe: Число параметров не соответствует ожидаемому идолжно быть, как минимум 5!\r\n";
        //                return;
        //            }
        //        }
        //    }
        //}
    }
}
