Imports System
Imports System.Collections.Generic
Imports Gson = com.google.gson.Gson
Imports TypeToken = com.google.gson.reflect.TypeToken
Imports Data = lombok.Data
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasModelUtils.parseJsonString

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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessing.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class TimeSeriesGenerator
	Public Class TimeSeriesGenerator

		Private Const DEFAULT_SAMPLING_RATE As Integer = 1
		Private Const DEFAULT_STRIDE As Integer = 1
		Private Shared ReadOnly DEFAULT_START_INDEX As Integer? = 0
		Private Shared ReadOnly DEFAULT_END_INDEX As Integer? = Nothing
		Private Const DEFAULT_SHUFFLE As Boolean = False
		Private Const DEFAULT_REVERSE As Boolean = False
		Private Const DEFAULT_BATCH_SIZE As Integer = 128

		Private data As INDArray
		Private targets As INDArray
'JAVA TO VB CONVERTER NOTE: The field length was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private length_Conflict As Integer
		Private samplingRate As Integer
		Private stride As Integer
		Private startIndex As Integer
		Private endIndex As Integer
		Private shuffle As Boolean
		Private reverse As Boolean
		Private batchSize As Integer

		' TODO: add pad_sequences, make_sampling_table, skipgrams utils

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static TimeSeriesGenerator fromJson(String jsonFileName) throws IOException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function fromJson(ByVal jsonFileName As String) As TimeSeriesGenerator
			Dim json As New String(Files.readAllBytes(Paths.get(jsonFileName)))
			Dim timeSeriesBaseConfig As IDictionary(Of String, Object) = parseJsonString(json)
			Dim timeSeriesConfig As IDictionary(Of String, Object)
			If timeSeriesBaseConfig.ContainsKey("config") Then
				timeSeriesConfig = DirectCast(timeSeriesBaseConfig("config"), IDictionary(Of String, Object))
			Else
				Throw New InvalidKerasConfigurationException("No configuration found for Keras tokenizer")
			End If


			Dim length As Integer = DirectCast(timeSeriesConfig("length"), Integer)
			Dim samplingRate As Integer = DirectCast(timeSeriesConfig("sampling_rate"), Integer)
			Dim stride As Integer = DirectCast(timeSeriesConfig("stride"), Integer)
			Dim startIndex As Integer = DirectCast(timeSeriesConfig("start_index"), Integer)
			Dim endIndex As Integer = DirectCast(timeSeriesConfig("end_index"), Integer)
			Dim batchSize As Integer = DirectCast(timeSeriesConfig("batch_size"), Integer)

			Dim shuffle As Boolean = DirectCast(timeSeriesConfig("shuffle"), Boolean)
			Dim reverse As Boolean = DirectCast(timeSeriesConfig("reverse"), Boolean)


			Dim gson As New Gson()
			Dim dataList As IList(Of IList(Of Double)) = gson.fromJson(DirectCast(timeSeriesConfig("data"), String), (New TypeTokenAnonymousInnerClass()).getType())
			Dim targetsList As IList(Of IList(Of Double)) = gson.fromJson(DirectCast(timeSeriesConfig("targets"), String), (New TypeTokenAnonymousInnerClass2()).getType())

			Dim dataPoints As Integer = dataList.Count
			Dim dataPointsPerRow As Integer = dataList(0).Count


			Dim data As INDArray = Nd4j.create(dataPoints, dataPointsPerRow)
			Dim targets As INDArray = Nd4j.create(dataPoints, dataPointsPerRow)
			For i As Integer = 0 To dataPoints - 1
				data.put(i, Nd4j.create(dataList(i)))
				targets.put(i, Nd4j.create(targetsList(i)))
			Next i


			Dim gen As New TimeSeriesGenerator(data, targets, length, samplingRate, stride, startIndex, endIndex, shuffle, reverse, batchSize)

			Return gen
		End Function

		Private Class TypeTokenAnonymousInnerClass
			Inherits TypeToken(Of IList(Of IList(Of Double)))

		End Class

		Private Class TypeTokenAnonymousInnerClass2
			Inherits TypeToken(Of IList(Of IList(Of Double)))

		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public TimeSeriesGenerator(org.nd4j.linalg.api.ndarray.INDArray data, org.nd4j.linalg.api.ndarray.INDArray targets, int length, int samplingRate, int stride, System.Nullable<Integer> startIndex, System.Nullable<Integer> endIndex, boolean shuffle, boolean reverse, int batchSize) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal data As INDArray, ByVal targets As INDArray, ByVal length As Integer, ByVal samplingRate As Integer, ByVal stride As Integer, ByVal startIndex As Integer?, ByVal endIndex As Integer?, ByVal shuffle As Boolean, ByVal reverse As Boolean, ByVal batchSize As Integer)

			Me.data = data
			Me.targets = targets
			Me.length_Conflict = length
			Me.samplingRate = samplingRate
			If stride <> 1 Then
				Throw New InvalidKerasConfigurationException("currently no strides > 1 supported, got: " & stride)
			End If
			Me.stride = stride
			Me.startIndex = startIndex.Value + length
			If endIndex Is Nothing Then
				endIndex = data.rows() -1
			End If
			Me.endIndex = endIndex
			Me.shuffle = shuffle
			Me.reverse = reverse
			Me.batchSize = batchSize

			If Me.startIndex > Me.endIndex Then
				Throw New System.ArgumentException("Start index of sequence has to be smaller then end index, got " & "startIndex : " & Me.startIndex & " and endIndex: " & Me.endIndex)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public TimeSeriesGenerator(org.nd4j.linalg.api.ndarray.INDArray data, org.nd4j.linalg.api.ndarray.INDArray targets, int length) throws org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Sub New(ByVal data As INDArray, ByVal targets As INDArray, ByVal length As Integer)
			Me.New(data, targets, length, DEFAULT_SAMPLING_RATE, DEFAULT_STRIDE, DEFAULT_START_INDEX, DEFAULT_END_INDEX, DEFAULT_SHUFFLE, DEFAULT_REVERSE, DEFAULT_BATCH_SIZE)
		End Sub

		Public Overridable Function length() As Integer
			Return (endIndex - startIndex + batchSize * stride) \ (batchSize * stride)
		End Function

		Public Overridable Function [next](ByVal index As Integer) As Pair(Of INDArray, INDArray)
			Dim rows As INDArray
			If shuffle Then
				rows = Nd4j.Random.nextInt(endIndex, New Integer() {batchSize})
				rows.addi(startIndex)
			Else
				Dim i As Integer = startIndex + batchSize + stride * index
				' TODO: add stride arg to arange
				rows = Nd4j.arange(i, Math.Min(i + batchSize * stride, endIndex + 1))
			End If
			Dim samples As INDArray = Nd4j.create(rows.length(), length_Conflict \ samplingRate, data.columns())
			Dim targets As INDArray = Nd4j.create(rows.length(), Me.targets.columns())

			Dim j As Integer = 0
			Do While j < rows.rows()
				Dim idx As Long = CLng(Math.Truncate(rows.getDouble(j)))
				Dim indices As INDArrayIndex = NDArrayIndex.interval(idx - Me.length_Conflict, Me.samplingRate, idx)
				samples.putSlice(j, Me.data.get(indices))
				Dim point As INDArrayIndex = NDArrayIndex.point(CLng(Math.Truncate(rows.getDouble(j))))
				targets.putSlice(j, Me.targets.get(point))
				j += 1
			Loop
			If reverse Then
				samples = Nd4j.reverse(samples)
			End If

			Return New Pair(Of INDArray, INDArray)(samples, targets)
		End Function
	End Class


End Namespace