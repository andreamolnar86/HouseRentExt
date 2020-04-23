using System;
using System.Data;
using System.Data.SqlClient;

namespace HouseRent
{
    /// <summary>
    /// Summary description for DAL.
    /// </summary>
    public abstract class DALGen
    {
        //protected lathatosagi szabaly-csak a tartalmazó osztályból és a származtatott osztályokban lehet használni
        protected static bool isConnected;

        //ebben a projektben egy SQL szerveren futo adatbazishoz akarunk csatlakozni
        //SQL adatbazishoz SqlConnection objektumokkal kapcsolodhatunk
        protected static SqlConnection sqlConnection;


        //kapcsolati karakterlanc (Connection string): MSSQL szerver neve, adatbazis neve, kapcsolodas modja

        //Windows Authentication eseten:
        protected string strSqlConn = "Data Source=DESKTOP-7PV9FR8\\SQLEXPRESS;Initial Catalog=Berles;Integrated Security=SSPI"; //Windows Authentication esetén
        //protected string strSqlConn = "Data Source=(local);Initial Catalog=Berlesek;Integrated Security=SSPI";                                                                                                                        // protected string strSqlConn = "Data Source=ANDI-PC;Initial Catalog=Berles;Integrated Security=SSPI"; //Windows Authentication esetén

        //SQL Server Authentication-nel valo kapcsolodas eseten:
        //protected string strSqlConn = "Data Source=(local);Initial Catalog=Berlesek;uid=andi;pwd=1234"; //SQL Server Authentication esetén
        //protected string strSqlConn = "Data Source=DESKTOP-7PV9FR8\\SQLEXPRESS;Initial Catalog=Berles;uid=andi;pwd=1234"; 


        //parancsobjektum-ezek segitsegevel tudunk SQL utasitasokat vegrehajtani
        //parancsobjektumok a kapcsolati objektumok segitsegevel tartanak kapcsolatot 
        //az adatbazissal
        //protected System.Data.SqlClient.SqlCommand sqlCommand;

        //a kovetkezo ket objektum segitsegevel az adatbazisbol adatokat olvashatunk be
        //illetve tarolhatjuk ezeket
        //protected System.Data.SqlClient.SqlDataReader sqlDataReader;
        //protected System.Data.DataSet dataSet;

        //kapcsolatot teremt az adatbazissal, ha ez meg nem tortent meg eddig         
        protected void CreateConnection(ref string errMess)
        {
            // Create the Connection if is was not already created.
            if (isConnected != true)
            {
                try
                {
                    //uj OleDbConnection objektum letrehozasa
                    //ezzel a konstruktorral letrehoztuk a kapcsolati objektumot es beallitottuk a ConnectionString
                    //tulajdonsagat is
                    //ez csak akkor allithato be, ha a kapcsolati objektum zarva van
                    sqlConnection = new SqlConnection(strSqlConn);
                    //sqlConnection.ConnectionString=strSqlConn

                    //kapcsolodunk az adatbazishoz
                    sqlConnection.Open();
                    //igazat terithetunk vissza, hisz a kapcsolat gond nelkul letrejott
                    errMess = "OK";
                }
                catch (SqlException ex)
                {
                    //tovabbkuldjuk az errorMessage-t
                    errMess = ex.Message;
                }
                finally
                // A finally block nagyon lenyeges, 
                // mivel ez biztositja, hogy a kapcsolat akkor is bezarodik, 
                // ha barmilyen hiba fellepett (ami kivetelt valtott ki).
                {
                    //igyekezzunk minel kevesebb ideig nyitva hagyni a kapcsolatot,
                    //mert igy sporolunk az adatbazis eroforrasaival
                    if (sqlConnection != null)
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }
               
        /// <summary>
        /// Open the Connection when the state is not already open.
        /// </summary>
        private void OpenConnection(ref string errMess)
        {
            // Open the Connection when the state is not already open.
            if (isConnected == false)
            {
                try
                {
                    sqlConnection.Open();
                    isConnected = true;
                    errMess = "OK";
                }
                catch (SqlException ex)
                {
                    errMess = ex.Message;
                }
            }
        }

        //bezarjuk a kapcsolatot, ha meg nyitva van
        //internal lathatosagi szabaly-csak a tartalmazo osztalybol lehet hozzaferni
        private void CloseConnection()
        {
            // Close the Connection when the connection is opened.
            if (isConnected == true)
            {
                sqlConnection.Close();
                isConnected = false;
            }
        }

        //vegrehajtja az elso parameterben megadott stringben levo select utasitast
        //egy eredmenyhalmaz jon letre ezaltal, melyet egy DataReader objektumban tarolunk el
        //es teritunk vissza
        protected SqlDataReader ExecuteReader(string sQuery, ref string errMess)
        {
            SqlDataReader sqlDataReader = null;
            try
            {
                OpenConnection(ref errMess);//megnyitja az adatbazis-kapcsolatot, ha meg nincs megnyitva) 

                //uj sqlcommand objektumot igy is letrehozhatunk:
                //sQuery-tartalmazza az SQL utasitast, melybol adatot kerunk le
                //mySqlConn-SqlConnection objektum 
                SqlCommand sqlCommand = new SqlCommand(sQuery, sqlConnection);
                //myComm OleDbCommand CommandText tulajdonsagat atkuldi az m_Conn
                //connection obj-nak es letrehoz egy DataReadert

                //ExecuterReader tagfgv-olyan select utasitasok vegrehajtasara szolgal, melyek
                //eredmenyhalmazt adnak vissza-az eredmenyhalmaz egy DataReader objektumban ter vissza
                sqlDataReader = sqlCommand.ExecuteReader();
                errMess = "OK";
            }
            catch (Exception e)
            {
                errMess = e.Message;
                CloseDataReader(sqlDataReader);
            }
            return sqlDataReader;
        }

        /// <summary>
        /// Closes the data reader given as a parameter, and also closes the connection
        /// </summary>
        /// <param name="rdr">The SqlDataReader to be closed</param>
        protected void CloseDataReader(SqlDataReader rdr)
        {
            if (rdr != null)
                rdr.Close();
            CloseConnection();
        }

        /// <summary>
        /// Executes a given query, and returns the result in a dataset.
        /// A megfelelo select utasitassal feltolti a DataSet-et a megfelelo adatokkal egy DataAdapter objektum segitsegevel
        /// </summary>
        /// <param name="sQuery"> The query to be executed </param>
        /// <param name="sTableName"> The name of the DataSet. </param>
        /// <param name="ErrMess"> Output error message </param>
        /// <returns></returns>
        protected DataSet ExecuteDS(string query, ref string errMess)
        {
            //letrehozunk egy Dataset objektumot-a parameter segitsegevel 
            //beallitottuk a DataSetName tulajdonsagat a DataSet objektumunknak
            DataSet dataSet = new DataSet();
            try
            {
                OpenConnection(ref errMess);

                //Create a SqlDataAdapter for the Houses table.
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, sqlConnection);

                //Fill()-DataSet tagfgv-e-osszehangolja a DataSet objektum sorait az adatbazis soraival
                //a visszaadott int ertek azt mutatja, hogy az adatbazis hany soranak osszehangolasa tortent meg
                //VAGYIS-a Fill-el tudjuk feltolteni a Dataset objektumot adatokkal
                dataAdapter.Fill(dataSet);

                errMess = "OK";
            }
            catch (SqlException e)
            {
                errMess = e.Message;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    CloseConnection();
                }
            }
            return dataSet;
        }

