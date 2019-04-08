﻿using BarCheck.Data;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BarCheck.ViewModel
{
    public class ValidateRulesViewModel : ViewModelBase
    {
        private readonly Dictionary<string, List<ValidateRule>> dictRules;
        private readonly string BarcodeFormatsXmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BarCheck", "BarcodeFormats.xml");

        public ValidateRulesViewModel()
        {
            this.dictRules = this.ReadBarcodeFormatsXml(this.BarcodeFormatsXmlFileName);
            this.CreateObsT1VRules();
            this.ObsT2VRuleNames = new ObservableCollection<string>(new List<string>());
            this.AutoSelectLastValidateRule("组件一1");
            //this.AutoSelectLastValidateRule("组件一2");
            //this.AutoSelectLastValidateRule("组件二2");
        }

        public void AutoSelectLastValidateRule(string lastT2ruleName)
        {
            foreach (var kv in this.dictRules)
            {
                if (kv.Value.Any(ru => ru.Name == lastT2ruleName))
                {
                    this.SelectedT1VRule = kv.Key;
                    this.selectedT2VRuleName = lastT2ruleName;
                    break;
                }

            }
        }

        private void CreateObsT1VRules()
        {
            var t1s = this.dictRules.Keys;
            this.ObsT1VRules = new ObservableCollection<string>(t1s);
        }

        private void CreateObsT2VRules(string t1)
        {
            var vrules = this.dictRules[t1];
            this.ObsT2VRuleNames.Clear();
            foreach (string s in vrules.Select(vr => vr.Name))
                this.ObsT2VRuleNames.Add(s);

        }

        private Dictionary<string, List<ValidateRule>> ReadBarcodeFormatsXml(string fileName)
        {
            var dict =
                new Dictionary<string, List<ValidateRule>>();
            XElement rules = XElement.Load(fileName);
            foreach (var t1 in rules.Elements())
            {
                var t2s = new List<ValidateRule>();
                foreach (var t2 in t1.Elements())
                {
                    var rule = new ValidateRule(t2.Name.LocalName, t2.Value);
                    t2s.Add(rule);
                }
                dict.Add(t1.Name.LocalName, t2s);
            }
            return dict;
        }

        private string selectedT1VRule;
        public string SelectedT1VRule
        {
            get
            {
                return this.selectedT1VRule;
            }
            set
            {
                if (this.selectedT1VRule != value)
                {
                    this.selectedT1VRule = value;
                    this.RaisePropertyChanged(nameof(SelectedT1VRule));
                    this.CreateObsT2VRules(value);
                }
            }
        }

        private ObservableCollection<string> obsT1VRules;
        public ObservableCollection<string> ObsT1VRules
        {
            get
            {
                return this.obsT1VRules;
            }
            private set
            {
                if (this.obsT1VRules != value)
                {
                    this.obsT1VRules = value;
                    this.RaisePropertyChanged(nameof(ObsT1VRules));
                }
            }
        }

        private string selectedT2VRuleName;
        public string SelectedT2VRuleName
        {
            get
            {
                return this.selectedT2VRuleName;
            }
            set
            {
                if (this.selectedT2VRuleName != value)
                {
                    this.selectedT2VRuleName = value;
                    this.RaisePropertyChanged(nameof(SelectedT2VRuleName));
                }
            }
        }

        private ObservableCollection<string> obsT2VRuleNames;
        public ObservableCollection<string> ObsT2VRuleNames
        {
            get
            {
                return this.obsT2VRuleNames;
            }
            private set
            {
                if (this.obsT2VRuleNames != value)
                {
                    this.obsT2VRuleNames = value;
                    this.RaisePropertyChanged(nameof(ObsT2VRuleNames));
                }
            }
        }
    }


}
