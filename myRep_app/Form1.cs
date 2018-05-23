using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace myRep_app
{
    public partial class Form1 : Form
    {
        public static int loggedUserID;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'myRep_ODS_HCO_DataSet.HCOSet' table. You can move, or remove it, as needed.
            this.hCOSetTableAdapter.Fill(this.myRep_ODS_HCO_DataSet.HCOSet);
            // TODO: This line of code loads data into the 'myRep_ODS_Address_DataSet.AddressSet' table. You can move, or remove it, as needed.
            this.addressSetTableAdapter.Fill(this.myRep_ODS_Address_DataSet.AddressSet);
            // TODO: This line of code loads data into the 'myRep_ODS_HCP_DataSet.HCPSet' table. You can move, or remove it, as needed.
            this.hCPSetTableAdapter.Fill(this.myRep_ODS_HCP_DataSet.HCPSet);
            //Ukrywa kolumny z ID w poszczególnych dataGridView
            this.hcpDataGridView.Columns[0].Visible = false;
            this.addressDataGridView.Columns[0].Visible = false;
            this.hcoDataGridView.Columns[0].Visible = false;
        }

/********
* HOME*
* *****/
        private void LoginButton_Click_1(object sender, EventArgs e)
        {

            //Nawiązanie połączenia z bazą danych
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Properties.Settings.Default.ConnectionString;
            conn.Open();
            try
            {
                //Logowanie - matchowanie passworda z wartością w bazie danych dla konkretnego usera
                String commandText = "SELECT dbo.UserCredentialsSet.PWD FROM dbo.UserCredentialsSet join dbo.UserSet on dbo.UserSet.userID = dbo.UserCredentialsSet.UserID  WHERE dbo.UserSet.Username = @param";
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@param", UsernameBox.Text.ToString());

                if (Convert.ToString(command.ExecuteScalar()) == PasswordBox.Text.ToString())
                {
                    // Pobieranie imienia i nazwiska + wyświetlanie komunikatu powitalnego + aktywacja przycisku SignOut + czyszczenie textBoxów
                    command.CommandText = "SELECT FirstName FROM dbo.UserSet WHERE Username = @param2";
                    command.Parameters.AddWithValue("@param2", UsernameBox.Text.ToString());
                    String fullname = (String)command.ExecuteScalar();
                    command.CommandText = "SELECT LastName FROM dbo.UserSet WHERE Username = @param3";
                    command.Parameters.AddWithValue("@param3", UsernameBox.Text.ToString());
                    fullname = fullname + " " + Convert.ToString(command.ExecuteScalar());
                    MessageBox.Show("Login successful. Welcome, " + fullname, "SIGN IN INFO");
                    LogoutButton.Enabled = true;
                    LoginButton.Enabled = false;
                    UsernameBox.Enabled = false;
                    PasswordBox.Enabled = false;

                    //Zapisanie w pamięci ID zalogowanego User'a
                    command.CommandText = "SELECT UserID FROM dbo.UserSet WHERE Username = @param5";
                    command.Parameters.AddWithValue("@param5", UsernameBox.Text.ToString());
                    loggedUserID = Convert.ToInt32(command.ExecuteScalar());

                    //Nadanie dostępu do odpowiednich zasobów na podstawie Job Title
                    command.CommandText = "SELECT JobTitle FROM dbo.UserSet WHERE Username = @param4";
                    command.Parameters.AddWithValue("@param4", UsernameBox.Text.ToString());
                    switch ((String)command.ExecuteScalar())
                    {
                        case "SYSADMIN": { myAccountsButton.Visible = true; myAccountsButton.Enabled = true; break; }
                        
                        default: { myAccountsButton.Visible = false; myAccountsButton.Enabled = false; break; }
                    }
                    UsernameBox.Text = "";
                    PasswordBox.Text = "";
                }
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server: " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
            conn.Close();
        }

        private void LogoutButton_Click_1(object sender, EventArgs e)
        {
            loggedUserID = 0;
            LogoutButton.Enabled = false;
            LoginButton.Enabled = true;
            UsernameBox.Enabled = true;
            PasswordBox.Enabled = true;
            myAccountsButton.Visible = false;
            myAccountsButton.Enabled = false;
            MessageBox.Show("GOODBYE!");
        }

        private void myAccountsButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = myAccountsPage;
        }


/**************
* My Accounts*
**************/
        private void createnewhcpButon(object sender, EventArgs e)
        {
            mainController.SelectedTab = newHCPPage;
        }

/**************
* newHCP*
**************/

        //Pokazanie i schowanie panelu do wyboru znanych języków
        private void LanguageSpokenComboBox_DropDown(object sender, EventArgs e)
        {
            languagespokenPanel.Visible = true;
        }
        private void LanguageSpokenComboBox_DropDownClosed(object sender, EventArgs e)
        {
            languagespokenPanel.Visible = false;
        }

        //AKTYWACJA PRZYCISKU "CREATE!" TYLKO KIEDY POLA MANDATORY SĄ WYPEŁNIONE - trzeba dać to na każde pole przy textChanged
        private void ToogleCreateNewHCPButton()
        {
            createHCP.Enabled = (fnameBox.Text != "") && (lnameBox.Text != "") && (selectedAddressLabel.Text != "") && ((mRadio.Checked==true) || (fRadio.Checked==true)) && (academicTitleList.Text != "") && (SpecialtyList.Text != "");
        }
        private void fnameBox_TextChanged(object sender, EventArgs e)
        {
          //      createHCP.Enabled = !string.IsNullOrEmpty(fnameBox.Text);
            ToogleCreateNewHCPButton();

        }

        private void newHCPPage_Click(object sender, EventArgs e)
        {

        }

        private void lnameBox_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void selectedAddressLabel_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void mRadio_MouseClick(object sender, MouseEventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void fRadio_MouseClick(object sender, MouseEventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void academicTitleList_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void SpecialtyList_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewHCPButton();
        }

        private void createHCP_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO HCPSet VALUES(@firstname, @middlename, @lastname, @gender, @academictitle, @birthdate, @phonenumber, @email, @kol, @specialty, @addressid, @hcoid)";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@firstname", textBox1.Text.ToString());
                command.Parameters.AddWithValue("@middlename", textBox2.Text.ToString());
                command.Parameters.AddWithValue("@lastname", textBox3.Text.ToString());
                command.Parameters.AddWithValue("@gender", comboBox1.Text.ToString());
                command.Parameters.AddWithValue("@academictitle", comboBox2.Text.ToString());
                command.Parameters.Add("@birthdate", SqlDbType.Date).Value = dateTimePicker1.Value.Date;
                maskedTextBox1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(maskedTextBox1.Text.ToString()));
                command.Parameters.AddWithValue("@email", textBox4.Text.ToString());
                command.Parameters.AddWithValue("@kol", Convert.ToBoolean(checkBox1.Checked.ToString()));
                command.Parameters.AddWithValue("@specialty", comboBox3.Text.ToString());
                command.Parameters.AddWithValue("@addressid", Convert.ToInt32(label12.Text.ToString()));
                command.Parameters.AddWithValue("@hcoid", Convert.ToInt32(label11.Text.ToString()));
                command.ExecuteNonQuery();
                conn.Close();
                DBMonitor powrot = new DBMonitor();
                powrot.Show();
                this.Hide();
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }
    }
}
