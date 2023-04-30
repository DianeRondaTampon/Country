using Newtonsoft.Json;
using Svg;
using System.Net;
using System.Runtime.InteropServices;

namespace WinFormsApp10
{
    public partial class Form1 : Form
    {

        private const string API_BASE_URL = "https://restcountries.com/v2/name/";


        public class CountryData
        {
            public string Name { get; set; }
            public string Capital { get; set; }
            public long Population { get; set; }
            public Currency[] Currencies { get; set; }
            public string Region { get; set; }
            public string Flag { get; set; }
            public Language[] Languages{ get; set; }
        }

        public class Currency
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Symbol { get; set; }
        }

        public class Language
        {
            public string iso639_1 { get; set; }
            public string iso639_2 { get; set; }
            public string name { get; set; }
            public string nativeName { get; set; }
        }

        private CountryData getCountryData(string countryName)
        {
            string apiUrl = $"{API_BASE_URL}{countryName}";
            string responseJson = makeApiRequest(apiUrl);
            if (responseJson == null)
            {
                lblName.Text = "Country not found.";
                return null;
            }
            else
            {
                CountryData[] countryDataArray = JsonConvert.DeserializeObject<CountryData[]>(responseJson);

                if (countryDataArray.Length == 0)
                {
                    lblName.Text = "Country not found.";
                    return null;
                }

                return countryDataArray[0];
            }  
        }

        private string getName(CountryData countryData)
        {
            return countryData.Name;
        }

        private string getCapitalCity(CountryData countryData)
        {
            return countryData.Capital;
        }

        private string getPopulation(CountryData countryData)
        {
            return $"{countryData.Population:N0}";
        }

        private string getCurrency(CountryData countryData)
        {
            Currency currency = countryData.Currencies[0];

            return $"{currency.Name} ({currency.Code})";
        }

        private string getRegion(CountryData countryData)
        {
            return countryData.Region;
        }

        private string getFlag(CountryData countryData)
        {
            return countryData.Flag;
        }

        private string getLanguage(CountryData countryData)
        {
            Language language = countryData.Languages[0];

            return language.name;
        }

        private void getImageFlag(string flag)
        {
            // download the SVG image from the URL
            WebClient webClient = new WebClient();
            byte[] svgBytes = webClient.DownloadData(flag);


            string svgContent;
            using (var streamReader = new StreamReader(new MemoryStream(svgBytes)))
            {
                svgContent = streamReader.ReadToEnd();
            }

            // load the SVG image from the string into an SvgDocument object
            SvgDocument svgDocument = SvgDocument.FromSvg<SvgDocument>(svgContent);


            // render the SvgDocument object into a Bitmap object
            Bitmap bitmap = new Bitmap(800, 600); // set the desired size of the bitmap
            svgDocument.Draw(bitmap);

            // assign the Bitmap object to the PictureBox control's Image property
            pictureBox1.Image = bitmap;
        }

        private string makeApiRequest(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    return null;
                }

            }

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetInformation_Click(object sender, EventArgs e)
        {
            string nameOfTheFCountry = txtNameOfTheCountry.Text;
            CountryData countryData = getCountryData(nameOfTheFCountry);

            if (countryData != null)
            {
                //response of API
                string nameCountry = getName(countryData);
                lblName.Text = nameCountry;
                string capital = getCapitalCity(countryData);
                lblCapitalCity.Text = capital;
                string population = getPopulation(countryData);
                lblPopulation.Text = population;
                string currency = getCurrency(countryData);
                lblCurreny.Text = currency;
                string region = getRegion(countryData);
                lblRegion.Text = region;
                string flag = getFlag(countryData);
                lblFlag.Text = flag;
                getImageFlag(flag);
                string language = getLanguage(countryData);
                lblLanguage.Text = language;
            }
            else
            {
                //empty form labels bcuz no country found
                lblCapitalCity.Text = "";
                lblPopulation.Text = "";
                lblCurreny.Text = "";
                lblRegion.Text = "";
                lblFlag.Text = "";
                lblLanguage.Text = "";
                pictureBox1.Image = null;
            }
        }

        private void txtNameOfTheCountry_TextChanged(object sender, EventArgs e)
        {

        }
    }
}