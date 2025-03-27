using System;

namespace ContainerApp
{
    public abstract class Container
    {
        private static int serialCounter = 1;

        public string SerialNumber { get; }
        public double MaxLoadKg { get; }
        public double CurrentLoadKg { get; protected set; }
        public double OwnWeightKg { get; }
        public double HeightCm { get; }
        public double DepthCm { get; }

        public Container(double maxLoadKg, double ownWeightKg, double heightCm, double depthCm, char typeCode)
        {
            MaxLoadKg = maxLoadKg;
            OwnWeightKg = ownWeightKg;
            HeightCm = heightCm;
            DepthCm = depthCm;
            SerialNumber = $"KON-{typeCode}-{serialCounter++}";
            CurrentLoadKg = 0;
        }

        public virtual void Load(double mass)
        {
            if (CurrentLoadKg + mass > MaxLoadKg)
                throw new OverfillException($"Próba przeładowania kontenera {SerialNumber}");

            CurrentLoadKg += mass;
        }

        public virtual void Unload()
        {
            CurrentLoadKg = 0;
        }
        
        public virtual void UnloadPortion(double mass)
        {
            if (mass <= 0) return;

            if (mass >= CurrentLoadKg)
            {
                CurrentLoadKg = 0;
            }
            else
            {
                CurrentLoadKg -= mass;
            }
        }


        public override string ToString() =>
            $"[{SerialNumber}] {CurrentLoadKg}/{MaxLoadKg} kg";
    }
}