Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports GravesLSTM = org.deeplearning4j.nn.conf.layers.GravesLSTM
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.layers.recurrent

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestRecurrentWeightInit extends org.deeplearning4j.BaseDL4JTest
	Public Class TestRecurrentWeightInit
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRWInit()
		Public Overridable Sub testRWInit()

			For Each rwInit As Boolean In New Boolean(){False, True}
				For i As Integer = 0 To 2

					Dim b As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).weightInit(New UniformDistribution(0, 1)).list()

					If rwInit Then
						Select Case i
							Case 0
								b.layer((New LSTM.Builder()).nIn(10).nOut(10).weightInitRecurrent(New UniformDistribution(2, 3)).build())
							Case 1
								b.layer((New GravesLSTM.Builder()).nIn(10).nOut(10).weightInitRecurrent(New UniformDistribution(2, 3)).build())
							Case 2
								b.layer((New SimpleRnn.Builder()).nIn(10).nOut(10).weightInitRecurrent(New UniformDistribution(2, 3)).build())
							Case Else
								Throw New Exception()
						End Select
					Else
						Select Case i
							Case 0
								b.layer((New LSTM.Builder()).nIn(10).nOut(10).build())
							Case 1
								b.layer((New GravesLSTM.Builder()).nIn(10).nOut(10).build())
							Case 2
								b.layer((New SimpleRnn.Builder()).nIn(10).nOut(10).build())
							Case Else
								Throw New Exception()
						End Select
					End If

					Dim net As New MultiLayerNetwork(b.build())
					net.init()

					Dim rw As INDArray = net.getParam("0_RW")
					Dim min As Double = rw.minNumber().doubleValue()
					Dim max As Double = rw.maxNumber().doubleValue()
					If rwInit Then
						assertTrue(min >= 2.0, min.ToString())
						assertTrue(max <= 3.0, max.ToString())
					Else
						assertTrue(min >= 0.0, min.ToString())
						assertTrue(max <= 1.0, max.ToString())
					End If
				Next i
			Next rwInit
		End Sub

	End Class

End Namespace