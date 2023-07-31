using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLServerTesterApp
{
    public partial class frmQueryTester : Form
    {
        public frmQueryTester()
        {
            InitializeComponent();
        }

        //  Global database variables

        //  Taken from URL:
        //  https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection?view=dotnet-plat-ext-6.0
        //  A SqlConnection object represents a unique session
        //  to a SQL Server data source. With a client/server
        //  database system, it is equivalent to a network
        //  connection to the server. SqlConnection is used
        //  together with SqlDataAdapter and SqlCommand to
        //  increase performance when connecting to a Microsoft
        //  SQL Server database. For all third-party SQL
        //  Server products and other OLE DB-supported data
        //  sources, use OleDbConnection.
        SqlConnection conn;

        //  Taken from URL:
        //  https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand?view=dotnet-plat-ext-6.0
        //  When an instance of SqlCommand is created, the
        //  read/write properties are set to their initial values. 
        SqlCommand queryCommand;

        //  Taken from the following URL:
        //  https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldataadapter?view=dotnet-plat-ext-6.0
        //  The SqlDataAdapter, serves as a bridge between a
        //  DataSet and SQL Server for retrieving and saving data.
        //
        //  The SqlDataAdapter provides this bridge by mapping Fill,
        //  which changes the data in the DataSet to match the data
        //  in the data source, and Update, which changes the data
        //  in the data source to match the data in the DataSet,
        //  using the appropriate Transact-SQL statements against
        //  the data source.
        SqlDataAdapter queryAdapter;

        //  Taken from the following URL:
        //  https://www.c-sharpcorner.com/UploadFile/mahesh/datatable-in-C-Sharp/
        //
        //  The DataTable class in C# ADO.NET is a database table
        //  representation and provides a collection of columns and
        //  rows to store data in a grid form. 
        DataTable table;

        private void frmQueryTester_Load(object sender, EventArgs e)
        {
            //  Set the connection string
            var connString = @"Data Source =(LocalDb)\MSSQLLocalDB;Initial Catalog=MMABooks; Integrated Security=SSPI";

            //  Create SqlConnection based on connString above
            conn = new SqlConnection(connString);

            //  Open the connection
            conn.Open();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            AttemptToExecuteQuery();
        }

        private void AttemptToExecuteQuery()
        {
            //  "Wipe out" whatever is currently
            //  in the queryCommand.
            queryCommand = null;

            //  Instantiate new SqlDataAdapter()
            queryAdapter = new SqlDataAdapter();

            //  Instantiate new DataTable()
            table = new DataTable();

            //  Check for an empty query input textbox
            if (txtQuery.Text.Trim() == "")
            {
                ShowMessage("Nothing Entered Into Query TextBox",
                            "QUERY TEXTBOX EMPTY");
                txtQuery.Focus();
                return;
            }

            //  There was something inputted into
            //  the query textbox. Handle it.
            try
            {
                //  Instantiate a new SqlCommand object
                queryCommand = new SqlCommand(txtQuery.Text, conn);

                //  Use the queryAdapter as a "bridge" between
                //  the program and the MMABooks database.
                queryAdapter.SelectCommand = queryCommand;

                //  Fill the database with the
                //  output of the queryAdapter.
                queryAdapter.Fill(table);

                //  Set the data source for
                //  the dgvMMABooks datagridview.
                dgvMMABooks.DataSource = table;

                //  Set the number of records
                //  returned in associated label.
                lblNumberOfRecords.Text =
                            table.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                ShowMessage("Invalid Query In TextBox\n\n" +
                            ex.Message,
                            "QUERY TEXTBOX CONTENTS INVALID");
                txtQuery.Text = "";
                txtQuery.Focus(); 
                return;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTheGUI();
        }

        private void ClearTheGUI()
        {
            //  https://www.codeproject.com/Questions/332902/how-to-clear-datagridview-in-csharp
            if (this.dgvMMABooks.DataSource != null)
            {
                this.dgvMMABooks.DataSource = null;
            }
            else
            {
                this.dgvMMABooks.Rows.Clear();
            }

            lblNumberOfRecords.Text = "0";
            txtQuery.Text = "";
            txtQuery.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ExitProgramOrNot();
        }

        private void ExitProgramOrNot()
        {
            DialogResult dialog = MessageBox.Show(
                "Do You Really Want To Exit?",
                "EXIT NOW?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                //  Close connection
                conn.Close();
                Application.Exit();
            }
        }

        private void ShowMessage(string msg, string title)
        {
            MessageBox.Show(msg, title,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }
}
