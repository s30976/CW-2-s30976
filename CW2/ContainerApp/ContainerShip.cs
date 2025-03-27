using System;
using System.Collections.Generic;
using System.Linq;

namespace ContainerApp
{
    public class ContainerShip
    {
        public string Name { get; }
        public int MaxContainerCount { get; }
        public double MaxWeightTons { get; }
        public double MaxSpeedKnots { get; }

        public List<Container> Containers { get; } = new();

        public ContainerShip(string name, int maxContainerCount, double maxWeightTons, double maxSpeedKnots)
        {
            Name = name;
            MaxContainerCount = maxContainerCount;
            MaxWeightTons = maxWeightTons;
            MaxSpeedKnots = maxSpeedKnots;
        }

        public void LoadContainer(Container container)
        {
            if (Containers.Count >= MaxContainerCount)
                throw new Exception("Przekroczono maksymalną liczbę kontenerów");

            double totalWeight = Containers.Sum(c => c.CurrentLoadKg + c.OwnWeightKg);
            if ((totalWeight + container.CurrentLoadKg + container.OwnWeightKg) / 1000.0 > MaxWeightTons)
                throw new Exception("Przekroczono maksymalną ładowność statku");

            Containers.Add(container);
        }

        public void UnloadContainer(string serial)
        {
            var found = Containers.FirstOrDefault(c => c.SerialNumber == serial);
            if (found != null) Containers.Remove(found);
        }

        public void ReplaceContainer(string serial, Container newContainer)
        {
            int index = Containers.FindIndex(c => c.SerialNumber == serial);
            if (index == -1)
                throw new Exception($"Kontener {serial} nie istnieje na statku {Name}.");

            var oldCont = Containers[index];
            oldCont.Unload();

            Containers[index] = newContainer;
        }


        public override string ToString() =>
            $"Statek: {Name}, Kontenery: {Containers.Count}/{MaxContainerCount}, Ładowność: {MaxWeightTons} ton";
    }
}