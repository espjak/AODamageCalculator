namespace AODamageCalculator.Data.SpecialAttacks
{
    //  Values taken from Budabot / Tyrbot
    public static class BrawlTemplates
    {
        public static BrawlTemplate Ql1Brawl = new() { BrawlSkill = 1, Damage = new IntRange(1, 2), CritModifier = 3 };

        public static BrawlTemplate Ql1000Brawl = new() { BrawlSkill = 1000, Damage = new IntRange(100, 500), CritModifier = 500 };

        public static BrawlTemplate Ql1001Brawl = new() { BrawlSkill = 1001, Damage = new IntRange(101, 501), CritModifier = 501 };

        public static BrawlTemplate Ql2000Brawl = new() { BrawlSkill = 2000, Damage = new IntRange(170, 850), CritModifier = 600 };

        public static BrawlTemplate Ql2001Brawl = new() { BrawlSkill = 2001, Damage = new IntRange(171, 851), CritModifier = 601 };

        public static BrawlTemplate Ql3000Brawl = new() { BrawlSkill = 3000, Damage = new IntRange(235, 1145), CritModifier = 725 };

        public static BrawlTemplate GetLowTemplate(int brawlSkill)
        {
            if (brawlSkill <= 1000)
                return Ql1Brawl;
            if (brawlSkill <= 2000)
                return Ql1001Brawl;

            return Ql2001Brawl;
        }

        public static BrawlTemplate GetHigTemplate(int brawlSkill)
        {
            if (brawlSkill <= 1000)
                return Ql1000Brawl;
            if (brawlSkill <= 2000)
                return Ql2000Brawl;

            return Ql3000Brawl;
        }
    }
}
