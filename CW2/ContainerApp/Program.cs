using System;
using System.Collections.Generic;
using System.Linq;

namespace ContainerApp
{
    class Program
    {
        static readonly List<Container> AllContainers = new();
        static readonly List<ContainerShip> AllShips = new();

        static void Main(string[] _)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n== MENU ==");
                Console.WriteLine("1. Dodaj statek");
                Console.WriteLine("2. Dodaj kontener");
                Console.WriteLine("3. Załaduj kontener na statek");
                Console.WriteLine("4. Załaduj listę kontenerów na statek");
                Console.WriteLine("5. Rozładuj kontener");
                Console.WriteLine("6. Usuń kontener ze statku");
                Console.WriteLine("7. Zastąp kontener na statku");
                Console.WriteLine("8. Przenieś kontener między statkami");
                Console.WriteLine("9. Szczegóły kontenera");
                Console.WriteLine("10. Szczegóły statków");
                Console.WriteLine("11. Załaduj ładunek do kontenera");
                Console.WriteLine("12. Wyładuj część ładunku z kontenera");
                Console.WriteLine("0. Wyjście");
                Console.Write("Wybierz opcję: ");

                switch (Console.ReadLine())
                {
                    case "1": DodajStatek(); break;
                    case "2": DodajKontener(); break;
                    case "3": ZaladujKontenerNaStatek(); break;
                    case "4": ZaladujListeNaStatek(); break;
                    case "5": RozladujKontener(); break;
                    case "6": UsunKontenerZeStatku(); break;
                    case "7": ZastapKontener(); break;
                    case "8": PrzeniesKontener(); break;
                    case "9": SzczegolyKontenera(); break;
                    case "10": SzczegolyStatkow(); break;
                    case "11": ZaladujLadunekDoKontenera(); break;
                    case "12": WyladujCzescLadunku(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Nieprawidłowa opcja."); break;
                }
            }
        }

        static void DodajStatek()
        {
            Console.Write("Nazwa statku: ");
            string? name = Console.ReadLine();
            if (!TryReadInt("Max kontenerów: ", out int maxCount)) return;
            if (!TryReadDouble("Max ładowność (tony): ", out double maxWeight)) return;
            if (!TryReadDouble("Max prędkość (węzły): ", out double speed)) return;
            AllShips.Add(new ContainerShip(name!, maxCount, maxWeight, speed));
            Console.WriteLine("Dodano statek.");
        }

