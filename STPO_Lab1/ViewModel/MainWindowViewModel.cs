using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private IEnumerable<String> _allTypes;
        private int _testCaseQuantity;
        private decimal _starterStep;
        private decimal _increment;


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

        #endregion

        public MainWindowViewModel()
        {
            AllTypes = new List<string>()
            {
                "Положительное",
                "Отрицательное"
            };
            SelectedType = AllTypes.First();
        }

        #region Commands

        public RelayCommand CalculateCommand
        {
            get
            {
                return _startCommand ??= new RelayCommand(x =>
                {
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
