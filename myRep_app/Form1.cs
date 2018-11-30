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
        public static string loggedUserTerritory;
        public static String username;
        public static string action_backTo = "";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'myRep_ODS_HCOHCPDataSet.HCOSet' table. You can move, or remove it, as needed.
            this.hCOSetTableAdapter1.Fill(this.myRep_ODS_HCOHCPDataSet.HCOSet);
            // TODO: This line of code loads data into the 'myRep_ODS_HCOHCPDataSet.HCOHCP' table. You can move, or remove it, as needed.
            this.hCOHCPTableAdapter.Fill(this.myRep_ODS_HCOHCPDataSet.HCOHCP);
            // TODO: This line of code loads data into the 'myRep_ODS_HCOHCP_DataSet.HCOHCP' table. You can move, or remove it, as needed.
            // TODO: This line of code loads data into the 'myRep_ODS_User_DataSet.UserSet' table. You can move, or remove it, as needed.
            this.userSetTableAdapter.Fill(this.myRep_ODS_User_DataSet.UserSet);
            this.myRep_ODS_HCP_DataSet.EnforceConstraints = false;
            this.myRep_ODS_HCO_DataSet.EnforceConstraints = false;
            this.myRep_ODS_Address_DataSet.EnforceConstraints = false;
            // TODO: This line of code loads data into the 'myRep_ODS_HCO_DataSet.HCOSet' table. You can move, or remove it, as needed.
            this.hCOSetTableAdapter.Fill(this.myRep_ODS_HCO_DataSet.HCOSet);
            // TODO: This line of code loads data into the 'myRep_ODS_Address_DataSet.AddressSet' table. You can move, or remove it, as needed.
            this.addressSetTableAdapter.Fill(this.myRep_ODS_Address_DataSet.AddressSet);
            // TODO: This line of code loads data into the 'myRep_ODS_HCP_DataSet.HCPSet' table. You can move, or remove it, as needed.
            this.hCPSetTableAdapter.Fill(this.myRep_ODS_HCP_DataSet.HCPSet);
            //Ukrywa kolumny z ID w poszczególnych dataGridView
            this.hcpDataGridView.Columns[0].Visible = false;
            this.addressDataGridView.Columns[0].Visible = false;

        }
        #region HomePage

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

                    //Zapisanie w pamięci ID i Territory zalogowanego User'a
                    command.CommandText = "SELECT UserID FROM dbo.UserSet WHERE Username = @param5";
                    command.Parameters.AddWithValue("@param5", UsernameBox.Text.ToString());
                    loggedUserID = Convert.ToInt32(command.ExecuteScalar());
                    command.CommandText = "SELECT Territory FROM dbo.UserSet WHERE Username = @param5";
                    loggedUserTerritory = Convert.ToString(command.ExecuteScalar());
                    //Ustawienie parametru do wyświetlania HCP
                    param_show_HCP7ToolStripTextBox.Text = loggedUserTerritory.ToString();
                    showDedicatedHCPToolStripButton.PerformClick();
                    //Ustawienie parametru do wyswietlania Addressu
                    paramToolStripTextBox.Text = loggedUserTerritory.ToString();
                    addressDedicatedBookToolStripButton.PerformClick();
                    addressDataGridView.Columns[0].Visible = false;

                    //Wyswietlanie tabelki z dedykowanymi HCO
                    SqlCommand command30 = new SqlCommand("HCODedicatedDisplay", conn);
                    command30.CommandType = CommandType.StoredProcedure;
                    command30.Parameters.AddWithValue("@terr", loggedUserTerritory.ToString());
                    //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
                    DataTable dt1 = new DataTable();
                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command30);
                    dataAdapter1.Fill(dt1);
                    hcoDataGridView.DataSource = dt1;
                    hcoDataGridView.Columns[1].Visible = false;


                    //Nadanie dostępu do odpowiednich zasobów na podstawie Job Title
                    command.CommandText = "SELECT JobTitle FROM dbo.UserSet WHERE Username = @param4";
                    command.Parameters.AddWithValue("@param4", UsernameBox.Text.ToString());
                    switch ((String)command.ExecuteScalar())
                    {
                        case "SYSADMIN": {
                                myAccountsButton.Visible = true; myAccountsButton.Enabled = true;
                                userMgmtButton.Visible = true; userMgmtButton.Enabled = true;
                                break; }
                        case "REP":
                            {
                                myAccountsButton.Visible = true; myAccountsButton.Enabled = true;
                                userMgmtButton.Visible = false; userMgmtButton.Enabled = false;
                                break;
                            }


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
            userMgmtButton.Visible = false;
            userMgmtButton.Enabled = false;
            MessageBox.Show("GOODBYE!");
        }

        private void myAccountsButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = myAccountsPage;
        }
        #endregion HomePage

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


        //AKTYWACJA PRZYCISKU "CREATE!" TYLKO KIEDY POLA MANDATORY SĄ WYPEŁNIONE - trzeba dać to na każde pole przy textChanged
        private void ToogleCreateNewHCPButton()
        {
            createHCP.Enabled = (fnameBox.Text != "") && (lnameBox.Text != "") && (selectedAddressLabel.Text != "") && ((mRadio.Checked==true) || (fRadio.Checked==true)) && (academicTitleList.Text != "") && (SpecialtyList.Text != "");
        }
        private void ToogleCreateNewAddressButton()
        {
            CreateNewAddress.Enabled = (StreetNEWaddressBox.Text != "") && (CityNEWaddressBox.Text != "") && (TerritoryNEWaddressBox.Text != "") && (CountryNEWaddressBOX.Text != "") && (ZipNEWaddressBox.Text != "");
        }
        private void fnameBox_TextChanged(object sender, EventArgs e)
        {
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

        //Create HCP
        private void createHCP_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO HCPSet VALUES(@firstname, @middlename, @lastname, @gender, @academictitle,@specialty, @birthdate, @phonenumber, @email, @kol, @languagesspoken, @addressid)";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@firstname", fnameBox.Text.ToString());
                if (string.IsNullOrEmpty(mnameBox.Text.ToString())) command.Parameters.AddWithValue("@middlename", DBNull.Value); else command.Parameters.AddWithValue("@middlename", mnameBox.Text.ToString());
                command.Parameters.AddWithValue("@lastname", lnameBox.Text.ToString());
                if(mRadio.Checked == true) command.Parameters.AddWithValue("@gender", "M"); else command.Parameters.AddWithValue("@gender", "F");
                command.Parameters.AddWithValue("@academictitle", academicTitleList.Text.ToString());
                command.Parameters.Add("@birthdate", SqlDbType.Date).Value = birthdatePicker.Value.Date;
                phnumberBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (string.IsNullOrEmpty(phnumberBox.Text.ToString())) command.Parameters.AddWithValue("@phonenumber", DBNull.Value); else command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(phnumberBox.Text.ToString()));
                command.Parameters.AddWithValue("@email", emailBox.Text.ToString());
                command.Parameters.AddWithValue("@kol", Convert.ToBoolean(kolBox.Checked.ToString()));

                if (langSpoken.SelectedItems.Count > 0)
                {
                    string text = string.Join(",", langSpoken.SelectedItems.OfType<string>().Select(x => x.ToString()).ToArray());
                    if (otherLangTextBox.Text.ToString() != "") text = text + "," + otherLangTextBox.Text.ToString();
                    command.Parameters.AddWithValue("@languagesspoken", text);
                }
                else
                {
                    command.Parameters.AddWithValue("@languagesspoken", DBNull.Value);
                }
                              

                command.Parameters.AddWithValue("@specialty", SpecialtyList.Text.ToString());
                command.Parameters.AddWithValue("@addressid", Convert.ToInt32(selectedAddressLabel.Text.ToString()));
                command.ExecuteNonQuery();

                //Utworznie powiązania między HCP a HCO jeśli wybrany adres jest zarezerwowany dla jakiegoś HCO
                commandText = "SELECT hcoID FROM HCOSet WHERE AddressID = @adID";
                SqlCommand command2 = new SqlCommand(commandText, conn);
                command2.Parameters.AddWithValue("@adID", selectedAddressLabel.Text.ToString());
                if (command2.ExecuteScalar() != DBNull.Value)
                {
                    //Wybranie HCOID do powiązania
                    int hcoid = Convert.ToInt32(command2.ExecuteScalar());
                    MessageBox.Show("HCOID " + Convert.ToString(hcoid));
                    //Wybranie HCPID do powiązania
                    commandText = "SELECT max(hcpID) from HCPSet";
                    SqlCommand command3 = new SqlCommand(commandText, conn);
                    int latesthcpid = Convert.ToInt32(command3.ExecuteScalar());
                    MessageBox.Show("HCPID " + Convert.ToString(latesthcpid));
                    commandText = "INSERT INTO HCOHCP VALUES (@hcoID, @hcpID)";
                    SqlCommand command4 = new SqlCommand(commandText, conn);
                    command4.Parameters.AddWithValue("@hcoID", hcoid);
                    command4.Parameters.AddWithValue("@hcpID", latesthcpid);
                    command4.ExecuteNonQuery();
                }
                


                conn.Close();
                mainController.SelectedTab = myAccountsPage;
                myAccounts_Controller.SelectedTab = hcpPage;
                this.myRep_ODS_HCP_DataSet.Reset();
                this.hCPSetTableAdapter.Fill(this.myRep_ODS_HCP_DataSet.HCPSet);


            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        //Create HCO
        private void CreateHCOButton_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO HCOSet VALUES(@Name,@PhoneNumber,@Email,@Website,@AddressID,@Range,@Level,@SpecialType,@Beds,@Employees)";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@Name", HCONameBox.Text.ToString());
                if (string.IsNullOrEmpty(RangeHCOBox.Text.ToString())) command.Parameters.AddWithValue("@Range", DBNull.Value); else command.Parameters.AddWithValue("@Range", RangeHCOBox.Text.ToString());
                if (string.IsNullOrEmpty(LevelHCOBox.Text.ToString()))
                    command.Parameters.AddWithValue("@Level", DBNull.Value);
                else
                {
                    if (LevelHCOBox.SelectedIndex == 1)
                        command.Parameters.AddWithValue("@Level", 1);
                    else if (LevelHCOBox.SelectedIndex == 2)
                        command.Parameters.AddWithValue("@Level", 2);
                    if (LevelHCOBox.SelectedIndex == 3)
                        command.Parameters.AddWithValue("@Level", 3);
                }
                if (string.IsNullOrEmpty(SpecialTypeHCOBox.Text.ToString())) command.Parameters.AddWithValue("@SpecialType", DBNull.Value); else command.Parameters.AddWithValue("@SpecialType", SpecialTypeHCOBox.Text.ToString());
                if (BedsHCOBox.Value == 0) command.Parameters.AddWithValue("@Beds", DBNull.Value); else command.Parameters.AddWithValue("@Beds", BedsHCOBox.Text.ToString());
                if (EmployeesHCOBox.Value == 0) command.Parameters.AddWithValue("@Employees", DBNull.Value); else command.Parameters.AddWithValue("@Employees", EmployeesHCOBox.Text.ToString());
                PhNumberHCOBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (string.IsNullOrEmpty(PhNumberHCOBox.Text.ToString())) command.Parameters.AddWithValue("@PhoneNumber", DBNull.Value); else command.Parameters.AddWithValue("@PhoneNumber", Convert.ToInt32(PhNumberHCOBox.Text.ToString()));
                PhNumberHCOBox.TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
                if (string.IsNullOrEmpty(EmailHCOBox.Text.ToString())) command.Parameters.AddWithValue("@Email", DBNull.Value); else command.Parameters.AddWithValue("@Email", EmailHCOBox.Text.ToString());
                if (string.IsNullOrEmpty(WebsiteHCOBox.Text.ToString())) command.Parameters.AddWithValue("@Website", DBNull.Value); else command.Parameters.AddWithValue("@Website", WebsiteHCOBox.Text.ToString());
                command.Parameters.AddWithValue("@AddressID", Convert.ToInt32(SelectedHCO_AddressIDLabel.Text.ToString()));
                command.ExecuteNonQuery();
                conn.Close();


                //REFRESH WYŚWIETLANYCH HCO W myAccounts
                SqlCommand command30 = new SqlCommand("HCODedicatedDisplay", conn);
                command30.CommandType = CommandType.StoredProcedure;
                command30.Parameters.AddWithValue("@terr", loggedUserTerritory.ToString());
                    //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
                DataTable dt1 = new DataTable();
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command30);
                dataAdapter1.Fill(dt1);
                hcoDataGridView.DataSource = dt1;
                hcoDataGridView.Columns[1].Visible = false;

                mainController.SelectedTab = myAccountsPage;
                myAccounts_Controller.SelectedTab = hcoPage;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void userMgmtButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = userMgmtPage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = newUserPage;
        }

        private void createUserButton_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO UserSet VALUES(@firstname,@middlename, @lastname,@email, @jobtitle, @phonenumber,  @hiredate, @managerid, @territory, @username)";
            String commandText2 = "select count(*) from UserSet where Username = @param2";

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Properties.Settings.Default.ConnectionString; 
            conn.Open();
            try
            {
                SqlCommand command2 = new SqlCommand(commandText2, conn);
                command2.Parameters.AddWithValue("@param2", usernameUserBox.Text.ToString());
                if (Convert.ToInt32(command2.ExecuteScalar()) > 0)
                {
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.CommandText = commandText;
                    command.Parameters.AddWithValue("@firstname", fnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@middlename", mnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@lastname", lnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("jobtitle", jobtitleUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@email", emailUserBox.Text.ToString());
                    phnumberUserBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                    command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(phnumberUserBox.Text.ToString()));
                    command.Parameters.Add("@hiredate", SqlDbType.Date).Value = hireDateUserPicker.Value.Date;

                    command.Parameters.AddWithValue("@managerid", DBNull.Value);
                    //command.Parameters.AddWithValue("@managerid", selectedManagerUserLabel.Text.ToString());

                    command.Parameters.AddWithValue("@territory", territoryUserBox.Text.ToString());
                    String pomoc = fnameUserBox.Text.ToString().ToLower() + "." + Convert.ToString((Convert.ToInt32(command2.ExecuteScalar()) + 1)) + "." + lnameUserBox.Text.ToString().ToLower();
                    command.Parameters.AddWithValue("@username", pomoc);
                    username = pomoc;
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = setPasswordPage;
                    conn.Close();
                    this.myRep_ODS_User_DataSet.Reset();
                    this.userSetTableAdapter.Fill(this.myRep_ODS_User_DataSet.UserSet);
                }
                else
                {
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.Parameters.AddWithValue("@firstname", fnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@middlename", mnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@lastname", lnameUserBox.Text.ToString());
                    command.Parameters.AddWithValue("jobtitle", jobtitleUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@email", emailUserBox.Text.ToString());
                    phnumberUserBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                    command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(phnumberUserBox.Text.ToString()));
                    command.Parameters.Add("@hiredate", SqlDbType.Date).Value = hireDateUserPicker.Value.Date;

                    command.Parameters.AddWithValue("@managerid", DBNull.Value);
                    //command.Parameters.AddWithValue("@managerid", selectedManagerUserLabel.Text.ToString());

                    command.Parameters.AddWithValue("@territory", territoryUserBox.Text.ToString());
                    command.Parameters.AddWithValue("@username", fnameUserBox.Text.ToString().ToLower() + ".x." + lnameUserBox.Text.ToString().ToLower());
                    username = fnameUserBox.Text.ToString().ToLower() + ".x." + lnameUserBox.Text.ToString().ToLower();
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = setPasswordPage;
                    conn.Close();
                    this.myRep_ODS_User_DataSet.Reset();
                    this.userSetTableAdapter.Fill(this.myRep_ODS_User_DataSet.UserSet);
                }

            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void SetPasswordButton_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO UserCredentialsSet VALUES(@uid,@pw)";
            String commandText2 = "SELECT UserID FROM UserSet WHERE Username = @param";

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Properties.Settings.Default.ConnectionString;
            conn.Open();
            try
            {
                SqlCommand command2 = new SqlCommand(commandText2, conn);
                command2.Parameters.AddWithValue("@param", username);

                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@uid", Convert.ToInt32(command2.ExecuteScalar()));
                command.Parameters.AddWithValue("@pw", newpwdBox.Text.ToString());

                command.ExecuteNonQuery();

                MessageBox.Show("all done!");
                mainController.SelectedTab = userMgmtPage;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
            conn.Close();

        }

        private void lnameUserBox_TextChanged(object sender, EventArgs e)
        {
            usernameUserBox.Text = fnameBox.Text.ToString() + ".x." + lnameUserBox.Text.ToString();

        }

        private void lnameUserBox_KeyUp(object sender, KeyEventArgs e)
        {
            usernameUserBox.Text = fnameUserBox.Text.ToString() + ".x." + lnameUserBox.Text.ToString();

        }

        private void fnameUserBox_KeyUp(object sender, KeyEventArgs e)
        {
            usernameUserBox.Text = fnameUserBox.Text.ToString() + ".x." + lnameUserBox.Text.ToString();

        }

        private void createnewhcoButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = newHCOPage;
        }

        private void hcpDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sConnection = Properties.Settings.Default.myRep_ODSConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand("", conn);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Id", hcpDataGridView.CurrentRow.Cells[0].Value.ToString());

                command.CommandText = "SELECT FirstName FROM dbo.HCPSet where hcpID = @Id";
                fnameLabel.Text = (String)command.ExecuteScalar();

                command.CommandText = "SELECT MiddleName FROM dbo.HCPSet where hcpID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                { mnameLabel.Text = (String)command.ExecuteScalar(); }
                else
                { mnameLabel.Text = "Brak danych!"; }

                command.CommandText = "SELECT LastName FROM dbo.HCPSet where hcpID = @Id";
                lnameLabel.Text = (String)command.ExecuteScalar();

                command.CommandText = "SELECT AcademicTitle FROM dbo.HCPSet where hcpID = @Id";
                hcpTitle.Text = (String)command.ExecuteScalar();

                command.CommandText = "SELECT Specialty FROM dbo.HCPSet where hcpID = @Id";
                hcpSpec.Text = (String)command.ExecuteScalar();

                command.CommandText = "SELECT Birthdate FROM dbo.HCPSet where hcpID = @Id";
                DateTime DateTime = (DateTime)command.ExecuteScalar();   
                dateBirthHCP.Text = Convert.ToString(DateTime.ToShortDateString());

                command.CommandText = "SELECT Gender FROM dbo.HCPSet where hcpID = @Id";
                genderHCP.Text = (String)command.ExecuteScalar();

                command.CommandText = "SELECT LanguageSpoken FROM dbo.HCPSet where hcpID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                { languageHCP.Text = (String)command.ExecuteScalar(); }
                else
                { languageHCP.Text = "Brak danych!"; }

                command.CommandText = "SELECT Email FROM dbo.HCPSet where hcpID = @Id";
                emailHCP.Text = (String)command.ExecuteScalar();

                command.CommandText = "SELECT PhoneNumber FROM dbo.HCPSet where hcpID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                { phoneHCP.Text = Convert.ToString(command.ExecuteScalar()); }
                else
                { phoneHCP.Text = "Brak danych!"; }

                SqlCommand command2 = new SqlCommand("dbo.HCPWorkPlaceDisplay",conn);
                command2.CommandType = CommandType.StoredProcedure;
                command2.Parameters.AddWithValue("@hcpID", hcpDataGridView.CurrentRow.Cells[0].Value);
                //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command2);
                dataAdapter.Fill(dt);
                dataGridViewHCPWorkPlace.DataSource = dt;

                command.CommandText = "SELECT KOL FROM dbo.HCPSet where hcpID = @Id";
                if (Convert.ToInt32(command.ExecuteScalar()) == 1)
                    kolHCP.Checked = true;
                else
                    kolHCP.Checked = false;

            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
        MessageBox.Show(text, "ERROR");
            }
}

        private void showDedicatedHCPToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.hCPSetTableAdapter.ShowDedicatedHCP(this.myRep_ODS_HCP_DataSet.HCPSet, param_show_HCP7ToolStripTextBox.Text);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void otherLangCheck_CheckedChanged(object sender, EventArgs e)
        {

            if (otherLangCheck.Checked == true)
            {
                otherLangTextBox.Enabled = true;
            }
            if (otherLangCheck.Checked == false)
            {
                otherLangTextBox.Enabled = false;
                otherLangTextBox.Text = "";
            }
        }

        private void hcoDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sConnection = Properties.Settings.Default.myRep_ODSConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand("", conn);
                command.CommandText = "SELECT Name FROM dbo.HCOSet where hcoID = @Id";
                command.Parameters.AddWithValue("@Id", hcoDataGridView.CurrentRow.Cells[1].Value.ToString());
                hcoNameLabel.Text = (String)command.ExecuteScalar();

                command.CommandText = "SELECT PhoneNumber FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoTelLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoTelLabel.Text = "Brak danych!";

                command.CommandText = "SELECT Email FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoEmailLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoEmailLabel.Text = "Brak danych!";

                command.CommandText = "SELECT Website FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoWWWLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoWWWLabel.Text = "Brak danych!";

                command.CommandText = "SELECT Range FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoRangeLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoRangeLabel.Text = "Brak danych!";

                command.CommandText = "SELECT Level FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoLevelLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoLevelLabel.Text = "Brak danych!";

                command.CommandText = "SELECT SpecialType FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoSpecialTypeLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoSpecialTypeLabel.Text = "Brak danych!";

                command.CommandText = "SELECT BedsAmount FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoNoBedLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoNoBedLabel.Text = "Brak danych!";

                command.CommandText = "SELECT EmployeesAmount FROM dbo.HCOSet where hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoNoEmpLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoNoEmpLabel.Text = "Brak danych!";

                command.CommandText = "SELECT AddressSet.Street from dbo.HCOSet join dbo.AddressSet on HCOSet.AddressID = AddressSet.addressID WHERE hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoStreetLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoStreetLabel.Text = "Brak danych!";

                command.CommandText = "SELECT AddressSet.City from dbo.HCOSet join dbo.AddressSet on HCOSet.AddressID = AddressSet.addressID WHERE hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoCityLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoCityLabel.Text = "Brak danych!";

                command.CommandText = "SELECT AddressSet.Territory from dbo.HCOSet join dbo.AddressSet on HCOSet.AddressID = AddressSet.addressID WHERE hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoTerrLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoTerrLabel.Text = "Brak danych!";

                command.CommandText = "SELECT AddressSet.Country from dbo.HCOSet join dbo.AddressSet on HCOSet.AddressID = AddressSet.addressID WHERE hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoCountryLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoCountryLabel.Text = "Brak danych!";

                command.CommandText = "SELECT AddressSet.ZipCode from dbo.HCOSet join dbo.AddressSet on HCOSet.AddressID = AddressSet.addressID WHERE hcoID = @Id";
                if (command.ExecuteScalar() != DBNull.Value)
                    hcoZipLabel.Text = Convert.ToString(command.ExecuteScalar());
                else
                    hcoZipLabel.Text = "Brak danych!";
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void addressDedicatedBookToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.addressSetTableAdapter.AddressDedicatedBook(this.myRep_ODS_Address_DataSet.AddressSet, paramToolStripTextBox.Text);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void addressDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sConnection = Properties.Settings.Default.myRep_ODSConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
            SqlCommand command21 = new SqlCommand("dbo.HCPunderAddress", conn);
            command21.CommandType = CommandType.StoredProcedure;
            command21.Parameters.AddWithValue("@AdID", addressDataGridView.CurrentRow.Cells[0].Value);
            //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
            DataTable dt3 = new DataTable();
            SqlDataAdapter dataAdapter3 = new SqlDataAdapter(command21);
            dataAdapter3.Fill(dt3);
            HCPunderAddressGridView.DataSource = dt3;
            HCPunderAddressGridView.Columns[0].Visible = false;

            SqlCommand command22 = new SqlCommand("dbo.HCOunderAddress", conn);
            command22.CommandType = CommandType.StoredProcedure;
            command22.Parameters.AddWithValue("@AdID", addressDataGridView.CurrentRow.Cells[0].Value);
            //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
            DataTable dt4 = new DataTable();
            SqlDataAdapter dataAdapter4 = new SqlDataAdapter(command22);
            dataAdapter4.Fill(dt4);
            HCOunderAddressGridView.DataSource = dt4;
            HCOunderAddressGridView.Columns[0].Visible = false;

            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
        MessageBox.Show(text, "ERROR");
            }
}

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void HCPunderAddressGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            new_meeting.Enabled = true;
        }

        private void selectAddressButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = select_address_Page;
            action_backTo = "NEWHCP_PAGE";
        }



        private void addressDedicatedBookToolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                this.addressSetTableAdapter.AddressDedicatedBook(this.myRep_ODS_Address_DataSet.AddressSet, paramToolStripTextBox1.Text);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (action_backTo == "NEWHCP_PAGE")
            {
                mainController.SelectedTab = newHCPPage;
                selectedAddressLabel.Text = Convert.ToString(setAddressGridView.CurrentRow.Cells[0].Value);
                selectedAddressFullLabel.Text = Convert.ToString(setAddressGridView.CurrentRow.Cells[1].Value) + ", " + Convert.ToString(setAddressGridView.CurrentRow.Cells[2].Value);
                action_backTo = "";
            }

        }

        private void AddressHCOButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = newAddressPage;
            action_backTo = "NEWHCO_PAGE";

        }

        private void CreateNewAddress_Click(object sender, EventArgs e)
        {
            String commandText = "INSERT INTO AddressSet VALUES(@Street,@City,@Territory,@Country,@ZipCode)";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@Street", StreetNEWaddressBox.Text.ToString());
                command.Parameters.AddWithValue("@City", CityNEWaddressBox.Text.ToString());
                //Territory powinno być brane z aktualnie zalogowanego usera
                command.Parameters.AddWithValue("@Territory", TerritoryNEWaddressBox.Text.ToString());
                command.Parameters.AddWithValue("@Country", CountryNEWaddressBOX.Text.ToString());
                command.Parameters.AddWithValue("@ZipCode", Convert.ToInt32(ZipNEWaddressBox.Text));
                command.ExecuteNonQuery();

                commandText = "SELECT max(addressID) FROM AddressSet";
                SqlCommand command2 = new SqlCommand(commandText, conn);
                SelectedHCO_AddressIDLabel.Text = Convert.ToString(command2.ExecuteScalar());
                SelectedHCO_AddressLabel.Text = StreetNEWaddressBox.Text.ToString() + ", " + CityNEWaddressBox.Text.ToString();

                if (action_backTo == "NEWHCO_PAGE") mainController.SelectedTab = newHCOPage;


                //CZYSZCZENIE PÓL
                StreetNEWaddressBox.Text = "";
                CityNEWaddressBox.Text = "";
                ZipNEWaddressBox.Text = "";
                
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void StreetNEWaddressBox_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewAddressButton();
        }

        private void CityNEWaddressBox_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewAddressButton();
        }

        private void TerritoryNEWaddressBox_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewAddressButton();
        }

        private void CountryNEWaddressBOX_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewAddressButton();
        }

        private void ZipNEWaddressBox_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateNewAddressButton();
        }

        private void HCONameBox_TextChanged(object sender, EventArgs e)
        {
            CreateHCOButton.Enabled = (HCONameBox.Text != "") && (Convert.ToInt32(SelectedHCO_AddressIDLabel.Text) > 0);
        }

        private void SelectedHCO_AddressIDLabel_TextChanged(object sender, EventArgs e)
        {
            CreateHCOButton.Enabled = (HCONameBox.Text != "") && (Convert.ToInt32(SelectedHCO_AddressIDLabel.Text) > 0);
        }
    }
}
