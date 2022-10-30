using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using STPO_Lab1.Model;
using WPF_MVVM_Classes;
using ViewModelBase = STPO_Lab1.Service.ViewModelBase;

namespace STPO_Lab1.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Variables

        private ParameterValue _parameterValue = new ParameterValue();

        private decimal _leftBorder;
        private decimal _rightBorder;
        private string _coeffString;
        private string _selectedType;
        private IEnumerable<string> _allTypes;
        private int _testCaseQuantity;
        private decimal _starterStep;
        private decimal _increment;
        private System.Windows.Visibility _positiveInputVisibility;
        private System.Windows.Visibility _negativeInputVisibility;
        private IEnumerable<string> _negativeInputList;


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
        public decimal LeftBorder
        {
            get => _leftBorder;
            set
            {
                _leftBorder = value;
                OnPropertyChanged();
            }
        }
        public decimal RightBorder
        {
            get => _rightBorder;
            set
            {
                _rightBorder = value;
                OnPropertyChanged();
            }
        }
        public string CoeffString
        {
            get => _coeffString;
            set
            {
                _coeffString = value;
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
        public int TestCaseQuantity
        {
            get => _testCaseQuantity;
            set
            {
                _testCaseQuantity = value;
                OnPropertyChanged();
            }
        }
        public decimal StarterStep
        {
            get => _starterStep;
            set
            {
                _starterStep = value;
                OnPropertyChanged();
            }
        }
        public decimal Increment
        {
            get => _increment;
            set
            {
                _increment = value;
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

        #endregion

        public MainWindowViewModel()
        {
            AllTypes = new List<string>()
            {
                "Положительное",
                "Отрицательное"
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
                    if (!Regex.IsMatch(CoeffString,
                            "^-?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)?$"))
                    {
                        MessageBox.Show("Коэффициенты полинома введены некорректно. Необходимо ввести 6 чисел, разделяя их пробелом", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    ParameterValue.LeftBorder = LeftBorder;
                    ParameterValue.RightBorder = RightBorder;
                    ParameterValue.CoeffString = CoeffString;
                    ParameterValue.TestCaseQuantity = TestCaseQuantity;
                    ParameterValue.StarterStep = StarterStep;
                    ParameterValue.Increment = Increment;
                });
            }
        }

        #endregion

        #region Functions

        

        #endregion
    }
}
