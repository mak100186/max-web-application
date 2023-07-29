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
namespace Kindred.Rewards.Rewards.FunctionalTests.Features.Uniboost.Claiming
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("GetClaims")]
    public partial class GetClaimsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "GetClaims.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Uniboost/Claiming", "GetClaims", "[GET] /claims", ProgrammingLanguage.CSharp, featureTags);
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
#line 5
#line hidden
            TechTalk.SpecFlow.Table table300 = new TechTalk.SpecFlow.Table(new string[] {
                        "customer_id",
                        "name",
                        "instance_id",
                        "bet_client_ref",
                        "sequence_no",
                        "bet_outcome_status",
                        "CurrencyCode"});
            table300.AddRow(new string[] {
                        "92819278",
                        "claim_1",
                        "1",
                        "-81",
                        "999999999",
                        "Losing",
                        "AUD"});
            table300.AddRow(new string[] {
                        "92819279",
                        "claim_2",
                        "2",
                        "-82",
                        "999999998",
                        "",
                        ""});
            table300.AddRow(new string[] {
                        "92819279",
                        "claim_3",
                        "3",
                        "-83",
                        "999999997",
                        "",
                        "AUD"});
#line 6
 testRunner.Given("I have claims in the system", ((string)(null)), table300, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - No filter")]
        public void GetClaimsByFilter_NoFilter()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - No filter", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 12
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
#line 13
 testRunner.When("I submit GetClaims request for empty criteria", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 14
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Default ascending order of cliams")]
        public void GetClaimsByFilter_DefaultAscendingOrderOfCliams()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Default ascending order of cliams", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 16
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table301 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id"});
                table301.AddRow(new string[] {
                            "92819279"});
#line 17
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table301, "When ");
#line hidden
#line 20
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table302 = new TechTalk.SpecFlow.Table(new string[] {
                            "sort_order"});
                table302.AddRow(new string[] {
                            "ascending"});
#line 21
 testRunner.And("the claims are in following order of their update times", ((string)(null)), table302, "And ");
#line hidden
#line 24
 testRunner.And("Claims should have correct CurrencyCode values", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Descending order of cliams")]
        public void GetClaimsByFilter_DescendingOrderOfCliams()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Descending order of cliams", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 26
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table303 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "is_descending"});
                table303.AddRow(new string[] {
                            "92819279",
                            "true"});
#line 27
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table303, "When ");
#line hidden
#line 30
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table304 = new TechTalk.SpecFlow.Table(new string[] {
                            "sort_order"});
                table304.AddRow(new string[] {
                            "descending"});
#line 31
 testRunner.And("the claims are in following order of their update times", ((string)(null)), table304, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - filter by UpdateDateUTC. Also testing that the update time" +
            " of \'2019-10-06 02:03:05.119516\', which doesn\'t exist in the AEST timezone, does" +
            "n\'t cause any error.")]
        public void GetClaimsByFilter_FilterByUpdateDateUTC_AlsoTestingThatTheUpdateTimeOf2019_10_06020305_119516WhichDoesntExistInTheAESTTimezoneDoesntCauseAnyError_()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - filter by UpdateDateUTC. Also testing that the update time" +
                    " of \'2019-10-06 02:03:05.119516\', which doesn\'t exist in the AEST timezone, does" +
                    "n\'t cause any error.", null, tagsOfScenario, argumentsOfScenario, featureTags);
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
#line 5
this.FeatureBackground();
#line hidden
#line 36
 testRunner.When("I update the UpdateTime of a claim for customerId \'92819278\' to \'2019-10-06 02:03" +
                        ":05.119516\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table305 = new TechTalk.SpecFlow.Table(new string[] {
                            "update_date_time_utc_from",
                            "update_date_time_utc_to"});
                table305.AddRow(new string[] {
                            "2019-10-06 01:00:05.119516Z",
                            "2019-10-06 04:00:05.119516Z"});
#line 37
 testRunner.And("I submit GetClaims request for criteria", ((string)(null)), table305, "And ");
