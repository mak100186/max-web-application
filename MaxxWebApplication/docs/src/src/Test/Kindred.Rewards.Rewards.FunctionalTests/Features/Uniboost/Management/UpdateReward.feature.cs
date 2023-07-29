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
namespace Kindred.Rewards.Rewards.FunctionalTests.Features.Uniboost.Management
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Updating rewards")]
    public partial class UpdatingRewardsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "UpdateReward.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Uniboost/Management", "Updating rewards", "[PATCH] /reward/{rewardRn}\r\n[PUT] /reward/{rewardRn}", ProgrammingLanguage.CSharp, featureTags);
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
#line 6
#line hidden
            TechTalk.SpecFlow.Table table380 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "rewardType",
                        "customer_id",
                        "expiryDaysFromNow",
                        "countryCode",
                        "startDaysFromNow",
                        "comments"});
            table380.AddRow(new string[] {
                        "uniboost_5_pct_bonus",
                        "uniboost",
                        "321",
                        "",
                        "AUS",
                        "",
                        ""});
            table380.AddRow(new string[] {
                        "uniboost_6_pct_bonus",
                        "uniboost",
                        "123",
                        "",
                        "RUS",
                        "",
                        ""});
            table380.AddRow(new string[] {
                        "uniboost_7_pct_bonus",
                        "uniboost",
                        "123",
                        "",
                        "PAK",
                        "1",
                        ""});
            table380.AddRow(new string[] {
                        "uniboost_8_pct_bonus",
                        "uniboost",
                        "123",
                        "",
                        "USA",
                        "",
                        ""});
            table380.AddRow(new string[] {
                        "uniboost_9_pct_bonus",
                        "uniboost",
                        "123",
                        "time-in-next-daylight-saving-time-shift",
                        "CHN",
                        "",
                        ""});
            table380.AddRow(new string[] {
                        "uniboost_4_pct_bonus",
                        "uniboost",
                        "123",
                        "",
                        "FRA",
                        "1",
                        ""});
            table380.AddRow(new string[] {
                        "uniboost_3_pct_bonus",
                        "uniboost",
                        "123",
                        "20",
                        "FRA",
                        "10",
                        ""});
            table380.AddRow(new string[] {
                        "uniboost_10_pct_bonus",
                        "uniboost",
                        "123",
                        "",
                        "BEL",
                        "",
                        "comment_uniboost_10_pct_bonus"});
#line 7
    testRunner.Given("I have bonuses in the system", ((string)(null)), table380, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Update an existing bonus")]
        [NUnit.Framework.CategoryAttribute("bonus")]
        public void UpdateAnExistingBonus()
        {
            string[] tagsOfScenario = new string[] {
                    "bonus"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Update an existing bonus", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 20
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
#line 21
    testRunner.When("I submit UpdateBonus request for \'uniboost_7_pct_bonus\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 22
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 23
    testRunner.When("I submit GetBonus request for \'uniboost_7_pct_bonus\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 24
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 25
    testRunner.And("the UpdateBonus response should return updated bonus", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Update an existing bonus with invalid parameters")]
        [NUnit.Framework.CategoryAttribute("bonus")]
        public void UpdateAnExistingBonusWithInvalidParameters()
        {
            string[] tagsOfScenario = new string[] {
                    "bonus"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Update an existing bonus with invalid parameters", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 28
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
#line 29
    testRunner.When("I submit UpdateBonus request for \'uniboost_7_pct_bonus\' with invalid parameters", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 30
    testRunner.Then("the HttpStatusCode should be 400", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 31
    testRunner.And("Response should contain error \'\'Max Stake Amount\' must be greater than \'0\'\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Updating an existing bonus with a new customer ID should be disallowed")]
        [NUnit.Framework.CategoryAttribute("bonus")]
        public void UpdatingAnExistingBonusWithANewCustomerIDShouldBeDisallowed()
        {
            string[] tagsOfScenario = new string[] {
                    "bonus"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Updating an existing bonus with a new customer ID should be disallowed", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 35
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
#line 36
    testRunner.When("I submit UpdateBonus request for \'uniboost_9_pct_bonus\' with \'3333\' as Customer I" +
                        "D", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 37
    testRunner.Then("the HttpStatusCode should be 400", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 38
    testRunner.And("Response should contain error \'CustomerId of an existing reward cannot be updated" +
                        "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Patching an existing bonus with null patch request should not update anything")]
        [NUnit.Framework.CategoryAttribute("bonus")]
        public void PatchingAnExistingBonusWithNullPatchRequestShouldNotUpdateAnything()
        {
            string[] tagsOfScenario = new string[] {
                    "bonus"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Patching an existing bonus with null patch request should not update anything", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 41
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
#line 42
    testRunner.When("I submit a null patch bonus request for \'uniboost_10_pct_bonus\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 43
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table381 = new TechTalk.SpecFlow.Table(new string[] {
                            "comments",
                            "name"});
                table381.AddRow(new string[] {
                            "comment_uniboost_10_pct_bonus",
                            "uniboost_10_pct_bonus"});
#line 44
    testRunner.And("the patch bonus response should return updated bonus with the following", ((string)(null)), table381, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Patching an existing bonus with patch request should update fields")]
        [NUnit.Framework.CategoryAttribute("bonus")]
        public void PatchingAnExistingBonusWithPatchRequestShouldUpdateFields()
        {
            string[] tagsOfScenario = new string[] {
                    "bonus"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Patching an existing bonus with patch request should update fields", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 49
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table382 = new TechTalk.SpecFlow.Table(new string[] {
                            "comments",
                            "name"});
                table382.AddRow(new string[] {
                            "hello",
                            "my-reward-name"});
#line 50
    testRunner.When("I submit a patch bonus request for the following criteria for \'uniboost_10_pct_bo" +
                        "nus\'", ((string)(null)), table382, "When ");
#line hidden
#line 53
    testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table383 = new TechTalk.SpecFlow.Table(new string[] {
                            "comments",
                            "name"});
                table383.AddRow(new string[] {
                            "hello",
                            "my-reward-name"});
#line 54
    testRunner.And("the patch bonus response should return updated bonus with the following", ((string)(null)), table383, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion