using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

using ActReport.Core.Entities;
using ActReport.Persistence;

namespace ActReport.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        private string _firstName;
        private string _lastName;
        private Employee _selectedEmployee;
        private ObservableCollection<Employee> _employees;
        private ICommand _cmdSaveChanges;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                FirstName = _selectedEmployee?.FirstName;
                LastName = _selectedEmployee?.LastName;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }
        public ICommand CmdSaveChanges
        {
            get
            {
                if(_cmdSaveChanges == null)
                {
                    _cmdSaveChanges = new RelayCommand(
                        execute: _ =>
                        {
                            using UnitOfWork uow = new UnitOfWork();
                            _selectedEmployee.FirstName = _firstName;
                            _selectedEmployee.LastName = _lastName;
                            uow.EmployeeRepository.Update(_selectedEmployee);
                            uow.Save();

                            LoadEmployees();
                        },
                        canExecute: _ => _selectedEmployee != null);
                }

                return _cmdSaveChanges;
            }
            set
            {
                _cmdSaveChanges = value;
            }
        }
        
        public EmployeeViewModel()
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using(UnitOfWork uow = new UnitOfWork())
            {
                var employees = uow.EmployeeRepository
                    .Get(
                        orderBy:
                            coll => coll.OrderBy(employees => employees.LastName))
                    .ToList();

                Employees = new ObservableCollection<Employee>(employees);
            }
        }
    }
}
