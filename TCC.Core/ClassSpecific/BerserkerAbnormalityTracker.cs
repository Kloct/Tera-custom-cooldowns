﻿using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Utilities.Extensions;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class BerserkerAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int BloodlustId = 400701;
        private const int FieryRageId = 400105;

        private const int UnleashId = 401705;
        private const int Sinister = 401707;
        private const int Dexter = 401709;
        private const int Rampage = 401710;

        private const int SinisterKR = 401706; // KR patch by HQ
        private const int DexterKR = 401706;   // KR patch by HQ

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckUnleashAbnormals(p);
            if (p.AbnormalityId == BloodlustId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Start(p.Duration);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Start(p.Duration);
            }
            if (p.AbnormalityId == UnleashId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Start(p.Duration);
                Utils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = true;
                Utils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = false;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckUnleashAbnormals(p);

            if (p.AbnormalityId == BloodlustId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Refresh(p.Duration, CooldownMode.Normal);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Refresh(p.Duration, CooldownMode.Normal);
            }
            if (p.AbnormalityId == UnleashId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Refresh(p.Duration, CooldownMode.Normal);
                Utils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = true;
                Utils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = false;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckUnleashAbnormals(p);
            if (p.AbnormalityId == BloodlustId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().Bloodlust.Buff.Refresh(0, CooldownMode.Normal);
            }
            if (p.AbnormalityId == FieryRageId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().FieryRage.Buff.Refresh(0, CooldownMode.Normal);
            }
            if (p.AbnormalityId == UnleashId)
            {
                Utils.CurrentClassVM<BerserkerLayoutVM>().Unleash.Buff.Refresh(0, CooldownMode.Normal);
                Utils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOn = false;
                Utils.CurrentClassVM<BerserkerLayoutVM>().IsUnleashOff = true;
            }
        }

        private static void CheckUnleashAbnormals(S_ABNORMALITY_BEGIN p)
        {
            if(TimeManager.Instance.CurrentRegion == RegionEnum.KR)  // KR patch by HQ
            {
                if (p.AbnormalityId == SinisterKR && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == DexterKR && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
            else
            {
                if (p.AbnormalityId == Sinister && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_REFRESH p)
        {
            if (TimeManager.Instance.CurrentRegion == RegionEnum.KR)  // KR patch by HQ
            {
                if (p.AbnormalityId == SinisterKR && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == DexterKR && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
            else
            {
                if (p.AbnormalityId == Sinister && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Dexter && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = p.Stacks;
                }
                if (p.AbnormalityId == Rampage && p.TargetId.IsMe())
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = p.Stacks;
                }
            }
        }
        private static void CheckUnleashAbnormals(S_ABNORMALITY_END p)
        {
            if (TimeManager.Instance.CurrentRegion == RegionEnum.KR)  // KR patch by HQ
            {
                if (p.AbnormalityId == SinisterKR)
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = 0;
                }
                if (p.AbnormalityId == DexterKR)
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = 0;
                }
                if (p.AbnormalityId == Rampage)
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = 0;
                }
            }
            else
            { 
                if (p.AbnormalityId == Sinister)
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().SinisterTracker.Val = 0;
                }
                if (p.AbnormalityId == Dexter)
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().DexterTracker.Val = 0;
                }
                if (p.AbnormalityId == Rampage)
                {
                    Utils.CurrentClassVM<BerserkerLayoutVM>().RampageTracker.Val = 0;
                }
            }
        }

    }
}
