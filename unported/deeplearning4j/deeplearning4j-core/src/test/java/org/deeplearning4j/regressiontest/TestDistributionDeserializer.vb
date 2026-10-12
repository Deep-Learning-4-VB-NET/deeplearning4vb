Imports Microsoft.VisualBasic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.distribution
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.regressiontest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.JACKSON_SERDE) public class TestDistributionDeserializer extends org.deeplearning4j.BaseDL4JTest
	Public Class TestDistributionDeserializer
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L 'Most tests should be fast, but slow download may cause timeout on slow connections
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDistributionDeserializer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDistributionDeserializer()
			'Test current format:
'JAVA TO VB CONVERTER NOTE: The variable distributions was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim distributions_Conflict() As Distribution = {
				New NormalDistribution(3, 0.5),
				New UniformDistribution(-2, 1),
				New GaussianDistribution(2, 1.0),
				New BinomialDistribution(10, 0.3)
			}

			Dim om As ObjectMapper = NeuralNetConfiguration.mapper()

			For Each d As Distribution In distributions_Conflict
				Dim json As String = om.writeValueAsString(d)
				Dim fromJson As Distribution = om.readValue(json, GetType(Distribution))

				assertEquals(d, fromJson)
			Next d
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDistributionDeserializerLegacyFormat() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDistributionDeserializerLegacyFormat()
			Dim om As ObjectMapper = NeuralNetConfiguration.mapper()

			Dim normalJson As String = "{" & vbLf & "          ""normal"" : {" & vbLf & "            ""mean"" : 0.1," & vbLf & "            ""std"" : 1.2" & vbLf & "          }" & vbLf & "        }"

			Dim nd As Distribution = om.readValue(normalJson, GetType(Distribution))
			assertTrue(TypeOf nd Is NormalDistribution)
			Dim normDist As NormalDistribution = DirectCast(nd, NormalDistribution)
			assertEquals(0.1, normDist.Mean, 1e-6)
			assertEquals(1.2, normDist.Std, 1e-6)


			Dim uniformJson As String = "{" & vbLf & "                ""uniform"" : {" & vbLf & "                  ""lower"" : -1.1," & vbLf & "                  ""upper"" : 2.2" & vbLf & "                }" & vbLf & "              }"

			Dim ud As Distribution = om.readValue(uniformJson, GetType(Distribution))
			assertTrue(TypeOf ud Is UniformDistribution)
			Dim uniDist As UniformDistribution = DirectCast(ud, UniformDistribution)
			assertEquals(-1.1, uniDist.getLower(), 1e-6)
			assertEquals(2.2, uniDist.getUpper(), 1e-6)


			Dim gaussianJson As String = "{" & vbLf & "                ""gaussian"" : {" & vbLf & "                  ""mean"" : 0.1," & vbLf & "                  ""std"" : 1.2" & vbLf & "                }" & vbLf & "              }"

			Dim gd As Distribution = om.readValue(gaussianJson, GetType(Distribution))
			assertTrue(TypeOf gd Is GaussianDistribution)
			Dim gDist As GaussianDistribution = DirectCast(gd, GaussianDistribution)
			assertEquals(0.1, gDist.Mean, 1e-6)
			assertEquals(1.2, gDist.Std, 1e-6)

			Dim bernoulliJson As String = "{" & vbLf & "                ""binomial"" : {" & vbLf & "                  ""numberOfTrials"" : 10," & vbLf & "                  ""probabilityOfSuccess"" : 0.3" & vbLf & "                }" & vbLf & "              }"

			Dim bd As Distribution = om.readValue(bernoulliJson, GetType(Distribution))
			assertTrue(TypeOf bd Is BinomialDistribution)
			Dim binDist As BinomialDistribution = DirectCast(bd, BinomialDistribution)
			assertEquals(10, binDist.NumberOfTrials)
			assertEquals(0.3, binDist.ProbabilityOfSuccess, 1e-6)
		End Sub

	End Class

End Namespace