        /// <summary>
        /// Executes a given insert/update/delete command. 
        /// (ErrMess is "OK" if no exception occured)
        /// </summary>
        /// <param name="sDelete">The command to be executed</param>
        protected void ExecuteNonQuery(string command, ref string errMess)
        {
            try
            {
                OpenConnection(ref errMess);
                SqlCommand sqlCommand = new SqlCommand(command, sqlConnection);
                sqlCommand.ExecuteNonQuery();
                errMess = "OK";
            }
            catch (SqlException ex)
            {
                errMess = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }

        //az elobbit megoldhatjuk parameterezett query-vel is - ezzel az SQL Injection-oket is ki tudjuk kerulni
        protected void ExecuteNonQueryParametrized(string command, string[] parameterNames, string[] parameterValues, ref string errMess)
        {
            try
            {
                OpenConnection(ref errMess);
                SqlCommand sqlCommand = new SqlCommand(command, sqlConnection);
                for (int i = 0; i < parameterNames.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                }
                sqlCommand.ExecuteNonQuery();
                errMess = "OK";
            }
            catch (SqlException ex)
            {
                errMess = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        ///  Executes a given select command that is expected to return scalar values (e.g. COUNT, AVG, MIN, MAX, SUM)
        ///  and returns the scalar value. 
        /// (ErrMess is "OK" if no exception occured)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected object ExecuteScalar(string query, ref string errMess)
        {
            object value;
            try
            {
                OpenConnection(ref errMess);
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                value = sqlCommand.ExecuteScalar();
                errMess = "OK";
            }
            catch (SqlException e)
            {
                value = null;
                errMess = e.Message;
            }
            finally
            {
                CloseConnection();
            }
            return value;
        }

        //az elobbit megoldhatjuk parameterezett query-vel is - ezzel az SQL Injection-oket is ki tudjuk kerulni
        protected object ExecuteScalarParametrized(string query, string[] parameterNames, string[] parameterValues, ref string errMess)
        {
            object value;
            try
            {
                OpenConnection(ref errMess);
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                for (int i = 0; i < parameterNames.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);

                    //a feladat trukkosebb, ha like-ot is hasznalunk a where feltetelben:
                    //pl. ha a j-edik parameter "like-ok koze van szoritva", akkor 
                    //a %-jeleket hozzáadhatjuk a paraméter értékéhez:
                    //sqlCommand.Parameters.AddWithValue(parameterNames[j], "%" + parameterValues[j] + "%");

                }
                value = sqlCommand.ExecuteScalar();
                errMess = "OK";
            }
            catch (SqlException e)
            {
                value = null;
                errMess = e.Message;
            }
            finally
            {
                CloseConnection();
            }
            return value;
        }
    }
}