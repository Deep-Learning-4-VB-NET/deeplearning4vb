Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasFlattenRnnPreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.KerasFlattenRnnPreprocessor
Imports PermutePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.PermutePreprocessor
Imports ReshapePreprocessor = org.deeplearning4j.nn.modelimport.keras.preprocessors.ReshapePreprocessor
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.deeplearning4j.nn.modelimport.keras.configurations

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Json Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.JACKSON_SERDE) class JsonTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class JsonTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Json Preprocessors") void testJsonPreprocessors() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJsonPreprocessors()
			Dim pp() As InputPreProcessor = {
				New KerasFlattenRnnPreprocessor(10, 5),
				New PermutePreprocessor(New Integer() { 0, 1, 2 }),
				New ReshapePreprocessor(New Long() { 10, 10 }, New Long() { 100, 1 }, True, Nothing)
			}
			For Each p As InputPreProcessor In pp
				Dim s As String = NeuralNetConfiguration.mapper().writeValueAsString(p)
				Dim p2 As InputPreProcessor = NeuralNetConfiguration.mapper().readValue(s, GetType(InputPreProcessor))
				assertEquals(p, p2)
			Next p
		End Sub
	End Class

End Namespace