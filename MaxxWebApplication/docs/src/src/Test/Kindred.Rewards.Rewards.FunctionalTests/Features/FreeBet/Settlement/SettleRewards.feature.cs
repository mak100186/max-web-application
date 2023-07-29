﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Kindred.Rewards.Rewards.FunctionalTests.Features.FreeBet.Settlement
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("ClaimSettle")]
    [NUnit.Framework.CategoryAttribute("Acceptance")]
    [NUnit.Framework.CategoryAttribute("freebet")]
    public partial class ClaimSettleFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "Acceptance",
                "freebet"};
        
#line 1 "SettleRewards.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/FreeBet/Settlement", "ClaimSettle", null, ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 4
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "combinationRn",
                        "selectionOutcomes"});
            table1.AddRow(new string[] {
                        "ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1",
                        "barcaWin"});
#line 5
    testRunner.Given("a bet \'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1\' for customer \'918737\' th" +
                    "at has formulae \'singles\' and stake \'14\' with the combinations:", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "outcome",
                        "price",
                        "market"});
            table2.AddRow(new string[] {
                        "barcaWin",
                        "1.2",
                        "ksp:market.1:[basketball:201711202200:barca_vs_liverpool]:1x2"});
#line 8
    testRunner.And("the bet \'ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1\' has the following stag" +
                    "es:", ((string)(null)), table2, "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Settling a claimed FreeBet for a singles without previous settlement should retur" +
            "n the negative of the combination stake when the bet payoff is greater than or e" +
            "qual to the freebet amount")]
        [NUnit.Framework.CategoryAttribute("singles")]
        public void SettlingAClaimedFreeBetForASinglesWithoutPreviousSettlementShouldReturnTheNegativeOfTheCombinationStakeWhenTheBetPayoffIsGreaterThanOrEqualToTheFreebetAmount()
        {
            string[] tagsOfScenario = new string[] {
                    "singles"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Settling a claimed FreeBet for a singles without previous settlement should retur" +
                    "n the negative of the combination stake when the bet payoff is greater than or e" +
                    "qual to the freebet amount", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 13
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "rewardType",
                            "allowedFormulae",
                            "minStages",
                            "maxStages",
                            "minCombinations",
                            "maxCombinations",
                            "customer_id",
                            "amount"});
                table3.AddRow(new string[] {
                            "uniboost_reward_1",
                            "freebet",
                            "singles",
                            "1",
                            "1",
                            "1",
                            "1",
                            "918737",
                            "5"});
#line 14
    testRunner.Given("I have bonuses in the system", ((string)(null)), table3, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "template_keys"});
                table4.AddRow(new string[] {
                            "918737",
                            ""});
#line 17
    testRunner.When("I submit GetEntitlement request for following criteria:", ((string)(null)), table4, "When ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "betRn"});
                table5.AddRow(new string[] {
                            "uniboost_reward_1",
                            "ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1"});
#line 20
    testRunner.When("a request to claim an entitlement is received for customer \'918737\':", ((string)(null)), table5, "When ");
#line hidden
#line 23
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "name"});
                table6.AddRow(new string[] {
                            "918737"});
#line 24
    testRunner.And("I expect following in the ClaimedEntitlements for the customerID=\'918737\':", ((string)(null)), table6, "And ");
#line hidden
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "combinationRn",
                            "settlementStatus",
                            "segmentStatuses"});
                table7.AddRow(new string[] {
                            "ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1",
                            "Resolved",
                            "Won"});
#line 27
    testRunner.When("a request to settle the claim is received for bet \'ksp:bet.1:c0640978-231d-405c-8" +
                        "ff5-f767b57a5eac:1\' with final payoff \'12\' and combination settlement statuses:", ((string)(null)), table7, "When ");
#line hidden
#line 30
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "payOff",
                            "prevPayOff"});
                table8.AddRow(new string[] {
                            "-14",
                            "0"});
#line 31
    testRunner.And("the response should be:", ((string)(null)), table8, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Settling a claimed FreeBet for a singles without previous settlement should retur" +
            "n the negative of the bet payoff when the bet payoff is less than freebet amount" +
            "")]
        [NUnit.Framework.CategoryAttribute("singles")]
        public void SettlingAClaimedFreeBetForASinglesWithoutPreviousSettlementShouldReturnTheNegativeOfTheBetPayoffWhenTheBetPayoffIsLessThanFreebetAmount()
        {
            string[] tagsOfScenario = new string[] {
                    "singles"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Settling a claimed FreeBet for a singles without previous settlement should retur" +
                    "n the negative of the bet payoff when the bet payoff is less than freebet amount" +
                    "", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 36
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "rewardType",
                            "allowedFormulae",
                            "minStages",
                            "maxStages",
                            "minCombinations",
                            "maxCombinations",
                            "customer_id",
                            "amount"});
                table9.AddRow(new string[] {
                            "uniboost_reward_1",
                            "freebet",
                            "singles",
                            "1",
                            "1",
                            "1",
                            "1",
                            "918737",
                            "5"});
