using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.ViewModels;
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

namespace CuratorApp.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ICuratorRepository repo, IGroupRepository groupRepository, Curator curator)
        {
            InitializeComponent();
            DataContext = new CuratorAccountViewModel(repo, groupRepository, curator);
            this.Title = curator.Username;
        }
    }
}
