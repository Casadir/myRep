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
        public static int old_HCOID_duringHCPupdate;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'myRep_ODSDataSet.ProductSet' table. You can move, or remove it, as needed.
            this.productSetTableAdapter.Fill(this.myRep_ODSDataSet.ProductSet);
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
                    hcoDataGridView.Columns[0].Visible = false;


                    //Nadanie dostępu do odpowiednich zasobów na podstawie Job Title
                    command.CommandText = "SELECT JobTitle FROM dbo.UserSet WHERE Username = @param4";
                    command.Parameters.AddWithValue("@param4", UsernameBox.Text.ToString());
                    switch ((String)command.ExecuteScalar())
                    {
                        case "SYSADMIN":
                            {
                                myAccountsButton.Visible = true; myAccountsButton.Enabled = true;
                                userMgmtButton.Visible = true; userMgmtButton.Enabled = true;
                                productsMgmtButton.Visible = true; productsMgmtButton.Enabled = true;
                                SampleRightsErrorLabel.Visible = true; newProductButton.Enabled = false; editProductButton.Enabled = false; NewSampleButton.Enabled = false; EditSampleButton.Enabled = false; GiveSampleButton.Enabled = false;
                                break;
                            }
                        case "SnPA":
                            {
                                myAccountsButton.Visible = false; myAccountsButton.Enabled = false;
                                userMgmtButton.Visible = false; userMgmtButton.Enabled = false;
                                productsMgmtButton.Visible = true; productsMgmtButton.Enabled = true;
                                newProductButton.Enabled = true; editProductButton.Enabled = true; NewSampleButton.Enabled = true; EditSampleButton.Enabled = true; GiveSampleButton.Enabled = true;
                                break;
                            }
                        case "REP":
                            {
                                myAccountsButton.Visible = true; myAccountsButton.Enabled = true;
                                userMgmtButton.Visible = false; userMgmtButton.Enabled = false;
                                productsMgmtButton.Visible = true; productsMgmtButton.Enabled = true;
                                SampleRightsErrorLabel.Visible = true; newProductButton.Enabled = false; editProductButton.Enabled = false; NewSampleButton.Enabled = false; EditSampleButton.Enabled = false; GiveSampleButton.Enabled = false;
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
        private void ToogleCreateUpdateSampleButton()
        {
            newedit_Sample_Button.Enabled = (name_neweditdampleBox.Text != "") && (Qty_neweditdampleBox.Value > 0) && (Value_neweditdampleBox.Value > 0);
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
                hcoDataGridView.Columns[0].Visible = false;

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
                if (command.ExecuteScalar() != DBNull.Value)
                {
                    DateTime DateTime = (DateTime)command.ExecuteScalar();
                    dateBirthHCP.Text = Convert.ToString(DateTime.ToShortDateString());
                }
                else
                dateBirthHCP.Text = "Brak danych!";

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
                dataGridViewHCPWorkPlace.Columns[0].Visible = false;

                command.CommandText = "SELECT KOL FROM dbo.HCPSet where hcpID = @Id";
                if (Convert.ToInt32(command.ExecuteScalar()) == 1)
                    kolHCP.Checked = true;
                else
                    kolHCP.Checked = false;

                dataGridViewHCPWorkPlace.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
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
                command.Parameters.AddWithValue("@Id", hcoDataGridView.CurrentRow.Cells[0].Value.ToString());
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

                //WYPEŁNIANIE GRIDA Z PRACOWNIKAMI
                SqlCommand command2 = new SqlCommand("HCPinHCO", conn);
                command2.CommandType = CommandType.StoredProcedure;
                command2.Parameters.AddWithValue("@hcoID", Convert.ToInt32(hcoDataGridView.CurrentRow.Cells[0].Value.ToString()));
                //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command2);
                dataAdapter.Fill(dt);
                HCPinHCOGridView.DataSource = dt;
                HCPinHCOGridView.Columns[0].Visible = false;
                HCPinHCOGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                HCPinHCOGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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
            addressDedicatedBookToolStripButton.PerformClick();
            addressDataGridView.Columns[0].Visible = false;
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
            if (action_backTo == "EDITHCP_PAGE")
            {
                mainController.SelectedTab = editHCP_Page;
                selectedaddressID_editHCP.Text = Convert.ToString(setAddressGridView.CurrentRow.Cells[0].Value);
                selected_addressLabel_editHCP.Text = Convert.ToString(setAddressGridView.CurrentRow.Cells[1].Value) + ", " + Convert.ToString(setAddressGridView.CurrentRow.Cells[2].Value);
                action_backTo = "";
            }
            if (action_backTo == "EDITHCO_PAGE")
            {
                AddressID_editHCOBox.Text = Convert.ToString(setAddressGridView.CurrentRow.Cells[0].Value);
                AddressLabel_editHCOBox.Text = Convert.ToString(setAddressGridView.CurrentRow.Cells[1].Value) + ", " + Convert.ToString(setAddressGridView.CurrentRow.Cells[2].Value);
                mainController.SelectedTab = edit_HCO_Page;
                action_backTo = "";
            }

        }

        private void AddressHCOButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = newAddressPage;
            TerritoryNEWaddressBox.Text = loggedUserTerritory;
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
                //STWORZENIE NOWEGO ADRESU
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@Street", StreetNEWaddressBox.Text.ToString());
                command.Parameters.AddWithValue("@City", CityNEWaddressBox.Text.ToString());
                command.Parameters.AddWithValue("@Territory", TerritoryNEWaddressBox.Text.ToString());
                command.Parameters.AddWithValue("@Country", CountryNEWaddressBOX.Text.ToString());
                command.Parameters.AddWithValue("@ZipCode", Convert.ToInt32(ZipNEWaddressBox.Text));
                command.ExecuteNonQuery();

                commandText = "SELECT max(addressID) FROM AddressSet";
                SqlCommand command2 = new SqlCommand(commandText, conn);

                if (action_backTo == "NEWHCO_PAGE")
                {
                    SelectedHCO_AddressIDLabel.Text = Convert.ToString(command2.ExecuteScalar());
                    SelectedHCO_AddressLabel.Text = StreetNEWaddressBox.Text.ToString() + ", " + CityNEWaddressBox.Text.ToString();
                    mainController.SelectedTab = newHCOPage;
                }

                if (action_backTo == "EDITHCO_PAGE")
                {
                    //AKTUALIZACJA ADRESó NA HCP. STARY ADRES ZOSTAJE WYKASOWANY A NOWY ZASTĄPIONY W MIEJSCU.
                    String commandText2 = "UPDATE HCPSet SET AddressID = @newAdID WHERE AddressID = @oldAdID";
                    SqlCommand command3 = new SqlCommand(commandText2, conn);
                    command3.Parameters.AddWithValue("@newAdID", Convert.ToInt32(command2.ExecuteScalar()));
                    command3.Parameters.AddWithValue("@oldAdID", AddressID_editHCOBox.Text.ToString());
                    command3.ExecuteNonQuery();

                    //AKTUALIZACJA ADRESU NA OBECNYM HCO.
                    String commandText4 = "UPDATE HCOSet SET AddressID = @newAdID WHERE hcoID = @HCOID";
                    SqlCommand command4 = new SqlCommand(commandText4, conn);
                    command4.Parameters.AddWithValue("@newAdID", Convert.ToInt32(command2.ExecuteScalar()));
                    command4.Parameters.AddWithValue("@HCOID", Convert.ToInt32(hcoDataGridView.CurrentRow.Cells[0].Value.ToString()));
                    command4.ExecuteNonQuery();

                    //USUNIĘCIE STAREGO ADRESU
                    String commandText5 = "DELETE FROM AddressSet WHERE addressID = @oldAdID2";
                    SqlCommand command5 = new SqlCommand(commandText5, conn);
                    command5.Parameters.AddWithValue("@oldAdID2", AddressID_editHCOBox.Text.ToString());
                    command5.ExecuteNonQuery();

                    AddressID_editHCOBox.Text = Convert.ToString(command2.ExecuteScalar());
                    AddressLabel_editHCOBox.Text = StreetNEWaddressBox.Text.ToString() + ", " + CityNEWaddressBox.Text.ToString();
                    mainController.SelectedTab = edit_HCO_Page;
                }


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

        private void NewAssossiationButton_Click(object sender, EventArgs e)
        {
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                //Wyswietlanie tabelki z dedykowanymi HCO
                SqlCommand command = new SqlCommand("WhereHCO_isNotWorking", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@hcpID", Convert.ToInt32(hcpDataGridView.CurrentRow.Cells[0].Value));
                command.Parameters.AddWithValue("@territory", loggedUserTerritory);
                //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
                DataTable dt1 = new DataTable();
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(command);
                dataAdapter1.Fill(dt1);
                Assossiation_AddManualView.DataSource = dt1;
                Assossiation_AddManualView.Columns[0].Visible = false;
                Assossiation_AddManualView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //Uzupełnienie imienia i nazwiska na karcie do New Assossiation + przełączenie karty
                HCPname_newAssossiation.Text = HCPname_newAssossiation.Text + hcpDataGridView.CurrentRow.Cells[1].Value.ToString() + " " + hcpDataGridView.CurrentRow.Cells[2].Value.ToString();
                mainController.SelectedTab = new_HCPHCO_Assossiation_Page;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void addAssossiationButton_Click(object sender, EventArgs e)
        {
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                //Dodanie nowego powiązania
                String commandText = "INSERT INTO HCOHCP VALUES(@hcoid, @hcpid)";
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@hcoid", Convert.ToInt32(Assossiation_AddManualView.CurrentRow.Cells[0].Value.ToString()));
                command.Parameters.AddWithValue("@hcpid", Convert.ToInt32(hcpDataGridView.CurrentRow.Cells[0].Value.ToString()));
                command.ExecuteNonQuery();
               
                //Odświeżenie tabeli z miejscem pracy i powrót do MyAccounts
                SqlCommand command2 = new SqlCommand("dbo.HCPWorkPlaceDisplay", conn);
                command2.CommandType = CommandType.StoredProcedure;
                command2.Parameters.AddWithValue("@hcpID", hcpDataGridView.CurrentRow.Cells[0].Value);
                //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command2);
                dataAdapter.Fill(dt);
                dataGridViewHCPWorkPlace.DataSource = dt;
                dataGridViewHCPWorkPlace.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                mainController.SelectedTab = myAccountsPage;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void edithcpbutton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = editHCP_Page;
            if (fnameLabel.Text.ToString() != "Brak danych!") fname_editHCPBox.Text = fnameLabel.Text;
            if (mnameLabel.Text.ToString() != "Brak danych!") mname_editHCPBox.Text = mnameLabel.Text;
            if (lnameLabel.Text.ToString() != "Brak danych!") lname_editHCPBox.Text = lnameLabel.Text;
            if (hcpTitle.Text.ToString() != "Brak danych!") title_editHCPBox.Text = hcpTitle.Text;
            if (hcpSpec.Text.ToString() != "Brak danych!") spec_editHCPBox.Text = hcpSpec.Text;
            if (dateBirthHCP.Text.ToString() == "Brak danych!") birthday_editHCPBox.Text = DateTime.Today.Date.ToString(); else birthday_editHCPBox.Text = dateBirthHCP.Text;
            if (genderHCP.Text == "M") Male_editHCPBox.Checked = true; else female_editHCPBox.Checked = true;
            if (languageHCP.Text.ToString() != "Brak danych!") otherLang_editHCPBoxrlang.Text = languageHCP.Text;
            if (emailHCP.Text.ToString() != "Brak danych!") email_editHCPBox.Text = emailHCP.Text;
            tel_editHCPBox.Text = phoneHCP.Text;
            if (kolHCP.Checked == true) kol_editHCPBox.Checked = true;

            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                String commandText = "SELECT AddressID from HCPSet WHERE hcpID = @HCPID";
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@HCPID", hcpDataGridView.CurrentRow.Cells[0].Value.ToString());
                selectedaddressID_editHCP.Text = Convert.ToString(command.ExecuteScalar());

                String commandText4 = "SELECT hcoID from HCOSet WHERE AddressID = @adID";
                SqlCommand command4 = new SqlCommand(commandText4, conn);
                command4.Parameters.AddWithValue("@adID", selectedaddressID_editHCP.Text.ToString());
                old_HCOID_duringHCPupdate = Convert.ToInt32(command4.ExecuteScalar());

                String commandText2 = "SELECT Street from AddressSet WHERE addressID = @adID";
                SqlCommand command2 = new SqlCommand(commandText2, conn);
                command2.Parameters.AddWithValue("@adID", selectedaddressID_editHCP.Text.ToString());
                selected_addressLabel_editHCP.Text = Convert.ToString(command2.ExecuteScalar());

                String commandText3 = "SELECT City from AddressSet WHERE addressID = @adID";
                SqlCommand command3 = new SqlCommand(commandText3, conn);
                command3.Parameters.AddWithValue("@adID", selectedaddressID_editHCP.Text.ToString());
                selected_addressLabel_editHCP.Text = selected_addressLabel_editHCP.Text + ", " + Convert.ToString(command3.ExecuteScalar());
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void HCP_edit_BUTTON_Click(object sender, EventArgs e)
        {
            String commandText = "UPDATE HCPSet SET FirstName=@firstname, MiddleName=@middlename, LastName=@lastname, Gender=@gender, AcademicTitle=@academictitle, Birthdate=@birthdate, PhoneNumber=@phonenumber, Email=@email, KOL=@kol, Specialty=@specialty, LanguageSpoken=@lngspk, AddressID=@adID WHERE HCPID=@hcpid";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@hcpid", Convert.ToInt32(hcpDataGridView.CurrentRow.Cells[0].Value));
                command.Parameters.AddWithValue("@firstname", fname_editHCPBox.Text);
                command.Parameters.AddWithValue("@middlename", mname_editHCPBox.Text);
                command.Parameters.AddWithValue("@lastname", lname_editHCPBox.Text);
                if (Male_editHCPBox.Checked == true) command.Parameters.AddWithValue("@gender", "M"); else if (female_editHCPBox.Checked == true) command.Parameters.AddWithValue("@gender", "F");
                command.Parameters.AddWithValue("@academictitle", title_editHCPBox.Text);
                if (birthday_editHCPBox.Value.Date == DateTime.Today.Date) command.Parameters.AddWithValue("@birthdate", DBNull.Value); else command.Parameters.AddWithValue("@birthdate", birthday_editHCPBox.Value.Date);
                tel_editHCPBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (tel_editHCPBox.Text != "") command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(tel_editHCPBox.Text.ToString())); else command.Parameters.AddWithValue("@phonenumber", DBNull.Value);
                command.Parameters.AddWithValue("@email", email_editHCPBox.Text);
                command.Parameters.AddWithValue("@lngspk", otherLang_editHCPBoxrlang.Text);
                if (kol_editHCPBox.Checked)
                {
                    command.Parameters.AddWithValue("@kol", true);
                }
                else
                {
                    command.Parameters.AddWithValue("@kol", false);
                }
                command.Parameters.AddWithValue("@specialty", spec_editHCPBox.Text);
                command.Parameters.AddWithValue("@adID", selectedaddressID_editHCP.Text);
                command.ExecuteNonQuery();
            
                //UTWORZENIE NOWEJ AFILIACJI HCP-HCO, pobranie HCOID i dodanie nowego rekordu HCOHCP
                String commandText3 = "SELECT hcoID FROM HCOSet WHERE AddressID = @adID2";
                SqlCommand command3 = new SqlCommand(commandText3, conn);
                command3.Parameters.AddWithValue("@adID2", Convert.ToInt32(selectedaddressID_editHCP.Text.ToString()));
                int new_updated_hco_to_affiliate = Convert.ToInt32(command3.ExecuteScalar());

                //SPRAWDZENIE CZY CZASAMI NOWA AFILIACJA JUŻ NIE ISTNIEJE. Jeżeli tak - nie rób nic , jeżeli nie - utwórz
                String commandText5 = "SELECT count(*) FROM HCOHCP WHERE HCO_hcoID=@hcoID3 AND HCP_hcpID=@hcpID3";
                SqlCommand command5 = new SqlCommand(commandText5, conn);
                command5.Parameters.AddWithValue("@hcoID3", new_updated_hco_to_affiliate);
                command5.Parameters.AddWithValue("@hcpID3", Convert.ToInt32(hcpDataGridView.CurrentRow.Cells[0].Value.ToString()));
                if (Convert.ToInt32(command5.ExecuteScalar()) == 0)
                {
                    String commandText4 = "INSERT INTO HCOHCP VALUES ( @hcoID , @hcpID)";
                    SqlCommand command4 = new SqlCommand(commandText4, conn);
                    command4.Parameters.AddWithValue("@hcoID", new_updated_hco_to_affiliate);
                    command4.Parameters.AddWithValue("@hcpID", Convert.ToInt32(hcpDataGridView.CurrentRow.Cells[0].Value.ToString()));
                    command4.ExecuteNonQuery();
                }
                conn.Close();
                myAccounts_Controller.SelectedTab = hcpPage;
                this.myRep_ODS_HCP_DataSet.Reset();
                this.hCPSetTableAdapter.Fill(this.myRep_ODS_HCP_DataSet.HCPSet);
                mainController.SelectedTab = myAccountsPage;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = select_address_Page;
            action_backTo = "EDITHCP_PAGE";

        }

        private void dataGridViewHCPWorkPlace_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            String commandText = "SELECT AddressID FROM HCOSet WHERE hcoID = @HCOID";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                //POBIERZ ADDRESSID DLA WYBRANEGO HCO
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@HCOID", Convert.ToInt32(dataGridViewHCPWorkPlace.CurrentRow.Cells[0].Value));
                int HCOaddressID = Convert.ToInt32(command.ExecuteScalar());

                //POBIERZ ADDRESSID DLA WYBRANEGO HCP
                String commandText2 = "SELECT AddressID FROM HCPSet WHERE hcpID = @HCPID";
                SqlCommand command2 = new SqlCommand(commandText2, conn);
                command2.Parameters.AddWithValue("@HCPID", Convert.ToInt32(hcpDataGridView.CurrentRow.Cells[0].Value.ToString()));
                int HCPaddressID = Convert.ToInt32(command2.ExecuteScalar());

                //JEŻELI OBA ADRESY SĄ TAKIE SAME TO ZABLOKUJ MOŻLIWOŚĆ USUNIĘCIA AFILIACJI - POWINNO SIĘ TO ROBIĆ POPRZEZ EDYCJE HCP
                //JEŻELI ADRESY SĄ RÓŻNE TO USUŃ POWIĄZANIE Z TABELI HCOHCP
                if (HCOaddressID.Equals(HCPaddressID))
                {
                    RemoveAssossiationButton.Enabled = false;
                    AffiliationErrorLabel.Visible = true;
                }
                else
                {
                    RemoveAssossiationButton.Enabled = true;
                    AffiliationErrorLabel.Visible = false;

                }
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
}

        private void RemoveAssossiationButton_Click(object sender, EventArgs e)
        {
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                String commandText3 = "DELETE FROM HCOHCP WHERE HCO_hcoID = @hcoID AND HCP_hcpID = @hcpID1 ";
                SqlCommand command3 = new SqlCommand(commandText3, conn);
                command3.Parameters.AddWithValue("@hcpID1", Convert.ToInt32(hcpDataGridView.CurrentRow.Cells[0].Value.ToString()));
                command3.Parameters.AddWithValue("@hcoID", Convert.ToInt32(dataGridViewHCPWorkPlace.CurrentRow.Cells[0].Value.ToString()));
                command3.ExecuteNonQuery();

                SqlCommand command = new SqlCommand("dbo.HCPWorkPlaceDisplay", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@hcpID", hcpDataGridView.CurrentRow.Cells[0].Value);
                //WYPEŁNIANIE GRIDA Z MIEJSCEM PRACY
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt);
                dataGridViewHCPWorkPlace.DataSource = dt;
                dataGridViewHCPWorkPlace.Columns[0].Visible = false;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
        MessageBox.Show(text, "ERROR");
            }
}

        private void editHCObutton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = edit_HCO_Page;
            hconame_editHCO.Text = hcoNameLabel.Text;
            if (hcoRangeLabel.Text.ToString() != "Brak danych!") range_editHCOBox.Text = hcoRangeLabel.Text; else range_editHCOBox.Text = "";
            if (hcoLevelLabel.Text.ToString() != "Brak danych!")
                if (hcoLevelLabel.Text.ToString() == "1")
                    level_editHCOBox.SelectedIndex = 1;
                else if (hcoLevelLabel.Text.ToString() == "2")
                    level_editHCOBox.SelectedIndex = 2;
                else if (hcoLevelLabel.Text.ToString() == "3")
                    level_editHCOBox.SelectedIndex = 3;
                else
                {
                    level_editHCOBox.Text = "";
                }
            if (hcoSpecialTypeLabel.Text.ToString() != "Brak danych!") type_editHCOBox.Text = hcoSpecialTypeLabel.Text; else type_editHCOBox.Text = "";
            if (hcoNoBedLabel.Text.ToString() != "Brak danych!") beds_editHCOBox.Text = hcoNoBedLabel.Text; else beds_editHCOBox.Text = "0";
            if (hcoNoEmpLabel.Text.ToString() != "Brak danych!") employees_editHCOBox.Text = hcoNoEmpLabel.Text; else employees_editHCOBox.Text = "0";
            if (hcoTelLabel.Text.ToString() != "Brak danych!") tel_editHCOBox.Text = hcoTelLabel.Text; else tel_editHCOBox.Text = "";
            if (hcoEmailLabel.Text.ToString() != "Brak danych!") email_editHCOBox.Text = hcoEmailLabel.Text; else email_editHCOBox.Text = "";
            if (hcoWWWLabel.Text.ToString() != "Brak danych!") www_editHCOBox.Text = hcoWWWLabel.Text; else www_editHCOBox.Text = "";
            AddressLabel_editHCOBox.Text = hcoStreetLabel.Text + ", " + hcoCityLabel.Text;

            //POBRANIE AddressID dla HCO
            string sConnection = Properties.Settings.Default.myRep_ODSConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand("", conn);
                command.CommandText = "SELECT AddressID FROM dbo.HCOSet WHERE hcoID = @Id";
                command.Parameters.AddWithValue("@Id", hcoDataGridView.CurrentRow.Cells[0].Value.ToString());
                AddressID_editHCOBox.Text = Convert.ToString(command.ExecuteScalar());
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
        MessageBox.Show(text, "ERROR");
            }
}

        private void select_address_editHCOBox_Click(object sender, EventArgs e)
        {
            action_backTo = "EDITHCO_PAGE";
            StreetNEWaddressBox.Text = hcoStreetLabel.Text;
            CityNEWaddressBox.Text = hcoCityLabel.Text;
            TerritoryNEWaddressBox.Text = loggedUserTerritory;
            CountryNEWaddressBOX.Text = hcoCountryLabel.Text;
            ZipNEWaddressBox.Text = hcoZipLabel.Text;
            mainController.SelectedTab = newAddressPage;
        }

        private void edit_OK_HCOButton_Click(object sender, EventArgs e)
        {
            String commandText = "UPDATE HCOSet SET Name=@name, PhoneNumber=@phonenumber, Email=@email, Website=@www, Range=@rng, Level=@lvl, SpecialType=@spectype, BedsAmount=@bedsno, EmployeesAmount=@empno WHERE hcoID=@HCOID";
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@HCOID", Convert.ToInt32(hcoDataGridView.CurrentRow.Cells[0].Value));
                command.Parameters.AddWithValue("@name", hconame_editHCO.Text.ToString());
                if (range_editHCOBox.Text.ToString() != "") command.Parameters.AddWithValue("@rng", range_editHCOBox.Text.ToString()); else command.Parameters.AddWithValue("@rng",DBNull.Value);
                tel_editHCOBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (tel_editHCOBox.Text != "") command.Parameters.AddWithValue("@phonenumber", Convert.ToInt32(tel_editHCOBox.Text.ToString())); else command.Parameters.AddWithValue("@phonenumber", DBNull.Value);
                if (email_editHCOBox.Text.ToString() != "") command.Parameters.AddWithValue("@email", email_editHCOBox.Text.ToString()); else command.Parameters.AddWithValue("@email", DBNull.Value);
                if (www_editHCOBox.Text.ToString() != "") command.Parameters.AddWithValue("@www", www_editHCOBox.Text.ToString()); else command.Parameters.AddWithValue("@www", DBNull.Value);

                if (string.IsNullOrEmpty(level_editHCOBox.Text.ToString()))
                    command.Parameters.AddWithValue("@lvl", DBNull.Value);
                else
                {
                    if (level_editHCOBox.SelectedIndex == 1)
                        command.Parameters.AddWithValue("@lvl", 1);
                    else if (level_editHCOBox.SelectedIndex == 2)
                        command.Parameters.AddWithValue("@lvl", 2);
                    else if (level_editHCOBox.SelectedIndex == 3)
                        command.Parameters.AddWithValue("@lvl", 3);
                }
                if (type_editHCOBox.Text.ToString() != "") command.Parameters.AddWithValue("@spectype", type_editHCOBox.Text.ToString()); else command.Parameters.AddWithValue("@spectype", DBNull.Value);
                if (beds_editHCOBox.Value == 0) command.Parameters.AddWithValue("@bedsno", DBNull.Value); else command.Parameters.AddWithValue("@bedsno", beds_editHCOBox.Text.ToString());
                if (employees_editHCOBox.Value == 0) command.Parameters.AddWithValue("@empno", DBNull.Value); else command.Parameters.AddWithValue("@empno", employees_editHCOBox.Text.ToString());
                command.ExecuteNonQuery();
                conn.Close();
                this.myRep_ODS_HCP_DataSet.Reset();
                this.hCPSetTableAdapter.Fill(this.myRep_ODS_HCP_DataSet.HCPSet);
                mainController.SelectedTab = myAccountsPage;
                myAccounts_Controller.SelectedTab = hcoPage;
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void productsMgmtButton_Click(object sender, EventArgs e)
        {
            mainController.SelectedTab = products_Mgmt_Page;

        }

        private void addNewProductButton_Click(object sender, EventArgs e)
        {
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                if (action_backTo == "NEWPRODUCT_PAGE")
                {
                    String commandText = "INSERT INTO ProductSet VALUES(@ProductName,@AntiDisease,@Manufacturer,@MainIngredient)";
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.Parameters.AddWithValue("@ProductName", productname_newProductBox.Text.ToString());
                    if (string.IsNullOrEmpty(category_newProductBox.Text.ToString())) command.Parameters.AddWithValue("@AntiDisease", DBNull.Value); else command.Parameters.AddWithValue("@AntiDisease", category_newProductBox.Text.ToString());
                    if (string.IsNullOrEmpty(manufacturer_newproductBox.Text.ToString())) command.Parameters.AddWithValue("@Manufacturer", DBNull.Value); else command.Parameters.AddWithValue("@Manufacturer", manufacturer_newproductBox.Text.ToString());
                    if (string.IsNullOrEmpty(MainIngredient_newProductBox.Text.ToString())) command.Parameters.AddWithValue("@MainIngredient", DBNull.Value); else command.Parameters.AddWithValue("@MainIngredient", MainIngredient_newProductBox.Text.ToString());
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = products_Mgmt_Page;
                    action_backTo = "";
                    productname_newProductBox.Text = "";
                    category_newProductBox.Text = "";
                    manufacturer_newproductBox.Text = "";
                    MainIngredient_newProductBox.Text = "";
                    this.myRep_ODSDataSet.Reset();
                    this.productSetTableAdapter.Fill(this.myRep_ODSDataSet.ProductSet);
                    productsDataGridView.Columns[0].Visible = false;
                }
                if (action_backTo == "EDITPRODUCT_PAGE")
                {
                    String commandText = "UPDATE ProductSet SET ProductName = @ProductName, AntiDisease = @AntiDisease, Manufacturer = @Manufacturer, MainIngredient = @MainIngredient WHERE productID = @pID";
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.Parameters.AddWithValue("@pID", Convert.ToInt32(productsDataGridView.CurrentRow.Cells[0].Value.ToString()));
                    command.Parameters.AddWithValue("@ProductName", productname_newProductBox.Text.ToString());
                    if (string.IsNullOrEmpty(category_newProductBox.Text.ToString())) command.Parameters.AddWithValue("@AntiDisease", DBNull.Value); else command.Parameters.AddWithValue("@AntiDisease", category_newProductBox.Text.ToString());
                    if (string.IsNullOrEmpty(manufacturer_newproductBox.Text.ToString())) command.Parameters.AddWithValue("@Manufacturer", DBNull.Value); else command.Parameters.AddWithValue("@Manufacturer", manufacturer_newproductBox.Text.ToString());
                    if (string.IsNullOrEmpty(MainIngredient_newProductBox.Text.ToString())) command.Parameters.AddWithValue("@MainIngredient", DBNull.Value); else command.Parameters.AddWithValue("@MainIngredient", MainIngredient_newProductBox.Text.ToString());
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = products_Mgmt_Page;
                    action_backTo = "";
                    productname_newProductBox.Text = "";
                    category_newProductBox.Text = "";
                    manufacturer_newproductBox.Text = "";
                    MainIngredient_newProductBox.Text = "";
                    this.myRep_ODSDataSet.Reset();
                    this.productSetTableAdapter.Fill(this.myRep_ODSDataSet.ProductSet);
                    productsDataGridView.Columns[0].Visible = false;
                }

                conn.Close();
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void newProductButton_Click(object sender, EventArgs e)
        {
            action_backTo = "NEWPRODUCT_PAGE";
            mainController.SelectedTab = new_product_page;
        }

        private void editProductButton_Click(object sender, EventArgs e)
        {
            action_backTo = "EDITPRODUCT_PAGE";
            productname_newProductBox.Text = productsDataGridView.CurrentRow.Cells[1].Value.ToString();
            category_newProductBox.Text = productsDataGridView.CurrentRow.Cells[2].Value.ToString();
            manufacturer_newproductBox.Text = productsDataGridView.CurrentRow.Cells[3].Value.ToString();
            MainIngredient_newProductBox.Text = productsDataGridView.CurrentRow.Cells[4].Value.ToString();
            mainController.SelectedTab = new_product_page;
        }

        private void productsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sConnection = Properties.Settings.Default.myRep_ODSConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand("dbo.SamplesPerProduct", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pID", productsDataGridView.CurrentRow.Cells[0].Value);
                //WYPEŁNIANIE GRIDA Z SAMPLAMI
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt);
                samplelistGridView.DataSource = dt;
                samplelistGridView.Columns[0].Visible = false;
                samplelistGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                conn.Close();
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void NewSampleButton_Click(object sender, EventArgs e)
        {
            action_backTo = "NEWSAMPLE_PAGE";
            mainController.SelectedTab = new_Sample_Page;
        }

        private void name_neweditdampleBox_TextChanged(object sender, EventArgs e)
        {
            ToogleCreateUpdateSampleButton();
        }

        private void Qty_neweditdampleBox_ValueChanged(object sender, EventArgs e)
        {
            ToogleCreateUpdateSampleButton();
        }

        private void Qty_neweditdampleBox_KeyUp(object sender, KeyEventArgs e)
        {
            ToogleCreateUpdateSampleButton();
        }

        private void Value_neweditdampleBox_ValueChanged(object sender, EventArgs e)
        {
            ToogleCreateUpdateSampleButton();
        }

        private void Value_neweditdampleBox_KeyUp(object sender, KeyEventArgs e)
        {
            ToogleCreateUpdateSampleButton();
        }

        private void newedit_Sample_Button_Click(object sender, EventArgs e)
        {
            string sConnection = Properties.Settings.Default.ConnectionString;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = sConnection;
            conn.Open();
            try
            {
                if (action_backTo == "NEWSAMPLE_PAGE")
                {
                    String commandText = "INSERT INTO SampleSet VALUES(@SampleName, @pID, @Qty, @Val)";
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.Parameters.AddWithValue("@SampleName", name_neweditdampleBox.Text.ToString());
                    command.Parameters.AddWithValue("@pID", Convert.ToInt32(productsDataGridView.CurrentRow.Cells[0].Value.ToString()));
                    command.Parameters.AddWithValue("@Qty", Convert.ToInt32(Qty_neweditdampleBox.Value.ToString()));
                    command.Parameters.AddWithValue("@Val", Convert.ToInt32(Value_neweditdampleBox.Value.ToString()));
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = products_Mgmt_Page;
                    action_backTo = "";
                    name_neweditdampleBox.Text = "";
                    Qty_neweditdampleBox.Value = 0;
                    Value_neweditdampleBox.Value = 0;
                    //REFRESH SAMPLE GRIDA
                    SqlCommand command2 = new SqlCommand("dbo.SamplesPerProduct", conn);
                    command2.CommandType = CommandType.StoredProcedure;
                    command2.Parameters.AddWithValue("@pID", productsDataGridView.CurrentRow.Cells[0].Value);
                    //WYPEŁNIANIE GRIDA Z SAMPLAMI
                    DataTable dt = new DataTable();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command2);
                    dataAdapter.Fill(dt);
                    samplelistGridView.DataSource = dt;
                    samplelistGridView.Columns[0].Visible = false;
                    samplelistGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
                if (action_backTo == "EDITSAMPLE_PAGE")
                {
                    String commandText = "UPDATE SampleSet SET SampleName=@sampname, QtyPerBox=@qty, Value=@val WHERE ProductID=@pID2";
                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.Parameters.AddWithValue("@sampname", name_neweditdampleBox.Text.ToString());
                    command.Parameters.AddWithValue("@pID2", productsDataGridView.CurrentRow.Cells[0].Value.ToString());
                    command.Parameters.AddWithValue("@qty", Convert.ToInt32(Qty_neweditdampleBox.Value.ToString()));
                    command.Parameters.AddWithValue("@val", Convert.ToInt32(Value_neweditdampleBox.Value.ToString()));
                    command.ExecuteNonQuery();
                    mainController.SelectedTab = products_Mgmt_Page;
                    action_backTo = "";
                    name_neweditdampleBox.Text = "";
                    Qty_neweditdampleBox.Value = 0;
                    Value_neweditdampleBox.Value = 0;
                    //REFRESH SAMPLE GRIDA
                    SqlCommand command2 = new SqlCommand("dbo.SamplesPerProduct", conn);
                    command2.CommandType = CommandType.StoredProcedure;
                    command2.Parameters.AddWithValue("@pID", productsDataGridView.CurrentRow.Cells[0].Value);
                    //WYPEŁNIANIE GRIDA Z SAMPLAMI
                    DataTable dt = new DataTable();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command2);
                    dataAdapter.Fill(dt);
                    samplelistGridView.DataSource = dt;
                    samplelistGridView.Columns[0].Visible = false;
                    samplelistGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
                conn.Close();
            }
            catch (SqlException er)
            {
                String text = "There was an error reported by SQL Server, " + er.Message;
                MessageBox.Show(text, "ERROR");
            }
        }

        private void EditSampleButton_Click(object sender, EventArgs e)
        {
            action_backTo = "EDITSAMPLE_PAGE";
            name_neweditdampleBox.Text = samplelistGridView.CurrentRow.Cells[1].Value.ToString();
            Value_neweditdampleBox.Value = Convert.ToInt32(samplelistGridView.CurrentRow.Cells[2].Value.ToString());
            Qty_neweditdampleBox.Value = Convert.ToInt32(samplelistGridView.CurrentRow.Cells[3].Value.ToString());
            mainController.SelectedTab = new_Sample_Page;

        }
    }
}
