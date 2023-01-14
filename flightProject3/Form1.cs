using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace flightProject3
{
    public partial class Form1 : Form
    {
        module3Entities ent = new module3Entities();
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = 0;
            radioButton1.Checked = true;

            groupBox3.Visible = true;

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'module3DataSet1.Airports' table. You can move, or remove it, as needed.
            this.airportsTableAdapter1.Fill(this.module3DataSet1.Airports);
            // TODO: This line of code loads data into the 'module3DataSet.Airports' table. You can move, or remove it, as needed.
            this.airportsTableAdapter.Fill(this.module3DataSet.Airports);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();

        }



        public bool checkRoute(List<Route> r, int route)
        {
            foreach (Route i in r)
            {
                if (i.Airport.ID == route)
                    return false;
            }
            return true;
        }

        public List<List<Schedule>> checkSchedule(List<Route> r,DateTime date)
        {
            if (r.Count == 0)
                return null;
            List<List<Schedule>> list = new List<List<Schedule>>();
            int total = 1;
            List<int> subTotal =new List<int>();

            for (int i = 0; i < r.Count; i++)
            {
                List<Schedule> ScheduleList = r[i].Schedules.Where(x=>x.Date==date).ToList();
                total *= ScheduleList.Count;
                list.Add(ScheduleList);
            }
            int tempTotal = total;
         
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Count == 0)
                    return null;
                tempTotal /= list[i].Count;
                subTotal.Add(tempTotal);
            }

            Console.WriteLine("List pattern "+list.Count);
            foreach (var listc in list)
            {
                foreach (var schedule in listc)
                    Console.Write(schedule.ID + " ");
                Console.WriteLine();
            }
                

            List<List<Schedule>> finallist = new List<List<Schedule>>();
            int count = 0;
            Console.WriteLine("total "+total);
            while (count < total)
            {
              
                for (int j = 0; j < list.Count; j++)
                {
                    
                    List<Schedule> templist = new List<Schedule>();
                    for (int k = 0; k < list.Count; k++)
                    {
                        int loc = (count / subTotal[k]) % list[j].Count;
                        templist.Add(list[k][loc]);
                        
                    }
                    count++;
                    bool add = true;
                    if(finallist.Count>0)
                        if (templist.SequenceEqual(finallist[finallist.Count-1]))
                            add = false;
                    for (int i = 0; i < templist.Count-1 &&add==true; i++)
                    {
                        if (templist[i].Date != templist[i + 1].Date)
                            add = false;
                        else if ((templist[i].Time + TimeSpan.Parse(templist[i].Route.FlightTime.ToString())) < templist[i + 1].Time)
                            add = false;
                       
                    }
                    if(add==true)
                        finallist.Add(templist);        
                }
            }
            count = 0;
            foreach(var listc in finallist)
            {
                Console.Write(count++ +" ");
                foreach(var schedule in listc)
                {
                    Console.Write(schedule.ID + " ");
                }
                Console.WriteLine();
                }
            return finallist;


        }





        public List<List<Route>> findRoute(int from, int to )
        {
            var list = new List<List<Route>>();
            var finalList = new List<List<Route>>();
            var record = new List<List<int>>();
            if (!ent.Routes.Where(x => x.Airport.ID == from ).Any())
            {
                return null;
            }
            var firstRoute = ent.Routes.Where(x => x.Airport.ID == from).ToList();
            foreach (var route in firstRoute)
            {
                var routeList = new List<Route>();
                routeList.Add(route);
                list.Add(routeList);

            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i][list[i].Count - 1].Airport1.ID == to)
                {
                    finalList.Add(list[i]);
                    continue;
                }

                int airportBef = (int)list[i][list[i].Count - 1].Airport1.ID;
                var subRoute = ent.Routes.Where(x => x.Airport.ID == airportBef).ToList();
                var temp = list[i];
                int token = 0;
                foreach (var route in subRoute)
                {
                    Console.WriteLine(i);
                    if (checkRoute(list[i], route.Airport1.ID))
                        if (token++ == 0)
                        {
                            list[i].Add(route);
                        }
                        else
                        {
                            var temp2 = temp;
                            temp2.Add(route);
                            list.Add(temp2);
                        }
                }
                if (token > 0)
                    i--;
            }
            Console.WriteLine("start");
            foreach (var routeList in finalList)
            {
                foreach (var route in routeList)
                {
                    Console.Write(route.ID + " ");
                }
                Console.WriteLine();
            }
            return finalList;
        }

        public void loadData(DateTime d1, DateTime? d2)
        {
            if(d1!=null)
            {
                dataGridView1.Rows.Clear();
                List<List<Route>> routeList = findRoute((int)comboBox1.SelectedValue, (int)comboBox2.SelectedValue);
                List<List<Schedule>> finalSchedule = new List<List<Schedule>>();
                foreach (var route in routeList)
                {
                    List<List<Schedule>> temp = checkSchedule(route, d1);
                    if (temp != null)
                        finalSchedule.AddRange(temp);
                }


                for (int i = 0; i < finalSchedule.Count; i++)
                {
                    if (finalSchedule[i].Count == 1)
                    {
                        dataGridView1.Rows.Add(
                            (i + 1),
                            finalSchedule[i][0].Route.Airport.IATACode,
                            finalSchedule[i][0].Route.Airport1.IATACode,
                            finalSchedule[i][0].Date.Date,
                            finalSchedule[i][0].Time,
                            finalSchedule[i][0].FlightNumber,
                            finalSchedule[i][0].EconomyPrice,
                            finalSchedule[i].Count - 1);
                    }
                    else
                    {
                        string id = "";
                        string flightNumber = "";
                        decimal price = 0;

                        foreach (var schedule in finalSchedule[i])
                        {
                            if (id.Equals(""))
                            {
                                id = schedule.ID.ToString();
                                flightNumber = schedule.FlightNumber;
                                price = schedule.EconomyPrice;
                            }
                            else
                            {
                                id += "." + schedule.ID.ToString();
                                flightNumber += " - " + schedule.FlightNumber;
                                price += schedule.EconomyPrice;
                            }
                        }
                        dataGridView1.Rows.Add(
                           id,
                           finalSchedule[i][0].Route.Airport.IATACode,
                           finalSchedule[i][finalSchedule[i].Count - 1].Route.Airport1.IATACode,
                           finalSchedule[i][0].Date.Date,
                           finalSchedule[i][0].Time,
                           flightNumber,
                           price,
                           finalSchedule[i].Count - 1);
                    }
                }
            }

            if(d2!=null)
            {
                dataGridView2.Rows.Clear();
                List<List<Route>> routeList = findRoute((int)comboBox2.SelectedValue, (int)comboBox1.SelectedValue);
                List<List<Schedule>> finalSchedule = new List<List<Schedule>>();
                foreach (var route in routeList)
                {
                    List<List<Schedule>> temp = checkSchedule(route, (DateTime)d2);
                    if (temp != null)
                        finalSchedule.AddRange(temp);
                }


                for (int i = 0; i < finalSchedule.Count; i++)
                {
                    if (finalSchedule[i].Count == 1)
                    {
                        dataGridView2.Rows.Add(
                            (i + 1),
                            finalSchedule[i][0].Route.Airport.IATACode,
                            finalSchedule[i][0].Route.Airport1.IATACode,
                            finalSchedule[i][0].Date.Date,
                            finalSchedule[i][0].Time,
                            finalSchedule[i][0].FlightNumber,
                            finalSchedule[i][0].EconomyPrice,
                            finalSchedule[i].Count - 1);
                    }
                    else
                    {
                        string id = "";
                        string flightNumber = "";
                        decimal price = 0;

                        foreach (var schedule in finalSchedule[i])
                        {
                            if (id.Equals(""))
                            {
                                id = schedule.ID.ToString();
                                flightNumber = schedule.FlightNumber;
                                price = schedule.EconomyPrice;
                            }
                            else
                            {
                                id += "." + schedule.ID.ToString();
                                flightNumber += " - " + schedule.FlightNumber;
                                price += schedule.EconomyPrice;
                            }
                        }
                        dataGridView2.Rows.Add(
                           id,
                           finalSchedule[i][0].Route.Airport.IATACode,
                           finalSchedule[i][finalSchedule[i].Count - 1].Route.Airport1.IATACode,
                           finalSchedule[i][0].Date.Date,
                           finalSchedule[i][0].Time,
                           flightNumber,
                           price,
                           finalSchedule[i].Count - 1);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox2.SelectedIndex)
            {
                MessageBox.Show("Outbound airport and Return airport cannot be same!");
                return;
            }

            if (radioButton2.Checked)
            {
                DateTime date1 = dateTimePicker1.Value.Date;
                loadData(date1, null);
            }
            else
            {
                DateTime date1 = dateTimePicker1.Value.Date;
                DateTime date2 = dateTimePicker2.Value.Date;
                loadData(date1, date2);
            }


        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Visible = radioButton1.Checked;
            label5.Visible = radioButton1.Checked;
            groupBox3.Visible = radioButton1.Checked;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label5.Visible = radioButton1.Checked;
            dateTimePicker2.Visible = radioButton1.Checked;
            groupBox3.Visible = radioButton1.Checked;
        }
    }

}

