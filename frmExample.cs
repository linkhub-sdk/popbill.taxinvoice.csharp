using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Linkhub;

namespace Popbill.Taxinvoice.Example
{
    public partial class frmExample : Form
    {

        private TaxinvoiceService taxinvoiceService;

        private String PartnerID = "INNOPOST";
        private String SecretKey = "VGBaxxHL7T4o4LrwDRcALHo0j8LgAxsLGhKqjuCwlX8=";


        public frmExample()
        {
            InitializeComponent();

            taxinvoiceService = new TaxinvoiceService(PartnerID, SecretKey);
            //It's for TEST.
            taxinvoiceService.IsTest = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            URLResponse response;

            try 
            {
                response = taxinvoiceService.GetPopbillURL("4108600477","innopost","SBOX");
                MessageBox.Show(response.url);

            }
            catch (LinkhubException le)
            {
                MessageBox.Show(le.Code + " | " + le.Message);
            }


        }
    }
}
