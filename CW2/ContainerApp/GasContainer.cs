using System;

namespace ContainerApp
{
    public class GasContainer : Container, IHazardNotifier
    {
        public double PressureAtm { get; }

        public GasContainer(double maxLoadKg, double ownWeightKg, double heightCm, double depthCm, double pressureAtm)
            : base(maxLoadKg, ownWeightKg, heightCm, depthCm, 'G')
        {
            PressureAtm = pressureAtm;
        }

        public override void Unload()
        {
            CurrentLoadKg *= 0.05; 
        }

        public override void Load(double mass)
        {
            if (mass + CurrentLoadKg > MaxLoadKg)
            {
                NotifyHazard($"Próba przeładowania kontenera {SerialNumber} (gaz)");
                throw new OverfillException("Załadunek przekracza pojemność kontenera gazowego.");
            }

            base.Load(mass);
        }
        
        public override void UnloadPortion(double mass)
        {
            if (mass <= 0) return;


            if (mass >= CurrentLoadKg)
            {
                CurrentLoadKg *= 0.05; 
            }
            else
            {
                base.UnloadPortion(mass);
            }
        }


        public void NotifyHazard(string message)
        {
            Console.WriteLine($"[HAZARD - GAS] {message}");
        }
    }
}