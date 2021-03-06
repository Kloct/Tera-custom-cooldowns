﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using TCC.Annotations;
using TCC.Data;
using TCC.Test;
using TCC.ViewModels;
using Button = System.Windows.Controls.Button;

namespace TCC.Windows
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed partial class DebugWindow : INotifyPropertyChanged
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private int _last;
        private int _sum;
        private int _max;

        public int Last
        {
            get => _last;
            private set { _last = value; NPC(); }
        }
        public int Max
        {
            get => _max;
            private set { _max = value; NPC(); }
        }

        private int Sum
        {
            get => _sum;
            set { _sum = value; NPC(); }
        }
        public double Avg => _count == 0 ? 0 : Sum / _count;

        private int _count;
        public void SetQueuedPackets(int val)
        {
            Last = val;
            if (val > Max) Max = val;
            NPC(nameof(Avg));
            if (int.MaxValue - Sum < val)
            {
                Sum = 0;
                _count = 0;
            }
            else Sum += val;

            _count++;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SwitchClass(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentPlayer.Class = (Class)Enum.Parse(typeof(Class), (sender as Button).Content.ToString());
            WindowManager.ClassWindow.VM.CurrentClass = SessionManager.CurrentPlayer.Class;
            WindowManager.CooldownWindow.VM.ClearSkills();
            WindowManager.CooldownWindow.VM.LoadSkills(SessionManager.CurrentPlayer.Class);
        }

        private void SetSorcElement(object sender, RoutedEventArgs e)
        {
            var el = (sender as Button).Content.ToString();

            var fire = el == "Fire";
            var ice = el == "Ice";
            var arc = el == "Arcane";

            var currFire = SessionManager.CurrentPlayer.Fire;
            var currIce = SessionManager.CurrentPlayer.Ice;
            var currArc = SessionManager.CurrentPlayer.Arcane;

            if (fire) SessionManager.SetSorcererElements(!currFire, currIce, currArc);
            if (ice) SessionManager.SetSorcererElements(currFire, !currIce, currArc);
            if (arc) SessionManager.SetSorcererElements(currFire, currIce, !currArc);
        }

        private void SetSorcElementBoost(object sender, RoutedEventArgs e)
        {
            var el = (sender as Button).Content.ToString().Split(' ')[0];

            var fire = el == "Fire";
            var ice = el == "Ice";
            var arc = el == "Arcane";

            var currFire = SessionManager.CurrentPlayer.FireBoost;
            var currIce = SessionManager.CurrentPlayer.IceBoost;
            var currArc = SessionManager.CurrentPlayer.ArcaneBoost;

            if (fire) SessionManager.SetSorcererElementsBoost(!currFire, currIce, currArc);
            if (ice) SessionManager.SetSorcererElementsBoost(currFire, !currIce, currArc);
            if (arc) SessionManager.SetSorcererElementsBoost(currFire, currIce, !currArc);


        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Utils.CurrentClassVM<SorcererLayoutVM>().ManaBoost.Buff.Start(10000);

            SkillManager.AddSkill(100700, 20000);
            SkillManager.AddSkill(400120, 20000);
            SkillManager.AddItemSkill(6298, 10);
            SkillManager.ResetSkill(400120);
            //CooldownWindowViewModel.Instance.AddOrRefresh(new SkillCooldown(new Skill(100700, Class.Warrior, "dfa", ""),20000, CooldownType.Skill, Dispatcher.CurrentDispatcher, true, true));
        }

        private void SetStance(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString() == "Assault") Utils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.Assault;
            else if (((Button)sender).Content.ToString() == "Defensive") Utils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.Defensive;
            else if (((Button)sender).Content.ToString() == "None") Utils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.None;
        }

        private void IncreaseEdge(object sender, RoutedEventArgs e)
        {
            if (Utils.CurrentClassVM<WarriorLayoutVM>().EdgeCounter.IsMaxed) Utils.CurrentClassVM<WarriorLayoutVM>().EdgeCounter.Val = 0;
            Utils.CurrentClassVM<WarriorLayoutVM>().EdgeCounter.Val++;

        }

        private void RegisterWebhook(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 80; i++)
            {
                var i1 = i;
                Dispatcher.BeginInvoke(new Action(() => Tester.RegisterWebhook("user" + i1)));
            }
        }

        private void FireWebhook(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 80; i++)
            {
                var i1 = i;
                Dispatcher.BeginInvoke(new Action(() => Tester.FireWebhook("user" + i1)));
            }
        }
        private void DungeonTest(object sender, RoutedEventArgs e)
        {
            //i = 0;
            //WindowManager.Dashboard.VM.SetDungeons(20000078, new Dictionary<uint, short>() { { 9770U, i++ } });

            WindowManager.Dashboard.VM.Characters[0].VanguardDailiesDone = App.Random.Next(0, 16);
            WindowManager.Dashboard.VM.Characters[0].VanguardWeekliesDone = App.Random.Next(0, 16);
        }
    }
}
