﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using LiveCharts;
using STPO_Lab1.Model;
using WPF_MVVM_Classes;
using ViewModelBase = STPO_Lab1.Service.ViewModelBase;

namespace STPO_Lab1.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Variables

        private ParameterValue _parameterValue = new ParameterValue();

        private string _selectedType;
        private IEnumerable<string> _allTypes;
        private System.Windows.Visibility _positiveInputVisibility;
        private System.Windows.Visibility _negativeInputVisibility;
        private IEnumerable<string> _negativeInputList;
        private int _negativeInputListSelected;
        private ChartValues<decimal> _parabolaValues = new();
        private ChartValues<decimal> _trapezeValues = new();
        private ChartValues<decimal> _monteCarloValues = new();
        private string _resultTextBlock = String.Empty;
        private string _resultFailTextBlock = String.Empty;
        private List<string> _stepOnChart = new();
        private bool _isExportEnabled;
        private bool _isRandomNegativeTests;
        private bool _isNegativeTestsEnabled;


        private RelayCommand? _startCommand;
        private RelayCommand? _exportCommand;

        #endregion

        #region Properties

        public ParameterValue ParameterValue
        {
            get => _parameterValue;
            set
            {
                _parameterValue = value;
                OnPropertyChanged();
            }
        }
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                if (_selectedType == AllTypes.First())
                {
                    PositiveInputVisibility = System.Windows.Visibility.Visible;
                    NegativeInputVisibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    PositiveInputVisibility = System.Windows.Visibility.Hidden;
                    NegativeInputVisibility = System.Windows.Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }
        public IEnumerable<string> AllTypes
        {
            get => _allTypes;
            set
            {
                _allTypes = value;
                OnPropertyChanged();
            }
        }
        public System.Windows.Visibility PositiveInputVisibility
        {
            get => _positiveInputVisibility;
            set
            {
                _positiveInputVisibility = value;
                OnPropertyChanged();
            }
        }
        public System.Windows.Visibility NegativeInputVisibility
        {
            get => _negativeInputVisibility;
            set
            {
                _negativeInputVisibility = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<string> NegativeInputList
        {
            get => _negativeInputList;
            set
            {
                _negativeInputList = value;
                OnPropertyChanged();
            }
        }
        public int NegativeInputListSelected
        {
            get => _negativeInputListSelected;
            set
            {
                _negativeInputListSelected = value;
                OnPropertyChanged();
            }
        }
        public ChartValues<decimal> ParabolaValues
        {
            get => _parabolaValues;
            set
            {
                _parabolaValues = value;
                OnPropertyChanged();
            }
        }
        public ChartValues<decimal> TrapezeValues
        {
            get => _trapezeValues;
            set
            {
                _trapezeValues = value;
                OnPropertyChanged();
            }
        }
        public ChartValues<decimal> MonteCarloValues
        {
            get => _monteCarloValues;
            set
            {
                _monteCarloValues = value;
                OnPropertyChanged();
            }
        }
        public string ResultTextBlock
        {
            get => _resultTextBlock;
            set
            {
                _resultTextBlock = value;
                OnPropertyChanged();
            }
        }
        public string ResultFailTextBlock
        {
            get => _resultFailTextBlock;
            set
            {
                _resultFailTextBlock = value;
                OnPropertyChanged();
            }
        }
        public List<string> StepOnChart
        {
            get => _stepOnChart;
            set
            {
                _stepOnChart = value;
                OnPropertyChanged();
            }
        }
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set
            {
                _isExportEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsNegativeTestsEnabled
        {
            get => _isNegativeTestsEnabled;
            set
            {
                _isNegativeTestsEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsRandomNegativeTests
        {
            get => _isRandomNegativeTests;
            set
            {
                _isRandomNegativeTests = value;
                IsNegativeTestsEnabled = !_isRandomNegativeTests;
                OnPropertyChanged();
            }
        }
        public Func<double, string> YFormatter { get; set; }

        #endregion

        public MainWindowViewModel()
        {
            IsExportEnabled = false;
            IsNegativeTestsEnabled = true;
            YFormatter = value => value.ToString("N");
            AllTypes = new List<string>()
            {
                "Позитивное",
                "Негативное"
            };
            SelectedType = AllTypes.First();
            NegativeInputList = new List<string>()
            {
                "Левая граница не является числом",
                "Правая граница не является числом",
                "Левая граница больше правой",
                "Шаг вне пределов",
                "Параметр метода интегрирования вне пределов",
                "Число входящих параметров меньше 5"
            };
        }

        #region Commands

        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand ??= new RelayCommand(x =>
                {
                    ParabolaValues.Clear();
                    TrapezeValues.Clear();
                    MonteCarloValues.Clear();

                    if (!File.Exists("Integral3x.exe"))
                    {
                        MessageBox.Show("Проверьте наличие файла тестируемого приложения в директории программного комплекса.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int selectedTypeNum = SelectedType == AllTypes.First() ? 1 : 2;

                    if (!CheckDataCorrectness(ParameterValue, selectedTypeNum))
                        return;

                    
                    DataProccessing dataProccessing = new DataProccessing();
                    string resultTextBlock = String.Empty;
                    string resultFailTextBlock = String.Empty;

                    if (selectedTypeNum == 1)
                    {
                        dataProccessing.ProcessDataPositive(ParameterValue, out List<decimal> parabolaValueList, 
                            out List<decimal> trapezeValueList, out List<decimal> monteCarloValueList, 
                            out resultTextBlock, out resultFailTextBlock);
                        ParabolaValues.Clear(); TrapezeValues.Clear(); MonteCarloValues.Clear();
                        for (int i = 0; i < parabolaValueList.Count; i++)
                        {
                            StepOnChart.Add((ParameterValue.StarterStep + i * ParameterValue.Increment).ToString());
                            ParabolaValues.Add(parabolaValueList[i]);
                            TrapezeValues.Add(trapezeValueList[i]);
                            MonteCarloValues.Add(monteCarloValueList[i]);
                        }
                    }
                    else
                    {
                        if (IsRandomNegativeTests)
                        {
                            dataProccessing.ProcessDataNegativeRandom(ParameterValue.TestCaseQuantity,out resultTextBlock, out resultFailTextBlock);

                        }
                        else
                        {
                            dataProccessing.ProcessDataNegative(ParameterValue.TestCaseQuantity, NegativeInputListSelected, out resultTextBlock, out resultFailTextBlock);
                        }
                    }

                    ResultTextBlock = resultTextBlock;
                    ResultFailTextBlock = resultFailTextBlock;
                    IsExportEnabled = true;
                });
            }
        }

        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ??= new RelayCommand(x =>
                {
                    DataProccessing dataProccessing = new DataProccessing();
                    dataProccessing.ExportDataToFile(ResultTextBlock);
                });
            }
        }

        #endregion

        #region Functions

        bool CheckDataCorrectness(ParameterValue parameterValue, int selectedType)
        {
            string errorStr = string.Empty;

            if (selectedType == 1)
            {
                if (parameterValue.LeftBorder >= parameterValue.RightBorder)
                    errorStr += "Левая граница должна быть меньше правой. \n";

                if (!string.IsNullOrWhiteSpace(ParameterValue.CoeffString))
                {
                    string[] tempTempCoeffStr = parameterValue.CoeffString.Split(" ");
                    List<string> tempCoeffStr = new List<string>();
                    foreach (var elemStr in tempTempCoeffStr)
                    {
                        if (elemStr != "")
                            tempCoeffStr.Add(elemStr);
                    }
                    if (tempCoeffStr.Count < 1)
                    {
                        errorStr += "Коэффициенты полинома не введены.\n";
                    }
                    else
                    {
                        foreach (var strElem in tempCoeffStr)
                        {
                            if (!Regex.IsMatch(strElem,
                                    "^-?[0-9]\\d*([\\.|\\,]\\d+)?$"))
                            {
                                errorStr += "Коэффициенты полинома введены некорректно. \n";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    errorStr += "Коэффициенты полинома не введены. \n";
                }

                if (parameterValue.AllowableEPS < 0)
                    errorStr += "Допустимая погрешность не может быть отрицательной. \n";

                if (parameterValue.StarterStep <= 0)
                    errorStr += "Начальный шаг интегрирования должен быть больше нуля. \n";

                if (!(ParameterValue.StarterStep >= 0.000001M && ParameterValue.StarterStep <= 0.5M))
                    errorStr += "Значение начального шага интегрирования должно находиться в интервале [0.000001;0.5]. \n";

                if ((ParameterValue.StarterStep + ParameterValue.Increment * ParameterValue.TestCaseQuantity) > 0.5M)
                    errorStr +=
                        "В ходе работы программы значение начального шага интегрирования выйдет из интервала допустимых значений, [0.000001;0.5].";

                if (parameterValue.Increment <= 0)
                    errorStr += "Инкремент должен быть больше нуля. \n";
            }

            if (parameterValue.TestCaseQuantity < 1)
                errorStr += "Количество тест-кейсов должно быть больше нуля. \n";

            if (errorStr != string.Empty)
            {
                MessageBox.Show(errorStr, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
