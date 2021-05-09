using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace projekt38
{
    public partial class Form1 : Form
    {
        NpgsqlConnection conn = new NpgsqlConnection();
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        NpgsqlDataAdapter da = new NpgsqlDataAdapter();
        NpgsqlCommand nc = new NpgsqlCommand();
        string sql = "";

        bool wlacznik = true;
        string wybranaZakladka = "INSERT";

        string id, nazwaFirmy, branza, adres1, adres2, telefonSta, telefonKom, fax, email, stronaWWW;
        string imie, nazwisko, wydzial;

        string szukanaFraza;
        string usuwaneID;

        public Form1()
        {
            InitializeComponent();
            textBox20.CharacterCasing = CharacterCasing.Lower;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x00A4)
            {
                if (button1.Text == "DISCONNECT")
                {
                    if (wybranaZakladka == "SHOW")
                    {
                        try
                        {
                            WyswietlDane();
                        }
                        catch (Exception msg)
                        {
                            MessageBox.Show(msg.ToString());
                            throw;
                        }
                    }
                    if (wybranaZakladka == "INSERT")
                    {
                        try
                        {
                            if (SprawdzDane())
                            {
                                UstawId();
                                nazwaFirmy = textBox6.Text;
                                branza = comboBox1.Text;
                                wydzial = textBox7.Text;
                                imie = textBox8.Text;
                                nazwisko = textBox9.Text;
                                adres1 = textBox10.Text + " " + textBox11.Text + "/" + textBox12.Text;
                                adres2 = textBox13.Text + " " + textBox14.Text;
                                telefonSta = textBox15.Text;
                                telefonKom = textBox16.Text;
                                fax = textBox17.Text;
                                email = textBox18.Text;
                                stronaWWW = textBox19.Text;

                                WstawDane();
                            }
                        }
                        catch (Exception msg)
                        {
                            MessageBox.Show(msg.ToString());
                            throw;
                        }
                    }
                    if (wybranaZakladka == "FIND")
                    {
                        try
                        {
                            szukanaFraza = textBox20.Text;
                            SzukajDanych();
                        }
                        catch (Exception msg)
                        {
                            MessageBox.Show(msg.ToString());
                            throw;
                        }
                    }
                    if (wybranaZakladka == "DELETE")
                    {
                        try
                        {
                            if (textBox22.Text.All(Char.IsDigit) && textBox22.Text.Length > 0)
                            {
                                usuwaneID = textBox22.Text;
                                UsunDane();
                            }
                        }
                        catch (Exception msg)
                        {
                            MessageBox.Show(msg.ToString());
                            throw;
                        }
                    }
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        void UstawId()
        {
            try
            {
                sql = "SELECT adresy.id FROM adresy;";
                da = new NpgsqlDataAdapter(sql, conn);
                id = da.Fill(ds).ToString();
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        void WypelnijCombobox()
        {
            try
            {
                sql = "SELECT branze.nazwa FROM branze;";
                nc = new NpgsqlCommand(sql, conn);
                NpgsqlDataReader mr;

                mr = nc.ExecuteReader();
                while (mr.Read())
                {
                    comboBox1.Items.Add(mr["nazwa"].ToString());
                }
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        void UstawElementy()
        {
            if (!wlacznik)
            {
                label1.ForeColor = Color.Red;
                label2.ForeColor = Color.Red;
                label3.ForeColor = Color.Red;
                label4.ForeColor = Color.Red;
                label5.ForeColor = Color.Red;
                tabControl1.Enabled = false;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                wlacznik = true;
                button1.Text = "CONNECT";

                comboBox1.Items.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                textBox7.Clear();
                textBox8.Clear();
                textBox9.Clear();
                textBox10.Clear();
                textBox11.Clear();
                textBox12.Clear();
                textBox13.Clear();
                textBox14.Clear();
                textBox15.Clear();
                textBox16.Clear();
                textBox17.Clear();
                textBox18.Clear();
                textBox19.Clear();

                textBox6.BackColor = Color.White;
                textBox7.BackColor = Color.White;
                textBox8.BackColor = Color.White;
                textBox9.BackColor = Color.White;
                textBox10.BackColor = Color.White;
                textBox11.BackColor = Color.White;
                textBox12.BackColor = Color.White;
                textBox13.BackColor = Color.White;
                textBox14.BackColor = Color.White;
                textBox15.BackColor = Color.White;
                textBox16.BackColor = Color.White;
                textBox17.BackColor = Color.White;
                textBox18.BackColor = Color.White;
                textBox19.BackColor = Color.White;
            }
            else
            {
                label1.ForeColor = Color.Green;
                label2.ForeColor = Color.Green;
                label3.ForeColor = Color.Green;
                label4.ForeColor = Color.Green;
                label5.ForeColor = Color.Green;
                tabControl1.Enabled = true;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                wlacznik = false;
                button1.Text = "DISCONNECT";

                WypelnijCombobox();
                MessageBox.Show("Ustanowiono połączenie z bazą danych.");
                WyswietlDane();
            }
        }

        void WyswietlDane()
        {
            try
            {
                sql = "SELECT adresy.id, adresy.nazwa_firmy, adresy.branza, uzytkownicy.wydzial, uzytkownicy.imie_pracownika, uzytkownicy.nazwisko_pracownika, adresy.adres1, adresy.adres2, adresy.telefon_sta, adresy.telefon_kom, adresy.fax, adresy.email, adresy.strona_www FROM adresy INNER JOIN uzytkownicy ON adresy.id=uzytkownicy.id;";
                da = new NpgsqlDataAdapter(sql, conn);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridView1.DataSource = dt;
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        void SzukajDanych()
        {
            try
            {
                sql = string.Format("SELECT adresy.id, adresy.nazwa_firmy, adresy.branza, uzytkownicy.wydzial, uzytkownicy.imie_pracownika, uzytkownicy.nazwisko_pracownika, adresy.adres1, adresy.adres2, adresy.telefon_sta, adresy.telefon_kom, adresy.fax, adresy.email, adresy.strona_www FROM adresy INNER JOIN uzytkownicy ON adresy.id=uzytkownicy.id WHERE CAST(adresy.id as VARCHAR(10)) LIKE '%{0}%' OR LOWER(adresy.nazwa_firmy) LIKE '%{0}%' OR LOWER(adresy.branza) LIKE '%{0}%' OR LOWER(uzytkownicy.wydzial) LIKE '%{0}%' OR LOWER(uzytkownicy.imie_pracownika) LIKE '%{0}%' OR LOWER(uzytkownicy.nazwisko_pracownika) LIKE '%{0}%' OR LOWER(adresy.adres1) LIKE '%{0}%' OR LOWER(adresy.adres2) LIKE '%{0}%' OR CAST(adresy.telefon_sta as VARCHAR(10)) LIKE '%{0}%' OR CAST(adresy.telefon_kom as VARCHAR(10)) LIKE '%{0}%' OR CAST(adresy.fax as VARCHAR(10)) LIKE '%{0}%' OR LOWER(adresy.email) LIKE '%{0}%' OR LOWER(adresy.strona_www) LIKE '%{0}%';", szukanaFraza);
                da = new NpgsqlDataAdapter(sql, conn);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridView1.DataSource = dt;
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        void WstawDane()
        {
            try
            {
                sql = string.Format("INSERT INTO adresy (id, nazwa_firmy, branza, adres1, adres2, telefon_sta, telefon_kom, fax, email, strona_www) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');", id, nazwaFirmy, branza, adres1, adres2, telefonSta, telefonKom, fax, email, stronaWWW);
                da = new NpgsqlDataAdapter(sql, conn);
                da.Fill(ds);
                sql = string.Format("INSERT INTO uzytkownicy (id, imie_pracownika, nazwisko_pracownika, wydzial) VALUES ('{0}', '{1}', '{2}', '{3}')", id, imie, nazwisko, wydzial);
                da = new NpgsqlDataAdapter(sql, conn);
                da.Fill(ds);
                WyswietlDane();
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        void UsunDane()
        {
            try
            {
                sql = string.Format("DELETE FROM adresy WHERE adresy.id={0};", usuwaneID);
                da = new NpgsqlDataAdapter(sql, conn);
                da.Fill(ds);
                sql = string.Format("DELETE FROM uzytkownicy WHERE uzytkownicy.id={0};", usuwaneID);
                da = new NpgsqlDataAdapter(sql, conn);
                da.Fill(ds);
                WyswietlDane();
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        bool SprawdzDane()
        {
            bool stan = true;

            if (textBox6.Text.Length > 2)
            {
                textBox6.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox6.BackColor = Color.Red;
                stan = false;
            }
            if (textBox7.Text.Length > 2)
            {
                textBox7.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox7.BackColor = Color.Red;
                stan = false;
            }
            if (textBox8.Text.All(Char.IsLetter) && textBox8.Text.Length > 2)
            {
                textBox8.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox8.BackColor = Color.Red;
                stan = false;
            }
            if (textBox9.Text.All(Char.IsLetter) && textBox9.Text.Length > 2)
            {
                textBox9.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox9.BackColor = Color.Red;
                stan = false;
            }
            if (textBox10.Text.All(Char.IsLetter) && textBox10.Text.Length > 2)
            {
                textBox10.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox10.BackColor = Color.Red;
                stan = false;
            }
            if (textBox11.Text.All(Char.IsDigit) && textBox11.Text.Length > 0)
            {
                textBox11.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox11.BackColor = Color.Red;
                stan = false;
            }
            if (textBox12.Text.All(Char.IsDigit) && textBox12.Text.Length > 0)
            {
                textBox12.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox12.BackColor = Color.Red;
                stan = false;
            }
            if (textBox13.Text.All(Char.IsDigit) && textBox13.Text.Length.Equals(5))
            {
                textBox13.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox13.BackColor = Color.Red;
                stan = false;
            }
            if (textBox14.Text.All(Char.IsLetter) && textBox14.Text.Length > 2)
            {
                textBox14.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox14.BackColor = Color.Red;
                stan = false;
            }
            if (textBox15.Text.All(Char.IsDigit) && textBox15.Text.Length.Equals(9))
            {
                textBox15.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox15.BackColor = Color.Red;
                stan = false;
            }
            if (textBox16.Text.All(Char.IsDigit) && textBox16.Text.Length.Equals(9))
            {
                textBox16.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox16.BackColor = Color.Red;
                stan = false;
            }
            if (textBox17.Text.All(Char.IsDigit) && textBox17.Text.Length.Equals(9))
            {
                textBox17.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox17.BackColor = Color.Red;
                stan = false;
            }
            if (textBox18.Text.Length > 6)
            {
                textBox18.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox18.BackColor = Color.Red;
                stan = false;
            }
            if (textBox19.Text.Length > 4)
            {
                textBox19.BackColor = Color.LimeGreen;
            }
            else
            {
                textBox19.BackColor = Color.Red;
                stan = false;
            }
            return stan;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                wybranaZakladka = "SHOW";
            }
            if (tabControl1.SelectedTab == tabPage2)
            {
                wybranaZakladka = "INSERT";
            }
            if (tabControl1.SelectedTab == tabPage3)
            {
                wybranaZakladka = "FIND";
            }
            if (tabControl1.SelectedTab == tabPage4)
            {
                wybranaZakladka = "DELETE";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (wlacznik)
                {
                    string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);
                    conn = new NpgsqlConnection(connstring);
                    conn.Open();
                    UstawElementy();
                }
                else
                {
                    conn.Close();
                    UstawElementy();
                }
            }
            catch
            {

            }
        }
    }
}
