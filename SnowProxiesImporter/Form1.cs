using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SnowProxiesImporter
{
    public partial class Form1 : Form
    {

        private const string snowBotDataPath = "\\SwiftBot\\Snowbot.exe_Url_3xl2s2jpuburhzypal2aujljehcxxm54\\1.0.0.0\\user.config";
        private const string snowBotTouchDataPath = "\\Snowbot_company\\SnowbotTouch.exe_Url_dcl3c1umqia1hgfui4wmtc5w2yyzfwsl\\1.0.0.0\\user.config";
        private const string snowBotRetroDataPath = "\\SwiftBot\\Snowbot.exe_Url_gt4m3hjyqihhzfktxxcn1dxqdso4b50c\\1.0.0.0\\user.config";

        private string localAppDataPath = Environment.GetEnvironmentVariable("LocalAppData");

        private string pathToImport = String.Empty;

        private int detectedConfigurations = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (File.Exists(localAppDataPath + snowBotDataPath))
            {
                lbl_statePC.Text = "Configuration détectée";
                lbl_statePC.ForeColor = Color.Green;
                detectedConfigurations++;
            } else {
                lbl_statePC.Text = "Configuration non disponible";
                lbl_statePC.ForeColor = Color.Red;
            }

            if (File.Exists(localAppDataPath + snowBotTouchDataPath))
            {
                lbl_stateTouch.Text = "Configuration détectée";
                lbl_stateTouch.ForeColor = Color.Green;
                detectedConfigurations++;
            }
            else
            {
                lbl_stateTouch.Text = "Configuration non disponible";
                lbl_stateTouch.ForeColor = Color.Red;
            }

            if (File.Exists(localAppDataPath + snowBotRetroDataPath))
            {
                lbl_stateRetro.Text = "Configuration détectée";
                lbl_stateRetro.ForeColor = Color.Green;
                detectedConfigurations++;
            }
            else
            {
                lbl_stateRetro.Text = "Configuration non disponible";
                lbl_stateRetro.ForeColor = Color.Red;
            }

            if (detectedConfigurations == 0)
            {
                MessageBox.Show("Aucune configuration de SnowBot n'a été trouvée sur votre système." + Environment.NewLine + "Merci de lancer SnowBot au moins une fois et ajouter un proxy (valide ou pas) manuellement.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (proxiesListDialog.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("Veuillez sélectionner votre liste de proxies !!");
                return;
            } else
            {
                btn_selectProxies.Text = proxiesListDialog.FileName;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                pathToImport = localAppDataPath + snowBotDataPath;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                pathToImport = localAppDataPath + snowBotTouchDataPath;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                pathToImport = localAppDataPath + snowBotRetroDataPath;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (btn_selectProxies.Text == "Parcourir ...")
            {
                MessageBox.Show("Merci de sélectionner un fichier contenant votre liste de proxies !", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            XmlDocument snowbotConfig = new XmlDocument();
            snowbotConfig.Load(pathToImport);
            XmlNode root = snowbotConfig.DocumentElement;
            XmlNode proxiesNode;

            if (radioButton2.Checked)
            {
                proxiesNode = root.SelectSingleNode("userSettings/SwiftBotTouch.My.MySettings/setting[@name='proxies']/value/ArrayOfString");
            } else
            {
                proxiesNode = root.SelectSingleNode("userSettings/SwiftBot.My.MySettings/setting[@name='proxies']/value/ArrayOfString");
            }

            var proxies = File.ReadLines(btn_selectProxies.Text);
            foreach (string proxyElement in proxies)
            {
                string[] proxyData = proxyElement.Split(':');

                if (proxyData[0] == null | proxyData[1] == null | proxyData[2] == null | proxyData[3] == null | proxyData[4] == null)
                {
                    MessageBox.Show("Erreur sur le proxy, merci de vérifier votre fichier");
                    return;
                }
            }

            foreach (string proxyElement in proxies)
            {
                string[] proxyData = proxyElement.Split(':');
                XmlElement elem = snowbotConfig.CreateElement("string");
                elem.InnerText = proxyData[0] + ":" + proxyData[1] + ":" + proxyData[2] + ":" + proxyData[3] + ":" + proxyData[4];
                proxiesNode.AppendChild(elem);
            }

            snowbotConfig.Save(pathToImport);

            MessageBox.Show("Importation des proxy : OK");

        }
    }
}
