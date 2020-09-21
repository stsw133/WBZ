using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WBZ.Classes;

namespace WBZ.Modules.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesAdd.xaml
    /// </summary>
    public partial class EmployeesAdd : Window
    {
        public EmployeesAdd(C_Employee employee, bool editMode)
        {
            InitializeComponent();
        }
    }
}
