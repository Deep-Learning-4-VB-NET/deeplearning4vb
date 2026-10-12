Imports ImportOption = com.tngtech.archunit.core.importer.ImportOption
Imports AnalyzeClasses = com.tngtech.archunit.junit.AnalyzeClasses
Imports ArchTest = com.tngtech.archunit.junit.ArchTest
Imports ArchRule = com.tngtech.archunit.lang.ArchRule
Imports ArchUnitExtension = com.tngtech.archunit.lang.extension.ArchUnitExtension
Imports ArchUnitExtensions = com.tngtech.archunit.lang.extension.ArchUnitExtensions
Imports RunWith = org.junit.runner.RunWith
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
import static com.tngtech.archunit.lang.syntax.ArchRuleDefinition.classes
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 
Namespace org.datavec.api.transform.ops

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AnalyzeClasses(packages = "org.datavec.api.transform.ops", importOptions = { ImportOption.DoNotIncludeTests.class }) @DisplayName("Aggregable Multi Op Arch Test") class AggregableMultiOpArchTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class AggregableMultiOpArchTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ArchTest public static final com.tngtech.archunit.lang.ArchRule ALL_AGGREGATE_OPS_MUST_BE_SERIALIZABLE = classes().that().resideInAPackage("org.datavec.api.transform.ops").and().doNotHaveSimpleName("AggregatorImpls").and().doNotHaveSimpleName("IAggregableReduceOp").and().doNotHaveSimpleName("StringAggregatorImpls").and().doNotHaveFullyQualifiedName("org.datavec.api.transform.ops.StringAggregatorImpls$1").should().implement(java.io.Serializable.class).because("All aggregate ops must be serializable.");
		Public Shared ReadOnly ALL_AGGREGATE_OPS_MUST_BE_SERIALIZABLE As ArchRule = classes().that().resideInAPackage("org.datavec.api.transform.ops").and().doNotHaveSimpleName("AggregatorImpls").and().doNotHaveSimpleName("IAggregableReduceOp").and().doNotHaveSimpleName("StringAggregatorImpls").and().doNotHaveFullyQualifiedName("org.datavec.api.transform.ops.StringAggregatorImpls$1").should().implement(GetType(Serializable)).because("All aggregate ops must be serializable.")
	End Class

End Namespace