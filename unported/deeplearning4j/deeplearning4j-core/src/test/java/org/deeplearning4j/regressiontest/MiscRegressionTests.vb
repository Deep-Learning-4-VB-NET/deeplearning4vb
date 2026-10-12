Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports FrozenLayer = org.deeplearning4j.nn.conf.layers.misc.FrozenLayer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
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
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class MiscRegressionTests extends org.deeplearning4j.BaseDL4JTest
	Public Class MiscRegressionTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFrozen() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFrozen()
			Dim f As File = (New ClassPathResource("regression_testing/misc/legacy_frozen/configuration.json")).File
			Dim json As String = FileUtils.readFileToString(f, StandardCharsets.UTF_8.name())
			Dim conf As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)

			Dim countFrozen As Integer = 0
			For Each e As KeyValuePair(Of String, GraphVertex) In conf.getVertices().entrySet()
				Dim gv As GraphVertex = e.Value
				assertNotNull(gv)
				If TypeOf gv Is LayerVertex Then
					Dim lv As LayerVertex = DirectCast(gv, LayerVertex)
					Dim layer As Layer = lv.getLayerConf().getLayer()
					If TypeOf layer Is FrozenLayer Then
						countFrozen += 1
					End If
				End If
			Next e

			assertTrue(countFrozen > 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFrozenNewFormat()
		Public Overridable Sub testFrozenNewFormat()
			Dim configuration As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, New FrozenLayer((New DenseLayer.Builder()).nIn(10).nOut(10).build())).build()

			Dim json As String = configuration.toJson()
			Dim fromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(configuration, fromJson)
		End Sub
	End Class

End Namespace