#line hidden
#line 40
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table306 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table306.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 41
 testRunner.And("GetClaims response should return result", ((string)(null)), table306, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by customer Id")]
        public void GetClaimsByFilter_FilterByCustomerId()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by customer Id", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 45
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table307 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id"});
                table307.AddRow(new string[] {
                            "92819278"});
#line 46
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table307, "When ");
#line hidden
#line 49
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table308 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table308.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 50
 testRunner.And("GetClaims response should return result", ((string)(null)), table308, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by instance Id")]
        public void GetClaimsByFilter_FilterByInstanceId()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by instance Id", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 54
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table309 = new TechTalk.SpecFlow.Table(new string[] {
                            "instance_id",
                            "row_count"});
                table309.AddRow(new string[] {
                            "1",
                            "1"});
#line 55
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table309, "When ");
#line hidden
#line 58
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table310 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table310.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 59
 testRunner.And("GetClaims response should return result", ((string)(null)), table310, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by reward key")]
        public void GetClaimsByFilter_FilterByRewardKey()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by reward key", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 63
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
#line 64
 testRunner.When("I submit a GetClaims request filtering by reward rn for claim: \'claim_1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 65
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table311 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table311.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 66
 testRunner.And("GetClaims response should return result", ((string)(null)), table311, "And ");
#line hidden
                TechTalk.SpecFlow.Table table312 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "bet_outcome_status"});
                table312.AddRow(new string[] {
                            "claim_1",
                            "Losing"});
#line 69
 testRunner.And("GetClaims response should return betoutcome status", ((string)(null)), table312, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by reward key for betoutcome")]
        public void GetClaimsByFilter_FilterByRewardKeyForBetoutcome()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by reward key for betoutcome", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 73
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
#line 74
 testRunner.When("I submit a GetClaims request filtering by reward rn for claim: \'claim_2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 75
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table313 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "bet_outcome_status"});
                table313.AddRow(new string[] {
                            "claim_2",
                            ""});
#line 76
 testRunner.And("GetClaims response should return betoutcome status", ((string)(null)), table313, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by promo id")]
        public void GetClaimsByFilter_FilterByPromoId()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by promo id", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 80
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table314 = new TechTalk.SpecFlow.Table(new string[] {
                            "name",
                            "row_count"});
                table314.AddRow(new string[] {
                            "claim_1",
                            "1"});
#line 81
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table314, "When ");
#line hidden
#line 84
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table315 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table315.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 85
 testRunner.And("GetClaims response should return result", ((string)(null)), table315, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by bet_client_ref")]
        public void GetClaimsByFilter_FilterByBet_Client_Ref()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by bet_client_ref", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 89
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table316 = new TechTalk.SpecFlow.Table(new string[] {
                            "bet_client_ref",
                            "row_count"});
                table316.AddRow(new string[] {
                            "-81",
                            "1"});
#line 90
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table316, "When ");
#line hidden
#line 93
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table317 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table317.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 94
 testRunner.And("GetClaims response should return result", ((string)(null)), table317, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by sequence_no")]
        public void GetClaimsByFilter_FilterBySequence_No()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by sequence_no", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 98
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table318 = new TechTalk.SpecFlow.Table(new string[] {
                            "sequence_no",
                            "row_count"});
                table318.AddRow(new string[] {
                            "999999999",
                            "1"});
#line 99
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table318, "When ");
#line hidden
#line 102
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table319 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table319.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 103
 testRunner.And("GetClaims response should return result", ((string)(null)), table319, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by reward type")]
        public void GetClaimsByFilter_FilterByRewardType()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by reward type", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 107
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table320 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "reward_type",
                            "row_count"});
                table320.AddRow(new string[] {
                            "92819278",
                            "Uniboost",
                            "1"});
#line 108
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table320, "When ");
#line hidden
#line 111
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table321 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table321.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 112
 testRunner.And("GetClaims response should return result", ((string)(null)), table321, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by bet_outcome_status")]
        public void GetClaimsByFilter_FilterByBet_Outcome_Status()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by bet_outcome_status", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 116
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table322 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "bet_outcome_status",
                            "row_count"});
                table322.AddRow(new string[] {
                            "92819278",
                            "Losing",
                            "1"});
#line 117
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table322, "When ");
#line hidden
#line 120
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table323 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table323.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 121
 testRunner.And("GetClaims response should return result", ((string)(null)), table323, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get claims by filter - Filter by claim_status")]
        public void GetClaimsByFilter_FilterByClaim_Status()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get claims by filter - Filter by claim_status", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 125
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table324 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "claim_status",
                            "row_count"});
                table324.AddRow(new string[] {
                            "92819278",
                            "Claimed",
                            "1"});
#line 126
 testRunner.When("I submit GetClaims request for criteria", ((string)(null)), table324, "When ");
#line hidden
#line 129
 testRunner.Then("the HttpStatusCode should be 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table325 = new TechTalk.SpecFlow.Table(new string[] {
                            "customer_id",
                            "row_count"});
                table325.AddRow(new string[] {
                            "92819278",
                            "1"});
#line 130
 testRunner.And("GetClaims response should return result", ((string)(null)), table325, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion