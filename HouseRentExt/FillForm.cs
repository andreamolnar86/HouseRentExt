using System;
using System.Collections.Generic;
//using System.Data;
using System.Windows.Forms;

namespace HouseRent
{
    public partial class FillForm : Form
    {
        private CountriesDAL countriesDAL;
        private HousesDAL housesDAL;
        private string errMess;
        private int errNumber;

        private int selectedHouseID;

        public FillForm()
        {
            InitializeComponent();
            string error = string.Empty;
            housesDAL = new HousesDAL(ref error);
            if (error != "OK")
            {
                errNumber = 1;
                errMess = "Error"+errNumber+Environment.NewLine+"Houses objektumot nem tudtam letrehozni. " + error;                
            }
            else
            {
                errMess = "OK";
                countriesDAL = new CountriesDAL();
            }
        }

        private bool first;

        /// <summary>
        /// This event occurs before a form is displayed for the first time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillForm_Load(object sender, EventArgs e)
        {
            if (errMess == "OK") //letrejott a Houses objektum
            {
                FillCbCountries();

                FillDgvHouses(-1);

                // Inicializaljuk a ListView-t  
                // Megj: DataGridView eseten konnyebb dolgunk van, 
                // feluletrol mindezeket a beallitasokat konnyen elvegezhetjuk (lasd. FillForm.cs[Design]), 
                // majd a DataSource property-jet a feltoltendo adatokat tartalmazo DataSet-re allitjuk
                InitializeListView();
                first = true;//ez az elso alkalom, hogy feltoltjuk a ListView-t adatokkal
                FillLvHouses(-1);
                first = false;
                // lblHouseNo.Text = m_Houses.GetHouseNumber().ToString();

                //!!!
                //a DataGridView es a ListView kozul csak az egyikre van szuksegunk, igy nincs is szukseg mindket 
                //metodushivasra (FillDgvHouses(), FillLvHouses())
            }

            //we tried how it works:
            // housesDAL.ShowExceptions();
        }

        /// <summary>
        /// Fills the country comboboxes with the country list.
        /// </summary>
        /// <param name="combo">specifies which combobox to fill</param>
        private void FillCbCountries()
        {
            string error = string.Empty;
            List<Country> countryList = countriesDAL.GetCountryList(ref error);

            if (error != "OK")
            {
                errNumber++;
                if (errMess == "OK") 
                    errMess = string.Empty;
                errMess = errMess + Environment.NewLine+ 
                    "Error"+errNumber+Environment.NewLine+"Hiba a ComboBox feltoltesenel." + error;
            }
            else
            {
                //fill the cbCountryFilter combobox
                cbCountryFilter.DataSource = countryList;
                //the text to be dispayed as the combobox text
                cbCountryFilter.DisplayMember = "CountryName";
                //the value of the combobox (can be accessed by the selectedValue property)
                cbCountryFilter.ValueMember = "CountryId";
            }
            
        }

        // Initialize ListView
        private void InitializeListView()
        {
            // Set the view to show details.
            lvHouses.View = View.Details;

            // Allow the user to edit item text.
            lvHouses.LabelEdit = true;

            // Allow the user to rearrange columns.
            lvHouses.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvHouses.FullRowSelect = true;

            // Display grid lines.
            lvHouses.GridLines = true;

            // Sort the items in the list in ascending order.
            lvHouses.Sorting = SortOrder.Ascending;

            // Attach Subitems to the ListView
            lvHouses.Columns.Add("NyaraloID", 0, HorizontalAlignment.Left); //trükk: 0-ra állítjuk a szélességét, így "tüntetve el"
            lvHouses.Columns.Add("NyaraloNev", 100, HorizontalAlignment.Left);
            lvHouses.Columns.Add("Orszagnev", 100, HorizontalAlignment.Left);
            lvHouses.Columns.Add("TulajNev", 100, HorizontalAlignment.Left);
            lvHouses.Columns.Add("Ferohely", 60, HorizontalAlignment.Right);
            lvHouses.Columns.Add("Ar", 50, HorizontalAlignment.Right);
            this.Controls.Add(lvHouses);
        }

        /// <summary>
        /// Fills the lvHouses ListView with data.
        /// adatok = a megadott karakterrel/karakterekkel kezdodo nevu es orszagba tartozo nyaralok
        /// az OrszagID ervenyes kell legyen (countryID>0)
        /// </summary>

        private void FillLvHouses(int countryID)
        {
            if (!first) //ha nem elso alkalommal toltjuk fel a ListView-t
            {
                lvHouses.Clear(); //toroljuk a tartalmat,
                InitializeListView(); //majd ujra inicializaljuk
            }
            string error = string.Empty;
            List<House> houseList = housesDAL.GetHouseListDataReader(countryID, ref error);

            if ((houseList.Count != 0)&&(error=="OK")) //ha van a felteteleknek eleget tevo nyaralo es nem lepett fel hiba,
            //akkor feltoltjuk a ListView-t a megfelelo adatokkal
            {
                foreach (House item in houseList)
                {
                    string[] s = new string[]{item.HouseId.ToString(),
                                              item.HouseName, 
                                              item.HouseCountry.CountryName,
                                              item.HouseOwner.OwnerName,
                                              item.Capacity.ToString(), 
                                              item.Price.ToString()};
                    ListViewItem lvi = new ListViewItem(s);
                    lvHouses.Items.Add(lvi);
                }

                lvHouses.Items[0].Selected = true;
                lvHouses.Select();
            }
            else if (error != "OK")
            {
                errNumber++;
                if (errMess == "OK") errMess = string.Empty;
                errMess = errMess + Environment.NewLine + 
                    "Error"+errNumber+Environment.NewLine+"Hiba a ListView feltoltesenel." + error;
            }
        }


        //dvgHouses DataGridView feltoltese adatokkal
        //adatok = a megadott karakterrel/karakterekkel kezdodo nevu es orszagba tartozo nyaralok
        //az OrszagID ervenyes kell legyen (countryID>0)
        private void FillDgvHouses(int countryID)
        {
            string error = string.Empty;
            dgvHouses.Rows.Clear();
            List<House> houseList = housesDAL.GetHouseListDataSet(countryID, ref error);

            if ((houseList.Count != 0) && (error == "OK")) //ha van a felteteleknek eleget tevo nyaralo es nem lepett fel hiba,
            //a lista elemeit hozzaadjuk a DataGridView-hoz(lesznek olyan oszlopok/cellak, melyek 
            //nem jelennek meg a DataGridView-ban
            {
                foreach (House item in houseList)
                {
                    try
                    {
                        dgvHouses.Rows.Add(item.HouseId,
                                           item.HouseName,
                                           item.HouseCountry.CountryId,
                                           item.HouseCountry.CountryName,
                                           item.HouseOwner.OwnerId,
                                           item.HouseOwner.OwnerName,
                                           item.HouseOwner.OwnerEmail,
                                           item.Capacity,
                                           item.Price);
                    }
                    catch (Exception ex)
                    {
                        errNumber++;
                        if (errMess == "OK") errMess = string.Empty;
                        errMess = errMess + Environment.NewLine + 
                            "Error"+errNumber+Environment.NewLine+"Invalid data " + ex.Message;
                    }
                }
            }
            else if (error != "OK")
            {
                errNumber++;
                if (errMess == "OK") errMess = string.Empty;
                errMess = errMess + Environment.NewLine + 
                    "Error"+errNumber+Environment.NewLine+"Hiba a DataGridView feltoltesenel." + error;
            }
        }

        /// <summary>
        /// Filters the houselist based on the HouseName and the selected countryId 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilter_Click(object sender, EventArgs e)
        {
            //cbCountryFilter.SelectedValue -> countryID
            //cbCountryFilter.SelectedText -> countryName

            //!!!
            //a DataGridView es a ListView kozul csak az egyikre van szuksegunk, igy nincs is szukseg mindket 
            //metodushivasra (FillDgvHouses(), FillLvHouses())
            FillDgvHouses(Convert.ToInt32(cbCountryFilter.SelectedValue));
            FillLvHouses(Convert.ToInt32(cbCountryFilter.SelectedValue));

            if (errMess != "OK")
            {
                ErrorForm errorForm = new ErrorForm(errMess);
                errorForm.Show();
                errorForm.Focus();
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            
            Application.Exit();
        }

        /// <summary>
        /// Occurs whenever the form is first shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillForm_Shown(object sender, EventArgs e)
        {
            if (errMess != "OK")
            {
                ErrorForm errorForm = new ErrorForm(errMess);
                errorForm.Show();
                errorForm.Focus();
            }
        }

        //csak a DataGridView-bol kivalasztott sorra mukodik
        private void btGetCurrentPrice_Click(object sender, EventArgs e)
        {

            //meg kell nezzuk, hogy van-e sor kivalasztva:
            if (dgvHouses.SelectedRows.Count != 0)
            {
                //kiszedjuk a kivalasztott sorban levo nyaralo id-jat (pont emiatt volt erdemes hozzaadni a view-hoz az id-t es a lathatosagat false-ra tenni)
                this.selectedHouseID = Convert.ToInt32(dgvHouses.SelectedRows[0].Cells["houseId"].Value);
               
                //ha listView-val dolgozunk: 
                //[Ugyeljetek, hogy a feltoltesnel a NyaraloID-kat is be kell tolteni a ListView-ba - megfeleloen bovitve a FillLvHouses() metodus]
                Console.WriteLine("Kiválasztott nyaraló ID-ja és neve: " + lvHouses.SelectedItems[0].SubItems[0].Text + " " + lvHouses.SelectedItems[0].SubItems[1].Text);

                //adatbazisban levo ertek --> megszerzesehez szuksegunk van egy select parancsra
                int currentprice = housesDAL.GetHousePrice(this.selectedHouseID, ref errMess);

                //parameterezett query-kkel dolgozo metodushivas:
                //int currentprice = housesDAL.GetHousePriceParametrized(this.selectedHouseID, ref errMess);

                if (errMess == "OK")
                    lbCurrentPrice.Text = currentprice.ToString();
                else
                    Console.WriteLine("Nem sikerült lekérni az értéket.");
            }
            else
            {
                MessageBox.Show("Nincs kiválasztott sor.");
            }

        }
    }
}
