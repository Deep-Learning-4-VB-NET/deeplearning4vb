Imports System
Imports System.Collections.Generic
Imports TFGraphRunnerService = org.nd4j.TFGraphRunnerService
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports TensorDataType = org.nd4j.tensorflow.conversion.TensorDataType

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

Namespace org.nd4j.tensorflow.conversion.graphrunner


	Public Class GraphRunnerServiceProvider
		Implements TFGraphRunnerService

'JAVA TO VB CONVERTER NOTE: The variable graphRunner was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private graphRunner_Conflict As GraphRunner
		Friend inputs As IDictionary(Of String, INDArray)

		Public Overridable Function init(ByVal inputNames As IList(Of String), ByVal outputNames As IList(Of String), ByVal graphBytes() As SByte, ByVal constants As IDictionary(Of String, INDArray), ByVal inputDataTypes As IDictionary(Of String, String)) As TFGraphRunnerService
			If inputNames.Count <> inputDataTypes.Count Then
				Throw New System.ArgumentException("inputNames.size() != inputDataTypes.size()")
			End If
			Dim convertedDataTypes As IDictionary(Of String, TensorDataType) = New Dictionary(Of String, TensorDataType)()
			For i As Integer = 0 To inputNames.Count - 1
				convertedDataTypes(inputNames(i)) = TensorDataType.fromProtoValue(inputDataTypes(inputNames(i)))
			Next i
			Dim castConstants As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each e As KeyValuePair(Of String, INDArray) In constants.SetOfKeyValuePairs()
				Dim requiredDtype As DataType = TensorDataType.toNd4jType(TensorDataType.fromProtoValue(inputDataTypes(e.Key)))
				castConstants(e.Key) = e.Value.castTo(requiredDtype)
			Next e
			Me.inputs = castConstants
			graphRunner_Conflict = GraphRunner.builder().inputNames(inputNames).outputNames(outputNames).graphBytes(graphBytes).inputDataTypes(convertedDataTypes).build()
			Return Me

		End Function

		Public Overridable Function run(ByVal inputs As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray) Implements TFGraphRunnerService.run
			If graphRunner_Conflict Is Nothing Then
				Throw New Exception("GraphRunner not initialized.")
			End If
			Me.inputs.PutAll(inputs)
			Return graphRunner_Conflict.run(Me.inputs)
		End Function
	End Class

End Namespace