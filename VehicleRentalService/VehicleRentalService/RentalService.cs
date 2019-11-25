using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VehicleRentalService
{
    public interface Vehicle : IComparable<Vehicle>
    {
        public string Name { get; set; }
        public int cost { get; set; }
        public string Branch { get; set; }
        public List<TimeSlot> BookedSlots { get; }
    }

    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeSlot(DateTime st, DateTime et)
        {
            this.StartTime = st;
            this.EndTime = et;
        }

        public bool IsTimeSlotOverlap(TimeSlot t)
        {
            if (this.StartTime < t.EndTime && this.EndTime > t.StartTime)
                return true;
            else
                return false;
        }
    }

    public class Suv : Vehicle
    {
        public string Name
        {
            get
            {
                return "suv";
            }
            set { }
        }

        public Suv(string name)
        {
            this.Name = name;
        }

        private List<TimeSlot> _bookedSlots = null;

        public int cost { get; set; } = 0;

        public List<TimeSlot> BookedSlots
        {
            get
            {
                if (_bookedSlots == null)
                {
                    _bookedSlots = new List<TimeSlot>();
                }
                return _bookedSlots;
            }
        }

        public string Branch { get; set; } = "";

        public int CompareTo(Vehicle other)
        {
            return this.cost.CompareTo(other.cost);
        }
    }

    public class Sedan : Vehicle
    {
        public string Name
        {
            get
            {
                return "sedan";
            }
            set { }
        }

        public Sedan(string name)
        {
            this.Name = name;
        }

        private List<TimeSlot> _bookedSlots = null;

        public int cost { get; set; } = 0;
        public List<TimeSlot> BookedSlots
        {
            get
            {
                if (_bookedSlots == null)
                {
                    _bookedSlots = new List<TimeSlot>();
                }
                return _bookedSlots;
            }
        }

        public string Branch { get; set; } = "";

        public int CompareTo(Vehicle other)
        {
            return this.cost.CompareTo(other.cost);
        }
    }

    public class Branch
    {
        public string Name
        {
            get; set;
        }
        public Dictionary<string, List<Vehicle>> vehicles = new Dictionary<string, List<Vehicle>>();
    }

    public class City
    {
        public string name;
        public Dictionary<string, Branch> branchesInCity;
        public City(string name)
        {
            this.name = name;
            branchesInCity = new Dictionary<string, Branch>();
        }
    }

    class RentalService
    {
        public static Dictionary<string, Type> vehicleDic = new Dictionary<string, Type>();
        public static void Main(string[] args)
        {
            List<string> testCases = new List<string>();
            AddTestCases(testCases);
            City c = new City("Bangalore");
            PopulateAvailableVehicles(vehicleDic);
            foreach (string inp in testCases)
            {
                if (inp.ToLower().StartsWith("add_branch"))
                {
                    AddBranchToCity(c, inp);
                }
                else if (inp.ToLower().StartsWith("add_vehicle"))
                {
                    string branch = inp.Substring(inp.IndexOf('(') + 2, inp.IndexOf(',') - "add_vehicle".Length - 3);
                    AddVehicleToBranch(c.branchesInCity[branch], inp);
                }
                else if (inp.ToLower().StartsWith("rent_vehicle"))
                {
                    RentVehicle(c, inp);
                }
                else if (inp.ToLower().StartsWith("get_available_vehicles"))
                {
                    string branch = inp.Substring(inp.IndexOf('\'') + 1, inp.LastIndexOf('\'') - inp.IndexOf('\'') - 1);
                    GetAvailableVehiclesInBranch(c.branchesInCity[branch]);
                }
                else if (inp.ToLower().StartsWith("print_system_view"))
                {
                    string st = inp.Substring(inp.IndexOf('(') + 1, inp.IndexOf(',') - inp.IndexOf('(') - 1);
                    string et = inp.Substring(inp.IndexOf(',') + 2, inp.IndexOf(')') - inp.IndexOf(',') - 2);
                    PrintSystemView(c, DateTime.ParseExact(st, "dd-MM-yyyy hh:mm:ss tt", null), DateTime.ParseExact(et, "dd-MM-yyyy hh:mm:ss tt", null));
                }
                else
                {
                    Console.WriteLine("Incorrect command");
                }
            }

        }

        private static void AddTestCases(List<string> testCases)
        {
            testCases.Add("add_branch('gachibowli', ['1 suv for Rs.12 per hour', '3 sedan for Rs.10 per hour'])");
            testCases.Add("add_branch('miyapur', ['3 sedan for Rs.9 per hour'])");
            testCases.Add("add_vehicle('gachibowli', '1 sedan')");
            testCases.Add("print_system_view(20-02-2019 10:00:00 PM, 21-02-2019 01:00:00 AM)");
            testCases.Add("rent_vehicle('suv', 20-02-2019 10:00:00 PM, 21-02-2019 01:00:00 AM)");
            testCases.Add("rent_vehicle('sedan', 20-02-2019 10:00:00 PM, 21-02-2019 01:00:00 AM)");
            testCases.Add("rent_vehicle('suv', 20-02-2019 11:00:00 PM, 21-02-2019 01:00:00 AM)");
            testCases.Add("rent_vehicle('suv', 21-02-2019 01:00:00 AM, 21-02-2019 03:00:00 AM)");
            testCases.Add("get_available_vehicles('gachibowli')");
            testCases.Add("print_system_view(20-02-2019 10:00:00 PM, 21-02-2019 01:00:00 AM)");
        }

        private static void PrintSystemView(City c, DateTime startTime, DateTime endTime)
        {
            foreach (string bn in c.branchesInCity.Keys)
            {
                Console.WriteLine(bn);
                foreach (string vt in c.branchesInCity[bn].vehicles.Keys)
                {
                    int countAvailable = 0;
                    Vehicle testVeh = getInstanceOfVehicle(vt);
                    testVeh.BookedSlots.Add(new TimeSlot(startTime, endTime));
                    foreach (Vehicle v in c.branchesInCity[bn].vehicles[vt])
                    {
                        bool available = true;
                        foreach (TimeSlot bookedSlot in v.BookedSlots)
                        {
                            if (bookedSlot.IsTimeSlotOverlap(testVeh.BookedSlots[0]))
                            {
                                available = false;
                            }
                        }
                        if (available)
                        {
                            countAvailable++;
                        }
                    }
                    if (countAvailable > 0)
                    {
                        Console.WriteLine(
                            string.Format("\t{0} {1} available for Rs.{2}", countAvailable, c.branchesInCity[bn].vehicles[vt][0].Name, c.branchesInCity[bn].vehicles[vt][0].cost));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("\tAll {0} are booked", c.branchesInCity[bn].vehicles[vt][0].Name));
                    }
                }
            }
        }

        private static void GetAvailableVehiclesInBranch(Branch b)
        {
            foreach (string vt in b.vehicles.Keys)
            {
                Console.WriteLine(b.vehicles[vt].Count + " " + b.vehicles[vt][0].Name + " available for Rs." + b.vehicles[vt][0].cost);
            }
        }

        private static void RentVehicle(City c, string inp)
        {
            string vehType = inp.Substring(inp.IndexOf('(') + 2, inp.IndexOf(',') - inp.IndexOf('(') - 3);
            string st = inp.Split(',')[1].Trim(), et = inp.Split(',')[2].Trim().Substring(0, inp.Split(',')[2].Trim().Length - 1);
            DateTime startTime = DateTime.ParseExact(st, "dd-MM-yyyy hh:mm:ss tt", null);
            DateTime endTime = DateTime.ParseExact(et, "dd-MM-yyyy hh:mm:ss tt", null);
            List<Vehicle> availableVehicles = GetAvailableVehiclesInGivenTimeSlot(c, vehType, startTime, endTime);
            availableVehicles.Sort();
            if (availableVehicles.Count > 0)
            {
                availableVehicles[0].BookedSlots.Add(new TimeSlot(startTime, endTime));
                Console.WriteLine("Vehicle booked from " + availableVehicles[0].Branch);
            }
            else
                Console.WriteLine("All " + vehType + "'s booked for timeslot " + startTime + " " + endTime);
        }

        private static List<Vehicle> GetAvailableVehiclesInGivenTimeSlot(City c, string vehType, DateTime startTime, DateTime endTime)
        {
            List<Vehicle> availableVeh = new List<Vehicle>();
            Vehicle reqVeh = getInstanceOfVehicle(vehType);
            reqVeh.BookedSlots.Add(new TimeSlot(startTime, endTime));
            foreach (string b in c.branchesInCity.Keys)
            {
                reqVeh.Branch = b;
                if (c.branchesInCity[b].vehicles.ContainsKey(vehType))
                {
                    foreach (Vehicle v in c.branchesInCity[b].vehicles[vehType])
                    {
                        bool available = true;
                        foreach (TimeSlot bookedSlot in v.BookedSlots)
                        {
                            if (bookedSlot.IsTimeSlotOverlap(reqVeh.BookedSlots[0]))
                            {
                                available = false;
                            }
                        }
                        if (available)
                        {
                            availableVeh.Add(v);
                        }
                    }

                }
            }
            return availableVeh;
        }

        private static void AddVehicleToBranch(Branch b, string inp)
        {
            string vehicleStr = inp.Substring(inp.IndexOf(',') + 3, inp.IndexOf(')') - 2 - inp.IndexOf(',') - 2);
            string vehStr = vehicleStr.Split()[1];
            Vehicle vehObj = getInstanceOfVehicle(vehStr);
            vehObj.Branch = b.Name;
            vehObj.cost = b.vehicles[vehStr][0].cost;
            AddVehicleToBranchUtil(b, vehStr, vehObj);
        }

        private static void PopulateAvailableVehicles(Dictionary<string, Type> vehicleDic)
        {
            vehicleDic.Add("suv", typeof(Suv));
            vehicleDic.Add("sedan", typeof(Sedan));
        }

        private static void AddVehiclesToBranch(Branch branch, List<string> vs)
        {
            foreach (string v in vs)
            {
                string vT = v.Trim();
                List<string> vehicleSPlit = vT.Substring(1, vT.Length - 2).Split().ToList();
                int noOfV = int.Parse(vehicleSPlit[0]);
                for (int i = 0; i < noOfV; i++)
                {
                    Vehicle vehObj = getInstanceOfVehicle(vehicleSPlit[1]);
                    vehObj.cost = int.Parse(vehicleSPlit[3].Substring(vehicleSPlit[3].IndexOf('.') + 1));
                    vehObj.Branch = branch.Name;
                    AddVehicleToBranchUtil(branch, vehObj.Name, vehObj);
                }
            }
        }

        private static void AddVehicleToBranchUtil(Branch branch, string vehStr, Vehicle vehObj)
        {
            if (!branch.vehicles.ContainsKey(vehStr))
            {
                branch.vehicles.Add(vehStr, new List<Vehicle>());
            }
            branch.vehicles[vehStr].Add(vehObj);
        }

        private static Vehicle getInstanceOfVehicle(string v)
        {
            return Activator.CreateInstance(vehicleDic[v], new object[] { v }) as Vehicle;
        }

        private static void AddBranchToCity(City c, string inp)
        {
            string branch = inp.Substring(inp.IndexOf('(') + 2, inp.IndexOf(',') - "add_branch".Length - 3);
            c.branchesInCity.Add(branch, new Branch());
            c.branchesInCity[branch].Name = branch;
            string vehicleString = inp.Substring(inp.IndexOf('[') + 1, inp.IndexOf(']') - inp.IndexOf('[') - 1);
            List<string> vs = vehicleString.Split(',').ToList();
            AddVehiclesToBranch(c.branchesInCity[branch], vs);
        }

        private static string ReadCommand()
        {
            return Console.ReadLine();
        }
    }
}
