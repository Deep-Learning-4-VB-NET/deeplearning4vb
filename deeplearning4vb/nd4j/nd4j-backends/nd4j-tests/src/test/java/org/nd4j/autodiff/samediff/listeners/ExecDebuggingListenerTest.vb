Imports System
Imports Microsoft.VisualBasic
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports ExecDebuggingListener = org.nd4j.autodiff.listeners.debugging.ExecDebuggingListener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam

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

Namespace org.nd4j.autodiff.samediff.listeners

	Public Class ExecDebuggingListenerTest
		Inherits BaseNd4jTestWithBackends


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExecDebugListener(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExecDebugListener(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 3)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, 1, 2)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 3, 2))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 1, 2))
			Dim sm As SDVariable = sd.nn_Conflict.softmax("softmax", [in].mmul(w).add(b))
			Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, sm)

			Dim i As INDArray = Nd4j.rand(DataType.FLOAT, 1, 3)
			Dim l As INDArray = Nd4j.rand(DataType.FLOAT, 1, 2)

			sd.TrainingConfig = TrainingConfig.builder().dataSetFeatureMapping("in").dataSetLabelMapping("label").updater(New Adam(0.001)).build()

			For Each pm As ExecDebuggingListener.PrintMode In System.Enum.GetValues(GetType(ExecDebuggingListener.PrintMode))
				sd.setListeners(New ExecDebuggingListener(pm, -1, True))
	'            sd.output(m, "softmax");
				sd.fit(New DataSet(i, l))

				Console.WriteLine(vbLf & vbLf & vbLf)
			Next pm

		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace