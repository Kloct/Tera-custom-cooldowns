﻿using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class ReaperAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int ShadowReapingId = 10151010;
        private const int ShadowStepId = 10151000;
        private const int DeathSpiralId = 10151131;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckShadowReaping(p);
            CheckShadowStep(p);
            CheckDeathSpiral(p);
        }

        private void CheckDeathSpiral(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != DeathSpiralId) return;
            StartPrecooldown("icon_skills.chainbrandish_tex", p.Duration);
        }

        private void CheckShadowStep(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != ShadowStepId) return;
            StartPrecooldown("icon_skills.instantleap_tex", p.Duration);
        }

        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckShadowReaping(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckShadowReaping(p);
        }

        private static void CheckShadowReaping(S_ABNORMALITY_BEGIN p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            ((ReaperBarManager)ClassWindowViewModel.Instance.CurrentManager).ShadowReaping.Buff.Start(p.Duration);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_REFRESH p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            ((ReaperBarManager)ClassWindowViewModel.Instance.CurrentManager).ShadowReaping.Buff.Refresh(p.Duration);
        }
        private static void CheckShadowReaping(S_ABNORMALITY_END p)
        {
            if (ShadowReapingId != p.AbnormalityId) return;
            ((ReaperBarManager)ClassWindowViewModel.Instance.CurrentManager).ShadowReaping.Buff.Refresh(0);
        }

    }
}
