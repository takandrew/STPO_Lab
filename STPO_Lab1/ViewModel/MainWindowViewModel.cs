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

        private string _selectedType;
        private IEnumerable<string> _allTypes;
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
                    if (SelectedType == AllTypes.First()) 
                        if(!CheckDataCorrectness(ParameterValue)) 
                            return;


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
                if (!Regex.IsMatch(ParameterValue.CoeffString,
                        "^-?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)? -?[0-9]\\d*([\\.|\\,]\\d+)?$"))
                {
                    errorStr += "Коэффициенты полинома введены некорректно. Необходимо ввести 6 чисел, разделяя их пробелом. \n";
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
