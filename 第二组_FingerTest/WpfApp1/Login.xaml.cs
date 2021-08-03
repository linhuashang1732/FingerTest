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

namespace WpfApp1
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private string username;
        private string password;
        public Login()
        {
            InitializeComponent();
        }
        //登录
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tb.Text == null || pb.Password == null || tb.Text == "" || pb.Password == "")
            {
                MessageBox.Show("用户名或密码不能为空，请重新填写");
            }
            else
            {

                using (DatabaseUserEntities context = new DatabaseUserEntities())
                {
                    try
                    {
                        var q1 = from t in context.User
                                 where t.Username == tb.Text
                                 select t;

                        if (q1.Equals(null))
                        {
                            MessageBox.Show("该用户未注册，请重新输登录");
                            tb.Clear();
                            pb.Clear();
                        }
                        else
                        {
                            var q = q1.FirstOrDefault();
                            if (pb.Password == q.Password)
                            {
                                MainWindow mw = new MainWindow();

                                mw.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("密码错误，请重新输入");
                                pb.Clear();
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("发生错误：" + ex.Message);

                    }
                }

            }
        }
        //注册
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(tb.Text);
            if (tb.Text == null || pb.Password == null || tb.Text == "" || pb.Password == "")
            {
                MessageBox.Show("用户名或密码不能为空，请重新填写");
            }
            else
            {
                using (DatabaseUserEntities context = new DatabaseUserEntities())
                {

                    var q1 = from t in context.User
                             where t.Username == tb.Text
                             select t;
                    var p = q1.FirstOrDefault();
                    if (p==null)
                    {
                        User user = new User();
                        user.Password = pb.Password;
                        user.Username = tb.Text;
                        try
                        {
                            context.User.Add(user);
                            context.SaveChanges();
                            MessageBox.Show("注册成功");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("发生错误：" + ex.Message);

                        }

                    }
                    else
                    {
                        MessageBox.Show("该用户已被注册，请重新填写");
                        tb.Clear();
                        pb.Clear();

                    }

                }
            }
         

        }
    }
}
