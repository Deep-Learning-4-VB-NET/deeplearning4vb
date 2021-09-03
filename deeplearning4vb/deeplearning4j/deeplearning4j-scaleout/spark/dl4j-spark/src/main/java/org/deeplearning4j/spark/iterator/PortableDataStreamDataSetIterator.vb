Imports System
Imports System.Collections.Generic
Imports System.IO
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException

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

Namespace org.deeplearning4j.spark.iterator


	<Serializable>
	Public Class PortableDataStreamDataSetIterator
		Inherits BaseDataSetIterator(Of PortableDataStream)

		Public Sub New(ByVal iter As IEnumerator(Of PortableDataStream))
			Me.dataSetStreams = Nothing
			Me.iter = iter
		End Sub

		Public Sub New(ByVal dataSetStreams As ICollection(Of PortableDataStream))
			Me.dataSetStreams = dataSetStreams
			iter = dataSetStreams.GetEnumerator()
		End Sub

		Public Overrides Function [next]() As DataSet
			Dim ds As DataSet
			If preloadedDataSet IsNot Nothing Then
				ds = preloadedDataSet
				preloadedDataSet = Nothing
			Else
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ds = load(iter.next())
			End If

			If ds.Labels.size(1) > Integer.MaxValue OrElse ds.Features.size(1) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			totalOutcomes_Conflict = CInt(ds.Labels.size(1))
			inputColumns_Conflict = CInt(ds.Features.size(1))
			batch_Conflict = ds.numExamples()

			If preprocessor_Conflict IsNot Nothing Then
				preprocessor_Conflict.preProcess(ds)
			End If
			Return ds
		End Function

		Protected Friend Overridable Overloads Function load(ByVal pds As PortableDataStream) As DataSet
			Dim ds As New DataSet()
			Try
					Using [is] As Stream = pds.open()
					ds.load([is])
					End Using
			Catch e As IOException
				Throw New Exception("Error loading DataSet at path " & pds.getPath() & " - DataSet may be corrupt or invalid." & " Spark DataSets can be validated using org.deeplearning4j.spark.util.data.SparkDataValidation", e)
			End Try
			cursor += 1
			Return ds
		End Function

	End Class

End Namespace