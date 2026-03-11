using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PharmaSI
{
    public partial class ConsultationRapport : Form
    {
        ConnexionSql maConnexion;

        public ConsultationRapport()
        {
            InitializeComponent();
            maConnexion = ConnexionSql.getInstance("localhost", "ppe", "root", "");
        }

        private void ConsultationRapport_Load(object sender, EventArgs e)
        {
            ChargerRapports();
        }

        // Charger les rapports dans la liste déroulante
        private void ChargerRapports()
        {
            comboBoxRapport.Items.Clear();

            try
            {
                maConnexion.OpenConnexion();

                string req = "SELECT idRapport FROM rapport_visite";

                MySqlCommand cmd = maConnexion.reqExec(req);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBoxRapport.Items.Add(reader["idRapport"].ToString());
                }

                reader.Close();
                maConnexion.CloseConnexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            if (comboBoxRapport.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un rapport.");
                return;
            }

            listBoxInfos.Items.Clear();
            listBoxProduits.Items.Clear();

            int idRapport = Convert.ToInt32(comboBoxRapport.SelectedItem);

            try
            {
                maConnexion.OpenConnexion();

                // Infos rapport
                string req = @"SELECT rv.dateRapport, rv.bilan, p.nom, p.prenom, m.libelle
                               FROM rapport_visite rv
                               JOIN praticien p ON rv.idPraticien = p.idPraticien
                               JOIN motif m ON rv.idMotif = m.idMotif
                               WHERE rv.idRapport = @idRapport";

                MySqlCommand cmd = maConnexion.reqExec(req);
                cmd.Parameters.AddWithValue("@idRapport", idRapport);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    listBoxInfos.Items.Add("Date : " + reader["dateRapport"]);
                    listBoxInfos.Items.Add("Praticien : " + reader["prenom"] + " " + reader["nom"]);
                    listBoxInfos.Items.Add("Motif : " + reader["libelle"]);
                    listBoxInfos.Items.Add("Bilan : " + reader["bilan"]);
                }

                reader.Close();

                // Produits offerts
                string reqProduits = @"SELECT pr.Nom, o.qteOfferte
                                       FROM offrir o
                                       JOIN produit pr ON o.numProduit = pr.Num
                                       WHERE o.idRapport = @idRapport";

                MySqlCommand cmdProduits = maConnexion.reqExec(reqProduits);
                cmdProduits.Parameters.AddWithValue("@idRapport", idRapport);

                MySqlDataReader readerProduits = cmdProduits.ExecuteReader();

                while (readerProduits.Read())
                {
                    listBoxProduits.Items.Add(
                        readerProduits["Nom"] + " - Echantillons : " +
                        readerProduits["qteOfferte"]);
                }

                readerProduits.Close();

                if (listBoxProduits.Items.Count == 0)
                {
                    listBoxProduits.Items.Add("Aucun produit offert");
                }

                maConnexion.CloseConnexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}