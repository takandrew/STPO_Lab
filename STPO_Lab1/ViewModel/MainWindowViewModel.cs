using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
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
        private List<string> _stepOnChart = new();


        private RelayCommand? _startCommand;

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
        public List<string> StepOnChart
        {
            get => _stepOnChart;
            set
            {
                _stepOnChart = value;
                OnPropertyChanged();
            }
        }
        public Func<double, string> YFormatter { get; set; }

        #endregion

        public MainWindowViewModel()
        {
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
                    if (SelectedType == AllTypes.First()) 
                        if(!CheckDataCorrectness(ParameterValue)) 
                            return;


                    List<decimal> parabolaValueList;
                    List<decimal> trapezeValueList;
                    List<decimal> monteCarloValueList;
                    int selectedTypeNum = SelectedType == AllTypes.First() ? 1 : 2;
                    DataProccessing dataProccessing = new DataProccessing();
                    dataProccessing.ProccessData(ParameterValue, selectedTypeNum, NegativeInputListSelected, out parabolaValueList, out trapezeValueList, out monteCarloValueList, out string resultTextBlock);
                    ResultTextBlock = resultTextBlock;
                    ParabolaValues.Clear(); TrapezeValues.Clear(); MonteCarloValues.Clear();
                    for (int i = 0; i < parabolaValueList.Count; i++)
                    {
                        StepOnChart.Add((ParameterValue.StarterStep+i*ParameterValue.Increment).ToString());
                        ParabolaValues.Add(parabolaValueList[i]);
                        TrapezeValues.Add(trapezeValueList[i]);
                        MonteCarloValues.Add(monteCarloValueList[i]);
                    }
                });
            }
        }

        #endregion

        #region Functions

        bool CheckDataCorrectness(ParameterValue parameterValue)
        {
            string errorStr = string.Empty;

            if (parameterValue.LeftBorder >= parameterValue.RightBorder)
                errorStr += "Левая граница должна быть меньше правой. \n";

            if (ParameterValue.CoeffString != null)
            {
                string[] tempCoeffStr = parameterValue.CoeffString.Split(' ');
                if (tempCoeffStr.Length < 5)
                {
                    errorStr += "Коэффициенты полинома введены некорректно. Необходимо ввести не менее 5 чисел, разделяя их пробелом. \n";
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

            if (parameterValue.TestCaseQuantity < 1)
                errorStr += "Количество тест-кейсов должно быть больше нуля \n";

            if (parameterValue.StarterStep <= 0)
                errorStr += "Начальный шаг интегрирования должен быть больше нуля \n";

            if (parameterValue.Increment <= 0)
                errorStr += "Инкремент должен быть больше нуля \n";

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
