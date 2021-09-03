Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports WritableConverter = org.datavec.api.io.WritableConverter
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports StringSplit = org.datavec.api.split.StringSplit
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil

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

Namespace org.deeplearning4j.spark.datavec


	Public Class RecordReaderFunction
		Implements [Function](Of String, DataSet)

		Private recordReader As RecordReader
		Private labelIndex As Integer = -1
		Private numPossibleLabels As Integer = -1
		Private converter As WritableConverter

		Public Sub New(ByVal recordReader As RecordReader, ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer, ByVal converter As WritableConverter)
			Me.recordReader = recordReader
			Me.labelIndex = labelIndex
			Me.numPossibleLabels = numPossibleLabels
			Me.converter = converter

		End Sub

		Public Sub New(ByVal recordReader As RecordReader, ByVal labelIndex As Integer, ByVal numPossibleLabels As Integer)
			Me.New(recordReader, labelIndex, numPossibleLabels, Nothing)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet call(String v1) throws Exception
		Public Overrides Function [call](ByVal v1 As String) As DataSet
			recordReader.initialize(New org.datavec.api.Split.StringSplit(v1))
			Dim dataSets As IList(Of DataSet) = New List(Of DataSet)()
			Dim currList As IList(Of Writable) = recordReader.next()

			Dim label As INDArray = Nothing
			Dim featureVector As INDArray = Nd4j.create(1,If(labelIndex >= 0, currList.Count - 1, currList.Count))
			Dim count As Integer = 0
			For j As Integer = 0 To currList.Count - 1
				If labelIndex >= 0 AndAlso j = labelIndex Then
					If numPossibleLabels < 1 Then
						Throw New System.InvalidOperationException("Number of possible labels invalid, must be >= 1")
					End If
					Dim current As Writable = currList(j)
					If converter IsNot Nothing Then
						current = converter.convert(current)
					End If
					label = FeatureUtil.toOutcomeVector(current.toInt(), numPossibleLabels)
				Else
					Dim current As Writable = currList(j)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: featureVector.putScalar(count++, current.toDouble());
					featureVector.putScalar(count, current.toDouble())
						count += 1
				End If
			Next j

			dataSets.Add(New DataSet(featureVector,If(labelIndex >= 0, label, featureVector)))



			Dim inputs As IList(Of INDArray) = New List(Of INDArray)()
			Dim labels As IList(Of INDArray) = New List(Of INDArray)()
			For Each data As DataSet In dataSets
				inputs.Add(data.Features)
				labels.Add(data.Labels)
			Next data


			Dim ret As New DataSet(Nd4j.vstack(CType(inputs, List(Of INDArray)).ToArray()), Nd4j.vstack(CType(labels, List(Of INDArray)).ToArray()))
			Return ret
		End Function
	End Class

End Namespace