        static void DodajKontener()
        {
            Console.WriteLine("Typ: 1-Liquid, 2-Gas, 3-Refrigerated");
            string? type = Console.ReadLine();
            if (!TryReadDouble("Max ładowność [kg]: ", out double max)) return;
            if (!TryReadDouble("Waga własna [kg]: ", out double own)) return;
            if (!TryReadDouble("Wysokość [cm]: ", out double h)) return;
            if (!TryReadDouble("Głębokość [cm]: ", out double d)) return;

            switch (type)
            {
                case "1":
                    Console.Write("Niebezpieczny? (true/false): ");
                    if (!bool.TryParse(Console.ReadLine(), out bool hazard)) return;
                    AllContainers.Add(new LiquidContainer(max, own, h, d, hazard));
                    break;
                case "2":
                    if (!TryReadDouble("Ciśnienie [atm]: ", out double p)) return;
                    AllContainers.Add(new GasContainer(max, own, h, d, p));
                    break;
                case "3":
                {
                    Console.WriteLine(" 0-Bananas, 1-Chocolate, 2-Fish, 3-Meat, 4-IceCream,");
                    Console.WriteLine(" 5-FrozenPizza, 6-Cheese, 7-Sausages, 8-Butter, 9-Eggs");

                    if (!int.TryParse(Console.ReadLine(), out int pt) || pt < 0 || pt > 9) return;
                    if (!TryReadDouble("Temperatura [°C]: ", out double t)) return;

                    try
                    {
                        var refrCont = new RefrigeratedContainer(max, own, h, d, (ProductType)pt, t);
                        AllContainers.Add(refrCont);
                        Console.WriteLine("Dodano kontener chłodniczy.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("BŁĄD przy tworzeniu kontenera chłodniczego: " + ex.Message);
                        return; 
                    }
                    break;
                }
            }
            Console.WriteLine("Dodano kontener.");
        }

        static void ZaladujKontenerNaStatek()
        {
            if (!WybierzStatek(out var ship) || !WybierzKontener(out var cont)) return;
            try
            {
                ship.LoadContainer(cont);
        
                AllContainers.Remove(cont);
        
                Console.WriteLine("Załadowano.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }


        static void ZaladujListeNaStatek()
        {
            if (!WybierzStatek(out var ship)) return;

            while (true)
            {
                if (AllContainers.Count == 0)
                {
                    Console.WriteLine("Brak wolnych kontenerów.");
                    return;
                }
        
                Console.WriteLine("\nWolne kontenery:");
                for (int i = 0; i < AllContainers.Count; i++)
                {
                    Console.WriteLine($"{i}: {AllContainers[i].SerialNumber}");
                }

                Console.Write("Podaj indeks kontenera do załadowania (lub '-' żeby zakończyć): ");
                string? input = Console.ReadLine();
                if (input == null) continue;  
        
                if (input.Trim() == "-")
                {
                    break;
                }

                if (!int.TryParse(input, out int idx) || idx < 0 || idx >= AllContainers.Count)
                {
                    Console.WriteLine("Błędny indeks kontenera.");
                    continue;
                }

                var cont = AllContainers[idx];

                try
                {
                    ship.LoadContainer(cont);
                    AllContainers.Remove(cont);
                    Console.WriteLine($"Załadowano kontener {cont.SerialNumber} na statek {ship.Name}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Błąd: {e.Message}");
                }
            }

            Console.WriteLine("Koniec ładowania kontenerów na statek.");
        }


        static void RozladujKontener()
        {
            if (!WybierzKontener(out var cont)) return;
            cont.Unload();
            Console.WriteLine("Kontener rozładowany.");
        }

        static void UsunKontenerZeStatku()
        {
            if (!WybierzStatek(out var ship)) return;

            Console.Write("Podaj numer kontenera do usunięcia: ");
            string? serial = Console.ReadLine();
            if (serial == null) return;

            var found = ship.Containers.FirstOrDefault(c => c.SerialNumber == serial);
            if (found == null)
            {
                Console.WriteLine("Nie znaleziono takiego kontenera na statku.");
                return;
            }

            ship.UnloadContainer(serial);

            AllContainers.Add(found);

            Console.WriteLine("Usunięto kontener ze statku i przywrócono do listy wolnych kontenerów.");
        }


        static void ZastapKontener()
        {
            if (!WybierzStatek(out var ship)) 
                return;

            Console.WriteLine("Wybierz kontener do zastąpienia:");
            if (!WybierzKontenerZeStatku(ship, out var oldCont)) 
                return;

            Console.WriteLine("Wybierz NOWY kontener do wstawienia:");
            if (!WybierzKontener(out var newCont)) 
                return;

            try
            {
                ship.ReplaceContainer(oldCont.SerialNumber, newCont);
                Console.WriteLine($"Zastąpiono kontener {oldCont.SerialNumber} nowym {newCont.SerialNumber}.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Błąd: " + e.Message);
            }
        }

        
        static bool WybierzKontenerZeStatku(ContainerShip ship, out Container cont)
        {
            cont = null!;
            if (ship.Containers.Count == 0)
            {
                Console.WriteLine("Ten statek nie ma żadnych kontenerów.");
                return false;
            }

            for (int i = 0; i < ship.Containers.Count; i++)
                Console.WriteLine($"{i}: {ship.Containers[i].SerialNumber}");

            Console.Write("Wybierz indeks kontenera: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 0 || idx >= ship.Containers.Count)
            {
                Console.WriteLine("Błędny indeks kontenera.");
                return false;
            }

            cont = ship.Containers[idx];
            return true;
        }




        static void PrzeniesKontener()
        {
            Console.WriteLine("Z którego statku?");
            if (!WybierzStatek(out var fromShip)) return;
            Console.Write("Numer kontenera: ");
            string? sn = Console.ReadLine();
            var cont = fromShip.Containers.FirstOrDefault(c => c.SerialNumber == sn);
            if (cont == null) { Console.WriteLine("Nie znaleziono."); return; }

            Console.WriteLine("Na który statek?");
            if (!WybierzStatek(out var toShip)) return;

            fromShip.UnloadContainer(sn!);
            try { toShip.LoadContainer(cont); Console.WriteLine("Przeniesiono."); }
            catch (Exception e) { Console.WriteLine("Błąd: " + e.Message); }
        }

        static void SzczegolyKontenera()
        {
            Console.Write("Numer seryjny kontenera: ");
            string? sn = Console.ReadLine();
            var cont = AllContainers.FirstOrDefault(c => c.SerialNumber == sn);
            Console.WriteLine(cont != null ? cont.ToString() : "Nie znaleziono.");
        }

        static void SzczegolyStatkow()
        {
            foreach (var ship in AllShips)
            {
                Console.WriteLine(ship);
                foreach (var c in ship.Containers)
                    Console.WriteLine("  - " + c);
            }
        }

        static bool WybierzStatek(out ContainerShip ship)
        {
            ship = null!;
            if (AllShips.Count == 0) { Console.WriteLine("Brak statków."); return false; }
            for (int i = 0; i < AllShips.Count; i++)
                Console.WriteLine($"{i}: {AllShips[i].Name}");
            Console.Write("Wybierz numer statku: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 0 || idx >= AllShips.Count) return false;
            ship = AllShips[idx]; return true;
        }

        static bool WybierzKontener(out Container cont)
        {
            cont = null!;
            if (AllContainers.Count == 0) { Console.WriteLine("Brak kontenerów."); return false; }
            for (int i = 0; i < AllContainers.Count; i++)
                Console.WriteLine($"{i}: {AllContainers[i].SerialNumber}");
            Console.Write("Wybierz numer kontenera: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 0 || idx >= AllContainers.Count) return false;
            cont = AllContainers[idx]; return true;
        }
        static void ZaladujLadunekDoKontenera()
        {
            var cont = WybierzDowolnyKontener();
            if (cont == null) return;  

            Console.Write("Ile kg chcesz załadować? ");
            string? input = Console.ReadLine();
            if (!double.TryParse(input, out double mass))
            {
                Console.WriteLine("Nieprawidłowa liczba.");
                return;
            }

            try
            {
                cont.Load(mass);
                Console.WriteLine("Załadowano ładunek do kontenera.");
            }
            catch (OverfillException ex)
            {
                Console.WriteLine($"BŁĄD: {ex.Message}");
            }
        }
        
        
        static void WyladujCzescLadunku()
        {
            var cont = WybierzDowolnyKontener();
            if (cont == null) return;

            Console.Write("Ile kg chcesz wyładować? ");
            string? input = Console.ReadLine();
            if (!double.TryParse(input, out double mass) || mass < 0)
            {
                Console.WriteLine("Nieprawidłowa wartość.");
                return;
            }

            double before = cont.CurrentLoadKg; 
            cont.UnloadPortion(mass);
            double after = cont.CurrentLoadKg;

            Console.WriteLine(
                $"Wyładowano {before - after} kg z kontenera {cont.SerialNumber}.\n" +
                $"Aktualna masa ładunku: {after} kg."
            );
        }


        
        static Container? WybierzDowolnyKontener()
        {
            var merged = new List<Container>();

            merged.AddRange(AllContainers);

            foreach (var s in AllShips)
            {
                merged.AddRange(s.Containers);
            }

            if (merged.Count == 0)
            {
                Console.WriteLine("Brak kontenerów w systemie.");
                return null;
            }

            for (int i = 0; i < merged.Count; i++)
                Console.WriteLine($"{i}: {merged[i].SerialNumber} (na statku? {IsOnShip(merged[i])})");

            Console.Write("Wybierz numer kontenera: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= merged.Count)
            {
                Console.WriteLine("Błędny wybór.");
                return null;
            }

            return merged[index];
        }
        
        static bool IsOnShip(Container c)
        {
            return AllShips.Any(ship => ship.Containers.Contains(c));
        }


        static bool TryReadInt(string label, out int result)
        {
            Console.Write(label); return int.TryParse(Console.ReadLine(), out result);
        }

        static bool TryReadDouble(string label, out double result)
        {
            Console.Write(label); return double.TryParse(Console.ReadLine(), out result);
        }
    }
}