#line 37
    testRunner.Given("I have bonuses in the system", ((string)(null)), table9, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "template_keys"});
                table10.AddRow(new string[] {
                            "918737",
                            ""});
#line 40
    testRunner.When("I submit GetEntitlement request for following criteria:", ((string)(null)), table10, "When ");
#line hidden
                TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "betRn"});
                table11.AddRow(new string[] {
                            "uniboost_reward_1",
                            "ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1"});
#line 43
    testRunner.When("a request to claim an entitlement is received for customer \'918737\':", ((string)(null)), table11, "When ");
#line hidden
#line 46
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                            "name"});
                table12.AddRow(new string[] {
                            "918737"});
#line 47
    testRunner.And("I expect following in the ClaimedEntitlements for the customerID=\'918737\':", ((string)(null)), table12, "And ");
#line hidden
                TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
                            "combinationRn",
                            "settlementStatus",
                            "segmentStatuses"});
                table13.AddRow(new string[] {
                            "ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1",
                            "Resolved",
                            "Won"});
#line 50
    testRunner.When("a request to settle the claim is received for bet \'ksp:bet.1:c0640978-231d-405c-8" +
                        "ff5-f767b57a5eac:1\' with final payoff \'2\' and combination settlement statuses:", ((string)(null)), table13, "When ");
#line hidden
#line 53
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                            "payOff",
                            "prevPayOff"});
                table14.AddRow(new string[] {
                            "-2",
                            "0"});
#line 54
    testRunner.And("the response should be:", ((string)(null)), table14, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Settling a claimed FreeBet for a singles without previous settlement should retur" +
            "n 0 when the betpayoff is 0")]
        [NUnit.Framework.CategoryAttribute("singles")]
        public void SettlingAClaimedFreeBetForASinglesWithoutPreviousSettlementShouldReturn0WhenTheBetpayoffIs0()
        {
            string[] tagsOfScenario = new string[] {
                    "singles"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Settling a claimed FreeBet for a singles without previous settlement should retur" +
                    "n 0 when the betpayoff is 0", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 59
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "rewardType",
                            "allowedFormulae",
                            "minStages",
                            "maxStages",
                            "minCombinations",
                            "maxCombinations",
                            "customer_id",
                            "amount"});
                table15.AddRow(new string[] {
                            "uniboost_reward_1",
                            "freebet",
                            "singles",
                            "1",
                            "1",
                            "1",
                            "1",
                            "918737",
                            "5"});
#line 60
    testRunner.Given("I have bonuses in the system", ((string)(null)), table15, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table16 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "template_keys"});
                table16.AddRow(new string[] {
                            "918737",
                            ""});
#line 63
    testRunner.When("I submit GetEntitlement request for following criteria:", ((string)(null)), table16, "When ");
#line hidden
                TechTalk.SpecFlow.Table table17 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "betRn"});
                table17.AddRow(new string[] {
                            "uniboost_reward_1",
                            "ksp:bet.1:c0640978-231d-405c-8ff5-f767b57a5eac:1"});
#line 66
    testRunner.When("a request to claim an entitlement is received for customer \'918737\':", ((string)(null)), table17, "When ");
#line hidden
#line 69
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table18 = new TechTalk.SpecFlow.Table(new string[] {
                            "name"});
                table18.AddRow(new string[] {
                            "918737"});
#line 70
    testRunner.And("I expect following in the ClaimedEntitlements for the customerID=\'918737\':", ((string)(null)), table18, "And ");
#line hidden
                TechTalk.SpecFlow.Table table19 = new TechTalk.SpecFlow.Table(new string[] {
                            "combinationRn",
                            "settlementStatus",
                            "segmentStatuses"});
                table19.AddRow(new string[] {
                            "ksp:combination.1:c0640978-231d-405c-8ff5-f767b57a5eac:1:1",
                            "Resolved",
                            "Won"});
#line 73
    testRunner.When("a request to settle the claim is received for bet \'ksp:bet.1:c0640978-231d-405c-8" +
                        "ff5-f767b57a5eac:1\' with final payoff \'0\' and combination settlement statuses:", ((string)(null)), table19, "When ");
#line hidden
#line 76
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table20 = new TechTalk.SpecFlow.Table(new string[] {
                            "payOff",
                            "prevPayOff"});
                table20.AddRow(new string[] {
                            "0",
                            "0"});
#line 77
    testRunner.And("the response should be:", ((string)(null)), table20, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion