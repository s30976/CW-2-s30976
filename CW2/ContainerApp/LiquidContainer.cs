using System;

namespace ContainerApp
{
    public class LiquidContainer : Container, IHazardNotifier
    {
        public bool IsHazardous { get; }

        public LiquidContainer(double maxLoadKg, double ownWeightKg, double heightCm, double depthCm, bool isHazardous)
            : base(maxLoadKg, ownWeightKg, heightCm, depthCm, 'L')
        {
            IsHazardous = isHazardous;
        }

        public override void Load(double mass)
        {
            double allowed = IsHazardous ? MaxLoadKg * 0.5 : MaxLoadKg * 0.9;
            if (mass > allowed)
            {
                NotifyHazard($"Próba niebezpiecznego załadunku kontenera {SerialNumber}");
                throw new OverfillException($"Załadunek przekracza limity: {SerialNumber}");
            }

            base.Load(mass);
        }

        public void NotifyHazard(string message)
        {
            Console.WriteLine($"[HAZARD - LIQUID] {message}");
        }
    }
}