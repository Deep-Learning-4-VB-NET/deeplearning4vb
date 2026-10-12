Imports System.Linq
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RepeatVector = org.deeplearning4j.nn.conf.layers.misc.RepeatVector
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Repeat Vector Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class RepeatVectorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class RepeatVectorTest
		Inherits BaseDL4JTest

		Private REPEAT As Integer = 4

		Private ReadOnly Property RepeatVectorLayer As Layer
			Get
				Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).dataType(DataType.DOUBLE).layer((New RepeatVector.Builder(REPEAT)).build()).build()
				Return conf.getLayer().instantiate(conf, Nothing, 0, Nothing, False, DataType.DOUBLE)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Repeat Vector") void testRepeatVector()
		Friend Overridable Sub testRepeatVector()
			Dim arr() As Double = { 1.0, 2.0, 3.0, 1.0, 2.0, 3.0, 1.0, 2.0, 3.0, 1.0, 2.0, 3.0 }
			Dim expectedOut As INDArray = Nd4j.create(arr, New Long() { 1, 3, REPEAT }, "f"c)
			Dim input As INDArray = Nd4j.create(New Double() { 1.0, 2.0, 3.0 }, New Long() { 1, 3 })
			Dim layer As Layer = RepeatVectorLayer
			Dim output As INDArray = layer.activate(input, False, LayerWorkspaceMgr.noWorkspaces())
			assertTrue(expectedOut.shape().SequenceEqual(output.shape()))
			assertEquals(expectedOut, output)
			Dim epsilon As INDArray = Nd4j.ones(1, 3, 4)
			Dim [out] As Pair(Of Gradient, INDArray) = layer.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			Dim outEpsilon As INDArray = [out].Second
			Dim expectedEpsilon As INDArray = Nd4j.create(New Double() { 4.0, 4.0, 4.0 }, New Long() { 1, 3 })
			assertEquals(expectedEpsilon, outEpsilon)
		End Sub
	End Class

End Namespace